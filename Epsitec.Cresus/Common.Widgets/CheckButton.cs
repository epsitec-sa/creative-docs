namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CheckButton r�alise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			this.InternalState |= InternalState.AutoToggle;
		}
		
		public CheckButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight+1;
			}
		}

		protected override void UpdateTextLayout()
		{
			if ( this.TextLayout != null )
			{
				Drawing.Point offset = this.LabelOffset;
				double dx = this.Client.Width - offset.X;
				double dy = this.Client.Height;
				this.TextLayout.Alignment = this.Alignment;
				this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		// Retourne l'alignement par d�faut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds();
			
			if ( this.TextLayout != null )
			{
				Drawing.Rectangle text = this.TextLayout.StandardRectangle;
				text.Offset(this.LabelOffset);
				text.Inflate(1, 1);
				rect.MergeWith(text);
			}
			
			return rect;
		}


		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = 0;
			rect.Right  = CheckButton.checkHeight;
			rect.Bottom = (this.Client.Height-CheckButton.checkHeight)/2;
			rect.Top    = rect.Bottom+CheckButton.checkHeight;
			adorner.PaintCheck(graphics, rect, this.PaintState);

			adorner.PaintGeneralTextLayout(graphics, this.LabelOffset, this.TextLayout, this.PaintState, PaintTextStyle.CheckButton, this.BackColor);
		}

		protected Drawing.Point LabelOffset
		{
			get
			{
				return new Drawing.Point(CheckButton.checkWidth, 0);
			}
		}

		protected static readonly double checkHeight = 13;
		protected static readonly double checkWidth = 20;
	}
}
