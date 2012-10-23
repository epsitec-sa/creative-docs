//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing.Platform;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ImageDataStore</c> is used to store images into the database.
	/// </summary>
	public sealed class ImageDataStore : CoreDataComponent
	{
		public ImageDataStore(CoreData data)
			: base (data)
		{
		}

		
		private DataContext						DataContext
		{
			get
			{
				if (this.context == null)
				{
					this.NewDataContext ();
				}

				return this.context;
			}
		}

		public override bool CanExecuteSetupPhase()
		{
			return this.Host.IsReady && this.Host.ConnectionManager.IsReady;
		}

		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();
			this.NewDataContext ();
		}

		/// <summary>
		/// Gets the image data for the specified item code. If duplicates are found, returns the
		/// most up-to-date image data found in the database.
		/// </summary>
		/// <param name="code">The item code.</param>
		/// <param name="thumbnailSize">Size of the thumbnail (or 0 to get the original image).</param>
		/// <returns>
		/// The image data from the database or <c>null</c> if it cannot be found.
		/// </returns>
		public ImageData GetImageData(string code, int thumbnailSize = 0)
		{
			var blob = this.GetImageBlob (code, thumbnailSize);

			if (blob == null)
			{
				return null;
			}

			return new ImageData (blob);
		}

		/// <summary>
		/// Checks whether the specified image in the database matches the image data; the
		/// comparison does only take into account the image contents, but neither its name,
		/// nor its URI, nor its date.
		/// </summary>
		/// <param name="code">The item code used in the database.</param>
		/// <param name="imageData">The image data.</param>
		/// <returns><c>true</c> if the database contains an up-to-date image; otherwise, <c>false</c>.</returns>
		public bool CheckEqual(string code, ImageData imageData)
		{
			var repository = new ImageBlobRepository (this.Host, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				Code = code,
				WeakHash = imageData.WeakHash,
				ThumbnailSize = 0,
			};

			var matches = repository.GetByExample (example);
			
			return matches.Where (x => x.StrongHash == imageData.StrongHash).Any ();
		}

		/// <summary>
		/// Updates the specified image entity.
		/// </summary>
		/// <param name="context">The data context used by the image entity.</param>
		/// <param name="image">The image entity.</param>
		/// <param name="file">The file to read data from.</param>
		public void UpdateImage(DataContext context, ImageEntity image, System.IO.FileInfo file)
		{
			var imageBlob = this.PersistImageBlob (file);

			if ((image.ImageBlob.IsNotNull ()) &&
				(image.ImageBlob.Code == imageBlob.Code))
			{
				//	The image data did not change. This image still points to the exact same
				//	bytes in the database.
			}
			else
			{
				image.ImageBlob = context.GetLocalEntity (imageBlob);
			}

			//	Note: there is no need to refresh the thumbnails; they will be created on the fly
			//	if the image was updated, since they no longer exist for the new IItemCode.Code.
		}

		/// <summary>
		/// Persists the specified image file into the database, if needed. If an equivalent/
		/// image is already stored in the database, no new data will be persisted and the
		/// existing image blob will be returned instead.
		/// </summary>
		/// <param name="file">The image file.</param>
		/// <returns>The image blob entity of the persisted image.</returns>
		public ImageBlobEntity PersistImageBlob(System.IO.FileInfo file)
		{
			var imageBlob = this.CreateImageBlob (file);
			return this.PersistImageBlob (imageBlob);
		}

		/// <summary>
		/// Reduce the memory consumption of the image data store. The
		/// images will have to be reloaded from the database.
		/// </summary>
		public void Trim()
		{
			this.DisposeDataContext ();
		}

		
		/// <summary>
		/// Gets the image BLOB for the specified item code. If duplicates are found, returns the
		/// most up-to-date image BLOB found in the database.
		/// </summary>
		/// <param name="code">The item code.</param>
		/// <param name="thumbnailSize">Size of the thumbnail (or 0 to get the original image).</param>
		/// <returns>
		/// The image BLOB from the database or <c>null</c> if it cannot be found.
		/// </returns>
		private ImageBlobEntity GetImageBlob(string code, int thumbnailSize = 0)
		{
			var repository = new ImageBlobRepository (this.Host, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				Code = code,
			};

			example.ThumbnailSize = -1;
			example.ThumbnailSize = thumbnailSize;

			int pass = 0;

		again:

			var match = repository.GetByExample (example).OrderByDescending (x => x.LastModificationDate).FirstOrDefault ();

			if (match == null)
			{
				if (thumbnailSize == 0)
				{
					return null;
				}

				var blob = this.GetImageBlob (code);

				if (blob == null)
				{
					return null;
				}

				this.UpdateThumbnails (blob, thumbnailSize);

				if (pass++ == 0)
				{
					goto again;
				}
			}

			return match;
		}

		private ImageBlobEntity PersistImageBlob(ImageBlobEntity imageBlob)
		{
			var otherBlob = this.FindSimilarImageBlob (imageBlob);

			if (otherBlob == null)
			{
				this.DataContext.SaveChanges ();
				return imageBlob;
			}
			else
			{
				this.DataContext.DeleteEntity (imageBlob);
				return otherBlob;
			}
		}

		private void UpdateThumbnails(ImageBlobEntity imageBlob, params int[] thumbnailSizes)
		{
			var nativeImage = NativeBitmap.Load (imageBlob.Data);
			
			var repository = new ImageBlobRepository (this.Host, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				Code = imageBlob.Code,
			};

			var thumbnails = repository.GetByExample (example).ToList ();
			var sizes      = new HashSet<int> (thumbnailSizes.Concat (thumbnails.Select (x => x.ThumbnailSize)));

			foreach (int size in sizes)
			{
				if (size == 0)
				{
					continue;
				}

				var thumbnailImage = nativeImage == null ? null : nativeImage.MakeThumbnail (size);
				var thumbnailBlob  = this.CreateThumbnailImageBlob (imageBlob, thumbnailImage, size);

				this.PersistImageBlob (thumbnailBlob);
			}
		}

		private ImageBlobEntity CreateImageBlob(System.IO.FileInfo file)
		{
			var path = file.FullName;
			var data = System.IO.File.ReadAllBytes (path);
			var blob = this.DataContext.CreateEntity<ImageBlobEntity> ();

			var uri  = new Epsitec.Common.IO.UriBuilder ()
			{
				Scheme   = System.Uri.UriSchemeFile,
				Path     = path,
				Host     = System.Environment.MachineName.ToLowerInvariant (),
				UserName = System.Environment.UserName.ToLowerInvariant ()
			};

			blob.CreationDate         = file.CreationTime;
			blob.LastModificationDate = file.LastWriteTime;
			blob.FileName             = file.Name;
			blob.FileUri              = uri.ToString ();
			blob.FileMimeType         = MimeTypeDictionary.MimeTypeToString (MimeTypeDictionary.GetMimeTypeFromExtension (file.Extension));
			blob.Code                 = (string) ItemCodeGenerator.NewCode ();
			blob.Data                 = data;

			using (var bitmap = NativeBitmap.Load (data))
			{
				ImageDataStore.FillBlobMetadata (bitmap, blob);
			}

			return blob;
		}

		private ImageBlobEntity CreateThumbnailImageBlob(ImageBlobEntity fullSizeImageBlob, NativeBitmap bitmap, int thumbnailSize)
		{
			byte[] data = bitmap == null ? null : bitmap.SaveToMemory (BitmapFileType.Png);
		
			var blob = this.DataContext.CreateEntity<ImageBlobEntity> ();

			blob.CreationDate         = System.DateTime.UtcNow;
			blob.LastModificationDate = blob.CreationDate;
			blob.FileName             = null;
			blob.FileUri              = null;
			blob.FileMimeType         = MimeTypeDictionary.MimeTypeToString (MimeType.ImagePng);
			blob.Code                 = fullSizeImageBlob.Code;
			blob.Data                 = data;

			ImageDataStore.FillBlobMetadata (bitmap, blob, thumbnailSize);

			return blob;
		}

		private static void FillBlobMetadata(NativeBitmap bitmap, ImageBlobEntity blob, int thumbnailSize = 0)
		{
			if (bitmap != null)
			{
				blob.PixelWidth    = bitmap.Width;
				blob.PixelHeight   = bitmap.Height;
				blob.ThumbnailSize = thumbnailSize;
				blob.BitsPerPixel  = bitmap.BitsPerPixel;
				blob.Dpi           = ((decimal) System.Math.Round (1000 * (bitmap.DpiX + bitmap.DpiY) / 2)) / 1000M;

				blob.SetHashes (blob.Data);
			}
		}

		private ImageBlobEntity FindSimilarImageBlob(ImageBlobEntity imageBlob)
		{
			var repository = new ImageBlobRepository (this.Host, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				WeakHash = imageBlob.WeakHash,
				StrongHash = imageBlob.StrongHash,
			};

			return repository.GetByExample (example).FirstOrDefault ();
		}
		
		private void NewDataContext()
		{
			this.DisposeDataContext ();

			int contextId   = ImageDataStore.GetNextContextId ();
			var contextName = string.Format (System.Globalization.CultureInfo.InvariantCulture, "ImageDataStore #{0}", contextId);
			
			this.context = this.Host.CreateDataContext (contextName);
		}

		private void DisposeDataContext()
		{
			if (this.context != null)
			{
				this.Host.DisposeDataContext (this.context);
				this.context = null;
			}
		}

		private static int GetNextContextId()
		{
			return System.Threading.Interlocked.Increment (ref ImageDataStore.nextContextId);
		}

		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			public bool CanCreate(CoreData data)
			{
				return data.IsReady;
			}

			public bool ShouldCreate(CoreData host)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new ImageDataStore (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (ImageDataStore);
			}
		}

		#endregion
		
		private static int						nextContextId;

		private DataContext						context;
	}
}