//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Printing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class XmlPrintedPagePreviewer : Widget
	{
		public XmlPrintedPagePreviewer()
		{
		}


		public Bitmap Bitmap
		{
			get
			{
				return this.bitmap;
			}
			set
			{
				if (this.bitmap != value)
				{
					this.bitmap = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (0.9));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (0));

			if (this.bitmap != null)
			{
				double dx = this.bitmap.Width;
				double dy = this.bitmap.Height;

				Rectangle bounds = new Rectangle (10, 10, dx, dy);

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (1));

				rect.Deflate (0.5);
				graphics.PaintImage (this.bitmap, bounds);

				bounds.Inflate (0.5);
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}


		private Bitmap								bitmap;
	}
}
