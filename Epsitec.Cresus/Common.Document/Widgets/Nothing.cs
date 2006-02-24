using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe Nothing permet de représenter une croix.
	/// </summary>
	public class Nothing : Widget
	{
		public Nothing()
		{
		}
		
		public Nothing(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine la croix.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			graphics.AddLine(rect.BottomLeft, rect.TopRight);
			graphics.AddLine(rect.BottomRight, rect.TopLeft);
			graphics.RenderSolid(adorner.ColorBorder);
		}
	}
}
