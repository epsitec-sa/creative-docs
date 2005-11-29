using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextStyleSample est un widget affichant un échantillon d'une propriété
	/// de style de texte.
	/// </summary>
	public class TextStyleSample : Widget
	{
		public TextStyleSample()
		{
		}

		public TextStyleSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Document associé.
		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		// Type de l'échantillon.
		public Text.Properties.WellKnownType Type
		{
			get
			{
				return this.type;
			}

			set
			{
				this.type = value;
			}
		}

		// Style représenté.
		public Text.TextStyle TextStyle
		{
			get
			{
				return this.textStyle;
			}

			set
			{
				this.textStyle = value;
			}
		}

		// Affiche "..." au lieu de la croix si la propriété n'existe pas.
		public bool Dots
		{
			get
			{
				return this.dots;
			}

			set
			{
				this.dots = value;
			}
		}


		// Dessine l'échantillon.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if ( this.document == null )  return;

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			Color color = this.IsSelected ? adorner.ColorCaption : adorner.ColorTextBackground;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(color);  // dessine le fond
			bool drawSelection = false;

			string text = "";

			if ( text != "" )
			{
				double size = rect.Height*0.5;
				double x = rect.Left+1.0;
				double y = rect.Bottom+rect.Height*0.3;
				graphics.Color = adorner.ColorText(this.PaintState);
				graphics.PaintText(x, y, text, Font.DefaultFont, size);
			}
			else if ( this.type == Common.Text.Properties.WellKnownType.Font )
			{
				this.PaintFont(graphics, rect);
				drawSelection = true;
			}
			else if ( this.textStyle == null )  // dessine une croix ?
			{
				if ( this.dots )
				{
					double size = rect.Height*0.8;
					double x = rect.Left+1.0;
					double y = rect.Bottom+rect.Height*0.3;
					graphics.Color = adorner.ColorText(this.PaintState);
					graphics.PaintText(x, y, "...", Font.DefaultFont, size);
				}
				else
				{
					rect.Deflate(0.5);
					color = adorner.ColorBorder;
					color.A = 0.3;

					graphics.AddLine(rect.BottomLeft, rect.TopRight);
					graphics.RenderSolid(color);

					graphics.AddLine(rect.TopLeft, rect.BottomRight);
					graphics.RenderSolid(color);
				}
			}

			if ( this.IsSelected && drawSelection )
			{
				rect = this.Client.Bounds;

				rect.Deflate(0.5);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorCaption);

				rect.Deflate(1.0);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(adorner.ColorWindow);
			}
		}

		protected void PaintFont(Graphics graphics, Rectangle rect)
		{
			Text.Properties.FontProperty      font      = this.textStyle[Common.Text.Properties.WellKnownType.Font]      as Text.Properties.FontProperty;
			Text.Properties.FontSizeProperty  fontSize  = this.textStyle[Common.Text.Properties.WellKnownType.FontSize]  as Text.Properties.FontSizeProperty;
			Text.Properties.FontColorProperty fontColor = this.textStyle[Common.Text.Properties.WellKnownType.FontColor] as Text.Properties.FontColorProperty;

			double size = rect.Height*0.5;
			double x = rect.Left+1.0;
			double y = rect.Bottom+rect.Height*0.3;

			Color textColor = Color.FromBrightness(0);
			Color backColor = Color.FromBrightness(1.0);  // blanc
			double intensity = textColor.R + textColor.G + textColor.B;
			if ( intensity > 2.0 )  // couleur claire ?
			{
				backColor = Color.FromBrightness(0.0);  // noir
			}

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(backColor);

			string text = font.FaceName;
			graphics.Color = textColor;
			graphics.PaintText(x, y, text, Font.DefaultFont, size);
		}


		protected Document						document;
		protected Text.Properties.WellKnownType	type;
		protected Text.TextStyle				textStyle;
		protected bool							dots = false;
	}
}
