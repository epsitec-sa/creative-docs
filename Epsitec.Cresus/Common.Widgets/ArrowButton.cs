namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ArrowButton dessine un bouton flèche.
	/// </summary>
	public class ArrowButton : Button
	{
		public ArrowButton()
		{
			this.direction = Direction.None;
			this.InternalState &= ~InternalState.AutoFocus;
			this.InternalState &= ~InternalState.Focusable;
		}
		
		public ArrowButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Direction Direction
		{
			get
			{
				return this.direction;
			}

			set
			{
				this.direction = value;
			}
		}
		
		public override double DefaultWidth
		{
			get
			{
				return 17;
			}
		}
		
		public override double DefaultHeight
		{
			get
			{
				return 17;
			}
		}
		

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			adorner.PaintButtonBackground(graphics, rect, this.PaintState, this.direction, this.buttonStyle);
			adorner.PaintArrow(graphics, rect, this.PaintState, this.direction, PaintTextStyle.Button);
		}

		
		protected Direction				direction;
	}
}
