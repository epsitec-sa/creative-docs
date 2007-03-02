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


		public ProgressIndicatorStyle ProgressStyle
		{
			//	Style graphique du widget.
			get
			{
				return this.progressStyle;
			}
			set
			{
				this.progressStyle = value;
			}
		}

		public double ProgressValue
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
			adorner.PaintProgressIndicator(graphics, this.Client.Bounds, this.progressStyle, this.progressValue);
		}


		protected ProgressIndicatorStyle			progressStyle;
		protected double							progressValue;
	}
}
