//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe StaticText représente du texte non éditable. Ce texte
	/// peut comprendre des images et des liens hypertexte.
	/// </summary>
	public class StaticText : Widget
	{
		public StaticText()
		{
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
		
		
		static StaticText()
		{
			Helpers.VisualPropertyMetadata metadataAlign = new Helpers.VisualPropertyMetadata (Drawing.ContentAlignment.MiddleLeft, Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata (Widget.DefaultFontHeight, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			
			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (StaticText), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (StaticText), metadataHeight);
		}
		
#if false	//#fix
		public override Drawing.Size				PreferredSize
		{
			get
			{
				return this.MapClientToParent (this.TextLayout.SingleLineSize);
			}
		}
#endif
		
		public override Drawing.Point GetBaseLine()
		{
			if (this.TextLayout != null)
			{
				return this.MapClientToParent (this.TextLayout.GetLineOrigin (0)) - this.ActualLocation;
			}
			
			return base.GetBaseLine ();
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
			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();
			
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}
			
			if (this.TextLayout != null)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				adorner.PaintGeneralTextLayout (graphics, clipRect, pos, this.TextLayout, state, this.paint_text_style, TextDisplayMode.Default, this.BackColor);
			}
			
			base.PaintBackgroundImplementation (graphics, clipRect);
		}


		protected PaintTextStyle					paint_text_style = PaintTextStyle.StaticText;
	}
	
//	public class StaticTextSmall : StaticText
//	{
//		public StaticTextSmall()
//		{
//			this.Client.SetZoom (0.9);
//		}
//		
//		public StaticTextSmall(Widget embedder) : this ()
//		{
//			this.SetEmbedder (this);
//		}
//		
//		public StaticTextSmall(string text) : this ()
//		{
//			this.Text = text;
//		}
//		
//		public StaticTextSmall(Widget embedder, string text) : this (embedder)
//		{
//			this.Text = text;
//		}
//	}
//	
//	public class StaticTextLarge : StaticText
//	{
//		public StaticTextLarge()
//		{
//			this.Client.SetZoom (1.25);
//		}
//		
//		public StaticTextLarge(Widget embedder) : this ()
//		{
//			this.SetEmbedder (this);
//		}
//		
//		public StaticTextLarge(string text) : this ()
//		{
//			this.Text = text;
//		}
//		
//		public StaticTextLarge(Widget embedder, string text) : this (embedder)
//		{
//			this.Text = text;
//		}
//	}
}
