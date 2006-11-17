//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler (Window.HandleSystemEventsUserPreferenceChanged);
			
			Window.dispatch_window = new Window ();
			Window.DummyHandleEater (Window.dispatch_window.Handle);
			
			//	The asynchronous binding mechanisms need to be able to execute
			//	code on the main application thread. Thus, we have to register
			//	the special thread invoker interface :
			
			Types.BindingAsyncOperation.DefineApplicationThreadInvoker (Window.dispatch_window);
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
		
		
		private delegate void BoundsOffsetCallback(Drawing.Rectangle bounds, Drawing.Point offset);
		private delegate void AnimatorCallback(Animator animator);
		private delegate void DoubleCallback(double value);
		
		private Window()
		{
		}
		
		
		internal Window(Epsitec.Common.Widgets.Window window)
		{
			this.widget_window = window;
			
			this.dirty_rectangle = Drawing.Rectangle.Empty;
			this.dirty_region    = new Drawing.DirtyRegion ();

			base.MinimumSize = new System.Drawing.Size (1, 1);

			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);
			
			this.WindowType   = WindowType.Document;
			this.WindowStyles = WindowStyles.CanResize | WindowStyles.HasCloseButton;
			
			this.graphics = new Epsitec.Common.Drawing.Graphics ();
			this.graphics.AllocatePixmap ();
			
			Window.DummyHandleEater (this.Handle);
			
			//	Fait en sorte que les changements de dimensions en [x] et en [y] provoquent un
			//	redessin complet de la fenêtre, sinon Windows tente de recopier l'ancien contenu
			//	en le décalant, ce qui donne des effets bizarres :
			
			int class_window_style = Win32Api.GetClassLong (this.Handle, Win32Const.GCL_STYLE);
			
			class_window_style |= Win32Const.CS_HREDRAW;
			class_window_style |= Win32Const.CS_VREDRAW;
			
			Win32Api.SetClassLong (this.Handle, Win32Const.GCL_STYLE, class_window_style);
			
			this.ReallocatePixmap ();
			
			WindowList.Insert (this);
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
		}
		
		internal void MakeFixedSizeWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox     = false;
			this.MinimizeBox     = false;
			Window.DummyHandleEater (this.Handle);
		}
		
		internal void MakeButtonlessWindow()
		{
			this.ControlBox      = false;
			this.MaximizeBox     = false;
			this.MinimizeBox     = false;
			Window.DummyHandleEater (this.Handle);
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
			this.is_tool_window  = true;
			Window.DummyHandleEater (this.Handle);
		}
		
		internal void MakeFloatingWindow()
		{
			this.ShowInTaskbar   = false;
			this.is_tool_window  = true;
			Window.DummyHandleEater (this.Handle);
		}
		
		internal void ResetHostingWidgetWindow()
		{
			this.widget_window_disposed = true;
		}
		
		
		static void DummyHandleEater(System.IntPtr handle)
		{
		}
		
		
		internal void AnimateShow(Animation animation, Drawing.Rectangle bounds)
		{
			Window.DummyHandleEater (this.Handle);

			if (this.is_layered)
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
			
			double start_alpha = this.alpha;
			
			this.is_animating_active_window = true;
			this.WindowBounds = bounds;
			this.MarkForRepaint ();
			this.RefreshGraphics ();
			
			Animator animator;
			
			switch (animation)
			{
				default:
				case Animation.None:
					this.ShowWindow ();
					this.is_animating_active_window = false;
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
					this.is_frozen = true;
					this.IsLayered = true;
					this.Alpha = 0.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeInTime);
					animator.SetCallback (new DoubleCallback (this.AnimateAlpha), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (0.0, start_alpha);
					animator.Start ();
					this.ShowWindow ();
					return;
				
				case Animation.FadeOut:
					this.is_frozen = true;
					this.IsLayered = true;
//					this.Alpha = 1.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeOutTime);
					animator.SetCallback (new DoubleCallback (this.AnimateAlpha), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (start_alpha, 0.0);
					animator.Start ();
					return;
			}
			
			switch (animation)
			{
				case Animation.RollDown:
				case Animation.RollUp:
				case Animation.RollRight:
				case Animation.RollLeft:
					this.is_frozen = true;
					this.form_min_size = this.MinimumSize;
					this.MinimumSize = new System.Drawing.Size (1, 1);
					this.WindowBounds = b1;
					this.UpdateLayeredWindow ();
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback (new BoundsOffsetCallback (this.AnimateWindowBounds), new AnimatorCallback (this.AnimateCleanup));
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
			
			double start_alpha = this.alpha;
			
			this.WindowBounds = bounds;
			this.MarkForRepaint ();
			this.RefreshGraphics ();
			
			Animator animator;
			
			switch (animation)
			{
				case Animation.None:
					this.Hide ();
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
					this.Hide ();
					return;
			}
			
			switch (animation)
			{
				case Animation.RollDown:
				case Animation.RollUp:
				case Animation.RollRight:
				case Animation.RollLeft:
					this.is_frozen = true;
					this.is_animating_active_window = this.IsActive;
					this.WindowBounds = b1;
					this.UpdateLayeredWindow ();
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback (new BoundsOffsetCallback (this.AnimateWindowBounds), new AnimatorCallback (this.AnimateCleanup));
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
			this.paint_offset = offset;
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
			
			this.MinimumSize = this.form_min_size;
			this.is_frozen = false;
			this.is_animating_active_window = false;
			this.Invalidate ();
			this.widget_window.OnWindowAnimationEnded ();
		}
		
		
		
		internal WindowStyles					WindowStyles
		{
			get
			{
				return this.window_styles;
			}
			set
			{
				if (this.window_styles != value)
				{
					this.window_styles = value;
					this.UpdateWindowTypeAndStyles ();
				}
			}
		}
		
		internal WindowType						WindowType
		{
			get
			{
				return this.window_type;
			}
			set
			{
				if (this.window_type != value)
				{
					this.window_type = value;
					this.UpdateWindowTypeAndStyles ();
				}
			}
		}
		
		
		private void UpdateWindowTypeAndStyles()
		{
			switch (this.window_type)
			{
				case WindowType.Document:
					if ((this.window_styles & WindowStyles.CanResize) == 0)
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
					if ((this.window_styles & WindowStyles.CanResize) == 0)
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
					if ((this.window_styles & WindowStyles.CanResize) == 0)
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
			
			this.MinimizeBox = ((this.window_styles & WindowStyles.CanMinimize)    != 0);
			this.MaximizeBox = ((this.window_styles & WindowStyles.CanMaximize)    != 0);
			this.HelpButton  = ((this.window_styles & WindowStyles.HasHelpButton)  != 0);
			this.ControlBox  = ((this.window_styles & WindowStyles.HasCloseButton) != 0);
		}
		
		
		internal bool							PreventSyncPaint
		{
			get
			{
				return this.disable_sync_paint > 0;
			}
		}
		
		internal bool							PreventAutoClose
		{
			get
			{
				return this.prevent_close;
			}
			set
			{
				this.prevent_close = value;
			}
		}
		
		internal bool							PreventAutoQuit
		{
			get
			{
				return this.prevent_quit;
			}
			set
			{
				this.prevent_quit = value;
			}
		}
		
		internal bool							IsLayered
		{
			get
			{
				return this.is_layered;
			}
			set
			{
				if (this.is_layered != value)
				{
					if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
					{
						throw new System.Exception ("A layered window may not have a border");
					}
					
					if (SystemInformation.SupportsLayeredWindows)
					{
						int ex_style = Win32Api.GetWindowExStyle (this.Handle);
						
						if (value)
						{
							ex_style |= Win32Const.WS_EX_LAYERED;
						}
						else
						{
							ex_style &= ~ Win32Const.WS_EX_LAYERED;
						}
						
						Win32Api.SetWindowExStyle (this.Handle, ex_style);
						this.is_layered = value;
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
				return (this.is_frozen)
					|| (this.widget_window == null)
					|| (this.widget_window.Root == null);
			}
		}
		
		internal bool							IsAnimatingActiveWindow
		{
			get
			{
				return this.is_animating_active_window;
			}
		}
		
		internal bool							IsMouseActivationEnabled
		{
			get
			{
				return !this.is_no_activate;
			}
			
			set
			{
				this.is_no_activate = !value;
			}
		}
		
		internal bool							IsToolWindow
		{
			get
			{
				return this.is_tool_window;
			}
		}
		
		internal bool							IsSizeMoveInProgress
		{
			get
			{
				return this.is_size_move_in_progress;
			}
			set
			{
				if (this.is_size_move_in_progress != value)
				{
					this.is_size_move_in_progress = value;
					this.widget_window.OnWindowSizeMoveStatusChanged ();
				}
			}
		}
		
		
		internal WindowMode						WindowMode
		{
			get
			{
				return this.window_mode;
			}
			set
			{
				this.window_mode = value;
			}
		}
		
		internal Drawing.Rectangle				WindowBounds
		{
			get
			{
				System.Drawing.Rectangle rect;
				
				if (this.form_bounds_set)
				{
					rect = this.form_bounds;
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
				if (this.window_bounds != value)
				{
					int ox = this.MapToWinFormsX (value.Left);
					int oy = this.MapToWinFormsY (value.Top);
					int dx = this.MapToWinFormsWidth (value.Width);
					int dy = this.MapToWinFormsHeight (value.Height);
					
					this.window_bounds   = value;
					this.form_bounds     = new System.Drawing.Rectangle (ox, oy, dx, dy);
					this.form_bounds_set = true;
					this.on_resize_event = true;
					
					if (this.is_layered)
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
				
				if (this.form_bounds_set)
				{
					int deltaWidth  = this.form_bounds.Width - this.Width;
					int deltaHeight = this.form_bounds.Height - this.Height;
					
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
				return this.minimum_size;
			}
			set
			{
				this.minimum_size = value;
			}
		}
		
		internal Drawing.Rectangle				WindowPlacementNormalBounds
		{
			get
			{
				Win32Api.WindowPlacement placement = new Win32Api.WindowPlacement ();
				placement.Length = 4+4+4+8+8+16;
				Win32Api.GetWindowPlacement (this.Handle, out placement);
				
				double ox = this.MapFromWinFormsX (placement.NormalPosition.Left);
				double oy = this.MapFromWinFormsY (placement.NormalPosition.Bottom);
				double dx = this.MapFromWinFormsWidth (placement.NormalPosition.Right - placement.NormalPosition.Left);
				double dy = this.MapFromWinFormsHeight (placement.NormalPosition.Bottom - placement.NormalPosition.Top);
				
				//	Attention: les coordonnées retournées par WindowPlacement sont exprimées
				//	en "workspace coordinates" (elles tiennent compte de la présence d'une
				//	barre "Desktop").
				
				//	Cf. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/windows/windowreference/windowstructures/windowplacement.asp

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
					
						Win32Api.GetWindowPlacement (f.Handle, out placement);
					
						ox -= placement.NormalPosition.Left - f.Location.X;
						oy += placement.NormalPosition.Top - f.Location.Y;
						
//						System.Diagnostics.Trace.WriteLine ("Adjust X by " + (-placement.NormalPosition.Left + f.Location.X));
//						System.Diagnostics.Trace.WriteLine ("Adjust Y by " + (placement.NormalPosition.Top - f.Location.Y));
					}
				}
				
				return new Drawing.Rectangle (ox, oy, dx, dy);
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
		
		internal double							Alpha
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
				System.Drawing.Size border_size = new System.Drawing.Size (0, 0);
				
				switch (this.FormBorderStyle)
				{
					case System.Windows.Forms.FormBorderStyle.Fixed3D:
						border_size = System.Windows.Forms.SystemInformation.Border3DSize;
						break;
					
					case System.Windows.Forms.FormBorderStyle.Sizable:
					case System.Windows.Forms.FormBorderStyle.SizableToolWindow:
						border_size = System.Windows.Forms.SystemInformation.FrameBorderSize;
						break;
					
					case System.Windows.Forms.FormBorderStyle.FixedDialog:
						border_size = System.Windows.Forms.SystemInformation.FixedFrameBorderSize;
						break;
					
					case System.Windows.Forms.FormBorderStyle.FixedSingle:
					case System.Windows.Forms.FormBorderStyle.FixedToolWindow:
						border_size = new System.Drawing.Size (1, 1);
						break;
					
					case System.Windows.Forms.FormBorderStyle.None:
						break;
				}
				
				return border_size;
			}
		}
		
		
		internal bool							FilterMouseMessages
		{
			get { return this.filter_mouse_messages; }
			set { this.filter_mouse_messages = value; }
		}
		
		internal bool							FilterKeyMessages
		{
			get { return this.filter_key_messages; }
			set { this.filter_key_messages = value; }
		}
		
		
		internal Epsitec.Common.Widgets.Window	HostingWidgetWindow
		{
			get { return this.widget_window; }
		}
		
		internal static bool					IsApplicationActive
		{
			get { return Window.is_app_active; }
		}
		
		
		internal new void Close()
		{
			try
			{
				this.forced_close = true;
				base.Close ();
			}
			finally
			{
				this.forced_close = false;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				WindowList.Remove (this);
				
				//	Attention: il n'est pas permis de faire un Dispose si l'appelant provient d'une
				//	WndProc, car cela perturbe le bon acheminement des messages dans Windows. On
				//	préfère donc remettre la destruction à plus tard si on détecte cette condition.
				
				if (this.wnd_proc_depth > 0)
				{
					Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_DISPOSE, System.IntPtr.Zero, System.IntPtr.Zero);
					return;
				}
				
				if (this.graphics != null)
				{
					this.graphics.Dispose ();
				}
				
				this.graphics = null;
				
				if (this.widget_window != null)
				{
					if (this.widget_window_disposed == false)
					{
						this.widget_window.PlatformWindowDisposing ();
						this.widget_window_disposed = true;
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
			
			this.widget_window.OnWindowClosed ();
			
			base.OnClosed (e);
		}
		
		protected override void OnGotFocus(System.EventArgs e)
		{
			base.OnGotFocus (e);
			this.widget_window.NotifyWindowFocused ();
		}
		
		protected override void OnLostFocus(System.EventArgs e)
		{
			base.OnLostFocus (e);
			this.widget_window.NotifyWindowDefocused ();
		}


		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			
			if (this.forced_close)
			{
				return;
			}
			
			this.widget_window.OnWindowCloseClicked ();
			
			if (this.prevent_close)
			{
				e.Cancel = true;
				
				if (this.prevent_quit)
				{
					return;
				}
				
				//	Empêche la fermeture de la fenêtre lorsque l'utilisateur clique sur le bouton de
				//	fermeture, et synthétise un événement clavier ALT + F4 à la place...
				
				System.Windows.Forms.Keys alt_f4 = System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt;
				System.Windows.Forms.KeyEventArgs fake_event = new System.Windows.Forms.KeyEventArgs (alt_f4);
				Message message = Message.FromKeyEvent (MessageType.KeyDown, fake_event);
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
			System.Windows.Forms.MouseEventArgs fake_event = new System.Windows.Forms.MouseEventArgs (System.Windows.Forms.MouseButtons.None, 0, point.X, point.Y, 0);
			
			Message message = Message.FromMouseEvent (MessageType.MouseEnter, this, fake_event);
			
			if (this.widget_window.FilterMessage (message) == false)
			{
				this.DispatchMessage (message);
			}
		}

		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave (e);
			
			Message message = Message.FromMouseEvent (MessageType.MouseLeave, this, null);
			
			if (this.widget_window.FilterMessage (message) == false)
			{
				this.DispatchMessage (message);
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

		protected override void OnSizeChanged(System.EventArgs e)
		{
//			System.Diagnostics.Debug.WriteLine ("OnSizeChanged");
			this.disable_sync_paint++;
			
			try
			{
				if ((this.Created == false) &&
					(this.form_bounds_set) &&
					(this.form_bounds.Size != this.Size))
				{
					this.Size = this.form_bounds.Size;
				}
				else if ((this.form_bounds_set) &&
					     (this.form_bounds.Size == this.Size) &&
					     (this.on_resize_event == false))
				{
					//	Rien à faire, car la taille correspond à la dernière taille mémorisée.
				}
				else
				{
					this.form_bounds_set = true;
					this.on_resize_event = false;
					this.form_bounds     = this.Bounds;
					this.window_bounds   = this.WindowBounds;
					
					base.OnSizeChanged (e);
					this.ReallocatePixmap ();
				}
				
				this.form_bounds_set = false;
			}
			finally
			{
				this.disable_sync_paint--;
			}
		}
		
		protected override void OnActivated(System.EventArgs e)
		{
			base.OnActivated (e);
			
			if (this.widget_window != null)
			{
				this.widget_window.OnWindowActivated ();
			}
		}
		
		protected override void OnDeactivate(System.EventArgs e)
		{
			base.OnDeactivate (e);
			
			if (this.widget_window != null)
			{
				this.widget_window.OnWindowDeactivated ();
			}
		}

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged (e);
			
			if (this.Visible)
			{
				if (! this.is_pixmap_ok)
				{
					this.ReallocatePixmap ();
				}
				this.widget_window.OnWindowShown ();
			}
			else
			{
				this.widget_window.OnWindowHidden ();
			}
		}
		
		
		protected override void OnDragEnter(System.Windows.Forms.DragEventArgs drgevent)
		{
			base.OnDragEnter (drgevent);
			
			if (this.widget_window != null)
			{
				this.widget_window.OnWindowDragEntered (new WindowDragEventArgs (drgevent));
			}
		}
		
		protected override void OnDragLeave(System.EventArgs e)
		{
			base.OnDragLeave (e);
			
			if (this.widget_window != null)
			{
				this.widget_window.OnWindowDragLeft ();
			}
		}
		
		protected override void OnDragDrop(System.Windows.Forms.DragEventArgs drgevent)
		{
			base.OnDragDrop (drgevent);
			
			if (this.widget_window != null)
			{
				this.widget_window.OnWindowDragDropped (new WindowDragEventArgs (drgevent));
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
//				System.Diagnostics.Debug.WriteLine ("ReallocatePixmapLowLevel" + (this.is_frozen ? " (frozen)" : "") + " Size: " + width.ToString () + "," + height.ToString());
				
				this.graphics.Pixmap.Clear ();
				
				this.widget_window.Root.NotifyWindowSizeChanged (width, height);
				this.dirty_rectangle = new Drawing.Rectangle (0, 0, width, height);
				this.dirty_region    = new Drawing.DirtyRegion ();
				this.dirty_region.Add (this.dirty_rectangle);

				changed = true;
			}
			
			this.is_pixmap_ok = true;
			
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
			
			this.dirty_rectangle.MergeWith (rect);
			this.dirty_region.Add (rect);
			
			int top    = (int) (rect.Top);
			int bottom = (int) (rect.Bottom);
			
			int width  = (int) (rect.Width);
			int height = top - bottom + 1;
			int x      = (int) (rect.Left);
			int y      = this.ClientSize.Height - top;
			
			if (this.is_layered)
			{
				this.is_layered_dirty = true;
			}
			
			this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
		}
		
		internal void SynchronousRepaint()
		{
			if (this.is_layout_in_progress)
			{
				return;
			}

			this.is_layout_in_progress = true;

			try
			{
				this.widget_window.ForceLayout ();
			}
			finally
			{
				this.is_layout_in_progress = false;
			}
			
			if (this.dirty_rectangle.IsValid)
			{
				this.Update ();
			}
		}
		
		internal void SendQueueCommand()
		{
			Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_EXEC_CMD, System.IntPtr.Zero, System.IntPtr.Zero);
		}
		
		internal void SendValidation()
		{
			Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_VALIDATION, System.IntPtr.Zero, System.IntPtr.Zero);
		}
		
		
		internal void StartSizeMove()
		{
			this.IsSizeMoveInProgress = true;
			this.disable_sync_paint++;
		}
		
		internal void StopSizeMove()
		{
			this.IsSizeMoveInProgress = false;
			this.disable_sync_paint--;
		}
		
		
		internal static void SendSynchronizeCommandCache()
		{
			Window.is_sync_requested = true;
			
			try
			{
				Win32Api.PostMessage (Window.dispatch_window.Handle, Win32Const.WM_APP_SYNCMDCACHE, System.IntPtr.Zero, System.IntPtr.Zero);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Exception thrown in Platform.Window.SendSynchronizeCommandCache :");
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
		}

		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
		{
//			System.Diagnostics.Debug.WriteLine (msg.ToString ());
			Application.ExecuteAsyncCallbacks ();

			if (RestartManager.HandleWndProc (ref msg))
			{
				return;
			}
			
			if (Window.is_sync_requested)
			{
				Window.is_sync_requested = false;
				CommandCache.Instance.Synchronize ();
			}
			
			if (Window.dispatch_window == this)
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

//					System.Diagnostics.Debug.WriteLine (string.Format ("dx={0} MinWidth={1} MinimumSize={2}", rect->Right - rect->Left, this.minimum_size.Width, base.MinimumSize.Width));

					int dx = System.Math.Max (this.minimum_size.Width, rect->Right - rect->Left);
					int dy = System.Math.Max (this.minimum_size.Height, rect->Bottom - rect->Top);

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
					this.form_bounds = new System.Drawing.Rectangle (wp->X, wp->Y, wp->Width, wp->Height);
				}
			}
			
			if (msg.Msg == Win32Const.WM_APP_DISPOSE)
			{
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth == 0);
				
				//	L'appelant avait tenté de nous détruire alors qu'il était dans un WndProc,
				//	on reçoint maintenant la commande explicite (asynchrone) qui nous autorise
				//	à nous détruire réellement.
				
				this.Dispose ();
				return;
			}
			
			if (msg.Msg == Win32Const.WM_APP_EXEC_CMD)
			{
				if (this.wnd_proc_depth == 0)
				{
					try
					{
						this.widget_window.DispatchQueuedCommands ();
					}
					catch (System.Exception ex)
					{
						Window.ProcessException (ex, "WndProc/A");
					}
				}
				else
				{
					this.is_dispatch_pending = true;
				}
				
				return;
			}
			
			if (msg.Msg == Win32Const.WM_APP_VALIDATION)
			{
				this.widget_window.DispatchValidation ();
			}
			
			this.wnd_proc_depth++;
			
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
				
				int w_param = (int) msg.WParam;
				int l_param = (int) msg.LParam;
				
				if ((w_param != Win32Const.VK_SPACE) && (w_param != Win32Const.VK_MENU))
				{
					switch (msg.Msg)
					{
						case Win32Const.WM_SYSKEYDOWN:	msg.Msg = Win32Const.WM_KEYDOWN;	break;
						case Win32Const.WM_SYSKEYUP:	msg.Msg = Win32Const.WM_KEYUP;		break;
						case Win32Const.WM_SYSCHAR:		msg.Msg = Win32Const.WM_CHAR;		break;
						case Win32Const.WM_SYSDEADCHAR:	msg.Msg = Win32Const.WM_DEADCHAR;	break;
					}
				}
				
				//	Filtre les répétitions clavier des touches super-shift. Cela n'a, à mon avis, aucun
				//	sens, puisqu'une touche super-shift est soit enfoncée, soit non enfoncée...
				
				if ((msg.Msg == Win32Const.WM_KEYDOWN) ||
					(msg.Msg == Win32Const.WM_SYSKEYDOWN))
				{
					switch (w_param)
					{
						case Win32Const.VK_SHIFT:
						case Win32Const.VK_CONTROL:
						case Win32Const.VK_MENU:
							if ((l_param & 0x40000000) != 0)
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
					
					msg.Result = (System.IntPtr) (this.is_no_activate ? Win32Const.MA_NOACTIVATE : Win32Const.MA_ACTIVATE);
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
				Window.ProcessException (ex, "WndProc/B");
			}
			finally
			{
				System.Diagnostics.Debug.Assert (this.IsDisposed == false);
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth > 0);
				this.wnd_proc_depth--;
				
				if ((this.wnd_proc_depth == 0) &&
					(this.is_dispatch_pending))
				{
					this.is_dispatch_pending = false;
					this.widget_window.DispatchQueuedCommands ();
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
						
						if (this.is_tool_window)
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
								
								if (window.PlatformWindow.is_tool_window)
								{
									active = true;
								}
							}
							else if (this.is_tool_window)
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
					
					if (this.is_tool_window)
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
					
					if ((Window.is_app_active == false) &&
						(active == true))
					{
						Window.is_app_active = true;
//						System.Diagnostics.Debug.WriteLine ("Fire ApplicationActivated (synthetic)");
						this.widget_window.OnApplicationActivated ();
					}
					
					break;
				
				case Win32Const.WM_ACTIVATEAPP:
					active = (((int) msg.WParam) != 0);
//					System.Diagnostics.Debug.WriteLine (string.Format ("Window {0} got WM_ACTIVATEAPP {1}.", this.Name, active));
					if (Window.is_app_active != active)
					{
						Window.is_app_active = active;
						if (active)
						{
//							System.Diagnostics.Debug.WriteLine ("Fire ApplicationActivated");
							this.widget_window.OnApplicationActivated ();
						}
						else
						{
//							System.Diagnostics.Debug.WriteLine ("Fire ApplicationDeactivated");
							this.widget_window.OnApplicationDeactivated ();
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

		private void ReleaseCaptureAndSendMessage(uint ht)
		{
			Win32Api.ReleaseCapture ();
			Win32Api.SendMessage (this.Handle, Platform.Win32Const.WM_NCLBUTTONDOWN, (System.IntPtr) ht, (System.IntPtr) 0);
		}
		
		protected void FakeActivate(bool active)
		{
			if (this.has_active_frame != active)
			{
				this.has_active_frame = active;
				
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
					if (window.is_tool_window)
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
			Message message = Message.FromWndProcMessage (this, ref msg);
			bool    enabled = Win32Api.IsWindowEnabled (this.Handle);

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
			
			if (this.filter_mouse_messages)
			{
				//	Si le filtre des messages souris est actif, on mange absolument tous
				//	les événements relatifs à la souris, jusqu'à ce que tous les boutons
				//	aient été relâchés.
				
				if (Message.IsMouseMsg (msg))
				{
					if (Message.CurrentState.Buttons == Widgets.MouseButtons.None)
					{
						this.filter_mouse_messages = false;
					}
					
					return true;
				}
			}
			
			if (message != null)
			{
				if (this.filter_key_messages)
				{
					if (message.IsKeyType)
					{
						if (message.MessageType != MessageType.KeyDown)
						{
							return true;
						}
						
						this.filter_key_messages = false;
					}
				}
				
				if (this.widget_window.FilterMessage (message))
				{
					return true;
				}
				
				if (message.NonClient)
				{
					//	Les messages "non-client" ne sont pas acheminés aux widgets normaux,
					//	car ils ne présentent aucun intérêt. Par contre, le filtre peut les
					//	voir.
					
					return false;
				}
				
				Widgets.Window  w_window = Message.CurrentState.LastWindow;
				Platform.Window p_window = (w_window != null) ? w_window.PlatformWindow : null;
				
				if (p_window == null)
				{
					p_window = this;
				}
				
				if (p_window.IsDisposed)
				{
					return true;
				}
				
				p_window.DispatchMessage (message);
				
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
		
		
		protected void DispatchPaint(System.Drawing.Graphics win_graphics, System.Drawing.Rectangle win_clip_rect)
		{
			//	Ce que Windows appelle "Paint", nous l'appelons "Display". En effet, lorsque l'on reçoit un événement
			//	de type WM_PAINT (PaintEvent), on doit simplement afficher le contenu de la fenêtre, sans regénérer le
			//	contenu du pixmap servant de cache.
			
			if ((this.UpdateLayeredWindow ()) &&
				(this.graphics != null))
			{
				Drawing.Pixmap pixmap = this.graphics.Pixmap;
				
				if (pixmap != null)
				{
					System.Drawing.Point offset = new System.Drawing.Point ((int)(this.paint_offset.X), (int)(this.paint_offset.Y));
					pixmap.Paint (win_graphics, offset, win_clip_rect);
				}
			}
		}
		
		protected bool RefreshGraphics()
		{
			if (this.is_layout_in_progress)
			{
				return false;
			}

			this.is_layout_in_progress = true;
			
			try
			{
				this.widget_window.ForceLayout ();
			}
			finally
			{
				this.is_layout_in_progress = false;
			}

			if (this.IsFrozen)
			{
				return false;
			}

			return this.RefreshGraphicsLowLevel ();
		}

		private bool RefreshGraphicsLowLevel()
		{
			if (this.dirty_rectangle.IsValid)
			{
				Drawing.Rectangle repaint = this.dirty_rectangle;
				Drawing.Rectangle[] strips  = this.dirty_region.GenerateStrips ();

				this.dirty_rectangle = Drawing.Rectangle.Empty;
				this.dirty_region = new Drawing.DirtyRegion ();

				this.widget_window.RefreshGraphics (this.graphics, repaint, strips);

				return true;
			}

			return false;
		}
		
		protected bool UpdateLayeredWindow()
		{
			bool paint_needed = true;

			this.RefreshGraphics ();
			
			if (this.is_layered)
			{
				if (this.is_layered_dirty)
				{
					this.is_layered_dirty = false;
				}

				//	UpdateLayeredWindow can be called as the result of setting a
				//	new WindowBounds rectangle. If this is the case, the Bounds
				//	property, inherited from WinForms, won't have been updated
				//	yet, so use the cached value :
				
				System.Drawing.Rectangle rect;
				
				if (this.form_bounds_set)
				{
					rect = this.form_bounds;
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

//					System.Diagnostics.Debug.WriteLine ("UpdateLayeredWindow" + (this.is_frozen ? " (frozen)" : "") + " Bounds: " + rect.ToString ());
					
					paint_needed = !Win32Api.UpdateLayeredWindow (this.Handle, bitmap, rect, this.alpha);
				}
			}
			
			return paint_needed;
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

			Support.Clipboard.WriteData data = new Epsitec.Common.Support.Clipboard.WriteData ();
			data.WriteText (buffer.ToString ());
			Support.Clipboard.SetData (data);
			
			string msg_fr = "Une erreur interne s'est produite. Veuillez SVP envoyer un mail avec la\n" +
							"description de ce que vous étiez en train de faire au moment où ce message\n" + 
							"est apparu et collez y (CTRL+V) le contenu du presse-papiers.\n\n" +
							"Envoyez s'il-vous-plaît ces informations à bugs@opac.ch\n\n" +
							"Merci pour votre aide.";
			
			string msg_en = "An internal error occurred. Please send an e-mail with a short description\n" +
							"of what you were doing when this message appeared and include (press CTRL+V)\n" +
							"contents of the clipboard, which contains useful debugging information.\n\n" +
							"Please send these informations to bugs@opac.ch\n\n" +
							"Thank you very much for your help.";
			
			bool is_french = (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr");
			
			string title   = is_french ? "Erreur interne" : "Internal error";
			string message = is_french ? msg_fr : msg_en;
			
			System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			System.Windows.Forms.MessageBox.Show (null, message, title);
		}
		
		
		internal void DispatchMessage(Message message)
		{
			if (this.widget_window != null)
			{
				this.widget_window.DispatchMessage (message);
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
			
			this.ShowDialog (this.Owner);
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
		
		private bool							widget_window_disposed;
		private Epsitec.Common.Widgets.Window	widget_window;
		
		private Drawing.Graphics				graphics;
		private Drawing.Rectangle				dirty_rectangle;
		private Drawing.DirtyRegion				dirty_region;
		private Drawing.Rectangle				window_bounds;
		private Drawing.Point					paint_offset;
		private System.Drawing.Rectangle		form_bounds;
		private System.Drawing.Size				form_min_size;
		private System.Drawing.Size				minimum_size;
		private bool							form_bounds_set = false;
		private bool							on_resize_event = false;
		
		private bool							is_layered;
		private bool							is_layered_dirty;
		private bool							is_frozen;
		private bool							is_animating_active_window;
		private bool							is_no_activate;
		private bool							is_tool_window;
		
		private bool							has_active_frame;
		
		private bool							prevent_close;
		private bool							prevent_quit;
		private bool							forced_close;
		private bool							filter_mouse_messages;
		private bool							filter_key_messages;
		private double							alpha = 1.0;
		
		private WindowMode						window_mode = WindowMode.Window;
		private WindowStyles					window_styles;
		private WindowType						window_type;
		
		private int								wnd_proc_depth;
		private bool							is_dispatch_pending;
		private bool							is_pixmap_ok;
		private bool							is_size_move_in_progress;
		private bool							is_layout_in_progress;
		private int								disable_sync_paint;
		
		private static bool						is_app_active;
		private static bool						is_sync_requested;
		private static Window					dispatch_window;
	}
}
