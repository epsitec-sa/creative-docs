//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
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
			this.mimeType = MimeTypeDictionary.ParseMimeType (image.FileMimeType);
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

		private Weak<Image>						cache;
	}
}
