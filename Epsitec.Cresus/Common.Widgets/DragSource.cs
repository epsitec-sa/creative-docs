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
		
		public Drawing.Point				DragLocation
		{
			get
			{
				return this.drag_window.WindowLocation;
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
			this.drag_window.WindowLocation += e.Offset;
			
			if (this.Dragging != null)
			{
				this.Dragging (this, e);
			}
		}
		
		void Helpers.IDragBehaviorHost.OnDragEnd()
		{
			this.drag_window.Hide ();
			this.drag_window.Dispose ();
			this.drag_window = null;
		}
		
		
		public event DragEventHandler		Dragging;
		
		
		protected Widget					widget;
		protected Drawing.Margins			widget_margins;
		protected DragWindow				drag_window;
		protected Helpers.DragBehavior		drag_behavior;
	}
}
