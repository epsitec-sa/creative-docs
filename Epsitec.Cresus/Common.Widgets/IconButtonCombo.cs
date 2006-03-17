//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButtonCombo impl�mente un IconButton avec bouton "v"
	/// qui fait appara�tre un menu dit "combo" pour permettre de choisir une
	/// ic�ne pr�d�finie.
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
			//	Donne la collection d'�l�ments de type IconButtonCombo.Item.
			get
			{
				return this.items;
			}
		}

		public int								SelectedIndex
		{
			//	Donne le rang de l'�l�ment s�lectionn� (-1 si aucune s�lection).
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
			//	Met � jour l'ic�ne dans le bouton, en fonction de la s�lection.
			int sel = this.SelectedIndex;
			if ( sel == -1 )  return;
			if ( sel >= this.items.Count )  return;

			Item item = this.items[sel] as Item;
			System.Diagnostics.Debug.Assert(item != null);

			this.buttonMain.IconName = item.Icon;
		}
		
		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu d�roulant
			//	est ferm�.
			
			if ( this.IsComboOpen == false )
			{
				if ( this.SelectedIndexChanged != null )
				{
					this.SelectedIndexChanged(this);
				}
			}
		}

		public event Support.EventHandler		SelectedIndexChanged;


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
		
		
		protected virtual void ProcessComboActivatedIndex(int sel)
		{
			//	Cette m�thode n'est appel�e que lorsque le contenu de la liste d�roul�e
			//	est valid�e par un clic de souris, au contraire de ProcessComboSelectedIndex
			//	qui est appel�e � chaque changement "visuel".
			
			int index = this.MapComboListToIndex(sel);
			
			if ( index >= 0 )
			{
				this.SelectedIndex = index;
				this.menu.Behavior.Accept();
			}
		}
		
		protected virtual void ProcessComboSelectedIndex(int sel)
		{
			//	Met � jour le contenu de la combo en cas de changement de s�lection
			//	dans la liste, pour autant qu'une telle mise � jour "live" ait �t�
			//	activ�e.
			
			if ( this.isLiveUpdateEnabled )
			{
				this.SelectedIndex = this.MapComboListToIndex(sel);
			}
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
			//	Cherche le nom suivant ou pr�c�dent dans la comboList, m�me si elle
			//	n'est pas "d�roul�e".
			
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
			//	Rend la liste visible et d�marre l'interaction.
			
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
			
			if ( this.menu != null )
			{
				//?this.scrollList.SelectedIndex = this.MapIndexToComboList(this.SelectedIndex);
				//?this.scrollList.ShowSelected(ScrollShowMode.Center);
			}
			
			this.menu.ShowAsComboList(this, this.MapClientToScreen(new Drawing.Point(0, 0)), this.buttonMenu);
			
			this.menu.Accepted += new Support.EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected += new Support.EventHandler(this.HandleMenuRejected);
			
			if ( this.menu != null )
			{
				//?this.scrollList.SelectedIndexChanged += new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
				//?this.scrollList.SelectionActivated   += new Support.EventHandler(this.HandleScrollListSelectionActivated);
			}
			
			this.OnComboOpened();
		}
		
		protected virtual void CloseCombo(CloseMode mode)
		{
			//	Ferme la liste (si n�cessaire) et valide/rejette la modification
			//	en fonction du mode sp�cifi�.
			
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
				//?this.scrollList.SelectionActivated   -= new Support.EventHandler(this.HandleScrollListSelectionActivated);
				//?this.scrollList.SelectedIndexChanged -= new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
				
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
			
			for ( int i=0 ; i<this.items.Count ; i++ )
			{
				Item item = this.items[i] as Item;
				System.Diagnostics.Debug.Assert(item != null);

				MenuItem cell = new MenuItem("", item.Icon, item.Text, "", item.Name);
				menu.Items.Add(cell);
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
			//	L'utilisateur a cliqu� dans le bouton d'ouverture de la liste.
			this.OpenCombo();
		}
		
		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqu� dans la liste pour terminer son choix.
			//?this.ProcessComboActivatedIndex(this.scrollList.SelectedIndex);
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			//	L'utilisateur a simplement d�plac� la souris dans la liste.
			//?this.ProcessComboSelectedIndex(this.scrollList.SelectedIndex);
		}
		
		private void HandleMenuAccepted(object sender)
		{
			this.CloseCombo(CloseMode.Accept);
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo(CloseMode.Reject);
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
				//	D�termine la hauteur maximale disponible par rapport � la position
				//	actuelle :
				
				Drawing.Point     pos = Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				Drawing.Point     hot = Helpers.VisualTree.MapVisualToScreen(item, new Drawing.Point(0, 1));
				ScreenInfo        screenInfo = ScreenInfo.Find(hot);
				Drawing.Rectangle workingArea = screenInfo.WorkingArea;
				
				double maxHeight = pos.Y - workingArea.Bottom;
				
				if ( maxHeight > size.Height ||
					 maxHeight > 100         )
				{
					//	Il y a assez de place pour d�rouler le menu vers le bas,
					//	mais il faudra peut-�tre le raccourcir un bout :
					
					this.menu.MaxSize = new Drawing.Size(this.menu.MaxSize.Width, maxHeight);
					this.menu.AdjustSize();
					
					size      = this.menu.Size;
					location  = pos;
					animation = Animation.RollDown;
				}
				else
				{
					//	Il faut d�rouler le menu vers le haut.
					
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
		
		protected bool							isLiveUpdateEnabled	= true;
		protected int							selectedIndex = -1;
		protected IconButton					buttonMain;
		protected GlyphButton					buttonMenu;
		protected System.Collections.ArrayList	items;
		protected AbstractMenu					menu;
	}
}
