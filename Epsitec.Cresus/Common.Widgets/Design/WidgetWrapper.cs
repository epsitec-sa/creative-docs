namespace Epsitec.Common.Widgets.Design
{
	using System.Collections;
	
	/// <summary>
	/// La classe WidgetWrapper encapsule un Widget de manière à le rendre
	/// éditable.
	/// </summary>
	public class WidgetWrapper
	{
		
		public WidgetWrapper()
		{
		}
		
		public void Attach(Widget widget)
		{
			if (this.widget == widget)
			{
				return;
			}
			
			if (this.widget != null)
			{
				this.widget.Invalidate ();
				
				this.widget.PaintForeground     -= new PaintEventHandler (HandleWidgetPaintForeground);
				this.widget.PreProcessing       -= new MessageEventHandler (this.HandleWidgetPreProcessing);
				this.widget.PaintBoundsCallback -= new PaintBoundsCallback (HandlePaintBoundsCallback);
				
				foreach (Widget parent in this.ancestors)
				{
					parent.PaintBoundsCallback -= new PaintBoundsCallback (HandlePaintBoundsCallback);
				}
				
				this.ancestors.Clear ();
				
				this.widget = null;
				this.parent = null;
			}
			
			this.widget = widget;
			
			if (this.widget != null)
			{
				this.widget.PaintForeground     += new PaintEventHandler (HandleWidgetPaintForeground);
				this.widget.PreProcessing       += new MessageEventHandler (this.HandleWidgetPreProcessing);
				this.widget.PaintBoundsCallback += new PaintBoundsCallback (HandlePaintBoundsCallback);
				
				Widget parent = widget.Parent;
				
				while (parent != null)
				{
					this.ancestors.Add (parent);
					parent.PaintBoundsCallback += new PaintBoundsCallback (HandlePaintBoundsCallback);
					parent = parent.Parent;
				}
				
				this.parent = widget.Parent;
				
				this.widget.Invalidate ();
			}
		}
		
		
		public Widget					Widget
		{
			get { return this.widget; }
		}
		
		public Widget					WidgetOriginalSurface
		{
			get { return this.widget_original_surface; }
		}
		
		
		public bool						GripsVisible
		{
			get { return this.grips_visible; }
			set
			{
				if (this.grips_visible != value)
				{
					this.grips_visible = value;
					this.widget.Invalidate ();
				}
			}
		}
		
		public bool						GripsHilited
		{
			get { return this.grips_hilited; }
			set
			{
				if (this.grips_hilited != value)
				{
					this.grips_hilited = value;
					this.widget.Invalidate ();
				}
			}
		}
		
		
		public bool						IsDropTargetValid
		{
			get { return this.is_drop_target_valid; }
		}
		
		
		public Drawing.Rectangle		OriginalBounds
		{
			get { return this.original_bounds; }
		}
		
		
		private void HandleWidgetPreProcessing(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			e.Suppress |= this.ProcessMessage (e.Message);
		}
		
		
		internal bool ProcessMessage(Message message)
		{
			if (message.IsMouseType)
			{
				Drawing.Point mouse = this.widget.MapRootToClient (message.Cursor);
				
				switch (message.Type)
				{
					case MessageType.MouseEnter:
						this.mouse_cursor = MouseCursor.AsSizeAll;
						break;
					case MessageType.MouseLeave:
						this.mouse_cursor = MouseCursor.Default;
						break;
					
					case MessageType.MouseDown:
						this.is_dragging = true;
						break;
					
					case MessageType.MouseUp:
						this.is_dragging = false;
						break;
					
					case MessageType.MouseMove:
						break;
				}
				
				this.widget.Window.MouseCursor = this.mouse_cursor;
			}
			
			message.ForceCapture = this.is_dragging;
			message.Consumer = this.widget;
			
			return true;
		}
		
		
		private void HandleWidgetPaintForeground(object sender, PaintEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			Drawing.Graphics graphics = e.Graphics;
			
			this.PaintGrips (graphics, this.Widget.Client.Bounds);
		}
		
		protected void PaintGrips(Drawing.Graphics graphics, Drawing.Rectangle bounds)
		{
			if (this.grips_visible)
			{
				graphics.AddRectangle (bounds);
				
				if (this.grips_hilited)
				{
					graphics.AddFilledRectangle (bounds.Left  - 2, bounds.Bottom - 2, 4, 4);
					graphics.AddFilledRectangle (bounds.Left  - 2, bounds.Top    - 2, 4, 4);
					graphics.AddFilledRectangle (bounds.Right - 2, bounds.Bottom - 2, 4, 4);
					graphics.AddFilledRectangle (bounds.Right - 2, bounds.Top    - 2, 4, 4);
				}
				
				graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1, 0, 0));
			}
		}
		
		private void HandlePaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds)
		{
			bounds.Inflate (3, 3);
		}
		
		
		public void DraggingBegin(Drawing.Point mouse)
		{
			this.original_bounds = this.widget.Bounds;
			this.original_mouse  = mouse;
			
			this.widget_original_surface = new StaticText ();
			
			this.widget_original_surface.Bounds      = this.original_bounds;
			this.widget_original_surface.Dock        = this.widget.Dock;
			this.widget_original_surface.LayoutFlags = this.widget.LayoutFlags;
			this.widget_original_surface.MinSize     = this.widget.MinSize;
			this.widget_original_surface.MaxSize     = this.widget.MaxSize;
			this.widget_original_surface.Name        = "WidgetWrapper Original Surface";
			this.widget_original_surface.BackColor   = Drawing.Color.Transparent;
			
			this.widget_original_surface.SetLayoutArgs (this.widget.LayoutArg1, this.widget.LayoutArg2);
			
			this.parent.Children.Replace (this.widget, this.widget_original_surface);
			
			this.widget.Dock   = DockStyle.None;
			this.widget.Parent = this.parent;
			
			this.GripsVisible = false;
		}
		
		public void DraggingEnd()
		{
			this.widget.Parent = null;
			
			this.widget.Bounds      = this.original_bounds;
			this.widget.Dock        = this.widget_original_surface.Dock;
			this.widget.LayoutFlags = this.widget_original_surface.LayoutFlags;
			this.widget.MinSize     = this.widget_original_surface.MinSize;
			this.widget.MaxSize     = this.widget_original_surface.MaxSize;
			
			this.widget.SetLayoutArgs (this.widget_original_surface.LayoutArg1, this.widget_original_surface.LayoutArg2);
			
			this.parent.Children.Replace (this.widget_original_surface, this.widget);
			
			this.widget_original_surface.Dispose ();
			this.widget_original_surface = null;
			
			this.GripsVisible = true;
		}
		
		
		public Drawing.Rectangle GetDraggingRectangle(Drawing.Point mouse)
		{
			return Drawing.Rectangle.Offset (this.original_bounds, mouse - this.original_mouse);
		}
		
		
		public void DraggingSetDropHint(Drawing.Rectangle drop_bounds)
		{
			if ((drop_bounds == this.widget_original_surface.Bounds) ||
				(drop_bounds.IsEmpty))
			{
				//	La cible du drag & drop est la même que la position de départ,
				//	ce qui signifie que l'on va cacher la surface de mise en évidence.
				
				this.widget_original_surface.BackColor = Drawing.Color.Transparent;
				this.widget.Bounds = this.widget_original_surface.Bounds;
				this.is_drop_target_valid = false;
			}
			else
			{
				this.widget_original_surface.BackColor = Drawing.Color.FromARGB (0.2, 1.0, 0.0, 0.0);
				this.widget.Bounds = drop_bounds;
				this.is_drop_target_valid = true;
			}

		}
		
		
		
		
		protected bool					is_dragging;
		protected bool					is_drop_target_valid;
		
		protected bool					grips_visible;
		protected bool					grips_hilited;
		
		protected MouseCursor			mouse_cursor = MouseCursor.Default;
		
		protected Widget				parent;
		protected Widget				widget;
		protected Widget				widget_original_surface;
		protected Drawing.Rectangle		original_bounds;
		protected Drawing.Point			original_mouse;
		protected ArrayList				ancestors = new ArrayList ();
	}
}
