//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButtonCombo implémente un IconButton avec bouton "v"
	/// qui fait apparaître un menu dit "combo" pour permettre de choisir une
	/// icône prédéfinie.
	/// </summary>
	public class IconButtonCombo : Widget
	{
		public IconButtonCombo()
		{
			this.items = new System.Collections.ArrayList();
			
			this.buttonMain = new IconButton(this);
			this.buttonMain.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonMain.Pressed += new MessageEventHandler(this.HandleButtonPressed);

			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.ArrowDown;
			this.buttonMenu.ButtonStyle = ButtonStyle.Combo;
			this.buttonMenu.Name = "Open";
			this.buttonMenu.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			
			this.AddEvent(CommandState.AdvancedStateProperty, new Types.PropertyChangedEventHandler(this.HandleAdvancedStatePropertyChanged));
		}
		
		public IconButtonCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public virtual bool						IsComboOpen
		{
			get
			{
				return this.menu != null;
			}
		}
		
		public bool								IsLiveUpdateEnabled
		{
			get
			{
				return this.isLiveUpdateEnabled;
			}

			set
			{
				this.isLiveUpdateEnabled = value;
			}
		}
		
		public System.Collections.ArrayList		Items
		{
			//	Donne la collection d'éléments de type IconButtonCombo.Item.
			get
			{
				return this.items;
			}
		}

		public int								SelectedIndex
		{
			//	Donne le rang de l'élément sélectionné (-1 si aucune sélection).
			get
			{
				return this.selectedIndex;
			}

			set
			{
				if ( this.selectedIndex != value )
				{
					this.selectedIndex = value;
					this.UpdateIcon();
					this.OnSelectedIndexChanged();
				}
			}
		}

		public string							SelectedName
		{
			//	Donne le nom de l'élément sélectionné (null si aucune sélection).
			get
			{
				int sel = this.SelectedIndex;
				if ( sel == -1 )  return null;

				int rank = 0;
				foreach ( Item item in this.items )
				{
					if ( item == null )  continue;  // séparateur ?

					if ( rank == sel )
					{
						return item.Name;
					}

					rank ++;
				}

				return null;
			}

			set
			{
				int rank = 0;
				foreach ( Item item in this.items )
				{
					if ( item == null )  continue;  // séparateur ?

					if ( item.Name == value )
					{
						this.SelectedIndex = rank;
						break;
					}

					rank ++;
				}
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.IsComboOpen )
				{
					this.CloseCombo(CloseMode.Reject);
				}
				
				this.buttonMain.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonMain.Dispose();
				this.buttonMain = null;

				this.buttonMenu.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonMenu.Dispose();
				this.buttonMenu = null;
				
				this.RemoveEvent(CommandState.AdvancedStateProperty, new Types.PropertyChangedEventHandler(this.HandleAdvancedStatePropertyChanged));
			}
			
			base.Dispose(disposing);
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonMain == null )  return;

			Drawing.Rectangle box = this.Client.Bounds;
			Drawing.Rectangle rect;

			rect = box;
			rect.Right -= 12-1;
			this.buttonMain.Bounds = rect;

			rect = box;
			rect.Left = rect.Right-12;
			this.buttonMenu.Bounds = rect;
		}

		protected void UpdateIcon()
		{
			//	Met à jour l'icône dans le bouton, en fonction de la sélection.
			int sel = this.SelectedIndex;
			if ( sel == -1 )  return;
			if ( sel >= this.items.Count )  return;

			int rank = 0;
			foreach ( Item item in this.items )
			{
				if ( item == null )  continue;  // séparateur ?

				if ( rank == sel )
				{
					this.buttonMain.IconName = item.Icon;
					break;
				}

				rank ++;
			}
		}
		
		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu déroulant
			//	est fermé.
			if ( this.IsComboOpen == false )
			{
				if ( this.SelectedIndexChanged != null )
				{
					this.SelectedIndexChanged(this);
				}
			}
		}


		protected virtual bool CheckIfOpenComboRequested(Message message)
		{
			return Feel.Factory.Active.TestComboOpenKey(message);
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseWheel:
					if ( message.Wheel > 0 )
					{
						this.Navigate(-1);
					}
					else if ( message.Wheel < 0 )
					{
						this.Navigate(1);
					}
					
					message.Consumer = this;
					return;
				
				case MessageType.MouseDown:
					this.OpenCombo();
					message.Consumer = this;
					return;
			}
			
			base.ProcessMessage(message, pos);
		}
		
		
		protected virtual int MapComboListToIndex(int value)
		{
			return (value < 0) ? -1 : value;
		}
		
		protected virtual int MapIndexToComboList(int value)
		{
			return (value < 0) ? -1 : value;
		}
		
		
		protected virtual void Navigate(int dir)
		{
			//	Cherche le nom suivant ou précédent dans la comboList, même si elle
			//	n'est pas "déroulée".
			if ( this.items.Count == 0 )
			{
				return;
			}
			
			int	 sel = this.SelectedIndex;

			sel += dir;
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);
			
			this.SelectedIndex = sel;
			this.Focus();
		}
		
		
		protected virtual void OpenCombo()
		{
			//	Rend la liste visible et démarre l'interaction.
			if ( this.IsComboOpen )
			{
				return;
			}
			
			Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
			this.OnComboOpening(cancelEvent);
			
			if ( cancelEvent.Cancel )
			{
				return;
			}
			
			this.menu = this.CreateMenu();
			this.menu.ShowAsComboList(this, this.MapClientToScreen(new Drawing.Point(0, 0)), this.buttonMenu);
			this.menu.Accepted += new Support.EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected += new Support.EventHandler(this.HandleMenuRejected);
			
			this.OnComboOpened();
		}
		
		protected virtual void CloseCombo(CloseMode mode)
		{
			//	Ferme la liste (si nécessaire) et valide/rejette la modification
			//	en fonction du mode spécifié.
			
			if ( this.menu.IsMenuOpen )
			{
				switch ( mode )
				{
					case CloseMode.Reject:
						this.menu.Behavior.Reject();
							return;
					case CloseMode.Accept:
						this.menu.Behavior.Accept();
						return;
				}
			}

			this.menu.Accepted -= new Support.EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected -= new Support.EventHandler(this.HandleMenuRejected);
			
			if ( this.menu != null )
			{
				this.menu.Dispose();
				this.menu = null;
			}
			
			if ( this.AutoFocus )
			{
				this.Focus();
			}
			
			this.OnComboClosed();
			
			//?if ( this.InitialText != this.Text )
			{
				this.OnSelectedIndexChanged();
			}
		}


		protected virtual AbstractMenu CreateMenu()
		{
			VMenu menu = new VMenu();
			
			int rank = 0;
			foreach ( Item item in this.items )
			{
				if ( item == null )  // séparateur ?
				{
					menu.Items.Add(new MenuSeparator());
				}
				else
				{
					MenuItem cell = new MenuItem("", item.Icon, item.Text, "", item.Name);

					if ( rank == this.SelectedIndex )
					{
						cell.ActiveState = ActiveState.Yes;
					}

					menu.Items.Add(cell);
					rank ++;
				}
			}
			
			menu.AdjustSize();
			MenuItem.SetMenuHost(this, new MenuHost(menu));
			
			return menu;
		}
		
		protected virtual void OnComboOpening(Support.CancelEventArgs e)
		{
			if ( this.ComboOpening != null )
			{
				this.ComboOpening(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == true);
			
			if ( this.ComboOpened != null )
			{
				this.ComboOpened(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == false);
			
			if ( this.ComboClosed != null )
			{
				this.ComboClosed(this);
			}
		}
		
		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqué dans le bouton d'ouverture de la liste.
			this.OpenCombo();
		}
		
		private void HandleMenuAccepted(object sender)
		{
			int rank = 0;
			foreach ( MenuItem item in this.menu.Items )
			{
				if ( item == null )  continue;

				if ( item.ItemType == MenuItemType.Selected )
				{
					this.SelectedIndex = rank;
				}

				rank ++;
			}

			this.CloseCombo(CloseMode.Accept);
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo(CloseMode.Reject);
		}
		
		private void HandleAdvancedStatePropertyChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			this.SelectedName = e.NewValue as string;
		}
		
		#region Items
		public class Item
		{
			public Item(string name, string icon, string text)
			{
				this.name = name;
				this.icon = icon;
				this.text = text;
			}

			public string Name
			{
				get { return this.name; }
			}

			public string Icon
			{
				get { return this.icon; }
			}

			public string Text
			{
				get { return this.text; }
			}

			protected string			name;
			protected string			icon;
			protected string			text;
		}
		#endregion


		#region CloseMode Enumeration
		protected enum CloseMode
		{
			Accept,
			Reject
		}
		#endregion
		
		#region MenuHost Class
		public class MenuHost : IMenuHost
		{
			public MenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, ref Drawing.Size size, out Drawing.Point location, out Animation animation)
			{
				//	Détermine la hauteur maximale disponible par rapport à la position
				//	actuelle :
				
				Drawing.Point     pos = Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				Drawing.Point     hot = Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				ScreenInfo        screenInfo = ScreenInfo.Find(hot);
				Drawing.Rectangle workingArea = screenInfo.WorkingArea;
				
				double maxHeight = pos.Y - workingArea.Bottom;
				
				if ( maxHeight > size.Height ||
					 maxHeight > 100         )
				{
					//	Il y a assez de place pour dérouler le menu vers le bas,
					//	mais il faudra peut-être le raccourcir un bout :
					
					this.menu.MaxSize = new Drawing.Size(this.menu.MaxSize.Width, maxHeight);
					this.menu.AdjustSize();
					
					size      = this.menu.Size;
					location  = pos;
					animation = Animation.RollDown;
				}
				else
				{
					//	Il faut dérouler le menu vers le haut.
					
					pos.Y += item.Height;
					
					maxHeight = workingArea.Top - pos.Y;
				
					this.menu.MaxSize = new Drawing.Size(this.menu.MaxSize.Width, maxHeight);
					this.menu.AdjustSize();
					
					pos.Y += this.menu.Height;
					
					size      = this.menu.Size;
					location  = pos;
					animation = Animation.RollUp;
				}
				
				location.X -= this.menu.MenuShadow.Left;
				location.Y -= size.Height;
				
				if ( location.X + size.Width > workingArea.Right )
				{
					location.X = workingArea.Right - size.Width;
				}
			}
			#endregion
			
			private AbstractMenu				menu;
		}
		#endregion
		

		public event Support.CancelEventHandler	ComboOpening;
		public event Support.EventHandler		ComboOpened;
		public event Support.EventHandler		ComboClosed;
		public event Support.EventHandler		SelectedIndexChanged;
		
		protected bool							isLiveUpdateEnabled	= true;
		protected int							selectedIndex = -1;
		protected IconButton					buttonMain;
		protected GlyphButton					buttonMenu;
		protected System.Collections.ArrayList	items;
		protected AbstractMenu					menu;
	}
}
