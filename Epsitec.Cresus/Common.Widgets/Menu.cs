namespace Epsitec.Common.Widgets
{
	public enum MenuType
	{
		Horizontal,						// ligne horizontale de textes seuls
		Vertical,						// liste verticale complète
	}

	/// <summary>
	/// La classe Menu représente un menu horizontal ou vertical.
	/// </summary>
	public class Menu : Widget
	{
		public enum TypeDeveloped
		{
			Close,
			Delay,
			Quick,
		}

		public Menu(MenuType type)
		{
			this.type = type;
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

		// Indique s'il faut développer les sous-menus.
		public TypeDeveloped MenuDeveloped
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
			MenuItem item = new MenuItem();
			item.OnlyText = false;
			item.IconName = iconName;
			item.MainText = mainText;
			item.ShortKey = shortKey;
			return this.Insert(item);
		}

		// Ajoute une case.
		public int InsertItem(string name)
		{
			MenuItem item = new MenuItem();
			item.OnlyText = true;
			item.MainText = name;
			return this.Insert(item);
		}

		// Ajoute un séparateur -----.
		public int InsertSep()
		{
			MenuItem item = new MenuItem();
			item.Separator = true;
			return this.Insert(item);
		}

		// Ajoute une case.
		public int Insert(MenuItem cell)
		{
			int rank = this.totalUsed;
			this.AllocateArray(rank+1);
			this.array[rank] = cell;
			this.Children.Add(cell);
			this.totalUsed ++;
			this.isDirty = true;

			cell.Pressed += new MessageEventHandler(this.HandleCellPressed);
			cell.Entered += new MessageEventHandler(this.HandleCellEntered);
			cell.Exited  += new MessageEventHandler(this.HandleCellExited);

			// debug !
			cell.Name = this.Name+":"+this.totalUsed;
			return rank;
		}

		// Modifie une case.
		public void Modify(int rank, MenuItem cell)
		{
			this.AllocateArray(rank+1);
			this.array[rank] = cell;
			this.Children.Add(cell);
			this.isDirty = true;
		}

		// Retourne le widget d'une case.
		public MenuItem GetWidget(int rank)
		{
			if ( rank >= this.array.Length )  return null;
			return this.array[rank];
		}

		// Spécifie le nombre de cases qui seront contenues dans la barre.
		// Cet appel est facultatif.
		public void SetSize(int max)
		{
			this.AllocateArray(max);
		}

		// Dimensionne le tableau des cases si nécessaire.
		protected void AllocateArray(int max)
		{
			if ( this.array == null )
			{
				this.array = new MenuItem[0];  // alloue un tableau vide
			}

			if ( max <= this.array.Length )  return;  // déjà assez grand ?

			MenuItem[] newArray = new MenuItem[max];  // nouveau tableau
			for ( int i=0 ; i<max ; i++ )
			{
				if ( i < this.array.Length )
				{
					newArray[i] = this.array[i];
				}
				else
				{
					newArray[i] = new MenuItem();
					newArray[i].Size = new Drawing.Size(0, 0);
				}
			}
			this.array = newArray;
		}

		// Ajuste les dimensions du menu selon son contenu.
		// Il faut appeler AdjustSize après avoir fini tous les InsertItem.
		public void AdjustSize()
		{
			if ( this.type == MenuType.Vertical )
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
				foreach ( MenuItem cell in this.array )
				{
					maxWidth = System.Math.Max(maxWidth, cell.IconWidth);
				}
				this.iconWidth = maxWidth;

				Drawing.Size size = new Drawing.Size(0, 0);
				foreach ( MenuItem cell in this.array )
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

			if ( this.array == null )  return;

			if ( this.type == MenuType.Horizontal )
			{
				double x = this.margin;
				foreach ( MenuItem cell in this.array )
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
				foreach ( MenuItem cell in this.array )
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


		// Ouvre le sous-menu correspondant à un item.
		protected bool OpenSubmenu(MenuItem item)
		{
			if ( this.submenu == item.Submenu )  return false;
			this.CloseSubmenu();
			this.submenu = item.Submenu;
			if ( this.submenu == null )  return false;
			System.Diagnostics.Debug.WriteLine("OpenSubmenu "+this.submenu.Name);

			int max = this.submenu.totalUsed;
			for ( int i=0 ; i<max ; i++ )
			{
				MenuItem mi = this.submenu.GetWidget(i);
				mi.SetSelected(false);
			}

			this.itemParent = item;
			this.itemParent.SetSelected(true);
			this.submenu.MenuDeveloped = TypeDeveloped.Delay;

			Drawing.Point pos = new Drawing.Point(0, 0);

			if ( this.type == MenuType.Horizontal )
			{
				Drawing.Rectangle pRect = item.Bounds;
				pRect.Offset(-pRect.Left, -pRect.Bottom);
				pRect.Offset(this.submenu.Left, this.submenu.Bottom);
				this.submenu.ParentRect = pRect;

				pos = item.MapClientToRoot(new Drawing.Point(0, -this.submenu.Height));
			}

			if ( this.type == MenuType.Vertical )
			{
				this.submenu.ParentRect = Drawing.Rectangle.Empty;

				pos = item.MapClientToRoot(new Drawing.Point(item.Width, item.Height-this.submenu.Height));
			}

			this.window = new WindowFrame();
			this.window.MakeFramelessWindow();
//			this.window.Owner = this.WindowFrame;
			pos = item.WindowFrame.MapWindowToScreen(pos);
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.submenu.Width, this.submenu.Height);
			this.window.WindowDeactivated += new System.EventHandler(this.HandleWindowDeactivated);
			this.window.Root.Children.Add(this.submenu);
			this.window.AnimateShow(Animation.RollDown);
			this.submenu.SetFocused(true);
			return true;
		}

		// Ferme le sous-menu ouvert.
		protected bool CloseSubmenu()
		{
			if ( this.window == null )  return false;
			
			System.Diagnostics.Debug.WriteLine("CloseSubmenu "+this.submenu.Name);
			System.Diagnostics.Debug.Assert(this.window.Root.HasChildren);
			
			// Commence par fermer immédiatement les sous-menus du sous-menu actuel.
			Widget[] widgets = this.window.Root.Children.Widgets;
			foreach ( Widget widget in widgets )
			{
				Menu submenu = widget as Menu;
				
				// Est-ce que le widget trouvé est un menu ?
				// Si oui, on le ferme. Si non, on le saute simplement.
				if ( submenu != null )
				{
					submenu.CloseSubmenu();
				}
			}
			
			this.itemParent.SetSelected(false);
			this.window.WindowDeactivated -= new System.EventHandler(this.HandleWindowDeactivated);
			this.window.Root.Children.Clear();
			this.window.Dispose();
			this.window = null;
			this.submenu = null;
			return true;
		}

		// Case du menu cliquée.
		private void HandleCellPressed(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("HandleCellPressed "+this.menuDeveloped);
			if ( this.menuDeveloped != TypeDeveloped.Close )
			{
				//WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
				this.menuDeveloped = TypeDeveloped.Close;
				this.CloseSubmenu();
			}
			else
			{
				//WindowFrame.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
				this.menuDeveloped = TypeDeveloped.Quick;
				MenuItem item = (MenuItem)sender;
				this.OpenSubmenu(item);
			}
		}

		private void HandlerMessageFilter(object sender, Message message)
		{
			WindowFrame window = sender as WindowFrame;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					break;
				
				case MessageType.MouseMove:
					break;

				case MessageType.MouseUp:
					break;
			}
			//message.Consumer = this;
		}

		private void HandleCellEntered(object sender, MessageEventArgs e)
		{
			if ( this.menuDeveloped == TypeDeveloped.Close )  return;
			MenuItem item = (MenuItem)sender;
			System.Diagnostics.Debug.WriteLine("HandleCellEntered "+item.MainText);
			this.OpenSubmenu(item);
		}
		
		private void HandleCellExited(object sender, MessageEventArgs e)
		{
			if ( this.menuDeveloped == TypeDeveloped.Close )  return;
		}
		
		// Appelé lorsque la fenêtre du sous-menu est désactivée.
		private void HandleWindowDeactivated(object sender, System.EventArgs e)
		{
			//this.CloseSubmenu();
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
		protected double				margin = 2;
		protected MenuItem[]			array;			// tableau des cases
		protected int					totalUsed;
		protected TypeDeveloped			menuDeveloped = TypeDeveloped.Close;
		protected WindowFrame			window;
		protected Menu					submenu;
		protected MenuItem				itemParent;
		protected double				iconWidth;
		protected Drawing.Rectangle		parentRect;
	}
}
