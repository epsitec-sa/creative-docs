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
			this.titleLayout = new TextLayout
			{
				DefaultFontSize = 10,
				Alignment = ContentAlignment.MiddleCenter,
				BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
			};
		}


		public Printers.DeserializedPage Page
		{
			get
			{
				return this.page;
			}
			set
			{
				if (this.page != value)
				{
					this.page = value;

					this.UpdateTitle ();
					this.bitmap = null;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.UpdateBitmap ();

			if (this.bitmap == null)
			{
				return;
			}

			double dx = this.bitmap.Width;
			double dy = this.bitmap.Height;

			var rectTitle   = new Rectangle (0, 0, dx, XmlPrintedPagePreviewer.titleHeight+2);
			var rectPreview = new Rectangle (0, XmlPrintedPagePreviewer.titleHeight, dx, dy);

			if (this.titleLayout != null)
			{
				rectTitle.Deflate (0.5);
				graphics.AddRectangle (rectTitle);
				graphics.RenderSolid (Color.FromBrightness (0));

				this.titleLayout.LayoutSize = rectTitle.Size;
				this.titleLayout.Paint (rectTitle.BottomLeft, graphics, Rectangle.MaxValue, Color.FromBrightness (0), GlyphPaintStyle.Normal);
			}

			if (this.bitmap != null)
			{
				rectPreview.Deflate (1);
				graphics.PaintImage (this.bitmap, rectPreview);

				rectPreview.Inflate (0.5);
				graphics.AddRectangle (rectPreview);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		private void UpdateTitle()
		{
			this.titleLayout.Text = this.page.ShortDescription;

			ToolTip.Default.SetToolTip (this, this.page.FullDescription);
		}

		private void UpdateBitmap()
		{
			double widgetWidth  = this.Client.Bounds.Width;
			double widgetHeight = this.Client.Bounds.Height - XmlPrintedPagePreviewer.titleHeight;

			double pageWidth  = this.page.ParentSection.PageSize.Width;
			double pageHeight = this.page.ParentSection.PageSize.Height;

			double zoomX = widgetWidth  / pageWidth;
			double zoomY = widgetHeight / pageHeight;

			double zoom = System.Math.Min (zoomX, zoomY);

			if (this.bitmap == null || this.lastZoom != zoom)
			{
				this.lastZoom = zoom;

				var port = new XmlPort (page.XRoot);
				this.bitmap = port.Deserialize (new Size (pageWidth, pageHeight), zoom);
			}
		}


		private static readonly double		titleHeight = 18;

		private Printers.DeserializedPage	page;
		private Bitmap						bitmap;
		private double						lastZoom;
		private TextLayout					titleLayout;
	}
}
