//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.window = window;
			
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);
			
			this.graphics = Epsitec.Common.Drawing.GraphicsFactory.NewGraphics ();
			
			this.ReallocatePixmap ();
		}
		
		internal void MakeFramelessWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ShowInTaskbar   = false;
		}
		
		internal void ResetWindow()
		{
			this.window = null;
		}
		
		
		internal void AnimateShow(Animation animation, Drawing.Rectangle bounds)
		{
			Drawing.Rectangle b1;
			Drawing.Rectangle b2;
			Drawing.Point o1;
			Drawing.Point o2;
			
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
					animator.SetValue (0.0, 1.0);
					animator.Start ();
					this.Show ();
					return;
				
				case Animation.FadeOut:
					this.is_frozen = true;
					this.IsLayered = true;
					this.Alpha = 1.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeOutTime);
					animator.SetCallback (new DoubleCallback (this.AnimateAlpha), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (1.0, 0.0);
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
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime);
					animator.SetCallback (new BoundsOffsetCallback (this.AnimateWindowBounds), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (0, b1, b2);
					animator.SetValue (1, o1, o2);
					animator.Start ();
					this.Show ();
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
			this.window.OnWindowAnimationEnded ();
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
		
		internal bool							IsFrozen
		{
			get { return this.is_frozen || (this.window.Root == null); }
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
		
		
		internal bool							FilterMouseMessages
		{
			get { return this.filter_mouse_messages; }
			set { this.filter_mouse_messages = value; }
		}
		
		
		internal Epsitec.Common.Widgets.Window	HostingWidgetWindow
		{
			get { return this.window; }
		}
		
		internal static bool					IsApplicationActive
		{
			get { return Window.is_app_active; }
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	Attention: il n'est pas permis de faire un Dispose si l'appelant provient d'une
				//	WndProc, car cela perturbe le bon acheminement des messages dans Windows. On
				//	pr�f�re donc remettre la destruction � plus tard si on d�tecte cette condition.
				
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
				
				if (this.window != null)
				{
					this.window.ResetWindow ();
					this.window.Dispose ();
				}
			}
			
			base.Dispose (disposing);
		}
		
		
		protected override void OnClosed(System.EventArgs e)
		{
			if (this.Focused)
			{
				//	Si la fen�tre avait le focus et qu'on la ferme, on aimerait bien que
				//	si elle avait une fen�tre "parent", alors ce soit le parent qui re�oive
				//	le focus � son tour. Ca para�t logique.
				
				System.Windows.Forms.Form form = this.Owner;
				
				if (form != null)
				{
					form.Activate ();
				}
			}
			
			this.window.OnWindowClosed ();
			
			base.OnClosed (e);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			
			if (this.prevent_close)
			{
				e.Cancel = true;
				
				//	Emp�che la fermeture de la fen�tre lorsque l'utilisateur clique sur le bouton de
				//	fermeture, et synth�tise un �v�nement clavier ALT + F4 � la place...
				
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
			
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseEnter, this, fake_event));
		}

		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseLeave, this, null));
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
				//	Rien � faire, car la taille correspond � la derni�re taille m�moris�e.
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
		}
		
		protected override void OnActivated(System.EventArgs e)
		{
			base.OnActivated (e);
			
			if (this.window != null)
			{
				this.window.OnWindowActivated ();
			}
		}
		
		protected override void OnDeactivate(System.EventArgs e)
		{
			base.OnDeactivate (e);
			
			if (this.window != null)
			{
				this.window.OnWindowDeactivated ();
			}
		}

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged (e);
			
			if (this.Visible)
			{
				this.window.OnWindowShown ();
			}
			else
			{
				this.window.OnWindowHidden ();
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
				
				this.window.Root.Size = new Drawing.Size (width, height);
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
			this.dirty_rectangle.MergeWith (rect);
			
			int x = (int) (rect.Left);
			int y = this.ClientSize.Height - (int) (rect.Top + 0.9999);
			int width  = (int) (rect.Width + 0.9999);
			int height = (int) (rect.Height + 0.9999);
			
			this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
		}
		
		internal void SynchronousRepaint()
		{
			this.Update ();
		}
		
		internal void SendQueueCommand()
		{
			Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_EXEC_CMD, System.IntPtr.Zero, System.IntPtr.Zero);
		}

		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
		{
			if (msg.Msg == Win32Const.WM_APP_DISPOSE)
			{
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth == 0);
				
				//	L'appelant avait tent� de nous d�truire alors qu'il �tait dans un WndProc,
				//	on re�oint maintenant la commande explicite (asynchrone) qui nous autorise
				//	� nous d�truire r�ellement.
				
				this.Dispose ();
				return;
			}
			
			if (msg.Msg == Win32Const.WM_APP_EXEC_CMD)
			{
				System.Diagnostics.Debug.Assert (this.wnd_proc_depth == 0);
				this.window.DispatchQueuedCommands ();
				return;
			}
			
			this.wnd_proc_depth++;
			
			try
			{
				if (this.WndProcActivation (ref msg))
				{
					return;
				}
				
				//	Tente d'unifier tous les �v�nements qui touchent au clavier, sans faire de traitement
				//	sp�cial pour les touches press�es en m�me temps que ALT. Mais si l'on veut que le menu
				//	syst�me continue � fonctionner (ALT + ESPACE), il faut laisser transiter les �v�nements
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
				
				//	Filtre les r�p�titions clavier des touches super-shift. Cela n'a, � mon avis, aucun
				//	sens, puisqu'une touche super-shift est soit enfonc�e, soit non enfonc�e...
				
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
					//	Si l'utilisateur clique dans la fen�tre, on veut recevoir l'�v�nement d'activation
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
			System.Windows.Forms.Message message;
			
			//	Top Level forms should keep their 'active' visual style as long as any top level
			//	form is active. This is implemented by faking WM_NCACTIVATE messages with the
			//	proper settings.
			
			//	The condition for a top level form to be represented with the 'inactive' visual
			//	style is that a modal dialog is being shown; and class CommonDialogs keeps track
			//	of this, which makes it the ideal fake Message provider.
			
			switch (msg.Msg)
			{
				case Win32Const.WM_ACTIVATE:
					break;
				
				case Win32Const.WM_ACTIVATEAPP:
					bool app_active = ((int) msg.WParam) != 0;
					if (Window.is_app_active != app_active)
					{
						Window.is_app_active = app_active;
						if (app_active)
						{
							this.window.OnApplicationActivated ();
						}
						else
						{
							this.window.OnApplicationDeactivated ();
						}
					}
					message = Window.CreateNCActivate (this, Window.is_app_active);
					base.WndProc (ref message);
					break;
				
				case Win32Const.WM_NCACTIVATE:
					if ((int)msg.WParam != 1)
					{
						message = Window.CreateNCActivate (this, true);
						base.WndProc (ref message);
						msg.Result = message.Result;
						return true;
					}
					break;
			}
			
			return false;
		}

		protected bool WndProcFiltering(ref System.Windows.Forms.Message msg)
		{
			Message message = Message.FromWndProcMessage (this, ref msg);
			
			if (this.filter_mouse_messages)
			{
				//	Si le filtre des messages souris est actif, on mange absolument tous
				//	les �v�nements relatifs � la souris, jusqu'� ce que tous les boutons
				//	aient �t� rel�ch�s.
				
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
				if (this.window.FilterMessage (message))
				{
					return true;
				}
				
				if (message.NonClient)
				{
					//	Les messages "non-client" ne sont pas achemin�s aux widgets normaux,
					//	car ils ne pr�sentent aucun int�r�t. Par contre, le filtre peut les
					//	voir.
					
					return false;
				}
				
				this.DispatchMessage (message);
				
				return true;
			}
			
			return false;
		}
		
		
		protected static System.Windows.Forms.Message CreateNCActivate(System.Windows.Forms.Form form, bool activate)
		{
			System.Windows.Forms.Message msg;
			msg = System.Windows.Forms.Message.Create (form.Handle, Win32Const.WM_NCACTIVATE, System.IntPtr.Zero, System.IntPtr.Zero);
			
			//	TODO: g�re le cas o� des fen�tres modales sont ouvertes... Cf. VirtualPen
			
			if (activate)
			{
				msg.WParam = (System.IntPtr)(1);
			}
			
			return msg;
		}
		
		
		protected void DispatchPaint(System.Drawing.Graphics win_graphics, System.Drawing.Rectangle win_clip_rect)
		{
			//	Ce que Windows appelle "Paint", nous l'appelons "Display". En effet, lorsque l'on re�oit un �v�nement
			//	de type WM_PAINT (PaintEvent), on doit simplement afficher le contenu de la fen�tre, sans reg�n�rer le
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
			
			if (this.dirty_rectangle.IsEmpty == false)
			{
				Drawing.Rectangle repaint = this.dirty_rectangle;
					
				this.dirty_rectangle = Drawing.Rectangle.Empty;
					
				this.graphics.ResetClippingRectangle ();
				this.graphics.SetClippingRectangle (repaint);
					
				this.window.Root.PaintHandler (this.graphics, repaint);
				
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
				using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
				{
					System.Drawing.Point  client = this.PointToScreen (new System.Drawing.Point (0, 0));
					System.Drawing.Point  offset = new System.Drawing.Point (client.X - this.Location.X, client.Y - this.Location.Y);
					
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage (bitmap))
					{
						System.Drawing.Rectangle clip = new System.Drawing.Rectangle (0, 0, this.ClientSize.Width, this.ClientSize.Height);
						Drawing.Pixmap pixmap = this.graphics.Pixmap;
						pixmap.Blend (graphics, offset, clip);
						paint_needed = ! Win32Api.UpdateLayeredWindow (this.Handle, bitmap, this.Bounds, this.alpha);
					}
				}
			}
			
			return paint_needed;
		}
		
		
		internal void DispatchMessage(Message message)
		{
			this.window.DispatchMessage (message);
		}
		
		
		
		
		private Epsitec.Common.Widgets.Window	window;
		
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
		private bool							prevent_close = false;
		private bool							filter_mouse_messages;
		private double							alpha = 1.0;
		private int								wnd_proc_depth;
		
		private static bool						is_app_active;
	}
}
