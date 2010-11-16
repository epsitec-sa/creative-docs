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

		public string PersistImage(System.IO.FileInfo file)
		{
			var imageBlob = this.CreateImageBlob (file);
			var otherBlob = FindSimilarImageBlob (imageBlob);
			
			if (otherBlob == null)
			{
				this.DataContext.SaveChanges ();
				return imageBlob.Code;
			}
			else
			{
				this.DataContext.DeleteEntity (imageBlob);
				return otherBlob.Code;
			}
		}


		private ImageBlobEntity CreateImageBlob(System.IO.FileInfo file)
		{
			var path = file.FullName;
			var data = System.IO.File.ReadAllBytes (path);
			var blob = this.DataContext.CreateEntity<ImageBlobEntity> ();

			var uriPath = System.Uri.EscapeUriString (path.Replace ('\\', '/'));
			var uriHost = System.Uri.EscapeUriString (System.Environment.MachineName.ToLowerInvariant ());
			var uriUser = System.Uri.EscapeUriString (System.Environment.UserName.ToLowerInvariant ());

			blob.CreationDate         = file.CreationTime;
			blob.LastModificationDate = file.LastWriteTime;
			blob.FileName             = file.Name;
			blob.FileUri              = string.Concat ("file://", uriUser, "@", uriHost, "/", uriPath);
			blob.FileMimeType         = MimeTypeDictionary.MimeTypeToString (MimeTypeDictionary.GetMimeTypeFromExtension (file.Extension));
			blob.Code                 = System.Guid.NewGuid ().ToString ("N");
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