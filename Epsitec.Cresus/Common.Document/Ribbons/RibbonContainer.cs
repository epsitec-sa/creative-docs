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
		}
		
		public RibbonContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		// Retourne la hauteur standard d'une barre.
		public override double DefaultHeight
		{
			get
			{
				return 66;
			}
		}
		
		// Retourne la hauteur pour le label supérieur.
		protected double LabelHeight
		{
			get
			{
				return 14;
			}
		}

		public override DockStyle DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Left;
			}
		}

		public void SetDocument(DocumentType type, InstallType install, Settings.GlobalSettings gs, Document document)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.SetDocument(type, install, gs, document);
			}
		}

		public void SetDirtyText()
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.SetDirtyText();
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetState state = this.PaintState;
			adorner.PaintRibbonTabBackground(graphics, rect, this.LabelHeight, state);
		}
	}
}
