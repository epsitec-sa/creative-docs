//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using ContentAlignment = Drawing.ContentAlignment;
	
	/// <summary>
	/// La classe CheckButton réalise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			this.AutoToggle = true;
		}
		
		public CheckButton(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}

		
		public override double					DefaultHeight
		{
			get
			{
				return System.Math.Ceiling (this.DefaultFontHeight + 1);
			}
		}

		static CheckButton()
		{
			Helpers.VisualPropertyMetadata metadata = new Helpers.VisualPropertyMetadata (Drawing.ContentAlignment.MiddleLeft, Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (CheckButton), metadata);
		}
		
		public Drawing.Point					LabelOffset
		{
			get
			{
				return new Drawing.Point (CheckButton.CheckWidth, 0);
			}
		}


		public override Drawing.Margins GetShapeMargins()
		{
			if ((this.TextLayout != null) &&
				(this.Text.Length > 0))
			{
				Drawing.Rectangle rect = this.TextLayout.StandardRectangle;
				Drawing.Rectangle bounds = this.Client.Bounds;

				rect.Offset (this.LabelOffset);
				rect.Inflate (1, 1);

				rect.MergeWith (bounds);

				return new Drawing.Margins (bounds.Left - rect.Left, rect.Right - bounds.Right, rect.Top - bounds.Top, bounds.Bottom - rect.Bottom);
			}
			else
			{
				return base.GetShapeMargins ();
			}
		}
		
		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size = (this.TextLayout == null) ? Drawing.Size.Empty : this.TextLayout.SingleLineSize;
			
			size.Width  = System.Math.Ceiling (size.Width + CheckButton.CheckWidth + 3);
			size.Height = System.Math.Max (System.Math.Ceiling (size.Height), CheckButton.CheckHeight);
			
			return size;
		}
		
		
		protected override void UpdateTextLayout()
		{
			System.Diagnostics.Debug.Assert (this.TextLayout != null);
			
			Drawing.Point offset = this.LabelOffset;
			double dx = this.Client.Size.Width - offset.X;
			double dy = this.Client.Size.Height;
			
			this.TextLayout.Alignment  = this.ContentAlignment;
			this.TextLayout.LayoutSize = new Drawing.Size (dx, dy);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle (0, (this.Client.Size.Height-CheckButton.CheckHeight)/2, CheckButton.CheckHeight, CheckButton.CheckHeight);
			WidgetPaintState       state = this.PaintState;
			
			adorner.PaintCheck (graphics, rect, state);
			adorner.PaintGeneralTextLayout (graphics, clipRect, this.LabelOffset, this.TextLayout, state, PaintTextStyle.CheckButton, TextDisplayMode.Default, this.BackColor);
		}

		
		protected static readonly double CheckHeight = 13;
		protected static readonly double CheckWidth  = 20;
	}
}
