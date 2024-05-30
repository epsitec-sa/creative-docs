//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using Epsitec.Common.Drawing;
using SDL2;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe Platform.PlatformWindow fait le lien avec la SDL
    /// </summary>
    internal class PlatformWindow : SDLWrapper.SDLWindow
    {
        internal PlatformWindow(Window window, WindowFlags windowFlags)
            : base("Creativedocs", 100, 100, PlatformWindow.MapToSDLWindowFlags(windowFlags))
        {
            this.widgetWindow = window;
            this.minimumSize = new Size(1, 1);
            this.WindowType = WindowType.Document;
            WindowList.Insert(this);
        }

        #region SDLWindow overrides
        protected override void RecreateGraphicBuffer(
            IntPtr pixels,
            int width,
            int height,
            int stride
        )
        {
            if (this.renderingBuffer != null)
            {
                this.renderingBuffer.Dispose();
            }
            this.renderingBuffer = new AntigrainSharp.GraphicBufferExternalData(
                pixels,
                (uint)width,
                (uint)height,
                -stride,
                Font.FontManager
            );
        }

        protected override void OnDraw()
        {
            var graphics = new Graphics(this.renderingBuffer.GraphicContext);
            this.RefreshGraphics(graphics);
        }

        public override void OnMouseButtonDown(int x, int y, int button)
        {
            Point mouse = this.WindowPointFromSDL(x, y);
            MouseButtons btn = this.ConvertMouseButton(button);
            Message msg = Message.FromMouseEvent(
                MessageType.MouseDown,
                this,
                btn,
                (int)mouse.X,
                (int)mouse.Y,
                0
            );
            this.DispatchMessage(msg);
        }

        public override void OnMouseButtonUp(int x, int y, int button)
        {
            Point mouse = this.WindowPointFromSDL(x, y);
            MouseButtons btn = this.ConvertMouseButton(button);
            Message msg = Message.FromMouseEvent(
                MessageType.MouseUp,
                this,
                btn,
                (int)mouse.X,
                (int)mouse.Y,
                0
            );
            this.DispatchMessage(msg);
        }

        public override void OnMouseMove(int x, int y)
        {
            Point mouse = this.WindowPointFromSDL(x, y);
            MouseButtons btn = MouseButtons.None;
            Message msg = Message.FromMouseEvent(
                MessageType.MouseMove,
                this,
                btn,
                (int)mouse.X,
                (int)mouse.Y,
                0
            );
            this.DispatchMessage(msg);
        }

        public override void OnMouseWheel(int wheelX, int wheelY)
        {
            this.DispatchMessage(
                Message.FromMouseEvent(
                    MessageType.MouseWheel,
                    this,
                    MouseButtons.None,
                    0,
                    0,
                    wheelY
                )
            );
        }

        protected override void OnResize(int sx, int sy)
        {
            this.widgetWindow.OnResize(sx, sy);
        }

        public override void OnUserEvent(int eventCode)
        {
            // for now, we have only one user event type to fire the timers
            // if we need to add more of them later, we can distinguish them with the eventCode
            Timer.FirePendingEvents();
        }

        //public override void OnKey(int x, int y, uint key, AntigrainSharp.InputFlags flags)
        //{
        //    KeyCode keyCode = (KeyCode)key;
        //    ModifierKeys modifiers = ModifierKeys.None;
        //    if (flags.HasFlag(AntigrainSharp.InputFlags.KbdShift))
        //    {
        //        modifiers |= ModifierKeys.Shift;
        //    }
        //    if (flags.HasFlag(AntigrainSharp.InputFlags.KbdCtrl))
        //    {
        //        modifiers |= ModifierKeys.Control;
        //    }
        //    Message msg = Message.FromKeyEvent(MessageType.KeyPress, keyCode, modifiers);
        //    this.DispatchMessage(msg);
        //    this.ForceRedraw();
        //}

        protected override void OnWindowShown()
        {
            if (this.widgetWindow == null)
            {
                return;
            }
            this.widgetWindow.OnWindowShown();
        }

        protected override void OnWindowHidden()
        {
            if (this.widgetWindow == null)
            {
                return;
            }
            this.widgetWindow.OnWindowHidden();
        }

        protected override void OnFocusGained()
        {
            if (this.widgetWindow == null)
            {
                return;
            }
            this.widgetWindow.NotifyWindowFocused();
        }

        protected override void OnFocusLost()
        {
            if (this.widgetWindow == null)
            {
                return;
            }
            this.widgetWindow.NotifyWindowDefocused();
        }

        protected override void OnWindowClosed()
        {
            // bl-net8-cross cleanup
            //if (this.Focused)
            //{
            //    //	Si la fenêtre avait le focus et qu'on la ferme, on aimerait bien que
            //    //	si elle avait une fenêtre "parent", alors ce soit le parent qui reçoive
            //    //	le focus à son tour. Ca paraît logique.

            //    System.Windows.Forms.Form form = this.Owner;

            //    if (form != null)
            //    {
            //        form.Activate();
            //    }
            //}

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowClosed();
            }
        }

        #endregion

        #region SDL <-> Creativedocs conversions
        private static SDL.SDL_WindowFlags MapToSDLWindowFlags(WindowFlags windowFlags)
        {
            SDL.SDL_WindowFlags flags = 0;
            if (windowFlags.HasFlag(WindowFlags.NoBorder))
            {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
            }
            if (windowFlags.HasFlag(WindowFlags.Resizable))
            {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            }
            if (windowFlags.HasFlag(WindowFlags.AlwaysOnTop))
            {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP;
            }
            if (windowFlags.HasFlag(WindowFlags.HideFromTaskbar))
            {
                flags |= SDL.SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR;
            }
            return flags;
        }

        private MouseButtons ConvertMouseButton(int button)
        {
            switch (button)
            {
                case 1:
                    return MouseButtons.Left;
                case 2:
                    return MouseButtons.Middle;
                case 3:
                    return MouseButtons.Right;
                default:
                    return MouseButtons.None;
            }
        }

        private Point WindowPointFromSDL(int x, int y)
        {
            /*
            The coordinate system used by SDL is oriented like this:

               ───────────────────────────►x
              ┌────────────────────────────┐
            │ │                            │
            │ │                            │
            │ │            SDL             │
            │ │                            │
            │ │                            │
            ▼ │                            │
            y └────────────────────────────┘

            The coordinate system used by Creativedocs has the y-axis flipped:

            y ┌────────────────────────────┐
            ▲ │                            │
            │ │                            │
            │ │       Creativedocs         │
            │ │                            │
            │ │                            │
            │ │                            │
              └────────────────────────────┘
               ───────────────────────────►x
            */
            return new Point(x, this.Height - y);
        }

        public Point ScreenPointToWindowPoint(Point screenPoint)
        {
            // See comment in WindowPointToScreenPoint about the coordinate system used
            return new Point(
                screenPoint.X - this.WindowX,
                this.WindowY + this.Height - screenPoint.Y
            );
        }

        public Point WindowPointToScreenPoint(Point windowPoint)
        {
            /*
            The coordinate system for the screen is the same as the one used by SDL:

               ───────────────────────────►x
              ┌────────────────────────────┐
            │ │                            │
            │ │                            │
            │ │        Screen (SDL)        │
            │ │                            │
            │ │                            │
            ▼ │                            │
            y └────────────────────────────┘

            The coordinate system used by Creativedocs has the y-axis flipped:

            y ┌────────────────────────────┐
            ▲ │                            │
            │ │                            │
            │ │       Creativedocs         │
            │ │                            │
            │ │                            │
            │ │                            │
              └────────────────────────────┘
               ───────────────────────────►x
            */
            return new Point(
                this.WindowX + windowPoint.X,
                this.WindowY + this.Height - windowPoint.Y
            );
        }
        #endregion

        #region old WinForms compatibility
        // --------------------------------------------------------------------------------------------
        //                             System.Windows.Forms.Form stubs
        // --------------------------------------------------------------------------------------------
        // We are missing several properties by removing the inheritance from System.Windows.Forms.Form
        // We add them here as stubs

        /// <summary>
        /// true if drag-and-drop operations are allowed in the control; otherwise, false. The default is false.
        /// </summary>
        public bool AllowDrop
        {
            // bl-net8-cross draganddrop
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control has captured the mouse.
        /// </summary>
        public bool Capture
        {
            // bl-net8-cross draganddrop
            get { return false; }
            set { }
        }
        // --------------------------------------------------------------------------------------------
        #endregion

        #region properties and methods
        internal WindowType WindowType
        {
            get { return this.windowType; }
            set
            {
                if (this.windowType != value)
                {
                    this.windowType = value;
                }
            }
        }

        internal bool PreventAutoClose
        {
            get { return this.preventClose; }
            set { this.preventClose = value; }
        }

        internal bool PreventAutoQuit
        {
            get { return this.preventQuit; }
            set { this.preventQuit = value; }
        }

        internal bool IsLayered
        {
            get { return this.isLayered; }
            set { this.isLayered = value; }
        }

        internal bool IsFrozen
        {
            get
            {
                return (this.isFrozen)
                    || (this.widgetWindow == null)
                    || (this.widgetWindow.Root == null)
                    || (this.widgetWindow.Root.IsFrozen);
            }
        }
        internal bool IsFullScreen
        {
            get { return this.isFullscreen; }
            set
            {
                this.SetFullscreen(value);
                this.isFullscreen = value;
            }
        }

        internal bool IsAnimatingActiveWindow
        {
            get { return this.isAnimatingActiveWindow; }
        }

        internal bool IsMouseActivationEnabled
        {
            get { return !this.isNoActivate; }
            set { this.isNoActivate = !value; }
        }

        internal bool IsToolWindow
        {
            get { return this.isToolWindow; }
        }

        internal bool IsSizeMoveInProgress
        {
            get { return this.isSizeMoveInProgress; }
            set
            {
                if (this.isSizeMoveInProgress != value)
                {
                    this.isSizeMoveInProgress = value;
                    if (this.widgetWindow != null)
                    {
                        this.widgetWindow.OnWindowSizeMoveStatusChanged();
                    }
                }
            }
        }

        internal WindowMode WindowMode
        {
            get { return this.windowMode; }
            set { this.windowMode = value; }
        }

        internal Drawing.Rectangle WindowBounds
        {
            get { return new Rectangle(this.WindowLocation, this.WindowSize); }
            set
            {
                this.WindowLocation = value.Location;
                this.WindowSize = value.Size;
            }
        }

        public Drawing.Size MinimumSize
        {
            get { return this.minimumSize; }
            set
            {
                this.minimumSize = value;
                Size targetSize = new Size(
                    System.Math.Max(this.Width, value.Width),
                    System.Math.Max(this.Height, value.Height)
                );
                if (targetSize != this.WindowSize)
                {
                    this.WindowSize = targetSize;
                }
            }
        }

        private WindowPlacement CurrentWindowPlacement
        {
            set
            {
                if (!this.windowPlacement.Equals(value))
                {
                    this.windowPlacement = value;

                    if (this.widgetWindow != null)
                    {
                        this.widgetWindow.OnWindowPlacementChanged();
                    }
                }
            }
        }
        internal Drawing.Point WindowLocation
        {
            get { return new Point(this.WindowX, this.WindowY); }
            set { this.SetPosition((int)value.X, (int)value.Y); }
        }

        internal Drawing.Size WindowSize
        {
            get { return new Drawing.Size(this.Width, this.Height); }
            set { this.SetSize((int)value.Width, (int)value.Height); }
        }

        internal string Text
        {
            get { return this.windowTitle; }
            set
            {
                this.windowTitle = value;
                this.SetTitle(value);
            }
        }

        internal string Name
        {
            get { return this.windowName; }
            set { this.windowName = value; }
        }

        internal Drawing.Image Icon
        {
            get { return this.icon; }
            set
            {
                this.icon = value;
                var pixels = value.BitmapImage.GetPixelBuffer();
                this.SetCustomIcon(pixels, (int)value.Width, (int)value.Height);
            }
        }
        internal double Alpha
        {
            get { return this.alpha; }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(
                        $"Invalid alpha value {value}, should be between 0.0 and 1.0"
                    );
                }
                Console.WriteLine($"Set opacity {value}");
                this.SetWindowOpacity((float)value);
                this.alpha = value;
            }
        }

        internal bool FilterMouseMessages
        {
            get { return this.filterMouseMessages; }
            set { this.filterMouseMessages = value; }
        }

        internal bool FilterKeyMessages
        {
            get { return this.filterKeyMessages; }
            set { this.filterKeyMessages = value; }
        }

        internal Window HostingWidgetWindow
        {
            get { return this.widgetWindow; }
        }

        internal void AnimateShow(Animation animation, Drawing.Rectangle bounds)
        {
            if (this.isLayered)
            {
                switch (animation)
                {
                    case Animation.RollDown:
                    case Animation.RollUp:
                    case Animation.RollLeft:
                    case Animation.RollRight:
                        animation = Animation.FadeIn;
                        break;
                }
            }

            Drawing.Rectangle b1;
            Drawing.Rectangle b2;
            Drawing.Point o1;
            Drawing.Point o2;

            double startAlpha = this.alpha;

            this.isAnimatingActiveWindow = true;
            this.WindowBounds = bounds;
            this.MarkForRepaint();
            // bl-net8-cross
            //this.RefreshGraphics();

            Animator animator;

            switch (animation)
            {
                default:
                case Animation.None:
                    this.Show();
                    this.isAnimatingActiveWindow = false;
                    return;

                case Animation.RollDown:
                    b1 = new Drawing.Rectangle(bounds.Left, bounds.Top - 1, bounds.Width, 1);
                    b2 = bounds;
                    o1 = new Drawing.Point(0, 1 - bounds.Height);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.RollUp:
                    b1 = new Drawing.Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 1);
                    b2 = bounds;
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.RollRight:
                    b1 = new Drawing.Rectangle(bounds.Left, bounds.Bottom, 1, bounds.Height);
                    b2 = bounds;
                    o1 = new Drawing.Point(1 - bounds.Width, 0);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.RollLeft:
                    b1 = new Drawing.Rectangle(bounds.Right - 1, bounds.Bottom, 1, bounds.Height);
                    b2 = bounds;
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.FadeIn:
                    this.isFrozen = true;
                    this.IsLayered = true;
                    this.Alpha = 0.0;

                    animator = new Animator(SystemInformation.MenuAnimationFadeInTime);
                    animator.SetCallback<double>(this.AnimateAlpha, this.AnimateCleanup);
                    animator.SetValue(0.0, startAlpha);
                    Console.WriteLine("start animator");
                    animator.Start();
                    Console.WriteLine("show window");
                    this.Show();
                    return;

                case Animation.FadeOut:
                    this.isFrozen = true;
                    this.IsLayered = true;
                    //					this.Alpha = 1.0;

                    animator = new Animator(SystemInformation.MenuAnimationFadeOutTime);
                    animator.SetCallback<double>(this.AnimateAlpha, this.AnimateCleanup);
                    animator.SetValue(startAlpha, 0.0);
                    animator.Start();
                    return;
            }

            switch (animation)
            {
                case Animation.RollDown:
                case Animation.RollUp:
                case Animation.RollRight:
                case Animation.RollLeft:
                    this.isFrozen = true;
                    this.MinimumSize = new Size(1, 1);
                    this.WindowBounds = b1;

                    animator = new Animator(SystemInformation.MenuAnimationRollTime);
                    animator.SetCallback<Drawing.Rectangle, Drawing.Point>(
                        this.AnimateWindowBounds,
                        this.AnimateCleanup
                    );
                    animator.SetValue(0, b1, b2);
                    animator.SetValue(1, o1, o2);
                    animator.Start();
                    this.Show();
                    break;
            }
        }

        internal void AnimateHide(Animation animation, Drawing.Rectangle bounds)
        {
            Drawing.Rectangle b1;
            Drawing.Rectangle b2;
            Drawing.Point o1;
            Drawing.Point o2;

            double startAlpha = this.alpha;

            this.WindowBounds = bounds;
            this.MarkForRepaint();
            //this.RefreshGraphics();

            Animator animator;

            switch (animation)
            {
                case Animation.None:
                    this.Hide();
                    return;

                case Animation.RollDown:
                    b1 = bounds;
                    b2 = new Drawing.Rectangle(bounds.Left, bounds.Top - 1, bounds.Width, 1);
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(0, 1 - bounds.Height);
                    break;

                case Animation.RollUp:
                    b1 = bounds;
                    b2 = new Drawing.Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 1);
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.RollRight:
                    b1 = bounds;
                    b2 = new Drawing.Rectangle(bounds.Left, bounds.Bottom, 1, bounds.Height);
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(1 - bounds.Width, 0);
                    break;

                case Animation.RollLeft:
                    b1 = bounds;
                    b2 = new Drawing.Rectangle(bounds.Right - 1, bounds.Bottom, 1, bounds.Height);
                    o1 = new Drawing.Point(0, 0);
                    o2 = new Drawing.Point(0, 0);
                    break;

                case Animation.FadeIn:
                case Animation.FadeOut:
                    this.AnimateShow(animation, bounds);
                    return;

                default:
                    this.Hide();
                    return;
            }

            switch (animation)
            {
                case Animation.RollDown:
                case Animation.RollUp:
                case Animation.RollRight:
                case Animation.RollLeft:
                    this.isFrozen = true;
                    this.isAnimatingActiveWindow = this.IsFocused;
                    this.WindowBounds = b1;

                    animator = new Animator(SystemInformation.MenuAnimationRollTime);
                    animator.SetCallback<Drawing.Rectangle, Drawing.Point>(
                        this.AnimateWindowBounds,
                        this.AnimateCleanup
                    );
                    animator.SetValue(0, b1, b2);
                    animator.SetValue(1, o1, o2);
                    animator.Start();
                    break;
            }
        }

        protected bool RefreshGraphics(Graphics graphics)
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.ForceLayout();
            }

            if (this.IsFrozen)
            {
                return false;
            }

            return this.RefreshGraphicsLowLevel(graphics);
        }

        private bool RefreshGraphicsLowLevel(Graphics graphics)
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.RefreshGraphics(graphics, Rectangle.MaxValue, []);
            }
            return true;
        }

        internal void DispatchMessage(Message message)
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.DispatchMessage(message);
            }
        }

        internal void ShowDialogWindow()
        {
            this.Show();
            ToolTip.Default.HideToolTip();
        }

        internal bool StartWindowManagerOperation(WindowManagerOperation op)
        {
            /*
            //	Documentation sur WM_NCHITTEST et les modes HT...
            //	Cf http://blogs.msdn.com/jfoscoding/archive/2005/07/28/444647.aspx
            //	Cf http://msdn.microsoft.com/netframework/default.aspx?pull=/libarary/en-us/dndotnet/html/automationmodel.asp

            switch (op)
            {
                case Platform.WindowManagerOperation.ResizeLeft:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_LEFT);
                    return true;
                case Platform.WindowManagerOperation.ResizeRight:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_RIGHT);
                    return true;
                case Platform.WindowManagerOperation.ResizeBottom:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_BOTTOM);
                    return true;
                case Platform.WindowManagerOperation.ResizeBottomRight:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_BOTTOMRIGHT);
                    return true;
                case Platform.WindowManagerOperation.ResizeBottomLeft:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_BOTTOMLEFT);
                    return true;
                case Platform.WindowManagerOperation.ResizeTop:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_TOP);
                    return true;
                case Platform.WindowManagerOperation.ResizeTopRight:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_TOPRIGHT);
                    return true;
                case Platform.WindowManagerOperation.ResizeTopLeft:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_TOPLEFT);
                    return true;
                case Platform.WindowManagerOperation.MoveWindow:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_CAPTION);
                    return true;
                case Platform.WindowManagerOperation.PressMinimizeButton:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_MINBUTTON);
                    return true;
                case Platform.WindowManagerOperation.PressMaximizeButton:
                    this.ReleaseCaptureAndSendMessage(Win32Const.HT_MAXBUTTON);
                    return true;
            }

            return false;
            */
            throw new System.NotImplementedException();
        }

        internal void SetFrozen(bool frozen)
        {
            this.isFrozen = frozen;
        }

        internal void SendQueueCommand()
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.DispatchQueuedCommands();
            }
        }

        internal void SendValidation()
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.DispatchValidation();
            }
        }

        public static void RunEventLoop()
        {
            Timer.PendingTimers += (_) =>
            {
                SDLWrapper.SDLWindowManager.PushUserEvent(0, null);
            };
            SDLWrapper.SDLWindowManager.RunApplicationEventLoop();
        }
        #endregion

        #region NotImplemented
        internal static bool UseWaitCursor
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull

            //get { return System.Windows.Forms.Application.UseWaitCursor; }
            get { return true; }
            //set { System.Windows.Forms.Application.UseWaitCursor = value; }
            set { }
        }

        internal Drawing.Rectangle WindowPlacementNormalBounds
        {
            get
            {
                /*
                Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement()
                {
                    Length = 4 + 4 + 4 + 2 * 4 + 2 * 4 + 4 * 4
                };

                Win32Api.GetWindowPlacement(this.Handle, ref placement);

                double ox = this.MapFromWinFormsX(placement.NormalPosition.Left);
                double oy = this.MapFromWinFormsY(placement.NormalPosition.Bottom);
                double dx = this.MapFromWinFormsWidth(
                    placement.NormalPosition.Right - placement.NormalPosition.Left
                );
                double dy = this.MapFromWinFormsHeight(
                    placement.NormalPosition.Bottom - placement.NormalPosition.Top
                );

                //	Attention: les coordonnées retournées par WindowPlacement sont exprimées
                //	en "workspace coordinates" (elles tiennent compte de la présence d'une
                //	barre "Desktop").

                //	Cf. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowstructures/windowplacement.asp

                PlatformWindow.AdjustWindowPlacementOrigin(placement, ref ox, ref oy);

                return new Drawing.Rectangle(ox, oy, dx, dy);
                */
                throw new NotImplementedException();
                return Drawing.Rectangle.Empty;
            }
        }

        internal WindowPlacement NativeWindowPlacement
        {
            get
            {
                /*
                Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement()
                {
                    Length = 4 + 4 + 4 + 2 * 4 + 2 * 4 + 4 * 4
                };

                Win32Api.GetWindowPlacement(this.Handle, ref placement);

                bool isMaximized = (placement.Flags & Win32Const.WPF_RESTORETOMAXIMIZED) != 0;
                bool isMinimized = (placement.ShowCmd == Win32Const.SW_SHOWMINIMIZED);
                bool isHidden = (placement.ShowCmd == Win32Const.SW_HIDE);

                var bounds = new Drawing.Rectangle(
                    placement.NormalPosition.Left,
                    placement.NormalPosition.Top,
                    placement.NormalPosition.Right - placement.NormalPosition.Left,
                    placement.NormalPosition.Bottom - placement.NormalPosition.Top
                );

                return new WindowPlacement(bounds, isMaximized, isMinimized, isHidden);
                */
                throw new NotImplementedException();
                return new WindowPlacement(Drawing.Rectangle.Empty, false, false, false);
            }
            set
            {
                /*
                int show = Win32Const.SW_SHOWNORMAL;
                int flags = 0;

                if (value.IsFullScreen)
                {
                    show = Win32Const.SW_SHOWMAXIMIZED;
                    flags |= Win32Const.WPF_RESTORETOMAXIMIZED;
                }
                if (value.IsMinimized)
                {
                    show = Win32Const.SW_SHOWMINIMIZED;
                }
                if (value.IsHidden)
                {
                    show = Win32Const.SW_HIDE;
                }

                Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement()
                {
                    Length = 4 + 4 + 4 + 2 * 4 + 2 * 4 + 4 * 4,
                    NormalPosition = new Win32Api.Rect()
                    {
                        Left = (int)value.Bounds.Left,
                        Right = (int)value.Bounds.Right,
                        Top = (int)value.Bounds.Bottom,
                        Bottom = (int)value.Bounds.Top
                    },
                    ShowCmd = show,
                    Flags = flags
                };

                try
                {
                    this.EnterWndProc();
                    Win32Api.SetWindowPlacement(this.Handle, ref placement);
                }
                finally
                {
                    this.ExitWndProc();
                }
                */
                throw new NotImplementedException();
            }
        }

        internal static void ProcessException(System.Exception ex, string tag)
        {
            /*
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            buffer.Append("------------------------------------------------------------");
            buffer.Append("\r\n");
            buffer.Append(tag);
            buffer.Append("\r\n");
            buffer.Append(
                System.Diagnostics.Process.GetCurrentProcess().MainModule.FileVersionInfo.ToString()
            );
            buffer.Append("\r\n");
            buffer.Append("PlatformWindow: ");
            buffer.Append(System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle);
            buffer.Append("\r\n");
            buffer.Append("Thread: ");
            buffer.Append(System.Threading.Thread.CurrentThread.Name);
            buffer.Append("\r\n");
            buffer.Append("\r\n");

            while (ex != null)
            {
                buffer.Append("Exception type: ");
                buffer.Append(ex.GetType().Name);
                buffer.Append("\r\n");
                buffer.Append("Message:        ");
                buffer.Append(ex.Message);
                buffer.Append("\r\n");
                buffer.Append("Stack:\r\n");
                buffer.Append(ex.StackTrace);

                ex = ex.InnerException;

                if (ex != null)
                {
                    buffer.Append("\r\nInner Exception found.\r\n\r\n");
                }
            }

            buffer.Append("\r\n");
            buffer.Append("------------------------------------------------------------");
            buffer.Append("\r\n");

            Support.ClipboardWriteData data = new Epsitec.Common.Support.ClipboardWriteData();
            data.WriteText(buffer.ToString());
            Support.Clipboard.SetData(data);

            string key = "Bug report e-mail";
            string email = Globals.Properties.GetProperty(key, "bugs@opac.ch");

            string msgFr =
                "Une erreur interne s'est produite. Veuillez SVP envoyer un mail avec la\n"
                + "description de ce que vous étiez en train de faire au moment où ce message\n"
                + "est apparu et collez y (CTRL+V) le contenu du presse-papiers.\n\n"
                + "Envoyez s'il-vous-plaît ces informations à "
                + email
                + "\n\n"
                + "Merci pour votre aide.";

            string msgEn =
                "An internal error occurred. Please send an e-mail with a short description\n"
                + "of what you were doing when this message appeared and include (press CTRL+V)\n"
                + "contents of the clipboard, which contains useful debugging information.\n\n"
                + "Please send these informations to "
                + email
                + "\n\n"
                + "Thank you very much for your help.";

            bool isFrench = (
                System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr"
            );

            string title = isFrench ? "Erreur interne" : "Internal error";
            string message = isFrench ? msgFr : msgEn;

            System.Diagnostics.Debug.WriteLine(buffer.ToString());
            System.Windows.Forms.MessageBox.Show(null, message, title);
            */
            throw new System.NotImplementedException();
        }

        internal static void ProcessCrossThreadOperation(System.Action action)
        {
            /*
            bool state = System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls;

            try
            {
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

                action();
            }
            finally
            {
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = state;
            }
            */
            throw new System.NotImplementedException();
        }

        private void ReleaseCaptureAndSendMessage(uint ht)
        {
            /*
            Win32Api.ReleaseCapture();
            Win32Api.SendMessage(
                this.Handle,
                Platform.Win32Const.WM_NCLBUTTONDOWN,
                (System.IntPtr)ht,
                (System.IntPtr)0
            );
            */
            throw new System.NotImplementedException();
        }

        protected void FakeActivate(bool active)
        {
            /*
            if (this.hasActiveFrame != active)
            {
                this.hasActiveFrame = active;

                if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                {
                    System.Windows.Forms.Message message = PlatformWindow.CreateNCActivate(this, active);
                    //					System.Diagnostics.Debug.WriteLine (string.Format ("PlatformWindow {0} faking WM_NCACTIVATE {1}.", this.Name, active));
                    base.WndProc(ref message);
                }
            }
            */
            throw new System.NotImplementedException();
        }

        protected void FakeActivateOwned(bool active)
        {
            /*
            this.FakeActivate(active);

            System.Windows.Forms.Form[] forms = this.OwnedForms;

            for (int i = 0; i < forms.Length; i++)
            {
                PlatformWindow platformWindow = forms[i] as PlatformWindow;

                if (platformWindow != null)
                {
                    if (platformWindow.isToolWindow)
                    {
                        platformWindow.FakeActivate(active);
                    }
                }
            }
            */
            throw new System.NotImplementedException();
        }

        protected bool IsOwnedWindow(Platform.PlatformWindow find)
        {
            /*
            System.Windows.Forms.Form[] forms = this.OwnedForms;

            for (int i = 0; i < forms.Length; i++)
            {
                PlatformWindow platformWindow = forms[i] as PlatformWindow;

                if (platformWindow != null)
                {
                    if (platformWindow == find)
                    {
                        return true;
                    }
                }
            }

            return false;
            */
            throw new System.NotImplementedException();
            return true;
        }

        internal Drawing.DrawingBitmap GetWindowPixmap()
        {
            /*
            if ((this.graphics != null) && (this.isPixmapOk))
            {
                return this.graphics.DrawingBitmap;
            }
            else
            {
                return null;
            }
            */
            throw new System.NotImplementedException();
        }

        protected void ReallocatePixmap()
        {
            /*
            if (this.IsFrozen)
            {
                return;
            }

            if (this.ReallocatePixmapLowLevel())
            {
                this.UpdateLayeredWindow();
            }
            */
            throw new System.NotImplementedException();
        }

        private bool ReallocatePixmapLowLevel()
        {
            /*
            bool changed = false;

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            if (this.graphics.SetPixmapSize(width, height))
            {
                //				System.Diagnostics.Debug.WriteLine ("ReallocatePixmapLowLevel" + (this.isFrozen ? " (frozen)" : "") + " Size: " + width.ToString () + "," + height.ToString());

                this.graphics.DrawingBitmap.Clear();

                if (this.widgetWindow != null)
                {
                    this.widgetWindow.Root.NotifyWindowSizeChanged(width, height);
                }
                this.dirtyRectangle = new Drawing.Rectangle(0, 0, width, height);
                this.dirtyRegion = new Drawing.DirtyRegion();
                this.dirtyRegion.Add(this.dirtyRectangle);

                changed = true;
            }

            this.isPixmapOk = true;

            return changed;
            */
            throw new System.NotImplementedException();
        }

        internal void MarkForRepaint()
        {
            // repaint all
            this.MarkForRepaint(new Drawing.Rectangle(0, 0, this.Width, this.Height));
        }

        internal void MarkForRepaint(Drawing.Rectangle rect)
        {
            // TODO bl-net8-cross
            // since the drawing works differently with AntigrainSharp than with winforms,
            // those Invalidate calls will probably not be needed anymore
            // If that turn out to be the case, we could delete them

            /*
            rect.RoundInflate();

            this.dirtyRectangle.MergeWith(rect);
            this.dirtyRegion.Add(rect);

            int top = (int)(rect.Top);
            int bottom = (int)(rect.Bottom);

            int width = (int)(rect.Width);
            int height = top - bottom + 1;
            int x = (int)(rect.Left);
            int y = this.ClientSize.Height - top;

            if (this.isLayered)
            {
                this.isLayeredDirty = true;
            }

            this.Invalidate(new System.Drawing.Rectangle(x, y, width, height));
            */
        }

        internal void SynchronousRepaint()
        {
            // TODO bl-net8-cross
            // since the drawing works differently with AntigrainSharp than with winforms,
            // those Invalidate calls will probably not be needed anymore
            // If that turn out to be the case, we could delete them
            /*
            if (this.isLayoutInProgress)
            {
                return;
            }

            this.isLayoutInProgress = true;

            try
            {
                if (this.widgetWindow != null)
                {
                    this.widgetWindow.ForceLayout();
                }
            }
            finally
            {
                this.isLayoutInProgress = false;
            }

            if (this.dirtyRectangle.IsValid)
            {
                using (this.isSyncUpdating.Enter())
                {
                    //this.Update(); // winforms: redraw the invalidated areas
                }
            }
            */
        }

        internal static void SendSynchronizeCommandCache()
        {
            // bl-net8-cross
            // not sure what this is for, can maybe be deleted
            /*
            PlatformWindow.isSyncRequested = true;

            try
            {
                Win32Api.PostMessage(
                    PlatformWindow.dispatchWindowHandle,
                    Win32Const.WM_APP_SYNCMDCACHE,
                    System.IntPtr.Zero,
                    System.IntPtr.Zero
                );
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Exception thrown in Platform.PlatformWindow.SendSynchronizeCommandCache :"
                );
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            */
        }

        internal static void SendAwakeEvent()
        {
            /*
            bool awake = false;

            lock (PlatformWindow.dispatchWindow)
            {
                if (PlatformWindow.isAwakeRequested == false)
                {
                    PlatformWindow.isAwakeRequested = true;
                    awake = true;
                }
            }

            if (awake)
            {
                try
                {
                    Win32Api.PostMessage(
                        PlatformWindow.dispatchWindowHandle,
                        Win32Const.WM_APP_AWAKE,
                        System.IntPtr.Zero,
                        System.IntPtr.Zero
                    );
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Exception thrown in Platform.PlatformWindow.SendAwakeEvent:"
                    );
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            */
            throw new System.NotImplementedException();
        }

        internal Platform.PlatformWindow FindRootOwner()
        {
            /*
            PlatformWindow owner = this.Owner as PlatformWindow;

            if (owner != null)
            {
                return owner.FindRootOwner();
            }

            return this;
            */
            throw new System.NotImplementedException();
            return null;
        }

        protected void AnimateWindowBounds(Drawing.Rectangle bounds, Drawing.Point offset)
        {
            /*
            if (this.IsDisposed)
            {
                return;
            }

            this.WindowBounds = bounds;
            this.paintOffset = offset;
            this.Invalidate();
            this.Update();
            */
            throw new NotImplementedException();
        }

        protected void AnimateAlpha(double alpha)
        {
            this.Alpha = alpha;
        }

        protected void AnimateCleanup(Animator animator)
        {
            animator.Dispose();

            this.isFrozen = false;
            this.isAnimatingActiveWindow = false;

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowAnimationEnded();
            }
        }

        internal void Close()
        {
            this.Dispose();
        }

        #endregion NotImplemented

        #region NotImplemented event handlers

        protected void OnMouseEnter(System.EventArgs e)
        //protected override void OnMouseEnter(System.EventArgs e)
        {
            /*
            base.OnMouseEnter(e);

            System.Drawing.Point point = this.PointToClient(
                System.Windows.Forms.Control.MousePosition
            );
            System.Windows.Forms.MouseEventArgs fakeEvent = new System.Windows.Forms.MouseEventArgs(
                System.Windows.Forms.MouseButtons.None,
                0,
                point.X,
                point.Y,
                0
            );

            Message message = Message.FromMouseEvent(MessageType.MouseEnter, this, fakeEvent);

            if (this.widgetWindow != null)
            {
                if (this.widgetWindow.FilterMessage(message) == false)
                {
                    this.DispatchMessage(message);
                }
            }
            */
            throw new NotImplementedException();
        }

        protected void OnMouseLeave(System.EventArgs e)
        //protected override void OnMouseLeave(System.EventArgs e)
        {
            /*
            base.OnMouseLeave(e);

            Message message = Message.FromMouseEvent(MessageType.MouseLeave, this, null);

            if (this.widgetWindow != null)
            {
                if (this.widgetWindow.FilterMessage(message) == false)
                {
                    this.DispatchMessage(message);
                }
            }
            */
            throw new NotImplementedException();
        }

        protected void OnResizeBegin(System.EventArgs e)
        //protected override void OnResizeBegin(System.EventArgs e)
        {
            /*
            base.OnResizeBegin(e);
            this.widgetWindow.OnWindowResizeBeginning();
            */
            throw new NotImplementedException();
        }

        protected void OnResizeEnd(System.EventArgs e)
        //protected override void OnResizeEnd(System.EventArgs e)
        {
            /*
            base.OnResizeEnd(e);
            this.widgetWindow.OnWindowResizeEnded();
            */
            throw new NotImplementedException();
        }

        protected void OnSizeChanged(System.EventArgs e)
        //protected override void OnSizeChanged(System.EventArgs e)
        {
            /*
            //			System.Diagnostics.Debug.WriteLine ("OnSizeChanged");
            using (this.isSyncPaintDisabled.Enter())
            {
                if (
                    (this.Created == false)
                    && (this.formBoundsSet)
                    && (this.formBounds.Size != this.Size)
                )
                {
                    this.Size = this.formBounds.Size;
                }
                else if (
                    (this.formBoundsSet)
                    && (this.formBounds.Size == this.Size)
                    && (this.onResizeEvent == false)
                )
                {
                    //	Rien à faire, car la taille correspond à la dernière taille mémorisée.
                }
                else
                {
                    this.formBoundsSet = true;
                    this.onResizeEvent = false;
                    this.formBounds = this.Bounds;
                    this.windowBounds = this.WindowBounds;


                    base.OnSizeChanged(e);
                    this.ReallocatePixmap();
                }

                this.formBoundsSet = false;
            }
            */
            throw new NotImplementedException();
        }

        protected void OnActivated(System.EventArgs e)
        //protected override void OnActivated(System.EventArgs e)
        {
            /*
            base.OnActivated(e);

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowActivated();
            }
            */
            throw new NotImplementedException();
        }

        protected void OnDeactivate(System.EventArgs e)
        //protected override void OnDeactivate(System.EventArgs e)
        {
            /*
            base.OnDeactivate(e);

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowDeactivated();
            }
            */
            throw new NotImplementedException();
        }

        protected void OnDragLeave(System.EventArgs e)
        //protected override void OnDragLeave(System.EventArgs e)
        {
            /*
            base.OnDragLeave(e);

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowDragExited();
            }
            */
            throw new NotImplementedException();
        }
        #endregion

        #region Memory management
        public new void Dispose()
        {
            if (this.widgetWindow == null)
            {
                return;
            }
            WindowList.Remove(this);

            if (this.renderingBuffer != null)
            {
                this.renderingBuffer.Dispose();
            }
            this.renderingBuffer = null;

            if (this.icon != null)
            {
                this.icon.Dispose();
            }
            this.icon = null;

            base.Dispose();

            // We do not call Dispose on our widgetWindow since it could still live without us.
            this.widgetWindow = null;
        }
        #endregion

        private Window widgetWindow;

        private AntigrainSharp.AbstractGraphicBuffer renderingBuffer;
        private Size minimumSize;

        private Image icon;

        private bool isFullscreen;
        private bool isLayered;
        private bool isFrozen;
        private bool isAnimatingActiveWindow;
        private bool isNoActivate;
        private bool isToolWindow;

        private bool preventClose;
        private bool preventQuit;
        private bool filterMouseMessages;
        private bool filterKeyMessages;
        private double alpha = 1.0;

        private string windowTitle;
        private string windowName;
        private WindowMode windowMode = WindowMode.Window;
        private WindowType windowType;

        private bool isSizeMoveInProgress;

        private WindowPlacement windowPlacement;
    }
}
