namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CheckButton réalise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			this.internal_state |= InternalState.AutoToggle;
		}


		protected override void UpdateLayoutSize()
		{
			if ( this.text_layout != null )
			{
				double dx = this.Client.Width - this.Client.Height*CheckButton.checkWidth;
				double dy = this.Client.Height;
				this.text_layout.Alignment = this.Alignment;
				this.text_layout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}

		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}

		public override Drawing.Rectangle GetPaintBounds()
		{
			Drawing.Rectangle rect = base.GetPaintBounds ();
			
			if (this.text_layout != null)
			{
				Drawing.Rectangle text = this.text_layout.StandardRectangle;
				text.Offset (this.LabelOffset);
				text.Inflate (1, 1);
				rect.MergeWith (text);
			}
			
			return rect;
		}


		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Height, this.Client.Height);
			adorner.PaintCheck(graphics, rect, this.PaintState, this.RootDirection);
			adorner.PaintGeneralTextLayout(graphics, this.LabelOffset, this.text_layout, this.PaintState, this.RootDirection);
		}

		protected Drawing.Point				LabelOffset
		{
			get
			{
				return new Drawing.Point(this.Client.Height*CheckButton.checkWidth, 0);
			}
		}

		protected static readonly double checkWidth = 1.5;
	}
}
