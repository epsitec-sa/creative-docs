namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class AlphaBar représente une barre translucide pendant un drag.
	/// </summary>
	public class AlphaBar : Widget
	{
		public AlphaBar()
		{
			this.color = Drawing.Color.FromAlphaRgb(0.15, 0,0,0);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine la barre.
			Drawing.Rectangle rect  = this.Client.Bounds;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.color);
		}
		
		
		protected Drawing.Color			color;
	}
}
