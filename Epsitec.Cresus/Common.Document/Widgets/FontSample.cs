using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe FontSample est un widget affichant un �chantillon d'une police.
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


		public OpenType.FontIdentity FontIdentity
		{
			//	Police repr�sent�e.
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

		public string FontFace
		{
			//	Nom de la police.
			get
			{
				return this.fontName;
			}

			set
			{
				if ( this.fontName != value )
				{
					this.fontName = value;
					
					if ( this.fontName == null )
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
						
						this.textLayout.Text = TextLayout.ConvertToTaggedText(this.fontName);
					}
				}
			}
		}

		public double FontHeight
		{
			//	Hauteur pour la police.
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

		public bool IsSampleAbc
		{
			//	Type d'un �chantillon.
			get
			{
				return this.isSampleAbc;
			}

			set
			{
				if ( this.isSampleAbc != value )
				{
					this.isSampleAbc = value;
					this.Invalidate();
				}
			}
		}

		public bool IsCenter
		{
			//	Echantillon centr� horizontalement.
			get
			{
				return this.isCenter;
			}

			set
			{
				if ( this.isCenter != value )
				{
					this.isCenter = value;
					this.Invalidate();
				}
			}
		}

		public bool IsSeparator
		{
			//	Indique si l'�chantillon est suivi d'un s�parateur.
			get
			{
				return this.isSeparator;
			}
			
			set
			{
				if ( this.isSeparator != value )
				{
					this.isSeparator = value;
					this.Invalidate();
				}
			}
		}

		public bool IsLast
		{
			//	Indique si l'�chantillon est le dernier d'une liste (donc entour� d'un rectangle complet).
			get
			{
				return this.isLast;
			}
			
			set
			{
				if ( this.isLast != value )
				{
					this.isLast = value;
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

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
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

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine l'�chantillon.
			this.UpdateClientGeometry();

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			Color backColor = adorner.ColorTextBackground;
			if ( this.ActiveState == ActiveState.Yes )
			{
				backColor = adorner.ColorCaption;
			}
			else if ( this.IsEntered )  // survol� ?
			{
				Color c1 = adorner.ColorTextBackground;
				Color c2 = adorner.ColorCaption;
				double r = c1.R + (c2.R-c1.R)*0.2;
				double g = c1.G + (c2.G-c1.G)*0.2;
				double b = c1.B + (c2.B-c1.B)*0.2;
				backColor = Color.FromRgb(r,g,b);
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
					//	Dessine le nom de la police.
					Point pos = new Point(5, oy-4);
					this.textLayout.Paint(pos, graphics, left, textColor, GlyphPaintStyle.Normal);

					//	Dessine le nombre de variantes.
					string text = this.fontIdentity.FontStyleCount.ToString();
					ox = this.frontier-16-1;
					graphics.PaintText(ox, oy-1, 16, 20, text, this.DefaultFont, this.DefaultFontSize, ContentAlignment.BottomCenter);

					ox = this.frontier+5;
				}

				//	Dessine l'�chantillon de la police.
				double size = (this.fontHeight == 0) ? rect.Height*0.85 : this.fontHeight*0.85;
				Path path;
				if ( this.isSampleAbc )
				{
					path = Common.Widgets.Helpers.FontPreviewer.GetPathAbc(this.fontIdentity, ox, oy, size);
				}
				else
				{
					path = Common.Widgets.Helpers.FontPreviewer.GetPath(this.fontIdentity, ox, oy, size);
				}
				
				if ( path != null )
				{
					double sx = 0;
					if ( this.isCenter )
					{
						Rectangle bounds = path.ComputeBounds();
						sx = (rect.Width-bounds.Width)/2-ox;
						graphics.TranslateTransform(sx, 0);
					}

					graphics.Color = textColor;
					graphics.PaintSurface(path);
					path.Dispose();

					if ( this.isCenter )
					{
						graphics.TranslateTransform(-sx, 0);
					}
				}
			}

			if ( this.textLayout != null )
			{
				graphics.Align(ref left);
				left.Deflate(0.5);
				graphics.AddLine(left.BottomRight, left.TopRight);  // trait vertical de s�paration
				left.Width -= 16;
				graphics.AddLine(left.BottomRight, left.TopRight);

				rect.Deflate(0.5);

				if ( this.isLast )
				{
					graphics.AddRectangle(rect);
				}
				else
				{
					graphics.AddLine(rect.BottomLeft, rect.TopLeft    );
					graphics.AddLine(rect.TopLeft,    rect.TopRight   );
					graphics.AddLine(rect.TopRight,   rect.BottomRight);  // U invers�
				}

				if ( this.isSeparator )
				{
					rect.Bottom += 1;
					graphics.AddLine(rect.BottomLeft, rect.BottomRight);  // double trait === en bas
				}

				graphics.RenderSolid(frameColor);  // dessine le cadre
			}
		}


		protected OpenType.FontIdentity			fontIdentity;
		protected double						fontHeight;
		protected bool							isSampleAbc;
		protected bool							isCenter;
		protected bool							isSeparator;
		protected bool							isLast;
		protected double						frontier;
		protected TextLayout					textLayout;
		protected string						fontName;
	}
}
