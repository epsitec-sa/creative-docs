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
				if ( this.textLayout == null )  return null;
				return this.textLayout.Text;
			}

			set
			{
				if ( value == null )
				{
					this.textLayout = null;
				}
				else
				{
					if ( this.textLayout == null )
					{
						this.textLayout = new TextLayout();
						this.textLayout.DefaultFont     = this.DefaultFont;
						this.textLayout.DefaultFontSize = this.DefaultFontSize;
						this.textLayout.Alignment       = ContentAlignment.MiddleLeft;
					}
					this.textLayout.Text = value;
				}
			}
		}

		// Hauteur pour la police.
		public double FontHeight
		{
			get
			{
				return this.fontHeight;
			}

			set
			{
				if ( this.fontHeight != value )
				{
					this.fontHeight = value;
					this.Invalidate();
				}
			}
		}

		// Type d'un échantillon.
		public bool SampleAbc
		{
			get
			{
				return this.sampleAbc;
			}

			set
			{
				if ( this.sampleAbc != value )
				{
					this.sampleAbc = value;
					this.Invalidate();
				}
			}
		}

		// Echantillon centré horizontalement.
		public bool Center
		{
			get
			{
				return this.center;
			}

			set
			{
				if ( this.center != value )
				{
					this.center = value;
					this.Invalidate();
				}
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

			if ( this.textLayout == null )
			{
				this.frontier = 0;
			}
			else
			{
				this.frontier = 160;
				this.textLayout.LayoutSize = new Size(this.frontier-5-16, 20);
			}
		}

		// Dessine l'échantillon.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.UpdateClientGeometry();

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			Color backColor = adorner.ColorTextBackground;
			if ( this.ActiveState == ActiveState.Yes )
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

			if ( this.textLayout != null )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(backColor);  // dessine le fond
			}

			Rectangle left = rect;
			left.Width = this.frontier;

			if ( this.fontIdentity != null )
			{
				double ox = 0;
				double oy = rect.Height*0.25;

				if ( this.textLayout == null )
				{
					ox = this.frontier+2;
				}
				else
				{
					// Dessine le nom de la police.
					Point pos = new Point(5, oy-4);
					this.textLayout.Paint(pos, graphics, left, textColor, GlyphPaintStyle.Normal);

					// Dessine le nombre de variantes.
					string text = this.fontIdentity.FontStyleCount.ToString();
					ox = this.frontier-16-1;
					graphics.PaintText(ox, oy-1, 16, 20, text, this.DefaultFont, this.DefaultFontSize, ContentAlignment.BottomCenter);

					ox = this.frontier+5;
				}

				// Dessine l'échantillon de la police.
				double size = (this.fontHeight == 0) ? rect.Height*0.85 : this.fontHeight*0.85;
				Path path;
				if ( this.sampleAbc )
				{
					path = Common.Widgets.Helpers.FontPreviewer.GetPathAbc(this.fontIdentity, ox, oy, size);
				}
				else
				{
					path = Common.Widgets.Helpers.FontPreviewer.GetPath(this.fontIdentity, ox, oy, size);
				}

				double sx = 0;
				if ( this.center )
				{
					Rectangle bounds = path.ComputeBounds();
					sx = (rect.Width-bounds.Width)/2-ox;
					graphics.TranslateTransform(sx, 0);
				}

				graphics.Color = textColor;
				graphics.PaintSurface(path);
				path.Dispose();

				if ( this.center )
				{
					graphics.TranslateTransform(-sx, 0);
				}
			}

			if ( this.textLayout != null )
			{
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
		}


		protected OpenType.FontIdentity			fontIdentity = null;
		protected double						fontHeight = 0;
		protected bool							sampleAbc = false;
		protected bool							center = false;
		protected bool							separator = false;
		protected bool							last = false;
		protected double						frontier = 0;
		protected TextLayout					textLayout = null;
	}
}
