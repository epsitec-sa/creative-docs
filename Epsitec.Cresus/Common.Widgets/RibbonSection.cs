using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonSection est la classe de base pour toutes les sections de rubans.
	/// </summary>
	public abstract class RibbonSection : Widget
	{
		public RibbonSection()
		{
			this.PreferredWidth = this.DefaultWidth;
			this.PreferredHeight = this.DefaultHeight;
			this.title = new TextLayout();
			this.title.DefaultFont     = this.DefaultFont;
			this.title.DefaultFontSize = this.DefaultFontSize;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		
		public virtual double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*2;
			}
		}

		public virtual double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight + 8 + 22 + 5 + 22;
			}
		}

		protected double LabelHeight
		{
			//	Retourne la hauteur pour le label supérieur.
			get
			{
				return 14;
			}
		}


		protected Rectangle UsefulZone
		{
			//	Retourne la zone rectangulaire utile pour les widgets.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= this.LabelHeight;
				rect.Deflate(4);
				return rect;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;
			adorner.PaintRibbonSectionBackground(graphics, rect, this.LabelHeight, state);

			Point pos = new Point(rect.Left+3, rect.Top-this.LabelHeight);
			this.title.LayoutSize = new Size(rect.Width-4, this.LabelHeight);
			adorner.PaintRibbonSectionTextLayout(graphics, pos, this.title, state);
		}


		protected TextLayout				title;
		protected int						tabIndex = 0;
		protected bool						ignoreChange = false;
		protected double					separatorWidth = 8;
	}
}
