//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Platform
{
	/// <summary>
	/// La classe Platform.Window fait le lien avec les WinForms.
	/// </summary>
	internal class Window : System.Windows.Forms.Form, Types.BindingAsyncOperation.IApplicationThreadInvoker
	{
		static Window()
		{
			RestartManager.Setup ();
			
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += Window.HandleSystemEventsUserPreferenceChanged;
			
			Window.dispatchWindow = new Window ();
			Window.dispatchWindow.CreateControl ();
			Window.dispatchWindowHandle = Window.dispatchWindow.Handle;
			
			Epsitec.Common.Drawing.Platform.Dispatcher.Initialize ();
			
			//	The asynchronous binding mechanisms need to be able to execute
			//	code on the main application thread. Thus, we have to register
			//	the special thread invoker interface :
			
			Types.BindingAsyncOperation.DefineApplicationThreadInvoker (Window.dispatchWindow);
		}


		public static void Initialize()
		{
			//	This invokes the static constructor...
		}
		
		private static void HandleSystemEventsUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
		{
			//	TODO: notifier d'autres classes du changement des préférences
			
			switch (e.Category)
			{
				case Microsoft.Win32.UserPreferenceCategory.Locale:
					System.Threading.Thread.CurrentThread.CurrentCulture.ClearCachedData ();
					System.Threading.Thread.CurrentThread.CurrentUICulture.ClearCachedData ();
					break;
				
				default:
					break;
			}
		}
		
		
		private Window()
		{
			this.isSyncPaintDisabled         = new SafeCounter ();
			this.isSyncUpdating              = new SafeCounter ();
			this.isWndProcHandlingRestricted = new SafeCounter ();
		}
		
		
		internal Window(Epsitec.Common.Widgets.Window window, System.Action<Window> platformWindowSetter)
			: this ()
		{
			this.widgetWindow = window;
			platformWindowSetter (this);
			
			this.dirtyRectangle = Drawing.Rectangle.Empty;
			this.dirtyRegion    = new Drawing.DirtyRegion ();

			base.MinimumSize = new System.Drawing.Size (1, 1);

			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);

			this.widgetWindow.WindowType   = WindowType.Document;
			this.widgetWindow.WindowStyles = WindowStyles.CanResize | WindowStyles.HasCloseButton;
			
			this.graphics = new Epsitec.Common.Drawing.Graphics ();
			this.graphics.AllocatePixmap ();
			
			Window.DummyHandleEater (this.Handle);
			
			//	Fait en sorte que les changements de dimensions en [x] et en [y] provoquent un
			//	redessin complet de la fenêtre, sinon Windows tente de recopier l'ancien contenu
			//	en le décalant, ce qui donne des effets bizarres :
			
			int classWindowStyle = Win32Api.GetClassLong (this.Handle, Win32Const.GCL_STYLE);
			
			classWindowStyle |= Win32Const.CS_HREDRAW;
			classWindowStyle |= Win32Const.CS_VREDRAW;
			
			Win32Api.SetClassLong (this.Handle, Win32Const.GCL_STYLE, classWindowStyle);
			
			this.ReallocatePixmap ();
			
			WindowList.Insert (this);
		}


		internal static bool IsInAnyWndProc
		{
			get
			{
				if (Window.globalWndProcDepth > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		internal void MakeTopLevelWindow()
		{
			this.TopLevel = true;
			this.TopMost  = true;
		}
		
		internal void MakeFramelessWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ShowInTaskbar   = false;
			Window.DummyHandleEater (this.Handle);
			this.widgetWindow.WindowStyles = this.WindowStyles | (WindowStyles.Frameless);
		}
		
		internal void MakeFixedSizeWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox     = false;
			this.MinimizeBox     = false;
			Window.DummyHandleEater (this.Handle);
			
			this.widgetWindow.WindowStyles = this.WindowStyles & ~(WindowStyles.CanMaximize | WindowStyles.CanMinimize | WindowStyles.CanResize);
		}

		internal void MakeMinimizableFixedSizeWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox     = false;
			this.MinimizeBox     = true;
			Window.DummyHandleEater (this.Handle);
			this.widgetWindow.WindowStyles = (this.WindowStyles & ~(WindowStyles.CanMaximize | WindowStyles.CanResize)) | WindowStyles.CanMinimize;
		}

		internal void MakeButtonlessWindow()
		{
			this.ControlBox      = false;
			this.MaximizeBox     = false;
			this.MinimizeBox     = false;
			Window.DummyHandleEater (this.Handle);
			this.widgetWindow.WindowStyles = this.WindowStyles & ~(WindowStyles.CanMaximize | WindowStyles.CanMinimize | WindowStyles.HasCloseButton);
		}

		internal bool IsFixedSize
		{
			get
			{
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
				
				throw new System.InvalidOperationException (string.Format ("{0} not supported", this.FormBorderStyle));
			}
		}
		
		internal void MakeSecondaryWindow()
		{
			this.ShowInTaskbar   = false;
			Window.DummyHandleEater (this.Handle);
		}

		internal void MakeToolWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.ShowInTaskbar   = false;
			this.isToolWindow  = true;
			Window.DummyHandleEater (this.Handle);
		}

		internal void MakeSizableToolWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.ShowInTaskbar   = false;
			this.isToolWindow  = true;
			Window.DummyHandleEater (this.Handle);
		}

		internal void MakeFloatingWindow()
		{
			this.ShowInTaskbar   = false;
			this.isToolWindow  = true;
			Window.DummyHandleEater (this.Handle);
		}
		
		internal void ResetHostingWidgetWindow()
		{
			this.widgetWindowDisposed = true;
		}


		internal void HideWindow()
		{
			using (this.isWndProcHandlingRestricted.Enter ())
			{
				this.Hide ();
			}
		}

		
		static void DummyHandleEater(System.IntPtr handle)
		{
		}
		
		
		internal void AnimateShow(Animation animation, Drawing.Rectangle bounds)
		{
			Window.DummyHandleEater (this.Handle);

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
			this.MarkForRepaint ();
			this.RefreshGraphics ();
			
			Animator animator;
			
			switch (animation)
			{
				default:
				case Animation.None:
					this.ShowWindow ();
					this.isAnimatingActiveWindow = false;
					return;
				
				case Animation.RollDown:
					b1 = new Drawing.Rectangle (bounds.Left, bounds.Top - 1, bounds.Width, 1);
					b2 = bounds;
					o1 = new Drawing.Point (0, 1 - bounds.Height);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.RollUp:
					b1 = new Drawing.Rectangle (bounds.Left, bounds.Bottom, bounds.Width, 1);
					b2 = bounds;
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.RollRight:
					b1 = new Drawing.Rectangle (bounds.Left, bounds.Bottom, 1, bounds.Height);
					b2 = bounds;
					o1 = new Drawing.Point (1 - bounds.Width, 0);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.RollLeft:
					b1 = new Drawing.Rectangle (bounds.Right - 1, bounds.Bottom, 1, bounds.Height);
					b2 = bounds;
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.FadeIn:
					this.isFrozen = true;
					this.IsLayered = true;
					this.Alpha = 0.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeInTime);
					animator.SetCallback<double> (this.AnimateAlpha, this.AnimateCleanup);
					animator.SetValue (0.0, startAlpha);
					animator.Start ();
					this.ShowWindow ();
					return;
				
				case Animation.FadeOut:
					this.isFrozen = true;
					this.IsLayered = true;
//					this.Alpha = 1.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeOutTime);
					animator.SetCallback<double> (this.AnimateAlpha, this.AnimateCleanup);
					animator.SetValue (startAlpha, 0.0);
					animator.Start ();
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
					this.MinimumSize = new System.Drawing.Size (1, 1);
					this.WindowBounds = b1;
					this.UpdateLayeredWindow ();
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback<Drawing.Rectangle, Drawing.Point> (this.AnimateWindowBounds, this.AnimateCleanup);
					animator.SetValue (0, b1, b2);
					animator.SetValue (1, o1, o2);
					animator.Start ();
					this.ShowWindow ();
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
			this.MarkForRepaint ();
			this.RefreshGraphics ();
			
			Animator animator;
			
			switch (animation)
			{
				case Animation.None:
					this.HideWindow ();
					return;
				
				case Animation.RollDown:
					b1 = bounds;
					b2 = new Drawing.Rectangle (bounds.Left, bounds.Top - 1, bounds.Width, 1);
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (0, 1 - bounds.Height);
					break;
				
				case Animation.RollUp:
					b1 = bounds;
					b2 = new Drawing.Rectangle (bounds.Left, bounds.Bottom, bounds.Width, 1);
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.RollRight:
					b1 = bounds;
					b2 = new Drawing.Rectangle (bounds.Left, bounds.Bottom, 1, bounds.Height);
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (1 - bounds.Width, 0);
					break;
				
				case Animation.RollLeft:
					b1 = bounds;
					b2 = new Drawing.Rectangle (bounds.Right - 1, bounds.Bottom, 1, bounds.Height);
					o1 = new Drawing.Point (0, 0);
					o2 = new Drawing.Point (0, 0);
					break;
				
				case Animation.FadeIn:
				case Animation.FadeOut:
					this.AnimateShow (animation, bounds);
					return;
				
				default:
					this.HideWindow ();
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
					this.UpdateLayeredWindow ();
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback<Drawing.Rectangle, Drawing.Point> (this.AnimateWindowBounds, this.AnimateCleanup);
					animator.SetValue (0, b1, b2);
					animator.SetValue (1, o1, o2);
					animator.Start ();
					break;
			}
		}
		
		protected void AnimateWindowBounds(Drawing.Rectangle bounds, Drawing.Point offset)
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			this.WindowBounds = bounds;
			this.paintOffset = offset;
			this.Invalidate ();
			this.Update ();
		}
		
		protected void AnimateAlpha(double alpha)
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			this.Alpha = alpha;
		}
		
		protected void AnimateCleanup(Animator animator)
		{
			animator.Dispose ();
			
			if (this.IsDisposed)
			{
				return;
			}
			
			this.MinimumSize = this.formMinSize;
			this.isFrozen = false;
			this.isAnimatingActiveWindow = false;
			this.Invalidate ();

			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowAnimationEnded ();
			}
		}
		
		
		
		internal WindowStyles					WindowStyles
		{
			get
			{
				return this.windowStyles;
			}
			set
			{
				if (this.windowStyles != value)
				{
					this.windowStyles = value;
					this.UpdateWindowTypeAndStyles ();
				}
			}
		}
		
		internal WindowType						WindowType
		{
			get
			{
				return this.windowType;
			}
			set
			{
				if (this.windowType != value)
				{
					this.windowType = value;
					this.UpdateWindowTypeAndStyles ();
				}
			}
		}
		
		
		private void UpdateWindowTypeAndStyles()
		{
			var windowStyles = this.WindowStyles;

			switch (this.windowType)
			{
				case WindowType.Document:
					if ((windowStyles & WindowStyles.Frameless) != 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					}
					else if ((windowStyles & WindowStyles.CanResize) == 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
					}
					else
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
					}
					this.ShowInTaskbar = true;
					break;
				
				case WindowType.Dialog:
					if ((windowStyles & WindowStyles.Frameless) != 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					}
					else if ((windowStyles & WindowStyles.CanResize) == 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
					}
					else
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
					}
					this.ShowInTaskbar = false;
					break;
				
				case WindowType.Palette:
					if ((windowStyles & WindowStyles.Frameless) != 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
					}
					else if ((windowStyles & WindowStyles.CanResize) == 0)
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
					}
					else
					{
						this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
					}
					this.ShowInTaskbar = false;
					break;
			}

			this.MinimizeBox = ((windowStyles & WindowStyles.CanMinimize)    != 0);
			this.MaximizeBox = ((windowStyles & WindowStyles.CanMaximize)    != 0);
			this.HelpButton  = ((windowStyles & WindowStyles.HasHelpButton)  != 0);
			this.ControlBox  = ((windowStyles & WindowStyles.HasCloseButton) != 0);
		}
		
		
		internal bool							PreventSyncPaint
		{
			get
			{
				return this.isSyncPaintDisabled.IsNotZero;
			}
		}
		
		internal bool							PreventAutoClose
		{
			get
			{
				return this.preventClose;
			}
			set
			{
				this.preventClose = value;
			}
		}
		
		internal bool							PreventAutoQuit
		{
			get
			{
				return this.preventQuit;
			}
			set
			{
				this.preventQuit = value;
			}
		}
		
		internal bool							IsLayered
		{
			get
			{
				return this.isLayered;
			}
			set
			{
				if (this.isLayered != value)
				{
					if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
					{
						throw new System.Exception ("A layered window may not have a border");
					}
					
					if (SystemInformation.SupportsLayeredWindows)
					{
						int exStyle = Win32Api.GetWindowExStyle (this.Handle);
						
						if (value)
						{
							exStyle |= Win32Const.WS_EX_LAYERED;
						}
						else
						{
							exStyle &= ~ Win32Const.WS_EX_LAYERED;
						}
						
						Win32Api.SetWindowExStyle (this.Handle, exStyle);
						this.isLayered = value;
					}
				}
			}
		}
		
		internal bool							IsActive
		{
			get
			{
				if (this.Handle == Win32Api.GetActiveWindow ())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		internal bool							IsFrozen
		{
			get
			{
				return (this.isFrozen)
					|| (this.widgetWindow == null)
					|| (this.widgetWindow.Root == null)
					|| (this.widgetWindow.Root.IsFrozen);
			}
		}
		
		internal bool							IsAnimatingActiveWindow
		{
			get
			{
				return this.isAnimatingActiveWindow;
			}
		}
		
		internal bool							IsMouseActivationEnabled
		{
			get
			{
				return !this.isNoActivate;
			}
			
			set
			{
				this.isNoActivate = !value;
			}
		}
		
		internal bool							IsToolWindow
		{
			get
			{
				return this.isToolWindow;
			}
		}
		
		internal bool							IsSizeMoveInProgress
		{
			get
			{
				return this.isSizeMoveInProgress;
			}
			set
			{
				if (this.isSizeMoveInProgress != value)
				{
					this.isSizeMoveInProgress = value;
					if (this.widgetWindow != null)
					{
						this.widgetWindow.OnWindowSizeMoveStatusChanged ();
					}
				}
			}
		}
		
		
		internal WindowMode						WindowMode
		{
			get
			{
				return this.windowMode;
			}
			set
			{
				this.windowMode = value;
			}
		}
		
		internal Drawing.Rectangle				WindowBounds
		{
			get
			{
				System.Drawing.Rectangle rect;
				
				if (this.formBoundsSet)
				{
					rect = this.formBounds;
				}
				else
				{
					rect = base.Bounds;
				}
				
				double ox = this.MapFromWinFormsX (rect.Left);
				double oy = this.MapFromWinFormsY (rect.Bottom);
				double dx = this.MapFromWinFormsWidth (rect.Width);
				double dy = this.MapFromWinFormsHeight (rect.Height);

				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
			set
			{
				if (this.windowBounds != value)
				{
					int ox = this.MapToWinFormsX (value.Left);
					int oy = this.MapToWinFormsY (value.Top);
					int dx = this.MapToWinFormsWidth (value.Width);
					int dy = this.MapToWinFormsHeight (value.Height);
					
					this.windowBounds   = value;
					this.formBounds     = new System.Drawing.Rectangle (ox, oy, dx, dy);
					this.formBoundsSet = true;
					this.onResizeEvent = true;
					
					if (this.isLayered)
					{
						//	Be very careful here: in order to avoid any jitter while
						//	moving & sizing the layered window, we must resize the
						//	suface pixmap first, update the layered window surface
						//	and only then move the window itself :
						
						this.ReallocatePixmapLowLevel ();
						this.UpdateLayeredWindow ();
						
						Win32Api.SetWindowPos (this.Handle, System.IntPtr.Zero, ox, oy, dx, dy, Win32Const.SWP_NOACTIVATE | Win32Const.SWP_NOCOPYBITS | Win32Const.SWP_NOOWNERZORDER | Win32Const.SWP_NOZORDER | Win32Const.SWP_NOREDRAW);
					}
					else
					{
						Win32Api.SetWindowPos (this.Handle, System.IntPtr.Zero, ox, oy, dx, dy, Win32Const.SWP_NOACTIVATE | Win32Const.SWP_NOCOPYBITS | Win32Const.SWP_NOOWNERZORDER | Win32Const.SWP_NOZORDER);
					}
					
					this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				}
			}
		}
		
		public new System.Drawing.Size			ClientSize
		{
			get
			{
				System.Drawing.Size clientSize = base.ClientSize;
				
				if (this.formBoundsSet)
				{
					int deltaWidth  = this.formBounds.Width - this.Width;
					int deltaHeight = this.formBounds.Height - this.Height;
					
					clientSize.Width  += deltaWidth;
					clientSize.Height += deltaHeight;
				}

				return clientSize;
			}
		}
		
		public new System.Drawing.Size			MinimumSize
		{
			get
			{
				return this.minimumSize;
			}
			set
			{
				this.minimumSize = value;
			}
		}
		
		internal Drawing.Rectangle				WindowPlacementNormalBounds
		{
			get
			{
				Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement ()
				{
					Length = 4+4+4+2*4+2*4+4*4
				};

				Win32Api.GetWindowPlacement (this.Handle, ref placement);

				double ox = this.MapFromWinFormsX (placement.NormalPosition.Left);
				double oy = this.MapFromWinFormsY (placement.NormalPosition.Bottom);
				double dx = this.MapFromWinFormsWidth (placement.NormalPosition.Right - placement.NormalPosition.Left);
				double dy = this.MapFromWinFormsHeight (placement.NormalPosition.Bottom - placement.NormalPosition.Top);

				//	Attention: les coordonnées retournées par WindowPlacement sont exprimées
				//	en "workspace coordinates" (elles tiennent compte de la présence d'une
				//	barre "Desktop").

				//	Cf. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowstructures/windowplacement.asp

				Window.AdjustWindowPlacementOrigin (placement, ref ox, ref oy);

				return new Drawing.Rectangle (ox, oy, dx, dy);
			}
		}

		private static void AdjustWindowPlacementOrigin(Win32Api.WindowPlacement placement, ref double ox, ref double oy)
		{
			//	La conversion entre "screen coordinates" et "workspace coordinates" est
			//	théoriquement impossible avec les informations que fournit Windows mais
			//	on peut s'arranger en créant une fenêtre temporaire pour déterminer son
			//	offset par rapport à l'endroit désiré :

			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint (new System.Drawing.Point (placement.NormalPosition.Left, placement.NormalPosition.Top));

			//				System.Diagnostics.Trace.WriteLine ("WorkingArea:  " + screen.WorkingArea);
			//				System.Diagnostics.Trace.WriteLine ("ScreenBounds: " + screen.Bounds);

			if (screen.WorkingArea != screen.Bounds)
			{
				using (System.Windows.Forms.Form f = new System.Windows.Forms.Form ())
				{
					f.Hide ();
					f.Location = new System.Drawing.Point (placement.NormalPosition.Left, placement.NormalPosition.Bottom);

					Win32Api.GetWindowPlacement (f.Handle, ref placement);

					int placementOffsetX = placement.NormalPosition.Left - f.Location.X;
					int placementOffsetY = placement.NormalPosition.Top - f.Location.Y;

					ox -= placementOffsetX;
					oy += placementOffsetY;
				}
			}
		}

		private WindowPlacement CurrentWindowPlacement
		{
			set
			{
				if (!this.windowPlacement.Equals (value))
				{
					this.windowPlacement = value;
					
					if (this.widgetWindow != null)
					{
						this.widgetWindow.OnWindowPlacementChanged ();
					}
				}
			}
		}

		internal WindowPlacement NativeWindowPlacement
		{
			get
			{
				Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement ()
				{
					Length = 4+4+4+2*4+2*4+4*4
				};

				Win32Api.GetWindowPlacement (this.Handle, ref placement);

				bool isMaximized = (placement.Flags & Win32Const.WPF_RESTORETOMAXIMIZED) != 0;
				bool isMinimized = (placement.ShowCmd == Win32Const.SW_SHOWMINIMIZED);
				bool isHidden    = (placement.ShowCmd == Win32Const.SW_HIDE);

				var bounds = new Drawing.Rectangle (placement.NormalPosition.Left, placement.NormalPosition.Top, placement.NormalPosition.Right - placement.NormalPosition.Left, placement.NormalPosition.Bottom - placement.NormalPosition.Top);

				return new WindowPlacement (bounds, isMaximized, isMinimized, isHidden);
			}
			set
			{
				int show  = Win32Const.SW_SHOWNORMAL;
				int flags = 0;

				if (value.IsFullScreen)
				{
					show   = Win32Const.SW_SHOWMAXIMIZED;
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


				Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement ()
				{
					Length = 4+4+4+2*4+2*4+4*4,
					NormalPosition = new Win32Api.Rect ()
					{
						Left = (int) value.Bounds.Left,
						Right = (int) value.Bounds.Right,
						Top = (int) value.Bounds.Bottom,
						Bottom = (int) value.Bounds.Top
					},
					ShowCmd = show,
					Flags = flags
				};

				try
				{
					this.EnterWndProc ();
					Win32Api.SetWindowPlacement (this.Handle, ref placement);
				}
				finally
				{
					this.ExitWndProc ();
				}
			}
		}

		
		internal Drawing.Point					WindowLocation
		{
			get
			{
				return this.WindowBounds.Location;
			}
			set
			{
				Drawing.Rectangle bounds = this.WindowBounds;
				
				if (bounds.Location != value)
				{
					bounds.Offset (value.X - bounds.X, value.Y - bounds.Y);
					this.WindowBounds = bounds;
				}
			}
		}
		
		internal Drawing.Size					WindowSize
		{
			get
			{
				return this.WindowBounds.Size;
			}
			set
			{
				Drawing.Rectangle bounds = this.WindowBounds;
				
				if (bounds.Size != value)
				{
					bounds.Size = value;
					this.WindowBounds = bounds;
				}
			}
		}
		
		internal new string						Text
		{
			get { return TextLayout.ConvertToTaggedText (base.Text); }
			set { base.Text = TextLayout.ConvertToSimpleText (value); }
		}
		
		internal new string						Name
		{
			get { return base.Name; }
			set { base.Name = value; }
		}
		
		internal new Drawing.Image				Icon
		{
			get
			{
				if (base.Icon == null)
				{
					return null;
				}
				
				return Drawing.Bitmap.FromNativeBitmap (base.Icon.ToBitmap ());
			}
			set
			{
				if (value == null)
				{
					base.Icon = null;
				}
				else
				{
					base.Icon = System.Drawing.Icon.FromHandle (value.BitmapImage.NativeBitmap.GetHicon ());
				}
			}
		}

		internal void SetNativeIcon(System.IO.Stream iconStream)
		{
			System.Drawing.Icon nativeIcon = new System.Drawing.Icon (iconStream);
			base.Icon = nativeIcon;
		}

		internal void SetNativeIcon(System.IO.Stream iconStream, int dx, int dy)
		{
			byte[] buffer = new	 byte[iconStream.Length];
			iconStream.Read (buffer, 0, buffer.Length);
			string path = System.IO.Path.GetTempFileName ();
			
			try
			{
				System.IO.File.WriteAllBytes (path, buffer);
				System.Drawing.Icon nativeIcon = Epsitec.Common.Drawing.Bitmap.LoadNativeIcon (path, dx, dy);

#if false
				//	This does not work (see http://stackoverflow.com/questions/2266479/setclasslonghwnd-gcl-hicon-hicon-cannot-replace-winforms-form-icon)
				if ((dx == 16) && (dy == 16))
				{
					Win32Api.SetClassLong (this.Handle, Win32Const.GCL_HICONSM, nativeIcon.Handle.ToInt32 ());
				}
				else if ((dx == 32) && (dy == 32))
				{
					Win32Api.SetClassLong (this.Handle, Win32Const.GCL_HICON, nativeIcon.Handle.ToInt32 ());
				}
				else
#endif
				{
					base.Icon = nativeIcon;
				}
			}
			finally
			{
				System.IO.File.Delete (path);
			}
		}

		internal double Alpha
		{
			get { return this.alpha; }
			set
			{
				if (this.alpha != value)
				{
					this.alpha = value;
					this.UpdateLayeredWindow ();
				}
			}
		}
		
		internal System.Drawing.Size			BorderSize
		{
			get
			{
				System.Drawing.Size borderSize = new System.Drawing.Size (0, 0);
				
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
						borderSize = new System.Drawing.Size (1, 1);
						break;
					
					case System.Windows.Forms.FormBorderStyle.None:
						break;
				}
				
				return borderSize;
			}
		}
		
		
		internal bool							FilterMouseMessages
		{
			get { return this.filterMouseMessages; }
			set { this.filterMouseMessages = value; }
		}
		
		internal bool							FilterKeyMessages
		{
			get { return this.filterKeyMessages; }
			set { this.filterKeyMessages = value; }
		}
		
		
		internal Epsitec.Common.Widgets.Window	HostingWidgetWindow
		{
			get { return this.widgetWindow; }
		}
		
		internal static bool					IsApplicationActive
		{
			get { return Window.isAppActive; }
		}

		internal static new bool				UseWaitCursor
		{
			get
			{
				return System.Windows.Forms.Application.UseWaitCursor;
			}
			set
			{
				System.Windows.Forms.Application.UseWaitCursor = value;
			}
		}
		
		
		internal new void Close()
		{
			try
			{
				this.forcedClose = true;
				base.Close ();
			}
			finally
			{
				this.forcedClose = false;
			}
		}

		internal void SetFrozen(bool frozen)
		{
			this.isFrozen = frozen;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				WindowList.Remove (this);
				
				//	Attention: il n'est pas permis de faire un Dispose si l'appelant provient d'une
				//	WndProc, car cela perturbe le bon acheminement des messages dans Windows. On
				//	préfère donc remettre la destruction à plus tard si on détecte cette condition.
				
				if (this.wndProcDepth > 0)
				{
					Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_DISPOSE, System.IntPtr.Zero, System.IntPtr.Zero);
					return;
				}
				
				if (this.graphics != null)
				{
					this.graphics.Dispose ();
				}
				
				this.graphics = null;
				
				if (this.widgetWindow != null)
				{
					if (this.widgetWindowDisposed == false)
					{
						this.widgetWindow.PlatformWindowDisposing ();
						this.widgetWindowDisposed = true;
					}
				}
			}
			
			base.Dispose (disposing);
		}
		
		
		protected override void OnClosed(System.EventArgs e)
		{
			if (this.Focused)
			{
				//	Si la fenêtre avait le focus et qu'on la ferme, on aimerait bien que
				//	si elle avait une fenêtre "parent", alors ce soit le parent qui reçoive
				//	le focus à son tour. Ca paraît logique.
				
				System.Windows.Forms.Form form = this.Owner;
				
				if (form != null)
				{
					form.Activate ();
				}
			}

			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowClosed ();
			}
			
			base.OnClosed (e);
		}
		
		protected override void OnGotFocus(System.EventArgs e)
		{
			base.OnGotFocus (e);
			if (this.widgetWindow != null)
			{
				this.widgetWindow.NotifyWindowFocused ();
			}
		}
		
		protected override void OnLostFocus(System.EventArgs e)
		{
			base.OnLostFocus (e);
			if (this.widgetWindow != null)
			{
				this.widgetWindow.NotifyWindowDefocused ();
			}
		}


		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			
			if (this.forcedClose)
			{
				return;
			}

			if (this.widgetWindow == null)
			{
				return;
			}

			this.widgetWindow.OnWindowCloseClicked ();
			
			if (this.preventClose)
			{
				e.Cancel = true;
				
				if (this.preventQuit)
				{
					return;
				}

				CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (this.widgetWindow);

				//	Don't generate an Alt-F4 event if there is no dispatcher attached with this
				//	window, as it would probably bubble up to the owner window and cause the
				//	application to quit...
				
				if (dispatcher == null)
				{
					return;
				}

				//	Empêche la fermeture de la fenêtre lorsque l'utilisateur clique sur le bouton de
				//	fermeture, et synthétise un événement clavier ALT + F4 à la place...
				
				System.Windows.Forms.Keys altF4 = System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt;
				System.Windows.Forms.KeyEventArgs fakeEvent = new System.Windows.Forms.KeyEventArgs (altF4);
				Message message = Message.FromKeyEvent (MessageType.KeyDown, fakeEvent);
				message.MarkAsDummyMessage ();
				this.DispatchMessage (message);
			}
		}
		
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown (e);
			
			Message message = Message.FromKeyEvent (MessageType.KeyDown, e);
			this.DispatchMessage (message);
			e.Handled = message.Handled;
		}
		
		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyUp (e);
			
			Message message = Message.FromKeyEvent (MessageType.KeyUp, e);
			this.DispatchMessage (message);
			e.Handled = message.Handled;
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress (e);
			
			Message message = Message.FromKeyEvent (MessageType.KeyPress, e);
			this.DispatchMessage (message);
			e.Handled = message.Handled;
		}

		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseWheel, this, e));
		}
		
		protected override void OnMouseEnter(System.EventArgs e)
		{
			base.OnMouseEnter (e);
			
			System.Drawing.Point point = this.PointToClient (System.Windows.Forms.Control.MousePosition);
			System.Windows.Forms.MouseEventArgs fakeEvent = new System.Windows.Forms.MouseEventArgs (System.Windows.Forms.MouseButtons.None, 0, point.X, point.Y, 0);
			
			Message message = Message.FromMouseEvent (MessageType.MouseEnter, this, fakeEvent);

			if (this.widgetWindow != null)
			{
				if (this.widgetWindow.FilterMessage (message) == false)
				{
					this.DispatchMessage (message);
				}
			}
		}

		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave (e);
			
			Message message = Message.FromMouseEvent (MessageType.MouseLeave, this, null);

			if (this.widgetWindow != null)
			{
				if (this.widgetWindow.FilterMessage (message) == false)
				{
					this.DispatchMessage (message);
				}
			}
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine ("OnPaint");
			base.OnPaint (e);
			this.DispatchPaint (e.Graphics, e.ClipRectangle);
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine ("OnPaintBackground called");
			base.OnPaintBackground (e);
			this.DispatchPaint (e.Graphics, e.ClipRectangle);
		}

		protected override void OnResize(System.EventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine ("OnResize");
			base.OnResize (e);
		}

		protected override void OnResizeBegin(System.EventArgs e)
		{
			base.OnResizeBegin (e);
			this.widgetWindow.OnWindowResizeBeginning ();
		}

		protected override void OnResizeEnd(System.EventArgs e)
		{
			base.OnResizeEnd (e);
			this.widgetWindow.OnWindowResizeEnded ();
		}

		protected override void OnSizeChanged(System.EventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine ("OnSizeChanged");
			using (this.isSyncPaintDisabled.Enter ())
			{
				if ((this.Created == false) &&
					(this.formBoundsSet) &&
					(this.formBounds.Size != this.Size))
				{
					this.Size = this.formBounds.Size;
				}
				else if ((this.formBoundsSet) &&
						 (this.formBounds.Size == this.Size) &&
						 (this.onResizeEvent == false))
				{
					//	Rien à faire, car la taille correspond à la dernière taille mémorisée.
				}
				else
				{
					this.formBoundsSet = true;
					this.onResizeEvent = false;
					this.formBounds     = this.Bounds;
					this.windowBounds   = this.WindowBounds;
					
					base.OnSizeChanged (e);
					this.ReallocatePixmap ();
				}
				
				this.formBoundsSet = false;
			}
		}
		
		protected override void OnActivated(System.EventArgs e)
		{
			base.OnActivated (e);
			
			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowActivated ();
			}
		}
		
		protected override void OnDeactivate(System.EventArgs e)
		{
			base.OnDeactivate (e);
			
			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowDeactivated ();
			}
		}

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged (e);
			
			if (this.Visible)
			{
				if (! this.isPixmapOk)
				{
					this.ReallocatePixmap ();
				}
				if (this.widgetWindow != null)
				{
					this.widgetWindow.OnWindowShown ();
				}
			}
			else
			{
				if (this.widgetWindow != null)
				{
					this.widgetWindow.OnWindowHidden ();
				}
			}
		}
		
		
		protected override void OnDragEnter(System.Windows.Forms.DragEventArgs drgevent)
		{
			base.OnDragEnter (drgevent);
			
			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowDragEntered (new WindowDragEventArgs (drgevent));
			}
		}
		
		protected override void OnDragLeave(System.EventArgs e)
		{
			base.OnDragLeave (e);
			
			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowDragExited ();
			}
		}
		
		protected override void OnDragDrop(System.Windows.Forms.DragEventArgs drgevent)
		{
			base.OnDragDrop (drgevent);
			
			if (this.widgetWindow != null)
			{
				this.widgetWindow.OnWindowDragDropped (new WindowDragEventArgs (drgevent));
			}
		}
		

		
		protected void ReallocatePixmap()
		{
			if (this.IsFrozen)
			{
				return;
			}

			if (this.ReallocatePixmapLowLevel ())
			{
				this.UpdateLayeredWindow ();
			}
		}
			
		private bool ReallocatePixmapLowLevel()
		{
			bool changed = false;
			
			int width  = this.ClientSize.Width;
			int height = this.ClientSize.Height;
			
			if (this.graphics.SetPixmapSize (width, height))
			{
//				System.Diagnostics.Debug.WriteLine ("ReallocatePixmapLowLevel" + (this.isFrozen ? " (frozen)" : "") + " Size: " + width.ToString () + "," + height.ToString());
				
				this.graphics.Pixmap.Clear ();

				if (this.widgetWindow != null)
				{
					this.widgetWindow.Root.NotifyWindowSizeChanged (width, height);
				}
				this.dirtyRectangle = new Drawing.Rectangle (0, 0, width, height);
				this.dirtyRegion    = new Drawing.DirtyRegion ();
				this.dirtyRegion.Add (this.dirtyRectangle);

				changed = true;
			}
			
			this.isPixmapOk = true;
			
			return changed;
		}

		
		internal int MapToWinFormsX(double x)
		{
			return (int) System.Math.Floor (x + 0.5);
		}
		
		internal int MapToWinFormsY(double y)
		{
			return (int) ScreenInfo.PrimaryHeight - (int) System.Math.Floor (y + 0.5);
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
			this.MarkForRepaint (new Drawing.Rectangle (0, 0, this.ClientSize.Width, this.ClientSize.Height));
		}
		
		internal void MarkForRepaint(Drawing.Rectangle rect)
		{
			rect.RoundInflate ();
			
			this.dirtyRectangle.MergeWith (rect);
			this.dirtyRegion.Add (rect);
			
			int top    = (int) (rect.Top);
			int bottom = (int) (rect.Bottom);
			
			int width  = (int) (rect.Width);
			int height = top - bottom + 1;
			int x      = (int) (rect.Left);
			int y      = this.ClientSize.Height - top;
			
			if (this.isLayered)
			{
				this.isLayeredDirty = true;
			}
			
			this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
		}
		
		internal void SynchronousRepaint()
		{
			if (this.isLayoutInProgress)
			{
				return;
			}

			this.isLayoutInProgress = true;

			try
			{
				if (this.widgetWindow != null)
				{
					this.widgetWindow.ForceLayout ();
				}
			}
			finally
			{
				this.isLayoutInProgress = false;
			}
			
			if (this.dirtyRectangle.IsValid)
			{
				using (this.isSyncUpdating.Enter ())
				{
					this.Update ();
				}
			}
		}
		
		internal void SendQueueCommand()
		{
			if (this.InvokeRequired)
			{
				this.Invoke (new SimpleCallback (this.SendQueueCommand));
			}
			else
			{
				Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_EXEC_CMD, System.IntPtr.Zero, System.IntPtr.Zero);
			}
		}
		
		internal void SendValidation()
		{
			Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_VALIDATION, System.IntPtr.Zero, System.IntPtr.Zero);
		}
		
		
		internal void StartSizeMove()
		{
			this.IsSizeMoveInProgress = true;
			this.isSyncPaintDisabled.Increment ();
		}
		
		internal void StopSizeMove()
		{
			this.IsSizeMoveInProgress = false;
			this.isSyncPaintDisabled.Decrement ();
		}
		
		
		internal static void SendSynchronizeCommandCache()
		{
			Window.isSyncRequested = true;
			
			try
			{
				Win32Api.PostMessage (Window.dispatchWindowHandle, Win32Const.WM_APP_SYNCMDCACHE, System.IntPtr.Zero, System.IntPtr.Zero);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Exception thrown in Platform.Window.SendSynchronizeCommandCache :");
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
		}

		internal static void SendAwakeEvent()
		{
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
					Win32Api.PostMessage (Window.dispatchWindowHandle, Win32Const.WM_APP_AWAKE, System.IntPtr.Zero, System.IntPtr.Zero);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine ("Exception thrown in Platform.Window.SendAwakeEvent:");
					System.Diagnostics.Debug.WriteLine (ex.Message);
				}
			}
		}

		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
		{
			//System.Diagnostics.Debug.WriteLine (msg.ToString ());

			if (this.isWndProcHandlingRestricted.IsNotZero)
			{
				base.WndProc (ref msg);
				return;
			}

			bool syncCommandCache = false;

			lock (Window.dispatchWindow)
			{
				if (this.isSyncUpdating.IsZero)
				{
					if (Window.isSyncRequested)
					{
						Window.isSyncRequested = false;
						syncCommandCache = true;
					}
				}
				
				if (Window.isAwakeRequested)
				{
					Window.isAwakeRequested = false;
				}
			}

			if (Window.IsInAnyWndProc == false)
			{
				Application.ExecuteAsyncCallbacks ();
			}

			if (RestartManager.HandleWndProc (ref msg))
			{
				return;
			}

			if (syncCommandCache)
			{
				CommandCache.Instance.Synchronize ();
			}
			
			if (Window.dispatchWindow == this)
			{
				base.WndProc (ref msg);
				return;
			}

			if (msg.Msg == Win32Const.WM_GETMINMAXINFO)
			{
				//	Pour des raisons mystérieuses, WinForms (?) se croit obligé de touiller les positions
				//	pour la fenêtre maximisée avec un résultat désastreux (si la barre des tâches est à
				//	gauche ou en haut). On force la position maximisée à ce qu'elle devrait toujours être.
				
				unsafe
				{
					Win32Api.MinMaxInfo* mmi = (Win32Api.MinMaxInfo*) msg.LParam.ToPointer ();
					mmi->MaxPosition.X = -4;
					mmi->MaxPosition.Y = -4;
				}
			}

			if (msg.Msg == Win32Const.WM_SIZING)
			{
				//	Since the size returned by WM_GETMINMAXINFO seems to be cached by Windows when the
				//	user drags a window border, we cannot use it. We must be able to update the Minimum
				//	size constraint dynamically.
				
				//	Handling WM_SIZING is the way to go: make sure the user does not reduce the window
				//	size below the specified size.
				
				unsafe
				{
					Win32Api.Rect* rect = (Win32Api.Rect*) msg.LParam.ToPointer ();
					int wParam = msg.WParam.ToInt32 ();

//					System.Diagnostics.Debug.WriteLine (string.Format ("dx={0} MinWidth={1} MinimumSize={2}", rect->Right - rect->Left, this.minimumSize.Width, base.MinimumSize.Width));

					int dx = System.Math.Max (this.minimumSize.Width, rect->Right - rect->Left);
					int dy = System.Math.Max (this.minimumSize.Height, rect->Bottom - rect->Top);

					dx = System.Math.Max (dx, base.MinimumSize.Width);
					dy = System.Math.Max (dy, base.MinimumSize.Height);
					
					switch (wParam)
					{
						case Win32Const.WMSZ_LEFT:
						case Win32Const.WMSZ_BOTTOMLEFT:
						case Win32Const.WMSZ_TOPLEFT:
							rect->Left = rect->Right - dx;
							break;
						case Win32Const.WMSZ_RIGHT:
						case Win32Const.WMSZ_BOTTOMRIGHT:
						case Win32Const.WMSZ_TOPRIGHT:
							rect->Right = rect->Left + dx;
							break;
					}
					
					switch (wParam)
					{
						case Win32Const.WMSZ_BOTTOM:
						case Win32Const.WMSZ_BOTTOMLEFT:
						case Win32Const.WMSZ_BOTTOMRIGHT:
							rect->Bottom = rect->Top + dy;
							break;
						case Win32Const.WMSZ_TOP:
						case Win32Const.WMSZ_TOPLEFT:
						case Win32Const.WMSZ_TOPRIGHT:
							rect->Top = rect->Bottom - dy;
							break;
					}
				}

				msg.Result = new System.IntPtr (1);
				return;
			}
			
//			if (msg.Msg == Win32Const.WM_WINDOWPOSCHANGING)
//			{
//				unsafe
//				{
//					Win32Api.WindowPos* wp = (Win32Api.WindowPos*) msg.LParam.ToPointer ();
//					System.Diagnostics.Debug.WriteLine (" WINDOWPOSCHANGING: "+wp->X+", "+wp->Y+", "+wp->Width+", "+wp->Height+", flags="+wp->Flags.ToString("X8"));
//					System.Diagnostics.Debug.WriteLine (" WindowState: "+this.WindowState.ToString());
//				}
//			}
			if (msg.Msg == Win32Const.WM_WINDOWPOSCHANGED)
			{
				unsafe
				{
					Win32Api.WindowPos* wp = (Win32Api.WindowPos*) msg.LParam.ToPointer ();
					this.formBounds = new System.Drawing.Rectangle (wp->X, wp->Y, wp->Width, wp->Height);
				}

				this.CurrentWindowPlacement = this.NativeWindowPlacement;
			}
			
			if (msg.Msg == Win32Const.WM_APP_DISPOSE)
			{
				System.Diagnostics.Debug.Assert (this.wndProcDepth == 0);
				
				//	L'appelant avait tenté de nous détruire alors qu'il était dans un WndProc,
				//	on reçoint maintenant la commande explicite (asynchrone) qui nous autorise
				//	à nous détruire réellement.
				
				this.Dispose ();
				return;
			}
			
			if (msg.Msg == Win32Const.WM_APP_EXEC_CMD)
			{
				if (this.wndProcDepth == 0)
				{
					try
					{
						if (this.widgetWindow != null)
						{
							this.widgetWindow.DispatchQueuedCommands ();
						}
					}
					catch (System.Exception ex)
					{
						if (RestartManager.UseWindowsErrorReporting)
						{
							throw;
						}
						else
						{
							Window.ProcessException (ex, "WndProc/A");
						}
					}
				}
				else
				{
					this.isDispatchPending = true;
				}
				
				return;
			}
			
			if (msg.Msg == Win32Const.WM_APP_VALIDATION)
			{
				if (this.widgetWindow != null)
				{
					this.widgetWindow.DispatchValidation ();
				}
			}

			this.EnterWndProc ();			
			try
			{
				if (this.WndProcActivation (ref msg))
				{
					return;
				}
				
				//	Tente d'unifier tous les événements qui touchent au clavier, sans faire de traitement
				//	spécial pour les touches pressées en même temps que ALT. Mais si l'on veut que le menu
				//	système continue à fonctionner (ALT + ESPACE), il faut laisser transiter les événements
				//	clavier qui ne concernent que ALT ou ALT + ESPACE sans modification.
				
				int wParam = (int) msg.WParam;
				int lParam = (int) msg.LParam;
				
				if ((wParam != Win32Const.VK_SPACE) && (wParam != Win32Const.VK_MENU))
				{
					switch (msg.Msg)
					{
						case Win32Const.WM_SYSKEYDOWN:	msg.Msg = Win32Const.WM_KEYDOWN;	break;
						case Win32Const.WM_SYSKEYUP:	msg.Msg = Win32Const.WM_KEYUP;		break;
						case Win32Const.WM_SYSCHAR:		msg.Msg = Win32Const.WM_CHAR;		break;
						case Win32Const.WM_SYSDEADCHAR:	msg.Msg = Win32Const.WM_DEADCHAR;	break;
					}
				}

				if ((Support.Globals.IsDebugBuild) &&
					(msg.Msg == Win32Const.WM_KEYDOWN) &&
					(((KeyCode) (int) msg.WParam) == KeyCode.ScrollLock))
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} :", this.widgetWindow.Text));
					Helpers.VisualTree.DebugDump (this.widgetWindow.Root, 1);
				}
				
				//	Filtre les répétitions clavier des touches super-shift. Cela n'a, à mon avis, aucun
				//	sens, puisqu'une touche super-shift est soit enfoncée, soit non enfoncée...
				
				if ((msg.Msg == Win32Const.WM_KEYDOWN) ||
					(msg.Msg == Win32Const.WM_SYSKEYDOWN))
				{
					switch (wParam)
					{
						case Win32Const.VK_SHIFT:
						case Win32Const.VK_CONTROL:
						case Win32Const.VK_MENU:
							if ((lParam & 0x40000000) != 0)
							{
								return;
							}
							break;
					}
				}
				
				if (msg.Msg == Win32Const.WM_MOUSEACTIVATE)
				{
					//	Si l'utilisateur clique dans la fenêtre, on veut recevoir l'événement d'activation
					//	dans tous les cas.
					
					msg.Result = (System.IntPtr) (this.isNoActivate ? Win32Const.MA_NOACTIVATE : Win32Const.MA_ACTIVATE);
					return;
				}
				
				if (msg.Msg == Win32Const.WM_ENTERSIZEMOVE)
				{
					this.StartSizeMove ();
				}
				
				if (msg.Msg == Win32Const.WM_EXITSIZEMOVE)
				{
					this.StopSizeMove ();
				}
				
				if (this.WndProcFiltering (ref msg))
				{
					return;
				}
				
				base.WndProc (ref msg);
			}
			catch (System.Exception ex)
			{
				if (RestartManager.UseWindowsErrorReporting)
				{
					throw;
				}
				else
				{
					Window.ProcessException (ex, "WndProc/B");
				}
			}
			finally
			{
				System.Diagnostics.Debug.Assert (this.IsDisposed == false);
				
				this.ExitWndProc ();
				
				if (Window.IsInAnyWndProc == false)
				{
					Application.ExecuteAsyncCallbacks ();
				}
				
				if ((this.wndProcDepth == 0) &&
					(this.isDispatchPending))
				{
					this.isDispatchPending = false;
					if (this.widgetWindow != null)
					{
						this.widgetWindow.DispatchQueuedCommands ();
					}
				}
			}
		}
		
		protected bool WndProcActivation(ref System.Windows.Forms.Message msg)
		{
			//	Top Level forms should keep their 'active' visual style as long as any top level
			//	form is active. This is implemented by faking WM_NCACTIVATE messages with the
			//	proper settings.
			
			//	The condition for a top level form to be represented with the 'inactive' visual
			//	style is that a modal dialog is being shown; and class CommonDialogs keeps track
			//	of this, which makes it the ideal fake Message provider.
			
			bool active = false;

			switch (msg.Msg)
			{
				case Win32Const.WM_ACTIVATE:
					active = (((int) msg.WParam) != 0);
					
//					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} got WM_ACTIVATE {1}.", this.Name, active));
					
					if (active)
					{
						//	Notre fenêtre vient d'être activée. Si c'est une fenêtre "flottante", alors il faut activer
						//	la fenêtre principale et les autres fenêtres "flottantes".
						
						if (this.isToolWindow)
						{
							Window owner = this.Owner as Window;
							
							if (owner != null)
							{
								owner.FakeActivateOwned (true);
							}
						}
					}
					else
					{
						//	Notre fenêtre vient d'être désactivée.
						
						Widgets.Window window = Widgets.Window.FindFromHandle (msg.LParam);
						
						if (window != null)
						{
							if (this.IsOwnedWindow (window.PlatformWindow))
							{
								//	La fenêtre qui sera activée (et qui a causé notre désactivation) nous appartient.
								//	Si cette fenêtre est "flottante", alors on doit s'assurer que notre état visuel
								//	reste actif.
								
								if (window.PlatformWindow.isToolWindow)
								{
									active = true;
								}
							}
							else if (this.isToolWindow)
							{
								if (this.FindRootOwner () == window.PlatformWindow)
								{
									//	La fenêtre qui va être activée est en fait la propriétaire de cette fenêtre
									//	"flottante". On doit donc conserver l'activation.
									
									active = true;
								}
							}
						}
					}
					
					if (this.isToolWindow)
					{
						//	Il ne faut touiller l'état d'activation des fenêtres que si la fenêtre
						//	actuelle est une palette...
						
						this.FindRootOwner ().FakeActivateOwned (active);
					}
					else
					{
						//	TODO: mieux gérer la question de l'affichage de l'état (activé ou non) des fenêtres
						//	de l'application... On aimerait pouvoir spécifier par programmation l'état à donner
						//	à chaque fenêtre, plus un état général lié à l'activation de l'application et aussi
						//	dépendant de la présence ou non d'un dialogue modal... Pfff...
						
						this.FakeActivate (active);
					}
					
					if ((Window.isAppActive == false) &&
						(active == true))
					{
						Window.isAppActive = true;
//						System.Diagnostics.Debug.WriteLine ("Fire ApplicationActivated (synthetic)");
						if (this.widgetWindow != null)
						{
							this.widgetWindow.OnApplicationActivated ();
						}
					}
					
					break;
				
				case Win32Const.WM_ACTIVATEAPP:
					active = (((int) msg.WParam) != 0);
//					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} got WM_ACTIVATEAPP {1}.", this.Name, active));
					if (Window.isAppActive != active)
					{
						Window.isAppActive = active;
						if (active)
						{
//							System.Diagnostics.Debug.WriteLine ("Fire ApplicationActivated");
							if (this.widgetWindow != null)
							{
								this.widgetWindow.OnApplicationActivated ();
							}
						}
						else
						{
//							System.Diagnostics.Debug.WriteLine ("Fire ApplicationDeactivated");
							if (this.widgetWindow != null)
							{
								this.widgetWindow.OnApplicationDeactivated ();
							}
						}
					}
					break;
				
				case Win32Const.WM_NCACTIVATE:
					active = (((int) msg.WParam) != 0);
//					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} got WM_NCACTIVATE {1}.", this.Name, active));
					msg.Result = (System.IntPtr) 1;
					return true;
			}
			
			return false;
		}
		
		internal Platform.Window FindRootOwner()
		{
			Window owner = this.Owner as Window;
			
			if (owner != null)
			{
				return owner.FindRootOwner ();
			}
			
			return this;
		}
		
		internal Platform.Window[] FindOwnedWindows()
		{
			System.Windows.Forms.Form[] forms = this.OwnedForms;
			Platform.Window[] windows = new Platform.Window[forms.Length];
			
			for (int i = 0; i < forms.Length; i++)
			{
				windows[i] = forms[i] as Platform.Window;
			}
			
			return windows;
		}

		internal bool StartWindowManagerOperation(WindowManagerOperation op)
		{
			//	Documentation sur WM_NCHITTEST et les modes HT...
			//	Cf http://blogs.msdn.com/jfoscoding/archive/2005/07/28/444647.aspx
			//	Cf http://msdn.microsoft.com/netframework/default.aspx?pull=/libarary/en-us/dndotnet/html/automationmodel.asp
			
			switch (op)
			{
				case Platform.WindowManagerOperation.ResizeLeft:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_LEFT);
					return true;
				case Platform.WindowManagerOperation.ResizeRight:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_RIGHT);
					return true;
				case Platform.WindowManagerOperation.ResizeBottom:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_BOTTOM);
					return true;
				case Platform.WindowManagerOperation.ResizeBottomRight:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_BOTTOMRIGHT);
					return true;
				case Platform.WindowManagerOperation.ResizeBottomLeft:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_BOTTOMLEFT);
					return true;
				case Platform.WindowManagerOperation.ResizeTop:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_TOP);
					return true;
				case Platform.WindowManagerOperation.ResizeTopRight:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_TOPRIGHT);
					return true;
				case Platform.WindowManagerOperation.ResizeTopLeft:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_TOPLEFT);
					return true;
				case Platform.WindowManagerOperation.MoveWindow:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_CAPTION);
					return true;
				case Platform.WindowManagerOperation.PressMinimizeButton:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_MINBUTTON);
					return true;
				case Platform.WindowManagerOperation.PressMaximizeButton:
					this.ReleaseCaptureAndSendMessage (Win32Const.HT_MAXBUTTON);
					return true;
			}

			return false;
		}

		private void EnterWndProc()
		{
			this.wndProcDepth++;
			System.Threading.Interlocked.Increment (ref Window.globalWndProcDepth);
		}
		
		private void ExitWndProc()
		{
			System.Diagnostics.Debug.Assert (this.wndProcDepth > 0);
			this.wndProcDepth--;
			System.Threading.Interlocked.Decrement (ref Window.globalWndProcDepth);
		}
		
		private void ReleaseCaptureAndSendMessage(uint ht)
		{
			Win32Api.ReleaseCapture ();
			Win32Api.SendMessage (this.Handle, Platform.Win32Const.WM_NCLBUTTONDOWN, (System.IntPtr) ht, (System.IntPtr) 0);
		}
		
		protected void FakeActivate(bool active)
		{
			if (this.hasActiveFrame != active)
			{
				this.hasActiveFrame = active;
				
				if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
				{
					System.Windows.Forms.Message message = Window.CreateNCActivate (this, active);
//					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} faking WM_NCACTIVATE {1}.", this.Name, active));
					base.WndProc (ref message);
				}
			}
		}
		
		protected void FakeActivateOwned(bool active)
		{
			this.FakeActivate (active);

			System.Windows.Forms.Form[] forms = this.OwnedForms;
			
			for (int i = 0; i < forms.Length; i++)
			{
				Window window = forms[i] as Window;
				
				if (window != null)
				{
					if (window.isToolWindow)
					{
						window.FakeActivate (active);
					}
				}
			}
		}
		
		protected bool IsOwnedWindow(Platform.Window find)
		{
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
		}
		

		protected bool WndProcFiltering(ref System.Windows.Forms.Message msg)
		{
			Message rawMessage = Message.FromWndProcMessage (this, ref msg);
			Message message    = Message.PostProcessMessage (rawMessage);
			
			bool enabled = Win32Api.IsWindowEnabled (this.Handle);

			if (!enabled)
			{
				if (Message.IsMouseMsg (msg))
				{
					return true;
				}
				if ((message != null) &&
					(message.IsKeyType))
				{
					return true;
				}
			}
			
			if (this.filterMouseMessages)
			{
				//	Si le filtre des messages souris est actif, on mange absolument tous
				//	les événements relatifs à la souris, jusqu'à ce que tous les boutons
				//	aient été relâchés.
				
				if (Message.IsMouseMsg (msg))
				{
					if (Message.CurrentState.Buttons == Widgets.MouseButtons.None)
					{
						this.filterMouseMessages = false;
					}
					
					return true;
				}
			}
			
			if (message != null)
			{
				if (this.filterKeyMessages)
				{
					if (message.IsKeyType)
					{
						if (message.MessageType != MessageType.KeyDown)
						{
							return true;
						}
						
						this.filterKeyMessages = false;
					}
				}

				if (this.widgetWindow != null)
				{
					if (this.widgetWindow.FilterMessage (message))
					{
						return true;
					}
				}
				
				if (message.NonClient)
				{
					//	Les messages "non-client" ne sont pas acheminés aux widgets normaux,
					//	car ils ne présentent aucun intérêt. Par contre, le filtre peut les
					//	voir.
					
					return false;
				}
				
				Widgets.Window  wWindow = Message.CurrentState.LastWindow;
				Platform.Window pWindow = (wWindow != null) ? wWindow.PlatformWindow : null;
				
				if (pWindow == null)
				{
					pWindow = this;
				}
				
				if (pWindow.IsDisposed)
				{
					return true;
				}
				
				pWindow.DispatchMessage (message);
				
				return true;
			}
			
			return false;
		}
		
		
		protected static System.Windows.Forms.Message CreateNCActivate(System.Windows.Forms.Form form, bool activate)
		{
			System.Windows.Forms.Message msg;
			msg = System.Windows.Forms.Message.Create (form.Handle, Win32Const.WM_NCACTIVATE, System.IntPtr.Zero, System.IntPtr.Zero);
			
			//	TODO: gère le cas où des fenêtres modales sont ouvertes... Cf. VirtualPen
			
			if (activate)
			{
				msg.WParam = (System.IntPtr)(1);
			}
			
			return msg;
		}

		internal Drawing.Pixmap GetWindowPixmap()
		{
			if ((this.graphics != null) &&
				(this.isPixmapOk))
			{
				return this.graphics.Pixmap;
			}
			else
			{
				return null;
			}
		}

		protected void DispatchPaint(System.Drawing.Graphics winGraphics, System.Drawing.Rectangle winClipRect)
		{
			//	Ce que Windows appelle "Paint", nous l'appelons "Display". En effet, lorsque l'on reçoit un événement
			//	de type WM_PAINT (PaintEvent), on doit simplement afficher le contenu de la fenêtre, sans regénérer le
			//	contenu du pixmap servant de cache.

			if ((this.widgetWindow == null) ||
				(this.widgetWindow.Root == null) ||
				(this.widgetWindow.Root.IsFrozen) ||
				(this.IsDisposed) ||
				(this.widgetWindow.IsDisposed) ||
				(winGraphics == null))
			{
				return;
			}
			
			if ((this.UpdateLayeredWindow ()) &&
				(this.graphics != null))
			{
				Drawing.Pixmap pixmap = this.graphics.Pixmap;
				
				if (pixmap != null)
				{
					System.Drawing.Point offset = new System.Drawing.Point ((int)(this.paintOffset.X), (int)(this.paintOffset.Y));
					pixmap.Paint (winGraphics, offset, winClipRect);
				}
			}
		}
		
		protected bool RefreshGraphics()
		{
			if (this.isLayoutInProgress)
			{
				return false;
			}

			this.isLayoutInProgress = true;
			
			try
			{
				if (this.widgetWindow != null)
				{
					this.widgetWindow.ForceLayout ();
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

			return this.RefreshGraphicsLowLevel ();
		}

		private bool RefreshGraphicsLowLevel()
		{
			if (this.dirtyRectangle.IsValid)
			{
				Drawing.Rectangle repaint = this.dirtyRectangle;
				Drawing.Rectangle[] strips  = this.dirtyRegion.GenerateStrips ();

				this.dirtyRectangle = Drawing.Rectangle.Empty;
				this.dirtyRegion = new Drawing.DirtyRegion ();

				if (this.widgetWindow != null)
				{
					this.widgetWindow.RefreshGraphics (this.graphics, repaint, strips);
				}

				return true;
			}

			return false;
		}
		
		protected bool UpdateLayeredWindow()
		{
			bool paintNeeded = true;

			this.RefreshGraphics ();
			
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
				
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
				
				using (bitmap)
				{
					Drawing.Pixmap.RawData src = new Drawing.Pixmap.RawData (this.graphics.Pixmap);
					Drawing.Pixmap.RawData dst = new Drawing.Pixmap.RawData (bitmap, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

					using (src)
					{
						using (dst)
						{
							src.CopyTo (dst);
						}
					}

//					System.Diagnostics.Debug.WriteLine ("UpdateLayeredWindow" + (this.isFrozen ? " (frozen)" : "") + " Bounds: " + rect.ToString ());
					
					paintNeeded = !Win32Api.UpdateLayeredWindow (this.Handle, bitmap, rect, this.alpha);
				}
			}
			
			return paintNeeded;
		}
		
		
		
		internal static void ProcessException(System.Exception ex, string tag)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("------------------------------------------------------------");
			buffer.Append ("\r\n");
			buffer.Append (tag);
			buffer.Append ("\r\n");
			buffer.Append (System.Diagnostics.Process.GetCurrentProcess ().MainModule.FileVersionInfo.ToString ());
			buffer.Append ("\r\n");
			buffer.Append ("Window: ");
			buffer.Append (System.Diagnostics.Process.GetCurrentProcess ().MainWindowTitle);
			buffer.Append ("\r\n");
			buffer.Append ("Thread: ");
			buffer.Append (System.Threading.Thread.CurrentThread.Name);
			buffer.Append ("\r\n");
			buffer.Append ("\r\n");
			
			while (ex != null)
			{
				buffer.Append ("Exception type: ");
				buffer.Append (ex.GetType ().Name);
				buffer.Append ("\r\n");
				buffer.Append ("Message:        ");
				buffer.Append (ex.Message);
				buffer.Append ("\r\n");
				buffer.Append ("Stack:\r\n");
				buffer.Append (ex.StackTrace);
				
				ex = ex.InnerException;
				
				if (ex != null)
				{
					buffer.Append ("\r\nInner Exception found.\r\n\r\n");
				}
			}
			
			buffer.Append ("\r\n");
			buffer.Append ("------------------------------------------------------------");
			buffer.Append ("\r\n");

			Support.ClipboardWriteData data = new Epsitec.Common.Support.ClipboardWriteData ();
			data.WriteText (buffer.ToString ());
			Support.Clipboard.SetData (data);

			string key   = "Bug report e-mail";
			string email = Globals.Properties.GetProperty (key, "bugs@opac.ch");
			
			string msgFr = "Une erreur interne s'est produite. Veuillez SVP envoyer un mail avec la\n" +
							"description de ce que vous étiez en train de faire au moment où ce message\n" + 
							"est apparu et collez y (CTRL+V) le contenu du presse-papiers.\n\n" +
							"Envoyez s'il-vous-plaît ces informations à " + email + "\n\n" +
							"Merci pour votre aide.";
			
			string msgEn = "An internal error occurred. Please send an e-mail with a short description\n" +
							"of what you were doing when this message appeared and include (press CTRL+V)\n" +
							"contents of the clipboard, which contains useful debugging information.\n\n" +
							"Please send these informations to " + email + "\n\n" +
							"Thank you very much for your help.";
			
			bool isFrench = (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr");
			
			string title   = isFrench ? "Erreur interne" : "Internal error";
			string message = isFrench ? msgFr : msgEn;
			
			System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			System.Windows.Forms.MessageBox.Show (null, message, title);
		}
		
		internal static void ProcessCrossThreadOperation(System.Action action)
		{
			bool state = System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls;

			try
			{
				System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

				action ();
			}
			finally
			{
				System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = state;
			}
		}
		
		
		
		internal void DispatchMessage(Message message)
		{
			if (this.widgetWindow != null)
			{
				this.widgetWindow.DispatchMessage (message);
			}
		}

		
		internal void ShowWindow()
		{
			this.UpdateLayeredWindow ();
			
			if (this.IsMouseActivationEnabled)
			{
				this.Show ();
			}
			else
			{
				Win32Api.ShowWindow (this.Handle, Win32Const.SW_SHOWNA);
			}
		}
		
		internal void ShowDialogWindow()
		{
			this.UpdateLayeredWindow ();
			
			System.Windows.Forms.Application.DoEvents ();

			ToolTip.HideAllToolTips ();

			bool preventQuit = this.preventQuit;
			int wndProcDepth = Window.globalWndProcDepth;

			try
			{
				this.preventQuit = true;
				Window.globalWndProcDepth = 0;

				if ((this.Owner != null) &&
					(this.Owner.InvokeRequired))
				{
					//	Don't set the owner... it won't work ! First turn off the cross-thread
					//	checking, or else we will crash (bug in WinForms).
					//	See http://stackoverflow.com/questions/5273674/cross-thread-exception-when-setting-winforms-form-owner-how-to-do-it-right

					Platform.Window.ProcessCrossThreadOperation (() => this.ShowDialog (this.Owner));
				}
				else
				{
					this.ShowDialog (this.Owner);
				}
			}
			finally
			{
				this.preventQuit = preventQuit;
				Window.globalWndProcDepth = wndProcDepth;
			}
		}

		class WindowHandleWrapper : System.Windows.Forms.IWin32Window
		{
			public WindowHandleWrapper(System.IntPtr handle)
			{
				this.handle = handle;
			}

			#region IWin32Window Members

			System.IntPtr System.Windows.Forms.IWin32Window.Handle
			{
				get
				{
					return this.handle;
				}
			}

			#endregion

			private readonly System.IntPtr handle;
		}

		#region IApplicationThreadInvoker Members

		void Epsitec.Common.Types.BindingAsyncOperation.IApplicationThreadInvoker.Invoke(SimpleCallback method)
		{
			//	Execute the method in the main UI thread. If the caller is
			//	on another thread, we have to go through the not specially
			//	pleasant WndProc-based "Invoke" mechanism :
			
			if (this.InvokeRequired)
			{
				this.Invoke (method);
			}
			else
			{
				method ();
			}
		}

		#endregion
		
		private bool							widgetWindowDisposed;
		private Epsitec.Common.Widgets.Window	widgetWindow;
		
		private Drawing.Graphics				graphics;
		private Drawing.Rectangle				dirtyRectangle;
		private Drawing.DirtyRegion				dirtyRegion;
		private Drawing.Rectangle				windowBounds;
		private Drawing.Point					paintOffset;
		private System.Drawing.Rectangle		formBounds;
		private System.Drawing.Size				formMinSize;
		private System.Drawing.Size				minimumSize;
		private bool							formBoundsSet = false;
		private bool							onResizeEvent = false;
		
		private bool							isLayered;
		private bool							isLayeredDirty;
		private bool							isFrozen;
		private bool							isAnimatingActiveWindow;
		private bool							isNoActivate;
		private bool							isToolWindow;
		
		private bool							hasActiveFrame;
		
		private bool							preventClose;
		private bool							preventQuit;
		private bool							forcedClose;
		private bool							filterMouseMessages;
		private bool							filterKeyMessages;
		private double							alpha = 1.0;
		
		private WindowMode						windowMode = WindowMode.Window;
		private WindowStyles					windowStyles;
		private WindowType						windowType;
		
		private static int						globalWndProcDepth;

		private int								wndProcDepth;
		
		private bool							isDispatchPending;
		private bool							isPixmapOk;
		private bool							isSizeMoveInProgress;
		private bool							isLayoutInProgress;
		private readonly SafeCounter			isSyncPaintDisabled;
		private readonly SafeCounter			isSyncUpdating;
		private readonly SafeCounter			isWndProcHandlingRestricted;

		private WindowPlacement					windowPlacement;
		
		private static bool						isAppActive;
		private static bool						isSyncRequested;
		private static bool						isAwakeRequested;
		private static Window					dispatchWindow;
		private static System.IntPtr			dispatchWindowHandle;
	}
}
