namespace Epsitec.Common.Widgets
{
	public enum MenuType
	{
		Horizontal,						// ligne horizontale de textes seuls
		Vertical,						// liste verticale compl�te
	}

	/// <summary>
	/// La classe Menu repr�sente un menu horizontal ou vertical.
	/// </summary>
	public class AbstractMenu : Widget
	{
		protected enum TypeDeveloped
		{
			Close,
			Delay,
			Quick,
		}
		
		public class MenuItemCollection : System.Collections.IList
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
				this.list.RemoveAt (index);
			}

			public void Insert(int index, object value)
			{
				this.list.Insert (index, value);
				this.HandleInsert (value as MenuItem);
			}

			public void Remove(object value)
			{
				this.HandleRemove (value as MenuItem);
				this.list.Remove (value);
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
				int index = this.list.Add (value);
				this.HandleInsert (value as MenuItem);
				return index;
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
				
				item.Pressed += new MessageEventHandler(this.menu.HandleCellPressed);
				item.Entered += new MessageEventHandler(this.menu.HandleCellEntered);
				item.Exited  += new MessageEventHandler(this.menu.HandleCellExited);
			}
			
			protected void HandleRemove(MenuItem item)
			{
				item.Pressed -= new MessageEventHandler(this.menu.HandleCellPressed);
				item.Entered -= new MessageEventHandler(this.menu.HandleCellEntered);
				item.Exited  -= new MessageEventHandler(this.menu.HandleCellExited);
				
				this.menu.Children.Remove (item);
				this.menu.isDirty = true;
			}
			
			private System.Collections.ArrayList	list;
			private AbstractMenu					menu;
		}


		protected AbstractMenu(MenuType type)
		{
			this.items = new MenuItemCollection (this);
			this.type  = type;
			this.timer = new Timer();
			this.timer.TimeElapsed += new EventHandler(this.HandleTimerTimeElapsed);
		}

		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLine ("Disposing menu " + this.Name);
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

		// Indique s'il faut d�velopper les sous-menus.
		protected TypeDeveloped MenuDeveloped
		{
			get
			{
				return this.menuDeveloped;
			}

			set
			{
				this.menuDeveloped = value;
			}
		}

		// Ajoute une case.
		public int InsertItem(string iconName, string mainText, string shortKey)
		{
			string   name = this.items.Count.ToString ();
			MenuItem item = new MenuItem(this.type, name, iconName, mainText, shortKey);
			return this.items.Add(item);
		}

		// Ajoute une case.
		public int InsertItem(string text)
		{
			string   name = this.items.Count.ToString ();
			MenuItem item = new MenuItem(this.type, name, text);
			return this.items.Add(item);
		}

		// Ajoute un s�parateur -----.
		public int InsertSep()
		{
			MenuItem item = new MenuItem(this.type);
			return this.items.Add(item);
		}


		// Objet occupant une case.		
		public MenuItemCollection Items
		{
			get
			{
				return this.items;
			}
		}


		// Ajuste les dimensions du menu selon son contenu.
		// Il faut appeler AdjustSize apr�s avoir fini tous les InsertItem.
		public void AdjustSize()
		{
			if ( this.type == MenuType.Vertical )
			{
				this.Size = this.RequiredSize;
			}
		}

		// Donne les dimensions n�cessaires pour tout le menu.
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

		// Met � jour si n�cessaire.
		protected void Update()
		{
			if ( !this.isDirty )  return;
			this.UpdateClientGeometry();
		}

		// Met � jour la g�om�trie du menu.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.items == null ) return;

			if ( this.type == MenuType.Horizontal )
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

			if ( this.type == MenuType.Vertical )
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


		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					ProcessKeyDown(message.KeyCode);
					break;
			}
			
			message.Consumer = this;
		}

		// Gestion d'une touche press�e avec KeyDown dans le menu.
		protected void ProcessKeyDown(int key)
		{
			AbstractMenu parent = this.parentMenu;

			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Up:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Up "+this.Name);
					this.OtherMenuItem(-1);
					break;

				case (int)System.Windows.Forms.Keys.Down:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Down "+this.Name);
					this.OtherMenuItem(1);
					break;

				case (int)System.Windows.Forms.Keys.Left:
					System.Diagnostics.Debug.WriteLine("ProcessKeyDown.Left "+this.Name);
					if ( parent != null )
					{
						if ( parent.type == MenuType.Horizontal )
						{
							parent.CloseSubmenu();
							parent.OtherMenuItem(-1);
							MenuItem item = parent.Items[parent.RetSelectMenuItem()];
							if ( parent.OpenSubmenu(item, true) )
							{
								parent.submenu.SelectMenuItem(0);
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
						int sel = this.RetSelectMenuItem();
						if ( sel == -1 || this.items[sel].Submenu == null )
						{
							while ( parent.parentMenu != null )  parent = parent.parentMenu;
							if ( parent.type == MenuType.Horizontal )
							{
								parent.CloseSubmenu();
								parent.OtherMenuItem(1);
								MenuItem item = parent.Items[parent.RetSelectMenuItem()];
								if ( parent.OpenSubmenu(item, true) )
								{
									parent.submenu.SelectMenuItem(0);
								}
							}
						}
						else
						{
							if ( !this.OpenMenuItem() )
							{
								parent.CloseSubmenu();
								parent.OtherMenuItem(1);
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

		// S�lectionne la case suivante ou pr�c�dente.
		protected void OtherMenuItem(int dir)
		{
			int sel = this.RetSelectMenuItem();
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
			this.SelectMenuItem(sel);
		}

		// Ouvre le sous-menu correspondant � la case s�lectionn�e.
		protected bool OpenMenuItem()
		{
			int sel = this.RetSelectMenuItem();
			if ( sel == -1 )  return false;
			if ( !this.OpenSubmenu(this.items[sel], true) )  return false;
			this.submenu.SelectMenuItem(0);
			return true;
		}

		// Ferme le sous-menu contenant la case s�lectionn�e.
		protected bool CloseMenuItem()
		{
			if ( this.parentMenu == null )  return false;
			return this.parentMenu.CloseSubmenu();
		}

		// Retourne la case s�lectionn�e dans un menu.
		protected int RetSelectMenuItem()
		{
			int max = this.items.Count;
			for ( int i=0 ; i<max ; i++ )
			{
				MenuItem mi = this.items[i];
				if ( mi.ItemType != MenuItemType.Deselect )  return i;
			}
			return -1;
		}

		// S�lectionne une (et une seule) case dans un menu.
		protected void SelectMenuItem(int rank)
		{
			int max = this.items.Count;
			for ( int i=0 ; i<max ; i++ )
			{
				MenuItem mi = this.items[i];
				if ( i == rank )  // case s�lectionn�e ?
				{
					mi.ItemType = this.isActive ? MenuItemType.Select : MenuItemType.Parent;
				}
				else
				{
					mi.ItemType = MenuItemType.Deselect;
				}
			}
		}

		// S�lectionne une (et une seule) case dans un menu.
		protected void SelectMenuItem(MenuItem item)
		{
			int max = this.items.Count;
			for ( int i=0 ; i<max ; i++ )
			{
				MenuItem mi = this.items[i];
				if ( mi == item )  // case s�lectionn�e ?
				{
					mi.ItemType = this.isActive ? MenuItemType.Select : MenuItemType.Parent;
				}
				else
				{
					mi.ItemType = MenuItemType.Deselect;
				}
			}
		}

		// Ferme compl�tement le menu et tous les sous-menus.
		protected void CloseAll()
		{
			AbstractMenu root = this;
			while ( root.parentMenu != null )  root = root.parentMenu;
			System.Diagnostics.Debug.WriteLine("CloseAll "+root.Name+" "+this.Name);

			root.CloseSubmenu();
			root.SelectMenuItem(null);
			root.menuDeveloped = TypeDeveloped.Close;
		}

		// Ouvre le sous-menu correspondant � un item.
		protected bool OpenSubmenu(MenuItem item, bool forceQuick)
		{
			if ( this.submenu == item.Submenu )  return false;
			bool closed = this.CloseSubmenu();
			this.submenu = item.Submenu;
			if ( this.submenu == null )  return false;
			System.Diagnostics.Debug.WriteLine("OpenSubmenu "+this.submenu.Name);

			this.isActive = false;
			this.SelectMenuItem(item);  // s�lectionne la case parent

			this.submenu.isActive = true;
			this.submenu.SelectMenuItem(null);  // d�s�lectionne tout
			this.submenu.MenuDeveloped = TypeDeveloped.Delay;
			this.submenu.parentMenu = this;
			this.submenu.parentItem = item;

			Drawing.Point pos = new Drawing.Point(0, 0);

			if ( this.type == MenuType.Horizontal )
			{
				Drawing.Rectangle pRect = item.Bounds;
				pRect.Offset(-pRect.Left, -pRect.Bottom);
				pRect.Offset(this.submenu.Left, this.submenu.Bottom);
				this.submenu.ParentRect = pRect;

				pos = new Drawing.Point(0, -this.submenu.Height);
			}

			if ( this.type == MenuType.Vertical )
			{
				this.submenu.ParentRect = Drawing.Rectangle.Empty;

				Drawing.Point test = new Drawing.Point(item.Width+this.submenu.Width, 0);
				test = this.MapClientToRoot(test);
				test = this.WindowFrame.MapWindowToScreen(test);
				ScreenInfo si = ScreenInfo.Find(test);
				Drawing.Rectangle wa = si.WorkingArea;

				if ( test.X <= wa.Right )  // sous-menu � droite ?
				{
					pos = new Drawing.Point(item.Width, item.Height-this.submenu.Height+1);
				}
				else	// sous-menu � gauche ?
				{
					pos = new Drawing.Point(-this.submenu.Width, item.Height-this.submenu.Height+1);
				}
#if false
				// TODO: Pourquoi la premi�re case est mal positionn�e verticalement ?
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
			if ( this.type == MenuType.Vertical || !closed )  anim = Animation.FadeIn;
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
			this.submenu.SelectMenuItem(null);
			this.submenu.parentMenu = null;
			this.submenu.parentItem = null;
			
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.window.Root.Children.Clear();
			this.window.Dispose();
			this.window = null;
			this.submenu = null;

			this.isActive = true;
			this.SelectMenuItem(this.RetSelectMenuItem());
			return true;
		}

		// Conversion d'une coordonn�e �cran -> parent du widget.
		protected Drawing.Point MapScreenToClient(Widget widget, Drawing.Point pos)
		{
			pos = widget.WindowFrame.MapScreenToWindow(pos);
			pos = widget.MapRootToClient(pos);
			pos = widget.MapClientToParent(pos);
			return pos;
		}

		// Cherche dans quel menu ou sous-menu est la souris.
		protected AbstractMenu SearchMenu(Drawing.Point mouse)
		{
			Drawing.Point pos;
			pos = this.MapScreenToClient(this, mouse);
			if ( this.HitTest(pos) )  return this;

			AbstractMenu sub = this;
			while ( true )
			{
				sub = sub.submenu;
				if ( sub == null )  break;

				pos = this.MapScreenToClient(sub, mouse);
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
				pos = this.MapScreenToClient(cell, mouse);
				if ( cell.HitTest(pos) )  return cell;
			}

			return null;
		}

		// Case du menu cliqu�e.
		private void HandleCellPressed(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("HandleCellPressed "+this.menuDeveloped);
			if ( this.menuDeveloped != TypeDeveloped.Close )
			{
				if ( this.parentMenu == null )
				{
					System.Diagnostics.Debug.WriteLine ("Filter removed in " + this.Name);
					WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
					this.menuDeveloped = TypeDeveloped.Close;
					this.CloseSubmenu();
				}
				else
				{
					MenuItem item = (MenuItem)sender;
					this.OpenSubmenu(item, false);
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Filter added in " + this.Name);
				WindowFrame.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
				this.menuDeveloped = TypeDeveloped.Quick;
				MenuItem item = (MenuItem)sender;
				this.parentMenu = null;
				this.parentItem = null;
				this.SetFocused(true);
				this.OpenSubmenu(item, false);
			}
		}

		private void MessageFilter(object sender, Message message)
		{
			if ( this.menuDeveloped == TypeDeveloped.Close )  return;
			WindowFrame window = sender as WindowFrame;

			Drawing.Point	mouse;
			AbstractMenu	menu;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					mouse = window.MapWindowToScreen(message.Cursor);
					menu = this.SearchMenu(mouse);
					if ( menu == null )
					{
						this.CloseAll();
						
						// On n'indique qu'un message est consomm� que s'il concerne
						// la partie client de la fen�tre...						
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
					menu = this.SearchMenu(mouse);
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
			this.SelectMenuItem(item);
			int sel = this.RetSelectMenuItem();

			if ( this.menuDeveloped != TypeDeveloped.Close )
			{
				System.Diagnostics.Debug.WriteLine("HandleCellEntered "+this.Name+":"+item.MainText+", active="+this.isActive);
				if ( this.type == MenuType.Horizontal )
				{
					this.OpenSubmenu(item, false);
				}
				else
				{
					if ( this.submenu == null )
					{
						// Il n'y a pas de sous-menu visible. Chaque fois que l'utilisateur
						// arrive sur une une autre ligne du menu, on remet � z�ro le compteur.
						this.delayedMenuItem = item;
						
						this.timer.Suspend();
						this.timer.Delay = SystemInformation.MenuShowDelay / 1000.0;
						this.timer.Start();
					}
					else
					{
						// Il y a un sous-menu visible. On d�marre le timer une seule fois,
						// lorsqu'une nouvelle ligne est activ�e (c'est forc�ment une autre
						// que celle qui a ouvert le sous-menu), et on se rappelle de la ligne
						// active.
						// Quand le temps est �coul�, on ouvre le sous-menu de la ligne active,
						// pour autant qu'il y en ait une (chaque fois que la souris sort d'une
						// ligne, on en prend note aussi).
						// Pour que �a marche, il faut aussi que lorsque la souris retourne
						// dans le sous-menu, �a active la bonne ligne dans le menu parent
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
				this.SelectMenuItem(null);
			}
		}
		
		private void HandleApplicationDeactivated(object sender)
		{
			// TODO: pourquoi ce n'est pas toujours appel� ?
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
				this.parentMenu.SelectMenuItem(this.parentItem);
				
				//	TODO: faire de m�me avec les parents du parent, etc. ?
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

			if ( this.type == MenuType.Vertical )
			{
				double iw = 0;
				if ( this.iconWidth > 10 )
				{
					iw = this.iconWidth+3;
				}
				adorner.PaintMenuBackground(graphics, rect, state, dir, this.parentRect, iw);
			}
		}


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
		
		protected TypeDeveloped			menuDeveloped = TypeDeveloped.Close;
	}
}
