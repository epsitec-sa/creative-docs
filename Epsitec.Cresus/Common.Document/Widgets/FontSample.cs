using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe FontSample est un widget affichant un échantillon d'une police.
	/// </summary>
	public class FontSample : Widget
	{
		public FontSample()
		{
			this.textLayout = new TextLayout();
			this.textLayout.DefaultFont     = this.DefaultFont;
			this.textLayout.DefaultFontSize = this.DefaultFontSize;
			this.textLayout.Alignment       = ContentAlignment.MiddleLeft;
		}

		public FontSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Police représentée.
		public OpenType.FontIdentity FontIdentity
		{
			get
			{
				return this.fontIdentity;
			}

			set
			{
				if ( this.fontIdentity != value )
				{
					this.fontIdentity = value;
					this.Invalidate();
				}
			}
		}

		// Nom de la police.
		public string FontFace
		{
			get
			{
				return this.textLayout.Text;
			}

			set
			{
				this.textLayout.Text = value;
			}
		}

		// Indique si l'échantillon est suivi d'un séparateur.
		public bool Separator
		{
			get
			{
				return this.separator;
			}
			
			set
			{
				if ( this.separator != value )
				{
					this.separator = value;
					this.Invalidate();
				}
			}
		}

		// Indique si l'échantillon est le dernier d'une liste (donc entouré d'un rectangle complet).
		public bool Last
		{
			get
			{
				return this.last;
			}
			
			set
			{
				if ( this.last != value )
				{
					this.last = value;
					this.Invalidate();
				}
			}
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered(e);
			this.Invalidate();
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited(e);
			this.Invalidate();
		}

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.textLayout == null )  return;

			Rectangle rect = this.Client.Bounds;

			this.frontier = 160;
			this.textLayout.LayoutSize = new Size(this.frontier-5-16, 20);
		}

		// Dessine l'échantillon.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			Color backColor = adorner.ColorTextBackground;
			if ( this.IsSelected )
			{
				backColor = adorner.ColorCaption;
			}
			else if ( this.IsEntered )  // survolé ?
			{
				Color c1 = adorner.ColorTextBackground;
				Color c2 = adorner.ColorCaption;
				double r = c1.R + (c2.R-c1.R)*0.2;
				double g = c1.G + (c2.G-c1.G)*0.2;
				double b = c1.B + (c2.B-c1.B)*0.2;
				backColor = Color.FromRGB(r,g,b);
			}

			Color textColor  = adorner.ColorText(this.PaintState);
			Color frameColor = adorner.ColorTextFieldBorder(this.IsEnabled);

			Color hiliColor = Color.Empty;
			if ( this.IsEntered )
			{
				hiliColor = adorner.ColorCaption;
				hiliColor.A = 0.2;
			}

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(backColor);  // dessine le fond

			Rectangle left = rect;
			left.Width = this.frontier;

			if ( this.fontIdentity != null )
			{
				double ox;
				double oy = rect.Height*0.25;

				// Dessine le nom de la piloce.
				Point pos = new Point(5, oy-4);
				this.textLayout.Paint(pos, graphics, left, textColor, GlyphPaintStyle.Normal);

				// Dessine le nombre de variantes.
				string text = this.fontIdentity.FontStyleCount.ToString();
				ox = this.frontier-16-1;
				graphics.PaintText(ox, oy-1, 16, 20, text, this.DefaultFont, this.DefaultFontSize, ContentAlignment.BottomCenter);

				// Dessine l'échantillon de la police.
				ox = this.frontier+5;
				Path path = Common.Widgets.Helpers.FontPreviewer.GetPath(this.fontIdentity, ox, oy, rect.Height*0.75);
				graphics.Color = textColor;
				graphics.PaintSurface(path);
				path.Dispose();
			}

			graphics.Align(ref left);
			left.Deflate(0.5);
			graphics.AddLine(left.BottomRight, left.TopRight);  // trait vertical de séparation
			left.Width -= 16;
			graphics.AddLine(left.BottomRight, left.TopRight);

			rect.Deflate(0.5);

			if ( this.last )
			{
				graphics.AddRectangle(rect);
			}
			else
			{
				graphics.AddLine(rect.BottomLeft, rect.TopLeft    );
				graphics.AddLine(rect.TopLeft,    rect.TopRight   );
				graphics.AddLine(rect.TopRight,   rect.BottomRight);  // U inversé
			}

			if ( this.Separator )
			{
				rect.Bottom += 1;
				graphics.AddLine(rect.BottomLeft, rect.BottomRight);  // double trait === en bas
			}

			graphics.RenderSolid(frameColor);  // dessine le cadre
		}


		protected OpenType.FontIdentity			fontIdentity = null;
		protected bool							separator = false;
		protected bool							last = false;
		protected double						frontier = 0;
		protected TextLayout					textLayout = null;
	}
}
