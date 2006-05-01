//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Menu représente un menu (horizontal ou vertical).
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractMenu : Widget, Collections.IWidgetCollectionHost
	{
		protected AbstractMenu()
		{
			if (Support.ObjectBundler.IsBooting)
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.UpdateAdornerInformation ();
			
			this.items = new MenuItemCollection (this);
			this.items.AutoEmbedding = true;
			
			MenuItem.SetMenuBehavior (this, new Behaviors.MenuBehavior ());
			MenuItem.SetMenuHost (this, new MenuHost (this));
		}
		
		
		public MenuItemCollection				Items
		{
			get
			{
				return this.items;
			}
		}
		
		
		public abstract MenuOrientation			MenuOrientation
		{
			get;
		}
		
		public bool								IsHorizontal
		{
			get
			{
				return this.MenuOrientation == MenuOrientation.Horizontal;
			}
		}

		public bool								IsVertical
		{
			get
			{
				return this.MenuOrientation == MenuOrientation.Vertical;
			}
		}

		public bool								IsMenuOpen
		{
			get
			{
				WindowRoot root = this.RootParent as WindowRoot;
				
				if (root != null)
				{
					return root.Window.IsVisible;
				}
				
				return false;
			}
		}
		
		public double							IconWidth
		{
			get
			{
				return this.icon_width;
			}
		}
		
		public Behaviors.MenuBehavior			Behavior
		{
			get
			{
				return MenuItem.GetMenuBehavior (this);
			}
		}
		
		public ICommandDispatcherHost			Host
		{
			get
			{
				return this.host;
			}
			set
			{
				this.host = value;
			}
		}
		
		
		public Drawing.Margins					MenuShadow
		{
			get
			{
				return this.shadow;
			}
		}
		
		public Drawing.Margins					MenuMargins
		{
			get
			{
				return this.margins;
			}
		}
		
#if false //#fix
		public override CommandDispatcher[]		CommandDispatchers
		{
			get
			{
				ICommandDispatcherHost host = this.FindHost ();
				
				return host == null ? base.CommandDispatchers : host.CommandDispatchers;
			}
		}
#endif
		
		
		public virtual void AdjustSize()
		{
			//	Ajuste les dimensions du menu selon son contenu.
			//	Il faut appeler AdjustSize après avoir fini de remplir le
			//	menu.
			
			if (this.IsVertical)
			{
				Drawing.Size size = this.GetBestFitSize ();

				this.Size = size;
				
				Layouts.LayoutContext.SyncArrange (this);
			}
		}
		
		public void ShowAsContextMenu(Window owner, Drawing.Point pos)
		{
			this.AdjustSize ();
			
			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			
			pos.Y -= this.Height;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenPopup (window, Behaviors.MenuBehavior.Animate.Yes);
			
			this.Focus ();
		}

		public void ShowAsContextMenu(Widget parent, Drawing.Point pos)
		{
			this.AdjustSize ();
			
			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			Window     owner  = parent.Window;
			
			pos.Y -= this.Height;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			window.ParentWidget   = parent;
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenPopup (window, Behaviors.MenuBehavior.Animate.Yes);
			
			this.Focus ();
		}
		
		public void ShowAsComboList(Widget parent, Drawing.Point pos, Widget button)
		{
			this.AdjustSize ();

			Window.ResetMouseCursor ();
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			Window     owner  = parent.Window;
			
			pos.Y -= this.Height;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			window.ParentWidget   = parent;
			
			if (button != null)
			{
				this.open_button = button;
				this.open_button.SetEngaged (true);
				this.open_button.SetFrozen (true);
			}
			
			this.ConnectEventHandlers ();
			this.Behavior.OpenCombo (window, Behaviors.MenuBehavior.Animate.Yes);
			
			this.Focus ();
			this.Window.MakeFocused ();
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			//	Retourne les dimensions nécessaires pour tout le menu,
			//	en partant de l'idée qu'il est vertical.
			
			this.UpdateAdornerInformation ();
			
			double max = 0;
			
			foreach (MenuItem item in this.items)
			{
				max = System.Math.Max (max, item.IconWidth);
			}
			
			this.icon_width = max;
			
			double dx = 0;
			double dy = 0;
			
			foreach (MenuItem item in this.items)
			{
				item.IconWidth = this.icon_width;
				
				Drawing.Size rs = item.GetBestFitSize ();
				
				dx  = System.Math.Max (dx, rs.Width);
				dy += rs.Height;
			}
			
			dx += this.shadow.Width  + this.margins.Width;
			dy += this.shadow.Height + this.margins.Height;

			dx = System.Math.Max (this.MinWidth, dx);
			dx = System.Math.Min (this.MaxWidth, dx);
			
			dy = System.Math.Max (this.MinHeight, dy);
			dy = System.Math.Min (this.MaxHeight, dy);

			return new Drawing.Size (dx, dy);
		}

		public override void Invalidate(Drawing.Rectangle rect)
		{
			if (this.IsHorizontal && this.IsVisible)
			{
				if (this.Parent != null)
				{
					Window window = this.Window;
					
					if (window != null)
					{
						if (window.IsSyncPaintDisabled == false)
						{
							window.SynchronousRepaint ();
							
							this.Parent.Invalidate (this.MapClientToParent (rect));
							
							window.SynchronousRepaint ();
							
							return;
						}
					}
				}
			}
			
			base.Invalidate(rect);
		}
		
		
		protected override void OnAdornerChanged()
		{
			this.AdjustSize ();
			base.OnAdornerChanged ();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.items != null)
				{
					MenuItem[] items = new MenuItem[this.items.Count];
					
					this.items.CopyTo (items, 0);
					this.items.Clear ();
					
					foreach (MenuItem item in items)
					{
						item.Dispose();
					}
					
					this.items.Dispose();
				}
				
				this.DisconnectEventHandlers ();
				
				this.items = null;
				this.host  = null;
			}
			
			base.Dispose (disposing);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			System.Diagnostics.Debug.Assert (this.FindHost() != null, "No Host defined for menu.",
				/**/						 "The menu you are trying to display has no associated command dispatcher host.\n"+
				/**/						 "Use AbstractMenu.Host to define it when you setup the menu.");
		}
		
		
		protected virtual void ConnectEventHandlers()
		{
			if (this.is_connected == false)
			{
				this.Behavior.Accepted += new Support.EventHandler (this.HandleBehaviorAccepted);
				this.Behavior.Rejected += new Support.EventHandler (this.HandleBehaviorRejected);
				
				this.is_connected = true;
			}
		}
		
		protected virtual void DisconnectEventHandlers()
		{
			if (this.is_connected)
			{
				if (this.open_button != null)
				{
					this.open_button.SetFrozen (false);
					this.open_button.SetEngaged (false);
					this.open_button = null;
				}
				
				this.Behavior.Accepted -= new Support.EventHandler (this.HandleBehaviorAccepted);
				this.Behavior.Rejected -= new Support.EventHandler (this.HandleBehaviorRejected);
				
				this.is_connected = false;
			}
		}
		
		
		private void HandleBehaviorAccepted(object sender)
		{
			this.OnAccepted ();
			this.DisconnectEventHandlers ();
		}
		
		private void HandleBehaviorRejected(object sender)
		{
			this.OnRejected ();
			this.DisconnectEventHandlers ();
		}
		
		
		private void UpdateAdornerInformation()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.margins = adorner.GeometryMenuMargins;
			this.shadow  = adorner.GeometryMenuShadow;
			
			if (this.IsVertical)
			{
				this.Padding = this.shadow + this.margins;
			}
			else
			{
				this.Padding = Drawing.Margins.Zero;
			}
		}

		private ICommandDispatcherHost FindHost()
		{
			if (this.host != null)
			{
				return this.host;
			}
			
			MenuWindow window = MenuItem.GetMenuWindow (this) as MenuWindow;
			
			if ((window != null) &&
				(window.ParentWidget != null))
			{
				Widget       parent = window.ParentWidget;
				AbstractMenu menu   = MenuItem.GetMenuRoot (parent) as AbstractMenu;
				
				if (menu != null)
				{
					ICommandDispatcherHost host = menu.FindHost ();
					
					if (host != null)
					{
						return host;
					}
				}
			}
			
			if (this.Window != null)
			{
				return this.Window;
			}
			
			return null;
		}
		
		
		#region MenuHost Class
		private class MenuHost : IMenuHost
		{
			public MenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, ref Drawing.Size size, out Drawing.Point location, out Animation animation)
			{
				ScreenInfo        screen_info;
				Drawing.Rectangle working_area;
				
				if (this.menu.IsHorizontal)
				{
					animation = Animation.RollDown;
					location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, 0));
					
					location.X -= this.menu.shadow.Left;
					location.Y -= size.Height;
				}
				else
				{
					Drawing.Point test = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (item.Width, 0));
					
					test.X += size.Width;
					
					screen_info  = ScreenInfo.Find (test);
					working_area = screen_info.WorkingArea;
					
					if (test.X <= working_area.Right)
					{
						//	Le menu a suffisamment de place pour s'afficher à droite
						//	de son parent :
						
						animation = Animation.RollRight;
						location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (item.Width, item.Height));
						
						location.X -= this.menu.shadow.Left;
						location.Y -= size.Height - this.menu.shadow.Top - 1;
					}
					else
					{
						//	Il n'y a plus de place à droite; on affiche par conséquent
						//	le sous-menu à gauche :
						
						animation = Animation.RollLeft;
						location  = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, item.Height));
						
						location.X -= size.Width + this.menu.shadow.Left;
						location.Y -= size.Height - this.menu.shadow.Top - 1;
					}
				}
				
				//	Détermine s'il faut décaler verticalement le menu pour qu'il tienne
				//	dans la partie visible de l'écran :
				
				screen_info  = ScreenInfo.Find (new Drawing.Rectangle (location, size));
				working_area = screen_info.WorkingArea;
				
				if (location.Y < working_area.Bottom)
				{
					animation  = Animation.FadeIn;
					location.Y = working_area.Bottom;
				}
			}
			#endregion
			
			private AbstractMenu				menu;
		}
		#endregion

		#region IWidgetCollectionHost Members
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			item.Size = item.GetBestFitSize ();
			item.DefineMenuOrientation (this.MenuOrientation);
			
			this.Children.Add (item);
			
			switch (this.MenuOrientation)
			{
				case MenuOrientation.Horizontal:
					item.Dock    = DockStyle.Left;
					item.Margins = new Drawing.Margins (2, 0, 2, 2);
					break;
				
				case MenuOrientation.Vertical:
					item.Dock    = DockStyle.Top;
					item.Margins = Drawing.Margins.Zero;
					break;
			}
		}
		
		public void NotifyRemoval(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			item.DefineMenuOrientation (MenuOrientation.Undefined);
			
			this.Children.Remove (item);
		}
		
		public void NotifyPostRemoval(Widget widget)
		{
		}
		#endregion
		
		#region MenuItemCollection Class
		public class MenuItemCollection : Collections.WidgetCollection
		{
			public MenuItemCollection(AbstractMenu menu) : base (menu)
			{
			}
			
			public new MenuItem this[int index]
			{
				get
				{
					return base[index] as MenuItem;
				}
			}
			
			public new MenuItem this[string name]
			{
				get
				{
					return base[name] as MenuItem;
				}
			}
		}
		#endregion
		
		protected virtual void OnAccepted()
		{
			Support.EventHandler handler = (Support.EventHandler) this.GetUserEventHandler ("Accepted");
			
			if (handler != null)
			{
				handler (this);
			}
		}
		
		protected virtual void OnRejected()
		{
			Support.EventHandler handler = (Support.EventHandler) this.GetUserEventHandler ("Rejected");
			
			if (handler != null)
			{
				handler (this);
			}
		}
		
		
		public event Support.EventHandler		Accepted
		{
			add
			{
				this.AddUserEventHandler ("Accepted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("Accepted", value);
			}
		}
		public event Support.EventHandler		Rejected
		{
			add
			{
				this.AddUserEventHandler ("Rejected", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("Rejected", value);
			}
		}
		
		private Drawing.Margins					margins = new Drawing.Margins (2,2,2,2);
		private Drawing.Margins					shadow  = Drawing.Margins.Zero;
		private MenuItemCollection				items;
		private ICommandDispatcherHost			host;
		private double							icon_width;
		private Widget							open_button;
		private bool							is_connected;
	}
}
