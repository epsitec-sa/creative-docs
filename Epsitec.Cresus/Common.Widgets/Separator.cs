namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Separator permet de dessiner des séparations.
	/// </summary>
	public class Separator : Widget
	{
		public Separator()
		{
		}
		
		public Separator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		public Drawing.Color Color
		{
			get { return this.color; }
			set { this.color = value; }
		}

		public double Alpha
		{
			get { return this.alpha; }
			set { this.alpha = value; }
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( !this.IsEnabled )  return;

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);

			Drawing.Color color = adorner.ColorBorder;
			if ( !this.color.IsEmpty )
			{
				color = this.color;
			}
			color.A = this.alpha;

			graphics.RenderSolid(color);
		}


		protected Drawing.Color			color = Drawing.Color.Empty;
		protected double				alpha = 1.0;
	}
}
