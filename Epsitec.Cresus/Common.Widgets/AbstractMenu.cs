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
	public class AbstractMenu : Widget
	{
		protected AbstractMenu(MenuType type)
		{
			this.type  = type;
			this.items = new MenuItemCollection (this);
			this.timer = new Timer();
			this.timer.TimeElapsed += new EventHandler(this.HandleTimerTimeElapsed);
		}
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			System.Collections.IList item_list = bundle.GetFieldBundleList ("items");
			
			if (item_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				
				foreach (Support.ResourceBundle item_bundle in item_list)
				{
					MenuItem item = bundler.CreateFromBundle (item_bundle) as MenuItem;
					
					this.Items.Add (item);
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
		protected void ProcessKeyDown(int key)
		{
			AbstractMenu parent = this.parentMenu;

			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Up:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Up "+this.Name);
					this.SelectOtherMenuItem(-1);
					break;

				case (int)System.Windows.Forms.Keys.Down:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Down "+this.Name);
					this.SelectOtherMenuItem(1);
					break;

				case (int)System.Windows.Forms.Keys.Left:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Left "+this.Name);
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

				case (int)System.Windows.Forms.Keys.Right:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Right "+this.Name);
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

				case (int)System.Windows.Forms.Keys.Return:
				case (int)System.Windows.Forms.Keys.Space:
					break;

				case (int)System.Windows.Forms.Keys.Escape:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Escape "+this.Name);
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

		// Ferme complètement le menu et tous les sous-menus.
		protected void CloseAll()
		{
			AbstractMenu root = this;
			while ( root.parentMenu != null )  root = root.parentMenu;
			System.Diagnostics.Debug.WriteLine("CloseAll "+root.Name+" "+this.Name);

			root.CloseSubmenu();
			root.SelectedIndex = -1;
			
			System.Diagnostics.Debug.WriteLine ("Filter removed in " + root.Name);
			WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			AbstractMenu.menuDeveloped = false;
		}

		// Ouvre le sous-menu correspondant à un item.
		protected bool OpenSubmenu(MenuItem item, bool forceQuick)
		{
			if ( this.submenu == item.Submenu )  return false;
			bool closed = this.CloseSubmenu();
			this.submenu = item.Submenu;
			if ( this.submenu == null )  return false;
			System.Diagnostics.Debug.WriteLine("OpenSubmenu "+this.submenu.Name);

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
				test = this.MapClientToRoot(test);
				test = this.WindowFrame.MapWindowToScreen(test);
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
#if false
				// TODO: Pourquoi la première case est mal positionnée verticalement ?
				// TODO: c'est MapWindowToScreen aui foire !
				Drawing.Point pp = item.WindowFrame.MapWindowToScreen(pos);
				System.Diagnostics.Debug.WriteLine("pos: "+item.Height+" "+this.submenu.Height+" "+item.Bottom+" "+pp.Y);
#endif
			}

			pos = item.MapClientToRoot(pos);

			this.window = new WindowFrame();
			this.window.MakeFramelessWindow();
			this.window.IsMouseActivationEnabled = false;
			pos = item.WindowFrame.MapWindowToScreen(pos);
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.submenu.Width, this.submenu.Height);
			WindowFrame.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			this.window.Root.Children.Add(this.submenu);

			Animation anim = Animation.None;
			if ( this.IsVertical || !closed )  anim = Animation.FadeIn;
			if ( forceQuick )  anim = Animation.None;
			this.window.AnimateShow(anim);
			this.submenu.SetFocused(true);
			return true;
		}

		// Ferme le sous-menu ouvert.
		protected bool CloseSubmenu()
		{
			if ( this.window == null )  return false;
			
			System.Diagnostics.Debug.WriteLine("CloseSubmenu "+this.submenu.Name);
			System.Diagnostics.Debug.Assert(this.window.Root.HasChildren);
			
			this.submenu.isActive = false;
			this.submenu.CloseSubmenu();  // ferme les sous-menus (reccursif)
			this.submenu.SelectedIndex = -1;
			this.submenu.parentMenu = null;
			this.submenu.parentItem = null;
			
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.window.Root.Children.Clear();
			this.window.Dispose();
			this.window = null;
			this.submenu = null;

			this.isActive = true;
			this.SelectedIndex = this.SelectedIndex;
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
					this.CloseAll();
				}
				else
				{
					MenuItem item = (MenuItem)sender;
					this.OpenSubmenu(item, false);
				}
			}
			else
			{
				WindowFrame.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
				AbstractMenu.menuDeveloped = true;
				MenuItem item = sender as MenuItem;
				this.parentMenu = null;
				this.parentItem = null;
				this.SetFocused(true);
				this.OpenSubmenu(item, false);
			}
		}

		private void MessageFilter(object sender, Message message)
		{
			System.Diagnostics.Debug.Assert ( AbstractMenu.menuDeveloped );
			
			WindowFrame window = sender as WindowFrame;

			Drawing.Point	mouse;
			AbstractMenu	menu;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = this.DetectMenu(mouse);
					if ( menu == null )
					{
						this.CloseAll();
						
						// On n'indique qu'un message est consommé que s'il concerne
						// la partie client de la fenêtre...						
						if ( !message.NonClient )
						{
							message.Handled = true;
							message.Swallowed = true;
						}
					}
					else
					{
						MenuItem cell = this.SearchItem(mouse, menu);
						if ( cell == null )
						{
							this.CloseAll();
							message.Handled = true;
							message.Swallowed = true;
						}
					}
					break;

				case MessageType.MouseUp:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = this.DetectMenu(mouse);
					if ( menu != null )
					{
						MenuItem cell = this.SearchItem(mouse, menu);
						if ( cell != null )
						{
							if ( cell.Submenu == null && !cell.Separator )
							{
								this.CloseAll();
								// TODO: envoyer la commande ...
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
						this.timer.Delay = SystemInformation.MenuShowDelay / 1000.0;
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
							this.timer.Delay = SystemInformation.MenuShowDelay / 1000.0;
							this.timer.Start();
						}
					}
				}
			}
		}
		
		private void HandleCellExited(object sender, MessageEventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			System.Diagnostics.Debug.WriteLine("HandleCellExited "+this.Name+":"+item.MainText);
			this.delayedMenuItem = null;
			
			if ( this.isActive )
			{
				this.SelectedIndex = -1;
			}
		}
		
		private void HandleApplicationDeactivated(object sender)
		{
			// TODO: pourquoi ce n'est pas toujours appelé ?
			System.Diagnostics.Debug.WriteLine("HandleApplicationDeactivated");
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
			Direction         dir   = this.RootDirection;

			this.Update();

			if ( this.IsVertical )
			{
				double iw = 0;
				if ( this.iconWidth > 10 )
				{
					iw = this.iconWidth+3;
				}
				adorner.PaintMenuBackground(graphics, rect, state, dir, this.parentRect, iw);
			}
		}

		#region MenuItemCollection Class
		public class MenuItemCollection : System.Collections.IList, System.IDisposable
		{
			public MenuItemCollection(AbstractMenu menu)
			{
				this.menu = menu;
				this.list = new System.Collections.ArrayList ();
			}
			
			public MenuItem this[int index]
			{
				get
				{
					return this.list[index] as MenuItem;
				}
			}
			
			public MenuItem this[string name]
			{
				get
				{
					foreach (MenuItem item in this.list)
					{
						if (item.Name == name)
						{
							return item;
						}
					}
					return null;
				}
			}
			
			public void Dispose()
			{
				System.Diagnostics.Debug.Assert (this.list.Count == 0);
				
				this.menu = null;
				this.list = null;
			}
			
			
			#region IList Members
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			object System.Collections.IList.this[int index]
			{
				get
				{
					return this.list[index];
				}
				set
				{
					this.list[index] = value;
				}
			}

			public void RemoveAt(int index)
			{
				MenuItem item = this.list[index] as MenuItem;
				this.HandleRemove (item);
				this.list.RemoveAt (index);
				this.RenumberItems ();
			}

			public void Insert(int index, object value)
			{
				this.list.Insert (index, value);
				this.HandleInsert (value as MenuItem);
				this.RenumberItems ();
			}

			public void Remove(object value)
			{
				this.HandleRemove (value as MenuItem);
				this.list.Remove (value);
				this.RenumberItems ();
			}

			public bool Contains(object value)
			{
				return this.Contains (value);
			}

			public void Clear()
			{
				foreach (MenuItem item in this.list)
				{
					this.HandleRemove (item);
				}
				this.list.Clear ();
			}

			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}

			public int Add(object value)
			{
				if (value is MenuItem)
				{
					int index = this.list.Add (value);
					this.HandleInsert (value as MenuItem);
					this.RenumberItems ();
					return index;
				}
				
				throw new System.ArgumentException ("Expecting MenuItem, got " + value.GetType ().Name);
			}

			public bool IsFixedSize
			{
				get
				{
					return this.list.IsFixedSize;
				}
			}
			#endregion
			
			#region ICollection Members
			public bool IsSynchronized
			{
				get
				{
					return this.list.IsSynchronized;
				}
			}

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}

			public object SyncRoot
			{
				get
				{
					return this.list.SyncRoot;
				}
			}

			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected void HandleInsert(MenuItem item)
			{
				this.menu.Children.Add (item);
				this.menu.isDirty = true;
				
				item.SetMenuType (this.menu.type);
				
				item.Pressed += new MessageEventHandler(this.menu.HandleCellPressed);
				item.Entered += new MessageEventHandler(this.menu.HandleCellEntered);
				item.Exited  += new MessageEventHandler(this.menu.HandleCellExited);
			}
			
			protected void HandleRemove(MenuItem item)
			{
				item.SetMenuType (MenuType.Invalid);
				
				item.Pressed -= new MessageEventHandler(this.menu.HandleCellPressed);
				item.Entered -= new MessageEventHandler(this.menu.HandleCellEntered);
				item.Exited  -= new MessageEventHandler(this.menu.HandleCellExited);
				
				this.menu.Children.Remove (item);
				this.menu.isDirty = true;
			}
			
			protected void RenumberItems()
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					MenuItem item = this.list[i] as MenuItem;
					item.Index = i;
				}
			}
			
			
			private System.Collections.ArrayList	list;
			private AbstractMenu					menu;
		}

		#endregion
		
		protected MenuType				type;
		protected bool					isDirty;
		protected bool					isActive = true;  // dernier menu (feuille)
		protected double				margin = 2;
		protected MenuItemCollection	items;
		protected WindowFrame			window;
		protected Timer					timer;
		protected AbstractMenu			submenu;
		protected AbstractMenu			parentMenu;
		protected MenuItem				parentItem;
		protected double				iconWidth;
		protected Drawing.Rectangle		parentRect;
		protected MenuItem				delayedMenuItem;
		
		protected static bool			menuDeveloped;
	}
}
