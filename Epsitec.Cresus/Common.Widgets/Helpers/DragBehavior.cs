namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe DragBehavior implémente le comportement de dragging, à savoir
	/// détecter la condition de début de drag, gérer le déplacement et terminer
	/// le drag.
	/// </summary>
	public class DragBehavior
	{
		public DragBehavior(IDragBehaviorHost host, Widget widget)
		{
			this.host   = host;
			this.widget = widget;
		}
		
		public DragBehavior(Widget widget) : this(widget as IDragBehaviorHost, widget)
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
			
			return message.Consumer == null;
		}
		
		
		protected void StartDragging(Message message, Drawing.Point pos)
		{
			message.Captured = true;
			message.Consumer = this.widget;
			
			this.host.OnDragBegin ();
			
			this.is_dragging = true;
			this.drag_offset = message.Cursor - this.host.DragLocation;
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
				
				Drawing.Point old_pos = this.host.DragLocation;
				Drawing.Point new_pos = message.Cursor - this.drag_offset;
				
				this.host.OnDragging (new DragEventArgs (old_pos, new_pos));
			}
		}
		
		
		protected IDragBehaviorHost			host;
		protected Widget					widget;
		
		protected bool						is_dragging;
		protected Drawing.Point				drag_offset;
	}
}
