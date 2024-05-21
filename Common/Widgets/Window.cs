//	Copyright © 2003-2014, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUDinternal void MakeTitlelessResizableWindow()

using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
    using System.Linq;
    using Epsitec.Common.Types.Collections;
    using Epsitec.Common.Widgets.Platform;
    using Epsitec.Common.Widgets.Platform.SDLWrapper;

    /// <summary>
    /// La classe Window représente une fenêtre du système d'exploitation. Ce
    /// n'est pas un widget en tant que tel: Window.Root définit le widget à la
    /// racine de la fenêtre.
    /// </summary>
    public class Window : Types.DependencyObject, Support.Data.IContainer, System.IDisposable
    {
        // ******************************************************************
        // TODO bl-net8-cross
        // implement Window (stub)
        // ******************************************************************
        public Window()
            : this(null, WindowFlags.None) { }

        public Window(WindowFlags windowFlags)
            : this(null, windowFlags) { }

        internal Window(WindowRoot root, WindowFlags windowFlags)
        {
            this.id = System.Threading.Interlocked.Increment(ref Window.nextWindowId);
            this.thread = System.Threading.Thread.CurrentThread;

            this.components = new Support.Data.ComponentCollection(this);
            this.ownedWindows = new HashSet<Window>();

            this.root = root ?? new WindowRoot(this);
            this.window = new PlatformWindow(this, windowFlags);
            this.timer = new Timer();

            this.root.Name = "Root";

            this.timer.TimeElapsed += this.HandleTimeElapsed;
            this.timer.AutoRepeat = 0.050;

            Window.windows.Add(this);
        }

        public long GetWindowSerialId()
        {
            return this.id;
        }

        public void OnResize(int width, int height)
        {
            this.root.NotifyWindowSizeChanged(width, height);
        }

        public void Run()
        {
            SDLWindowManager.RunApplicationEventLoop();
        }

        public static void Quit()
        {
            /*
            System.Windows.Forms.Application.Exit();
            */
        }

        public static void InvalidateAll(Window.InvalidateReason reason)
        {
            foreach (Window target in Window.windows)
            {
                if (target.IsDisposed)
                {
                    continue;
                }

                WindowRoot root = target.Root;

                switch (reason)
                {
                    case InvalidateReason.Generic:
                        root.Invalidate();
                        break;

                    case InvalidateReason.AdornerChanged:
                        root.NotifyAdornerChanged();
                        root.Invalidate();
                        break;

                    case InvalidateReason.CultureChanged:
                        root.NotifyCultureChanged();
                        break;
                }
            }
        }

        public static IEnumerable<Window> GetAllLiveWindows()
        {
            var currentThread = System.Threading.Thread.CurrentThread;
            return Window.windows.FindAll(window =>
                !window.IsDisposed && window.Thread == currentThread
            );
        }

        public static void GrabScreen(Drawing.Image bitmap, int x, int y)
        {
            // Win32Api.GrabScreen(bitmap, x, y);
            throw new System.NotImplementedException();
        }

        public static Window FindFirstLiveWindow()
        {
            return Window.windows.FindFirst(window => !window.IsDisposed);
        }

        public static Window[] FindFromPosition(Drawing.Point pos)
        {
            List<Window> list = new List<Window>();
            list.AddRange(
                Window.windows.FindAll(window =>
                    !window.IsDisposed && window.WindowBounds.Contains(pos) && window.IsVisible
                )
            );
            return list.ToArray();
        }

        public static Window FindFromText(string text)
        {
            return Window.windows.FindFirst(window => !window.IsDisposed && window.Text == text);
        }

        public static Window FindFromName(string name)
        {
            return Window.windows.FindFirst(window => !window.IsDisposed && window.Name == name);
        }

        public static Window FindCapturing()
        {
            return Window.windows.FindFirst(window =>
                !window.IsDisposed && window.CapturingWidget != null
            );
        }

        public static void PumpEvents()
        {
            /*
            System.Windows.Forms.Application.DoEvents();
            */
        }

        public void MakeLayeredWindow()
        {
            this.window.IsLayered = true;
        }

        public void MakeLayeredWindow(bool layered)
        {
            this.window.IsLayered = layered;
        }

        public void MakeActive()
        {
            if (!this.IsDisposed)
            {
                this.window.Activate();
            }
        }

        public void MakeFocused()
        {
            if (!this.IsDisposed)
            {
                this.window.Focus();
            }
        }

        public void SetNativeIconFromManifest(
            System.Reflection.Assembly assembly,
            string resourceName
        )
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                this.window.SetNativeIcon(stream);
            }
        }

        public void SetNativeIconFromManifest(
            System.Reflection.Assembly assembly,
            string resourceName,
            int dx,
            int dy
        )
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                this.window.SetNativeIcon(stream, dx, dy);
            }
        }

        public void DisableMouseActivation()
        {
            this.window.IsMouseActivationEnabled = false;
        }

        public bool StartWindowManagerOperation(Platform.WindowManagerOperation op)
        {
            if (this.window != null)
            {
                return this.window.StartWindowManagerOperation(op);
            }
            return false;
        }

        public virtual void Show()
        {
            this.AsyncValidation();

            if (this.showCount == 0)
            {
                this.showCount++;
                //				this.root.InternalUpdateGeometry ();
                this.root.Invalidate();
            }

            if (this.IsVisible == false)
            {
                this.OnAboutToShowWindow();
            }

            this.window.Show();
        }

        public virtual void ShowDialog()
        {
            this.AsyncValidation();

            Window.GetAllLiveWindows().ForEach(window => window.RefreshEnteredWidgets(null));

            if (this.showCount == 0)
            {
                this.showCount++;
                //				this.root.InternalUpdateGeometry ();
            }

            if (this.IsVisible == false)
            {
                this.OnAboutToShowWindow();
            }

            this.window.ShowDialogWindow();
            this.DispatchQueuedCommands();
        }

        public virtual void Hide()
        {
            if (this.IsVisible)
            {
                this.OnAboutToHideWindow();
                this.window.Hide();
            }
        }

        public virtual void Close()
        {
            if (this.IsVisible)
            {
                this.OnAboutToHideWindow();
            }

            // FIXME there seem to be an infinite when we try to close a popup window
            this.window.Close();
        }

        public void AdjustWindowSize()
        {
            this.ForceLayout();
            Drawing.Size size = this.root.RealMinSize;

            if (this.ClientSize.Width > size.Width)
            {
                size.Width = this.ClientSize.Width;
            }
            if (this.ClientSize.Height > size.Height)
            {
                size.Height = this.ClientSize.Height;
            }

            this.ClientSize = size;
        }

        public void SynchronousRepaint()
        {
            if (this.window != null)
            {
                this.window.SynchronousRepaint();
            }
        }

        public void AnimateShow(Animation animation)
        {
            if (this.IsVisible == false)
            {
                this.OnAboutToShowWindow();
            }
            this.window.AnimateShow(animation, this.WindowBounds);
        }

        public void AnimateShow(Animation animation, Drawing.Rectangle bounds)
        {
            if (this.IsVisible == false)
            {
                this.OnAboutToShowWindow();
            }
            this.window.AnimateShow(animation, bounds);
        }

        public void AnimateHide(Animation animation)
        {
            if (this.IsVisible)
            {
                this.OnAboutToHideWindow();
            }
            this.window.AnimateHide(animation, this.WindowBounds);
        }

        public void GenerateDummyMouseMoveEvent()
        {
            //Drawing.Point pos = Message.CurrentState.LastScreenPosition;
            //this.DispatchMessage(Message.CreateDummyMouseMoveEvent(this.ScreenPointToWindowPoint(pos)));
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the bitmap of the (repainted) window contents.
        /// </summary>
        /// <returns>The bitmap.</returns>
        public Drawing.Bitmap GetWindowBitmap()
        {
            this.SynchronousRepaint();

            var pixmap = this.window.GetWindowPixmap();

            if (pixmap == null)
            {
                return null;
            }
            else
            {
                return Drawing.Bitmap.FromPixmap(pixmap) as Drawing.Bitmap;
            }
        }

        public WindowRoot Root
        {
            get { return this.root; }
        }

        #region Ownership system
        public Window Owner
        {
            get { return this.owner; }
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentException("Window owner cannot be set to null");
                }
                if (this.owner != null)
                {
                    throw new System.InvalidOperationException("Window owner already set");
                }
                if (this.owner != value)
                {
                    this.owner = value;
                    this.owner.RegisterOwnedWindow(this);
                    Helpers.VisualTree.InvalidateCommandDispatcher(this);
                }
            }
        }

        public Window[] OwnedWindows
        {
            get
            {
                if (this.window == null)
                {
                    return [];
                }
                return this.ownedWindows.ToArray();
            }
        }

        private void RegisterOwnedWindow(Window child)
        {
            this.ownedWindows.Add(child);
        }

        private void RemoveOwnedWindow(Window child)
        {
            this.ownedWindows.Remove(child);
        }
        #endregion

        public Widget FocusedWidget
        {
            get
            {
                if ((this.focusedWidget != null) && (this.focusedWidget.Window == this))
                {
                    return this.focusedWidget;
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                if (this.focusedWidget != value)
                {
                    Widget oldFocus = this.focusedWidget;
                    Widget newFocus = value;

                    this.focusedWidget = null;

                    if (oldFocus != null)
                    {
                        oldFocus.InternalAboutToLoseFocus(
                            TabNavigationDir.None,
                            TabNavigationMode.None
                        );
                        oldFocus.SetFocused(false);
                    }

                    this.focusedWidget = newFocus;

                    if (newFocus != null)
                    {
                        newFocus.SetFocused(true);
                    }

                    this.OnFocusedWidgetChanged(
                        new DependencyPropertyChangedEventArgs("FocusedWidget", oldFocus, newFocus)
                    );
                }
            }
        }

        public Widget EngagedWidget
        {
            get { return this.engagedWidget; }
            set
            {
                if (this.engagedWidget != value)
                {
                    Widget oldEngage = this.engagedWidget;
                    Widget newEngage = value;

                    this.engagedWidget = null;

                    if (oldEngage != null)
                    {
                        oldEngage.SetEngaged(false);
                        this.timer.Stop();
                    }

                    this.engagedWidget = newEngage;

                    if (newEngage != null)
                    {
                        newEngage.SetEngaged(true);

                        if (newEngage.AutoRepeat)
                        {
                            this.timer.Stop();
                            this.timer.AutoRepeat = newEngage.AutoEngageDelay;
                            this.timer.Start();
                        }
                    }
                }
            }
        }

        public Widget CapturingWidget
        {
            get { return this.capturingWidget; }
        }

        public Widget ModalWidget
        {
            get { return this.modalWidget; }
            set { this.modalWidget = value; }
        }

        public IPaintFilter PaintFilter
        {
            get { return this.paintFilter; }
            set { this.paintFilter = value; }
        }

        public MouseCursor MouseCursor
        {
            set
            {
                if (this.window == null)
                {
                    return;
                }
                this.windowCursor = value;
                this.windowCursor.Use();
            }
        }

        public bool IsSyncPaintDisabled
        {
            get { return (this.window == null); }
        }

        public bool IsVisible
        {
            get
            {
                if (this.window != null)
                {
                    return this.window.IsVisible;
                }

                return false;
            }
        }

        public bool IsActive
        {
            get { return this.window.IsActive; }
        }

        public bool IsFrozen
        {
            get { return (this.window == null) || this.window.IsFrozen; }
            set
            {
                if (this.window != null)
                {
                    this.window.SetFrozen(value);
                }
            }
        }

        public System.Threading.Thread Thread
        {
            get { return this.thread; }
        }

        #region IIsDisposed Members

        public bool IsDisposed
        {
            get { return (this.window == null); }
        }

        #endregion

        public bool IsFocused
        {
            get
            {
                if ((this.window != null) && (this.window.IsAnimatingActiveWindow))
                {
                    return true;
                }

                return this.windowIsFocused;
            }
        }

        public bool IsOwned
        {
            get { return (this.owner != null); }
        }

        public bool IsFullScreen
        {
            get { return this.window.IsFullScreen; }
            set { this.window.IsFullScreen = value; }
        }

        public bool IsMinimized
        {
            /*            get
                        {
                            return (this.window != null)
                                && (this.window.WindowState == System.Windows.Forms.FormWindowState.Minimized);
                        }
            */get
            {
                throw new System.NotImplementedException();
                return true;
            }
            set
            {
                /*                if (this.window != null)
                                {
                                    this.window.WindowState = value
                                        ? System.Windows.Forms.FormWindowState.Minimized
                                        : System.Windows.Forms.FormWindowState.Normal;
                                }
                */
                throw new System.NotImplementedException();
            }
        }

        public bool IsToolWindow
        {
            get { return this.window.IsToolWindow; }
        }

        public bool IsSizeMoveInProgress
        {
            get { return (this.window != null) && (this.window.IsSizeMoveInProgress); }
        }

        public double Alpha
        {
            get { return this.window.Alpha; }
            set { this.window.Alpha = value; }
        }

        public Drawing.Rectangle WindowBounds
        {
            get { return this.window.WindowBounds; }
            set
            {
                this.windowLocationSet = true;
                this.window.WindowBounds = value;
            }
        }

        public Drawing.Image Icon
        {
            get { return this.window.Icon; }
            set { this.window.Icon = value; }
        }

        /// <summary>
        /// Gets or sets the type of the window. See <see cref="WindowRoot.WindowType"/>
        /// if you need to set this value.
        /// </summary>
        /// <value>The type of the window.</value>
        public WindowType WindowType
        {
            get { return this.window.WindowType; }
            internal set
            {
                this.window.WindowType = value;
                this.root.WindowType = value;
            }
        }

        internal WindowMode WindowMode
        {
            get { return this.window.WindowMode; }
            set { this.window.WindowMode = value; }
        }

        public bool PreventAutoClose
        {
            get { return this.window.PreventAutoClose; }
            set { this.window.PreventAutoClose = value; }
        }

        public bool PreventAutoQuit
        {
            get { return this.window.PreventAutoQuit; }
            set { this.window.PreventAutoQuit = value; }
        }

        public bool IsValidDropTarget
        {
            get { return this.window.AllowDrop; }
            set { this.window.AllowDrop = value; }
        }

        public static bool IsApplicationActive
        {
            //get { return Platform.PlatformWindow.IsApplicationActive; }
            get { return true; }
        }

        public static int DebugAliveWindowsCount
        {
            get { return Window.windows.Count; }
        }

        public static Window[] DebugAliveWindows
        {
            get { return Window.windows.ToArray(); }
        }

        public Drawing.Point WindowLocation
        {
            get { return this.window.WindowLocation; }
            set
            {
                this.windowLocationSet = true;
                this.window.WindowLocation = value;
            }
        }

        public Drawing.Size WindowSize
        {
            get { return this.window.WindowSize; }
            set
            {
                if (this.windowLocationSet)
                {
                    this.window.WindowSize = value;
                }
                else
                {
                    //	L'utilisateur n'a jamais positionné sa fenêtre et le système
                    //	dans son immense bonté nous a proposé une origine. Si nous
                    //	changeons sa taille avec notre système de coordonnées, le
                    //	sommet ne sera plus là où l'OS aurait voulu qu'il soit. Il
                    //	faut donc repositionner en même temps que l'on redimensionne
                    //	la fenêtre :

                    Drawing.Rectangle bounds = this.WindowBounds;

                    bounds.Bottom = bounds.Top - value.Height;
                    bounds.Width = value.Width;

                    this.WindowBounds = bounds;
                    this.windowLocationSet = false;
                }
            }
        }

        public Drawing.Rectangle WindowPlacementBounds
        {
            get
            {
                if (this.window == null)
                {
                    return Drawing.Rectangle.Empty;
                }

                return this.window.WindowPlacementNormalBounds;
            }
        }

        public WindowPlacement WindowPlacement
        {
            get { return this.window.NativeWindowPlacement; }
            set { this.window.NativeWindowPlacement = value; }
        }

        public Drawing.Size ClientSize
        {
            get { return this.window.WindowSize; }
            set
            {
                if ((this.window != null) && (this.ClientSize != value))
                {
                    Drawing.Size windowSize = this.window.WindowSize;
                    Drawing.Size clientSize = this.ClientSize;

                    this.WindowSize = new Drawing.Size(
                        value.Width - clientSize.Width + windowSize.Width,
                        value.Height - clientSize.Height + windowSize.Height
                    );
                }
            }
        }

        public string Text
        {
            get { return this.text ?? ""; }
            set { this.window.Text = this.text = value; }
        }

        public string Name
        {
            get { return this.name ?? ""; }
            set { this.window.Name = this.name = value; }
        }

        public static bool RunningInAutomatedTestEnvironment
        {
            get { return Window.isRunningInAutomatedTestEnvironment; }
            set
            {
                //	En mettant cette propriété à true, l'appel RunInTestEnvironment
                //	ne démarrera pas de "message loop", ce qui évite qu'une batterie
                //	de tests automatique (NUnit) ne se bloque après l'affichage d'une
                //	fenêtre.

                if (Window.isRunningInAutomatedTestEnvironment != value)
                {
                    Window.isRunningInAutomatedTestEnvironment = value;
                }
            }
        }

        public static void RunInTestEnvironment(Window window)
        {
            /*
            //	Cette méthode doit être appelée dans des tests basés sur NUnit, lorsque
            //	l'on désire que la fenêtre reste visible jusqu'à sa fermeture manuelle.

            //	Il est conseillé de rajouter un test nommé AutomatedTestEnvironment qui
            //	met la propriété RunningInAutomatedTestEnvironment à true; ainsi, si on
            //	exécute un test global (Run sans avoir sélectionné de test spécifique),
            //	RunInTestEnvironment ne bloquera pas.

            if (PlatformWindow.RunningInAutomatedTestEnvironment)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            else
            {
                System.Windows.Forms.Application.Run(window.PlatformWindow);
            }
            */
        }

        /*        public static void RunInTestEnvironment(System.Windows.Forms.Form form)
                {
        
                    if (PlatformWindow.RunningInAutomatedTestEnvironment)
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                    else
                    {
                        System.Windows.Forms.Application.Run(form);
                    }
        
                }
        */
        public System.IDisposable PushPaintFilter(IPaintFilter filter)
        {
            return new PushPaintFilterHelper(this, filter);
        }

        #region PushPaintFilterHelper Class

        private class PushPaintFilterHelper : System.IDisposable
        {
            public PushPaintFilterHelper(Window window, IPaintFilter filter)
            {
                this.window = window;
                this.filter = window.paintFilter;

                window.paintFilter = filter;
            }

            ~PushPaintFilterHelper()
            {
                throw new System.InvalidOperationException(
                    "Caller of PushPaintFilter forgot to call Dispose"
                );
            }

            #region IDisposable Members

            public void Dispose()
            {
                System.GC.SuppressFinalize(this);

                this.window.paintFilter = this.filter;
            }

            #endregion

            private readonly Window window;
            private readonly IPaintFilter filter;
        }

        #endregion

        #region IContainer Members
        public void NotifyComponentInsertion(
            Support.Data.ComponentCollection collection,
            Support.Data.IComponent component
        ) { }

        public void NotifyComponentRemoval(
            Support.Data.ComponentCollection collection,
            Support.Data.IComponent component
        ) { }

        public Support.Data.ComponentCollection Components
        {
            get { return this.components; }
        }
        #endregion

        public void DetachDispatcher(CommandDispatcher value)
        {
            if (CommandDispatcher.GetDispatcher(this) != null)
            {
                System.Diagnostics.Debug.Assert(CommandDispatcher.GetDispatcher(this) == value);
                CommandDispatcher.ClearDispatcher(this);

                Helpers.VisualTree.InvalidateCommandDispatcher(this);
            }
        }

        public void AttachLogicalFocus(Widget widget)
        {
            this.logicalFocusStack.Remove(widget);
            this.logicalFocusStack.Insert(0, widget);
        }

        public void DetachLogicalFocus(Widget widget)
        {
            this.logicalFocusStack.Remove(widget);
        }

        public Widget FindLogicalFocus()
        {
            foreach (Widget widget in this.logicalFocusStack)
            {
                if ((widget.IsVisible) && (widget.IsEnabled))
                {
                    return widget;
                }
            }

            return null;
        }

        public bool RestoreLogicalFocus()
        {
            Widget focus = this.FindLogicalFocus();

            if ((focus != null) && (focus.AcceptsFocus) && (focus.IsFocused != true))
            {
                focus.Focus();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Refreshes the focused widget. In some edge cases, a widget which lost its parent
        /// (and possibly got it back) might no longer be properly focused. Calling this
        /// method fixes these issues.
        /// </summary>
        public void RefreshFocusedWidget()
        {
            if ((this.focusedWidget != null) && (this.focusedWidget.KeyboardFocus == false))
            {
                if (this.focusedWidget.Window != this)
                {
                    this.ClearFocusedWidget();
                }
                else
                {
                    this.focusedWidget.SetFocused(true);
                }
            }
        }

        public void RefreshEnteredWidgets()
        {
            this.RefreshEnteredWidgets(Message.GetLastMessage());
        }

        public void RefreshEnteredWidgets(Message message)
        {
            this.ForceLayout();

            Widget.ClearAllEntered();

            if (message != null)
            {
                Widget child = this.Root.FindChild(
                    message.Cursor,
                    WidgetChildFindMode.Deep | WidgetChildFindMode.SkipHidden
                );
                Widget.UpdateEntered(this, child, message);
            }
        }

        public void ToggleMaximize()
        {
            /*
            if (this.window.WindowState == System.Windows.Forms.FormWindowState.Maximized)
            {
                this.window.WindowState = System.Windows.Forms.FormWindowState.Normal;
            }
            else
            {
                this.window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }

            //			var placement = this.WindowPlacement;
            //			placement = new WindowPlacement (placement.Bounds, !placement.IsFullScreen, placement.IsMinimized, placement.IsHidden);
            //			this.WindowPlacement = placement;
            */
            throw new System.NotImplementedException();
        }

        public void ToggleMinimize()
        {
            /*
            var placement = this.WindowPlacement;

            if (this.window.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                if (placement.IsFullScreen)
                {
                    this.window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
                else
                {
                    this.window.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
            }
            else
            {
                this.window.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            }

            //			placement = new WindowPlacement (placement.Bounds, placement.IsFullScreen, !placement.IsMinimized, placement.IsHidden);
            //			this.WindowPlacement = placement;
            */
            throw new System.NotImplementedException();
        }

        public void SimulateCloseClick()
        {
            this.window.SimulateCloseClick();
        }

        protected void OnAsyncNotification()
        {
            if (this.AsyncNotification != null)
            {
                this.AsyncNotification(this);
            }
        }

        protected virtual void OnWindowDisposing()
        {
            if (this.WindowDisposing != null)
            {
                this.WindowDisposing(this);
            }
        }

        protected virtual void OnAboutToShowWindow()
        {
            this.ForceLayout();

            if (this.AboutToShowWindow != null)
            {
                this.AboutToShowWindow(this);
            }
        }

        protected virtual void OnAboutToHideWindow()
        {
            if (this.AboutToHideWindow != null)
            {
                this.AboutToHideWindow(this);
            }
        }

        internal void OnWindowActivated()
        {
            this.WindowActivated.Raise(this);
        }

        internal void OnWindowDeactivated()
        {
            this.WindowDeactivated.Raise(this);
        }

        internal void OnWindowAnimationEnded()
        {
            this.WindowAnimationEnded.Raise(this);
        }

        internal void OnWindowShown()
        {
            this.root.NotifyWindowIsVisibleChanged();

            this.WindowShown.Raise(this);
            Window.GlobalWindowShown.Raise(this);
        }

        internal void OnWindowHidden()
        {
            /*
            System.Diagnostics.Debug.Assert(this.windowIsVisible);

            this.windowIsVisible = false;

            if ((this.owner != null) && (this.window != null) && (this.window.Owner != null))
            {
                this.window.Owner = null;
            }

            this.root.NotifyWindowIsVisibleChanged();
            this.WindowHidden.Raise(this);
            PlatformWindow.GlobalWindowHidden.Raise(this);
            */
            throw new System.NotImplementedException();
        }

        internal void OnWindowClosed()
        {
            this.WindowClosed.Raise(this);
        }

        internal void OnWindowCloseClicked()
        {
            this.WindowCloseClicked.Raise(this);
        }

        internal void OnApplicationActivated()
        {
            Window.ApplicationActivated.Raise(this);
        }

        internal void OnApplicationDeactivated()
        {
            Window.ApplicationDeactivated.Raise(this);
        }

        internal void OnGlobalFocusedWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            Window.GlobalFocusedWindowChanged.Raise(this, e);
        }

        internal void NotifyWidgetRemoval(Widget widget)
        {
            if (this.EngagedWidget == widget)
            {
                this.EngagedWidget = null;
            }
            if (this.FocusedWidget == widget)
            {
                this.FocusedWidget = null;
            }
            if (this.ModalWidget == widget)
            {
                this.ModalWidget = null;
            }
        }

        public bool IsSubmenuOpen
        {
            get
            {
                Window[] windows = this.OwnedWindows;

                foreach (Window window in windows)
                {
                    if (window is MenuWindow)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal void NotifyWindowFocused()
        {
            //-			System.Diagnostics.Debug.WriteLine ("PlatformWindow focused");
            if (this.windowIsFocused == false)
            {
                if (this.focusedWidget != null)
                {
                    this.windowIsFocused = true;
                    this.focusedWidget.Invalidate(Widgets.InvalidateReason.FocusedChanged);
                }
                else
                {
                    this.windowIsFocused = true;
                }

                this.OnWindowFocused();
            }
        }

        internal void NotifyWindowDefocused()
        {
            //-			System.Diagnostics.Debug.WriteLine ("PlatformWindow de-focused");
            if ((this.windowIsFocused == true) && (this.IsSubmenuOpen == false))
            {
                if (this.focusedWidget != null)
                {
                    this.windowIsFocused = false;
                    this.focusedWidget.Invalidate(Widgets.InvalidateReason.FocusedChanged);
                }
                else
                {
                    this.windowIsFocused = false;
                }

                this.OnWindowDefocused();

                if ((this.owner != null) && (this.owner.window.Focused == false))
                {
                    this.owner.NotifyWindowDefocused();
                }
            }
        }

        protected virtual void OnWindowFocused()
        {
            this.WindowFocused.Raise(this);

            if (Window.focusedWindow != this)
            {
                var oldWindow = Window.focusedWindow;
                var newWindow = this;

                Window.focusedWindow = this;

                this.OnGlobalFocusedWindowChanged(
                    new DependencyPropertyChangedEventArgs("FocusedWindow", oldWindow, newWindow)
                );
            }
        }

        protected virtual void OnWindowDefocused()
        {
            this.WindowDefocused.Raise(this);
        }

        internal void OnWindowDragEntered(WindowDragEventArgs e)
        {
            this.WindowDragEntered.Raise(this, e);
        }

        internal void OnWindowDragExited()
        {
            this.WindowDragExited.Raise(this);
        }

        internal void OnWindowDragDropped(WindowDragEventArgs e)
        {
            this.WindowDragDropped.Raise(this, e);
        }

        internal void OnWindowSizeMoveStatusChanged()
        {
            this.WindowSizeMoveStatusChanged.Raise(this);
        }

        internal void OnWindowResizeBeginning()
        {
            this.WindowResizeBeginning.Raise(this);
        }

        internal void OnWindowResizeEnded()
        {
            this.WindowResizeEnded.Raise(this);
        }

        internal void OnWindowPlacementChanged()
        {
            this.WindowPlacementChanged.Raise(this);
        }

        internal bool FilterMessage(Message message)
        {
            if (Window.MessageFilter != null)
            {
                Window.MessageFilter(this, message);

                //	Si le message a été "absorbé" par le filtre, il ne faut en aucun
                //	cas le transmettre plus loin.

                if (message.Handled)
                {
                    if (message.Swallowed && !this.IsFrozen)
                    {
                        //	Le message a été mangé. Il faut donc aussi manger le message
                        //	correspondant si les messages viennent par paire.

                        switch (message.MessageType)
                        {
                            case MessageType.MouseDown:
                                this.window.FilterMouseMessages = true;
                                break;

                            case MessageType.KeyDown:
                                this.window.FilterKeyMessages = true;
                                break;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        protected virtual void OnFocusedWidgetChanged(DependencyPropertyChangedEventArgs e)
        {
            this.root.ClearFocusChain();
            this.FocusedWidgetChanged.Raise(this, e);
            Window.GlobalFocusedWidgetChanged.Raise(this, e);
        }

        protected virtual void OnFocusedWidgetChanging(FocusChangingEventArgs e)
        {
            this.FocusedWidgetChanging.Raise(this, e);
        }

        #region QueueItem class
        private struct QueueItem
        {
            public QueueItem(Widget source, string command)
                : this(source)
            {
                this.commandObject = null;
                this.commandShortcut = null;
                this.commandLine = command;

                System.Diagnostics.Debug.Assert(this.dispatcherChain != null);
                System.Diagnostics.Debug.Assert(this.dispatcherChain.IsEmpty == false);
                System.Diagnostics.Debug.Assert(this.contextChain != null);
            }

            public QueueItem(DependencyObject source, string command)
                : this(source)
            {
                this.commandObject = null;
                this.commandShortcut = null;
                this.commandLine = command;

                System.Diagnostics.Debug.Assert(this.dispatcherChain != null);
                System.Diagnostics.Debug.Assert(this.dispatcherChain.IsEmpty == false);
                System.Diagnostics.Debug.Assert(this.contextChain != null);
            }

            public QueueItem(Widget source, Command command, Shortcut shortcut = null)
                : this(source)
            {
                this.commandObject = command;
                this.commandLine = null;
                this.commandShortcut = shortcut;

                System.Diagnostics.Debug.Assert(this.dispatcherChain != null);
                System.Diagnostics.Debug.Assert(this.dispatcherChain.IsEmpty == false);
                System.Diagnostics.Debug.Assert(this.contextChain != null);
            }

            public QueueItem(DependencyObject source, Command command)
                : this(source)
            {
                this.commandObject = command;
                this.commandShortcut = null;
                this.commandLine = null;

                System.Diagnostics.Debug.Assert(this.dispatcherChain != null);
                System.Diagnostics.Debug.Assert(this.dispatcherChain.IsEmpty == false);
                System.Diagnostics.Debug.Assert(this.contextChain != null);
            }

            private QueueItem(object source)
            {
                this.source = source;
                this.commandObject = null;
                this.commandShortcut = null;
                this.commandLine = null;
                this.dispatcherChain = CommandDispatcherChain.BuildChain(
                    source as DependencyObject
                );
                this.contextChain = CommandContextChain.BuildChain(source as DependencyObject);
                this.commandMessage = Message.GetLastMessage();
            }

            public object Source
            {
                get { return this.source; }
            }

            public string CommandLine
            {
                get { return this.commandLine; }
            }

            public Shortcut CommandShortcut
            {
                get { return this.commandShortcut; }
            }

            public Command CommandObject
            {
                get { return this.commandObject; }
            }

            public CommandDispatcherChain DispatcherChain
            {
                get { return this.dispatcherChain; }
            }

            public CommandContextChain ContextChain
            {
                get { return this.contextChain; }
            }

            public Message CommandMessage
            {
                get { return this.commandMessage; }
            }

            private readonly object source;
            private readonly string commandLine;
            private readonly Command commandObject;
            private readonly Shortcut commandShortcut;
            private readonly CommandDispatcherChain dispatcherChain;
            private readonly CommandContextChain contextChain;
            private readonly Message commandMessage;
        }
        #endregion

        public void QueueCommand(Widget source, string name)
        {
            if (CommandDispatcherChain.BuildChain(source) == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "Command '{0}' cannot be dispatched, no dispatcher defined.",
                        name
                    )
                );
            }
            else
            {
                this.QueueCommand(new QueueItem(source, name));
            }
        }

        public void QueueCommand(DependencyObject source, string name)
        {
            if (CommandDispatcherChain.BuildChain(source) == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "Command '{0}' cannot be dispatched, no dispatcher defined.",
                        name
                    )
                );
            }
            else
            {
                this.QueueCommand(new QueueItem(source, name));
            }
        }

        public void QueueCommand(Widget source, Command command, Shortcut shortcut = null)
        {
            if (CommandDispatcherChain.BuildChain(source) == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "Command '{0}' cannot be dispatched, no dispatcher defined.",
                        command.Name
                    )
                );
            }
            else
            {
                this.QueueCommand(new QueueItem(source, command, shortcut));
            }
        }

        public void QueueCommand(DependencyObject source, Command command)
        {
            if (CommandDispatcherChain.BuildChain(source) == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format(
                        "Command '{0}' cannot be dispatched, no dispatcher defined.",
                        command.Name
                    )
                );
            }
            else
            {
                this.QueueCommand(new QueueItem(source, command));
            }
        }

        private void QueueCommand(QueueItem item)
        {
            this.cmdQueue.Enqueue(item);

            if (this.cmdQueue.Count == 1)
            {
                if (this.window != null)
                {
                    this.SendQueueCommand();
                }
            }

            if (this.window == null)
            {
                this.DispatchQueuedCommands();
            }
        }

        public void AsyncDispose()
        {
            /*
            Platform.PlatformWindow.ProcessCrossThreadOperation(() => this.window.Owner = null);

            if (Application.MainUIThread == System.Threading.Thread.CurrentThread)
            {
                this.isDisposeQueued = true;
                this.SendQueueCommand();
            }
            else
            {
                this.Dispose();
            }
            */
        }

        public void AsyncNotify()
        {
            if (this.isAsyncNotificationQueued == false)
            {
                this.isAsyncNotificationQueued = true;
                this.SendQueueCommand();
            }
        }

        public void AsyncLayout()
        {
            if (this.IsVisible)
            {
                if (this.isAsyncLayoutQueued == false)
                {
                    this.isAsyncLayoutQueued = true;
                    this.SendQueueCommand();
                }
            }
        }

        public void AsyncValidation()
        {
            if (this.pendingValidation == false)
            {
                this.pendingValidation = true;
                this.window.SendValidation();
            }
        }

        public void AsyncValidation(Widget widget)
        {
            if (!this.asyncValidationList.Contains(widget))
            {
                this.asyncValidationList.Add(widget);
                this.AsyncValidation();
            }
        }

        public static void SuspendAsyncNotify()
        {
            System.Threading.Interlocked.Increment(ref Window.asyncSuspendCount);
        }

        public static void ResumeAsyncNotify()
        {
            if (System.Threading.Interlocked.Decrement(ref Window.asyncSuspendCount) == 0)
            {
                Window[] windows = new Window[0];

                lock (Window.pendingAsyncWindows)
                {
                    windows = Window.pendingAsyncWindows.ToArray();
                    Window.pendingAsyncWindows.Clear();
                }

                foreach (Window window in windows)
                {
                    if (window.window != null)
                    {
                        window.window.SendQueueCommand();
                    }
                }
            }
        }

        private void SendQueueCommand()
        {
            if (Window.asyncSuspendCount == 0)
            {
                this.window.SendQueueCommand();
            }
            else
            {
                lock (Window.pendingAsyncWindows)
                {
                    if (!Window.pendingAsyncWindows.Contains(this))
                    {
                        Window.pendingAsyncWindows.Add(this);
                    }
                }
            }
        }

        public void ForceLayout()
        {
            if (this.recursiveLayoutCount > 0)
            {
                return;
            }

            this.recursiveLayoutCount++;

            try
            {
                Layouts.LayoutContext context = Helpers.VisualTree.GetLayoutContext(this.root);
                int counter = 0;
                int total = 0;

                if (context != null)
                {
                    if ((context.ArrangeQueueLength > 0) || (context.MeasureQueueLength > 0))
                    {
                        total = context.TotalArrangeCount;
                        counter = context.SyncArrange();
                    }
                }

                if (counter > 0)
                {
                    //					System.Diagnostics.Debug.WriteLine (string.Format ("Arranged {0} widgets in {1} passes", context.TotalArrangeCount - total, counter));
                }
            }
            finally
            {
                this.recursiveLayoutCount--;
            }

            this.SyncMinSizeWithWindowRoot();
        }

        public static void ResetMouseCursor()
        {
            Window window = Message.CurrentState.LastWindow;

            if (window != null)
            {
                window.MouseCursor = MouseCursor.Default;
            }
        }

        private void ReleaseCapturingWidget()
        {
            Widget widget = this.capturingWidget;
            MouseButtons button = this.capturingButton;

            this.ClearCapture();

            if ((widget != null) && (button != MouseButtons.None))
            {
                widget.DispatchDummyMouseUpEvent(button, this.capturingCursor);
            }
        }

        private void ClearCapture()
        {
            this.capturingWidget = null;
            this.capturingButton = MouseButtons.None;
            this.window.Capture = false;
        }

        /// <summary>
        /// Dispatches the command line commands.
        /// </summary>
        /// <returns><c>true</c> if one or several commands were dispatched; otherwise, <c>false</c>.</returns>
        public bool DispatchCommandLineCommands()
        {
            string commandLine = System.Environment.CommandLine;
            bool dispatched = false;

            //	See also DispatchQueuedCommands to understand how we get here, after
            //	an UAC elevation, for instance.

            while (commandLine.Contains(Window.DispatchCommandIdOption))
            {
                string commandArgs = commandLine.Substring(
                    commandLine.IndexOf(Window.DispatchCommandIdOption)
                );
                string commandId = commandArgs
                    .Substring(Window.DispatchCommandIdOption.Length)
                    .Split(' ')[0];

                CommandDispatcherChain dispatcherChain = CommandDispatcherChain.BuildChain(this);
                CommandContextChain contextChain = CommandContextChain.BuildChain(this);

                Command command = Command.Get(commandId);

                CommandDispatcher.Dispatch(
                    dispatcherChain,
                    contextChain,
                    command,
                    null,
                    null,
                    null
                );

                commandLine = commandArgs.Substring(commandId.Length);
                dispatched = true;
            }

            return dispatched;
        }

        internal void DispatchQueuedCommands()
        {
            while (this.cmdQueue.Count > 0)
            {
                QueueItem item = this.cmdQueue.Dequeue();
                object source = item.Source;
                string commandLine = item.CommandLine;
                Command commandObject = item.CommandObject;
                Shortcut commandShortcut = item.CommandShortcut;
                Message commandMessage = item.CommandMessage;

                if (commandObject == null)
                {
                    commandObject = Command.Get(commandLine);
                }

                System.Diagnostics.Debug.Assert(item.DispatcherChain.IsEmpty == false);

                if (
                    (commandObject != null)
                    && (commandObject.IsAdminLevelRequired)
                    && (Support.PrivilegeManager.Current.IsUserAnAdministrator == false)
                )
                {
                    //	Ooooops. Cannot execute this command without prompting for a user
                    //	account elevation. The command won't be executed on the current
                    //	thread or process, but forwarded to an elevated version of the
                    //	same executable as ours.

                    //	See also DispatchCommandLineCommands.

                    //string args = string.Concat(
                    //    PlatformWindow.DispatchCommandIdOption,
                    //    commandObject.CommandId
                    //);

                    //Support.PrivilegeManager.Current.LaunchElevated(this.window.Handle, args);
                    throw new System.NotImplementedException();
                }
                else
                {
                    //	Fine, either this is a non privileged command, or the admin level is
                    //	required for this command and user happens to be an administrator; go
                    //	ahead, dispatch the command locally :

                    CommandDispatcher.Dispatch(
                        item.DispatcherChain,
                        item.ContextChain,
                        commandObject,
                        source,
                        commandShortcut,
                        commandMessage
                    );
                }
            }

            if (this.isDisposeQueued)
            {
                this.Dispose();
            }
            if (this.isAsyncLayoutQueued)
            {
                this.isAsyncLayoutQueued = false;
                this.ForceLayout();
            }
            if (this.isAsyncNotificationQueued)
            {
                this.isAsyncNotificationQueued = false;
                this.OnAsyncNotification();
            }
        }

        internal void DispatchValidation()
        {
            Widget[] widgets = this.asyncValidationList.ToArray();

            this.asyncValidationList.Clear();
            this.pendingValidation = false;

            for (int i = 0; i < widgets.Length; i++)
            {
                if (widgets[i].IsDisposed)
                {
                    continue;
                }

                widgets[i].Validate();
            }
        }

        internal void DispatchMessage(Message message)
        {
            this.DispatchMessage(message, this.modalWidget);
        }

        internal void DispatchMessage(Message message, Widget root)
        {
            this.ForceLayout();

            if (this.IsFrozen || (message == null))
            {
                return;
            }

            if (message.IsMouseType && message.MessageType != MessageType.MouseLeave)
            {
                this.lastInWidget = this.DetectWidget(message.Cursor);

                System.Diagnostics.Debug.Assert(this.lastInWidget != null);

                if (this.capturingWidget == null)
                {
                    //	C'est un message souris. Nous allons commencer par vérifier si tous les widgets
                    //	encore marqués comme IsEntered contiennent effectivement encore la souris. Si non,
                    //	on les retire de la liste en leur signalant qu'ils viennent de perdre la souris.

                    Widget.UpdateEntered(this, message);
                    this.lastInWidget.InternalSetEntered();
                }
                else
                {
                    //	Un widget a capturé les événements souris. Il ne faut donc gérer l'état Entered
                    //	uniquement pour ce widget-là.

                    Widget.UpdateEntered(this, this.capturingWidget, message);
                }
            }

            message.InWidget = this.lastInWidget;
            message.Consumer = null;

            if (this.capturingWidget == null)
            {
                //	La capture des événements souris n'est pas active. Si un widget a le focus, il va
                //	recevoir les événements clavier en priorité (message.FilterOnlyFocused = true).
                //	Dans les autres cas, les événements sont simplement acheminés de widget en widget,
                //	en utilisant une approche en profondeur d'abord.

                Drawing.Point pos = message.Cursor;

                if (root == null)
                {
                    root = this.root;
                }
                else
                {
                    pos = root.MapRootToClient(pos);
                    pos = root.MapClientToParent(pos);
                }

                root.MessageHandler(message, pos);

                if ((this.IsDisposed) || (message.Retired))
                {
                    return;
                }

                Window window = this;

                while (message.Handled == false)
                {
                    //	Le message n'a pas été consommé. Regarde si nous avons à faire
                    //	à une fenêtre chaînée avec un parent.

                    // bl-net8-cross I don't see why we would need to get anything in screen coordinates
                    //pos = Helpers.VisualTree.MapParentToScreen(root, pos);
                    root = MenuWindow.GetParentWidget(window);

                    if ((root == null) || (root.IsVisible == false))
                    {
                        break;
                    }

                    //pos = Helpers.VisualTree.MapScreenToParent(root, pos);
                    window = root.Window;

                    root.MessageHandler(message, pos);
                }

                this.window.Capture = false;
            }
            else
            {
                message.FilterNoChildren = true;
                message.Captured = true;

                Widget widget = this.capturingWidget;

                if (message.MessageType == MessageType.MouseUp)
                {
                    this.ClearCapture();
                }

                widget.MessageHandler(message);
            }

            if ((this.IsDisposed) || (message.Retired))
            {
                return;
            }

            this.PostProcessMessage(message);
#if false
			if ((this.window != null) &&
				(Platform.Window.IsInAnyWndProc == false))
			{
				Application.ExecuteAsyncCallbacks ();
			}
#endif
        }

        public static void SetCaptureAndRetireMessage(Widget widget, Message message)
        {
            Window window = widget.Window;

            if (window.capturingWidget != null)
            {
                if (window.capturingWidget.IsVisible == false)
                {
                    //	Il faut terminer la capture si le widget n'est plus visible,
                    //	sinon on risque de ne plus jamais recevoir d'événements pour
                    //	les autres widgets.

                    window.ReleaseCapturingWidget();
                }
            }

            window.capturingWidget = widget;
            window.capturingButton = message.Button;
            window.capturingCursor = message.Cursor;
            window.window.Capture = true;

            message.Captured = true;
            message.Retired = true;
        }

        public void ReleaseCapture()
        {
            this.ReleaseCapturingWidget();
        }

        public bool FocusWidget(Widget widget)
        {
            return this.FocusWidget(widget, TabNavigationDir.None, TabNavigationMode.None);
        }

        public bool FocusWidget(Widget widget, TabNavigationDir dir, TabNavigationMode mode)
        {
            if (
                (widget != null)
                && (widget.IsFocused == false)
                && (widget.AcceptsFocus)
                && (widget != this.focusedWidget)
            )
            {
                //	On va réaliser un changement de focus. Mais pour cela, il faut que le widget
                //	ayant le focus actuellement, ainsi que le widget candidat pour l'obtention du
                //	focus soient d'accord...

                if (
                    (this.focusedWidget == null)
                    || (
                        this.focusedWidget.AcceptsDefocus
                        && this.focusedWidget.InternalAboutToLoseFocus(dir, mode)
                    )
                )
                {
                    Widget focus;

                    if (widget.InternalAboutToGetFocus(dir, mode, out focus))
                    {
                        FocusChangingEventArgs e = new FocusChangingEventArgs(
                            this.focusedWidget,
                            focus,
                            dir,
                            mode
                        );

                        this.OnFocusedWidgetChanging(e);

                        if (e.Cancel)
                        {
                            //	Do nothing - the listener decided to cancel the event.
                        }
                        else
                        {
                            this.FocusedWidget = focus;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ClearFocusedWidget()
        {
            this.FocusedWidget = null;
        }

        public void SetEngageTimerDelay(double delay)
        {
            this.timer.Delay = delay;
        }

        internal void PostProcessMessage(Message message)
        {
            Widget consumer = message.Consumer;

            if (message.MessageType == MessageType.KeyUp)
            {
                if (this.EngagedWidget != null)
                {
                    this.EngagedWidget.SimulateReleased();
                    this.EngagedWidget.SimulateClicked();

                    this.EngagedWidget = null;
                }
            }

            if (message.IsKeyType)
            {
                //	If the user pressed or released one of the super-shift keys,
                //	we synthesize a mouse move event, so that the widget which
                //	contains the cursor may update itself accordingly :

                switch (message.KeyCodeOnly)
                {
                    case KeyCode.ShiftKey:
                    case KeyCode.ShiftKeyLeft:
                    case KeyCode.ShiftKeyRight:
                    case KeyCode.ControlKey:
                    case KeyCode.ControlKeyLeft:
                    case KeyCode.ControlKeyRight:
                    case KeyCode.AltKey:
                    case KeyCode.AltKeyLeft:
                    case KeyCode.AltKeyRight:
                        this.GenerateDummyMouseMoveEvent();
                        break;
                }
            }

            if (this.capturingWidget != null)
            {
                if (this.capturingWidget.IsVisible == false)
                {
                    //	Il faut terminer la capture si le widget n'est plus visible,
                    //	sinon on risque de ne plus jamais recevoir d'événements pour
                    //	les autres widgets.

                    this.ReleaseCapturingWidget();
                }
            }

            if ((consumer != null) && (consumer.Visibility))
            {
                switch (message.MessageType)
                {
                    case MessageType.MouseDown:
                        if ((consumer.AutoCapture) || (message.ForceCapture))
                        {
                            this.capturingWidget = consumer;
                            this.capturingButton = message.Button;
                            this.capturingCursor = message.Cursor;
                            this.window.Capture = true;
                        }
                        else
                        {
                            this.window.Capture = false;
                        }

                        if ((consumer.AutoFocus) && (message.CancelFocus == false))
                        {
                            this.FocusWidget(consumer);
                        }

                        if (message.IsLeftButton)
                        {
                            if (
                                (consumer.AutoEngage)
                                && (consumer.IsEngaged == false)
                                && (consumer.CanEngage)
                            )
                            {
                                this.EngagedWidget = consumer;
                                this.initiallyEngagedWidget = consumer;
                            }
                        }
                        break;

                    case MessageType.MouseUp:
                        this.ClearCapture();

                        if (message.IsLeftButton)
                        {
                            if (
                                (consumer.AutoToggle)
                                && (consumer.IsEnabled)
                                && (consumer.IsEntered)
                                && (!consumer.IsFrozen)
                            )
                            {
                                //	Change l'état du bouton...

                                consumer.Toggle();
                            }

                            this.EngagedWidget = null;
                            this.initiallyEngagedWidget = null;
                        }
                        break;

                    case MessageType.MouseLeave:
                        if (consumer.IsEngaged)
                        {
                            this.EngagedWidget = null;
                        }
                        break;

                    case MessageType.MouseEnter:
                        if (
                            (Message.CurrentState.IsLeftButton)
                            && (Message.CurrentState.IsSameWindowAsButtonDown)
                            && (this.initiallyEngagedWidget == consumer)
                            && (consumer.AutoEngage)
                            && (consumer.IsEngaged == false)
                            && (consumer.CanEngage)
                        )
                        {
                            this.EngagedWidget = consumer;
                        }
                        break;
                }
            }
            else if (!message.Handled)
            {
                Shortcut shortcut = Shortcut.FromMessage(message);

                if (shortcut != null)
                {
                    if (this.root.ShortcutHandler(shortcut))
                    {
                        message.Handled = true;
                        message.Swallowed = true;
                    }
                }

                if ((!message.Handled) && (this.owner != null))
                {
                    //	Le message n'a pas été traité. Peut-être qu'il pourrait intéresser la fenêtre
                    //	parent; pour l'instant, on poubellise simplement l'événement, car le faire
                    //	remonter m'a causé passablement de surprises...
                }
            }

            if (message.Swallowed)
            {
                //	Le message a été mangé. Il faut donc aussi manger le message
                //	correspondant si les messages viennent par paire.

                switch (message.MessageType)
                {
                    case MessageType.MouseDown:
                        this.window.FilterMouseMessages = true;
                        this.capturingWidget = null;
                        this.capturingButton = MouseButtons.None;
                        break;

                    case MessageType.KeyDown:
                        this.window.FilterKeyMessages = true;
                        break;
                }
            }
        }

        internal void RefreshGraphics(
            Drawing.Graphics graphics,
            Drawing.Rectangle repaint,
            Drawing.Rectangle[] strips
        )
        {
            if (this.isDisposed)
            {
                return;
            }

            if (strips.Length > 1)
            {
                //	On doit repeindre toute une série de rectangles :

                for (int i = 0; i < strips.Length; i++)
                {
                    graphics.Transform = Drawing.Transform.Identity;
                    graphics.ResetClippingRectangle();
                    graphics.SetClippingRectangle(strips[i]);

                    //-					System.Diagnostics.Debug.WriteLine (string.Format ("Strip {0} : {1}", i, strips[i].ToString ()));

                    this.Root.PaintHandler(graphics, strips[i], this.paintFilter);
                }

                //-				System.Diagnostics.Debug.WriteLine ("Done");

                graphics.Transform = Drawing.Transform.Identity;
                graphics.ResetClippingRectangle();
                graphics.SetClippingRectangle(repaint);
            }
            else
            {
                graphics.Transform = Drawing.Transform.Identity;
                graphics.ResetClippingRectangle();
                graphics.SetClippingRectangle(repaint);

                this.Root.PaintHandler(graphics, repaint, this.paintFilter);
            }

            while (this.postPaintQueue.Count > 0)
            {
                PostPaintRecord record = this.postPaintQueue.Dequeue();
                record.Paint(graphics);
            }
        }

        private Widget DetectWidget(Drawing.Point pos)
        {
            return this.root.FindChild(
                    pos,
                    WidgetChildFindMode.SkipHidden | WidgetChildFindMode.Deep
                ) ?? this.root;
        }

        public void MarkForRepaint()
        {
            if (this.window != null)
            {
                this.window.MarkForRepaint();
            }
        }

        public void MarkForRepaint(Drawing.Rectangle rect)
        {
            if (this.window != null)
            {
                this.window.MarkForRepaint(rect);
            }
        }

        public void QueuePostPaintHandler(
            Window.IPostPaintHandler handler,
            Drawing.Graphics graphics,
            Drawing.Rectangle repaint
        )
        {
            this.postPaintQueue.Enqueue(new Window.PostPaintRecord(handler, graphics, repaint));
        }

        public Drawing.Point WindowPointToScreenPoint(Drawing.Point point)
        {
            return this.window.WindowPointToScreenPoint(point);
        }

        public Drawing.Point ScreenPointToWindowPoint(Drawing.Point point)
        {
            return this.window.ScreenPointToWindowPoint(point);
        }

        protected void HandleTimeElapsed(object sender)
        {
            if (this.engagedWidget != null)
            {
                this.timer.AutoRepeat = this.engagedWidget.AutoEngageRepeatPeriod;

                if (this.engagedWidget.IsEngaged)
                {
                    this.engagedWidget.FireStillEngaged();
                    this.GenerateDummyMouseMoveEvent();
                    return;
                }
            }

            this.timer.Stop();
        }

        private void SyncMinSizeWithWindowRoot()
        {
            if ((this.window != null))
            {
                int width = (int)(this.root.RealMinSize.Width + 0.5);
                int height = (int)(this.root.RealMinSize.Height + 0.5);

                this.window.MinimumSize = new Drawing.Size(width, height);
            }
        }

        public new void Dispose()
        {
            if (this.window == null)
            {
                return;
            }

            this.OnWindowDisposing();

            if (this.IsVisible)
            {
                this.OnAboutToHideWindow();
            }

            base.Dispose();

            foreach (Window child in this.OwnedWindows)
            {
                child.Dispose();
            }
            this.ownedWindows = null;

            this.Owner?.RemoveOwnedWindow(this);

            if (this.cmdQueue.Count > 0)
            {
                //	Il y a encore des commandes dans la queue d'exécution. Il faut soit les transmettre
                //	à une autre fenêtre encore en vie, soit les exécuter tout de suite.

                Window helper = this.Owner ?? Window.FindFirstLiveWindow();
                if (helper == null)
                {
                    this.DispatchQueuedCommands();
                }
                else
                {
                    while (this.cmdQueue.Count > 0)
                    {
                        QueueItem item = this.cmdQueue.Dequeue();
                        helper.QueueCommand(item);
                    }
                }
            }

            CommandDispatcher.ClearDispatcher(this);

            if (this.root != null)
            {
                this.root.Dispose();
                this.root = null;
            }

            PlatformWindow oldWindow = this.window;
            // Since the platform window also has a reference to us, we need to
            // make sure we don't end up in an infinite Dispose() loop.
            // We first set our window attribute to null before calling Dispose
            // on the platform window.
            this.window = null;
            oldWindow.Dispose();

            this.timer.TimeElapsed -= this.HandleTimeElapsed;
            this.timer.Dispose();
            this.timer = null;

            this.owner = null;

            this.lastInWidget = null;
            this.capturingWidget = null;
            this.capturingButton = MouseButtons.None;
            this.focusedWidget = null;
            this.engagedWidget = null;

            if (this.components.Count > 0)
            {
                Support.Data.IComponent[] components = new Support.Data.IComponent[
                    this.components.Count
                ];
                this.components.CopyTo(components, 0);

                //	S'il y a des composants attachés, on les détruit aussi. Si l'utilisateur
                //	ne désire pas que ses composants soient détruits, il doit les détacher
                //	avant de faire le Dispose de la fenêtre !

                for (int i = 0; i < components.Length; i++)
                {
                    Support.Data.IComponent component = components[i];
                    this.components.Remove(component);
                    component.Dispose();
                }
            }

            this.components.Dispose();
            this.components = null;

            if (Message.CurrentState.LastWindow == this)
            {
                Message.ClearLastWindow();
            }
        }

        public bool Equals(Window otherWindow)
        {
            return otherWindow.id == this.id;
        }

        public override int GetHashCode()
        {
            return (int)this.id;
        }

        #region PostPaint related definitions
        public interface IPostPaintHandler
        {
            void Paint(Drawing.Graphics graphics, Drawing.Rectangle repaint);
        }

        private struct PostPaintRecord
        {
            public PostPaintRecord(
                Window.IPostPaintHandler handler,
                Drawing.Graphics graphics,
                Drawing.Rectangle repaint
            )
            {
                this.handler = handler;
                this.repaint = repaint;
                this.clipping = graphics.SaveClippingRectangle();
                this.transform = graphics.Transform;
            }

            public void Paint(Drawing.Graphics graphics)
            {
                graphics.RestoreClippingRectangle(this.clipping);
                graphics.Transform = this.transform;

                handler.Paint(graphics, this.repaint);
            }

            Window.IPostPaintHandler handler;
            Drawing.Rectangle repaint;
            Drawing.Rectangle clipping;
            Drawing.Transform transform;
        }
        #endregion

        public event EventHandler AsyncNotification;

        public event EventHandler WindowActivated;
        public event EventHandler WindowDeactivated;
        public event EventHandler WindowShown;
        public event EventHandler WindowHidden;
        public event EventHandler WindowClosed;
        public event EventHandler WindowCloseClicked;
        public event EventHandler WindowAnimationEnded;
        public event EventHandler WindowFocused;
        public event EventHandler WindowDefocused;
        public event EventHandler WindowDisposed;
        public event EventHandler WindowDisposing;
        public event EventHandler WindowSizeMoveStatusChanged;
        public event EventHandler WindowResizeBeginning;
        public event EventHandler WindowResizeEnded;
        public event EventHandler WindowPlacementChanged;

        public event EventHandler<DependencyPropertyChangedEventArgs> FocusedWidgetChanged;
        public event EventHandler<FocusChangingEventArgs> FocusedWidgetChanging;

        public event EventHandler AboutToShowWindow;
        public event EventHandler AboutToHideWindow;

        public event EventHandler<WindowDragEventArgs> WindowDragEntered;
        public event EventHandler WindowDragExited;
        public event EventHandler<WindowDragEventArgs> WindowDragDropped;

        public static event MessageHandler MessageFilter;
        public static event EventHandler ApplicationActivated;
        public static event EventHandler ApplicationDeactivated;
        public static event EventHandler<DependencyPropertyChangedEventArgs> GlobalFocusedWidgetChanged;
        public static event EventHandler<DependencyPropertyChangedEventArgs> GlobalFocusedWindowChanged;
        public static event EventHandler GlobalWindowShown;
        public static event EventHandler GlobalWindowHidden;

        public enum InvalidateReason
        {
            Generic,
            AdornerChanged,
            CultureChanged
        }

        private static readonly string DispatchCommandIdOption = "-dispatch-command-id:";

        private static long nextWindowId;
        private static Window focusedWindow;

        private string name;
        private string text;

        private readonly long id;
        private readonly System.Threading.Thread thread;

        private PlatformWindow window;
        private Window owner;
        private HashSet<Window> ownedWindows;
        private WindowRoot root;
        private bool windowIsFocused;
        private bool windowLocationSet;

        private int showCount;
        private int recursiveLayoutCount;
        private Widget lastInWidget;
        private Widget capturingWidget;
        private MouseButtons capturingButton;
        private Drawing.Point capturingCursor;
        private Widget focusedWidget;
        private Widget engagedWidget;
        private Widget initiallyEngagedWidget;
        private Widget modalWidget;
        private Timer timer;
        private MouseCursor windowCursor;

        private readonly List<Widget> logicalFocusStack = new List<Widget>();
        private readonly Queue<QueueItem> cmdQueue = new Queue<QueueItem>();
        private bool isDisposeQueued;
        private bool isAsyncNotificationQueued;
        private bool isAsyncLayoutQueued;
        private bool isDisposed;
        private static int asyncSuspendCount;
        private static List<Window> pendingAsyncWindows = new List<Window>();

        private bool pendingValidation;
        private List<Widget> asyncValidationList = new List<Widget>();

        private IPaintFilter paintFilter;

        private Queue<PostPaintRecord> postPaintQueue = new Queue<PostPaintRecord>();

        private Support.Data.ComponentCollection components;

        static WeakList<Window> windows = new WeakList<Window>();
        static bool isRunningInAutomatedTestEnvironment;
    }
}
