using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
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


		static RibbonContainer()
		{
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (66.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.PreferredHeightProperty.OverrideMetadata (typeof (RibbonContainer), metadataDy);
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

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;
			adorner.PaintRibbonTabBackground(graphics, rect, state);
		}
	}
}
