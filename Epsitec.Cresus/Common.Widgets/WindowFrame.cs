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
			
			this.graphics = new Epsitec.Common.Drawing.Graphics ();
			this.root     = new WindowRoot (this);
			
			this.root.Size = new Drawing.Size (this.ClientSize);
			this.root.Name = "Root";
			
			this.ReallocatePixmap ();
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
					}
					
					this.engaged_widget = new_engage;
					
					if (new_engage != null)
					{
						new_engage.SetEngaged (true);
					}
				}
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
			base.OnClosed (e);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing (e);
			e.Cancel = true;
			
			//	Empêche la fermeture de la fenêtre lorsque l'utilisateur clique sur le bouton de
			//	fermeture, et synthétise un événement clavier ALT + F4 à la place...
			
			System.Windows.Forms.Keys alt_f4 = System.Windows.Forms.Keys.F4 | System.Windows.Forms.Keys.Alt;
			System.Windows.Forms.KeyEventArgs fake_event = new System.Windows.Forms.KeyEventArgs (alt_f4);
			Message message = Message.FromKeyEvent (MessageType.KeyDown, fake_event);
			this.DispatchMessage (message);
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

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseDown, this, e));
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseUp, this, e));
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseMove, this, e));
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
			base.OnSizeChanged (e);
			this.ReallocatePixmap ();
		}
		
		
		protected virtual void ReallocatePixmap()
		{
			int width  = this.ClientSize.Width;
			int height = this.ClientSize.Height;
			
			this.graphics.SetPixmapSize (width, height);
			
			this.root.Size       = new Drawing.Size (width, height);
			this.dirty_rectangle = new Drawing.Rectangle (0, 0, width, height);
		}
		
		public virtual void MarkForRepaint(Drawing.Rectangle rect)
		{
			this.dirty_rectangle.MergeWith (rect);
			
			int x = (int) rect.Left;
			int y = this.ClientSize.Height - 1 - (int) rect.Top;
			int width  = (int) rect.Width;
			int height = (int) rect.Height;
			
			this.Invalidate (new System.Drawing.Rectangle (x, y, width, height));
		}

		
		protected override void WndProc(ref System.Windows.Forms.Message msg)
		{
			const int WM_KEYDOWN	= 0x0100;	const int WM_SYSKEYDOWN		= 0x0104;
			const int WM_KEYUP		= 0x0101;	const int WM_SYSKEYUP		= 0x0105;
			const int WM_CHAR		= 0x0102;	const int WM_SYSCHAR		= 0x0106;
			const int WM_DEADCHAR	= 0x0103;	const int WM_SYSDEADCHAR	= 0x0107;
			
			const int VK_SHIFT		= 0x0010;
			const int VK_CONTROL	= 0x0011;
			const int VK_MENU		= 0x0012;
			const int VK_SPACE		= 0x0020;
			
			//	Tente d'unifier tous les événements qui touchent au clavier, sans faire de traitement
			//	spécial pour les touches pressées en même temps que ALT. Mais si l'on veut que le menu
			//	système continue à fonctionner (ALT + ESPACE), il faut laisser transiter les événements
			//	clavier qui ne concernent que ALT ou ALT + ESPACE sans modification.
			
			int w_param = (int) msg.WParam;
			int l_param = (int) msg.LParam;
			
			if ((w_param != VK_SPACE) && (w_param != VK_MENU))
			{
				switch (msg.Msg)
				{
					case WM_SYSKEYDOWN:		msg.Msg = WM_KEYDOWN;	break;
					case WM_SYSKEYUP:		msg.Msg = WM_KEYUP;		break;
					case WM_SYSCHAR:		msg.Msg = WM_CHAR;		break;
					case WM_SYSDEADCHAR:	msg.Msg = WM_DEADCHAR;	break;
				}
			}
			
			//	Filtre les répétitions clavier des touches super-shift. Cela n'a, à mon avis, aucun
			//	sens, puisqu'une touche super-shift est soit enfoncée, soit non enfoncée...
			
			if ((msg.Msg == WM_KEYDOWN) ||
				(msg.Msg == WM_SYSKEYDOWN))
			{
				switch (w_param)
				{
					case VK_SHIFT:
					case VK_CONTROL:
					case VK_MENU:
						if ((l_param & 0x40000000) != 0)
						{
							return;
						}
						break;
				}
			}
			
			base.WndProc (ref msg);
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
			
			this.graphics.Pixmap.Paint (win_graphics, win_clip_rect);
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
						shortcut.KeyCode = message.KeyCode;
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
		
		
		protected WindowRoot			root;
		protected Drawing.Graphics		graphics;
		protected Drawing.Rectangle		dirty_rectangle;
		protected Widget				last_in_widget;
		protected Widget				capturing_widget;
		protected Widget				focused_widget;
		protected Widget				engaged_widget;
	}
}
