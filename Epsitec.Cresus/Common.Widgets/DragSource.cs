namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe DragSource abrite un widget qui, lorsqu'il est "dragged" vient
	/// automatiquement clôné dans un DragWindow.
	/// </summary>
	public class DragSource : Widget
	{
		public DragSource()
		{
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
		
		
		public DragWindow CreateDragWindow(Widget widget)
		{
			Support.ObjectBundler bundler = new Support.ObjectBundler ();
			
			Widget     copy   = bundler.CopyObject (widget) as Widget;
			DragWindow window = new DragWindow ();
			
			window.DefineWidget (copy, new Drawing.Margins (3, 3, 3, 3));
			
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
			if ((message.Type == MessageType.MouseDown) &&
				(message.Button == MouseButtons.Left))
			{
				this.drag_pos = pos;
				this.drag_window = this.CreateDragWindow (this.widget);
				this.drag_window.WindowLocation = this.MapClientToScreen (this.widget.Location);
				this.drag_window.Show ();
				
				message.Captured = true;
				message.Consumer = this;
			}
			else if (this.drag_window != null)
			{
				if (message.Type == MessageType.MouseMove)
				{
					message.Consumer = this;
					Drawing.Point delta = pos - this.drag_pos;
					this.drag_window.WindowLocation += delta;
					this.drag_pos = pos;
				}
				else if (message.Type == MessageType.MouseUp)
				{
					message.Consumer = this;
					this.drag_window.Hide ();
					this.drag_window.Dispose ();
					this.drag_window = null;
				}
			}
		}
		
		
		
		protected Widget					widget;
		protected DragWindow				drag_window;
		protected Drawing.Point				drag_pos;
	}
}
