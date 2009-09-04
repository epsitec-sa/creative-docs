//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
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
		
		public DragBehavior(IDragBehaviorHost host, Widget widget, bool isRelative, bool isZeroBased)
		{
			this.host          = host;
			this.widget        = widget;
			this.isRelative   = isRelative;
			this.isZeroBased = isZeroBased;
		}
		
		public DragBehavior(Widget widget) : this(widget as IDragBehaviorHost, widget)
		{
		}
		
		public DragBehavior(Widget widget, bool isRelative, bool isZeroBased) : this(widget as IDragBehaviorHost, widget, isRelative, isZeroBased)
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
			get { return this.isDragging; }
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.widget.IsEnabled)
			{
				switch (message.MessageType)
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
						if (Message.CurrentState.Buttons == MouseButtons.Left)
						{
							this.HandleDragging (message, pos);
						}
						break;
					
					case MessageType.KeyDown:
					case MessageType.KeyUp:
						if ((message.KeyCode == KeyCode.AltKey) ||
							(message.KeyCode == KeyCode.ShiftKey) ||
							(message.KeyCode == KeyCode.ControlKey))
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
			if (this.isRelative)
			{
				if (this.host.OnDragBegin (pos) == false)
				{
					return;
				}

				this.dragOffset = pos - this.host.DragLocation;
			}
			else
			{
				if (this.host.OnDragBegin (message.Cursor) == false)
				{
					return;
				}
				
				this.dragOffset = message.Cursor - this.host.DragLocation;
			}
			
			message.Captured = true;
			message.Consumer = this.widget;
			
			this.isDragging = true;
		}
		
		protected void StopDragging(Message message, Drawing.Point pos)
		{
			if (this.isDragging)
			{
				message.Consumer = this.widget;
				
				this.isDragging = false;
				
				this.host.OnDragEnd ();
			}
		}
		
		protected void HandleDragging(Message message, Drawing.Point pos)
		{
			if (this.isDragging)
			{
				message.Consumer = this.widget;
				
				if (this.isRelative)
				{
					Drawing.Point oldPos = this.host.DragLocation;
					Drawing.Point newPos = this.isZeroBased ? pos : pos - this.dragOffset;
					
					this.host.OnDragging (new DragEventArgs (oldPos, newPos));
				}
				else
				{
					Drawing.Point oldPos = this.host.DragLocation;
					Drawing.Point newPos = this.isZeroBased ? message.Cursor : message.Cursor - this.dragOffset;
					
					this.host.OnDragging (new DragEventArgs (oldPos, newPos));
				}
			}
		}
		
		
		protected IDragBehaviorHost			host;
		protected Widget					widget;
		
		protected bool						isDragging;
		protected bool						isRelative;
		protected bool						isZeroBased;
		
		protected Drawing.Point				dragOffset;
	}
}
