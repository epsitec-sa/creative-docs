
[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Separator))]

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
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}

		public double Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				if (this.alpha != value)
				{
					this.alpha = value;
					this.Invalidate();
				}
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( !this.IsEnabled || this.alpha == 0 )  return;

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
