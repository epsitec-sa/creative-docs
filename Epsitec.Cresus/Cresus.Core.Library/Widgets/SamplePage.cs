//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class SamplePage : Widget
	{
		public enum PageTypeEnum
		{
			WithoutIsr,
			WithIsr,
			SingleIsr,
		}



		public SamplePage()
		{
		}

		public SamplePage(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public PageTypeEnum PageType
		{
			get
			{
				return this.pageType;
			}
			set
			{
				if (this.pageType != value)
				{
					this.pageType = value;
					this.Invalidate ();
				}
			}
		}

		public string PageText
		{
			get
			{
				return this.pageText;
			}
			set
			{
				if (this.pageText != value)
				{
					this.pageText = value;
					this.Invalidate ();
				}
			}
		}

		public string PageBottomLabel
		{
			get
			{
				return this.pageBottomLabel;
			}
			set
			{
				if (this.pageBottomLabel != value)
				{
					this.pageBottomLabel = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var b = this.Client.Bounds;
			var pageRect  = new Rectangle (b.Left, b.Bottom+15, b.Width, b.Height-15);
			var bvRect    = new Rectangle (pageRect.Left, pageRect.Bottom, pageRect.Width, System.Math.Floor (pageRect.Height*(105.0/297.0)));
			var labelRect = new Rectangle (b.Left, b.Bottom, b.Width, 15);

			pageRect.Deflate (0.5);
			bvRect.Deflate (0.5);

			switch (this.pageType)
			{
				case PageTypeEnum.SingleIsr:
					graphics.AddFilledRectangle (bvRect);
					graphics.RenderSolid (Color.FromHexa ("ffe2d9"));
					pageRect = bvRect;
					break;

				case PageTypeEnum.WithIsr:
					graphics.AddFilledRectangle (pageRect);
					graphics.RenderSolid (Color.FromBrightness (1));

					graphics.AddFilledRectangle (bvRect);
					graphics.RenderSolid (Color.FromHexa ("ffe2d9"));
					break;

				default:
					graphics.AddFilledRectangle (pageRect);
					graphics.RenderSolid (Color.FromBrightness (1));
					break;
			}

			graphics.AddRectangle (pageRect);
			graphics.RenderSolid (Color.FromBrightness (0));

			if (!string.IsNullOrEmpty (this.pageBottomLabel))
			{
				pageRect.Deflate (1);
				graphics.AddRectangle (pageRect);
				graphics.RenderSolid (Color.FromBrightness (0));
				pageRect.Inflate (1);
			}

			graphics.PaintText (pageRect,  this.pageText,        SamplePage.regularFont, 12, ContentAlignment.MiddleCenter);
			graphics.PaintText (labelRect, this.pageBottomLabel, SamplePage.boldFont,    14, ContentAlignment.MiddleCenter);
		}


		private static readonly Font regularFont = Font.GetFont (Font.DefaultFontFamily, "Regular");
		private static readonly Font boldFont    = Font.GetFont (Font.DefaultFontFamily, "Bold");

		private PageTypeEnum					pageType;
		private string							pageText;
		private string							pageBottomLabel;
	}
}
