namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe DragSource abrite un widget qui, lorsqu'il est "dragged" vient
	/// automatiquement clôné dans un DragWindow.
	/// </summary>
	public class DragSource : Widget, Helpers.IDragBehaviorHost
	{
		public DragSource()
		{
			this.drag_behavior  = new Helpers.DragBehavior (this);
			this.widget_margins = new Drawing.Margins (3, 3, 3, 3);
			this.drop_adorner   = new Design.HiliteAdorner ();
		}
		
		public DragSource(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Widget						Widget
		{
			get { return this.widget; }
			set
			{
				if (this.widget != value)
				{
					this.DetachWidget (this.widget);
					this.widget = value;
					this.AttachWidget (this.widget);
				}
			}
		}
		
		public Drawing.Rectangle			DropScreenBounds
		{
			get
			{
				if (this.drag_window != null)
				{
					Drawing.Point pos  = this.drag_window.WindowLocation;
					pos.X += this.widget_margins.Left;
					pos.Y += this.widget_margins.Bottom;
					return new Drawing.Rectangle (pos, this.widget.Size);
				}
				
				return Drawing.Rectangle.Empty;
			}
		}
		
		public Drawing.Rectangle			DropTargetBounds
		{
			get
			{
				Drawing.Rectangle bounds = this.DropScreenBounds;
				
				if (bounds.IsValid)
				{
					return this.DropTarget.MapScreenToClient (bounds);
				}
				
				return bounds;
			}
		}
		
		public double						DropTargetBaseLineOffset
		{
			get
			{
				Drawing.Point base_line = this.widget.BaseLine;
				
				if (base_line.IsEmpty)
				{
					return -1;
				}
				
				return base_line.Y;
			}
		}
		
		public Widget						DropTarget
		{
			get
			{
				return this.drop_adorner.Widget;
			}
		}
		
		
		public DragWindow CreateDragWindow(Widget widget)
		{
			Support.ObjectBundler bundler = new Support.ObjectBundler ();
			
			Widget     copy   = bundler.CopyObject (widget) as Widget;
			DragWindow window = new DragWindow ();
			
			window.DefineWidget (copy, this.widget_margins);
			
			return window;
		}
		
		public Window FindTargetWindow(Drawing.Point pos)
		{
			Window[] windows = Platform.WindowList.GetVisibleWindows ();
			
			for (int i = 0; i < windows.Length; i++)
			{
				if (windows[i] is DragWindow)
				{
					continue;
				}
				
				if (windows[i].WindowBounds.Contains (pos))
				{
					return windows[i];
				}
			}
			
			return null;
		}
		
		public Widget FindTargetWidget(Window window, Drawing.Point pos)
		{
			if (window == null)
			{
				return null;
			}
			
			pos = window.Root.MapScreenToClient (pos);
			Widget hit    = null;
			Widget widget = window.Root;
			Widget temp   = window.Root;
			
			do
			{
				widget = temp;
				pos    = widget.MapParentToClient (pos);
				temp   = widget.FindChild (pos, Widget.ChildFindMode.SkipDisabled | Widget.ChildFindMode.SkipHidden);
				
				if (widget.PossibleContainer && !widget.IsEditionDisabled)
				{
					hit = widget;
				}
			}
			while (temp != null);
			
			return hit;
		}
		
		
		protected void AttachWidget(Widget widget)
		{
			if (widget != null)
			{
				widget.Parent = this;
				widget.Bounds = this.Client.Bounds;
				widget.Dock   = DockStyle.Fill;
				widget.SetFrozen (true);
			}
		}
		
		protected void DetachWidget(Widget widget)
		{
			if (widget != null)
			{
				widget.Parent = null;
			}
		}
		
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (this.drag_behavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}
		
		
		#region Interface IDragBehaviorHost
		public Drawing.Point				DragLocation
		{
			get
			{
				return this.drag_window.WindowLocation;
			}
		}
		
		
		void Helpers.IDragBehaviorHost.OnDragBegin()
		{
			Drawing.Point pos = this.MapClientToScreen (this.widget.Location);
			
			pos.X -= this.widget_margins.Left;
			pos.Y -= this.widget_margins.Bottom;
			
			this.drag_window = this.CreateDragWindow (this.widget);
			this.drag_window.WindowLocation = pos;
			this.drag_window.Show ();
		}
		
		void Helpers.IDragBehaviorHost.OnDragging(DragEventArgs e)
		{
			this.drag_cursor = this.Window.MapWindowToScreen (Message.State.LastPosition) - new Drawing.Point (1, 1);
			this.drag_window.WindowLocation += e.Offset;
			
			Window target = this.FindTargetWindow (this.drag_cursor);
			Widget widget = this.FindTargetWidget (target, this.drag_cursor);
			string title  = widget == null ? "-" : widget.ToString ();
			
			this.drop_adorner.Widget = widget;
			this.drop_adorner.HiliteMode = Design.HiliteMode.DropCandidate;
			this.drop_adorner.Path.Clear ();
			
			if (widget != null)
			{
				Design.Constraint cx = new Design.Constraint (5);
				Design.Constraint cy = new Design.Constraint (5);
				
				Design.SmartGuide guide  = new Design.SmartGuide (this.Widget, Drawing.GripId.Body, this.DropTarget);
				Drawing.Rectangle bounds = this.DropTargetBounds;
				
				guide.Constrain (bounds, this.DropTargetBaseLineOffset, cx, cy);
				
				if (cx.Segments.Length > 0)
				{
					for (int i = 0; i < cx.Segments.Length; i++)
					{
						this.drop_adorner.Path.MoveTo (cx.Segments[i].P1);
						this.drop_adorner.Path.LineTo (cx.Segments[i].P2);
					}
				}
				
				if (cy.Segments.Length > 0)
				{
					for (int i = 0; i < cy.Segments.Length; i++)
					{
						this.drop_adorner.Path.MoveTo (cy.Segments[i].P1);
						this.drop_adorner.Path.LineTo (cy.Segments[i].P2);
					}
				}
				
				if (! cx.Equals (this.drop_cx))
				{
					this.drop_cx = cx.Clone ();
					this.DropTarget.Invalidate ();
				}
				if (! cy.Equals (this.drop_cy))
				{
					this.drop_cy = cy.Clone ();
					this.DropTarget.Invalidate ();
				}
			}
			
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		void Helpers.IDragBehaviorHost.OnDragEnd()
		{
			if (this.drop_adorner.Widget != null)
			{
				Support.ObjectBundler bundler = new Support.ObjectBundler ();
				Drawing.Point         offset  = new Drawing.Point (this.widget_margins.Left, this.widget_margins.Bottom);
				Widget                copy    = bundler.CopyObject (this.widget) as Widget;
				AnchorStyles          anchor  = AnchorStyles.None;
				
				if (this.drop_cx.IsValid)
				{
					offset.X -= this.drop_cx.Distance;
					anchor   |= Design.Constraint.Hint.MergedAnchor (this.drop_cx.Hints);
				}
				
				if (this.drop_cy.IsValid)
				{
					offset.Y -= this.drop_cy.Distance;
					anchor   |= Design.Constraint.Hint.MergedAnchor (this.drop_cy.Hints);
				}
				
				if ((anchor & AnchorStyles.LeftAndRight) == 0)
				{
					anchor |= AnchorStyles.Left;
				}
				
				if ((anchor & AnchorStyles.TopAndBottom) == 0)
				{
					anchor |= AnchorStyles.Top;
				}
				
				copy.Location = this.drop_adorner.Widget.MapScreenToClient (this.DragLocation) + offset;
				copy.Dock     = DockStyle.None;
				copy.Anchor   = anchor;
				
				this.drop_adorner.Widget.Children.Add (copy);
				this.drop_adorner.Widget = null;
			}
			
			this.drag_window.Hide ();
			this.drag_window.Dispose ();
			this.drag_window = null;
		}
		#endregion
		
		public event DragEventHandler		Dragging;
		
		
		protected Widget					widget;
		protected Drawing.Margins			widget_margins;
		protected Drawing.Point				drag_cursor;
		protected DragWindow				drag_window;
		protected Helpers.DragBehavior		drag_behavior;
		protected Design.HiliteAdorner		drop_adorner;
		protected Design.Constraint			drop_cx;
		protected Design.Constraint			drop_cy;
	}
}
