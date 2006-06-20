//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.StaticText))]

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
		
		public PaintTextStyle					PaintTextStyle
		{
			get
			{
				return this.paintTextStyle;
			}
			set
			{
				if (this.paintTextStyle != value)
				{
					this.paintTextStyle = value;
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
			WidgetPaintState  state = this.PaintState;
			Drawing.Point     pos   = Drawing.Point.Zero;
			
			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}
			
			if (this.TextLayout != null)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				adorner.PaintGeneralTextLayout (graphics, clipRect, pos, this.TextLayout, state, this.paintTextStyle, TextDisplayMode.Default, this.BackColor);
			}
			
			base.PaintBackgroundImplementation (graphics, clipRect);
		}


		private PaintTextStyle					paintTextStyle = PaintTextStyle.StaticText;
	}
}
