namespace Epsitec.Common.Widgets
{
	public delegate void MessageHandler(object sender, Message message);
	
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
			set { this.filter_no_children = value; }
		}
		
		public bool							FilterOnlyFocused
		{
			get { return this.filter_only_focused; }
		}
		
		public bool							FilterOnlyOnHit
		{
			get { return this.filter_only_on_hit; }
		}
		
		
		public bool							Captured
		{
			get { return this.is_captured; }
			set { this.is_captured = value; }
		}
		
		public bool							Handled
		{
			get { return this.is_handled; }
			set { this.is_handled = value; }
		}
		
		public bool							Swallowed
		{
			get { return this.is_swallowed; }
			set { this.is_swallowed = value; }
		}
		
		public bool							NonClient
		{
			get { return this.is_non_client; }
		}
		
		public Widget						Consumer
		{
			get { return this.consumer; }
			set
			{
				this.consumer   = value;
				this.is_handled = (value == null) ? false : true;
			}
		}
		
		public Widget						InWidget
		{
			get { return this.in_widget; }
			set { this.in_widget = value; }
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
		
		public bool							IsLeftButton
		{
			get { return (this.button & MouseButtons.Left) != 0; }
		}
		
		public bool							IsRightButton
		{
			get { return (this.button & MouseButtons.Right) != 0; }
		}
		
		public bool							IsMiddleButton
		{
			get { return (this.button & MouseButtons.Middle) != 0; }
		}
		
		public bool							IsXButton1
		{
			get { return (this.button & MouseButtons.XButton1) != 0; }
		}
		
		public bool							IsXButton2
		{
			get { return (this.button & MouseButtons.XButton2) != 0; }
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
		
		public bool							IsMouseType
		{
			get
			{
				switch (this.type)
				{
					case MessageType.MouseDown:
					case MessageType.MouseEnter:
					case MessageType.MouseHover:
					case MessageType.MouseLeave:
					case MessageType.MouseMove:
					case MessageType.MouseUp:
					case MessageType.MouseWheel:
						return true;
				}
				
				return false;
			}
		}
		
		public bool							IsKeyType
		{
			get
			{
				switch (this.type)
				{
					case MessageType.KeyDown:
					case MessageType.KeyUp:
					case MessageType.KeyPress:
						return true;
				}
				
				return false;
			}
		}
		
		
		
		internal static System.Windows.Forms.MouseButtons ButtonsFromWParam(System.IntPtr w_param)
		{
			System.Windows.Forms.MouseButtons buttons = System.Windows.Forms.MouseButtons.None;
			int wp = (int) w_param;
			
			if ((wp & Win32Const.MK_LBUTTON) != 0)
			{
				buttons |= System.Windows.Forms.MouseButtons.Left;
			}
			if ((wp & Win32Const.MK_RBUTTON) != 0)
			{
				buttons |= System.Windows.Forms.MouseButtons.Right;
			}
			if ((wp & Win32Const.MK_MBUTTON) != 0)
			{
				buttons |= System.Windows.Forms.MouseButtons.Middle;
			}
			if ((wp & Win32Const.MK_XBUTTON1) != 0)
			{
				buttons |= System.Windows.Forms.MouseButtons.XButton1;
			}
			if ((wp & Win32Const.MK_XBUTTON2) != 0)
			{
				buttons |= System.Windows.Forms.MouseButtons.XButton2;
			}
			
			return buttons;
		}
		
		internal static int WheelDeltaFromWParam(System.IntPtr w_param)
		{
			int wp = (int) w_param;
			return (short)((wp >> 16) & 0x0000ffff);
		}
		
		internal static void XYFromLParam(System.IntPtr l_param, out int x, out int y)
		{
			x = (short)((((int)l_param) >>  0) & 0x0000ffff);
			y = (short)((((int)l_param) >> 16) & 0x0000ffff);
		}
		
		internal static bool IsMouseMsg(System.Windows.Forms.Message msg)
		{
			switch (msg.Msg)
			{
				case Win32Const.WM_LBUTTONDOWN:
				case Win32Const.WM_LBUTTONDBLCLK:
				case Win32Const.WM_LBUTTONUP:
				case Win32Const.WM_NCLBUTTONDOWN:
				
				case Win32Const.WM_RBUTTONDOWN:
				case Win32Const.WM_RBUTTONDBLCLK:
				case Win32Const.WM_RBUTTONUP:
				case Win32Const.WM_NCRBUTTONDOWN:
				
				case Win32Const.WM_MBUTTONDOWN:
				case Win32Const.WM_MBUTTONDBLCLK:
				case Win32Const.WM_MBUTTONUP:
				case Win32Const.WM_NCMBUTTONDOWN:
				
				case Win32Const.WM_XBUTTONDOWN:
				case Win32Const.WM_XBUTTONDBLCLK:
				case Win32Const.WM_XBUTTONUP:
				case Win32Const.WM_NCXBUTTONDOWN:
				
				case Win32Const.WM_MOUSEMOVE:
				case Win32Const.WM_MOUSEWHEEL:
				
				case Win32Const.WM_MOUSELEAVE:
					return true;
			}
			
			return false;
		}
		
		internal static System.Windows.Forms.MouseButtons ButtonFromMsg(System.Windows.Forms.Message msg)
		{
			switch (msg.Msg)
			{
				case Win32Const.WM_LBUTTONDOWN:
				case Win32Const.WM_LBUTTONDBLCLK:
				case Win32Const.WM_LBUTTONUP:
				case Win32Const.WM_NCLBUTTONDOWN:
					return System.Windows.Forms.MouseButtons.Left;
				
				case Win32Const.WM_RBUTTONDOWN:
				case Win32Const.WM_RBUTTONDBLCLK:
				case Win32Const.WM_RBUTTONUP:
				case Win32Const.WM_NCRBUTTONDOWN:
					return System.Windows.Forms.MouseButtons.Right;
				
				case Win32Const.WM_MBUTTONDOWN:
				case Win32Const.WM_MBUTTONDBLCLK:
				case Win32Const.WM_MBUTTONUP:
				case Win32Const.WM_NCMBUTTONDOWN:
					return System.Windows.Forms.MouseButtons.Middle;
				
				case Win32Const.WM_XBUTTONDOWN:
				case Win32Const.WM_XBUTTONDBLCLK:
				case Win32Const.WM_XBUTTONUP:
				case Win32Const.WM_NCXBUTTONDOWN:
					int w_param = (int) msg.WParam;
					switch (w_param & 0x00ff0000)
					{
						case 1:	return System.Windows.Forms.MouseButtons.XButton1;
						case 2:	return System.Windows.Forms.MouseButtons.XButton2;
					}
					break;
			}
			
			return System.Windows.Forms.MouseButtons.None;
		}
		
		internal static Message FromWndProcMessage(System.Windows.Forms.Form form, ref System.Windows.Forms.Message msg)
		{
			Message message = null;
			System.Windows.Forms.MouseButtons buttons;
			
			int x;
			int y;
			int wheel;
			
			switch (msg.Msg)
			{
				case Win32Const.WM_KEYDOWN:
				case Win32Const.WM_KEYUP:
				case Win32Const.WM_CHAR:
					//	TODO: g�n�re le message clavier
					break;
				
				case Win32Const.WM_MOUSEMOVE:
					Message.XYFromLParam (msg.LParam, out x, out y);
					buttons = Message.ButtonsFromWParam (msg.WParam);
					message = Message.FromMouseEvent (MessageType.MouseMove, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, 0));
					break;
				
				case Win32Const.WM_NCLBUTTONDOWN:
				case Win32Const.WM_NCRBUTTONDOWN:
				case Win32Const.WM_NCMBUTTONDOWN:
				case Win32Const.WM_NCXBUTTONDOWN:
					
					//	Sp�cial : �v�nement bouton press� dans la partie non-client (barre de titre ou cadre).
					//	En principe, l'application ne doit pas traiter cet �v�nement !
					
					Message.XYFromLParam (msg.LParam, out x, out y);
					System.Drawing.Point pt = form.PointToClient (new System.Drawing.Point (x, y));
					x = pt.X;
					y = pt.Y;
					buttons = Message.ButtonFromMsg (msg);
					message = Message.FromMouseEvent (MessageType.MouseDown, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, 0));
					message.is_non_client = true;
					break;
				
				case Win32Const.WM_LBUTTONDOWN:
				case Win32Const.WM_RBUTTONDOWN:
				case Win32Const.WM_MBUTTONDOWN:
				case Win32Const.WM_XBUTTONDOWN:
				case Win32Const.WM_LBUTTONDBLCLK:
				case Win32Const.WM_RBUTTONDBLCLK:
				case Win32Const.WM_MBUTTONDBLCLK:
				case Win32Const.WM_XBUTTONDBLCLK:
					Message.XYFromLParam (msg.LParam, out x, out y);
					buttons = Message.ButtonFromMsg (msg);
					message = Message.FromMouseEvent (MessageType.MouseDown, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, 0));
					break;
				
				case Win32Const.WM_LBUTTONUP:
				case Win32Const.WM_RBUTTONUP:
				case Win32Const.WM_MBUTTONUP:
				case Win32Const.WM_XBUTTONUP:
					Message.XYFromLParam (msg.LParam, out x, out y);
					buttons = Message.ButtonFromMsg (msg);
					message = Message.FromMouseEvent (MessageType.MouseUp, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, 0));
					break;
				
				case Win32Const.WM_MOUSEWHEEL:
					System.Drawing.Point point = form.PointToClient (System.Windows.Forms.Control.MousePosition);
					x = point.X;
					y = point.Y;
					buttons = Message.ButtonsFromWParam (msg.WParam);
					wheel   = Message.WheelDeltaFromWParam (msg.WParam);
					message = Message.FromMouseEvent (MessageType.MouseWheel, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, wheel));
					break;
				
				case Win32Const.WM_MOUSELEAVE:
					message = new Message ();
					
					message.type   = MessageType.MouseLeave;
					message.cursor = Message.state.cursor;
					break;
			}
			
			return message;
		}
		
		internal static Message FromMouseEvent(MessageType type, System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
		{
			Message message = new Message ();
			
			message.type = type;
			
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			int x = 0;
			int y = 0;
			
			if (e != null)
			{
				x = e.X;
				y = form.ClientSize.Height - e.Y - 1;
				
				message.cursor = new Drawing.Point (x, y);
				message.button = (MouseButtons) (int) e.Button;
				message.wheel  = e.Delta;
				
				Message.state.cursor = message.cursor;
			}
			
			//	G�re les clics multiples, en tenant compte des r�glages de l'utilisateur.
			
			if (type == MessageType.MouseDown)
			{
				System.Diagnostics.Debug.Assert (e != null);
				System.Diagnostics.Debug.Assert (e.Button != System.Windows.Forms.MouseButtons.None);
				
				int time_new   = message.TickCount;
				int time_delta = time_new - Message.state.button_down_time;
				int time_max   = SystemInformation.DoubleClickDelay;
				int down_count = 1;
				
				if ((time_delta < time_max) && (time_delta > 0) && (Message.state.button_down_id == message.button))
				{
					int max_dr2 = SystemInformation.DoubleClickRadius2;
					
					int dx  = x - Message.state.button_down_x;
					int dy  = y - Message.state.button_down_y;
					int dr2 = dx*dx + dy*dy;
					
					if (dr2 <= max_dr2)
					{
						down_count = Message.state.button_down_count + 1;
					}
				}
				
				Message.state.button_down_time  = time_new;
				Message.state.button_down_x     = x;
				Message.state.button_down_y     = y;
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
			
			if (e.Alt)
			{
				message.modifiers |= ModifierKeys.Alt;
			}
			
			if (e.Shift)
			{
				message.modifiers |= ModifierKeys.Shift;
			}
			
			if (e.Control)
			{
				message.modifiers |= ModifierKeys.Ctrl;
			}
			
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
			
			buffer.Append ("{");
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
			
			if (this.in_widget != null)
			{
				buffer.Append (" in='");
				buffer.Append (this.in_widget.Name);
				buffer.Append ("'");
			}
			
			if (this.wheel != 0)
			{
				buffer.Append (" wheel=");
				buffer.Append (this.wheel.ToString ());
			}
			
			buffer.Append ("}");
			return buffer.ToString ();
		}

		
		protected bool						filter_no_children;
		protected bool						filter_only_focused;
		protected bool						filter_only_on_hit;
		protected bool						is_captured;
		protected bool						is_handled;
		protected bool						is_non_client;
		protected bool						is_swallowed;
		protected Widget					in_widget;
		protected Widget					consumer;
		
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
		
		public bool							IsLeftButton
		{
			get { return (this.buttons & MouseButtons.Left) != 0; }
		}
		
		public bool							IsRightButton
		{
			get { return (this.buttons & MouseButtons.Right) != 0; }
		}
		
		public bool							IsMiddleButton
		{
			get { return (this.buttons & MouseButtons.Middle) != 0; }
		}
		
		public bool							IsXButton1
		{
			get { return (this.buttons & MouseButtons.XButton1) != 0; }
		}
		
		public bool							IsXButton2
		{
			get { return (this.buttons & MouseButtons.XButton2) != 0; }
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
		
		public Drawing.Point				LastPosition
		{
			get { return this.cursor; }
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
