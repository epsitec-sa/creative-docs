//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using Win32Api   = Epsitec.Common.Widgets.Platform.Win32Api;
	using Win32Const = Epsitec.Common.Widgets.Platform.Win32Const;
	
	public delegate void MessageHandler(object sender, Message message);
	
	/// <summary>
	/// La classe Message d�crit un �v�nement en provenance du clavier ou de
	/// la souris.
	/// </summary>
	public sealed class Message
	{
		public Message()
		{
			this.tickCount = System.Environment.TickCount;
			
			Message.state.buttons   = (MouseButtons) (int) System.Windows.Forms.Control.MouseButtons;
			Message.state.modifiers = (ModifierKeys) (int) System.Windows.Forms.Control.ModifierKeys;
			
			this.modifiers = Message.state.modifiers;
			
			this.cursor = Message.state.windowCursor;
			this.button = MouseButtons.None;

			Message.lastMessage = this;
		}

		public bool							IsDummy
		{
			get { return this.isDummyMessage; }
		}
		
		public bool							FilterNoChildren
		{
			get { return this.filterNoChildren; }
			set { this.filterNoChildren = value; }
		}
		
		public bool							FilterOnlyFocused
		{
			get { return this.filterOnlyFocused; }
		}
		
		public bool							FilterOnlyOnHit
		{
			get { return this.filterOnlyOnHit; }
		}
		
		
		public bool							Captured
		{
			get { return this.isCaptured; }
			set { this.isCaptured = value; }
		}
		
		public bool							Handled
		{
			get { return this.isHandled; }
			set { this.isHandled = value; }
		}
		
		public bool							Swallowed
		{
			get { return this.isSwallowed; }
			set
			{
				if (value)
				{
					this.isHandled = true;
					this.isSwallowed = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Message"/> is retired.
		/// A retired message won't be postprocessed in <see cref="Window.DispatchMessage"/>.
		/// </summary>
		/// <value><c>true</c> if retired; otherwise, <c>false</c>.</value>
		public bool							Retired
		{
			get
			{
				return this.isRetired;
			}
			set
			{
				this.isRetired = value;
			}
		}
		
		public bool							NonClient
		{
			get { return this.isNonClient; }
		}
		
		public Widget						Consumer
		{
			get { return this.consumer; }
			set
			{
				this.consumer   = value;
				this.isHandled = (value == null) ? false : true;
			}
		}
		
		public Widget						InWidget
		{
			get
			{
				return this.inWidget;
			}
			set
			{
				this.inWidget = value;
			}
		}

		public WindowRoot					WindowRoot
		{
			get
			{
				return this.windowRoot;
			}
			set
			{
				this.windowRoot = value;
			}
		}
		
		public bool							ForceCapture
		{
			get { return this.forceCapture; }
			set { this.forceCapture = value; }
		}

		public bool							CancelFocus
		{
			get;
			set;
		}
		
		
		public MessageType					MessageType
		{
			get { return this.type; }
		}
		
		public int							TickCount
		{
			get { return this.tickCount; }
		}
		
		public string						Command
		{
			get
			{
				return this.command;
			}
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
			get { return this.buttonDownCount; }
		}
		
		public ModifierKeys					ModifierKeys
		{
			get { return this.modifiers; }
		}
		
		public int							KeyChar
		{
			get { return this.keyChar; }
		}
		
		public KeyCode						KeyCode
		{
			get { return this.keyCode; }
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


		internal void MarkAsDummyMessage()
		{
			this.isDummyMessage = true;
		}

		public static Message CreateDummyMessage(MessageType type)
		{
			Message message = new Message ();
			
			message.type      = type;
			message.button    = Message.state.buttons;
			message.modifiers = Message.state.modifiers;

			if (message.IsMouseType)
			{
				message.filterNoChildren  = false;
				message.filterOnlyFocused = false;
				message.filterOnlyOnHit  = true;
			}
			else if (message.IsKeyType)
			{
				message.filterNoChildren  = false;
				message.filterOnlyFocused = true;
				message.filterOnlyOnHit  = false;
			}
			
			message.isDummyMessage = true;
			
			return message;
		}

		public static void ResetButtonDownCounter()
		{
			Message.state.buttonDownCount = 0;
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

		public static Message GetLastMessage()
		{
			return Message.lastMessage;
		}


		public void CopyFrom(Message other)
		{
			this.type = other.type;
			this.button = other.button;
			this.buttonDownCount = other.buttonDownCount;
			this.wheel = other.wheel;
			this.modifiers = other.modifiers;
			this.keyCode = other.keyCode;
			this.keyChar = other.keyChar;
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
				buffer.Append (this.buttonDownCount.ToString ());
				buffer.Append (")");
			}
			
			if (this.modifiers != ModifierKeys.None)
			{
				buffer.Append (" ");
				buffer.Append (this.modifiers.ToString ());
			}
			
			if (this.keyChar != 0)
			{
				buffer.Append (" char=");
				buffer.Append (this.keyChar.ToString ());
			}
			
			if (this.keyCode != 0)
			{
				buffer.Append (" code=");
				buffer.Append (this.keyCode.ToString ());
			}
			
			if (this.inWidget != null)
			{
				buffer.Append (" in='");
				buffer.Append (this.inWidget.Name);
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
		
		
		internal static System.Windows.Forms.MouseButtons ButtonsFromWParam(System.IntPtr wParam)
		{
			System.Windows.Forms.MouseButtons buttons = System.Windows.Forms.MouseButtons.None;
			int wp = (int) wParam;
			
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
		
		internal static int WheelDeltaFromWParam(System.IntPtr wParam)
		{
			int wp = (int) wParam;
			return (short)((wp >> 16) & 0x0000ffff);
		}
		
		internal static void XYFromLParam(System.IntPtr lParam, out int x, out int y)
		{
			x = (short)((((int)lParam) >>  0) & 0x0000ffff);
			y = (short)((((int)lParam) >> 16) & 0x0000ffff);
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
					int wParam = (int) msg.WParam;
					switch (wParam & 0x00ff0000)
					{
						case 0x00010000: return System.Windows.Forms.MouseButtons.XButton1;
						case 0x00020000: return System.Windows.Forms.MouseButtons.XButton2;
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
				case Win32Const.WM_APPCOMMAND:
					message = Message.FromApplicationCommand ((msg.LParam.ToInt32 () >> 16) & 0x0fff);
					break;
			
				case Win32Const.WM_KEYDOWN:
				case Win32Const.WM_SYSKEYDOWN:
				case Win32Const.WM_SYSKEYUP:
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
					
					//	Sp�cial : �v�nement bouton press� dans la partie non-client (barre de titre ou cadre).
					//	En principe, l'application ne doit pas traiter cet �v�nement !
					
					Message.XYFromLParam (msg.LParam, out x, out y);
					point = form.PointToClient (new System.Drawing.Point (x, y));
					x = point.X;
					y = point.Y;
					buttons = Message.ButtonFromMsg (msg);
					message = Message.FromMouseEvent (MessageType.MouseDown, form, new System.Windows.Forms.MouseEventArgs (buttons, 0, x, y, 0));
					message.isNonClient = true;
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
					message.cursor = Message.state.windowCursor;
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
			
			message.filterNoChildren  = false;
			message.filterOnlyFocused = false;
			message.filterOnlyOnHit  = true;
			
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
				Message.state.windowCursor = message.cursor;
				Message.state.screenCursor = Message.CurrentState.window == null ? Drawing.Point.Zero : Message.CurrentState.window.MapWindowToScreen (message.cursor);
			}
			
			//	G�re les clics multiples, en tenant compte des r�glages de l'utilisateur.
			
			if (type == MessageType.MouseDown)
			{
				System.Diagnostics.Debug.Assert (e != null);
				System.Diagnostics.Debug.Assert (e.Button != System.Windows.Forms.MouseButtons.None);
				
				Message.state.windowMouseDown = Message.state.window;
				
				int timeNew   = message.TickCount;
				int timeDelta = timeNew - Message.state.buttonDownTime;
				int timeMax   = (int)(SystemInformation.DoubleClickDelay * 1000);
				int downCount = 1;
				
				if ((timeDelta < timeMax) && (timeDelta > 0) && (Message.state.buttonDownId == message.button))
				{
					int maxDr2 = SystemInformation.DoubleClickRadius2;
					
					int dx  = x - Message.state.buttonDownX;
					int dy  = y - Message.state.buttonDownY;
					int dr2 = dx*dx + dy*dy;
					
					if (dr2 <= maxDr2)
					{
						downCount = Message.state.buttonDownCount + 1;
					}
				}
				
				Message.state.buttonDownTime  = timeNew;
				Message.state.buttonDownX     = x;
				Message.state.buttonDownY     = y;
				Message.state.buttonDownCount = downCount;
				Message.state.buttonDownId    = message.button;
			}
			
			if ((type == MessageType.MouseDown) ||
				(type == MessageType.MouseUp))
			{
				message.buttonDownCount = Message.state.buttonDownCount;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(MessageType type, System.Windows.Forms.KeyEventArgs e)
		{
			Message message = new Message ();
			
			message.type     = type;
			message.keyCode = (KeyCode) (int) e.KeyCode;
			
			message.filterNoChildren  = false;
			message.filterOnlyFocused = true;
			message.filterOnlyOnHit  = false;
			
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
				Message.state.keyDownCode = message.keyCode;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(int msg, System.IntPtr wParam, System.IntPtr lParam)
		{
			//	Synth�tise un �v�nement clavier � partir de la description de
			//	tr�s bas niveau...
			
			Message message = new Message ();
			
			switch (msg)
			{
				case Win32Const.WM_SYSKEYDOWN:	message.type = MessageType.KeyDown;		break;
				case Win32Const.WM_SYSKEYUP:	message.type = MessageType.KeyUp;		break;
				case Win32Const.WM_KEYDOWN:		message.type = MessageType.KeyDown;		break;
				case Win32Const.WM_KEYUP:		message.type = MessageType.KeyUp;		break;
				case Win32Const.WM_CHAR:		message.type = MessageType.KeyPress;	break;
			}
			
			message.filterNoChildren  = false;
			message.filterOnlyFocused = true;
			message.filterOnlyOnHit  = false;
			
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
				message.keyCode = Message.lastCode;
				message.keyChar = (int) wParam;
			}
			else
			{
				Message.lastCode = (KeyCode) (int) wParam;
				message.keyCode  = Message.lastCode;
			}
			
			return message;
		}
		
		internal static Message FromKeyEvent(MessageType type, System.Windows.Forms.KeyPressEventArgs e)
		{
			Message message = new Message ();
			
			message.type     = type;
			message.keyChar = e.KeyChar;
			message.keyCode = Message.state.keyDownCode;
			
			message.filterNoChildren  = false;
			message.filterOnlyFocused = true;
			message.filterOnlyOnHit  = false;
			
			return message;
		}
		
		internal static Message CreateDummyMouseMoveEvent()
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseMove;
			message.filterNoChildren  = false;
			message.filterOnlyFocused = false;
			message.filterOnlyOnHit  = true;
			
			return message;
		}
		
		internal static Message CreateDummyMouseMoveEvent(Drawing.Point pos)
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseMove;
			message.cursor              = pos;
			message.filterNoChildren  = false;
			message.filterOnlyFocused = false;
			message.filterOnlyOnHit  = true;
			
			return message;
		}
		
		internal static Message CreateDummyMouseUpEvent(MouseButtons button, Drawing.Point pos)
		{
			Message message = new Message ();
			
			message.type                = MessageType.MouseUp;
			message.button              = button;
			message.cursor              = pos;
			message.filterNoChildren  = false;
			message.filterOnlyFocused = false;
			message.filterOnlyOnHit  = true;
			
			return message;
		}

		internal static Message FromApplicationCommand(int cmd)
		{
			Message message = new Message ();

			message.type = MessageType.ApplicationCommand;
			
			switch (cmd)
			{
				case Win32Const.APPCOMMAND_BROWSER_BACKWARD:
					message.command = "BrowserBackward";
					break;
				case Win32Const.APPCOMMAND_BROWSER_FORWARD:
					message.command = "BrowserForward";
					break;
			}
			
			return message;
		}
		
		
		internal static void DefineLastWindow(Window window)
		{
			Message.state.window = window;
		}
		#endregion
		
		#region Message State Structure
		/// <summary>
		/// La structure State d�crit l'�tat des boutons et des touches super-
		/// shift, la derni�re position de la souris, la derni�re fen�tre visit�e,
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
				get { return this.windowCursor; }
			}
			
			public Drawing.Point				LastScreenPosition
			{
				get { return this.screenCursor; }
			}
			
			public Window						LastWindow
			{
				get { return this.window; }
			}
			
			public Window						MouseDownWindow
			{
				get { return this.windowMouseDown; }
			}
			
			public bool							IsSameWindowAsButtonDown
			{
				get { return (this.windowMouseDown == null) || (this.windowMouseDown == this.window); }
			}
			
			
			#region Internal Fields
			internal ModifierKeys				modifiers;
			internal KeyCode					keyDownCode;
			internal MouseButtons				buttons;
			internal MouseButtons				buttonDownId;
			internal int						buttonDownCount;
			internal int						buttonDownTime;
			internal int						buttonDownX;
			internal int						buttonDownY;
			internal Drawing.Point				windowCursor;
			internal Drawing.Point				screenCursor;
			internal Window						window;
			internal Window						windowMouseDown;
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
		
		
		private bool						filterNoChildren;
		private bool						filterOnlyFocused;
		private bool						filterOnlyOnHit;
		private bool						isCaptured;
		private bool						isHandled;
		private bool						isNonClient;
		private bool						isSwallowed;
		private bool						isDummyMessage;
		private bool						isRetired;
		private bool						forceCapture;
		private WindowRoot					windowRoot;
		private Widget						inWidget;
		private Widget						consumer;
		private string						command;
		
		private MessageType					type;
		private int							tickCount;
		private Drawing.Point				cursor;
		
		private MouseButtons				button;
		private int							buttonDownCount;
		private int							wheel;
		
		private ModifierKeys				modifiers;
		private KeyCode						keyCode;
		private int							keyChar;
		
		private static KeyCode				lastCode;
		private static Message				lastMessage;
		private static Message.State		state;
	}
}
