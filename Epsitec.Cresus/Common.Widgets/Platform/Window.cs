//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/11/2003

namespace Epsitec.Common.Widgets.Platform
{
	using CancelEventHandler	= System.ComponentModel.CancelEventHandler;
	
	/// <summary>
	/// La classe Platform.Window fait le lien avec les WinForms.
	/// </summary>
	internal class Window : System.Windows.Forms.Form
	{
		private delegate void BoundsOffsetCallback(Drawing.Rectangle bounds, Drawing.Point offset);
		private delegate void AnimatorCallback(Animator animator);
		private delegate void DoubleCallback(double value);
		
		internal Window(Epsitec.Common.Widgets.Window window)
		{
			this.widget_window = window;
			
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);
			
			this.graphics = new Epsitec.Common.Drawing.Graphics ();
			
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
					this.Show ();
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
					this.Show ();
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
				
				default:
					this.Show ();
					return;
			}
			
			switch (animation)
			{
				case Animation.RollDown:
				case Animation.RollUp:
				case Animation.RollRight:
				case Animation.RollLeft:
					this.is_frozen = true;
					this.WindowBounds = b1;
					this.UpdateLayeredWindow ();
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback (new BoundsOffsetCallback (this.AnimateWindowBounds), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (0, b1, b2);
					animator.SetValue (1, o1, o2);
					animator.Start ();
					this.Show ();
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
			
			this.is_frozen = false;
			this.Invalidate ();
			this.widget_window.OnWindowAnimationEnded ();
		}
		
		
		
		internal bool							PreventAutoClose
		{
			get { return this.prevent_close; }
			set { this.prevent_close = value; }
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
						ex_style |= Win32Const.WS_EX_LAYERED;
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
				System.Windows.Forms.Form active = System.Windows.Forms.Form.ActiveForm;
				return (active == this);
			}
		}
		
		internal bool							IsFrozen
		{
			get { return this.is_frozen || (this.widget_window.Root == null); }
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
		
		
		internal Drawing.Rectangle				WindowBounds
		{
			get
			{
				double ox = this.MapFromWinFormsX (this.Left);
				double oy = this.MapFromWinFormsY (this.Bottom);
				double dx = this.MapFromWinFormsWidth (this.Width);
				double dy = this.MapFromWinFormsHeight (this.Height);
				
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
					
					this.Bounds        = this.form_bounds;
					this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
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
				this.WindowBounds = new Drawing.Rectangle (value, this.WindowSize);
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
				this.WindowBounds = new Drawing.Rectangle (this.WindowLocation, value);
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
						this.widget_window.ResetWindow ();
						this.widget_window.Dispose ();
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
			this.widget_window.OnWindowFocused ();
		}
		
		protected override void OnLostFocus(System.EventArgs e)
		{
			base.OnLostFocus (e);
			this.widget_window.OnWindowDefocused ();
		}


		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			
			if (this.forced_close)
			{
				return;
			}
			
			if (this.prevent_close)
			{
				e.Cancel = true;
				
				//	Empêche la fermeture de la fenêtre lorsque l'utilisateur clique sur le bouton de
				//	fermeture, et synthétise un événement clavier ALT + F4 à la place...
				
				System.Windows.Forms.Keys alt_f4 = System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt;
				System.Windows.Forms.KeyEventArgs fake_event = new System.Windows.Forms.KeyEventArgs (alt_f4);
				Message message = Message.FromKeyEvent (MessageType.KeyDown, fake_event);
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
			base.OnPaint (e);
			this.DispatchPaint (e.Graphics, e.ClipRectangle);
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("OnPaintBackground called");
			base.OnPaintBackground (e);
			this.DispatchPaint (e.Graphics, e.ClipRectangle);
		}

		protected override void OnResize(System.EventArgs e)
		{
			base.OnResize (e);
		}

		protected override void OnSizeChanged(System.EventArgs e)
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
				this.widget_window.OnWindowShown ();
			}
			else
			{
				this.widget_window.OnWindowHidden ();
			}
		}

		
		protected void ReallocatePixmap()
		{
			int width  = this.ClientSize.Width;
			int height = this.ClientSize.Height;
			
			if (this.IsFrozen)
			{
				return;
			}
			
			if (this.graphics.SetPixmapSize (width, height))
			{
				this.graphics.Pixmap.Clear ();
				
				this.widget_window.Root.Size = new Drawing.Size (width, height);
				this.dirty_rectangle  = new Drawing.Rectangle (0, 0, width, height);
				
				this.UpdateLayeredWindow ();
			}
		}

		
		internal int MapToWinFormsX(double x)
		{
			return (int) System.Math.Floor (x + 0.5);
		}
		
		internal int MapToWinFormsY(double y)
		{
			return System.Windows.Forms.SystemInformation.VirtualScreen.Height - (int) System.Math.Floor (y + 0.5);
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
			return System.Windows.Forms.SystemInformation.VirtualScreen.Height - y;
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
			
			int top    = (int) (rect.Top);
			int bottom = (int) (rect.Bottom);
			
			int width  = (int) (rect.Width);
			int height = top - bottom + 1;
			int x      = (int) (rect.Left);
			int y      = this.ClientSize.Height - top;
			
			if (this.is_layered)
			{
				this.UpdateLayeredWindow ();
			}
			else
			{
				this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
			}
		}
		
		internal void SynchronousRepaint()
		{
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

		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
		{
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
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth == 0);
				this.widget_window.DispatchQueuedCommands ();
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
				
				if (this.WndProcFiltering (ref msg))
				{
					return;
				}
				
				base.WndProc (ref msg);
			}
			
			finally
			{
				System.Diagnostics.Debug.Assert (this.IsDisposed == false);
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth > 0);
				this.wnd_proc_depth--;
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
					
					this.FindRootOwner ().FakeActivateOwned (active);
					
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
			
			if (this.filter_mouse_messages)
			{
				//	Si le filtre des messages souris est actif, on mange absolument tous
				//	les événements relatifs à la souris, jusqu'à ce que tous les boutons
				//	aient été relâchés.
				
				if (Message.IsMouseMsg (msg))
				{
					if (Message.State.Buttons == Widgets.MouseButtons.None)
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
						if (message.Type != MessageType.KeyDown)
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
				
				Widgets.Window  w_window = Message.State.LastWindow;
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
			
			if (this.UpdateLayeredWindow ())
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
			if (this.IsFrozen)
			{
				return false;
			}
			
			if (this.dirty_rectangle.IsValid)
			{
				Drawing.Rectangle repaint = this.dirty_rectangle;
				
				this.dirty_rectangle = Drawing.Rectangle.Empty;
				
				this.widget_window.RefreshGraphics (this.graphics, repaint);
				
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
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
				
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
					
					paint_needed = ! Win32Api.UpdateLayeredWindow (this.Handle, bitmap, this.Bounds, this.alpha);
				}
			}
			
			return paint_needed;
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
			if (this.IsMouseActivationEnabled)
			{
				this.Show ();
			}
			else
			{
				Win32Api.ShowWindow (this.Handle, Win32Const.SW_SHOWNA);
			}
		}
		
		
		private bool							widget_window_disposed;
		private Epsitec.Common.Widgets.Window	widget_window;
		
		private Drawing.Graphics				graphics;
		private Drawing.Rectangle				dirty_rectangle;
		private Drawing.Rectangle				window_bounds;
		private Drawing.Point					paint_offset;
		private System.Drawing.Rectangle		form_bounds;
		private bool							form_bounds_set = false;
		private bool							on_resize_event = false;
		
		private bool							is_layered;
		private bool							is_frozen;
		private bool							is_no_activate;
		private bool							is_tool_window;
		
		private bool							has_active_frame;
		
		private bool							prevent_close;
		private bool							forced_close;
		private bool							filter_mouse_messages;
		private bool							filter_key_messages;
		private double							alpha = 1.0;
		private int								wnd_proc_depth;
		
		private static bool						is_app_active;
	}
}
