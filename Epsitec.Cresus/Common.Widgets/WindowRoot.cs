//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute   = Support.BundleAttribute;
	using CommandDispatcher = Support.CommandDispatcher;
	
	/// <summary>
	/// La classe WindowRoot impl�mente le fond de chaque fen�tre. L'utilisateur obtient
	/// en g�n�ral une instance de WindowRoot en appelant Window.Root.
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
		
		public override CommandDispatcher		CommandDispatcher
		{
			get
			{
				if (this.window != null)
				{
					return this.window.CommandDispatcher;
				}
				
				return null;
			}
		}
		
		
		[Bundle] public WindowStyles			WindowStyles
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
		
		[Bundle] public WindowType				WindowType
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
		
		
		#region IBundleSupport Members
		public override string						PublicClassName
		{
			get { return "Window"; }
		}
		
		public override void RestoreFromBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
			string       name = this.Name;
			Drawing.Size size = this.Size;
			string       text = this.Text;
			
			WindowStyles window_styles = this.WindowStyles;
			WindowType   window_type   = this.WindowType;
			
			this.window = new Window (this);
			
			this.window.Name             = name;
			this.window.ClientSize       = size;
			this.window.Text             = this.ResourceManager.ResolveTextRef (text);
			this.window.WindowStyles     = window_styles;
			this.window.WindowType       = window_type;
			this.window.PreventAutoClose = true;
			
			this.Name = name;
			this.Text = text;
			
			base.RestoreFromBundle (bundler, bundle);
			
			if (bundle["icon"].Type == Support.ResourceFieldType.Data)
			{
				this.window.Icon = this.ResourceManager.GetImage ("res:" + bundle["icon"].AsString);
			}
			
			this.is_ready = true;
			this.Invalidate ();
		}
		#endregion
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			System.Diagnostics.Debug.Assert (this.Parent == null);
			
			if (this.window != null)
			{
				if ((this.InternalState & InternalState.SyncPaint) != 0)
				{
					this.window.SynchronousRepaint ();
					this.window.MarkForRepaint (this.MapClientToParent (rect));
					this.window.SynchronousRepaint ();
				}
				else
				{
					this.window.MarkForRepaint (this.MapClientToParent (rect));
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
				return this.Size;
			}
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
				//	Le raccourci clavier n'a pas �t� consomm�. Il faut voir si le raccourci clavier
				//	est attach� � une commande globale.
				
				System.Diagnostics.Debug.WriteLine (shortcut.ToString ());
				
				if (shortcut.IsAltPressed)
				{
					//	TODO: g�re les commandes globales
					
					if (shortcut.KeyCodeOnly == KeyCode.FuncF4)
					{
						this.Window.QueueCommand (this, "Quit" + this.Window.Name);
					}
				}
				
				Widget focused = this.window.FocusedWidget;
				bool   execute = true;
				
				if ((focused != null) &&
					(focused.AutoRadio))
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
					CommandDispatcher                dispatcher = this.CommandDispatcher;
					CommandDispatcher.CommandState[] states     = dispatcher.CommandStates;
					
					foreach (CommandDispatcher.CommandState state in states)
					{
						CommandState command = state as CommandState;
						
						if (command != null)
						{
							if ((command.Shortcut != null) &&
								(command.Shortcut.Match (shortcut)) &&
								(command.Enabled))
							{
								//	Ex�cute la commande.
								
								dispatcher.Dispatch (command.Name, this);
								return true;
							}
						}
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
						dir  = Message.State.IsShiftPressed ? TabNavigationDir.Backwards : TabNavigationDir.Forwards;
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
				}
				
				if ((mode != TabNavigationMode.Passive) &&
					(dir != TabNavigationDir.None))
				{
					//	Navigue dans la hi�rarchie...
					
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
				this.window.Text = this.ResourceManager.ResolveTextRef (this.Text);
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
			
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
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
				adorner.PaintWindowBackground(graphics, this.Client.Bounds, rect, WidgetState.None);
			}
		}
		
		
		protected virtual  void OnWindowStylesChanged()
		{
			if (this.WindowStylesChanged != null)
			{
				this.WindowStylesChanged (this);
			}
		}
		
		protected virtual  void OnWindowTypeChanged()
		{
			if (this.WindowTypeChanged != null)
			{
				this.WindowTypeChanged (this);
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
		
		
		
		public event Support.EventHandler			WindowStylesChanged;
		public event Support.EventHandler			WindowTypeChanged;
		
		
		protected WindowStyles						window_styles;
		protected WindowType						window_type;
		protected Window							window;
		protected bool								is_ready;
	}
}
