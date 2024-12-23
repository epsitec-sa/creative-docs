/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/



namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>Message</c> class describes a user event, such as a key press or a
    /// mouse click.
    /// </summary>
    public sealed class Message
    {
        public Message()
        {
            this.tickCount = System.Environment.TickCount;
            this.userMessageId = Message.currentUserMessageId;

            /*            Message.state.buttons = (MouseButtons)(int)System.Windows.Forms.Control.MouseButtons;
                        Message.state.modifiers = (ModifierKeys)(int)System.Windows.Forms.Control.ModifierKeys;
            */
            this.modifiers = Message.state.modifiers;

            this.cursor = Message.state.windowCursor;
            this.button = MouseButtons.None;

            Message.lastMessage = this;
        }

        private Message(MessageType messageType)
            : this()
        {
            this.messageType = messageType;
        }

        public bool IsDummy
        {
            get { return this.isDummyMessage; }
        }

        public bool FilterNoChildren
        {
            get { return this.filterNoChildren; }
            set { this.filterNoChildren = value; }
        }

        public bool FilterOnlyFocused
        {
            get { return this.filterOnlyFocused; }
        }

        public bool FilterOnlyOnHit
        {
            get { return this.filterOnlyOnHit; }
        }

        public bool Captured
        {
            get { return this.isCaptured; }
            set { this.isCaptured = value; }
        }

        public bool Handled
        {
            get { return this.isHandled; }
            set { this.isHandled = value; }
        }

        public bool Swallowed
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
        public bool Retired
        {
            get { return this.isRetired; }
            set { this.isRetired = value; }
        }

        public bool NonClient
        {
            get { return this.isNonClient; }
        }

        public Widget Consumer
        {
            get { return this.consumer; }
            set
            {
                this.consumer = value;
                this.isHandled = (value == null) ? false : true;
            }
        }

        public Widget InWidget
        {
            get { return this.inWidget; }
            set { this.inWidget = value; }
        }

        public WindowRoot WindowRoot
        {
            get { return this.windowRoot; }
            set { this.windowRoot = value; }
        }

        public bool ForceCapture
        {
            get { return this.forceCapture; }
            set { this.forceCapture = value; }
        }

        public bool CancelFocus { get; set; }

        public MessageType MessageType
        {
            get { return this.messageType; }
        }

        public long UserMessageId
        {
            get { return this.userMessageId; }
        }

        public static long CurrentUserMessageId
        {
            get { return Message.currentUserMessageId; }
        }

        public int TickCount
        {
            get { return this.tickCount; }
        }

        public string ApplicationCommand
        {
            get { return this.command; }
        }

        public static Message.State CurrentState
        {
            get { return Message.state; }
        }

        public Drawing.Point Cursor
        {
            get { return this.cursor; }
        }

        public double X
        {
            get { return this.cursor.X; }
        }

        public double Y
        {
            get { return this.cursor.Y; }
        }

        public int Wheel
        {
            get { return this.wheel; }
        }

        public double WheelAmplitude
        {
            get
            {
                // bl-net8-cross
                /*
                int scale;

                if (
                    Win32Api.SystemParametersInfo(
                        Win32Const.SPI_GETWHEELSCROLLLINES,
                        0,
                        out scale,
                        0
                    )
                )
                {
                    return (double)(this.wheel * scale) / Win32Const.WHEEL_DELTA;
                }
                else
                {
                    return (double)(this.wheel) * 3 / Win32Const.WHEEL_DELTA;
                }
                */
                return (double)(this.wheel) * 3 / 120;
            }
        }

        public MouseButtons Button
        {
            get { return this.button; }
        }

        public bool IsLeftButton
        {
            get
            {
                return Message.state.IsSameWindowAsButtonDown
                    && (this.button & MouseButtons.Left) != 0;
            }
        }

        public bool IsRightButton
        {
            get
            {
                return Message.state.IsSameWindowAsButtonDown
                    && (this.button & MouseButtons.Right) != 0;
            }
        }

        public bool IsMiddleButton
        {
            get
            {
                return Message.state.IsSameWindowAsButtonDown
                    && (this.button & MouseButtons.Middle) != 0;
            }
        }

        public bool IsXButton1
        {
            get
            {
                return Message.state.IsSameWindowAsButtonDown
                    && (this.button & MouseButtons.XButton1) != 0;
            }
        }

        public bool IsXButton2
        {
            get
            {
                return Message.state.IsSameWindowAsButtonDown
                    && (this.button & MouseButtons.XButton2) != 0;
            }
        }

        public int ButtonDownCount
        {
            get { return this.buttonDownCount; }
        }

        public ModifierKeys ModifierKeys
        {
            get { return this.modifiers; }
        }

        public int KeyChar
        {
            get { return this.keyChar; }
        }

        public KeyCode KeyCode
        {
            get { return this.keyCode; }
        }

        public KeyCode KeyCodeOnly
        {
            get { return this.KeyCode & KeyCode.KeyCodeMask; }
        }

        public bool IsNoModifierPressed
        {
            get
            {
                return (
                        this.modifiers
                        & (ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift)
                    ) == 0;
            }
        }

        public bool IsShiftPressed
        {
            get { return (this.modifiers & ModifierKeys.Shift) != 0; }
        }

        public bool IsControlPressed
        {
            get { return (this.modifiers & ModifierKeys.Control) != 0; }
        }

        public bool IsAltPressed
        {
            get { return (this.modifiers & ModifierKeys.Alt) != 0; }
        }

        public bool IsMouseType
        {
            get
            {
                switch (this.messageType)
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

        public bool IsKeyType
        {
            get
            {
                switch (this.messageType)
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
            Message message = new Message(type);

            message.button = Message.state.buttons;
            message.modifiers = Message.state.modifiers;

            if (message.IsMouseType)
            {
                message.filterNoChildren = false;
                message.filterOnlyFocused = false;
                message.filterOnlyOnHit = true;
            }
            else if (message.IsKeyType)
            {
                message.filterNoChildren = false;
                message.filterOnlyFocused = true;
                message.filterOnlyOnHit = false;
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

            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            if ((code & KeyCode.ModifierControl) != 0)
            {
                buffer.Append(Message.GetSimpleKeyName(KeyCode.ControlKey));
                buffer.Append("+");
            }

            if ((code & KeyCode.ModifierAlt) != 0)
            {
                buffer.Append(Message.GetSimpleKeyName(KeyCode.AltKey));
                buffer.Append("+");
            }

            if ((code & KeyCode.ModifierShift) != 0)
            {
                buffer.Append(Message.GetSimpleKeyName(KeyCode.ShiftKey));
                buffer.Append("+");
            }

            buffer.Append(Message.GetSimpleKeyName(code & KeyCode.KeyCodeMask));

            return buffer.ToString();
        }

        public static Message GetLastMessage()
        {
            return Message.lastMessage;
        }

        public override string ToString()
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            buffer.Append("{");
            buffer.Append(this.messageType.ToString());
            buffer.Append(" ");
            buffer.Append(this.cursor.ToString());

            if (this.button != MouseButtons.None)
            {
                buffer.Append(" ");
                buffer.Append(this.button.ToString());
                buffer.Append(" (");
                buffer.Append(this.buttonDownCount.ToString());
                buffer.Append(")");
            }

            if (this.modifiers != ModifierKeys.None)
            {
                buffer.Append(" ");
                buffer.Append(this.modifiers.ToString());
            }

            if (this.keyChar != 0)
            {
                buffer.Append(" char=");
                buffer.Append(this.keyChar.ToString());
            }

            if (this.keyCode != 0)
            {
                buffer.Append(" code=");
                buffer.Append(this.keyCode.ToString());
            }

            if (this.inWidget != null)
            {
                buffer.Append(" in='");
                buffer.Append(this.inWidget.Name);
                buffer.Append("'");
            }

            if (this.wheel != 0)
            {
                buffer.Append(" wheel=");
                buffer.Append(this.wheel.ToString());
            }

            buffer.Append("}");
            return buffer.ToString();
        }

        #region Internal Static Methods
        internal static void ClearLastWindow()
        {
            Message.state.window = null;
        }

        internal static int WheelDeltaFromWParam(System.IntPtr wParam)
        {
            int wp = (int)wParam;
            return (short)((wp >> 16) & 0x0000ffff);
        }

        internal static void XYFromLParam(System.IntPtr lParam, out int x, out int y)
        {
            x = (short)((((int)lParam) >> 0) & 0x0000ffff);
            y = (short)((((int)lParam) >> 16) & 0x0000ffff);
        }

        internal static Message PostProcessMessage(Message message)
        {
            // bl-net8-cross maybedelete
            if (message == null)
            {
                return null;
            }

            /*            //	Simulate Alt-Left and Alt-Right when the user clicks the special
                        //	<-- and --> buttons on the mouse; let's hope that this is indeed
                        //	what the mouse buttons are configured to do !
            
                        if (
                            (message.Button == MouseButtons.XButton1)
                            || (message.Button == MouseButtons.XButton2)
                        )
                        {
                            System.Windows.Forms.Keys alt = System.Windows.Forms.Keys.Alt;
                            System.Windows.Forms.Keys key =
                                message.Button == MouseButtons.XButton1
                                    ? System.Windows.Forms.Keys.Left
                                    : System.Windows.Forms.Keys.Right;
            
                            switch (message.MessageType)
                            {
                                case MessageType.MouseDown:
                                    return Message.FromKeyEvent(
                                        MessageType.KeyDown,
                                        new System.Windows.Forms.KeyEventArgs(alt | key)
                                    );
            
                                case MessageType.MouseUp:
                                    return Message.FromKeyEvent(
                                        MessageType.KeyUp,
                                        new System.Windows.Forms.KeyEventArgs(alt | key)
                                    );
                            }
                        }
            */
            return message;
        }

        internal static Message FromMouseEvent(
            MessageType type,
            Platform.PlatformWindow window,
            MouseButtons button,
            int x,
            int y,
            int wheelDist
        )
        {
            Message message = new Message(type);

            message.filterNoChildren = false;
            message.filterOnlyFocused = false;
            message.filterOnlyOnHit = true;

            if (type == MessageType.MouseWheel)
            {
                message.cursor = Message.state.windowCursor;
                message.button = MouseButtons.None;
                message.wheel = wheelDist;
            }
            else
            {
                message.cursor = new Drawing.Point(x, y);
                message.button = button;
                message.wheel = wheelDist;

                Message.state.window = window.HostingWidgetWindow;
                Message.state.windowCursor = message.cursor;
                Message.state.screenCursor =
                    Message.CurrentState.window == null
                        ? Drawing.Point.Zero
                        : Message.CurrentState.window.WindowPointToScreenPoint(message.cursor);
            }

            //	Gère les clics multiples, en tenant compte des réglages de l'utilisateur.

            if (type == MessageType.MouseDown)
            {
                Message.state.windowMouseDown = Message.state.window;

                int timeNew = message.TickCount;
                int timeDelta = timeNew - Message.state.buttonDownTime;
                int timeMax = (int)(SystemInformation.DoubleClickDelay * 1000);
                int downCount = 1;

                if (
                    (timeDelta < timeMax)
                    && (timeDelta > 0)
                    && (Message.state.buttonDownId == message.button)
                )
                {
                    int maxDr2 = SystemInformation.DoubleClickRadius2;

                    int dx = x - Message.state.buttonDownX;
                    int dy = y - Message.state.buttonDownY;
                    int dr2 = dx * dx + dy * dy;

                    if (dr2 <= maxDr2)
                    {
                        downCount = Message.state.buttonDownCount + 1;
                    }
                }

                Message.state.buttonDownTime = timeNew;
                Message.state.buttonDownX = x;
                Message.state.buttonDownY = y;
                Message.state.buttonDownCount = downCount;
                Message.state.buttonDownId = message.button;
            }

            if (type == MessageType.MouseUp)
            {
                if (Message.state.buttonDownCount == 0)
                {
                    //	The button was pressed in a window not controlled by this framework
                    //	(e.g. in a native "Open File" dialog) and we are getting the residual
                    //	mouse release, after a double-click which closed the dialog, for
                    //	instance.
                }
            }

            if ((type == MessageType.MouseDown) || (type == MessageType.MouseUp))
            {
                message.buttonDownCount = Message.state.buttonDownCount;
            }

            return message;
        }

        internal static Message FromKeyEvent(
            MessageType type,
            KeyCode keyCode,
            ModifierKeys modifiers
        )
        {
            Message message = new Message(type);

            message.keyCode = keyCode;

            message.filterNoChildren = false;
            message.filterOnlyFocused = true;
            message.filterOnlyOnHit = false;

            message.modifiers = modifiers;

            if (type == MessageType.KeyDown)
            {
                Message.state.keyDownCode = message.keyCode;
            }

            return message;
        }

        internal static Message FromKeyEvent(int msg, System.IntPtr wParam, System.IntPtr lParam)
        {
            MessageType messageType = MessageType.None;
            Message message = new Message(messageType);
            /*            //	Synthétise un événement clavier à partir de la description de
                        //	très bas niveau...
            
                        MessageType messageType = MessageType.None;
            
                        switch (msg)
                        {
                            case Win32Const.WM_SYSKEYDOWN:
                                messageType = MessageType.KeyDown;
                                break;
                            case Win32Const.WM_SYSKEYUP:
                                messageType = MessageType.KeyUp;
                                break;
                            case Win32Const.WM_KEYDOWN:
                                messageType = MessageType.KeyDown;
                                break;
                            case Win32Const.WM_KEYUP:
                                messageType = MessageType.KeyUp;
                                break;
                            case Win32Const.WM_CHAR:
                                messageType = MessageType.KeyPress;
                                break;
                        }
            
                        Message message = new Message(messageType);
            
                        message.filterNoChildren = false;
                        message.filterOnlyFocused = true;
                        message.filterOnlyOnHit = false;
            
                        if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Alt) != 0)
                        {
                            message.modifiers |= ModifierKeys.Alt;
                        }
            
                        if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != 0)
                        {
                            message.modifiers |= ModifierKeys.Shift;
                        }
            
                        if (
                            (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) != 0
                        )
                        {
                            message.modifiers |= ModifierKeys.Control;
                        }
            
                        int rawKeyChar = (int)wParam;
                        KeyCode rawKeyCode = (KeyCode)rawKeyChar;
                        int rawLParam = (int)lParam;
            
                        if ((rawLParam & (1 << 24)) != 0)
                        {
                            //	This is an extended key which might require some remapping to get the real.
            
                            switch (rawKeyCode)
                            {
                                case KeyCode.Return:
                                    rawKeyCode = KeyCode.NumericEnter;
                                    break;
            
                                default:
                                    break;
                            }
                        }
            
                        if (message.messageType == MessageType.KeyPress)
                        {
                            message.keyCode = Message.lastCode;
                            message.keyChar = rawKeyChar;
                        }
                        else
                        {
                            Message.lastCode = rawKeyCode;
                            message.keyCode = Message.lastCode;
                        }
            */
            throw new System.NotImplementedException();
            return message;
        }

        internal static Message CreateDummyMouseMoveEvent()
        {
            Message message = new Message(MessageType.MouseMove);

            message.filterNoChildren = false;
            message.filterOnlyFocused = false;
            message.filterOnlyOnHit = true;

            return message;
        }

        internal static Message CreateDummyMouseMoveEvent(Drawing.Point pos)
        {
            Message message = new Message(MessageType.MouseMove);

            message.cursor = pos;
            message.filterNoChildren = false;
            message.filterOnlyFocused = false;
            message.filterOnlyOnHit = true;

            return message;
        }

        internal static Message CreateDummyMouseUpEvent(MouseButtons button, Drawing.Point pos)
        {
            Message message = new Message(MessageType.MouseUp);

            message.button = button;
            message.cursor = pos;
            message.filterNoChildren = false;
            message.filterOnlyFocused = false;
            message.filterOnlyOnHit = true;

            return message;
        }

        internal static Message FromApplicationCommand(int cmd)
        {
            /*
            Message message = new Message(MessageType.ApplicationCommand);

            switch (cmd)
            {
                case Win32Const.APPCOMMAND_BROWSER_BACKWARD:
                    message.command = ApplicationCommands.BrowserBackward;
                    break;

                case Win32Const.APPCOMMAND_BROWSER_FORWARD:
                    message.command = ApplicationCommands.BrowserForward;
                    break;
            }

            return message;
            */
            throw new System.NotImplementedException();
        }

        public static class ApplicationCommands
        {
            public const string BrowserBackward = "BrowserBackward";
            public const string BrowserForward = "BrowserForward";
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
            public MouseButtons Buttons
            {
                get { return this.buttons; }
            }

            public int ButtonDownCount
            {
                get { return this.buttonDownCount; }
            }

            public ModifierKeys ModifierKeys
            {
                get { return this.modifiers; }
            }

            public bool IsLeftButton
            {
                get { return (this.buttons & MouseButtons.Left) != 0; }
            }

            public bool IsRightButton
            {
                get { return (this.buttons & MouseButtons.Right) != 0; }
            }

            public bool IsMiddleButton
            {
                get { return (this.buttons & MouseButtons.Middle) != 0; }
            }

            public bool IsXButton1
            {
                get { return (this.buttons & MouseButtons.XButton1) != 0; }
            }

            public bool IsXButton2
            {
                get { return (this.buttons & MouseButtons.XButton2) != 0; }
            }

            public bool IsShiftPressed
            {
                get { return (this.modifiers & ModifierKeys.Shift) != 0; }
            }

            public bool IsControlPressed
            {
                get { return (this.modifiers & ModifierKeys.Control) != 0; }
            }

            public bool IsAltPressed
            {
                get { return (this.modifiers & ModifierKeys.Alt) != 0; }
            }

            public Drawing.Point LastPosition
            {
                get { return this.windowCursor; }
            }

            public Drawing.Point LastScreenPosition
            {
                get { return this.screenCursor; }
            }

            public Window LastWindow
            {
                get { return this.window; }
            }

            public Window MouseDownWindow
            {
                get { return this.windowMouseDown; }
            }

            public bool IsSameWindowAsButtonDown
            {
                get
                {
                    return (this.windowMouseDown == null) || (this.windowMouseDown == this.window);
                }
            }

            #region Internal Fields
            internal ModifierKeys modifiers;
            internal KeyCode keyDownCode;
            internal MouseButtons buttons;
            internal MouseButtons buttonDownId;
            internal int buttonDownCount;
            internal int buttonDownTime;
            internal int buttonDownX;
            internal int buttonDownY;
            internal Drawing.Point windowCursor;

            internal Drawing.Point screenCursor;
            internal Window window;
            internal Window windowMouseDown;
            #endregion
        }
        #endregion

        private static string GetSimpleKeyName(KeyCode code)
        {
            return KeyCodeHelper.ConvertToString(code);
        }

        private static KeyCode lastCode;
        private static Message lastMessage;
        private static Message.State state;
        private static long currentUserMessageId;

        private readonly MessageType messageType;
        private readonly long userMessageId;
        private readonly int tickCount;

        private bool filterNoChildren;
        private bool filterOnlyFocused;
        private bool filterOnlyOnHit;
        private bool isCaptured;
        private bool isHandled;
        private bool isNonClient;
        private bool isSwallowed;
        private bool isDummyMessage;
        private bool isRetired;
        private bool forceCapture;
        private WindowRoot windowRoot;
        private Widget inWidget;
        private Widget consumer;
        private string command;

        private Drawing.Point cursor;
        private MouseButtons button;
        private int buttonDownCount;
        private int wheel;

        private ModifierKeys modifiers;
        private KeyCode keyCode;
        private int keyChar;
    }
}
