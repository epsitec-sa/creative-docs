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

			this.Padding = new Margins(5, 5, 5, RibbonSection.LabelHeight+5);
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
				rect.Bottom += RibbonSection.LabelHeight;
				rect.Deflate(4);
				return rect;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;

			Rectangle userRect = rect;
			userRect.Bottom += RibbonSection.LabelHeight;

			Rectangle textRect = rect;
			textRect.Top = textRect.Bottom+RibbonSection.LabelHeight;
			
			adorner.PaintRibbonSectionBackground(graphics, rect, userRect, textRect, this.title, this.PaintState);
		}


		protected static readonly double	LabelHeight = 14;

		protected TextLayout				title;
	}
}
