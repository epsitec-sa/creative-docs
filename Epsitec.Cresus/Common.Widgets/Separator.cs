namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Separator permet de dessiner des séparations.
	/// </summary>
	public class Separator : Widget
	{
		public Separator()
		{
		}
		
		public Separator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}
	}
}
