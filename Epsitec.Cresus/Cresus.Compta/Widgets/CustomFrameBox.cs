//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class CustomFrameBox : FrameBox
	{
		public CustomFrameBox()
		{
		}


		public bool HilitedFrame
		{
			//	Indique s'il faut mettre en évidence le cadre.
			get
			{
				return this.hilitedFrame;
			}
			set
			{
				if (this.hilitedFrame != value)
				{
					this.hilitedFrame = value;
					this.Invalidate ();
				}
			}
		}

		public Color FrameColor
		{
			//	Couleur de fond pour la mise en évidence du cadre.
			get
			{
				return this.frameColor;
			}
			set
			{
				if (this.frameColor != value)
				{
					this.frameColor = value;
					this.Invalidate ();
				}
			}
		}

		public bool EmptyLineAdorner
		{
			//	Indique si le champ fait partie d'une ligne vide.
			get
			{
				return this.emptyLineAdorner;
			}
			set
			{
				if (this.emptyLineAdorner != value)
				{
					this.emptyLineAdorner = value;
					this.Invalidate ();
				}
			}
		}

		public bool BaseTVAAdorner
		{
			//	Dessine la première partie d'une flèche de haut en bas.
			get
			{
				return this.baseTVAAdorner;
			}
			set
			{
				if (this.baseTVAAdorner != value)
				{
					this.baseTVAAdorner = value;
					this.Invalidate ();
				}
			}
		}

		public bool CodeTVAAdorner
		{
			//	Dessine la seconde partie d'une flèche de haut en bas.
			get
			{
				return this.codeTVAAdorner;
			}
			set
			{
				if (this.codeTVAAdorner != value)
				{
					this.codeTVAAdorner = value;
					this.Invalidate ();
				}
			}
		}

		public FormattedText OverlayText
		{
			get
			{
				return this.overlayText;
			}
			set
			{
				if (this.overlayText != value)
				{
					this.overlayText = value;
					this.Invalidate ();
				}
			}
		}

		public Color OverlayTextColor
		{
			get
			{
				return this.overlayTextColor;
			}
			set
			{
				if (this.overlayTextColor != value)
				{
					this.overlayTextColor = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}

			Rectangle rect = this.GetFrameRectangle ();

			if (this.hilitedFrame)
			{
				//	Met en évidence le cadre.
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.frameColor);

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromAlphaRgb (0.3, 0.0, 0.0, 0.0));
			}

			if (this.emptyLineAdorner)
			{
				//	Dessine des hachures grises translucides dans le fond.
				graphics.LineWidth = 7;

				for (double x = rect.Left-rect.Height; x < rect.Right; x+=20)
				{
					graphics.AddLine (x, rect.Bottom, x+rect.Height, rect.Top);
				}

				graphics.RenderSolid (UIBuilder.FieldEmptyLineColor);
				graphics.LineWidth = 1;
			}

			if (this.baseTVAAdorner)
			{
				//	Dessine la première partie d'une flèche de haut en bas.
				Point o = rect.BottomRight;
				double h = System.Math.Floor (rect.Height/2);

				graphics.AddLine (o.X-h*2, o.Y+h, o.X-h, o.Y+h);  // _
				graphics.AddLine (o.X-h, o.Y+h, o.X-h, o.Y);      //  |

				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.codeTVAAdorner)
			{
				//	Dessine la seconde partie d'une flèche de haut en bas.
				Point o = rect.TopRight;
				double h = System.Math.Floor (rect.Height/2);
				double a = rect.Height*0.2;

				graphics.AddLine (o.X-h, o.Y, o.X-h, o.Y-h);            //
				graphics.AddLine (o.X-h, o.Y-h, o.X-h*2, o.Y-h);        //   |
				graphics.AddLine (o.X-h*2, o.Y-h, o.X-h*2+a, o.Y-h+a);  // <- 
				graphics.AddLine (o.X-h*2, o.Y-h, o.X-h*2+a, o.Y-h-a);  // 

				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (!this.overlayText.IsNullOrEmpty ())
			{
				if (this.overlayTextLayout == null)
				{
					this.overlayTextLayout = new TextLayout ();
				}

				var r = rect;
				r.Deflate (2, 0);

				this.overlayTextLayout.FormattedText = this.overlayText;
				this.overlayTextLayout.LayoutSize = r.Size;
				this.overlayTextLayout.Paint (r.BottomLeft, graphics, rect, this.overlayTextColor, GlyphPaintStyle.Normal);
			}
		}


		private bool			hilitedFrame;
		private Color			frameColor;
		private bool			emptyLineAdorner;
		private bool			baseTVAAdorner;
		private bool			codeTVAAdorner;
		private FormattedText	overlayText;
		private Color			overlayTextColor;
		private TextLayout		overlayTextLayout;
	}
}
