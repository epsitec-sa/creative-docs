namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CheckButton réalise un bouton cochable.
	/// </summary>
	public class CheckButton : AbstractButton
	{
		public CheckButton()
		{
			//? this.internal_state |= InternalState.AutoToggle;
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

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Height, this.Client.Height);
			adorner.PaintCheck(graphics, rect, this.PaintState, this.RootDirection);

			Drawing.Point origine = new Drawing.Point(this.Client.Height*CheckButton.checkWidth, 0);
			adorner.PaintGeneralTextLayout(graphics, origine, this.text_layout, this.PaintState, this.RootDirection);
		}


		protected static readonly double checkWidth = 1.5;
	}
}
