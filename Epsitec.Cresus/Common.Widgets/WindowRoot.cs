//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute   = Support.BundleAttribute;
	using CommandDispatcher = Support.CommandDispatcher;
	
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
			this.InternalState |= InternalState.AutoDoubleClick;
		}
		
		
		public WindowRoot(Window window) : this ()
		{
			this.window       = window;
			this.is_ready     = true;
		}
		
		
		public override bool					IsVisible
		{
			get
			{
				return true;
			}
		}
		
		public override LayoutStyles			Layout
		{
			get
			{
				return LayoutStyles.Manual;
			}
			set
			{
				//	Poubellise toute modification...
			}
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
		
		
		protected override void SetBounds(double x1, double y1, double x2, double y2)
		{
			double dx = x2 - x1;
			double dy = y2 - y1;
			
			if (this.window != null)
			{
				this.window.ClientSize = new Drawing.Size (dx, dy);
			}
			
			base.SetBounds (0, 0, dx, dy);
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
			
			if (this.window != null)
			{
				//	Le raccourci clavier n'a pas été consommé. Il faut voir si le raccourci clavier
				//	est attaché à une commande globale.
				
				System.Diagnostics.Debug.WriteLine (shortcut.ToString ());
				
				if (shortcut.IsAltPressed)
				{
					//	TODO: gère les commandes globales
					
					if (shortcut.KeyCodeOnly == KeyCode.FuncF4)
					{
						this.Window.QueueCommand (this, "Quit" + this.Window.Name);
					}
				}
				
				Widget focused = this.window.FocusedWidget;
				
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
				IAdorner adorner = Widgets.Adorner.Factory.Active;
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
