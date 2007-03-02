using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ProgressIndicator implémente la barre d'avance.
	/// </summary>
	public class ProgressIndicator : Widget
	{
		public ProgressIndicator() : base ()
		{
		}

		public ProgressIndicator(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}


		public double Value
		{
			//	Valeur de la progression (0..1).
			get
			{
				return this.progressValue;
			}
			set
			{
				if (this.progressValue != value)
				{
					this.progressValue = value;
					this.Invalidate();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState state = this.PaintState;
			Rectangle box = this.Client.Bounds;

			graphics.AddRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(0));

			box.Left = box.Left + box.Width*this.progressValue - 5;
			box.Width = 10;
			graphics.AddFilledRectangle(box);
			graphics.RenderSolid(Color.FromBrightness(0));
		}


		protected double							progressValue;
	}
}
