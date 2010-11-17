//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ImageDataStore</c> is used to store images into the database.
	/// </summary>
	public sealed class ImageDataStore
	{
		public ImageDataStore(CoreData data)
		{
			this.data = data;
			this.NewDataContext ();
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


		/// <summary>
		/// Gets the image data for the specified item code. If duplicates are found, returns the
		/// most up-to-date image data found in the database.
		/// </summary>
		/// <param name="code">The item code.</param>
		/// <returns>The image data from the database or <c>null</c> if it cannot be found.</returns>
		public ImageData GetImageData(string code)
		{
			var repository = new ImageBlobRepository (this.data, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				Code = code
			};

			var allImages = repository.GetByExample (example).OrderByDescending (x => x.LastModificationDate).ToList ();
			var liveImage = allImages.Where (x => x.IsArchive == false).FirstOrDefault ();

			if (liveImage != null)
            {
				return new ImageData (liveImage);
            }
			if (allImages.Count > 0)
			{
				return new ImageData (allImages[0]);
			}

			return null;
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
			var repository = new ImageBlobRepository (this.data, this.DataContext);
			var example    = new ImageBlobEntity ()
			{
				Code = code,
				WeakHash = imageData.WeakHash
			};

			var matches = repository.GetByExample (example);
			
			return matches.Where (x => x.StrongHash == imageData.StrongHash).Any ();

		}

		/// <summary>
		/// Persists the specified image file into the database, if needed. If an equivalent
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
		
		public void UpdateImage(DataContext context, ImageEntity image, System.IO.FileInfo file)
		{
			var imageBlob = this.PersistImageBlob (file);

			if ((image.ImageBlob.IsNotNull ()) &&
				(image.ImageBlob.Code == imageBlob.Code))
			{
				//	The image data did not change. This image still points to the exact same
				//	bytes in the database and therefore, we need not update the thumbnail.
			}
			else
			{
				var nativeImage = Epsitec.Common.Drawing.Platform.NativeBitmap.Load (imageBlob.Data);
				
				var thumbnailImage = nativeImage.MakeThumbnail (512);
				var thumbnailBlob  = this.CreateImageBlob (thumbnailImage.SaveToMemory (Epsitec.Common.Drawing.Platform.BitmapFileType.Png), MimeType.ImagePng);

				image.ImageBlob     = context.GetLocalEntity (imageBlob);
				image.ThumbnailBlob = context.GetLocalEntity (this.PersistImageBlob (thumbnailBlob));
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
			blob.Code                 = Business.ItemCodeGenerator.NewCode ();
			blob.Data                 = data;

			blob.SetHashes (data);

			return blob;
		}

		private ImageBlobEntity CreateImageBlob(byte[] data, MimeType mimeType)
		{
			var blob = this.DataContext.CreateEntity<ImageBlobEntity> ();

			blob.CreationDate         = System.DateTime.Now;
			blob.LastModificationDate = blob.CreationDate;
			blob.FileName             = null;
			blob.FileUri              = null;
			blob.FileMimeType         = MimeTypeDictionary.MimeTypeToString (mimeType);
			blob.Code                 = Business.ItemCodeGenerator.NewCode ();
			blob.Data                 = data;

			blob.SetHashes (data);

			return blob;
		}

		private ImageBlobEntity FindSimilarImageBlob(ImageBlobEntity imageBlob)
		{
			var repository = new ImageBlobRepository (this.data, this.DataContext);
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
			
			this.context = this.data.CreateDataContext (contextName);
		}

		private void DisposeDataContext()
		{
			if (this.context != null)
			{
				this.data.DisposeDataContext (this.context);
				this.context = null;
			}
		}


		private static int GetNextContextId()
		{
			return System.Threading.Interlocked.Increment (ref ImageDataStore.nextContextId);
		}

		private static int nextContextId;

		private readonly CoreData data;
		private DataContext context;
	}
}