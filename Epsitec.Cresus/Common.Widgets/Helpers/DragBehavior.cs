namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe DragBehavior impl�mente le comportement de dragging, � savoir
	/// d�tecter la condition de d�but de drag, g�rer le d�placement et terminer
	/// le drag.
	/// </summary>
	public class DragBehavior
	{
		public DragBehavior(IDragBehaviorHost host, Widget widget) : this (host, widget, false, false)
		{
		}
		
		public DragBehavior(IDragBehaviorHost host, Widget widget, bool is_relative, bool is_zero_based)
		{
			this.host          = host;
			this.widget        = widget;
			this.is_relative   = is_relative;
			this.is_zero_based = is_zero_based;
		}
		
		public DragBehavior(Widget widget) : this(widget as IDragBehaviorHost, widget)
		{
		}
		
		public DragBehavior(Widget widget, bool is_relative, bool is_zero_based) : this(widget as IDragBehaviorHost, widget, is_relative, is_zero_based)
		{
		}
		
		
		public IDragBehaviorHost			Host
		{
			get { return this.host; }
		}
		
		public Widget						Widget
		{
			get { return this.widget; }
		}
		
		public bool							IsDragging
		{
			get { return this.is_dragging; }
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.widget.IsEnabled)
			{
				switch (message.Type)
				{
					case MessageType.MouseDown:
						if ((message.Button == MouseButtons.Left) &&
							(message.ButtonDownCount == 1))
						{
							this.StartDragging (message, pos);
							this.HandleDragging (message, pos);
						}
						break;
					
					case MessageType.MouseUp:
						if (message.Button == MouseButtons.Left)
						{
							this.StopDragging (message, pos);
						}
						break;
					
					case MessageType.MouseMove:
						if (Message.State.Buttons == MouseButtons.Left)
						{
							this.HandleDragging (message, pos);
						}
						break;
				}
			}
			
			return message.Handled;
		}
		
		
		protected void StartDragging(Message message, Drawing.Point pos)
		{
			message.Captured = true;
			message.Consumer = this.widget;
			
			if (this.is_relative)
			{
				this.host.OnDragBegin (pos);
				this.drag_offset = pos - this.host.DragLocation;
			}
			else
			{
				this.host.OnDragBegin (message.Cursor);
				this.drag_offset = message.Cursor - this.host.DragLocation;
			}
			
			this.is_dragging = true;
		}
		
		protected void StopDragging(Message message, Drawing.Point pos)
		{
			if (this.is_dragging)
			{
				message.Consumer = this.widget;
				
				this.is_dragging = false;
				
				this.host.OnDragEnd ();
			}
		}
		
		protected void HandleDragging(Message message, Drawing.Point pos)
		{
			if (this.is_dragging)
			{
				message.Consumer = this.widget;
				
				if (this.is_relative)
				{
					Drawing.Point old_pos = this.host.DragLocation;
					Drawing.Point new_pos = this.is_zero_based ? pos : pos - this.drag_offset;
					
					this.host.OnDragging (new DragEventArgs (old_pos, new_pos));
				}
				else
				{
					Drawing.Point old_pos = this.host.DragLocation;
					Drawing.Point new_pos = this.is_zero_based ? message.Cursor : message.Cursor - this.drag_offset;
					
					this.host.OnDragging (new DragEventArgs (old_pos, new_pos));
				}
			}
		}
		
		
		protected IDragBehaviorHost			host;
		protected Widget					widget;
		
		protected bool						is_dragging;
		protected bool						is_relative;
		protected bool						is_zero_based;
		
		protected Drawing.Point				drag_offset;
	}
}
