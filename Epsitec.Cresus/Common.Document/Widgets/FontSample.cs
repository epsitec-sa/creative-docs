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

		// Dessine l'échantillon.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			double sep = rect.Width*0.5;

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

			rect.Width = sep;
			graphics.Align(ref rect);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(backColor);  // dessine le fond

			double ox = 5;
			double oy = rect.Height*0.25;
			double size = 10;
			if ( this.fontIdentity != null )
			{
				graphics.Color = textColor;
				graphics.PaintText(ox, oy, this.fontIdentity.InvariantFaceName, Drawing.Font.DefaultFont, size);
			}

			rect.Offset(sep, 0);
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(backColor);  // dessine le fond

			ox = sep+5;
			size = rect.Height*0.75;
			if ( this.fontIdentity != null )
			{
				Path path = Common.Widgets.Helpers.FontPreviewer.GetPath(this.fontIdentity, ox, oy, size);
				graphics.Color = textColor;
				graphics.PaintSurface(path);
				path.Dispose();
			}
			
			graphics.AddLine(rect.BottomLeft, rect.TopLeft);

			rect = this.Client.Bounds;
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
				graphics.AddLine(rect.BottomLeft, rect.BottomRight);
			}

			graphics.RenderSolid(frameColor);  // dessine le cadre
		}


		protected OpenType.FontIdentity			fontIdentity = null;
		protected bool							separator = false;
		protected bool							last = false;
	}
}
