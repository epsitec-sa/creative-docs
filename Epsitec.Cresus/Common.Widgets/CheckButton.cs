//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	using ContentAlignment = Drawing.ContentAlignment;
	
	/// <summary>
	/// La classe CheckButton r�alise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			this.InternalState |= InternalState.AutoToggle;
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

		public override ContentAlignment		DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		public Drawing.Point					LabelOffset
		{
			get
			{
				return new Drawing.Point (CheckButton.CheckWidth, 0);
			}
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds ();
			
			if (this.TextLayout != null)
			{
				Drawing.Rectangle text_rect = this.TextLayout.StandardRectangle;
				
				text_rect.Offset (this.LabelOffset);
				text_rect.Inflate (1, 1);
				
				rect.MergeWith (text_rect);
			}
			
			return rect;
		}
		
		public override Epsitec.Common.Drawing.Size GetBestFitSize()
		{
			Drawing.Size size = this.TextLayout.SingleLineSize;
			
			size.Width  = System.Math.Ceiling (size.Width + CheckButton.CheckWidth + 3);
			size.Height = System.Math.Max (System.Math.Ceiling (size.Height), CheckButton.CheckHeight);
			
			return size;
		}
		
		
		protected override void UpdateTextLayout()
		{
			System.Diagnostics.Debug.Assert (this.TextLayout != null);
			
			Drawing.Point offset = this.LabelOffset;
			double        dx     = this.Client.Width - offset.X;
			double        dy     = this.Client.Height;
			
			this.TextLayout.Alignment  = this.Alignment;
			this.TextLayout.LayoutSize = new Drawing.Size (dx, dy);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			Drawing.Rectangle rect  = new Drawing.Rectangle (0, (this.Client.Height-CheckButton.CheckHeight)/2, CheckButton.CheckHeight, CheckButton.CheckHeight);
			WidgetState       state = this.PaintState;
			
			adorner.PaintCheck (graphics, rect, state);
			adorner.PaintGeneralTextLayout (graphics, this.LabelOffset, this.TextLayout, state, PaintTextStyle.CheckButton, this.BackColor);
		}

		
		protected static readonly double CheckHeight = 13;
		protected static readonly double CheckWidth  = 20;
	}
}
