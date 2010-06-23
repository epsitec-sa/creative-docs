using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
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
			WidgetPaintState state = this.GetPaintState ();
			Drawing.Rectangle rect = this.Client.Bounds;
			Drawing.Color color = adorner.ColorTextFieldBorder((state&WidgetPaintState.Enabled) != 0);

			graphics.AddLine(rect.BottomLeft, rect.TopRight);
			graphics.AddLine(rect.BottomRight, rect.TopLeft);
			graphics.RenderSolid(color);
		}
	}
}
