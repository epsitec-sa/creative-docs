namespace Epsitec.Common.Widgets
{
	using CommandDispatcher = Support.CommandDispatcher;
	
	public enum MenuType
	{
		Invalid,
		Vertical,
		Horizontal
	}
	
	public class MenuEventArgs : Support.EventArgs
	{
		public MenuEventArgs()
		{
			this.root = AbstractMenu.RootMenu;
		}
		
		
		public MenuItem							MenuItem
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}
		
		public AbstractMenu						RootMenu
		{
			get
			{
				return this.root;
			}
		}
		
		
		private MenuItem						item;
		private AbstractMenu					root;
	}
	
	public delegate void MenuEventHandler(object sender, MenuEventArgs e);
	
	
	/// <summary>
	/// La classe Menu représente un menu horizontal ou vertical.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractMenu : Widget, Collections.IWidgetCollectionHost
	{
		protected AbstractMenu(MenuType type)
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.UpdateAdornerInformation ();

			this.type  = type;
			this.items = new MenuItemCollection(this);
			this.items.AutoEmbedding = true;
			this.timer = new Timer();
			this.timer.TimeElapsed += new Support.EventHandler(this.HandleTimerTimeElapsed);
			
			MenuItem.SetMenuBehavior (this, new Behaviors.MenuBehavior ());
			MenuItem.SetMenuHost (this, new MenuHost (this));
		}
		
		
		public bool								IsHorizontal
		{
			get { return this.type == MenuType.Horizontal; }
		}

		public bool								IsVertical
		{
			get { return this.type == MenuType.Vertical; }
		}

		
		public Support.ICommandDispatcherHost	Host
		{
			get { return this.host; }
			set { this.host = value; }
		}
		
		public override CommandDispatcher		CommandDispatcher
		{
			get
			{
				Support.ICommandDispatcherHost host = this.GetHost();
				
				if ( host != null )
				{
					return host.CommandDispatcher;
				}
				
				return base.CommandDispatcher;
			}
		}
		
		public override double					DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 6 + this.margin*2;
			}
		}

		public Drawing.Rectangle				ParentRect
		{
			get
			{
				return this.parentRect;
			}

			set
			{
				this.parentRect = value;
			}
		}

		public Drawing.Size						RequiredSize
		{
			get
			{
				// Retourne les dimensions nécessaires pour tout le menu vertical.
				double maxWidth = 0;
				foreach ( MenuItem cell in this.items )
				{
					maxWidth = System.Math.Max(maxWidth, cell.IconWidth);
				}
				this.iconWidth = maxWidth;

				Drawing.Size size = new Drawing.Size(0, 0);
				foreach ( MenuItem cell in this.items )
				{
					cell.IconWidth = maxWidth;
					Drawing.Size rs = cell.RequiredSize;
					size.Width = System.Math.Max(size.Width, rs.Width);
					size.Height += rs.Height;
				}
				size.Width  += this.shadow.Width  + this.margins.Width;
				size.Height += this.shadow.Height + this.margins.Height;
				return size;
			}
		}

		
		public MenuItemCollection				Items
		{
			get
			{
				return this.items;
			}
		}

		public int								SelectedIndex
		{
			get
			{
				// Retourne la case sélectionnée dans un menu.
				for ( int i=0 ; i<this.items.Count ; i++ )
				{
					if ( this.items[i].ItemType != MenuItemType.Deselect )
					{
						return i;
					}
				}
				return -1;
			}
			set
			{
				// Sélectionne une (et une seule) case dans un menu.
				for ( int i=0 ; i<this.items.Count ; i++ )
				{
					if ( i == value )
					{
						this.items[i].ItemType = this.isActive ? MenuItemType.Select : MenuItemType.Parent;
					}
					else
					{
						this.items[i].ItemType = MenuItemType.Deselect;
					}
				}
			}
		}
		
		
		public static bool						IsMenuDeveloped
		{
			get
			{
				// Indique si au moins un menu est actuellement ouvert (on ne compte pas
				// le menu horizontal au sommet d'une fenêtre).
				return AbstractMenu.menuDeveloped;
			}
		}
		
		public static AbstractMenu				RootMenu
		{
			get
			{
				AbstractMenu root = AbstractMenu.menuRoot;
				
				if ( root == null )
				{
					return null;
				}
				
				while ( root.parentMenu != null )
				{
					root = root.parentMenu;
				}
				
				return root;
			}
		}
		
		public static Window					RootWindow
		{
			get
			{
				AbstractMenu menu = AbstractMenu.RootMenu;
				Window window = null;
				
				if ( menu != null )
				{
					window = menu.Window;
					
					while ( window != null && window.IsOwned )
					{
						window = window.Owner;
					}
				}
				
				return window;
			}
		}

		
		public override Drawing.Size GetBestFitSize()
		{
			this.UpdateAdornerInformation ();
			System.Diagnostics.Debug.WriteLine ("GetBestFitSize --> " + this.RequiredSize.ToString ());
			return this.RequiredSize;
		}

		public void AdjustSize()
		{
			// Ajuste les dimensions du menu selon son contenu.
			// Il faut appeler AdjustSize après avoir fini de remplir le menu vertical.
			
			this.UpdateAdornerInformation ();
			
			if (this.IsVertical)
			{
				this.Size = this.RequiredSize;
			}
		}
		
		private void UpdateAdornerInformation()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.margins = adorner.GeometryMenuMargins;
			this.shadow  = adorner.GeometryMenuShadow;
			
			if (this.IsVertical)
			{
				this.DockPadding = this.shadow + this.margins;
			}
			else
			{
				this.DockPadding = new Drawing.Margins (0, 0, 0, 0);
			}
		}

		
		public void ShowAsContextMenu(Window owner, Drawing.Point pos)
		{
			this.AdjustSize ();
			
			MenuWindow             window   = MenuItem.GetMenuWindow (this) as MenuWindow;
			Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
			
			pos.Y -= this.Height;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;
			
			window.Owner          = owner;
			window.WindowLocation = pos;
			
			behavior.OpenSubmenu (window, Behaviors.MenuBehavior.Animate.Yes);
			
#if false
			// Affiche un menu contextuel dont on spécifie le coin supérieur/gauche.

			this.isContextRoot = true;
			
			Window lastWindow = Message.State.LastWindow;
			if ( lastWindow != null )
			{
				lastWindow.MouseCursor = MouseCursor.Default;
			}
			
			pos.Y -= this.Height;
			pos.X -= this.shadow.Left;
			pos.Y += this.shadow.Top;

			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.MakeFloatingWindow();
			this.window.Owner = owner;
			this.window.CommandDispatcher = owner.CommandDispatcher;
			this.window.Name = "ContextMenu";
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			if ( adorner.AlphaMenu < 1.0 )
			{
				this.window.MakeLayeredWindow();
				this.window.Alpha = adorner.AlphaMenu;
				this.window.Root.BackColor = Drawing.Color.Transparent;
			}
			this.window.DisableMouseActivation();
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.Width, this.Height);
			
			this.SetParent (this.window.Root);
			AbstractMenu.RegisterFilter(this);

			this.window.AnimateShow(Animation.FadeIn);
			this.SetFocused(true);
			
			//	TODO: vérifier que lorsque le menu est refermé, les deux event handlers sont
			//	bien supprimés correctement...
#endif
		}

		
		protected Support.ICommandDispatcherHost GetHost()
		{
			if ( this.host != null )
			{
				return this.host;
			}
			if ( this.parentMenu != null )
			{
				this.parentMenu.GetHost();
			}
			if ( this.Window != null )
			{
				return this.Window;
			}
			
			return null;
		}
		
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			if ( this.IsHorizontal && this.IsVisible )
			{
				if ( this.Parent != null )
				{
					Window window = this.Window;
					
					if ( window != null )
					{
						if ( window.IsSyncPaintDisabled )
						{
							base.Invalidate(rect);
						}
						else
						{
							window.SynchronousRepaint();
							this.Parent.Invalidate(this.MapClientToParent(rect));
							window.SynchronousRepaint();
						}
						return;
					}
				}
			}
			
			base.Invalidate(rect);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.timer != null )
				{
					this.timer.TimeElapsed -= new Support.EventHandler(this.HandleTimerTimeElapsed);
					this.timer.Dispose();
				}
				
				if ( this.items != null )
				{
					MenuItem[] items = new MenuItem[this.items.Count];
					this.items.CopyTo(items, 0);
					this.items.Clear();
					
					foreach ( MenuItem item in items )
					{
						item.Dispose();
					}
					
					this.items.Dispose();
				}
				
				this.items = null;
				this.timer = null;
				
				this.host = null;
			}
			
			base.Dispose(disposing);
		}


		protected override void OnAdornerChanged()
		{
			this.AdjustSize ();
			this.UpdateClientGeometry ();
			
			base.OnAdornerChanged ();
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					if ( message.IsAltPressed == false &&
						 message.IsCtrlPressed == false &&
						 message.IsShiftPressed == false &&
						 this.ProcessKeyDown(message) )
					{
						message.Consumer = this;
					}
					break;
			}
		}

		protected virtual  bool ProcessKeyDown(Message message)
		{
			// Gestion d'une touche pressée avec KeyDown dans le menu.
			
			AbstractMenu parent = this.parentMenu;
			
			switch ( message.KeyCode )
			{
				case KeyCode.ArrowUp:
					this.SelectOtherMenuItem(-1);
					break;

				case KeyCode.ArrowDown:
					this.SelectOtherMenuItem(1);
					break;

				case KeyCode.ArrowLeft:
					if ( parent != null )
					{
						if ( parent.IsHorizontal )
						{
							parent.CloseSubmenu();
							parent.SelectOtherMenuItem(-1);
							MenuItem item = parent.Items[parent.SelectedIndex];
							if ( parent.OpenSubmenu(item, true) )
							{
								parent.submenu.SelectedIndex = 0;
							}
						}
						else
						{
							this.CloseMenuItem();
						}
					}
					break;

				case KeyCode.ArrowRight:
					if ( parent == null )
					{
						this.OpenMenuItem();
					}
					else
					{
						int sel = this.SelectedIndex;
						if ( sel == -1 || this.items[sel].Submenu == null )
						{
							parent = AbstractMenu.RootMenu;
							if ( parent.IsHorizontal )
							{
								parent.CloseSubmenu();
								parent.SelectOtherMenuItem(1);
								MenuItem item = parent.Items[parent.SelectedIndex];
								if ( parent.OpenSubmenu(item, true) )
								{
									parent.submenu.SelectedIndex = 0;
								}
							}
						}
						else
						{
							if ( !this.OpenMenuItem() )
							{
								parent.CloseSubmenu();
								parent.SelectOtherMenuItem(1);
							}
						}
					}
					break;

				default:
					IFeel feel = Feel.Factory.Active;
					
					if ( feel.TestSelectItemKey(message) )
					{
						int sel = this.SelectedIndex;
						
						if ( this.items[sel].Submenu == null && !this.items[sel].Separator )
						{
							AbstractMenu.ValidateAndExecuteCommand();
						}
						break;
					}
					
					if ( feel.TestCancelKey(message) )
					{
						AbstractMenu.Reject();
						break;
					}
					
					return false;
			}
			
			return true;
		}

		
		protected bool OpenMenuItem()
		{
			// Ouvre le sous-menu correspondant à la case sélectionnée.
			int sel = this.SelectedIndex;
			if ( sel == -1 )  return false;
			if ( !this.OpenSubmenu(this.items[sel], true) )  return false;
			this.submenu.SelectedIndex = 0;
			return true;
		}

		protected void CloseMenuItem()
		{
			// Ferme le sous-menu contenant la case sélectionnée.
			this.CloseSubmenu();

			this.parentMenu.submenu = null;
			this.parentMenu.isActive = true;
		}
		
		protected void SelectOtherMenuItem(int dir)
		{
			// Sélectionne la case suivante ou précédente.
			
			int sel = this.SelectedIndex;
			if ( sel == -1 )
			{
				sel = 0;
			}
			else
			{
				for ( int i=0 ; i<this.items.Count ; i++ )
				{
					sel += dir;
					if ( sel >= this.items.Count )
					{
						sel = 0;
					}
					if ( sel < 0 )
					{
						sel = this.items.Count-1;
					}
					if ( !this.items[sel].Separator )  break;
				}
			}
			this.SelectedIndex = sel;
		}

		
		protected static void ValidateAndExecuteCommand()
		{
			AbstractMenu menu = AbstractMenu.menuLastLeaf;
			if ( menu == null )  menu = AbstractMenu.RootMenu;
			
			MenuEventArgs e = new MenuEventArgs();
			
			int index = menu.SelectedIndex;
			
			if ( index >= 0 )
			{
				// L'utilisateur a sélectionné une commande dans un menu valide. On doit
				// encore générer la commande ad hoc.
				
				MenuItem item = menu.Items[index];
				
				e.MenuItem = item;
				
				menu.OnAccepted(e);
				item.ExecuteCommand();
			}
			
			AbstractMenu.CloseAll();
		}
		
		protected static void CloseAll()
		{
			// Ferme complètement tous les menus ouverts.
			AbstractMenu root = AbstractMenu.RootMenu;
			
			if ( root.isContextRoot )  // racine du menu contextuel ?
			{
				root.CloseSubmenu();
			}
			else	// menu normal ?
			{
				if ( root.submenu != null )
				{
					root.submenu.CloseSubmenu();
					root.submenu = null;
				}
				root.isActive = true;
				root.SelectedIndex = -1;
			}

			// Il faut dés-enregistrer la même instance que celle qui avait été enregistrée
			// au départ, sinon on se retrouve avec un filtre qui traîne...
			AbstractMenu.UnregisterFilter();
		}

		protected static void Reject()
		{
			AbstractMenu root = AbstractMenu.RootMenu;
			AbstractMenu.CloseAll();
			
			if ( root != null )
			{
				root.OnRejected();
			}
		}
		
		private bool OpenSubmenu(MenuItem item, bool forceQuick)
		{
			// Ouvre le sous-menu correspondant à un item.
			
			if ( this.submenu == item.Submenu )  return false;
			bool closed = false;
			if ( this.submenu != null )
			{
				closed = this.submenu.CloseSubmenu();
			}
			this.submenu = item.Submenu as AbstractMenu;
			if ( this.submenu == null )  return false;

			this.isActive = false;
			this.SelectedIndex = item.Index;  // sélectionne la case parent

			this.submenu.AdjustSize();
			this.submenu.isActive = true;
			this.submenu.SelectedIndex = -1;  // désélectionne tout
			this.submenu.parentMenu = this;
			this.submenu.parentItem = item;

			Drawing.Point pos    = new Drawing.Point(0, 0);
			Drawing.Point offset = new Drawing.Point(0, 0);

			if ( this.IsHorizontal )
			{
				Drawing.Rectangle pRect = item.Bounds;
				pRect.Offset(-pRect.Left, -pRect.Bottom);
				pRect.Offset(this.submenu.Left, this.submenu.Bottom);
				this.submenu.ParentRect = pRect;

				offset.Y = -this.submenu.Height;
				offset.X = -this.shadow.Left;
			}
			else
			{
				this.submenu.ParentRect = Drawing.Rectangle.Empty;

				Drawing.Point test = new Drawing.Point(item.Width, 0);
				test = this.MapClientToScreen(test);
				test.X += this.submenu.Width;
				ScreenInfo si = ScreenInfo.Find(test);
				Drawing.Rectangle wa = si.WorkingArea;

				if ( test.X <= wa.Right )  // sous-menu à droite ?
				{
					pos.X = item.Width;
					pos.Y = item.Height;
					offset.Y = 1-this.submenu.Height;
				}
				else	// sous-menu à gauche ?
				{
					pos.Y = item.Height;
					offset.X =  -this.submenu.Width;
					offset.Y = 1-this.submenu.Height;
				}
				pos.X -= this.shadow.Left;
				pos.Y += this.shadow.Top;
			}

			pos = item.MapClientToScreen(pos) + offset;

			if ( this.IsVertical )
			{
				ScreenInfo si = ScreenInfo.Find(pos);
				Drawing.Rectangle wa = si.WorkingArea;
				if ( pos.Y < wa.Bottom )
				{
					pos.Y = wa.Bottom;
				}
			}

			this.submenu.window = new Window();
			this.submenu.window.MakeFramelessWindow();
			this.submenu.window.MakeFloatingWindow();
			this.submenu.window.Owner = item.Window;
			this.submenu.window.CommandDispatcher = item.Window.CommandDispatcher;
			this.submenu.window.Name = "Menu";
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			if ( this.submenu.IsVertical && adorner.AlphaMenu < 1.0 )
			{
				this.submenu.window.MakeLayeredWindow();
				this.submenu.window.Alpha = adorner.AlphaMenu;
				this.submenu.window.Root.BackColor = Drawing.Color.Transparent;
			}
			this.submenu.window.DisableMouseActivation();
			this.submenu.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.submenu.Width, this.submenu.Height);
			
			this.submenu.SetParent (this.submenu.window.Root);
			AbstractMenu.RegisterFilter(this.submenu);
			 
			Animation anim = Animation.None;
			if ( this.IsVertical || !closed )  anim = Animation.FadeIn;
			if ( forceQuick )  anim = Animation.None;
			this.submenu.window.AnimateShow(anim);
			this.submenu.SetFocused(true);
			
			AbstractMenu.menuLastLeaf = this.submenu;
			
			return true;
		}
		
		
		#region MenuHost Class
		private class MenuHost : IMenuHost
		{
			public MenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, Drawing.Size size, out Drawing.Point location, out Animation animation)
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

		private bool CloseSubmenu()
		{
			// Ferme le sous-menu ouvert.
			
			if ( this.window == null )  return false;
			
			System.Diagnostics.Debug.Assert(this.window.Root.HasChildren);
			
			if ( this.submenu != null )
			{
				if ( AbstractMenu.menuRoot == this.submenu )
				{
					AbstractMenu.menuRoot = null;
				}
				
				this.submenu.isActive = false;
				this.submenu.CloseSubmenu();  // ferme les sous-menus (recursif)
				this.submenu.SelectedIndex = -1;
				this.submenu.parentMenu = null;
				this.submenu.parentItem = null;
			}
			
			this.window.Root.Children.Clear();
			this.submenu = null;
			
			if ( this.Window != null )
			{
				if ( Window.IsApplicationActive )
				{
					this.Window.MakeActive();
				}
			}

			this.window.Dispose();
			this.window = null;
			
			if ( this.parentMenu != null )
			{
				// Le menu parent devient la dernière feuille de l'arbre des menus.
				AbstractMenu.menuLastLeaf = this.parentMenu;
			
				this.isActive = true;
				this.parentMenu.SelectedIndex = this.parentMenu.SelectedIndex;
			}
			
			return true;
		}

		
		private static AbstractMenu DetectMenu(Drawing.Point mouse)
		{
			// Cherche dans quel menu ou sous-menu est la souris.
			
			AbstractMenu menu = AbstractMenu.RootMenu;
			Drawing.Point pos;
			pos = menu.MapScreenToParent(mouse);
			if ( menu.HitTest(pos) )  return menu;

			while ( true )
			{
				menu = menu.submenu;
				if ( menu == null )  break;

				pos = menu.MapScreenToParent(mouse);
				if ( menu.HitTest(pos) )  return menu;
			}

			return null;
		}

		private static MenuItem     SearchItem(Drawing.Point mouse, AbstractMenu menu)
		{
			// Cherche dans quel item d'un menu est la souris.
			foreach ( MenuItem cell in menu.items )
			{
				Drawing.Point pos;
				pos = cell.MapScreenToParent(mouse);
				if ( cell.HitTest(pos) )  return cell;
			}

			return null;
		}

		
		private static void MessageFilter(object sender, Message message)
		{
			if ( !AbstractMenu.menuDeveloped )
			{
				System.Diagnostics.Debug.Assert(AbstractMenu.menuDeveloped);
			}
			
			Window window = sender as Window;

			Drawing.Point	mouse;
			AbstractMenu	menu;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = AbstractMenu.DetectMenu(mouse);
					if ( menu == null )
					{
						AbstractMenu.Reject();
						
						// On n'indique qu'un message est consommé que s'il concerne
						// la partie client de la fenêtre...						
						if ( !message.NonClient )
						{
							message.Swallowed = true;
						}
					}
					else
					{
						MenuItem cell = AbstractMenu.SearchItem(mouse, menu);
						if ( cell == null )
						{
							AbstractMenu.Reject();
							message.Swallowed = true;
						}
					}
					break;

				case MessageType.MouseUp:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = AbstractMenu.DetectMenu(mouse);
					if ( menu != null )
					{
						MenuItem cell = AbstractMenu.SearchItem(mouse, menu);
						if ( cell != null )
						{
							if ( cell.Submenu == null && !cell.Separator )
							{
								AbstractMenu.ValidateAndExecuteCommand();
								message.Swallowed = true;
							}
						}
					}
					break;
				
				case MessageType.MouseEnter:
				case MessageType.MouseMove:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = AbstractMenu.DetectMenu(mouse);
					if ( menu == null )
					{
						message.Swallowed = true;
					}
					break;
			}
		}

		private static void HandleApplicationDeactivated(object sender)
		{
			AbstractMenu.Reject();
		}

		
		private void HandleCellPressed(object sender, MessageEventArgs e)
		{
			// Case du menu cliquée.
			if ( AbstractMenu.menuDeveloped )
			{
				if ( this.parentMenu == null )
				{
					AbstractMenu.ValidateAndExecuteCommand();
				}
				else
				{
					MenuItem item = (MenuItem) sender;
					this.OpenSubmenu(item, false);
				}
			}
			else
			{
				Window window = AbstractMenu.RootWindow;
				
				if ( window == null )
				{
					window = this.Window;
				}
				
				if ( AbstractMenu.initiallyFocusedWidget == null &&
					 window != null )
				{
					AbstractMenu.initiallyFocusedWidget = window.FocusedWidget;
				}
				
				MenuItem item = sender as MenuItem;
				
				this.parentMenu = null;
				this.parentItem = null;
				this.SetFocused(true);
				this.OpenSubmenu(item, false);
			}
		}

		private void HandleCellEntered(object sender, MessageEventArgs e)
		{
			MenuItem item = (MenuItem) sender;
			this.SelectedIndex = item.Index;
			int sel = this.SelectedIndex;

			if ( AbstractMenu.menuDeveloped )
			{
				if ( this.IsHorizontal )
				{
					this.OpenSubmenu(item, false);
				}
				else
				{
					if ( this.submenu == null )
					{
						// Il n'y a pas de sous-menu visible. Chaque fois que l'utilisateur
						// arrive sur une une autre ligne du menu, on remet à zéro le compteur.
						this.delayedMenuItem = item;
						
						this.timer.Suspend();
						this.timer.Delay = SystemInformation.MenuShowDelay;
						this.timer.Start();
					}
					else
					{
						// Il y a un sous-menu visible. On démarre le timer une seule fois,
						// lorsqu'une nouvelle ligne est activée (c'est forcément une autre
						// que celle qui a ouvert le sous-menu), et on se rappelle de la ligne
						// active.
						// Quand le temps est écoulé, on ouvre le sous-menu de la ligne active,
						// pour autant qu'il y en ait une (chaque fois que la souris sort d'une
						// ligne, on en prend note aussi).
						// Pour que ça marche, il faut aussi que lorsque la souris retourne
						// dans le sous-menu, ça active la bonne ligne dans le menu parent
						// (voir OnEntered).
						this.delayedMenuItem = item;
						
						if ( this.timer.State != TimerState.Running )
						{
							this.timer.Suspend();
							this.timer.Delay = SystemInformation.MenuShowDelay;
							this.timer.Start();
						}
					}
				}
			}
		}
		
		private void HandleCellExited(object sender, MessageEventArgs e)
		{
			MenuItem item = (MenuItem) sender;
			this.delayedMenuItem = null;
			
			if ( this.isActive )
			{
				this.SelectedIndex = -1;
			}
		}
		
		private void HandleTimerTimeElapsed(object sender)
		{
			if ( this.delayedMenuItem != null )
			{
				this.OpenSubmenu(this.delayedMenuItem, false);
			}
		}

		
		protected override void OnPressed(MessageEventArgs e)
		{
			base.OnPressed (e);
			
			if (this.IsHorizontal)
			{
				MenuItem.GetMenuBehavior (this).Reject ();
			}
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered(e);
			
			if ( this.parentMenu != null )
			{
				this.parentMenu.SelectedIndex = this.parentItem.Index;
				
				//	TODO: faire de même avec les parents du parent, etc. ?
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			System.Diagnostics.Debug.Assert (this.GetHost() != null, "No Host defined for menu.",
				/**/						 "The menu you are trying to display has no associated command dispatcher host.\n"+
				/**/						 "Use AbstractMenu.Host to define it when you setup the menu.");
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			WidgetState       state = this.PaintState;

			if ( this.IsVertical )
			{
				double iw = 0;
				if ( this.iconWidth > 10 )
				{
					iw = this.iconWidth+3;
				}
				adorner.PaintMenuBackground(graphics, rect, state, Direction.Down, this.parentRect, iw);
			}
		}

		
		private static void RegisterFilter(AbstractMenu root)
		{
			if ( AbstractMenu.menuDeveloped == false )
			{
//#				Window.ApplicationDeactivated += new Support.EventHandler(AbstractMenu.HandleApplicationDeactivated);
//#				Window.MessageFilter += new MessageHandler(AbstractMenu.MessageFilter);
				AbstractMenu.menuDeveloped = true;
				AbstractMenu.menuRoot      = root;
				AbstractMenu.menuLastLeaf  = root;
				
				Window window = AbstractMenu.RootWindow;
				
				if ( AbstractMenu.initiallyFocusedWidget == null &&
					 window != null )
				{
					AbstractMenu.initiallyFocusedWidget = window.FocusedWidget;
				}
			}
			else if ( AbstractMenu.menuRoot == null )
			{
				AbstractMenu.menuRoot = root;
			}
		}
		
		private static void UnregisterFilter()
		{
			if ( AbstractMenu.menuDeveloped )
			{
//#				Window.ApplicationDeactivated -= new Support.EventHandler(AbstractMenu.HandleApplicationDeactivated);
//#				Window.MessageFilter -= new MessageHandler(AbstractMenu.MessageFilter);
				AbstractMenu.menuDeveloped = false;
				AbstractMenu.menuLastLeaf  = null;
				AbstractMenu.menuRoot      = null;
				
				if ( AbstractMenu.initiallyFocusedWidget != null )
				{
					if ( AbstractMenu.initiallyFocusedWidget.Window != null )
					{
						AbstractMenu.initiallyFocusedWidget.SetFocused(true);
					}
					
					AbstractMenu.initiallyFocusedWidget = null;
				}
			}
		}
		
		
		#region IWidgetCollectionHost Members
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			item.Size = item.RequiredSize;
			this.Children.Add(item);
			
			item.SetMenuType(this.type);
			
			switch (this.type)
			{
				case MenuType.Horizontal:
					item.Dock = DockStyle.Left;
					item.DockMargins = new Drawing.Margins (this.margin, 0, this.margin, this.margin);
					break;
				
				case MenuType.Vertical:
					item.Dock = DockStyle.Top;
					item.DockMargins = new Drawing.Margins (0, 0, 0, 0);
					break;
			}
			
//#			item.Pressed += new MessageEventHandler(this.HandleCellPressed);
//#			item.Entered += new MessageEventHandler(this.HandleCellEntered);
//#			item.Exited  += new MessageEventHandler(this.HandleCellExited);
		}

		public void NotifyRemoval(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			item.SetMenuType(MenuType.Invalid);
			
//#			item.Pressed -= new MessageEventHandler(this.HandleCellPressed);
//#			item.Entered -= new MessageEventHandler(this.HandleCellEntered);
//#			item.Exited  -= new MessageEventHandler(this.HandleCellExited);
			
			this.Children.Remove(item);
		}
		
		public void NotifyPostRemoval(Widget widget)
		{
		}
		#endregion
		
		#region MenuItemCollection Class
		public class MenuItemCollection : Collections.WidgetCollection
		{
			public MenuItemCollection(AbstractMenu menu) : base(menu)
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
		
		
		protected virtual void OnAccepted(MenuEventArgs e)
		{
			if ( this.Accepted != null )
			{
				this.Accepted(this, e);
			}
		}
		
		protected virtual void OnRejected()
		{
			if ( this.Rejected != null )
			{
				this.Rejected(this);
			}
		}
		
		
		public event MenuEventHandler				Accepted;
		public event Support.EventHandler			Rejected;
		
		protected MenuType							type;
		
		private bool								isActive = true;	// dernier menu (feuille)
		private bool								isContextRoot = false;
		private readonly double						margin = 2;			// pour menu horizontal
		private Drawing.Margins						margins = new Drawing.Margins(2,2,2,2);
		private Drawing.Margins						shadow  = new Drawing.Margins(0,0,0,0);
		private MenuItemCollection					items;
		private Window								window;
		private Support.ICommandDispatcherHost		host;
		private Timer								timer;
		private AbstractMenu						submenu;
		private AbstractMenu						parentMenu;
		private MenuItem							parentItem;
		private double								iconWidth;
		private Drawing.Rectangle					parentRect;
		private MenuItem							delayedMenuItem;
		
		private static bool							menuDeveloped;
		private static AbstractMenu					menuLastLeaf;
		private static AbstractMenu					menuRoot;
		
		private static Widget						initiallyFocusedWidget;
	}
}
