//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe Platform.Window fait le lien avec la SDL
    /// </summary>
    internal class Window : SDLWrapper.SDLWindow
    {
        // ******************************************************************
        // TODO bl-net8-cross
        // implement Window (stub)
        // ******************************************************************

        // --------------------------------------------------------------------------------------------
        //                             SDLWindow overrides
        // --------------------------------------------------------------------------------------------
        ~Window()
        {
            System.Console.WriteLine("Delete window");
        }

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
            Rectangle repaint = Rectangle.MaxValue;
            this.graphics = new Graphics(this.renderingBuffer.GraphicContext);
            this.widgetWindow.RefreshGraphics(this.graphics, repaint, []);
            this.graphics = null;
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

        public override void OnMouseButtonDown(int x, int y, int button)
        {
            Console.WriteLine($"MouseDown {button} {x} {y}");
            MouseButtons btn = this.ConvertMouseButton(button);
            Message msg = Message.FromMouseEvent(MessageType.MouseDown, this, btn, x, y, 0);
            this.DispatchMessage(msg);
        }

        public override void OnMouseButtonUp(int x, int y, int button)
        {
            Console.WriteLine($"MouseUp {button} {x} {y}");
            MouseButtons btn = this.ConvertMouseButton(button);
            Message msg = Message.FromMouseEvent(MessageType.MouseUp, this, btn, x, y, 0);
            this.DispatchMessage(msg);
        }

        public override void OnMouseMove(int x, int y)
        {
            MouseButtons btn = MouseButtons.None;
            Message msg = Message.FromMouseEvent(MessageType.MouseMove, this, btn, x, y, 0);
            this.DispatchMessage(msg);
        }

        protected override void OnResize(int sx, int sy)
        {
            Console.WriteLine($"Resize {sx} {sy}");
            //this.clientSize = new System.Drawing.Size(sx, sy);
            //this.ForceRedraw();
        }

        // --------------------------------------------------------------------------------------------
        //                             System.Windows.Forms.Form stubs
        // --------------------------------------------------------------------------------------------
        // We are missing several properties by removing the inheritance from System.Windows.Forms.Form
        // We add them here as stubs

        /// <summary>
        /// Activates the form and gives it focus.
        /// </summary>
        public void Activate()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// true if drag-and-drop operations are allowed in the control; otherwise, false. The default is false.
        /// </summary>
        public bool AllowDrop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the control has captured the mouse.
        /// </summary>
        public bool Capture { get; set; }

        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        public bool Focused { get; }

        public bool Focus()
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point p)
        {
            // bl-net8-cross delete or implement this conversion
            return p;
        }

        public System.Drawing.Point PointToScreen(System.Drawing.Point p)
        {
            // bl-net8-cross delete or implement this conversion
            return p;
        }

        // --------------------------------------------------------------------------------------------

        internal Window(
            Epsitec.Common.Widgets.Window window,
            System.Action<Window> platformWindowSetter
        )
            : base("Creativedocs", 800, 600)
        {
            Console.WriteLine("internal Window()");
            this.widgetWindow = window;
            platformWindowSetter(this);

            this.dirtyRectangle = Drawing.Rectangle.Empty;
            this.dirtyRegion = new Drawing.DirtyRegion();

            /* //REMOVED (bl-net8-cross)
            base.MinimumSize = new System.Drawing.Size(1, 1);

            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;

            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.ResizeRedraw, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            */

            this.widgetWindow.WindowType = WindowType.Document;
            this.widgetWindow.WindowStyles = WindowStyles.CanResize | WindowStyles.HasCloseButton;

            //this.graphics.AllocatePixmap();

            //Window.DummyHandleEater(this.Handle);

            /*
            //	Fait en sorte que les changements de dimensions en [x] et en [y] provoquent un
            //	redessin complet de la fenêtre, sinon Windows tente de recopier l'ancien contenu
            //	en le décalant, ce qui donne des effets bizarres :

            int classWindowStyle = Win32Api.GetClassLong(this.Handle, Win32Const.GCL_STYLE);

            classWindowStyle |= Win32Const.CS_HREDRAW;
            classWindowStyle |= Win32Const.CS_VREDRAW;

            Win32Api.SetClassLong(this.Handle, Win32Const.GCL_STYLE, classWindowStyle);
            */

            //this.ReallocatePixmap();

            WindowList.Insert(this);
        }

        internal void MakeTopLevelWindow()
        {
            /*
            this.TopLevel = true;
            this.TopMost = true;
            */
            throw new NotImplementedException();
        }

        internal void MakeFramelessWindow()
        {
            /*
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowInTaskbar = false;
            Window.DummyHandleEater(this.Handle);
            this.widgetWindow.WindowStyles = this.WindowStyles | (WindowStyles.Frameless);
            */
            throw new NotImplementedException();
        }

        internal void MakeFixedSizeWindow()
        {
            /*
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Window.DummyHandleEater(this.Handle);

            this.widgetWindow.WindowStyles =
                this.WindowStyles
                & ~(WindowStyles.CanMaximize | WindowStyles.CanMinimize | WindowStyles.CanResize);
            */
            throw new NotImplementedException();
        }

        internal void MakeMinimizableFixedSizeWindow()
        {
            /*
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            Window.DummyHandleEater(this.Handle);
            this.widgetWindow.WindowStyles =
                (this.WindowStyles & ~(WindowStyles.CanMaximize | WindowStyles.CanResize))
                | WindowStyles.CanMinimize;
            */
            throw new NotImplementedException();
        }

        internal void MakeButtonlessWindow()
        {
            /*
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            Window.DummyHandleEater(this.Handle);
            this.widgetWindow.WindowStyles =
                this.WindowStyles
                & ~(
                    WindowStyles.CanMaximize
                    | WindowStyles.CanMinimize
                    | WindowStyles.HasCloseButton
                );
            */
            throw new NotImplementedException();
        }

        internal bool IsFixedSize
        {
            get
            {
                /*
                switch (this.FormBorderStyle)
                {
                    case System.Windows.Forms.FormBorderStyle.Fixed3D:
                    case System.Windows.Forms.FormBorderStyle.FixedDialog:
                    case System.Windows.Forms.FormBorderStyle.FixedSingle:
                    case System.Windows.Forms.FormBorderStyle.FixedToolWindow:
                    case System.Windows.Forms.FormBorderStyle.None:
                        return true;
                    case System.Windows.Forms.FormBorderStyle.Sizable:
                    case System.Windows.Forms.FormBorderStyle.SizableToolWindow:
                        return false;
                }

                throw new System.InvalidOperationException(
                    string.Format("{0} not supported", this.FormBorderStyle)
                );
                */
                throw new NotImplementedException();
                return false;
            }
        }

        internal void MakeSecondaryWindow()
        {
            /*
            this.ShowInTaskbar = false;
            Window.DummyHandleEater(this.Handle);
            */
            throw new NotImplementedException();
        }

        internal void MakeToolWindow()
        {
            /*
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.isToolWindow = true;
            Window.DummyHandleEater(this.Handle);
            */
            throw new NotImplementedException();
        }

        internal void MakeSizableToolWindow()
        {
            /*
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            this.isToolWindow = true;
            Window.DummyHandleEater(this.Handle);
            */
            throw new NotImplementedException();
        }

        internal void MakeFloatingWindow()
        {
            /*
            this.ShowInTaskbar = false;
            this.isToolWindow = true;
            Window.DummyHandleEater(this.Handle);
            */
            throw new NotImplementedException();
        }

        internal void ResetHostingWidgetWindow()
        {
            this.widgetWindowDisposed = true;
        }

        internal void HideWindow()
        {
            /*
            using (this.isWndProcHandlingRestricted.Enter())
            {
                this.Hide();
            }
            */
            throw new NotImplementedException();
        }

        static void DummyHandleEater(System.IntPtr handle) { }

        internal void AnimateShow(Animation animation, Drawing.Rectangle bounds)
        {
            /*
            Window.DummyHandleEater(this.Handle);

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
            this.RefreshGraphics();

            Animator animator;

            switch (animation)
            {
                default:
                case Animation.None:
                    this.ShowWindow();
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
                    animator.Start();
                    this.ShowWindow();
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
                    this.formMinSize = this.MinimumSize;
                    this.MinimumSize = new System.Drawing.Size(1, 1);
                    this.WindowBounds = b1;
                    this.UpdateLayeredWindow();

                    animator = new Animator(SystemInformation.MenuAnimationRollTime);
                    animator.SetCallback<Drawing.Rectangle, Drawing.Point>(
                        this.AnimateWindowBounds,
                        this.AnimateCleanup
                    );
                    animator.SetValue(0, b1, b2);
                    animator.SetValue(1, o1, o2);
                    animator.Start();
                    this.ShowWindow();
                    break;
            }
            */
            throw new NotImplementedException();
        }

        internal void AnimateHide(Animation animation, Drawing.Rectangle bounds)
        {
            /*
            Drawing.Rectangle b1;
            Drawing.Rectangle b2;
            Drawing.Point o1;
            Drawing.Point o2;

            double startAlpha = this.alpha;

            this.WindowBounds = bounds;
            this.MarkForRepaint();
            this.RefreshGraphics();

            Animator animator;

            switch (animation)
            {
                case Animation.None:
                    this.HideWindow();
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
                    this.HideWindow();
                    return;
            }

            switch (animation)
            {
                case Animation.RollDown:
                case Animation.RollUp:
                case Animation.RollRight:
                case Animation.RollLeft:
                    this.isFrozen = true;
                    this.isAnimatingActiveWindow = this.IsActive;
                    this.WindowBounds = b1;
                    this.UpdateLayeredWindow();

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
            */
            throw new System.NotImplementedException();
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
            /*
            if (this.IsDisposed)
            {
                return;
            }

            this.Alpha = alpha;
            */
            throw new NotImplementedException();
        }

        protected void AnimateCleanup(Animator animator)
        {
            /*
            animator.Dispose();

            if (this.IsDisposed)
            {
                return;
            }

            this.MinimumSize = this.formMinSize;
            this.isFrozen = false;
            this.isAnimatingActiveWindow = false;
            this.Invalidate();

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowAnimationEnded();
            }
            */
            throw new NotImplementedException();
        }

        internal WindowStyles WindowStyles
        {
            get { return this.windowStyles; }
            set
            {
                if (this.windowStyles != value)
                {
                    this.windowStyles = value;
                    this.UpdateWindowTypeAndStyles();
                }
            }
        }

        internal WindowType WindowType
        {
            get { return this.windowType; }
            set
            {
                if (this.windowType != value)
                {
                    this.windowType = value;
                    this.UpdateWindowTypeAndStyles();
                }
            }
        }

        private void UpdateWindowTypeAndStyles()
        {
            // bl-net8-cross
            // refaire la gestion des fenêtres avec ou sans bordure
            // il faut aussi refaire les Make______Window()

            var windowStyles = this.WindowStyles;

            switch (this.windowType)
            {
                case WindowType.Document:
                    if ((windowStyles & WindowStyles.Frameless) != 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    }
                    else if ((windowStyles & WindowStyles.CanResize) == 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                    }
                    else
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    }
                    // this.ShowInTaskbar = true;
                    break;

                case WindowType.Dialog:
                    if ((windowStyles & WindowStyles.Frameless) != 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    }
                    else if ((windowStyles & WindowStyles.CanResize) == 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                    }
                    else
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    }
                    // this.ShowInTaskbar = false;
                    break;

                case WindowType.Palette:
                    if ((windowStyles & WindowStyles.Frameless) != 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    }
                    else if ((windowStyles & WindowStyles.CanResize) == 0)
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                    }
                    else
                    {
                        // this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
                    }
                    // this.ShowInTaskbar = false;
                    break;
            }

            //this.MinimizeBox = ((windowStyles & WindowStyles.CanMinimize) != 0);
            //this.MaximizeBox = ((windowStyles & WindowStyles.CanMaximize) != 0);
            //this.HelpButton = ((windowStyles & WindowStyles.HasHelpButton) != 0);
            //this.ControlBox = ((windowStyles & WindowStyles.HasCloseButton) != 0);
            throw new NotImplementedException();
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
            set
            {
                /*
                if (this.isLayered != value)
                {
                    if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                    {
                        throw new System.Exception("A layered window may not have a border");
                    }

                    if (SystemInformation.SupportsLayeredWindows)
                    {
                        int exStyle = Win32Api.GetWindowExStyle(this.Handle);

                        if (value)
                        {
                            exStyle |= Win32Const.WS_EX_LAYERED;
                        }
                        else
                        {
                            exStyle &= ~Win32Const.WS_EX_LAYERED;
                        }

                        Win32Api.SetWindowExStyle(this.Handle, exStyle);
                        this.isLayered = value;
                    }
                }
                */
                throw new NotImplementedException();
            }
        }

        internal bool IsActive
        {
            get
            {
                /*
                if (this.Handle == Win32Api.GetActiveWindow())
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
                throw new NotImplementedException();
                return true;
            }
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
            get
            {
                /*
                System.Drawing.Rectangle rect;

                if (this.formBoundsSet)
                {
                    rect = this.formBounds;
                }
                else
                {
                    rect = base.Bounds;
                }

                double ox = this.MapFromWinFormsX(rect.Left);
                double oy = this.MapFromWinFormsY(rect.Bottom);
                double dx = this.MapFromWinFormsWidth(rect.Width);
                double dy = this.MapFromWinFormsHeight(rect.Height);

                return new Drawing.Rectangle(ox, oy, dx, dy);
                */
                throw new System.NotImplementedException();
                //return Drawing.Rectangle.Empty;
            }
            set
            {
                /*
                if (this.windowBounds != value)
                {
                    int ox = this.MapToWinFormsX(value.Left);
                    int oy = this.MapToWinFormsY(value.Top);
                    int dx = this.MapToWinFormsWidth(value.Width);
                    int dy = this.MapToWinFormsHeight(value.Height);

                    this.windowBounds = value;
                    this.formBounds = new System.Drawing.Rectangle(ox, oy, dx, dy);
                    this.formBoundsSet = true;
                    this.onResizeEvent = true;

                    if (this.isLayered)
                    {
                        //	Be very careful here: in order to avoid any jitter while
                        //	moving & sizing the layered window, we must resize the
                        //	suface pixmap first, update the layered window surface
                        //	and only then move the window itself :

                        this.ReallocatePixmapLowLevel();
                        this.UpdateLayeredWindow();

                        Win32Api.SetWindowPos(
                            this.Handle,
                            System.IntPtr.Zero,
                            ox,
                            oy,
                            dx,
                            dy,
                            Win32Const.SWP_NOACTIVATE
                                | Win32Const.SWP_NOCOPYBITS
                                | Win32Const.SWP_NOOWNERZORDER
                                | Win32Const.SWP_NOZORDER
                                | Win32Const.SWP_NOREDRAW
                        );
                    }
                    else
                    {
                        Win32Api.SetWindowPos(
                            this.Handle,
                            System.IntPtr.Zero,
                            ox,
                            oy,
                            dx,
                            dy,
                            Win32Const.SWP_NOACTIVATE
                                | Win32Const.SWP_NOCOPYBITS
                                | Win32Const.SWP_NOOWNERZORDER
                                | Win32Const.SWP_NOZORDER
                        );
                    }

                    this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                }
                */
                throw new NotImplementedException();
            }
        }

        public System.Drawing.Size ClientSize
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull
            get { return new System.Drawing.Size(this.Width, this.Height); }
        }

        public Drawing.Size MinimumSize
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull
            get { return this.minimumSize; }
            set { this.minimumSize = value; }
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

                Window.AdjustWindowPlacementOrigin(placement, ref ox, ref oy);

                return new Drawing.Rectangle(ox, oy, dx, dy);
                */
                throw new NotImplementedException();
                return Drawing.Rectangle.Empty;
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

        internal Drawing.Point WindowLocation
        {
            get { return this.WindowBounds.Location; }
            set
            {
                Drawing.Rectangle bounds = this.WindowBounds;

                if (bounds.Location != value)
                {
                    bounds.Offset(value.X - bounds.X, value.Y - bounds.Y);
                    this.WindowBounds = bounds;
                }
            }
        }

        internal Drawing.Size WindowSize
        {
            //get { return this.WindowBounds.Size; }
            get { return new Drawing.Size(this.Width, this.Height); }
            set
            {
                /*
                Drawing.Rectangle bounds = this.WindowBounds;

                if (bounds.Size != value)
                {
                    bounds.Size = value;
                    this.WindowBounds = bounds;
                }
                */
                throw new System.NotImplementedException();
            }
        }

        internal string Text
        {
            get { return this.windowTitle; }
            set
            {
                this.windowTitle = value;
                // bl-net8-cross
                //this.SetCaption(value);
            }
        }

        internal string Name
        {
            get { return this.windowName; }
            set { this.windowName = value; }
        }

        internal Drawing.Image Icon
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull
            get
            {
                /*
                if (base.Icon == null)
                {
                    return null;
                }

                return Drawing.Bitmap.FromNativeBitmap(base.Icon.ToBitmap());
                */
                throw new NotImplementedException();
                return null;
            }
            set
            {
                /*
                if (value == null)
                {
                    base.Icon = null;
                }
                else
                {
                    base.Icon = System.Drawing.Icon.FromHandle(
                        value.BitmapImage.NativeBitmap.GetHicon()
                    );
                }
                */
                throw new NotImplementedException();
            }
        }

        internal void SetNativeIcon(System.IO.Stream iconStream)
        {
            /*
            System.Drawing.Icon nativeIcon = new System.Drawing.Icon(iconStream);
            base.Icon = nativeIcon;
            */
            throw new NotImplementedException();
        }

        internal void SetNativeIcon(System.IO.Stream iconStream, int dx, int dy)
        {
            /*
            byte[] buffer = new byte[iconStream.Length];
            iconStream.Read(buffer, 0, buffer.Length);
            string path = System.IO.Path.GetTempFileName();

            int smallDx = Bitmap.GetIconWidth(IconSize.Small);
            int smallDy = Bitmap.GetIconHeight(IconSize.Small);

            int largeDx = Bitmap.GetIconWidth(IconSize.Normal);
            int largeDy = Bitmap.GetIconHeight(IconSize.Normal);

            try
            {
                System.IO.File.WriteAllBytes(path, buffer);
                var nativeIcon = Epsitec.Common.Drawing.Bitmap.LoadNativeIcon(path, dx, dy);

                //	This does not work (see http://stackoverflow.com/questions/2266479/setclasslonghwnd-gcl-hicon-hicon-cannot-replace-winforms-form-icon)
                if ((dx == smallDx) && (dy == smallDy))
                {
                    Win32Api.SetClassLong(
                        this.Handle,
                        Win32Const.GCL_HICONSM,
                        nativeIcon.Handle.ToInt32()
                    );
                    Win32Api.SendMessage(
                        this.Handle,
                        Win32Const.WM_SETICON,
                        (System.IntPtr)Win32Const.ICON_SMALL,
                        nativeIcon.Handle
                    );
                }
                else if ((dx == largeDx) && (dy == largeDy))
                {
                    Win32Api.SetClassLong(
                        this.Handle,
                        Win32Const.GCL_HICON,
                        nativeIcon.Handle.ToInt32()
                    );
                    Win32Api.SendMessage(
                        this.Handle,
                        Win32Const.WM_SETICON,
                        (System.IntPtr)Win32Const.ICON_BIG,
                        nativeIcon.Handle
                    );
                }
                else
                {
                    base.Icon = nativeIcon;
                }
            }
            finally
            {
                System.IO.File.Delete(path);
            }
            */
            throw new NotImplementedException();
        }

        internal double Alpha
        {
            get { return this.alpha; }
            set
            {
                if (this.alpha != value)
                {
                    this.alpha = value;
                    this.UpdateLayeredWindow();
                }
            }
        }

        internal System.Drawing.Size BorderSize
        {
            get
            {
                /*
                System.Drawing.Size borderSize = new System.Drawing.Size(0, 0);

                switch (this.FormBorderStyle)
                {
                    case System.Windows.Forms.FormBorderStyle.Fixed3D:
                        borderSize = System.Windows.Forms.SystemInformation.Border3DSize;
                        break;

                    case System.Windows.Forms.FormBorderStyle.Sizable:
                    case System.Windows.Forms.FormBorderStyle.SizableToolWindow:
                        borderSize = System.Windows.Forms.SystemInformation.FrameBorderSize;
                        break;

                    case System.Windows.Forms.FormBorderStyle.FixedDialog:
                        borderSize = System.Windows.Forms.SystemInformation.FixedFrameBorderSize;
                        break;

                    case System.Windows.Forms.FormBorderStyle.FixedSingle:
                    case System.Windows.Forms.FormBorderStyle.FixedToolWindow:
                        borderSize = new System.Drawing.Size(1, 1);
                        break;

                    case System.Windows.Forms.FormBorderStyle.None:
                        break;
                }

                return borderSize;
                */
                //return System.Drawing.Size.Empty;
                throw new System.NotImplementedException();
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

        internal Epsitec.Common.Widgets.Window HostingWidgetWindow
        {
            get { return this.widgetWindow; }
        }

        internal static bool UseWaitCursor
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull

            //get { return System.Windows.Forms.Application.UseWaitCursor; }
            get { return true; }
            //set { System.Windows.Forms.Application.UseWaitCursor = value; }
            set { }
        }

        internal void Close()
        {
            // bl-net8-cross
            // old thing from winforms, see if still usefull
            /*
            try
            {
                this.forcedClose = true;
                base.Close();
            }
            finally
            {
                this.forcedClose = false;
            }
            */
            throw new NotImplementedException();
        }

        internal void SimulateCloseClick()
        {
            /*
            base.Close();
            */
            throw new NotImplementedException();
        }

        internal void SetFrozen(bool frozen)
        {
            this.isFrozen = frozen;
        }

        protected void Dispose(bool disposing)
        //protected override void Dispose(bool disposing)
        {
            /*
            if (disposing)
            {
                WindowList.Remove(this);

                //	Attention: il n'est pas permis de faire un Dispose si l'appelant provient d'une
                //	WndProc, car cela perturbe le bon acheminement des messages dans Windows. On
                //	préfère donc remettre la destruction à plus tard si on détecte cette condition.

                if (this.wndProcDepth > 0)
                {
                    Win32Api.PostMessage(
                        this.Handle,
                        Win32Const.WM_APP_DISPOSE,
                        System.IntPtr.Zero,
                        System.IntPtr.Zero
                    );
                    return;
                }

                if (this.graphics != null)
                {
                    this.graphics.Dispose();
                }

                this.graphics = null;

                if (this.widgetWindow != null)
                {
                    if (this.widgetWindowDisposed == false)
                    {
                        this.widgetWindow.PlatformWindowDisposing();
                        this.widgetWindowDisposed = true;
                    }
                }
            }

            base.Dispose(disposing);
            */
            throw new NotImplementedException();
        }

        protected void OnClosed(System.EventArgs e)
        //protected override void OnClosed(System.EventArgs e)
        {
            /*
            if (this.Focused)
            {
                //	Si la fenêtre avait le focus et qu'on la ferme, on aimerait bien que
                //	si elle avait une fenêtre "parent", alors ce soit le parent qui reçoive
                //	le focus à son tour. Ca paraît logique.

                System.Windows.Forms.Form form = this.Owner;

                if (form != null)
                {
                    form.Activate();
                }
            }

            if (this.widgetWindow != null)
            {
                this.widgetWindow.OnWindowClosed();
            }

            base.OnClosed(e);
            */
            throw new NotImplementedException();
        }

        protected void OnGotFocus(System.EventArgs e)
        //protected override void OnGotFocus(System.EventArgs e)
        {
            /*
            base.OnGotFocus(e);
            if (this.widgetWindow != null)
            {
                this.widgetWindow.NotifyWindowFocused();
            }
            */
            throw new NotImplementedException();
        }

        protected void OnLostFocus(System.EventArgs e)
        //protected override void OnLostFocus(System.EventArgs e)
        {
            /*
            base.OnLostFocus(e);
            if (this.widgetWindow != null)
            {
                this.widgetWindow.NotifyWindowDefocused();
            }
            */
            throw new NotImplementedException();
        }

        protected void OnClosing(System.ComponentModel.CancelEventArgs e)
        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            /*
            base.OnClosing(e);

            if (this.forcedClose)
            {
                return;
            }

            if (this.widgetWindow == null)
            {
                return;
            }

            this.widgetWindow.OnWindowCloseClicked();

            if (this.preventClose)
            {
                e.Cancel = true;

                if (this.preventQuit)
                {
                    return;
                }

                CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher(this.widgetWindow);

                //	Don't generate an Alt-F4 event if there is no dispatcher attached with this
                //	window, as it would probably bubble up to the owner window and cause the
                //	application to quit...

                if (dispatcher == null)
                {
                    return;
                }

                //	Empêche la fermeture de la fenêtre lorsque l'utilisateur clique sur le bouton de
                //	fermeture, et synthétise un événement clavier ALT + F4 à la place...

                System.Windows.Forms.Keys altF4 =
                    System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt;
                System.Windows.Forms.KeyEventArgs fakeEvent = new System.Windows.Forms.KeyEventArgs(
                    altF4
                );
                Message message = Message.FromKeyEvent(MessageType.KeyDown, fakeEvent);
                message.MarkAsDummyMessage();
                this.DispatchMessage(message);
            }
            */
            throw new NotImplementedException();
        }

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

        protected void OnVisibleChanged(System.EventArgs e)
        //protected override void OnVisibleChanged(System.EventArgs e)
        {
            /*
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                if (!this.isPixmapOk)
                {
                    this.ReallocatePixmap();
                }
                if (this.widgetWindow != null)
                {
                    this.widgetWindow.OnWindowShown();
                }
            }
            else
            {
                if (this.widgetWindow != null)
                {
                    this.widgetWindow.OnWindowHidden();
                }
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

        internal int MapToWinFormsX(double x)
        {
            return (int)System.Math.Floor(x + 0.5);
        }

        internal int MapToWinFormsY(double y)
        {
            return (int)ScreenInfo.PrimaryHeight - (int)System.Math.Floor(y + 0.5);
        }

        internal int MapToWinFormsWidth(double width)
        {
            return (int)(width + 0.5);
        }

        internal int MapToWinFormsHeight(double height)
        {
            return (int)(height + 0.5);
        }

        internal double MapFromWinFormsX(int x)
        {
            return x;
        }

        internal double MapFromWinFormsY(int y)
        {
            return ScreenInfo.PrimaryHeight - y;
        }

        internal double MapFromWinFormsWidth(int width)
        {
            return width;
        }

        internal double MapFromWinFormsHeight(int height)
        {
            return height;
        }

        internal void MarkForRepaint()
        {
            // repaint all
            this.MarkForRepaint(
                new Drawing.Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height)
            );
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
            throw new System.NotImplementedException();
        }

        internal void SynchronousRepaint()
        {
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
            throw new System.NotImplementedException();
        }

        internal void SendQueueCommand()
        {
            /*
            if (this.InvokeRequired)
            {
                this.Invoke(new SimpleCallback(this.SendQueueCommand));
            }
            else
            {
                Win32Api.PostMessage(
                    this.Handle,
                    Win32Const.WM_APP_EXEC_CMD,
                    System.IntPtr.Zero,
                    System.IntPtr.Zero
                );
            }
            */
            throw new System.NotImplementedException();
        }

        internal void SendValidation()
        {
            /*
            Win32Api.PostMessage(
                this.Handle,
                Win32Const.WM_APP_VALIDATION,
                System.IntPtr.Zero,
                System.IntPtr.Zero
            );
            */
            throw new System.NotImplementedException();
        }

        internal static void SendSynchronizeCommandCache()
        {
            // bl-net8-cross
            /*
            Window.isSyncRequested = true;

            try
            {
                Win32Api.PostMessage(
                    Window.dispatchWindowHandle,
                    Win32Const.WM_APP_SYNCMDCACHE,
                    System.IntPtr.Zero,
                    System.IntPtr.Zero
                );
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Exception thrown in Platform.Window.SendSynchronizeCommandCache :"
                );
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            */
            throw new System.NotImplementedException();
        }

        internal static void SendAwakeEvent()
        {
            // bl-net8-cross
            /*
            bool awake = false;

            lock (Window.dispatchWindow)
            {
                if (Window.isAwakeRequested == false)
                {
                    Window.isAwakeRequested = true;
                    awake = true;
                }
            }

            if (awake)
            {
                try
                {
                    Win32Api.PostMessage(
                        Window.dispatchWindowHandle,
                        Win32Const.WM_APP_AWAKE,
                        System.IntPtr.Zero,
                        System.IntPtr.Zero
                    );
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Exception thrown in Platform.Window.SendAwakeEvent:"
                    );
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            */
            throw new System.NotImplementedException();
        }

        internal Platform.Window FindRootOwner()
        {
            /*
            Window owner = this.Owner as Window;

            if (owner != null)
            {
                return owner.FindRootOwner();
            }

            return this;
            */
            throw new System.NotImplementedException();
            return null;
        }

        internal Platform.Window[] FindOwnedWindows()
        {
            /*
            System.Windows.Forms.Form[] forms = this.OwnedForms;
            Platform.Window[] windows = new Platform.Window[forms.Length];

            for (int i = 0; i < forms.Length; i++)
            {
                windows[i] = forms[i] as Platform.Window;
            }

            return windows;
            */
            throw new System.NotImplementedException();
            return null;
        }

        internal bool StartWindowManagerOperation(WindowManagerOperation op)
        {
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
                    System.Windows.Forms.Message message = Window.CreateNCActivate(this, active);
                    //					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} faking WM_NCACTIVATE {1}.", this.Name, active));
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
                Window window = forms[i] as Window;

                if (window != null)
                {
                    if (window.isToolWindow)
                    {
                        window.FakeActivate(active);
                    }
                }
            }
            */
            throw new System.NotImplementedException();
        }

        protected bool IsOwnedWindow(Platform.Window find)
        {
            /*
            System.Windows.Forms.Form[] forms = this.OwnedForms;

            for (int i = 0; i < forms.Length; i++)
            {
                Window window = forms[i] as Window;

                if (window != null)
                {
                    if (window == find)
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

        protected bool RefreshGraphics()
        {
            /*
            if (this.isLayoutInProgress)
            {
                return false;
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

            if (this.IsFrozen)
            {
                return false;
            }

            return this.RefreshGraphicsLowLevel();
            */
            throw new System.NotImplementedException();
        }

        private bool RefreshGraphicsLowLevel()
        {
            /*
            Drawing.Rectangle repaint = this.dirtyRectangle;
            Drawing.Rectangle[] strips = this.dirtyRegion.GenerateStrips();

            this.dirtyRectangle = Drawing.Rectangle.Empty;
            this.dirtyRegion = new Drawing.DirtyRegion();

            if (this.widgetWindow != null)
            {
                this.widgetWindow.RefreshGraphics(this.graphics, repaint, strips);
            }

            return true;
            */
            throw new System.NotImplementedException();
        }

        protected bool UpdateLayeredWindow()
        {
            /*
            bool paintNeeded = true;

            this.RefreshGraphics();

            /*
            if (this.isLayered)
            {
                if (this.isLayeredDirty)
                {
                    this.isLayeredDirty = false;
                }

                //	UpdateLayeredWindow can be called as the result of setting a
                //	new WindowBounds rectangle. If this is the case, the Bounds
                //	property, inherited from WinForms, won't have been updated
                //	yet, so use the cached value :

                System.Drawing.Rectangle rect;

                if (this.formBoundsSet)
                {
                    rect = this.formBounds;
                }
                else
                {
                    rect = this.Bounds;
                }

                //	Copy the bits from the source buffer to the destination buffer
                //	which must be premultiplied AlphaRGB.

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                    rect.Width,
                    rect.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb
                );

                using (bitmap)
                {
                    Drawing.DrawingBitmap.RawData src = new Drawing.DrawingBitmap.RawData(this.graphics.DrawingBitmap);
                    Drawing.DrawingBitmap.RawData dst = new Drawing.DrawingBitmap.RawData(
                        bitmap,
                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb
                    );

                    using (src)
                    {
                        using (dst)
                        {
                            src.CopyTo(dst);
                        }
                    }

                    //					System.Diagnostics.Debug.WriteLine ("UpdateLayeredWindow" + (this.isFrozen ? " (frozen)" : "") + " Bounds: " + rect.ToString ());

                    paintNeeded = !Win32Api.UpdateLayeredWindow(
                        this.Handle,
                        bitmap,
                        rect,
                        this.alpha
                    );
                }
            }

            return paintNeeded;
            */
            throw new System.NotImplementedException();
            return true;
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
            buffer.Append("Window: ");
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

        internal void DispatchMessage(Message message)
        {
            if (this.widgetWindow != null)
            {
                this.widgetWindow.DispatchMessage(message);
            }
        }

        internal void ShowWindow()
        {
            /*
            bool ok = this.Init(
                (uint)this.clientSize.Width,
                (uint)this.clientSize.Height,
                AntigrainSharp.WindowFlags.Resize
            );
            if (!ok)
            {
                throw new Exception("Failed to initialize antigrain window");
            }
            */
            this.Show();
            this.UpdateLayeredWindow();
        }

        internal void ShowDialogWindow()
        {
            this.ShowWindow();
            ToolTip.HideAllToolTips();
        }

        private bool widgetWindowDisposed;
        private Epsitec.Common.Widgets.Window widgetWindow;

        private AntigrainSharp.AbstractGraphicBuffer renderingBuffer;
        private Drawing.Graphics graphics;
        private Drawing.Rectangle dirtyRectangle;
        private Drawing.DirtyRegion dirtyRegion;
        private Drawing.Size minimumSize;

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
        private WindowStyles windowStyles;
        private WindowType windowType;

        private bool isSizeMoveInProgress;

        private WindowPlacement windowPlacement;
    }
}
