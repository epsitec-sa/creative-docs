namespace Epsitec.Common.Widgets
{
	public enum MenuType
	{
		Invalid,
		Vertical,
		Horizontal
	}
	
	/// <summary>
	/// La classe Menu représente un menu horizontal ou vertical.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractMenu : Widget, Helpers.IWidgetCollectionHost
	{
		protected AbstractMenu(MenuType type)
		{
			this.type  = type;
			this.items = new MenuItemCollection(this);
			this.timer = new Timer();
			this.timer.TimeElapsed += new EventHandler(this.HandleTimerTimeElapsed);
		}
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			Support.ResourceBundle.FieldList item_list = bundle["items"].AsList;
			
			if (item_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				
				foreach (Support.ResourceBundle.Field field in item_list)
				{
					Support.ResourceBundle item_bundle = field.AsBundle;
					MenuItem               item_widget = bundler.CreateFromBundle (item_bundle) as MenuItem;
					
					this.Items.Add (item_widget);
				}
				
				this.AdjustSize ();
			}
		}
		#endregion
		
		
		public bool IsHorizontal
		{
			get { return this.type == MenuType.Horizontal; }
		}

		public bool IsVertical
		{
			get { return this.type == MenuType.Vertical; }
		}

		
		protected override void BuildCommandName(System.Text.StringBuilder buffer)
		{
			if (this.parentMenu != null)
			{
				this.parentMenu.BuildCommandName (buffer);
			}
			int length = buffer.Length;
			
			if ((length > 0) &&
				(buffer[length-1] != '.'))
			{
				buffer.Append (".");
			}
			
			buffer.Append (this.Name);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				MenuItem[] items = new MenuItem[this.items.Count];
				this.items.CopyTo (items, 0);
				this.items.Clear ();
				
				foreach (MenuItem item in items)
				{
					item.Dispose ();
				}
				
				this.timer.TimeElapsed -= new EventHandler(this.HandleTimerTimeElapsed);
				this.timer.Dispose();
				this.items.Dispose();
				
				this.items = null;
				this.timer = null;
			}
			
			base.Dispose(disposing);
		}


		// Retourne la hauteur standard d'un menu.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 6 + this.margin*2;
			}
		}

		// Rectangle parent.
		public Drawing.Rectangle ParentRect
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

		// Ajoute une case.
		protected int InsertItem(string iconName, string mainText, string shortKey)
		{
			string   name = this.items.Count.ToString ();
			MenuItem item = new MenuItem(name, iconName, mainText, shortKey);
			return this.items.Add(item);
		}

		
		public MenuItemCollection Items
		{
			get
			{
				return this.items;
			}
		}


		// Ajuste les dimensions du menu selon son contenu.
		// Il faut appeler AdjustSize après avoir fini tous les InsertItem.
		public void AdjustSize()
		{
			if ( this.IsVertical )
			{
				this.Size = this.RequiredSize;
			}
		}

		// Donne les dimensions nécessaires pour tout le menu.
		protected Drawing.Size RequiredSize
		{
			get
			{
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
				size.Width  += this.margin*2;
				size.Height += this.margin*2;
				return size;
			}
		}

		// Met à jour si nécessaire.
		protected void Update()
		{
			if ( !this.isDirty )  return;
			this.UpdateClientGeometry();
		}

		// Met à jour la géométrie du menu.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.items == null ) return;

			if ( this.IsHorizontal )
			{
				double x = this.margin;
				foreach ( MenuItem cell in this.items )
				{
					Drawing.Size rs = cell.RequiredSize;
					Drawing.Rectangle rect = new Drawing.Rectangle();
					rect.Left   = x;
					rect.Right  = x+rs.Width;
					rect.Bottom = this.margin;
					rect.Top    = this.Height-this.margin;
					cell.Bounds = rect;
					x += rs.Width;
				}
			}
			else
			{
				double y = this.Height-this.margin;
				foreach ( MenuItem cell in this.items )
				{
					Drawing.Size rs = cell.RequiredSize;
					y -= rs.Height;
					Drawing.Rectangle rect = new Drawing.Rectangle();
					rect.Left   = this.margin;
					rect.Right  = this.Width-this.margin;
					rect.Bottom = y;
					rect.Top    = y+rs.Height;
					cell.Bounds = rect;
				}
			}

			this.isDirty = false;
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					this.ProcessKeyDown(message.KeyCode);
					break;
			}
			
			message.Consumer = this;
		}

		// Gestion d'une touche pressée avec KeyDown dans le menu.
		protected void ProcessKeyDown(KeyCode key)
		{
			AbstractMenu parent = this.parentMenu;

			switch ( key )
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
							if ( !this.CloseMenuItem() )
							{
								parent.CloseSubmenu();
							}
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
							while ( parent.parentMenu != null )  parent = parent.parentMenu;
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

				case KeyCode.Return:
				case KeyCode.Space:
					this.ValidateAndExecuteCommand();
					break;

				case KeyCode.Escape:
					this.CloseAll();
					break;
			}
		}

		// Sélectionne la case suivante ou précédente.
		protected void SelectOtherMenuItem(int dir)
		{
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

		// Ouvre le sous-menu correspondant à la case sélectionnée.
		protected bool OpenMenuItem()
		{
			int sel = this.SelectedIndex;
			if ( sel == -1 )  return false;
			if ( !this.OpenSubmenu(this.items[sel], true) )  return false;
			this.submenu.SelectedIndex = 0;
			return true;
		}

		// Ferme le sous-menu contenant la case sélectionnée.
		protected bool CloseMenuItem()
		{
			if ( this.parentMenu == null )  return false;
			return this.parentMenu.CloseSubmenu();
		}
		
		
		public int SelectedIndex
		{
			get
			{
				// Retourne la case sélectionnée dans un menu.
				for (int i = 0; i < this.items.Count; i++)
				{
					if (this.items[i].ItemType != MenuItemType.Deselect)
					{
						return i;
					}
				}
				return -1;
			}
			set
			{
				// Sélectionne une (et une seule) case dans un menu.
				for (int i = 0; i < this.items.Count; i++)
				{
					if (i == value)
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
		
		protected void ValidateAndExecuteCommand()
		{
			AbstractMenu menu = AbstractMenu.menuLastLeaf;
			
			if ( menu == null )  menu = this;
			
			if ( menu.SelectedIndex >= 0 )
			{
				// L'utilisateur a sélectionné une commande dans un menu valide. On doit
				// encore générer la commande ad hoc.
				
				MenuItem item = menu.Items[menu.SelectedIndex];
				
				item.ExecuteCommand ();
			}
			
			this.CloseAll();
		}

		// Ferme complètement le menu et tous les sous-menus.
		protected void CloseAll()
		{
			AbstractMenu root = this;
			while ( root.parentMenu != null )  root = root.parentMenu;

			root.CloseSubmenu();
			root.SelectedIndex = -1;
			
			// Il faut dés-enregistrer la même instance que celle qui avait été enregistrée
			// au départ, sinon on se retrouve avec un filtre qui traîne...
			
			Window.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(AbstractMenu.menuFiltering.MessageFilter);
			AbstractMenu.menuDeveloped = false;
			AbstractMenu.menuFiltering = null;
			AbstractMenu.menuLastLeaf  = null;
		}

		// Affiche un menu contextuel dont on spécifie le coin sup/gauche.
		public void ShowContextMenu(Drawing.Point pos)
		{
			pos.Y -= this.Height;

			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.DisableMouseActivation();
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.Width, this.Height);
			Window.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			
			Window.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			AbstractMenu.menuDeveloped = true;
			AbstractMenu.menuFiltering = this;

			this.window.Root.Children.Add(this);
			this.window.AnimateShow(Animation.FadeIn);
			this.SetFocused(true);
			
			//	TODO: vérifier que lorsque le menu est refermé, les deux event handlers sont
			//	bien supprimés correctement...
		}

		// Ouvre le sous-menu correspondant à un item.
		protected bool OpenSubmenu(MenuItem item, bool forceQuick)
		{
			if ( this.submenu == item.Submenu )  return false;
			bool closed = this.CloseSubmenu();
			this.submenu = item.Submenu;
			if ( this.submenu == null )  return false;

			this.isActive = false;
			this.SelectedIndex = item.Index;  // sélectionne la case parent

			this.submenu.isActive = true;
			this.submenu.SelectedIndex = -1;  // désélectionne tout
			this.submenu.parentMenu = this;
			this.submenu.parentItem = item;

			Drawing.Point pos = new Drawing.Point(0, 0);

			if ( this.IsHorizontal )
			{
				Drawing.Rectangle pRect = item.Bounds;
				pRect.Offset(-pRect.Left, -pRect.Bottom);
				pRect.Offset(this.submenu.Left, this.submenu.Bottom);
				this.submenu.ParentRect = pRect;

				pos = new Drawing.Point(0, -this.submenu.Height);
			}
			else
			{
				this.submenu.ParentRect = Drawing.Rectangle.Empty;

				Drawing.Point test = new Drawing.Point(item.Width+this.submenu.Width, 0);
				test = this.MapClientToScreen(test);
				ScreenInfo si = ScreenInfo.Find(test);
				Drawing.Rectangle wa = si.WorkingArea;

				if ( test.X <= wa.Right )  // sous-menu à droite ?
				{
					pos = new Drawing.Point(item.Width, item.Height-this.submenu.Height+1);
				}
				else	// sous-menu à gauche ?
				{
					pos = new Drawing.Point(-this.submenu.Width, item.Height-this.submenu.Height+1);
				}
			}

			pos = item.MapClientToScreen(pos);

			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.DisableMouseActivation();
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.submenu.Width, this.submenu.Height);
			Window.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			
			this.submenu.Parent = this.window.Root;
			
			Animation anim = Animation.None;
			if ( this.IsVertical || !closed )  anim = Animation.FadeIn;
			if ( forceQuick )  anim = Animation.None;
			this.window.AnimateShow(anim);
			this.submenu.SetFocused(true);
			
			// Prend note du dernier menu "feuille" actif.
			AbstractMenu.menuLastLeaf = this.submenu;
			
			return true;
		}

		// Ferme le sous-menu ouvert.
		protected bool CloseSubmenu()
		{
			if ( this.window == null )  return false;
			
			System.Diagnostics.Debug.Assert(this.window.Root.HasChildren);
			
			if ( this.submenu != null )
			{
				this.submenu.isActive = false;
				this.submenu.CloseSubmenu();  // ferme les sous-menus (reccursif)
				this.submenu.SelectedIndex = -1;
				this.submenu.parentMenu = null;
				this.submenu.parentItem = null;
			}
			
			Window.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.window.Root.Children.Clear();
			this.window.Dispose();
			this.window = null;
			this.submenu = null;
			
			if ( this.Window != null )
			{
				this.Window.MakeActive();
			
				// Ce menu devient la dernière feuille de l'arbre des menus...
				AbstractMenu.menuLastLeaf = this;
			
				this.isActive = true;
				this.SelectedIndex = this.SelectedIndex;
			}
			return true;
		}


		// Cherche dans quel menu ou sous-menu est la souris.
		protected AbstractMenu DetectMenu(Drawing.Point mouse)
		{
			Drawing.Point pos;
			pos = this.MapScreenToParent(mouse);
			if ( this.HitTest(pos) )  return this;

			AbstractMenu sub = this;
			while ( true )
			{
				sub = sub.submenu;
				if ( sub == null )  break;

				pos = sub.MapScreenToParent(mouse);
				if ( sub.HitTest(pos) )  return sub;
			}

			return null;
		}

		// Cherche dans quel item d'un menu est la souris.
		protected MenuItem SearchItem(Drawing.Point mouse, AbstractMenu menu)
		{
			foreach ( MenuItem cell in menu.items )
			{
				Drawing.Point pos;
				pos = cell.MapScreenToParent(mouse);
				if ( cell.HitTest(pos) )  return cell;
			}

			return null;
		}

		// Case du menu cliquée.
		private void HandleCellPressed(object sender, MessageEventArgs e)
		{
			if ( AbstractMenu.menuDeveloped )
			{
				if ( this.parentMenu == null )
				{
					this.ValidateAndExecuteCommand();
				}
				else
				{
					MenuItem item = (MenuItem)sender;
					this.OpenSubmenu(item, false);
				}
			}
			else
			{
				if (AbstractMenu.menuDeveloped == false)
				{
					Window.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
					AbstractMenu.menuDeveloped = true;
					AbstractMenu.menuFiltering = this;
				}
				
				MenuItem item = sender as MenuItem;
				
				this.parentMenu = null;
				this.parentItem = null;
				this.SetFocused(true);
				this.OpenSubmenu(item, false);
			}
		}

		private void MessageFilter(object sender, Message message)
		{
			if (!AbstractMenu.menuDeveloped)
			{
				System.Diagnostics.Debug.Assert ( AbstractMenu.menuDeveloped );
			}
			
			Window window = sender as Window;

			Drawing.Point	mouse;
			AbstractMenu	menu;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					mouse = window.Root.MapClientToScreen(message.Cursor);
					menu = this.DetectMenu(mouse);
					if ( menu == null )
					{
						this.CloseAll();
						
						// On n'indique qu'un message est consommé que s'il concerne
						// la partie client de la fenêtre...						
						if ( !message.NonClient )
						{
							message.Swallowed = true;
						}
					}
					else
					{
						MenuItem cell = this.SearchItem(mouse, menu);
						if ( cell == null )
						{
							this.CloseAll();
							message.Swallowed = true;
						}
					}
					break;

				case MessageType.MouseUp:
					mouse = window.Root.MapClientToScreen(message.Cursor);
					menu = this.DetectMenu(mouse);
					if ( menu != null )
					{
						MenuItem cell = this.SearchItem(mouse, menu);
						if ( cell != null )
						{
							if ( cell.Submenu == null && !cell.Separator )
							{
								this.ValidateAndExecuteCommand();
								message.Swallowed = true;
							}
						}
					}
					break;
			}
		}

		private void HandleCellEntered(object sender, MessageEventArgs e)
		{
			MenuItem item = (MenuItem)sender;
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
			MenuItem item = (MenuItem)sender;
			this.delayedMenuItem = null;
			
			if ( this.isActive )
			{
				this.SelectedIndex = -1;
			}
		}
		
		private void HandleApplicationDeactivated(object sender)
		{
			// TODO: pourquoi ce n'est pas toujours appelé ?
			this.CloseAll();
		}

		private void HandleTimerTimeElapsed(object sender)
		{
			if ( this.delayedMenuItem != null )
			{
				this.OpenSubmenu(this.delayedMenuItem, false);
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


		// Dessine le menu.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;

			this.Update();

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

		#region IWidgetCollectionHost Members
		public void NotifyInsertion(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			this.Children.Add (item);
			this.isDirty = true;
			
			item.SetMenuType (this.type);
			
			item.Pressed += new MessageEventHandler(this.HandleCellPressed);
			item.Entered += new MessageEventHandler(this.HandleCellEntered);
			item.Exited  += new MessageEventHandler(this.HandleCellExited);
		}

		public void NotifyRemoval(Widget widget)
		{
			MenuItem item = widget as MenuItem;
			
			item.SetMenuType (MenuType.Invalid);
			
			item.Pressed -= new MessageEventHandler(this.HandleCellPressed);
			item.Entered -= new MessageEventHandler(this.HandleCellEntered);
			item.Exited  -= new MessageEventHandler(this.HandleCellExited);
			
			this.Children.Remove (item);
			this.isDirty = true;
		}
		#endregion
		
		
		#region MenuItemCollection Class
		public class MenuItemCollection : Helpers.WidgetCollection
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
		
		protected MenuType					type;
		protected bool						isDirty;
		protected bool						isActive = true;  // dernier menu (feuille)
		protected double					margin = 2;
		protected MenuItemCollection		items;
		protected Window					window;
		protected Timer						timer;
		protected AbstractMenu				submenu;
		protected AbstractMenu				parentMenu;
		protected MenuItem					parentItem;
		protected double					iconWidth;
		protected Drawing.Rectangle			parentRect;
		protected MenuItem					delayedMenuItem;
		
		protected static bool				menuDeveloped;
		protected static AbstractMenu		menuFiltering;
		protected static AbstractMenu		menuLastLeaf;
	}
}
