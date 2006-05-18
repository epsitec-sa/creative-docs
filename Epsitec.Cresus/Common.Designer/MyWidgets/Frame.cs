using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget de type 'groupe' avec un cadre.
	/// </summary>
	public class Frame : AbstractGroup
	{
		public Frame() : base()
		{
		}

		public Frame(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static Frame()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(Frame), metadata);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le texte.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}

	}
}
