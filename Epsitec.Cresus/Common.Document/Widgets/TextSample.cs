using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextSample est un widget affichant un échantillon d'une propriété de texte.
	/// </summary>
	public class TextSample : AbstractSample
	{
		public TextSample() : base()
		{
		}

		public TextSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Text.TextStyle TextStyle
		{
			//	Style représenté.
			get
			{
				return this.textStyle;
			}

			set
			{
				this.textStyle = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'échantillon.
			if ( this.document == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ( this.textStyle == null )  // dessine une croix ?
			{
				rect.Deflate(0.5);
				Color color = adorner.ColorBorder;
				color.A = 0.3;

				graphics.AddLine(rect.BottomLeft, rect.TopRight);
				graphics.RenderSolid(color);

				graphics.AddLine(rect.TopLeft, rect.BottomRight);
				graphics.RenderSolid(color);
			}
			else
			{
			}
		}


		protected Text.TextStyle			textStyle;
	}
}
