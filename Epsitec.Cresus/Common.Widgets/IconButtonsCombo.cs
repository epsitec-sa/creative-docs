//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButtonsCombo impl�mente une liste de IconButton avec bouton "v"
	/// qui fait appara�tre un menu dit "combo" pour permettre de choisir une
	/// ic�ne pr�d�finie.
	/// </summary>
	public class IconButtonsCombo : Widget
	{
		public IconButtonsCombo()
		{
			this.items = new System.Collections.ArrayList();

			this.TotalButtons = 2;
			
			this.buttonPrev = new GlyphButton(this);
			this.buttonPrev.GlyphShape = GlyphShape.ArrowUp;
			this.buttonPrev.ButtonStyle = ButtonStyle.Combo;
			this.buttonPrev.Name = "Prev";
			this.buttonPrev.Pressed += new MessageEventHandler(this.HandleButtonPrevPressed);
			
			this.buttonNext = new GlyphButton(this);
			this.buttonNext.GlyphShape = GlyphShape.ArrowDown;
			this.buttonNext.ButtonStyle = ButtonStyle.Combo;
			this.buttonNext.Name = "Next";
			this.buttonNext.Pressed += new MessageEventHandler(this.HandleButtonNextPressed);
			
			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.Menu;
			this.buttonMenu.ButtonStyle = ButtonStyle.Combo;
			this.buttonMenu.Name = "Menu";
			this.buttonMenu.Pressed += new MessageEventHandler(this.HandleButtonMenuPressed);
			
			this.AddEvent(CommandState.AdvancedStateProperty, new Types.PropertyChangedEventHandler(this.HandleAdvancedStatePropertyChanged));
			this.AddEvent(Visual.AutoFocusProperty, new Types.PropertyChangedEventHandler(this.HandleAutoFocusChanged));
		}
		
		public IconButtonsCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double					DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return base.DefaultWidth + IconButtonsCombo.menuWidth;
			}
		}

		public virtual bool						IsComboOpen
		{
			get
			{
				return this.menu != null;
			}
		}

		public int								TotalButtons
		{
			//	Nombre de IconButton dans le widget.
			get
			{
				if ( this.buttonMain == null )  return 0;
				return this.buttonMain.Length;
			}

			set
			{
				if ( this.buttonMain == null || this.buttonMain.Length != value )
				{
					if ( this.buttonMain != null )
					{
						for ( int i=0 ; i<this.buttonMain.Length ; i++ )
						{
							this.buttonMain[i].Pressed -= new MessageEventHandler(this.HandleButtonMainPressed);
							this.buttonMain[i].Dispose();
							this.buttonMain[i] = null;
						}
					}

					this.buttonMain = new IconButton[value];

					for ( int i=0 ; i<this.buttonMain.Length ; i++ )
					{
						this.buttonMain[i] = new IconButton(this);
						this.buttonMain[i].ButtonStyle = ButtonStyle.ActivableIcon;
						this.buttonMain[i].AutoFocus = this.AutoFocus;
						this.buttonMain[i].Pressed += new MessageEventHandler(this.HandleButtonMainPressed);
					}
				}
			}
		}

		public IconButton IconButton(int rank)
		{
			return this.buttonMain[rank];
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
		
		public bool								MenuDrawFrame
		{
			get
			{
				return this.menuDrawFrame;
			}

			set
			{
				this.menuDrawFrame = value;
			}
		}
		
		public bool								AllLinesWidthSameWidth
		{
			get
			{
				return this.allLinesWidthSameWidth;
			}

			set
			{
				this.allLinesWidthSameWidth = value;
			}
		}
		
		public System.Collections.ArrayList		Items
		{
			//	Donne la collection d'�l�ments de type IconButtonsCombo.Item.
			get
			{
				return this.items;
			}
		}

		public int								SelectedIndex
		{
			//	Rang de l'�l�ment s�lectionn� (-1 si aucune s�lection).
			get
			{
				return this.selectedIndex;
			}

			set
			{
				if ( this.selectedIndex != value )
				{
					this.selectedIndex = value;
					this.UpdateButtons();
					this.OnSelectedIndexChanged();
				}
			}
		}

		public string							SelectedName
		{
			//	Donne le nom de l'�l�ment s�lectionn� (null si aucune s�lection).
			get
			{
				int sel = this.SelectedIndex;
				if ( sel == -1 )  return null;

				int i = 0;
				foreach ( Item item in this.items )
				{
					if ( item == null )  continue;  // s�parateur ?

					if ( i == sel )
					{
						return item.Name;
					}

					i ++;
				}

				return null;
			}

			set
			{
				int i = 0;
				foreach ( Item item in this.items )
				{
					if ( item == null )  continue;  // s�parateur ?

					if ( item.Name == value )
					{
						this.SelectedIndex = i;
						return;
					}

					i ++;
				}

				this.SelectedIndex = -1;
			}
		}


		public void UpdateButtons()
		{
			//	Met � jour tous les boutons en fonctions de la liste Items.
			for ( int i=0 ; i<this.buttonMain.Length ; i++ )
			{
				this.UpdateIcon(i, this.FirstIconVisible+i);
			}

			int first = this.FirstIconVisible;
			this.buttonPrev.Enable = (first > 0);
			this.buttonNext.Enable = (first < ((this.items.Count-1)/this.buttonMain.Length)*this.buttonMain.Length);
		}

		protected void UpdateIcon(int rank, int index)
		{
			//	Met � jour l'ic�ne dans le bouton.
			if ( index < this.items.Count )
			{
				Item item = this.items[index] as Item;
				this.buttonMain[rank].IconName = item.BriefIcon;
				this.buttonMain[rank].ActiveState = (this.SelectedIndex == index) ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.buttonMain[rank].IconName = null;
				this.buttonMain[rank].ActiveState = ActiveState.No;
			}
		}
		

		public int								FirstIconVisible
		{
			//	Rang de la premi�re ic�ne visible, selon les boutons pr�c�dent/suivant.
			get
			{
				return this.firstIconVisible;
			}

			set
			{
				if ( this.firstIconVisible != value )
				{
					this.firstIconVisible = value;
					this.UpdateButtons();
					this.OnFirstIconChanged();
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
				
				for ( int i=0 ; i<this.buttonMain.Length ; i++ )
				{
					this.buttonMain[i].Pressed -= new MessageEventHandler(this.HandleButtonMainPressed);
					this.buttonMain[i].Dispose();
					this.buttonMain[i] = null;
				}

				this.buttonPrev.Pressed -= new MessageEventHandler(this.HandleButtonPrevPressed);
				this.buttonPrev.Dispose();
				this.buttonPrev = null;
				
				this.buttonNext.Pressed -= new MessageEventHandler(this.HandleButtonNextPressed);
				this.buttonNext.Dispose();
				this.buttonNext = null;
				
				this.buttonMenu.Pressed -= new MessageEventHandler(this.HandleButtonMenuPressed);
				this.buttonMenu.Dispose();
				this.buttonMenu = null;
				
				this.RemoveEvent(CommandState.AdvancedStateProperty, new Types.PropertyChangedEventHandler(this.HandleAdvancedStatePropertyChanged));
				this.RemoveEvent(Visual.AutoFocusProperty, new Types.PropertyChangedEventHandler(this.HandleAutoFocusChanged));
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
			rect.Right -= IconButtonsCombo.menuWidth;
			double last = rect.Right;
			rect.Width = System.Math.Floor(rect.Width/this.buttonMain.Length);
			for ( int i=0 ; i<this.buttonMain.Length ; i++ )
			{
				if ( i == this.buttonMain.Length-1 )
				{
					rect.Right = last;
				}

				this.buttonMain[i].Bounds = rect;
				rect.Offset(rect.Width, 0);
			}

			rect = box;
			rect.Left = rect.Right-IconButtonsCombo.menuWidth;
			rect.Bottom = rect.Top-System.Math.Floor(rect.Height*0.33);
			this.buttonPrev.Bounds = rect;
			rect.Offset(0, -(rect.Height-1));
			this.buttonNext.Bounds = rect;
			rect.Offset(0, -(rect.Height+1));
			rect.Bottom = box.Bottom;
			this.buttonMenu.Bounds = rect;
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

		protected virtual void OnFirstIconChanged()
		{
			if ( this.FirstIconChanged != null )
			{
				this.FirstIconChanged(this);
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
			this.menu.ShowAsComboList(this, this.MapClientToScreen(new Drawing.Point(0, 0)), this.buttonMenu);
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectedIndex = this.MapIndexToComboList(this.SelectedIndex);
				this.scrollList.ShowSelected(ScrollShowMode.Center);
			}
			
			this.menu.Accepted += new Support.EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected += new Support.EventHandler(this.HandleMenuRejected);
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectedIndexChanged += new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
				this.scrollList.SelectionActivated   += new Support.EventHandler(this.HandleScrollListSelectionActivated);
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
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectionActivated   -= new Support.EventHandler(this.HandleScrollListSelectionActivated);
				this.scrollList.SelectedIndexChanged -= new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
				
				this.scrollList.Dispose();
				this.scrollList = null;
			}
			
			this.menu.Dispose();
			this.menu = null;
			
			if ( this.AutoFocus )
			{
				this.Focus();
			}
			
			this.OnComboClosed();
			this.OnSelectedIndexChanged();
		}


		protected virtual AbstractMenu CreateMenu()
		{
			TextFieldComboMenu menu = new TextFieldComboMenu();
			
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.DrawFrame = this.menuDrawFrame;
			this.scrollList.AllLinesWidthSameWidth = this.allLinesWidthSameWidth;
			
			menu.Contents = this.scrollList;
			
			//	Remplit la liste :
			int rank = 0;
			foreach ( Item item in this.items )
			{
				if ( item == null )  continue;  // s�parateur ?

				bool sel = (rank == this.SelectedIndex);
				this.scrollList.Items.Add(item.Name, sel ? item.SelectedText : item.RegularText);
				rank ++;
			}

			Drawing.Size size = this.scrollList.GetBestSize();
			this.scrollList.LineHeight = size.Height;
			menu.Size = new Drawing.Size(size.Width, 200);
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
		
		
		private void HandleButtonMainPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqu� dans l'un des bouton.
			for ( int i=0 ; i<this.buttonMain.Length ; i++ )
			{
				if ( sender == this.buttonMain[i] )
				{
					i += this.firstIconVisible;
					if ( i < this.items.Count )
					{
						this.SelectedIndex = i;
					}
				}
			}
		}
		
		private void HandleButtonPrevPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqu� dans le bouton 'pr�c�dent'.
			int first = this.FirstIconVisible;
			first -= this.buttonMain.Length;
			first = System.Math.Max(first, 0);
			this.FirstIconVisible = first;
		}
		
		private void HandleButtonNextPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqu� dans le bouton 'suivant'.
			int first = this.FirstIconVisible;
			first += this.buttonMain.Length;
			first = System.Math.Min(first, ((this.items.Count-1)/this.buttonMain.Length)*this.buttonMain.Length);
			this.FirstIconVisible = first;
		}
		
		private void HandleButtonMenuPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqu� dans le bouton d'ouverture du menu-combo.
			this.OpenCombo();
		}
		
		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqu� dans la liste pour terminer son choix.
			this.ProcessComboActivatedIndex(this.scrollList.SelectedIndex);
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			//	L'utilisateur a simplement d�plac� la souris dans la liste.
			this.ProcessComboSelectedIndex(this.scrollList.SelectedIndex);
		}

		private void HandleMenuAccepted(object sender)
		{
			this.CloseCombo(CloseMode.Accept);
			this.ExecuteCommand();
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo(CloseMode.Reject);
		}
		
		private void HandleAdvancedStatePropertyChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			this.SelectedName = e.NewValue as string;
		}

		private void HandleAutoFocusChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			bool autoFocus = (bool) e.NewValue;

			for ( int i=0 ; i<this.buttonMain.Length ; i++ )
			{
				this.buttonMain[i].AutoFocus = autoFocus;
			}

			this.buttonPrev.AutoFocus = autoFocus;
			this.buttonNext.AutoFocus = autoFocus;
			this.buttonMenu.AutoFocus = autoFocus;
		}
		
		#region Items
		public class Item
		{
			//	Les instances de cette classe servent � peupler la liste Items de IconButtonsCombo.
			//	Chaque instance correspond � une ligne du menu-combo.

			public Item(string name, string briefIcon, string regularText, string selectedText)
			{
				this.name         = name;
				this.briefIcon    = briefIcon;
				this.regularText  = regularText;
				this.selectedText = selectedText;
			}

			public string Name
			{
				get { return this.name; }
			}

			public string BriefIcon
			{
				get { return this.briefIcon; }
			}

			public string RegularText
			{
				get { return this.regularText; }
			}

			public string SelectedText
			{
				get { return this.selectedText; }
			}

			protected string			name;
			protected string			briefIcon;
			protected string			regularText;
			protected string			selectedText;
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
				double scrollWidth = 0;
				
				if ( maxHeight > size.Height ||
					 maxHeight > 100         )
				{
					//	Il y a assez de place pour d�rouler le menu vers le bas,
					//	mais il faudra peut-�tre le raccourcir un bout :
					
					if ( this.menu.Height > maxHeight )  // pas assez de place en hauteur ?
					{
						scrollWidth = 17;  // place pour l'ascenseur � droite
					}

					this.menu.MaxSize = new Drawing.Size(this.menu.MaxSize.Width, maxHeight);
					this.menu.AdjustSize();
					
					size      = this.menu.Size;
					location  = pos;
					animation = Animation.RollDown;
				}
				else
				{
					//	Il faut d�rouler le menu vers le haut.
					
					pos.Y += item.Height-2;
					
					maxHeight = workingArea.Top - pos.Y;
				
					if ( this.menu.Height > maxHeight )  // pas assez de place en hauteur ?
					{
						scrollWidth = 17;  // place pour l'ascenseur � droite
					}

					this.menu.MaxSize = new Drawing.Size(this.menu.MaxSize.Width, maxHeight);
					this.menu.AdjustSize();
					
					pos.Y += this.menu.Height;
					
					size      = this.menu.Size;
					location  = pos;
					animation = Animation.RollUp;
				}
				
				size.Width += scrollWidth;

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
		public event Support.EventHandler		FirstIconChanged;
		
		protected static readonly double		menuWidth = 12;
		protected bool							isLiveUpdateEnabled	= true;
		protected bool							menuDrawFrame = false;
		protected bool							allLinesWidthSameWidth = false;
		protected int							selectedIndex = -1;
		protected int							firstIconVisible = 0;
		protected IconButton[]					buttonMain;
		protected GlyphButton					buttonPrev;
		protected GlyphButton					buttonNext;
		protected GlyphButton					buttonMenu;
		protected System.Collections.ArrayList	items;
		protected AbstractMenu					menu;
		protected ScrollList					scrollList;
	}
}
