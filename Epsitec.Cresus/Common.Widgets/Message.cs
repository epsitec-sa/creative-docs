namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message
	{
		public Message()
		{
			this.tick_count = System.Environment.TickCount;
			
			Message.state.buttons   = (MouseButtons) (int) System.Windows.Forms.Control.MouseButtons;
			Message.state.modifiers = (ModifierKeys) (int) System.Windows.Forms.Control.ModifierKeys;
			
			this.modifiers = Message.state.modifiers;
			
			this.cursor = Message.state.cursor;
			this.button = MouseButtons.None;
		}
		
		
		public bool							FilterNoChildren
		{
			get { return this.filter_no_children; }
		}
		
		public bool							FilterOnlyFocused
		{
			get { return this.filter_only_focused; }
		}
		
		public bool							FilterOnlyOnHit
		{
			get { return this.filter_only_on_hit; }
		}
		
		public bool							Handled
		{
			get { return this.is_handled; }
			set { this.is_handled = value; }
		}
		
		
		public MessageType					Type
		{
			get { return this.type; }
		}
		
		public int							TickCount
		{
			get { return this.tick_count; }
		}
		
		public static MessageState			State
		{
			get { return Message.state; }
		}
		
		
		public Drawing.Point				Cursor
		{
			get { return this.cursor; }
		}
		
		public double						X
		{
			get { return this.cursor.X; }
		}

		public double						Y
		{
			get { return this.cursor.Y; }
		}

		public int							Wheel
		{
			get { return this.wheel; }
		}
		
		public MouseButtons					Button
		{
			get { return this.button; }
		}
		
		public int							ButtonDownCount
		{
			get { return this.button_down_count; }
		}
		
		public ModifierKeys					ModifierKeys
		{
			get { return this.modifiers; }
		}
		
		public int							KeyChar
		{
			get { return this.key_char; }
		}
		
		public int							KeyCode
		{
			get { return this.key_code; }
		}
		
		
		public bool							IsShiftPressed
		{
			get { return (this.modifiers & ModifierKeys.Shift) != 0; }
		}
		
		public bool							IsCtrlPressed
		{
			get { return (this.modifiers & ModifierKeys.Ctrl) != 0; }
		}
		
		public bool							IsAltPressed
		{
			get { return (this.modifiers & ModifierKeys.Alt) != 0; }
		}
		
		
		internal static Message FromMouseEvent(MessageType type, System.Windows.Forms.MouseEventArgs e)
		{
			Message message = new Message ();
			
			message.type = type;
			
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			if (e != null)
			{
				message.cursor = new Drawing.Point (e.X, e.Y);
				message.button = (MouseButtons) (int) e.Button;
				message.wheel  = e.Delta;
				
				Message.state.cursor = message.cursor;
			}
			
			//	Gère les clics multiples, en tenant compte des réglages de l'utilisateur.
			
			if (type == MessageType.MouseDown)
			{
				System.Diagnostics.Debug.Assert (e != null);
				System.Diagnostics.Debug.Assert (e.Button != System.Windows.Forms.MouseButtons.None);
				
				int time_new   = message.TickCount;
				int time_delta = time_new - Message.state.button_down_time;
				int time_max   = System.Windows.Forms.SystemInformation.DoubleClickTime;
				int down_count = 1;
				
				if ((time_delta < time_max) && (time_delta > 0) && (Message.state.button_down_id == message.button))
				{
					System.Drawing.Size offset_max = System.Windows.Forms.SystemInformation.DoubleClickSize;
					
					int max_dx  = offset_max.Width;
					int max_dy  = offset_max.Height;
					int max_dr2 = max_dx*max_dx + max_dy*max_dy;
					
					int dx  = e.X - Message.state.button_down_x;
					int dy  = e.Y - Message.state.button_down_y;
					int dr2 = dx*dx + dy*dy;
					
					if (dr2 <= max_dr2)
					{
						down_count = Message.state.button_down_count + 1;
					}
				}
				
				Message.state.button_down_time  = time_new;
				Message.state.button_down_x     = e.X;
				Message.state.button_down_y     = e.Y;
				Message.state.button_down_count = down_count;
				Message.state.button_down_id    = message.button;
			}
			
			if ((type == MessageType.MouseDown) ||
				(type == MessageType.MouseUp))
			{
				message.button_down_count = Message.state.button_down_count;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(MessageType type, System.Windows.Forms.KeyEventArgs e)
		{
			Message message = new Message ();
			
			message.type     = type;
			message.key_code = (int) e.KeyCode;
			
			message.filter_no_children  = false;
			message.filter_only_focused = true;
			message.filter_only_on_hit  = false;
			
			if (type == MessageType.KeyDown)
			{
				Message.state.key_down_code = e.KeyValue;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(MessageType type, System.Windows.Forms.KeyPressEventArgs e)
		{
			Message message = new Message ();
			
			message.type     = type;
			message.key_char = e.KeyChar;
			message.key_code = Message.state.key_down_code;
			
			message.filter_no_children  = false;
			message.filter_only_focused = true;
			message.filter_only_on_hit  = false;
			
			return message;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.type.ToString ());
			buffer.Append (" ");
			buffer.Append (this.cursor.ToString ());
			
			if (this.button != MouseButtons.None)
			{
				buffer.Append (" ");
				buffer.Append (this.button.ToString ());
				buffer.Append (" (");
				buffer.Append (this.button_down_count.ToString ());
				buffer.Append (")");
			}
			
			if (this.modifiers != ModifierKeys.None)
			{
				buffer.Append (" ");
				buffer.Append (this.modifiers.ToString ());
			}
			
			if (this.key_char != 0)
			{
				buffer.Append (" char=");
				buffer.Append (this.key_char.ToString ());
			}
			
			return buffer.ToString ();
		}

		
		protected bool						filter_no_children;
		protected bool						filter_only_focused;
		protected bool						filter_only_on_hit;
		protected bool						is_handled;
		
		protected MessageType				type;
		protected int						tick_count;
		protected Drawing.Point				cursor;
		
		protected MouseButtons				button;
		protected int						button_down_count;
		protected int						wheel;
		
		protected ModifierKeys				modifiers;
		protected int						key_code;
		protected int						key_char;
		
		protected static MessageState		state;
		
	}
	
	public struct MessageState
	{
		public MouseButtons					Buttons
		{
			get { return this.buttons; }
		}
		
		public ModifierKeys					ModifierKeys
		{
			get { return this.modifiers; }
		}
		
		public bool							IsShiftPressed
		{
			get { return (this.modifiers & ModifierKeys.Shift) != 0; }
		}
		
		public bool							IsCtrlPressed
		{
			get { return (this.modifiers & ModifierKeys.Ctrl) != 0; }
		}
		
		public bool							IsAltPressed
		{
			get { return (this.modifiers & ModifierKeys.Alt) != 0; }
		}
		
		
		internal ModifierKeys				modifiers;
		internal int						key_down_code;
		internal MouseButtons				buttons;
		internal MouseButtons				button_down_id;
		internal int						button_down_count;
		internal int						button_down_time;
		internal int						button_down_x;
		internal int						button_down_y;
		internal Drawing.Point				cursor;
	}
	
	
	public enum MessageType
	{
		None,
		
		MouseEnter,
		MouseLeave,
		MouseMove,
		MouseHover,
		MouseDown,
		MouseUp,
		MouseWheel,
		
		KeyDown,
		KeyUp,
		KeyPress,
	}

	
	[System.Flags] public enum MouseButtons
	{
		None			= 0,
		
		Left			= 0x00100000,
		Right			= 0x00200000,
		Middle			= 0x00400000,
		XButton1		= 0x00800000,
		XButton2		= 0x01000000
	}
	
	[System.Flags] public enum ModifierKeys
	{
		None			= 0,
		
		Shift			= 0x00010000,
		Ctrl			= 0x00020000,
		Alt				= 0x00040000,
	}
}
