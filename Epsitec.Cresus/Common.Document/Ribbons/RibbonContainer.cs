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


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard d'une barre.
			get
			{
				return 66;
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

		public override DockStyle DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Left;
			}
		}

		public void NotifyChanged(string changed)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyChanged(changed);
			}
		}

		public void NotifyTextStylesChanged(System.Collections.ArrayList textStyleList)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyTextStylesChanged(textStyleList);
			}
		}

		public void NotifyTextStylesChanged()
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyTextStylesChanged();
			}
		}

		public void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.SetDocument(type, install, debug, gs, document);
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
