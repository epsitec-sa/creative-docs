//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe HiliteAdorner dessine une mise en évidence permettant
	/// d'identifier quel widget est actuellement "chaud" (survolé par la
	/// souris).
	/// </summary>
	public class HiliteAdorner : AbstractAdorner
	{
		public HiliteAdorner()
		{
			this.path = new Drawing.Path ();
		}
		
		public Drawing.Path					Path
		{
			get { return this.path; }
		}
		
		
		protected override void PaintDecoration(Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			Drawing.FillMode old_fill_mode = graphics.Rasterizer.FillMode;
			
			graphics.Rasterizer.FillMode = Drawing.FillMode.EvenOdd;
			graphics.AddFilledRectangle (this.widget.Client.Bounds);
			graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (this.widget.Client.Bounds, -1, -1));
			graphics.RenderSolid (Drawing.Color.FromColor (HiliteAdorner.FrameColor, 0.75));
			graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (this.widget.Client.Bounds, -1, -1));
			graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (this.widget.Client.Bounds, -2, -2));
			graphics.RenderSolid (HiliteAdorner.FrameColor);
			
			if (this.path.IsValid)
			{
				graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
				graphics.Rasterizer.AddOutline (this.path, 1.0, Drawing.CapStyle.Square, Drawing.JoinStyle.Miter);
				graphics.RenderSolid (HiliteAdorner.HintColor);
			}
			
			graphics.Rasterizer.FillMode = old_fill_mode;
		}
		
		
		public static Drawing.Color			FrameColor
		{
			get { return Drawing.Color.FromRGB (0.57, 0.66, 0.80); }
		}
		
		public static Drawing.Color			HintColor
		{
			get { return Drawing.Color.FromRGB (0.00, 0.00, 1.00); }
		}
		
		
		protected Drawing.Path				path;
	}
}
