//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StaticText représente du texte non éditable. Ce texte
	/// peut comprendre des images et des liens hyper-texte.
	/// </summary>
	public class StaticText : Widget
	{
		public StaticText()
		{
			this.TextBreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
		}
		
		public StaticText(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public StaticText(string text) : this ()
		{
			this.Text = text;
		}
		
		public StaticText(Widget embedder, string text) : this (embedder)
		{
			this.Text = text;
		}
		
		
		public override double						DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight;
			}
		}

		public override Drawing.ContentAlignment	DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		public override Drawing.Size				PreferredSize
		{
			get
			{
				return this.MapClientToParent (this.TextLayout.SingleLineSize);
			}
		}
		
		public override Drawing.Point				BaseLine
		{
			get
			{
				if (this.TextLayout != null)
				{
					return this.MapClientToParent (this.TextLayout.GetLineOrigin (0)) - this.Location;
				}
				
				return base.BaseLine;
			}
		}
		
		public PaintTextStyle						PaintTextStyle
		{
			get
			{
				return this.paint_text_style;
			}
			set
			{
				if (this.paint_text_style != value)
				{
					this.paint_text_style = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size = this.TextLayout.SingleLineSize;
			
			size.Width  = System.Math.Ceiling (size.Width);
			size.Height = System.Math.Ceiling (size.Height);
			
			return size;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			if (this.TextLayout == null)
			{
				return;
			}
			
			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();
			
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}
			
			adorner.PaintGeneralTextLayout (graphics, pos, this.TextLayout, state, this.paint_text_style, this.BackColor);
		}


		protected PaintTextStyle					paint_text_style = PaintTextStyle.StaticText;
	}
	
	public class StaticTextSmall : StaticText
	{
		public StaticTextSmall()
		{
			this.Client.SetZoom (0.9);
		}
		
		public StaticTextSmall(Widget embedder) : this ()
		{
			this.SetEmbedder (this);
		}
		
		public StaticTextSmall(string text) : this ()
		{
			this.Text = text;
		}
		
		public StaticTextSmall(Widget embedder, string text) : this (embedder)
		{
			this.Text = text;
		}
	}
	
	public class StaticTextLarge : StaticText
	{
		public StaticTextLarge()
		{
			this.Client.SetZoom (1.25);
		}
		
		public StaticTextLarge(Widget embedder) : this ()
		{
			this.SetEmbedder (this);
		}
		
		public StaticTextLarge(string text) : this ()
		{
			this.Text = text;
		}
		
		public StaticTextLarge(Widget embedder, string text) : this (embedder)
		{
			this.Text = text;
		}
	}
}
