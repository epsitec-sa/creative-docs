//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Daniel ROUX

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;

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
			this.SetColumnsAndRows(2, 1);
			
			this.buttonPrev = new GlyphButton(this);
			this.buttonPrev.GlyphShape = GlyphShape.ArrowUp;
			this.buttonPrev.ButtonStyle = ButtonStyle.Combo;
			this.buttonPrev.Name = "Prev";
			this.buttonPrev.Pressed += new MessageEventHandler(this.HandleButtonPrevPressed);
			ToolTip.Default.SetToolTip(this.buttonPrev, Res.Strings.IconButtonsCombo.Prev);
			
			this.buttonNext = new GlyphButton(this);
			this.buttonNext.GlyphShape = GlyphShape.ArrowDown;
			this.buttonNext.ButtonStyle = ButtonStyle.Combo;
			this.buttonNext.Name = "Next";
			this.buttonNext.Pressed += new MessageEventHandler(this.HandleButtonNextPressed);
			ToolTip.Default.SetToolTip(this.buttonNext, Res.Strings.IconButtonsCombo.Next);
			
			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.Menu;
			this.buttonMenu.ButtonStyle = ButtonStyle.Combo;
			this.buttonMenu.Name = "Menu";
			this.buttonMenu.Pressed += new MessageEventHandler(this.HandleButtonMenuPressed);
			ToolTip.Default.SetToolTip(this.buttonMenu, Res.Strings.IconButtonsCombo.Menu);
			
			this.AddEventHandler (Widgets.CommandState.AdvancedStateProperty, new PropertyChangedEventHandler(this.HandleAdvancedStatePropertyChanged));
			this.AddEventHandler (Visual.AutoFocusProperty, new PropertyChangedEventHandler (this.HandleAutoFocusChanged));
		}
		
		public IconButtonsCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public void SetColumnsAndRows(int columns, int rows)
		{
			//	D�termine le nombre de colonnes et de lignes.
			//	Le nombre n�cessaire de IconButton est cr��.
			System.Diagnostics.Debug.Assert(columns > 0 && columns <= 10);
			System.Diagnostics.Debug.Assert(rows    > 0 && rows    <= 10);
			this.columns = columns;
			this.rows    = rows;

			this.CreateButtons(this.columns*this.rows);
		}


		public virtual bool						IsComboOpen
		{
			//	Indique si le menu est ouvert.
			get
			{
				return this.menu != null;
			}
		}

		public int								Columns
		{
			//	Donne le nombre de colonnes (1..10).
			get
			{
				return this.columns;
			}
		}

		public int								Rows
		{
			//	Donne le nombre de lignes (1..10).
			get
			{
				return this.rows;
			}
		}

		public bool								IsLiveUpdateEnabled
		{
			//	D�termine comment se comporte le widget lorsque le menu est d�roul�.
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
			//	D�termine si chaque ligne du menu doit avoir un cadre.
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
			//	D�termine si toutes les lignes du menu ont la m�me largeur.
			//	Ceci permet d'acc�l�rer l'ouverture du menu.
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

		public double							ButtonWidth
		{
			//	Donne la largeur d'un bouton.
			get
			{
				return System.Math.Floor((this.Client.Bounds.Width-IconButtonsCombo.menuWidth)/this.columns);
			}
		}

		public double							ButtonHeight
		{
			//	Donne la hauteur d'un bouton.
			get
			{
				return System.Math.Floor(this.Client.Bounds.Height/this.rows);
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
			this.buttonMenu.Enable = (this.items.Count > this.buttonMain.Length);
		}

		public IconButton IconButton(int rank)
		{
			//	Donne un IconButton.
			if ( rank < 0 || rank >= this.buttonMain.Length )
			{
				return null;
			}
			else
			{
				return this.buttonMain[rank];
			}
		}
		

		protected void CreateButtons(int total)
		{
			//	Modifie le nombre de IconButton dans le widget.
			if ( this.buttonMain == null || this.buttonMain.Length != total )
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

				this.buttonMain = new IconButton[total];

				for ( int i=0 ; i<this.buttonMain.Length ; i++ )
				{
					this.buttonMain[i] = new IconButton(this);
					this.buttonMain[i].ButtonStyle = ButtonStyle.ActivableIcon;
					this.buttonMain[i].AutoFocus = this.AutoFocus;
					this.buttonMain[i].Pressed += new MessageEventHandler(this.HandleButtonMainPressed);
				}
			}
		}

		protected void UpdateIcon(int rank, int index)
		{
			//	Met � jour l'ic�ne dans le bouton.
			if ( index < this.items.Count )
			{
				Item item = this.items[index] as Item;
				this.buttonMain[rank].IconName = item.BriefIcon;
				this.buttonMain[rank].PreferredIconSize = new Drawing.Size(this.ButtonWidth, this.ButtonHeight);
				this.buttonMain[rank].ActiveState = (this.SelectedIndex == index) ? ActiveState.Yes : ActiveState.No;
				this.buttonMain[rank].SetSelected(this.SelectedIndex == index);
				ToolTip.Default.SetToolTip(this.buttonMain[rank], item.Tooltip);
			}
			else
			{
				this.buttonMain[rank].IconName = null;
				this.buttonMain[rank].ActiveState = ActiveState.No;
				this.buttonMain[rank].SetSelected(false);
				string tooltip = null;
				ToolTip.Default.SetToolTip(this.buttonMain[rank], tooltip);
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

				this.RemoveEventHandler (Widgets.CommandState.AdvancedStateProperty, new PropertyChangedEventHandler (this.HandleAdvancedStatePropertyChanged));
				this.RemoveEventHandler(Visual.AutoFocusProperty, new PropertyChangedEventHandler(this.HandleAutoFocusChanged));
			}
			
			base.Dispose(disposing);
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonMain == null )  return;

			this.UpdateButtons();

			Drawing.Rectangle box = this.Client.Bounds;
			Drawing.Rectangle rect;

			double width  = this.ButtonWidth;
			double height = this.ButtonHeight;

			double lastRight  = box.Right-IconButtonsCombo.menuWidth;
			double lastBottom = box.Bottom;

			double py = box.Top;

			int i = 0;
			for ( int row=0 ; row<this.rows ; row++ )
			{
				double px = box.Left;

				for ( int column=0 ; column<this.columns ; column++ )
				{
					rect = new Drawing.Rectangle(px, py-height-1, width, height+1);
					if ( column == this.columns-1 )  rect.Right  = lastRight;
					if ( row    == this.rows-1    )  rect.Bottom = lastBottom;
					this.buttonMain[i++].SetManualBounds(rect);

					px += width;
				}

				py -= height;
			}

			rect = box;
			rect.Left = rect.Right-IconButtonsCombo.menuWidth;
			rect.Bottom = rect.Top-System.Math.Floor(rect.Height*0.33);
			this.buttonPrev.SetManualBounds(rect);
			rect.Offset(0, -(rect.Height-1));
			this.buttonNext.SetManualBounds(rect);
			rect.Offset(0, -(rect.Height+1));
			rect.Bottom = box.Bottom;
			this.buttonMenu.SetManualBounds(rect);
		}

		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu d�roulant est ferm�.
			if ( this.IsComboOpen == false )
			{
				EventHandler handler = (EventHandler) this.GetUserEventHandler("SelectedIndexChanged");
				if (handler != null)
				{
					handler(this);
				}
			}
		}

		protected virtual void OnFirstIconChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("FirstIconChanged");
			if (handler != null)
			{
				handler(this);
			}
		}


		protected virtual bool CheckIfOpenComboRequested(Message message)
		{
			return Feel.Factory.Active.TestComboOpenKey(message);
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.MessageType )
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
			
			CancelEventArgs cancelEvent = new CancelEventArgs();
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
			
			this.menu.Accepted += new EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected += new EventHandler(this.HandleMenuRejected);
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectedIndexChanged += new EventHandler(this.HandleScrollerSelectedIndexChanged);
				this.scrollList.SelectionActivated   += new EventHandler(this.HandleScrollListSelectionActivated);
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

			this.menu.Accepted -= new EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected -= new EventHandler(this.HandleMenuRejected);
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectionActivated   -= new EventHandler(this.HandleScrollListSelectionActivated);
				this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollerSelectedIndexChanged);
				
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

			menu.MinWidth = this.ActualWidth;
			
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.DrawFrame = this.menuDrawFrame;
			this.scrollList.AllLinesWidthSameWidth = this.allLinesWidthSameWidth;
			
			menu.Contents = this.scrollList;
			
			//	Remplit la liste :
			int rank = 0;
			foreach (Item item in this.items)
			{
				if (item == null)
				{
					continue;  // s�parateur ?
				}

				bool sel = (rank == this.SelectedIndex);
				this.scrollList.Items.Add (item.Name, sel ? item.SelectedText : item.RegularText);
				rank++;
			}

			TextFieldCombo.AdjustScrollListWidth (this.scrollList);
			TextFieldCombo.AdjustComboSize (this, menu);
			
			return menu;
		}
		
		protected virtual void OnComboOpening(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = (EventHandler<CancelEventArgs>) this.GetUserEventHandler("ComboOpening");
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == true);

			EventHandler handler = (EventHandler) this.GetUserEventHandler("ComboOpened");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == false);

			EventHandler handler = (EventHandler) this.GetUserEventHandler("ComboClosed");
			if (handler != null)
			{
				handler(this);
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
		
		private void HandleAdvancedStatePropertyChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			this.SelectedName = e.NewValue as string;
		}

		private void HandleAutoFocusChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
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

			public Item(string name, string briefIcon, string regularText, string selectedText, string tooltip)
			{
				this.name         = name;
				this.briefIcon    = briefIcon;
				this.regularText  = regularText;
				this.selectedText = selectedText;
				this.tooltip      = tooltip;
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

			public string Tooltip
			{
				get { return this.tooltip; }
			}

			protected string			name;
			protected string			briefIcon;
			protected string			regularText;
			protected string			selectedText;
			protected string			tooltip;
		}
		#endregion

		#region CloseMode Enumeration
		protected enum CloseMode
		{
			Accept,
			Reject
		}
		#endregion
		

		public event EventHandler<CancelEventArgs> ComboOpening
		{
			add
			{
				this.AddUserEventHandler("ComboOpening", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ComboOpening", value);
			}
		}

		public event EventHandler				ComboOpened
		{
			add
			{
				this.AddUserEventHandler("ComboOpened", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ComboOpened", value);
			}
		}

		public event EventHandler				ComboClosed
		{
			add
			{
				this.AddUserEventHandler("ComboClosed", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ComboClosed", value);
			}
		}

		public event EventHandler				SelectedIndexChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedIndexChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedIndexChanged", value);
			}
		}

		public event EventHandler				FirstIconChanged
		{
			add
			{
				this.AddUserEventHandler("FirstIconChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("FirstIconChanged", value);
			}
		}

		
		protected static readonly double		menuWidth = 12;
		protected bool							isLiveUpdateEnabled	= true;
		protected bool							menuDrawFrame = false;
		protected bool							allLinesWidthSameWidth = false;
		protected int							selectedIndex = -1;
		protected int							firstIconVisible = 0;
		protected int							columns;
		protected int							rows;
		protected IconButton[]					buttonMain;
		protected GlyphButton					buttonPrev;
		protected GlyphButton					buttonNext;
		protected GlyphButton					buttonMenu;
		protected System.Collections.ArrayList	items;
		protected AbstractMenu					menu;
		protected ScrollList					scrollList;
	}
}
