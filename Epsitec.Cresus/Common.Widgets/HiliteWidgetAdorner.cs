//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets
{
	public enum WidgetHiliteMode
	{
		None,
		SelectCandidate,
		DropCandidate
	}
	
	/// <summary>
	/// La classe HiliteWidgetAdorner dessine une mise en évidence permettant
	/// d'identifier quel widget est actuellement "chaud" (survolé par la
	/// souris).
	/// </summary>
	public class HiliteWidgetAdorner : AbstractWidgetAdorner
	{
		public HiliteWidgetAdorner()
		{
			this.path = new Drawing.Path ();
		}
		
		
		public Drawing.Path						Path
		{
			get { return this.path; }
		}
		
		public virtual Drawing.Rectangle		HiliteBounds
		{
			get
			{
				Drawing.Rectangle rect = Drawing.Rectangle.Empty;
				
				switch (this.hilite_mode)
				{
					case WidgetHiliteMode.DropCandidate:
						rect = this.widget.Client.Bounds;
						rect.Deflate (this.widget.GetInternalPadding ());
						break;
					
					case WidgetHiliteMode.SelectCandidate:
						rect = this.widget.Client.Bounds;
						break;
				}
				
				return rect;
			}
		}
		
		public WidgetHiliteMode					HiliteMode
		{
			get { return this.hilite_mode; }
			set { this.hilite_mode = value; }
		}
		
		
		protected override void PaintDecoration(Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			
			Drawing.FillMode  old_fill_mode = graphics.FillMode;
			Drawing.Rectangle bounds        = this.HiliteBounds;
			
			if (bounds.IsValid)
			{
				graphics.FillMode = Drawing.FillMode.EvenOdd;
				graphics.AddFilledRectangle (bounds);
				graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (bounds, -1, -1));
				graphics.RenderSolid (Drawing.Color.FromColor (HiliteWidgetAdorner.FrameColor, 0.75));
				graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (bounds, -1, -1));
				graphics.AddFilledRectangle (Drawing.Rectangle.Inflate (bounds, -2, -2));
				graphics.RenderSolid (HiliteWidgetAdorner.FrameColor);
			}
			
			if (this.path.IsValid)
			{
				graphics.FillMode = Drawing.FillMode.NonZero;
				graphics.Rasterizer.AddOutline (this.path, 1.0, Drawing.CapStyle.Square, Drawing.JoinStyle.Miter);
				graphics.RenderSolid (HiliteWidgetAdorner.HintColor);
			}
			
			graphics.FillMode = old_fill_mode;
		}
		
		
		public static Drawing.Color				FrameColor
		{
			get { return Drawing.Color.FromRgb (0.57, 0.66, 0.80); }
		}
		
		public static Drawing.Color				HintColor
		{
			get { return Drawing.Color.FromRgb (0.00, 0.00, 1.00); }
		}
		
		
		protected Drawing.Path					path;
		protected WidgetHiliteMode				hilite_mode;
	}
}
