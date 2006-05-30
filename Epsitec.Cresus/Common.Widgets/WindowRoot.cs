//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot implémente le fond de chaque fenêtre. L'utilisateur obtient
	/// en général une instance de WindowRoot en appelant Window.Root.
	/// </summary>
	public class WindowRoot : AbstractGroup
	{
		protected WindowRoot()
		{
			this.WindowType   = WindowType.Document;
			this.WindowStyles = WindowStyles.CanResize | WindowStyles.HasCloseButton;
			
			this.InternalState |= InternalState.PossibleContainer;
			this.AutoDoubleClick = true;
		}
		
		
		public WindowRoot(Window window) : this ()
		{
			this.window       = window;
			this.is_ready     = true;
		}
		
		
		public override Window					Window
		{
			get
			{
				return this.window;
			}
		}
		
		public WindowStyles						WindowStyles
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
					
					if (this.window != null)
					{
						this.window.WindowStyles = value;
					}
					
					this.OnWindowStylesChanged ();
				}
			}
		}
		
		public WindowType						WindowType
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
					
					if (this.window != null)
					{
						this.window.WindowType = value;
					}
					
					this.OnWindowTypeChanged ();
				}
			}
		}
		
		
		public bool DoesVisualContainKeyboardFocus(Visual visual)
		{
			//	Retourne true si le visual passé en entrée contient le focus,
			//	ou qu'un de ses enfants contient le focus.

			this.RefreshFocusChain ();
			
			return this.focus_chain.Contains (visual);
		}
		
		public override void MessageHandler(Message message, Drawing.Point pos)
		{
			message.WindowRoot = this;
			base.MessageHandler (message, pos);
		}

		
		public override void InvalidateRectangle(Drawing.Rectangle rect, bool sync)
		{
			System.Diagnostics.Debug.Assert (this.Parent == null);
			
			if (this.window != null)
			{
				if (sync)
				{
					this.window.SynchronousRepaint ();
					this.window.MarkForRepaint (rect);
					this.window.SynchronousRepaint ();
				}
				else
				{
					this.window.MarkForRepaint (rect);
				}
			}
		}
		
		public override Epsitec.Common.Drawing.Size GetBestFitSize()
		{
			Widget[] children = this.Children.Widgets;
			
			if (children.Length == 1)
			{
				return children[0].GetBestFitSize ();
			}
			else
			{
				return this.PreferredSize;
			}
		}

		internal void RefreshFocusChain()
		{
			//	Si la chaîne des widgets décrivant les widgets contenant le
			//	focus n'existe pas, on la construit

			if (this.focus_chain.Count == 0)
			{
				Widget widget = this.window.FocusedWidget;

				while (widget != null)
				{
					this.focus_chain.Add (widget);
					widget = widget.Parent;
				}
			}
		}
		
		internal void ClearFocusChain()
		{
			this.focus_chain.Clear ();
		}
		
		internal override void SetBounds(Drawing.Rectangle value)
		{
			double dx = value.Width;
			double dy = value.Height;
			
			if (this.window != null)
			{
				this.window.ClientSize = new Drawing.Size (dx, dy);
			}
			
			base.SetBounds (value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.window = null;
			}
			
			base.Dispose (disposing);
		}

		protected override bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			if (base.ShortcutHandler (shortcut, execute_focused))
			{
				return true;
			}
			
			if (this.InternalShortcutHandler (shortcut, execute_focused))
			{
				return true;
			}
			
			return false;
		}
		
		protected virtual bool InternalShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			if (this.window != null)
			{
				//	Le raccourci clavier n'a pas été consommé. Il faut voir si le raccourci clavier
				//	est attaché à une commande globale.
				
				System.Diagnostics.Debug.WriteLine (shortcut.ToString ());
				
				Widget focused = this.window.FocusedWidget;
				bool   execute = true;
				
				if ((focused != null) &&
					(focused.AutoRadio) &&
					(shortcut.IsAltPressed == false) &&
					(shortcut.IsControlPressed == false) &&
					(shortcut.IsShiftPressed == false))
				{
					switch (shortcut.KeyCodeOnly)
					{
						case KeyCode.ArrowLeft:
						case KeyCode.ArrowRight:
						case KeyCode.ArrowUp:
						case KeyCode.ArrowDown:
							execute = false;
							break;
					}
				}
				
				if (execute)
				{
					Widget widget = focused == null ? this : focused;
					Window window = this.window;

					Command command = Widgets.Command.Find (shortcut);
					
					if (command != null)
					{
						window.QueueCommand (widget, command.Name);
						return true;
					}
					
					if ((shortcut.KeyCodeOnly == KeyCode.FuncF4) &&
						(shortcut.IsAltPressed) &&
						(shortcut.IsControlPressed == false) &&
						(shortcut.IsShiftPressed == false))
					{
						window.QueueCommand (this, "Quit" + this.Window.Name);
						return true;
					}
				}
				
				if (focused == null)
				{
					return false;
				}
				
				TabNavigationMode mode = TabNavigationMode.Passive;
				TabNavigationDir  dir  = TabNavigationDir.None;
				
				switch (shortcut.KeyCodeOnly)
				{
					case KeyCode.Tab:
						mode = TabNavigationMode.ActivateOnTab;
						dir  = Message.CurrentState.IsShiftPressed ? TabNavigationDir.Backwards : TabNavigationDir.Forwards;
						break;
					
					case KeyCode.ArrowLeft:
						mode = TabNavigationMode.ActivateOnCursorX;
						dir  = TabNavigationDir.Backwards;
						break;
					
					case KeyCode.ArrowRight:
						mode = TabNavigationMode.ActivateOnCursorX;
						dir  = TabNavigationDir.Forwards;
						break;
					
					case KeyCode.ArrowUp:
						mode = TabNavigationMode.ActivateOnCursorY;
						dir  = TabNavigationDir.Backwards;
						break;
					
					case KeyCode.ArrowDown:
						mode = TabNavigationMode.ActivateOnCursorY;
						dir  = TabNavigationDir.Forwards;
						break;
					
					case KeyCode.Return:
						if (window != null)
						{
							if (window.RestoreLogicalFocus ())
							{
								return true;
							}
						}
						break;
					
					case KeyCode.Escape:
						if (window != null)
						{
							if (window.RestoreLogicalFocus ())
							{
								return true;
							}
						}
						break;
				}
				
				if ((mode != TabNavigationMode.Passive) &&
					(dir != TabNavigationDir.None))
				{
					//	Navigue dans la hiérarchie...
					
					Widget find = focused.FindTabWidget (dir, mode);
					
					if (find != null)
					{
						if (find != focused)
						{
							Widget focus;
							
							if (focused.InternalAboutToLoseFocus (dir, mode) &&
								find.InternalAboutToGetFocus (dir, mode, out focus))
							{
								focus.SetFocused (true);
							}
						}
					}
				}
			}
				
			return false;
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			if (this.window != null)
			{
				this.window.Text = this.Text;
			}
		}
		
		protected override void OnNameChanged()
		{
			base.OnNameChanged ();
			
			if (this.window != null)
			{
				this.window.Name = this.Name;
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			if (this.is_ready == false)
			{
				return;
			}

			double dx = this.Client.Size.Width;
			double dy = this.Client.Size.Height;
			
			double x1 = System.Math.Max (clip_rect.Left, 0);
			double y1 = System.Math.Max (clip_rect.Bottom, 0);
			double x2 = System.Math.Min (clip_rect.Right, dx);
			double y2 = System.Math.Min (clip_rect.Top, dy);
			
			if (this.BackColor.IsValid)
			{
				if (this.BackColor.A != 1.0)
				{
					graphics.Pixmap.Erase (new System.Drawing.Rectangle ((int) x1, (int) y1, (int) x2 - (int) x1, (int) y2 - (int) y1));
				}
				if (this.BackColor.A > 0.0)
				{
					graphics.SolidRenderer.Color = this.BackColor;
					graphics.AddFilledRectangle (x1, y1, x2-x1, y2-y1);
					graphics.RenderSolid ();
				}
			}
			else
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, y1, x2-x1, y2-y1);
				adorner.PaintWindowBackground(graphics, this.Client.Bounds, rect, WidgetPaintState.None);
			}
		}
		
		
		protected virtual  void OnWindowStylesChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("WindowStylesChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual  void OnWindowTypeChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("WindowTypeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		internal void NotifyAdornerChanged()
		{
			this.HandleAdornerChanged ();
		}
		
		internal void NotifyCultureChanged()
		{
			this.HandleCultureChanged ();
		}

		internal void NotifyWindowIsVisibleChanged()
		{
			//	Copie l'état de visibilité de la fenêtre de manière à ce que
			//	notre propriété IsVisible soit toujours synchronisée avec la
			//	fenêtre :
			
			this.SetValue (Visual.IsVisibleProperty, this.window.IsVisible);
		}

		internal void NotifyWindowSizeChanged(double width, double height)
		{
			this.SetManualBounds (new Drawing.Rectangle (0, 0, width, height));
			
			Layouts.LayoutContext.AddToArrangeQueue (this);
		}
		
		internal override void SetDirtyLayoutFlag()
		{
		}
		
		
		public event EventHandler					WindowStylesChanged
		{
			add
			{
				this.AddUserEventHandler("WindowStylesChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("WindowStylesChanged", value);
			}
		}

		public event EventHandler					WindowTypeChanged
		{
			add
			{
				this.AddUserEventHandler("WindowTypeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("WindowTypeChanged", value);
			}
		}

		
		protected WindowStyles						window_styles;
		protected WindowType						window_type;
		protected Window							window;
		protected bool								is_ready;
		protected List<Visual>						focus_chain = new List<Visual> ();

	}
}
