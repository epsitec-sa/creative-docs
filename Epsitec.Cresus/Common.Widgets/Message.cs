//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using Win32Api   = Epsitec.Common.Widgets.Platform.Win32Api;
	using Win32Const = Epsitec.Common.Widgets.Platform.Win32Const;
	
	public delegate void MessageHandler(object sender, Message message);
	
	/// <summary>
	/// La classe Message décrit un événement en provenance du clavier ou de
	/// la souris.
	/// </summary>
	public sealed class Message
	{
		public Message()
		{
			this.tick_count = System.Environment.TickCount;
			
			Message.state.buttons   = (MouseButtons) (int) System.Windows.Forms.Control.MouseButtons;
			Message.state.modifiers = (ModifierKeys) (int) System.Windows.Forms.Control.ModifierKeys;
			
			this.modifiers = Message.state.modifiers;
			
			this.cursor = Message.state.window_cursor;
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
			set
			{
				if (value)
				{
					this.is_handled = true;
					this.is_swallowed = true;
				}
			}
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
			get
			{
				return this.in_widget;
			}
			set
			{
				this.in_widget = value;
			}
		}

		public WindowRoot					WindowRoot
		{
			get
			{
				return this.window_root;
			}
			set
			{
				this.window_root = value;
			}
		}
		
		public bool							ForceCapture
		{
			get { return this.force_capture; }
			set { this.force_capture = value; }
		}
		
		
		public MessageType					Type
		{
			get { return this.type; }
		}
		
		public int							TickCount
		{
			get { return this.tick_count; }
		}
		
		
		public static Message.State			CurrentState
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
			get { return Message.state.IsSameWindowAsButtonDown && (this.button & MouseButtons.Left) != 0; }
		}
		
		public bool							IsRightButton
		{
			get { return Message.state.IsSameWindowAsButtonDown && (this.button & MouseButtons.Right) != 0; }
		}
		
		public bool							IsMiddleButton
		{
			get { return Message.state.IsSameWindowAsButtonDown && (this.button & MouseButtons.Middle) != 0; }
		}
		
		public bool							IsXButton1
		{
			get { return Message.state.IsSameWindowAsButtonDown && (this.button & MouseButtons.XButton1) != 0; }
		}
		
		public bool							IsXButton2
		{
			get { return Message.state.IsSameWindowAsButtonDown && (this.button & MouseButtons.XButton2) != 0; }
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
		
		public KeyCode						KeyCode
		{
			get { return this.key_code; }
		}
		
		public KeyCode						KeyCodeOnly
		{
			get { return this.KeyCode & KeyCode.KeyCodeMask; }
		}
		
		
		public bool							IsShiftPressed
		{
			get { return (this.modifiers & ModifierKeys.Shift) != 0; }
		}
		
		public bool							IsControlPressed
		{
			get { return (this.modifiers & ModifierKeys.Control) != 0; }
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
		
		
		
		public static void ResetButtonDownCounter()
		{
			Message.state.button_down_count = 0;
		}
		
		public static string GetKeyName(KeyCode code)
		{
			if (code == 0)
			{
				return "";
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if ((code & KeyCode.ModifierControl) != 0)
			{
				buffer.Append (Message.GetSimpleKeyName (KeyCode.ControlKey));
				buffer.Append ("+");
			}
			
			if ((code & KeyCode.ModifierAlt) != 0)
			{
				buffer.Append (Message.GetSimpleKeyName (KeyCode.AltKey));
				buffer.Append ("+");
			}
			
			if ((code & KeyCode.ModifierShift) != 0)
			{
				buffer.Append (Message.GetSimpleKeyName (KeyCode.ShiftKey));
				buffer.Append ("+");
			}
			
			buffer.Append (Message.GetSimpleKeyName (code & KeyCode.KeyCodeMask));
			
			return buffer.ToString ();
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
			
			if (this.key_code != 0)
			{
				buffer.Append (" code=");
				buffer.Append (this.key_code.ToString ());
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

		
		#region Internal Static Methods
		internal static void ClearLastWindow()
		{
			Message.state.window = null;
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
		
		
		internal static Message FromWndProcMessage(Platform.Window form, ref System.Windows.Forms.Message msg)
		{
			Message message = null;
			System.Windows.Forms.MouseButtons buttons;
			
			int x;
			int y;
			int wheel;
			
			System.Drawing.Point point;
			
			switch (msg.Msg)
			{
				case Win32Const.WM_KEYDOWN:
				case Win32Const.WM_KEYUP:
				case Win32Const.WM_CHAR:
					message = Message.FromKeyEvent (msg.Msg, msg.WParam, msg.LParam);
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
					
					//	Spécial : événement bouton pressé dans la partie non-client (barre de titre ou cadre).
					//	En principe, l'application ne doit pas traiter cet événement !
					
					Message.XYFromLParam (msg.LParam, out x, out y);
					point = form.PointToClient (new System.Drawing.Point (x, y));
					x = point.X;
					y = point.Y;
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
					point = form.PointToClient (System.Windows.Forms.Control.MousePosition);
					x = point.X;
					y = point.Y;
					buttons = Message.ButtonsFromWParam (msg.WParam);
					wheel   = Message.WheelDeltaFromWParam (msg.WParam);
					message = Message.FromMouseEvent (MessageType.MouseWheel, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, wheel));
					break;
				
				case Win32Const.WM_MOUSELEAVE:
					message = new Message ();
					
					message.type   = MessageType.MouseLeave;
					message.cursor = Message.state.window_cursor;
					break;
			}
			
			if (message != null)
			{
				Message.state.window = form.HostingWidgetWindow;
			}
			
			return message;
		}
		
		internal static Message FromMouseEvent(MessageType type, Platform.Window form, System.Windows.Forms.MouseEventArgs e)
		{
			Message message = new Message ();
			
			message.type = type;
			
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			int x = 0;
			int y = 0;
			
			if (e == null)
			{
				message.button    = Message.state.buttons;
				message.modifiers = Message.state.modifiers;
			}
			else
			{
				x = e.X;
				y = form.ClientSize.Height - e.Y - 1;
				
				message.cursor = new Drawing.Point (x, y);
				message.button = (MouseButtons) (int) e.Button;
				message.wheel  = e.Delta;
				
				Message.state.window = form.HostingWidgetWindow;
				Message.state.window_cursor = message.cursor;
				Message.state.screen_cursor = Message.CurrentState.window == null ? Drawing.Point.Zero : Message.CurrentState.window.MapWindowToScreen (message.cursor);
			}
			
			//	Gère les clics multiples, en tenant compte des réglages de l'utilisateur.
			
			if (type == MessageType.MouseDown)
			{
				System.Diagnostics.Debug.Assert (e != null);
				System.Diagnostics.Debug.Assert (e.Button != System.Windows.Forms.MouseButtons.None);
				
				Message.state.window_mouse_down = Message.state.window;
				
				int time_new   = message.TickCount;
				int time_delta = time_new - Message.state.button_down_time;
				int time_max   = (int)(SystemInformation.DoubleClickDelay * 1000);
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
			message.key_code = (KeyCode) (int) e.KeyCode;
			
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
				message.modifiers |= ModifierKeys.Control;
			}
			
			if (type == MessageType.KeyDown)
			{
				Message.state.key_down_code = message.key_code;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(int msg, System.IntPtr w_param, System.IntPtr l_param)
		{
			//	Synthétise un événement clavier à partir de la description de
			//	très bas niveau...
			
			Message message = new Message ();
			
			switch (msg)
			{
				case Win32Const.WM_KEYDOWN:		message.type = MessageType.KeyDown;		break;
				case Win32Const.WM_KEYUP:		message.type = MessageType.KeyUp;		break;
				case Win32Const.WM_CHAR:		message.type = MessageType.KeyPress;	break;
			}
			
			message.filter_no_children  = false;
			message.filter_only_focused = true;
			message.filter_only_on_hit  = false;
			
			if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Alt) != 0)
			{
				message.modifiers |= ModifierKeys.Alt;
			}
			
			if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != 0)
			{
				message.modifiers |= ModifierKeys.Shift;
			}
			
			if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) != 0)
			{
				message.modifiers |= ModifierKeys.Control;
			}
			
			if (message.type == MessageType.KeyPress)
			{
				message.key_code = Message.last_code;
				message.key_char = (int) w_param;
			}
			else
			{
				Message.last_code = (KeyCode) (int) w_param;
				message.key_code  = Message.last_code;
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
		
		internal static Message CreateDummyMouseMoveEvent()
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseMove;
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			return message;
		}
		
		internal static Message CreateDummyMouseMoveEvent(Drawing.Point pos)
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseMove;
			message.cursor              = pos;
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			return message;
		}
		
		internal static Message CreateDummyMouseUpEvent(MouseButtons button, Drawing.Point pos)
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseUp;
			message.button              = button;
			message.cursor              = pos;
			message.filter_no_children  = false;
			message.filter_only_focused = false;
			message.filter_only_on_hit  = true;
			
			return message;
		}
		
		
		internal static void DefineLastWindow(Window window)
		{
			Message.state.window = window;
		}
		#endregion
		
		#region Message State Structure
		/// <summary>
		/// La structure State décrit l'état des boutons et des touches super-
		/// shift, la dernière position de la souris, la dernière fenêtre visitée,
		/// etc.
		/// </summary>
		public struct State
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
			
			public bool							IsControlPressed
			{
				get { return (this.modifiers & ModifierKeys.Control) != 0; }
			}
			
			public bool							IsAltPressed
			{
				get { return (this.modifiers & ModifierKeys.Alt) != 0; }
			}
			
			public Drawing.Point				LastPosition
			{
				get { return this.window_cursor; }
			}
			
			public Drawing.Point				LastScreenPosition
			{
				get { return this.screen_cursor; }
			}
			
			public Window						LastWindow
			{
				get { return this.window; }
			}
			
			public Window						MouseDownWindow
			{
				get { return this.window_mouse_down; }
			}
			
			public bool							IsSameWindowAsButtonDown
			{
				get { return (this.window_mouse_down == null) || (this.window_mouse_down == this.window); }
			}
			
			
			#region Internal Fields
			internal ModifierKeys				modifiers;
			internal KeyCode					key_down_code;
			internal MouseButtons				buttons;
			internal MouseButtons				button_down_id;
			internal int						button_down_count;
			internal int						button_down_time;
			internal int						button_down_x;
			internal int						button_down_y;
			internal Drawing.Point				window_cursor;
			internal Drawing.Point				screen_cursor;
			internal Window						window;
			internal Window						window_mouse_down;
			#endregion
		}
		#endregion
		
		private static string GetSimpleKeyName(KeyCode code)
		{
			string name;
			
			if (Platform.Win32Api.GetKeyName (code, out name))
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				string[] elems = name.Split ('.');
				
				for (int i = 0; i < elems.Length; i++)
				{
					string upper = elems[i];
					string lower = upper.ToLower ();
					
					if (i > 0)
					{
						buffer.Append (".");
					}
					
					if (upper.Length > 0)
					{
						buffer.Append (upper.Substring (0, 1));
						buffer.Append (lower.Substring (1));
					}
				}
				
				return buffer.ToString ();
			}
			
			name = code.ToString ();
			
			if (name.StartsWith ("Func"))
			{
				return name.Substring (4);
			}
			if (name.StartsWith ("Alpha"))
			{
				return name.Substring (5);
			}
			
			return name;
		}
		
		
		private bool						filter_no_children;
		private bool						filter_only_focused;
		private bool						filter_only_on_hit;
		private bool						is_captured;
		private bool						is_handled;
		private bool						is_non_client;
		private bool						is_swallowed;
		private bool						force_capture;
		private WindowRoot					window_root;
		private Widget						in_widget;
		private Widget						consumer;
		
		private MessageType					type;
		private int							tick_count;
		private Drawing.Point				cursor;
		
		private MouseButtons				button;
		private int							button_down_count;
		private int							wheel;
		
		private ModifierKeys				modifiers;
		private KeyCode						key_code;
		private int							key_char;
		
		private static KeyCode				last_code = 0;
		private static Message.State		state;
	}
}
