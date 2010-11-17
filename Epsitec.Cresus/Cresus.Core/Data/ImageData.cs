//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public class ImageData
	{
		public ImageData(ImageBlobEntity image)
		{
			this.imageBytes = image.Data;
			this.mimeType   = MimeTypeDictionary.ParseMimeType (image.FileMimeType);
			this.name       = image.FileName;
			this.uri        = new UriBuilder (image.FileUri);
			this.weakHash   = image.WeakHash;
			this.strongHash = image.StrongHash;
		}

		public ImageData(byte[] imageBytes)
		{
			this.imageBytes = imageBytes;
			this.mimeType   = Common.Types.MimeType.Unknown;
			this.name       = null;
			this.uri        = null;
		}


		public byte[]							ImageBytes
		{
			get
			{
				return this.imageBytes;
			}
		}

		public MimeType							MimeType
		{
			get
			{
				return this.mimeType;
			}
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public UriBuilder						Uri
		{
			get
			{
				return this.uri;
			}
		}

		public int								WeakHash
		{
			get
			{
				if (this.weakHash == null)
				{
					System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
					watch.Start ();
					this.weakHash = Checksum.ComputeAdler32 (this.imageBytes, 32*1024);
					watch.Stop ();
					System.Diagnostics.Debug.WriteLine (string.Format ("Adler32 on {0} KB took {1} us", this.imageBytes.Length / 1024, watch.ElapsedTicks/10));
				}

				return this.weakHash.Value;
			}
		}

		public string							StrongHash
		{
			get
			{
				if (this.strongHash == null)
				{
					System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
					watch.Start ();
					this.strongHash = Checksum.ComputeMd5Hash (this.imageBytes);
					watch.Stop ();
					System.Diagnostics.Debug.WriteLine (string.Format ("MD5 hash on {0} KB took {1} us", this.imageBytes.Length / 1024, watch.ElapsedTicks/10));
				}

				return strongHash;
			}
		}


		public Image GetImage()
		{
			Image cache = this.cache == null ? null : this.cache.Target;

			if (cache == null)
			{
				cache = Bitmap.FromData (this.imageBytes);

				if (cache != null)
				{
					this.cache = new Weak<Image> (cache);
				}
			}

			return cache;
		}

		
		private readonly byte[]					imageBytes;
		private readonly MimeType				mimeType;
		private readonly string					name;
		private readonly UriBuilder				uri;

		private int?							weakHash;
		private string							strongHash;

		private Weak<Image>						cache;
	}
}
