namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for Panel.
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle shape = base.GetShapeBounds ();
			
			double x1 = shape.Left   - this.frame_margins.Left;
			double x2 = shape.Right  + this.frame_margins.Right;
			double y1 = shape.Bottom - this.frame_margins.Bottom;
			double y2 = shape.Top    + this.frame_margins.Top;
			
			return new Drawing.Rectangle (x1, y1, x2-x1, y2-y1);
		}
		
		public override Drawing.Rectangle GetClipBounds()
		{
			Drawing.Rectangle parent_clip = this.MapParentToClient (this.parent.GetClipBounds ());
			Drawing.Rectangle client_clip = base.GetClipBounds ();
			
			return Drawing.Rectangle.Intersection (parent_clip, client_clip);
		}
		
		
		public Drawing.Margins			FrameMargins
		{
			get
			{
				return this.frame_margins;
			}
			
			set
			{
				if (this.frame_margins != value)
				{
					this.frame_margins = value;
					this.Invalidate ();
				}
			}
		}
		
		public Drawing.Rectangle		FrameBounds
		{
			get
			{
				return this.frame_bounds;
			}
			set
			{
				if (this.frame_bounds != value)
				{
					this.frame_bounds = value;
					this.Invalidate ();
				}
			}
		}
		
		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clip_rect)
		{
			Drawing.Rectangle old_clip = graphics.SaveClippingRectangle ();
			Drawing.Rectangle new_clip = this.MapClientToRoot (this.frame_bounds);
			
			graphics.RestoreClippingRectangle (new_clip);
			graphics.AddRectangle (this.frame_bounds);
			graphics.RenderSolid (Drawing.Color.FromRGB (0, 0, 1));
			graphics.RestoreClippingRectangle (old_clip);
			
			base.PaintBackgroundImplementation (graphics, clip_rect);
		}

		
		
		protected Drawing.Margins		frame_margins;
		protected Drawing.Rectangle		frame_bounds;
	}
}
