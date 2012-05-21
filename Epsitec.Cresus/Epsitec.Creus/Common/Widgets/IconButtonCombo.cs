//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler=Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;

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
			this.buttonMain.Pressed += this.HandleButtonPressed;

			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.Menu;
			this.buttonMenu.ButtonStyle = ButtonStyle.Combo;
			this.buttonMenu.Name = "Open";
			this.buttonMenu.Pressed += this.HandleButtonPressed;

			this.AddEventHandler (Widgets.CommandState.AdvancedStateProperty, this.HandleAdvancedStatePropertyChanged);
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

		public IconButton						IconButton
		{
			get
			{
				return this.buttonMain;
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
			//	Donne la collection d'éléments de type IconButtonCombo.Item.
			get
			{
				return this.items;
			}
		}

		public int								SelectedItemIndex
		{
			//	Donne le rang de l'élément sélectionné (-1 si aucune sélection).
			get
			{
				return this.selectedItemIndex;
			}

			set
			{
				if ( this.selectedItemIndex != value )
				{
					this.selectedItemIndex = value;
					this.UpdateIcon();
					this.OnSelectedItemChanged ();
				}
			}
		}

		public string							SelectedName
		{
			//	Donne le nom de l'élément sélectionné (null si aucune sélection).
			get
			{
				int sel = this.SelectedItemIndex;
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
						this.SelectedItemIndex = rank;
						return;
					}

					rank ++;
				}

				this.SelectedItemIndex = -1;
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
				
				this.buttonMain.Pressed -= this.HandleButtonPressed;
				this.buttonMain.Dispose();
				this.buttonMain = null;

				this.buttonMenu.Pressed -= this.HandleButtonPressed;
				this.buttonMenu.Dispose();
				this.buttonMenu = null;

				this.RemoveEventHandler (Widgets.CommandState.AdvancedStateProperty, this.HandleAdvancedStatePropertyChanged);
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
			rect.Right -= IconButtonCombo.menuWidth-1;
			this.buttonMain.SetManualBounds(rect);

			rect = box;
			rect.Left = rect.Right-IconButtonCombo.menuWidth;
			this.buttonMenu.SetManualBounds(rect);
		}

		protected void UpdateIcon()
		{
			//	Met à jour l'icône dans le bouton, en fonction de la sélection.
			int sel = this.SelectedItemIndex;

			if ( sel == -1 )
			{
				this.buttonMain.IconUri = null;
				return;
			}

			if ( sel >= this.items.Count )  return;

			int rank = 0;
			foreach ( Item item in this.items )
			{
				if ( item == null )  continue;  // séparateur ?

				if ( rank == sel )
				{
					this.buttonMain.IconUri = item.BriefIcon;
					break;
				}

				rank ++;
			}
		}


		protected virtual void OnSelectedItemChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu déroulant
			//	est fermé.
			if ( this.IsComboOpen == false )
			{
				var handler = this.GetUserEventHandler ("SelectedItemChanged");
				if (handler != null)
				{
					handler(this);
				}
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
				
				case MessageType.MouseDown:
					this.OpenCombo();
					message.Consumer = this;
					return;
			}
			
			base.ProcessMessage(message, pos);
		}
		
		protected virtual void ProcessComboActivatedIndex(int sel)
		{
			//	Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			//	est validée par un clic de souris, au contraire de ProcessComboSelectedIndex
			//	qui est appelée à chaque changement "visuel".
			int index = this.MapComboListToIndex(sel);
			
			if ( index >= 0 )
			{
				this.SelectedItemIndex = index;
				this.menu.Behavior.Accept();
			}
		}
		
		protected virtual void ProcessComboSelectedIndex(int sel)
		{
			//	Met à jour le contenu de la combo en cas de changement de sélection
			//	dans la liste, pour autant qu'une telle mise à jour "live" ait été
			//	activée.
			if ( this.isLiveUpdateEnabled )
			{
				this.SelectedItemIndex = this.MapComboListToIndex (sel);
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
			//	Cherche le nom suivant ou précédent dans la comboList, même si elle
			//	n'est pas "déroulée".
			if ( this.items.Count == 0 )
			{
				return;
			}

			int	 sel = this.SelectedItemIndex;

			sel += dir;
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);

			this.SelectedItemIndex = sel;
			this.Focus();
		}
		
		
		protected virtual void OpenCombo()
		{
			//	Rend la liste visible et démarre l'interaction.
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
				this.scrollList.SelectedItemIndex = this.MapIndexToComboList (this.SelectedItemIndex);
				this.scrollList.ShowSelected(ScrollShowMode.Center);
			}
			
			this.menu.Accepted += this.HandleMenuAccepted;
			this.menu.Rejected += this.HandleMenuRejected;
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;
				this.scrollList.SelectionActivated   += this.HandleScrollListSelectionActivated;
			}
			
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

			this.menu.Accepted -= this.HandleMenuAccepted;
			this.menu.Rejected -= this.HandleMenuRejected;
			
			if ( this.scrollList != null )
			{
				this.scrollList.SelectionActivated   -= this.HandleScrollListSelectionActivated;
				this.scrollList.SelectedItemChanged -= this.HandleScrollListSelectedItemChanged;
				
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
			this.OnSelectedItemChanged();
		}


		protected virtual AbstractMenu CreateMenu()
		{
			TextFieldComboMenu menu = new TextFieldComboMenu();
			
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.DrawFrame = this.menuDrawFrame;
			this.scrollList.AllRowsHaveSameWidth = this.allLinesWidthSameWidth;
			
			menu.Contents = this.scrollList;
			
			//	Remplit la liste :
			int rank = 0;
			foreach ( Item item in this.items )
			{
				if ( item == null )  continue;  // séparateur ?

				bool sel = (rank == this.SelectedItemIndex);
				this.scrollList.Items.Add(item.Name, sel ? item.SelectedText : item.RegularText);
				rank ++;
			}

			TextFieldCombo.AdjustScrollListWidth (this.scrollList);
			TextFieldCombo.AdjustComboSize (this, menu, false);
			
			return menu;
		}
		
		protected virtual void OnComboOpening(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = this.GetUserEventHandler<CancelEventArgs> ("ComboOpening");
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == true);

			var handler = this.GetUserEventHandler("ComboOpened");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == false);

			var handler = this.GetUserEventHandler("ComboClosed");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqué dans le bouton d'ouverture de la liste.
			this.OpenCombo();
		}
		
		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			this.ProcessComboActivatedIndex (this.scrollList.SelectedItemIndex);
		}

		private void HandleScrollListSelectedItemChanged(object sender)
		{
			//	L'utilisateur a simplement déplacé la souris dans la liste.
			this.ProcessComboSelectedIndex(this.scrollList.SelectedItemIndex);
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
		
		#region Items
		public class Item
		{
			//	Les instances de cette classe servent à peupler la liste Items de IconButtonCombo.
			//	Chaque instance correspond à une ligne du menu-combo.

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

		public event EventHandler				SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectedItemChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedItemChanged", value);
			}
		}

		
		protected static readonly double		menuWidth = 12;
		protected bool							isLiveUpdateEnabled	= true;
		protected bool							menuDrawFrame = false;
		protected bool							allLinesWidthSameWidth = false;
		protected int							selectedItemIndex = -1;
		protected IconButton					buttonMain;
		protected GlyphButton					buttonMenu;
		protected System.Collections.ArrayList	items;
		protected AbstractMenu					menu;
		protected ScrollList					scrollList;
	}
}
