//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/10/2003

namespace Epsitec.Common.Widgets
{
	using BundleAttribute		= Epsitec.Common.Support.BundleAttribute;
	using CancelEventHandler	= System.ComponentModel.CancelEventHandler;
	
	/// <summary>
	/// La classe WindowFrame fait le lien avec les WinForms.
	/// </summary>
	public class WindowFrame : System.Windows.Forms.Form, Epsitec.Common.Support.IBundleSupport
	{
		protected delegate void BoundsOffsetCallback(Drawing.Rectangle bounds, Drawing.Point offset);
		protected delegate void AnimatorCallback(Animator animator);
		protected delegate void DoubleCallback(double value);
		
		public WindowFrame()
		{
			this.cmd_dispatcher = Support.CommandDispatcher.Default;
			
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);
			
			this.graphics = Epsitec.Common.Drawing.GraphicsFactory.NewGraphics ();
			this.root     = new WindowRoot (this);
			this.root.MinSizeChanged += new EventHandler (HandleRootMinSizeChanged);
			
			this.root.Size = new Drawing.Size (this.ClientSize);
			this.root.Name = "Root";
			
			this.winforms_timer = new System.Windows.Forms.Timer ();
			this.winforms_timer.Tick += new System.EventHandler(HandleWinFormsTimerTick);
			this.winforms_timer.Interval = 50;
			
			this.prevent_close = false;
			
			this.ReallocatePixmap ();
		}
		
		public void MakeFramelessWindow()
		{
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.ShowInTaskbar   = false;
		}
		
		public new void Show()
		{
			base.Show ();
		}
		
		public void AnimateShow(Animation animation)
		{
			this.AnimateShow (animation, this.WindowBounds);
		}
		
		public void AnimateShow(Animation animation, Drawing.Rectangle bounds)
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
					
					animator = new Animator (SystemInformation.MenuAnimationFadeInTime / 1000.0);
					animator.SetCallback (new DoubleCallback (this.AnimateAlpha), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (0.0, 1.0);
					animator.Start ();
					this.Show ();
					return;
				
				case Animation.FadeOut:
					this.is_frozen = true;
					this.IsLayered = true;
					this.Alpha = 1.0;
					
					animator = new Animator (SystemInformation.MenuAnimationFadeOutTime / 1000.0);
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
					
					animator = new Animator (SystemInformation.MenuAnimationRollTime / 1000.0);
					animator.SetCallback (new BoundsOffsetCallback (this.AnimateWindowBounds), new AnimatorCallback (this.AnimateCleanup));
					animator.SetValue (0, b1, b2);
					animator.SetValue (1, o1, o2);
					animator.Start ();
					this.Show ();
					break;
			}
		}
		
		
		
		protected virtual void AnimateWindowBounds(Drawing.Rectangle bounds, Drawing.Point offset)
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			this.WindowBounds = bounds;
			this.paint_offset = offset;
			this.Invalidate ();
		}
		
		protected virtual void AnimateAlpha(double alpha)
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			this.Alpha = alpha;
		}
		
		protected virtual void AnimateCleanup(Animator animator)
		{
			animator.Dispose ();
			
			if (this.IsDisposed)
			{
				return;
			}
			
			this.is_frozen = false;
			this.Invalidate ();
			
			if (this.WindowAnimationEnded != null)
			{
				this.WindowAnimationEnded (this);
			}
		}
		
		
		
		public WindowRoot					Root
		{
			get { return this.root; }
		}
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get { return this.cmd_dispatcher; }
			set { this.cmd_dispatcher = value; }
		}
		
		public Widget						CapturingWidget
		{
			get { return this.capturing_widget; }
		}
		
		public Widget						FocusedWidget
		{
			get { return this.focused_widget; }
			set
			{
				if (this.focused_widget != value)
				{
					Widget old_focus = this.focused_widget;
					Widget new_focus = value;
					
					this.focused_widget = null;
					
					if (old_focus != null)
					{
						old_focus.SetFocused (false);
					}
					
					this.focused_widget = new_focus;
					
					if (new_focus != null)
					{
						new_focus.SetFocused (true);
					}
				}
			}
		}
		
		public Widget						EngagedWidget
		{
			get { return this.engaged_widget; }
			set
			{
				if (this.engaged_widget != value)
				{
					Widget old_engage = this.engaged_widget;
					Widget new_engage = value;
					
					this.engaged_widget = null;
					
					if (old_engage != null)
					{
						old_engage.SetEngaged (false);
						this.winforms_timer.Enabled = false;
					}
					
					this.engaged_widget = new_engage;
					
					if (new_engage != null)
					{
						new_engage.SetEngaged (true);
						
						if (new_engage.AutoRepeatEngaged)
						{
							this.winforms_timer.Enabled = true;
							this.winforms_timer.Interval = (int) (1000 * SystemInformation.InitialKeyboardDelay);
							this.tick_count = 0;
						}
					}
				}
			}
		}
		
		public bool							PreventAutoClose
		{
			get { return this.prevent_close; }
			set { this.prevent_close = value; }
		}
		
		public bool							IsLayered
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
		
		public bool							IsFrozen
		{
			get { return this.is_frozen; }
		}
		
		public bool							IsMouseActivationEnabled
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
		
		public double						Alpha
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
		
									
		public Drawing.Rectangle			WindowBounds
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
		
		
		[Bundle ("pos")]	public Drawing.Point	WindowLocation
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
		
		[Bundle ("size")]	public Drawing.Size		WindowSize
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
		
		[Bundle ("text")]	public new string		Text
		{
			get { return TextLayout.ConvertToTaggedText (base.Text); }
			set { base.Text = TextLayout.ConvertToSimpleText (value); }
		}
		
		[Bundle ("name")]	public new string		Name
		{
			get { return base.Name; }
			set { base.Name = value; }
		}
		
		
		#region Interface IBundleSupport
		public virtual string				PublicClassName
		{
			get { return "Window"; }
		}
		
		public virtual void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			//	Il faut tricher un petit peu ici, car la classe WindowFrame ne fait pas
			//	partie de la hiérarchie dérivée de Widget. Cependant, l'utilisateur ne
			//	doit pas en avoir conscience. On laisse simplement "Root" gérer toute
			//	l'initialisation.
			
			this.Root.Name = this.Name;
			this.Root.RestoreFromBundle (bundler, bundle);
		}
		#endregion
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
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
				
				if (this.root != null)
				{
					this.root.Dispose ();
				}
				
				this.graphics = null;
				this.root = null;
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
			
			base.OnClosed (e);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			
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
			System.Diagnostics.Debug.Assert (this.Resize == null, "WindowFrame.Resize event may not be used!");
			System.Diagnostics.Debug.Assert (this.SizeChanged == null, "WindowFrame.SizeChanged event may not be used!");

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
		}
		
		protected override void OnActivated(System.EventArgs e)
		{
			base.OnActivated (e);
			
			if (this.WindowActivated != null)
			{
				this.WindowActivated (this, e);
			}
		}
		
		protected override void OnDeactivate(System.EventArgs e)
		{
			base.OnDeactivate (e);
			
			if (this.WindowDeactivated != null)
			{
				this.WindowDeactivated (this, e);
			}
		}

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged (e);
			
			if (this.Visible)
			{
				if (this.WindowShown != null)
				{
					this.WindowShown (this, e);
				}
			}
			else
			{
				if (this.WindowHidden != null)
				{
					this.WindowHidden (this, e);
				}
			}
		}

		
		protected virtual void ReallocatePixmap()
		{
			int width  = this.ClientSize.Width;
			int height = this.ClientSize.Height;
			
			if (this.is_frozen)
			{
				return;
			}
			
			if (this.graphics.SetPixmapSize (width, height))
			{
				this.graphics.Pixmap.Clear ();
				
				this.root.Size       = new Drawing.Size (width, height);
				this.dirty_rectangle = new Drawing.Rectangle (0, 0, width, height);
				
				this.UpdateLayeredWindow ();
			}
		}
		
		
		protected int MapToWinFormsX(double x)
		{
			return (int)(x + 0.5);
		}
		
		protected int MapToWinFormsY(double y)
		{
			return System.Windows.Forms.SystemInformation.VirtualScreen.Height - (int)(y + 0.5);
		}
		
		protected int MapToWinFormsWidth(double width)
		{
			return (int)(width + 0.5);
		}
		
		protected int MapToWinFormsHeight(double height)
		{
			return (int)(height + 0.5);
		}
		
		protected double MapFromWinFormsX(int x)
		{
			return x;
		}
		
		protected double MapFromWinFormsY(int y)
		{
			return System.Windows.Forms.SystemInformation.VirtualScreen.Height - y;
		}
		
		protected double MapFromWinFormsWidth(int width)
		{
			return width;
		}
		
		protected double MapFromWinFormsHeight(int height)
		{
			return height;
		}
		
		
		public virtual Drawing.Point MapWindowToScreen(Drawing.Point point)
		{
			int x = (int)(point.X + 0.5);
			int y = this.ClientSize.Height-1 - (int)(point.Y + 0.5);
			
			System.Drawing.Point pt = this.PointToScreen (new System.Drawing.Point (x, y));
			
			double xx = this.MapFromWinFormsX (pt.X);
			double yy = this.MapFromWinFormsY (pt.Y)-1;
			
			return new Drawing.Point (xx, yy);
		}
		
		public virtual Drawing.Point MapScreenToWindow(Drawing.Point point)
		{
			int x = this.MapToWinFormsX (point.X);
			int y = this.MapToWinFormsY (point.Y)-1;
			
			System.Drawing.Point pt = this.PointToClient (new System.Drawing.Point (x, y));
			
			double xx = pt.X;
			double yy = this.ClientSize.Height-1 - pt.Y;
			
			return new Drawing.Point (xx, yy);
		}
		
		
		public virtual void MarkForRepaint()
		{
			this.MarkForRepaint (new Drawing.Rectangle (0, 0, this.ClientSize.Width, this.ClientSize.Height));
		}
		
		public virtual void MarkForRepaint(Drawing.Rectangle rect)
		{
			this.dirty_rectangle.MergeWith (rect);
			
			int x = (int) (rect.Left);
			int y = this.ClientSize.Height - (int) (rect.Top + 0.9999);
			int width  = (int) (rect.Width + 0.9999);
			int height = (int) (rect.Height + 0.9999);
			
			this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
		}
		
		public virtual void SynchronousRepaint()
		{
			this.Update ();
		}
		
		
		public virtual void QueueCommand(Widget source)
		{
			this.command_queue.Enqueue (source);
			
			if (this.command_queue.Count == 1)
			{
				Win32Api.PostMessage (this.Handle, Win32Const.WM_APP_EXEC_CMD, System.IntPtr.Zero, System.IntPtr.Zero);
			}
		}
		
		protected virtual void DispatchCommands()
		{
			while (this.command_queue.Count > 0)
			{
				Widget widget = this.command_queue.Dequeue () as Widget;
				string name   = widget.CommandName;
				
				this.cmd_dispatcher.Dispatch (name, widget);
			}
		}
		
		protected virtual void HandleWinFormsTimerTick(object sender, System.EventArgs e)
		{
			if (this.engaged_widget != null)
			{
				this.tick_count++;
				
				//	Accélération progressive: première attente = délai avant répétition clavier,
				//	puis de 20% à 100% de la vitesse de répétition clavier...
				
				int max   = 10;
				int phase = System.Math.Min (this.tick_count, max);
				double t1 = SystemInformation.InitialKeyboardDelay;
				double t2 = SystemInformation.KeyboardRepeatPeriod;
				double t3 = System.Math.Min (t1, t2 * 5);
				
				this.winforms_timer.Interval = (int) ((t3*(max-phase) + t2*phase) * 1000 / max);
				
				if (this.engaged_widget.IsEngaged)
				{
					this.engaged_widget.FireStillEngaged ();
					return;
				}
			}
			
			this.winforms_timer.Enabled = false;
		}
		
		protected virtual void HandleRootMinSizeChanged(object sender)
		{
			int width  = (int) (this.root.MinSize.Width + 0.5);
			int height = (int) (this.root.MinSize.Height + 0.5);
			
			width  += this.Size.Width  - this.ClientSize.Width;
			height += this.Size.Height - this.ClientSize.Height;
			
			this.MinimumSize = new System.Drawing.Size (width, height);
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
				this.DispatchCommands ();
				return;
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
		
		protected virtual bool WndProcActivation(ref System.Windows.Forms.Message msg)
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
					if (WindowFrame.is_app_active != app_active)
					{
						WindowFrame.is_app_active = app_active;
						if (app_active)
						{
							if (WindowFrame.ApplicationActivated != null)
							{
								WindowFrame.ApplicationActivated (this);
							}
						}
						else
						{
							if (WindowFrame.ApplicationDeactivated != null)
							{
								WindowFrame.ApplicationDeactivated (this);
							}
						}
					}
					message = WindowFrame.CreateNCActivate (this, WindowFrame.is_app_active);
					base.WndProc (ref message);
					break;
				
				case Win32Const.WM_NCACTIVATE:
					if ((int)msg.WParam != 1)
					{
						message = WindowFrame.CreateNCActivate (this, true);
						base.WndProc (ref message);
						msg.Result = message.Result;
						return true;
					}
					break;
			}
			
			return false;
		}

		protected virtual bool WndProcFiltering(ref System.Windows.Forms.Message msg)
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
				if (WindowFrame.MessageFilter != null)
				{
					WindowFrame.MessageFilter (this, message);
					
					//	Si le message a été "absorbé" par le filtre, il ne faut en aucun
					//	cas le transmettre plus loin.
					
					if (message.Handled)
					{
						if (message.Swallowed)
						{
							//	Le message a été mangé. Il faut donc aussi manger le message
							//	correspondant si les messages viennent par paire.
							
							switch (message.Type)
							{
								case MessageType.MouseDown:
									this.filter_mouse_messages = true;
									break;
								
								case MessageType.KeyDown:
									//	TODO: prend note qu'il faut manger l'événement
									break;
							}
						}
						
						return true;
					}
				}
				
				if (message.NonClient)
				{
					System.Diagnostics.Debug.WriteLine ("NonClient Message: " + message.ToString ());
					
					//	Les messages "non-client" ne sont pas acheminés aux widgets normaux,
					//	car ils ne présentent aucun intérêt. Par contre, le filtre peut les
					//	voir.
					
					return false;
				}
				
				this.DispatchMessage (message);
				
				return false;
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
		
		
		protected virtual void DispatchPaint(System.Drawing.Graphics win_graphics, System.Drawing.Rectangle win_clip_rect)
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
		
		protected virtual bool RefreshGraphics()
		{
			if (this.is_frozen)
			{
				return false;
			}
			
			if (this.dirty_rectangle.IsEmpty == false)
			{
				Drawing.Rectangle repaint = this.dirty_rectangle;
					
				this.dirty_rectangle = Drawing.Rectangle.Empty;
					
				this.graphics.ResetClippingRectangle ();
				this.graphics.SetClippingRectangle (repaint);
					
				this.root.PaintHandler (this.graphics, repaint);
				
				return true;
			}
			
			return false;
		}
		
		protected virtual bool UpdateLayeredWindow()
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
		
		
		public virtual void DispatchMessage(Message message)
		{
			if (message.IsMouseType)
			{
				//	C'est un message souris. Nous allons commencer par vérifier si tous les widgets
				//	encore marqués comme IsEntered contiennent effectivement encore la souris. Si non,
				//	on les retire de la liste en leur signalant qu'ils viennent de perdre la souris.
				
				Widget.UpdateEntered (this, message);
				
				this.last_in_widget = this.DetectWidget (message.Cursor);
			}
			
			message.InWidget = this.last_in_widget;
			message.Consumer = null;
			
			if (this.capturing_widget == null)
			{
				//	La capture des événements souris n'est pas active. Si un widget a le focus, il va
				//	recevoir les événements clavier en priorité (message.FilterOnlyFocused = true).
				//	Dans les autres cas, les événements sont simplement acheminés de widget en widget,
				//	en utilisant une approche en profondeur d'abord.
				
				this.root.MessageHandler (message, message.Cursor);
				this.Capture = false;
			}
			else
			{
				message.FilterNoChildren = true;
				message.Captured         = true;
				
				this.capturing_widget.MessageHandler (message);
			}
			
			this.PostProcessMessage (message);
		}
		
		public virtual void PostProcessMessage(Message message)
		{
			Widget consumer = message.Consumer;
			
			if (message.Type == MessageType.KeyUp)
			{
				if (this.EngagedWidget != null)
				{
					this.EngagedWidget.SimulateReleased ();
					this.EngagedWidget.SimulateClicked ();
					this.EngagedWidget = null;
				}
			}
			
			if (consumer != null)
			{
				switch (message.Type)
				{
					case MessageType.MouseDown:
						if (consumer.AutoCapture)
						{
							this.capturing_widget = consumer;
							this.Capture = true;
						}
						else
						{
							this.Capture = false;
						}
						
						if ((consumer.AutoFocus) &&
							(consumer.IsFocused == false) &&
							(consumer.CanFocus))
						{
							this.FocusedWidget = consumer;
						}
						
						if (message.IsLeftButton)
						{
							if ((consumer.AutoEngage) &&
								(consumer.IsEngaged == false) &&
								(consumer.CanEngage))
							{
								this.EngagedWidget = consumer;
							}
						}
						break;
					
					case MessageType.MouseUp:
						this.capturing_widget = null;
						this.Capture = false;
						
						if (message.IsLeftButton)
						{
							if ((consumer.AutoToggle) &&
								(consumer.IsEnabled) &&
								(consumer.IsEntered) &&
								(!consumer.IsFrozen))
							{
								//	Change l'état du bouton...
								
								consumer.Toggle ();
							}
							
							this.EngagedWidget = null;
						}
						break;
					
					case MessageType.MouseLeave:
						if (consumer.IsEngaged)
						{
							this.EngagedWidget = null;
						}
						break;
					
					case MessageType.MouseEnter:
						if ((Message.State.IsLeftButton) &&
							(consumer.AutoEngage) &&
							(consumer.IsEngaged == false) &&
							(consumer.CanEngage))
						{
							this.EngagedWidget = consumer;
						}
						break;
				}
			}
			else
			{
				Shortcut shortcut = null;
				
				if (message.Type == MessageType.KeyDown)
				{
					//	Gère les raccourcis clavier générés avec la touche Alt. Ils ne génèrent pas d'événement
					//	KeyPress
				
					int key_code = message.KeyCode & 0x0000ffff;
				
					if ((key_code != 0) && (message.IsAltPressed))
					{
						shortcut = new Shortcut ();
						shortcut.KeyCode = message.KeyCode | (int) (System.Windows.Forms.Keys.Alt);
					}
				}
				else if (message.Type == MessageType.KeyPress)
				{
					shortcut = new Shortcut ();
					shortcut.KeyCode = message.KeyCode;
				}
				
				if (shortcut != null)
				{
					message.Handled = this.root.ShortcutHandler (shortcut);
				}
			}
		}
		
		protected virtual Widget DetectWidget(Drawing.Point pos)
		{
			Widget child = this.root.FindChild (pos);
			return (child == null) ? this.root : child;
		}

		
		
		public static bool						IsApplicationActive
		{
			get { return WindowFrame.is_app_active; }
		}
		
		
		
		public event System.EventHandler		WindowActivated;
		public event System.EventHandler		WindowDeactivated;
		public event System.EventHandler		WindowShown;
		public event System.EventHandler		WindowHidden;
		public event EventHandler				WindowAnimationEnded;
		
		public static event MessageHandler		MessageFilter;
		public static event EventHandler		ApplicationActivated;
		public static event EventHandler		ApplicationDeactivated;
		
		public new event System.EventHandler	Resize;
		public new event System.EventHandler	SizeChanged;
		
		
		protected Support.CommandDispatcher		cmd_dispatcher;
		protected WindowRoot					root;
		protected Drawing.Graphics				graphics;
		protected Drawing.Rectangle				dirty_rectangle;
		protected Drawing.Rectangle				window_bounds;
		protected Drawing.Point					paint_offset;
		protected System.Drawing.Rectangle		form_bounds;
		protected bool							form_bounds_set = false;
		protected bool							on_resize_event = false;
		protected Widget						last_in_widget;
		protected Widget						capturing_widget;
		protected Widget						focused_widget;
		protected Widget						engaged_widget;
		protected System.Windows.Forms.Timer	winforms_timer;
		
		protected int							tick_count;
		
		protected bool							prevent_close;
		protected bool							is_layered;
		protected bool							is_frozen;
		protected bool							is_no_activate;
		protected bool							filter_mouse_messages;
		protected double						alpha = 1.0;
		protected int							wnd_proc_depth;
		
		protected static bool					is_app_active;
		protected System.Collections.Queue		command_queue = new System.Collections.Queue ();
	}
}
