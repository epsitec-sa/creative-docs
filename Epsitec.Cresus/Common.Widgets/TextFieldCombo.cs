//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v"
	/// qui fait apparaître un menu dit "combo" pour permettre de choisir une
	/// option prédéfinie.
	/// </summary>
	public class TextFieldCombo : AbstractTextField, Collections.IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public TextFieldCombo()
		{
			this.textFieldStyle = TextFieldStyle.Combo;
			
			this.select_item_behavior = new Behaviors.SelectItemBehavior (new Behaviors.SelectItemCallback (this.AutomaticItemSelection));

			this.items = new Collections.StringCollection (this);
			this.items.AcceptsRichText = true;
			
			this.button = this.CreateButton ();
			
			this.button.Name     = "Open";
			this.button.Pressed += new MessageEventHandler (this.HandleButtonPressed);
			
			this.default_button_width = this.button.PreferredWidth;
			this.margins.Right        = this.button.PreferredWidth;
		}
		
		public TextFieldCombo(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected virtual Button CreateButton()
		{
			GlyphButton button = new GlyphButton(this);
			
			button.GlyphShape  = GlyphShape.Menu;
			button.ButtonStyle = ButtonStyle.Combo;
			
			return button;
		}
		
		
		public Button							Button
		{
			get
			{
				return this.button;
			}
		}
		
		public ShowCondition					ButtonShowCondition
		{
			get
			{
				return this.button_show_condition;
			}
			set
			{
				if (this.button_show_condition != value)
				{
					this.button_show_condition = value;
					this.UpdateButtonVisibility ();
				}
			}
		}
		
		
		public override bool					IsCombo
		{
			get
			{
				return true;
			}
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
				return this.is_live_update_enabled;
			}
			set
			{
				this.is_live_update_enabled = value;
			}
		}
		
		public ComboArrowMode					ComboArrowMode
		{
			get
			{
				return this.combo_arrow_mode;
			}
			set
			{
				this.combo_arrow_mode = value;
			}
		}
		
		
		protected ScrollList					ScrollList
		{
			get
			{
				return this.scroll_list;
			}
		}
		
		
		public bool FindMatch(string find, out int index, out bool exact_match)
		{
			//	Trouve l'index de l'élément recherché. Indique s'il s'agit d'un
			//	match exact ('find' est égal au contenu de la cellule) ou non
			//	('find' correspond au début de la cellule).
			
			index = this.items.FindExactMatch (find);
			
			if (index < 0)
			{
				exact_match = false;
				
				if (find == "")
				{
					return false;
				}
				
				index = this.items.FindStartMatch (find);
				
				if (index < 0)
				{
					return false;
				}
			}
			else
			{
				exact_match = true;
			}
			
			return true;
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.IsComboOpen)
				{
					this.CloseCombo (CloseMode.Reject);
				}
				
				this.button.Pressed -= new MessageEventHandler (this.HandleButtonPressed);
				this.button.Dispose ();
				this.button = null;
			}
			
			base.Dispose(disposing);
		}

		
		protected override void UpdateButtonGeometry()
		{
			//	Met à jour la position du bouton; la marge droite de la ligne
			//	éditable est ajustée pour tenir compte de la présence (ou non)
			//	du bouton.
			
			base.UpdateButtonGeometry();
			
			if (this.button != null)
			{
				this.margins.Right = this.button.Visibility ? this.default_button_width : 0;
				this.button.SetManualBounds(this.GetButtonBounds());
			}
		}
		
		protected override void UpdateButtonVisibility()
		{
			this.SetButtonVisibility (this.ComputeButtonVisibility ());
		}
		
		
		protected bool ComputeButtonVisibility()
		{
			bool show = false;
			
			switch (this.button_show_condition)
			{
				case ShowCondition.Always:				show = true;										break;
				case ShowCondition.Never:				show = false;										break;
				case ShowCondition.WhenFocused:			show = this.IsFocused         || this.IsComboOpen;	break;
				case ShowCondition.WhenKeyboardFocused:	show = this.KeyboardFocus || this.IsComboOpen;	break;
				case ShowCondition.WhenModified:		show = this.HasEditedText;							break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.button_show_condition));
			}
			
			return show;
		}
		
		protected void SetButtonVisibility(bool visibility)
		{
			if (this.button != null)
			{
				if (this.button.Visibility != visibility)
				{
					this.button.Visibility = visibility;
					
					this.UpdateButtonGeometry ();
					this.UpdateTextLayout ();
					this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
				}
			}
		}
		
		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu déroulant
			//	est fermé.
			
			if (this.IsComboOpen == false)
			{
				EventHandler handler = (EventHandler) this.GetUserEventHandler("SelectedIndexChanged");

				if (handler != null)
				{
					handler(this);
				}
			}
		}

		protected virtual void AutomaticItemSelection(string search, bool continued)
		{
			int  index;
			bool exact;
			
			if (this.FindMatch (search, out index, out exact))
			{
				this.SelectedIndex = index;
			}
		}

		protected virtual bool CheckIfOpenComboRequested(Message message)
		{
			return Feel.Factory.Active.TestComboOpenKey (message);
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseWheel:
					if (message.Wheel > 0)
					{
						this.Navigate(-1);
					}
					else if (message.Wheel < 0)
					{
						this.Navigate(1);
					}
					
					message.Consumer = this;
					return;
				
				case MessageType.MouseDown:
					if (this.IsReadOnly)
					{
						this.OpenCombo ();
						message.Consumer = this;
						return;
					}
					break;
			}
			
			base.ProcessMessage(message, pos);
		}
		
		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			//	Gère les pressions de touches (en particulier les flèches haut
			//	et bas qui permettent soit d'ouvrir un combo, soit de cycler le
			//	contenu).
			
			switch (this.ComboArrowMode)
			{
				case ComboArrowMode.None:
					break;
				
				case ComboArrowMode.Cycle:
					switch (message.KeyCode)
					{
						case KeyCode.ArrowUp:	this.Navigate (-1);	return true;
						case KeyCode.ArrowDown:	this.Navigate (1);	return true;
					}
					break;
				
				case ComboArrowMode.Open:
					if (this.CheckIfOpenComboRequested (message))
					{
						this.OpenCombo ();
						return true;
					}
					break;
			}
			
			if ((this.IsComboOpen) &&
				(Feel.Factory.Active.TestCancelKey (message)))
			{
				this.menu.Behavior.Reject ();
				return true;
			}
			
			return base.ProcessKeyDown(message, pos);
		}
		
		protected override bool ProcessKeyPress(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if ((this.IsReadOnly) &&
				(this.ProcessKeyPressInSelectItemBehavior (message)))
			{
				return true;
			}
			
			return base.ProcessKeyPress (message, pos);
		}

		
		protected virtual bool ProcessKeyPressInSelectItemBehavior(Message message)
		{
			return this.select_item_behavior.ProcessKeyPress (message);
		}
		
		protected virtual void ProcessComboActivatedIndex(int sel)
		{
			//	Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			//	est validée par un clic de souris, au contraire de ProcessComboSelectedIndex
			//	qui est appelée à chaque changement "visuel".
			
			int index = this.MapComboListToIndex (sel);
			
			if (index >= 0)
			{
				this.SelectedIndex = index;
				this.menu.Behavior.Accept ();
			}
		}
		
		protected virtual void ProcessComboSelectedIndex(int sel)
		{
			//	Met à jour le contenu de la combo en cas de changement de sélection
			//	dans la liste, pour autant qu'une telle mise à jour "live" ait été
			//	activée.
			
			if (this.is_live_update_enabled)
			{
				this.SelectedIndex = this.MapComboListToIndex (sel);
			}
		}
		
		
		protected virtual void CopyItemsToComboList(Collections.StringCollection list)
		{
			for (int i = 0 ; i < this.items.Count; i++)
			{
				string name = this.items.GetName (i);
				string text = this.items[i];
				
				list.Add (name, text);
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
			
			if (this.items.Count == 0)
			{
				return;
			}
			
			int	 sel;
			bool exact;

			if (this.FindMatch (this.Text, out sel, out exact))
			{
				if (exact)
				{
					sel += dir;
				}
			}
			
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);
			
			this.SelectedIndex = sel;
			this.Focus ();
		}
		
		
		protected virtual void OpenCombo()
		{
			//	Rend la liste visible et démarre l'interaction.
			
			if (this.IsComboOpen)
			{
				return;
			}
			
			Support.CancelEventArgs cancel_event = new Support.CancelEventArgs ();
			this.OnComboOpening (cancel_event);
			
			if (cancel_event.Cancel)
			{
				return;
			}
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.menu = this.CreateMenu ();

			if (this.menu == null)
			{
				return;
			}
			
			this.menu.ShowAsComboList (this, this.MapClientToScreen (new Drawing.Point (0, 0)), this.Button);
			
			if (this.scroll_list != null)
			{
				this.scroll_list.SelectedIndex = this.MapIndexToComboList (this.SelectedIndex);
				this.scroll_list.ShowSelected (ScrollShowMode.Center);
			}
			
			this.menu.Accepted += new EventHandler (this.HandleMenuAccepted);
			this.menu.Rejected += new EventHandler (this.HandleMenuRejected);
			
			if (this.scroll_list != null)
			{
				this.scroll_list.SelectedIndexChanged += new EventHandler(this.HandleScrollerSelectedIndexChanged);
				this.scroll_list.SelectionActivated   += new EventHandler(this.HandleScrollListSelectionActivated);
			}
			
			this.StartEdition ();
			this.OnComboOpened ();
		}
		
		protected virtual void CloseCombo(CloseMode mode)
		{
			//	Ferme la liste (si nécessaire) et valide/rejette la modification
			//	en fonction du mode spécifié.
			
			if (this.menu.IsMenuOpen)
			{
				switch (mode)
				{
					case CloseMode.Reject:
						this.menu.Behavior.Reject ();
							return;
					case CloseMode.Accept:
						this.menu.Behavior.Accept ();
						return;
				}
			}

			this.menu.Accepted -= new EventHandler(this.HandleMenuAccepted);
			this.menu.Rejected -= new EventHandler(this.HandleMenuRejected);
			
			if (this.scroll_list != null)
			{
				this.scroll_list.SelectionActivated   -= new EventHandler(this.HandleScrollListSelectionActivated);
				this.scroll_list.SelectedIndexChanged -= new EventHandler(this.HandleScrollerSelectedIndexChanged);
				
				this.scroll_list.Dispose();
				this.scroll_list = null;
			}
			
			this.menu.Dispose ();
			this.menu = null;
			
			this.SelectAll ();
			
			if (this.AutoFocus)
			{
				this.Focus ();
			}
			
			switch (mode)
			{
				case CloseMode.Reject:	this.RejectEdition ();	break;
				case CloseMode.Accept:	this.AcceptEdition ();	break;
			}
			
			this.OnComboClosed ();
			
			if (this.InitialText != this.Text )
			{
				this.OnSelectedIndexChanged ();
			}
		}

		public static void AdjustComboSize(Widget parent, AbstractMenu menu)
		{
			menu.AdjustSize ();
			
			MenuItem.SetMenuHost (parent, new MenuHost (menu));
		}
		
		protected virtual AbstractMenu CreateMenu()
		{
			TextFieldComboMenu menu = new TextFieldComboMenu ();
			
			menu.MinWidth = this.ActualWidth;
			
			this.scroll_list = new ScrollList ();
			this.scroll_list.ScrollListStyle = ScrollListStyle.Menu;
			
			menu.Contents = this.scroll_list;
			
			//	Remplit la liste :
			
			this.CopyItemsToComboList (this.scroll_list.Items);

			TextFieldCombo.AdjustComboSize (this, menu);
			
			return menu;
		}
		
		protected virtual void OnComboOpening(Support.CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = (EventHandler<CancelEventArgs>) this.GetUserEventHandler("ComboOpening");

			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == true);
			
			this.UpdateButtonVisibility ();

			EventHandler handler = (EventHandler) this.GetUserEventHandler("ComboOpened");

			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == false);
			
			this.UpdateButtonVisibility ();

			EventHandler handler = (EventHandler) this.GetUserEventHandler("ComboClosed");

			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	L'utilisateur a cliqué dans le bouton d'ouverture de la liste.
			
			this.OpenCombo ();
		}
		
		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			
			this.ProcessComboActivatedIndex (this.scroll_list.SelectedIndex);
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			//	L'utilisateur a simplement déplacé la souris dans la liste.
			
			this.ProcessComboSelectedIndex (this.scroll_list.SelectedIndex);
		}
		
		private void HandleMenuAccepted(object sender)
		{
			this.CloseCombo(CloseMode.Accept);
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo(CloseMode.Reject);
		}
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
		}
		
		
		public Collections.StringCollection		Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion
		
		#region INamedStringSelection Members
		public virtual int						SelectedIndex
		{
			get
			{
				int	 sel;
				bool exact;
				
				if (this.FindMatch (this.Text, out sel, out exact))
				{
					return sel;
				}
				
				return -1;
			}

			set
			{
				string text = "";
				
				if ((value >= 0) &&
					(value < this.items.Count))
				{
					text = this.items[value];
				}

				if (this.Text != text)
				{
					this.Text = text;
					this.OnSelectedIndexChanged ();
					this.SelectAll ();
				}
			}
		}
		
		public string							SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				
				if (index < 0)
				{
					return "";
				}
				else
				{
					return this.items[index];
				}
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				
				int index = this.Items.IndexOf (value);
				
				if (index < 0)
				{
					this.Text = value;
					this.SelectAll ();
				}
				else
				{
					this.SelectedIndex = index;
				}
			}
		}

		public string							SelectedName
		{
			//	Nom de la ligne sélectionnée, "" si aucune.
			get
			{
				int index = this.SelectedIndex;
				
				if (index < 0)
				{
					return "";
				}
				else
				{
					return this.items.GetName(index);
				}
			}
			
			set
			{
				if (value == null)
				{
					value = "";
				}
				
				if (this.SelectedName != value)
				{
					int index = -1;
					
					if (value.Length > 0)
					{
						index = this.items.FindNameIndex (value);
						
						if (index < 0)
						{
							throw new System.ArgumentException (string.Format ("No element named '{0}' in list", value));
						}
					}
					
					this.SelectedIndex = index;
				}
			}
		}


		public event EventHandler					SelectedIndexChanged
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
				
				Drawing.Point     pos = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, 0));
				Drawing.Point     hot = Helpers.VisualTree.MapVisualToScreen (item, new Drawing.Point (0, 0));
				ScreenInfo        screen_info  = ScreenInfo.Find (hot);
				Drawing.Rectangle working_area = screen_info.WorkingArea;
				
				double max_height = pos.Y - working_area.Bottom;
				
				if ((max_height > size.Height) ||
					(max_height > 100))
				{
					//	Il y a assez de place pour dérouler le menu vers le bas,
					//	mais il faudra peut-être le raccourcir un bout :
					
					this.menu.MaxSize = new Drawing.Size (this.menu.MaxWidth, max_height);
					this.menu.AdjustSize ();
					
					size      = this.menu.ActualSize;
					location  = pos;
					animation = Animation.RollDown;
				}
				else
				{
					//	Il faut dérouler le menu vers le haut.
					
					pos.Y += item.ActualHeight-2;
					
					max_height = working_area.Top - pos.Y;

					this.menu.MaxSize = new Drawing.Size (this.menu.MaxWidth, max_height);
					this.menu.AdjustSize ();
					
					pos.Y += this.menu.ActualHeight;
					
					size      = this.menu.ActualSize;
					location  = pos;
					animation = Animation.RollUp;
				}
				
				location.X -= this.menu.MenuShadow.Left;
				location.Y -= size.Height;
				
				if (location.X + size.Width > working_area.Right)
				{
					location.X = working_area.Right - size.Width;
				}
			}
			#endregion
			
			private AbstractMenu				menu;
		}
		#endregion
		
		public event EventHandler<CancelEventArgs>	ComboOpening
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
		
		private Behaviors.SelectItemBehavior	select_item_behavior;
		private ComboArrowMode					combo_arrow_mode		= ComboArrowMode.Open;
		private bool							is_live_update_enabled	= true;
		
		protected AbstractMenu					menu;
		
		private ScrollList						scroll_list;
		
		protected Button						button;
		protected Collections.StringCollection	items;
		protected ShowCondition					button_show_condition = ShowCondition.Always;
		protected double						default_button_width;
	}
}
