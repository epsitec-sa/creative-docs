using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonSection est la classe de base pour toutes les sections de rubans.
	/// </summary>
	public class RibbonSection : Widget
	{
		public RibbonSection()
		{
			this.title = new TextLayout();
			this.title.DefaultFont     = this.DefaultFont;
			this.title.DefaultFontSize = this.DefaultFontSize;

			this.Padding = new Margins(5, 5, RibbonSection.LabelHeight+5, 5);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		public string						Title
		{
			get
			{
				return this.title.Text;
			}

			set
			{
				this.title.Text = value;
			}
		}


		protected Rectangle UsefulZone
		{
			//	Retourne la zone rectangulaire utile pour les widgets.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= RibbonSection.LabelHeight;
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
			//?adorner.PaintRibbonSectionBackground(graphics, rect, RibbonSection.LabelHeight, state);
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Color.FromBrightness(0));
			rect.Inflate(0.5);

			rect.Bottom = rect.Top-RibbonSection.LabelHeight;
			adorner.PaintRibbonSectionTextLayout(graphics, rect, this.title, state);
		}


		protected static readonly double	LabelHeight = 14;

		protected TextLayout				title;
	}
}
