namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class AlphaBar représente une barre translucide pendant un drag.
	/// </summary>
	public class AlphaBar : Widget
	{
		public AlphaBar()
		{
			this.color = Drawing.Color.FromARGB(0.15, 0,0,0);
		}
		
		// Dessine la barre.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.color);
		}
		
		
		protected Drawing.Color			color;
	}
}
