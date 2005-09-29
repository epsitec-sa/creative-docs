using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe RibbonContainer permet de réaliser des rubans horizontaux.
	/// </summary>
	public class RibbonContainer : AbstractToolBar
	{
		public RibbonContainer()
		{
			this.direction = Direction.Up;
			
			//?double m = (this.DefaultHeight-this.defaultButtonHeight)/2;
			//?this.DockPadding = new Drawing.Margins(m, m, m, m);
		}
		
		public RibbonContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double				DefaultHeight
		{
			// Retourne la hauteur standard d'une barre.
			get
			{
				return 66;
			}
		}
		
		protected double					LabelHeight
		{
			// Retourne la hauteur pour le label supérieur.
			get
			{
				return 14;
			}
		}

		public override DockStyle			DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Left;
			}
		}

		public void SetDirtyContent()
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.SetDirtyContent();
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetState state = this.PaintState;
			adorner.PaintRibbonTabBackground(graphics, rect, this.LabelHeight, state);
		}
	}
}
