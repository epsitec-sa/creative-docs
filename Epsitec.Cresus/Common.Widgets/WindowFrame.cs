namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowFrame fait le lien avec les WinForms.
	/// </summary>
	public class WindowFrame : System.Windows.Forms.Form
	{
		public WindowFrame()
		{
			this.SetStyle (System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.Opaque, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.ResizeRedraw, true);
			this.SetStyle (System.Windows.Forms.ControlStyles.UserPaint, true);
			
			this.graphics = Epsitec.Common.Drawing.GraphicsFactory.NewGraphics ();
			this.root     = new WindowRoot (this);
			
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
		
		
		public WindowRoot				Root
		{
			get { return this.root; }
		}
		
		public Widget					CapturingWidget
		{
			get { return this.capturing_widget; }
		}
		
		public Widget					FocusedWidget
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
		
		public Widget					EngagedWidget
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
							this.winforms_timer.Interval = (int) (1000 * WindowFrame.InitialKeyboardDelay);
							this.tick_count = 0;
						}
					}
				}
			}
		}
		
		public bool						PreventAutoClose
		{
			get { return this.prevent_close; }
			set { this.prevent_close = value; }
		}
		
		public bool						IsLayered
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
					
					if (WindowFrame.SupportsLayeredWindows)
					{
						int ex_style = Win32Api.GetWindowExStyle (this.Handle);
						ex_style |= Win32Const.WS_EX_LAYERED;
						Win32Api.SetWindowExStyle (this.Handle, ex_style);
						this.is_layered = value;
					}
				}
			}
		}
		
		public double					Alpha
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
		
		public Drawing.Rectangle		WindowBounds
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
		
		public Drawing.Point			WindowLocation
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
		
		public Drawing.Size				WindowSize
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
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.graphics != null)
				{
					this.graphics.Dispose ();
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

		
		protected virtual void ReallocatePixmap()
		{
			int width  = this.ClientSize.Width;
			int height = this.ClientSize.Height;
			
			this.graphics.SetPixmapSize (width, height);
			this.graphics.Pixmap.Clear ();
			
			this.root.Size       = new Drawing.Size (width, height);
			this.dirty_rectangle = new Drawing.Rectangle (0, 0, width, height);
			
			this.UpdateLayeredWindow ();
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
		
		
		protected void HandleWinFormsTimerTick(object sender, System.EventArgs e)
		{
			if (this.engaged_widget != null)
			{
				this.tick_count++;
				
				//	Accélération progressive: première attente = délai avant répétition clavier,
				//	puis de 20% à 100% de la vitesse de répétition clavier...
				
				int max   = 10;
				int phase = System.Math.Min (this.tick_count, max);
				double t1 = WindowFrame.InitialKeyboardDelay;
				double t2 = WindowFrame.KeyboardRepeatPeriod;
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
		
		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
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
			
			if (this.WndProcFiltering (ref msg))
			{
				return;
			}
			
			base.WndProc (ref msg);
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
					message = WindowFrame.CreateNCActivate (this, ((int) msg.WParam) != 0);
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
			
			if (message != null)
			{
				if (WindowFrame.MessageFilter != null)
				{
					WindowFrame.MessageFilter (this, message);
					
					//	Si le message a été "absorbé" par le filtre, il ne faut en aucun
					//	cas le transmettre plus loin.
					
					if (message.Handled)
					{
						return true;
					}
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
			
			if (this.dirty_rectangle.IsEmpty == false)
			{
				Drawing.Rectangle repaint = this.dirty_rectangle;
				
				this.dirty_rectangle = Drawing.Rectangle.Empty;
				
				this.graphics.ResetClippingRectangle ();
				this.graphics.SetClippingRectangle (repaint);
				
				this.root.PaintHandler (this.graphics, repaint);
			}
			
			if (this.UpdateLayeredWindow ())
			{
				Drawing.Pixmap pixmap = this.graphics.Pixmap;
			
				if (pixmap != null)
				{
					pixmap.Paint (win_graphics, win_clip_rect);
				}
			}
		}
		
		protected virtual bool UpdateLayeredWindow()
		{
			bool paint_needed = true;
			
			if (this.is_layered)
			{
				if (this.dirty_rectangle.IsEmpty == false)
				{
					Drawing.Rectangle repaint = this.dirty_rectangle;
					
					this.dirty_rectangle = Drawing.Rectangle.Empty;
					
					this.graphics.ResetClippingRectangle ();
					this.graphics.SetClippingRectangle (repaint);
					
					this.root.PaintHandler (this.graphics, repaint);
				}
				
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
			
			return paint_needed;
		}
		
		public virtual void DispatchMessage(Message message)
		{
			if (message.IsMouseType)
			{
				//	C'est un message souris. Nous allons commencer par vérifier si tous les widgets
				//	encore marqués comme IsEntered contiennent effectivement encore la souris. Si non,
				//	on les retire de la liste en leur signalant qu'ils viennent de perdre la souris.
				
				Widget.UpdateEntered (message);
				
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

		
		public static double					InitialKeyboardDelay
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Keyboard"))
					{
						switch (System.Int32.Parse ((string) key.GetValue ("KeyboardDelay")))
						{
							case 0:	return 0.250;
							case 1: return 0.500;
							case 2: return 0.750;
							case 3: return 1.000;
						}
					}
				}
				catch
				{
				}
				
				return 0.5;
			}
		}
		
		public static double					KeyboardRepeatPeriod
		{
			get
			{
				try
				{
					using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey (@"Control Panel\Keyboard"))
					{
						int speed = System.Int32.Parse ((string) key.GetValue ("KeyboardSpeed")) + 2;
						return 1.0 / speed;
					}
				}
				catch
				{
				}
				
				return 0.1;
			}
		}
		
		public static bool						SupportsLayeredWindows
		{
			get
			{
				return System.Windows.Forms.OSFeature.Feature.GetVersionPresent (System.Windows.Forms.OSFeature.LayeredWindows) != null;
			}
		}
		
		
		
		public event System.EventHandler		WindowActivated;
		public event System.EventHandler		WindowDeactivated;
		
		public static event MessageHandler		MessageFilter;
		
		public new event System.EventHandler	Resize;
		public new event System.EventHandler	SizeChanged;
		
		
		protected WindowRoot					root;
		protected Drawing.Graphics				graphics;
		protected Drawing.Rectangle				dirty_rectangle;
		protected Drawing.Rectangle				window_bounds;
		protected System.Drawing.Rectangle		form_bounds;
		protected bool							form_bounds_set = false;
		protected bool							on_resize_event = false;
		protected Widget						last_in_widget;
		protected Widget						capturing_widget;
		protected Widget						focused_widget;
		protected Widget						engaged_widget;
		protected System.Windows.Forms.Timer	winforms_timer;
		protected int							tick_count;
		
		private bool							prevent_close;
		private bool							is_layered;
		private double							alpha = 1.0;
	}
}
