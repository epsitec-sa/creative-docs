//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Internal
{
	internal class ImageDocumentInfo : IDocumentInfo
	{
		public ImageDocumentInfo(Drawing.ImageData image)
		{
			this.image = image;
		}

		#region IDocumentInfo Members

		public string GetDescription()
		{
			return "";
		}

		public Epsitec.Common.Drawing.Image GetThumbnail()
		{
			Drawing.Image image = null;

			if (this.thumbnail != null)
			{
				image = this.thumbnail.Target;
			}

			if (image == null)
			{
				image = Drawing.Bitmap.FromImage (this.image.GetThumbnail ());
			}

			if (image == null)
			{
				this.thumbnail = null;
			}
			else
			{
				this.thumbnail = new Types.Weak<Drawing.Image> (image);
			}

			return image;
		}

		#endregion
		
		Drawing.ImageData image;
		Types.Weak<Drawing.Image> thumbnail;
	}
}
