namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Window fait le lien avec les WinForms.
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
			this.root     = new WindowRoot ();
			
			this.root.Size = this.ClientSize;
			this.root.Name = "Root";
		}
		
		
		public WindowRoot					Root
		{
			get { return this.root; }
		}
		

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
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
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseDown, e));
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseUp, e));
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseMove, e));
		}

		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseWheel, e));
		}

		protected override void OnMouseEnter(System.EventArgs e)
		{
			base.OnMouseEnter (e);
			
			System.Drawing.Point point = this.PointToClient (System.Windows.Forms.Control.MousePosition);
			System.Windows.Forms.MouseEventArgs fake_event = new System.Windows.Forms.MouseEventArgs (System.Windows.Forms.MouseButtons.None, 0, point.X, point.Y, 0);
			
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseEnter, fake_event));
		}

		protected override void OnMouseLeave(System.EventArgs e)
		{
			base.OnMouseLeave (e);
			this.DispatchMessage (Message.FromMouseEvent (MessageType.MouseLeave, null));
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
			this.root.Size = this.ClientSize;
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
			System.IntPtr hdc = win_graphics.GetHdc ();
			
			try
			{
				this.graphics.AttachHandle (hdc);
				System.Drawing.RectangleF clip_rect = new System.Drawing.RectangleF (win_clip_rect.X, win_clip_rect.Y, win_clip_rect.Width, win_clip_rect.Height);
				this.root.PaintHandler (this.graphics, clip_rect);
			}
			finally
			{
				this.graphics.DetachHandle ();
				win_graphics.ReleaseHdc (hdc);
			}
		}
		
		protected virtual void DispatchMessage(Message message)
		{
			this.root.MessageHandler (message, message.Cursor);
			
			if (message.Handled == false)
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
		
		
		protected WindowRoot			root;
		protected Drawing.Graphics		graphics;
	}
}
