namespace Epsitec.Common.Widgets.Design
{
	using System.Collections;
	
	/// <summary>
	/// La classe WidgetWrapper encapsule un Widget de manière à lui ajouter
	/// un feed-back visuel permettant de visualiser son état d'édition.
	/// </summary>
	public class WidgetWrapper : Window.IPostPaintHandler
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
				
				this.widget.PaintForeground     -= new PaintEventHandler (this.HandleWidgetPaintForeground);
				this.widget.PaintBoundsCallback -= new PaintBoundsCallback (this.HandlePaintBoundsCallback);
				
				foreach (Widget parent in this.ancestors)
				{
					parent.PaintBoundsCallback -= new PaintBoundsCallback (this.HandlePaintBoundsCallback);
				}
				
				this.ancestors.Clear ();
				
				this.widget = null;
				this.parent = null;
			}
			
			this.widget = widget;
			
			if (this.widget != null)
			{
				this.widget.PaintForeground     += new PaintEventHandler (this.HandleWidgetPaintForeground);
				this.widget.PaintBoundsCallback += new PaintBoundsCallback (this.HandlePaintBoundsCallback);
				
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
		
		public void Detach()
		{
			this.Attach (null);
		}
		
		
		public Widget					Widget
		{
			get { return this.widget; }
		}
		
		public Widget					WidgetBeforeDrag
		{
			//	Retourne un widget qui occupe la surface avant que l'opération de
			//	drag & drop ne commence.
			
			get { return this.widget_before_drag; }
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
		
		
		
		private void HandleWidgetPaintForeground(object sender, PaintEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			//	La peinture des "ornements" se fait après-coup, dans une dernière phase
			//	d'affichage, afin d'être sûr qu'aucun widget ne couvre notre dessin.
			
			this.widget.Window.QueuePostPaintHandler (this, e.Graphics, e.ClipRectangle);
		}
		
		private void HandlePaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds)
		{
			double m = WidgetWrapper.GripperRadius + 1;
			
			bounds.Inflate (m, m);
		}
		
		
		#region IPostPaintHandler Members
		void Window.IPostPaintHandler.Paint(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle repaint)
		{
			double m = WidgetWrapper.GripperRadius + 1;
			graphics.RestoreClippingRectangle (Drawing.Rectangle.Inflate (graphics.SaveClippingRectangle (), m, m));
			this.PaintGrips (graphics, this.Widget.Client.Bounds);
		}
		#endregion
		
		protected void PaintGrips(Drawing.Graphics graphics, Drawing.Rectangle bounds)
		{
			if (this.grips_visible)
			{
				graphics.AddRectangle (bounds);
				
				if (this.grips_hilited)
				{
					double r = WidgetWrapper.GripperRadius;
					double d = WidgetWrapper.GripperRadius * 2;
					
					graphics.AddFilledRectangle (bounds.Left  - r, bounds.Bottom - r, d, d);
					graphics.AddFilledRectangle (bounds.Left  - r, bounds.Top    - r, d, d);
					graphics.AddFilledRectangle (bounds.Right - r, bounds.Bottom - r, d, d);
					graphics.AddFilledRectangle (bounds.Right - r, bounds.Top    - r, d, d);
				}
				
				graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1, 0, 0));
			}
		}
		
		
		public void DragBegin(Drawing.Point mouse)
		{
			this.original_bounds = this.widget.Bounds;
			this.original_mouse  = mouse;
			
			this.widget_before_drag = new StaticText ();
			
			this.widget_before_drag.Bounds      = this.original_bounds;
			this.widget_before_drag.Dock        = this.widget.Dock;
			this.widget_before_drag.LayoutFlags = this.widget.LayoutFlags;
			this.widget_before_drag.MinSize     = this.widget.MinSize;
			this.widget_before_drag.MaxSize     = this.widget.MaxSize;
			this.widget_before_drag.Name        = "WidgetWrapper-BeforeDrag";
			this.widget_before_drag.BackColor   = Drawing.Color.Transparent;
			this.widget_before_drag.Anchor      = this.widget.Anchor;
			
			this.widget_before_drag.SetLayoutArgs (this.widget.LayoutArg1, this.widget.LayoutArg2);
			
			this.parent.Children.Replace (this.widget, this.widget_before_drag);
			
			this.widget.Dock   = DockStyle.None;
			this.widget.Anchor = AnchorStyles.None;
			this.widget.Parent = this.parent;
			
			this.GripsVisible = false;
		}
		
		public void DragEnd()
		{
			this.widget.Parent = null;
			
			this.widget.Bounds      = this.original_bounds;
			this.widget.Dock        = this.widget_before_drag.Dock;
			this.widget.LayoutFlags = this.widget_before_drag.LayoutFlags;
			this.widget.MinSize     = this.widget_before_drag.MinSize;
			this.widget.MaxSize     = this.widget_before_drag.MaxSize;
			this.widget.Anchor      = this.widget_before_drag.Anchor;
			
			this.widget.SetLayoutArgs (this.widget_before_drag.LayoutArg1, this.widget_before_drag.LayoutArg2);
			
			this.parent.Children.Replace (this.widget_before_drag, this.widget);
			
			this.widget_before_drag.Dispose ();
			this.widget_before_drag = null;
			
			this.GripsVisible = true;
		}
		
		
		public Drawing.Rectangle GetDragRectangle(Drawing.Point mouse)
		{
			return Drawing.Rectangle.Offset (this.original_bounds, mouse - this.original_mouse);
		}
		
		
		public void DragSetDropHint(Drawing.Rectangle drop_bounds)
		{
			if ((drop_bounds == this.widget_before_drag.Bounds) ||
				(drop_bounds.IsEmpty))
			{
				//	La cible du drag & drop est la même que la position de départ,
				//	ce qui signifie que l'on va cacher la surface de mise en évidence.
				
				this.widget_before_drag.BackColor = Drawing.Color.Transparent;
				this.widget.Bounds = this.widget_before_drag.Bounds;
				this.is_drop_target_valid = false;
			}
			else
			{
				this.widget_before_drag.BackColor = Drawing.Color.FromARGB (0.2, 1.0, 0.0, 0.0);
				this.widget.Bounds = drop_bounds;
				this.is_drop_target_valid = true;
			}
		}
		
		
		
		
		protected bool					is_dragging;
		protected bool					is_drop_target_valid;
		
		protected bool					grips_visible;
		protected bool					grips_hilited;
		
		protected Widget				parent;
		protected Widget				widget;
		protected Widget				widget_before_drag;
		protected Drawing.Rectangle		original_bounds;
		protected Drawing.Point			original_mouse;
		protected ArrayList				ancestors = new ArrayList ();
		
		protected const double			GripperRadius = 2;
	}
}
