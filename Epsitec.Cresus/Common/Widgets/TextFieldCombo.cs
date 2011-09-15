//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v"
	/// qui fait apparaître un menu dit "combo" pour permettre de choisir une
	/// option prédéfinie.
	/// </summary>
	public class TextFieldCombo : AbstractTextField, Collections.IStringCollectionHost, Support.Data.IKeyedStringSelection
	{
		public TextFieldCombo()
			: base (TextFieldStyle.Combo)
		{
			this.ButtonShowCondition = ButtonShowCondition.Always;
			
			this.selectItemBehavior = new Behaviors.SelectItemBehavior (this.AutomaticItemSelection);

			this.items = new Collections.StringCollection (this);
			this.items.AcceptsRichText = true;
			
			this.button = this.CreateButton ();
			
			this.button.Name     = "Open";
			this.button.Pressed += this.HandleButtonPressed;
			
			this.defaultButtonWidth = this.button.PreferredWidth;
			this.margins.Right      = this.button.PreferredWidth;
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


		public double							MenuButtonWidth
		{
			get
			{
				return this.button.PreferredWidth;
			}
			set
			{
				this.button.PreferredWidth = value;

				this.defaultButtonWidth = this.button.PreferredWidth;
				this.margins.Right      = this.button.PreferredWidth;
			}
		}

		public Button Button
		{
			get
			{
				return this.button;
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
				return this.isLiveUpdateEnabled;
			}
			set
			{
				this.isLiveUpdateEnabled = value;
			}
		}
		
		public ComboArrowMode					ComboArrowMode
		{
			get
			{
				return this.comboArrowMode;
			}
			set
			{
				this.comboArrowMode = value;
			}
		}

		public System.Converter<string, string> ItemTextConverter
		{
			get
			{
				return this.itemTextConverter;
			}
			set
			{
				this.itemTextConverter = value;
			}
		}

		public System.Converter<string, string>	ListTextConverter
		{
			get
			{
				return this.listTextConverter;
			}
			set
			{
				this.listTextConverter = value;
			}
		}

		
		
		protected ScrollList					ScrollList
		{
			get
			{
				return this.scrollList;
			}
		}
		
		
		public bool FindMatch(string find, out int index, out bool exactMatch)
		{
			//	Trouve l'index de l'élément recherché. Indique s'il s'agit d'un
			//	match exact ('find' est égal au contenu de la cellule) ou non
			//	('find' correspond au début de la cellule).
			
			index = this.items.FindIndexByValueExactMatch (find);
			
			if (index < 0)
			{
				exactMatch = false;
				
				if (find == "")
				{
					return false;
				}
				
				index = this.items.FindIndexByValueStartMatch (find);
				
				if (index < 0)
				{
					return false;
				}
			}
			else
			{
				exactMatch = true;
			}
			
			return true;
		}

		public void AcceptCombo()
		{
			if (this.IsComboOpen)
			{
				this.CloseCombo (CloseMode.Accept);
			}
		}

		public void RejectCombo()
		{
			if (this.IsComboOpen)
			{
				this.CloseCombo (CloseMode.Reject);
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.IsComboOpen)
				{
					this.CloseCombo (CloseMode.Reject);
				}
				
				this.button.Pressed -= this.HandleButtonPressed;
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
				this.margins.Right = this.button.Visibility ? this.defaultButtonWidth : 0;
				this.button.SetManualBounds(this.GetButtonBounds());
			}
		}
		
		protected override void UpdateButtonVisibility()
		{
			base.UpdateButtonVisibility ();
			this.SetButtonVisibility (this.ComputeButtonVisibility ());
		}
		
		
		protected bool ComputeButtonVisibility()
		{
			bool show = false;
			
			switch (this.ButtonShowCondition)
			{
				case ButtonShowCondition.Always:				show = true;									break;
				case ButtonShowCondition.Never:					show = false;									break;
				case ButtonShowCondition.WhenFocused:			show = this.IsFocused     || this.IsComboOpen;	break;
				case ButtonShowCondition.WhenKeyboardFocused:	show = this.KeyboardFocus || this.IsComboOpen;	break;
				case ButtonShowCondition.WhenModified:			show = this.HasEditedText;						break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
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
		
		
		protected virtual void OnSelectedItemChanged()
		{
			//	Ne notifie les changements d'index que lorsque le menu déroulant
			//	est fermé.
			
			if (this.IsComboOpen == false)
			{
				var handler = this.GetUserEventHandler("SelectedItemChanged");

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
				this.SelectedItemIndex = index;
			}
		}

		protected virtual bool CheckIfOpenComboRequested(Message message)
		{
			return Feel.Factory.Active.TestComboOpenKey (message);
		}

		
		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
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
		
		protected override bool ProcessKeyDown(Message message, Point pos)
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
		
		protected override bool ProcessKeyPress(Message message, Point pos)
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
			return this.selectItemBehavior.ProcessKeyPress (message);
		}
		
		protected virtual void ProcessComboActivatedIndex(int sel)
		{
			//	Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			//	est validée par un clic de souris, au contraire de ProcessComboSelectedIndex
			//	qui est appelée à chaque changement "visuel".
			
			int index = this.MapComboListToIndex (sel);
			
			if (index >= 0)
			{
				this.SelectedItemIndex = index;
				this.menu.Behavior.Accept ();
			}
		}
		
		protected virtual void ProcessComboSelectedIndex(int sel)
		{
			//	Met à jour le contenu de la combo en cas de changement de sélection
			//	dans la liste, pour autant qu'une telle mise à jour "live" ait été
			//	activée.
			
			if (this.isLiveUpdateEnabled)
			{
				this.SelectedItemIndex = this.MapComboListToIndex (sel);
			}
		}
		
		
		protected virtual void CopyItemsToComboList(Collections.StringCollection list)
		{
			for (int i = 0 ; i < this.items.Count; i++)
			{
				string name = this.items.GetKey (i);
				string text = this.items[i];

				if (this.listTextConverter != null)
				{
					text = this.listTextConverter (text);
				}
				
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
			
			this.SelectedItemIndex = sel;
			this.Focus ();
		}
		
		
		protected virtual void OpenCombo()
		{
			//	Rend la liste visible et démarre l'interaction.
			
			if (this.IsComboOpen)
			{
				return;
			}
			
			Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs ();
			this.OnComboOpening (cancelEvent);
			
			if (cancelEvent.Cancel)
			{
				return;
			}
			
			this.menu = this.CreateMenu ();

			if (this.menu == null)
			{
				return;
			}

			this.menu.AutoDispose = true;
			this.menu.ShowAsComboList (this, this.MapClientToScreen (new Point(0, 0)), this.Button);
			
			if (this.scrollList != null)
			{
				this.scrollList.SelectedItemIndex = this.MapIndexToComboList (this.SelectedItemIndex);
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}
			
			this.menu.Accepted += this.HandleMenuAccepted;
			this.menu.Rejected += this.HandleMenuRejected;
			
			if (this.scrollList != null)
			{
				this.scrollList.SelectedItemChanged += this.HandleScrollerSelectedItemChanged;
				this.scrollList.SelectionActivated   += this.HandleScrollListSelectionActivated;
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

			this.menu.Accepted -= this.HandleMenuAccepted;
			this.menu.Rejected -= this.HandleMenuRejected;
			
			if (this.scrollList != null)
			{
				this.scrollList.SelectionActivated   -= this.HandleScrollListSelectionActivated;
				this.scrollList.SelectedItemChanged -= this.HandleScrollerSelectedItemChanged;
				
				this.scrollList.Dispose();
				this.scrollList = null;
			}
			
//-			this.menu.Dispose ();
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
				this.OnSelectedItemChanged ();
			}
		}

		public static void AdjustComboSize(Widget parent, AbstractMenu menu, bool isScrollable)
		{
			menu.AdjustSize();

			if (isScrollable)
			{
				MenuItem.SetMenuHost(parent, new ScrollableMenuHost(menu));
			}
			else
			{
				MenuItem.SetMenuHost(parent, new FixMenuHost(menu));
			}
		}

		public static void AdjustScrollListWidth(ScrollList scrollList)
		{
			Size size = scrollList.GetBestFitSizeBasedOnContent ();
			
			scrollList.RowHeight     = size.Height;
			scrollList.PreferredWidth = size.Width;
		}
		
		protected virtual AbstractMenu CreateMenu()
		{
			TextFieldComboMenu menu = new TextFieldComboMenu ();
			
			menu.MinWidth = this.ActualWidth;
			
			this.scrollList = new ScrollList ();
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.AutomaticScrollEnable = false;
			
			menu.Contents = this.scrollList;
			
			//	Remplit la liste :
			
			this.CopyItemsToComboList (this.scrollList.Items);

			TextFieldCombo.AdjustScrollListWidth (this.scrollList);
			TextFieldCombo.AdjustComboSize (this, menu, true);
			
			return menu;
		}
		
		protected virtual void OnComboOpening(Support.CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = this.GetUserEventHandler<CancelEventArgs> ("ComboOpening");

			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == true);
			
			this.UpdateButtonVisibility ();

			var handler = this.GetUserEventHandler("ComboOpened");

			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == false);
			
			this.UpdateButtonVisibility ();

			var handler = this.GetUserEventHandler("ComboClosed");

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
			
			this.ProcessComboActivatedIndex (this.scrollList.SelectedItemIndex);
		}
		
		private void HandleScrollerSelectedItemChanged(object sender)
		{
			//	L'utilisateur a simplement déplacé la souris dans la liste.
			
			this.ProcessComboSelectedIndex (this.scrollList.SelectedItemIndex);
		}
		
		private void HandleMenuAccepted(object sender)
		{
			this.CloseCombo(CloseMode.Accept);
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo(CloseMode.Reject);
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			this.selectedItemIndex = -1;
		}
		
		#region IStringCollectionHost Members
		
		public void NotifyStringCollectionChanged()
		{
			this.selectedItemIndex = -1;
		}
		
		
		public Collections.StringCollection		Items
		{
			get
			{
				return this.items;
			}
		}
		
		#endregion

		#region IKeyedStringSelection Members

		public virtual int						SelectedItemIndex
		{
			get
			{
				if (this.selectedItemIndex == -1)
				{
					int sel;
					bool exact;

					if (this.FindMatch (this.Text, out sel, out exact))
					{
						this.selectedItemIndex = sel;
					}
				}

				return this.selectedItemIndex;
			}

			set
			{
				int oldIndex = this.SelectedItemIndex;
				int newIndex = value;

				string text = "";
				
				if ((value >= 0) &&
					(value < this.items.Count))
				{
					text = this.items[value];

					if (this.itemTextConverter != null)
					{
						text = this.itemTextConverter (text);
					}
				}

				if (this.Text != text)
				{
					this.Text = text;
					this.SelectAll ();
				}

				this.selectedItemIndex = newIndex;

				if (oldIndex != newIndex)
				{
					this.OnSelectedItemChanged ();
					this.SelectAll ();
				}
			}
		}
		
		public string							SelectedItem
		{
			get
			{
				int index = this.SelectedItemIndex;
				
				if (index < 0)
				{
					return "";
				}
				else
				{
					string text = this.items[index];
					
					if (this.itemTextConverter != null)
					{
						text = this.itemTextConverter (text);
					}

					return text;
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
					this.SelectedItemIndex = index;
				}
			}
		}

		public string							SelectedKey
		{
			//	Nom de la ligne sélectionnée, "" si aucune.
			get
			{
				int index = this.SelectedItemIndex;
				
				if (index < 0 || index >= this.items.Count)
				{
					return "";
				}
				else
				{
					return this.items.GetKey(index);
				}
			}
			
			set
			{
				if (value == null)
				{
					value = "";
				}
				
				if (this.SelectedKey != value)
				{
					int index = -1;
					
					if (value.Length > 0)
					{
						index = this.items.FindIndexByKey (value);
						
						if (index < 0)
						{
							throw new System.ArgumentException (string.Format ("No element named '{0}' in list", value));
						}
					}

					this.SelectedItemIndex = index;
				}
			}
		}


		public event EventHandler				SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedItemChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedItemChanged", value);
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

		#region ScrollableMenuHost Class
		protected class ScrollableMenuHost : IMenuHost
		{
			public ScrollableMenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, ref Size size, out Point location, out Animation animation)
			{
				//	Détermine la hauteur maximale disponible par rapport à la position
				//	actuelle :
				Point pos = Helpers.VisualTree.MapVisualToScreen(item, new Point(0, 0));
				Point hot = Helpers.VisualTree.MapVisualToScreen(item, new Point(0, 0));
				ScreenInfo screenInfo  = ScreenInfo.Find(hot);
				Rectangle workingArea = screenInfo.WorkingArea;

				double maxHeight = pos.Y - workingArea.Bottom;

				if (maxHeight > size.Height || maxHeight > 100)
				{
					//	Il y a assez de place pour dérouler le menu vers le bas,
					//	mais il faudra peut-être le raccourcir un bout :
					this.menu.MaxSize = new Size(this.menu.MaxWidth, maxHeight);
					this.menu.AdjustSize();
					
					size      = this.menu.ActualSize;
					location  = new Point (pos.X, pos.Y+1);
					animation = Animation.RollDown;
				}
				else
				{
					//	Il faut dérouler le menu vers le haut.
					pos.Y += item.ActualHeight-2;

					maxHeight = workingArea.Top - pos.Y;

					this.menu.MaxSize = new Size(this.menu.MaxWidth, maxHeight);
					this.menu.AdjustSize();
					
					pos.Y += this.menu.ActualHeight;
					
					size      = this.menu.ActualSize;
					location  = pos;
					animation = Animation.RollUp;
				}
				
				location.X -= this.menu.MenuShadow.Left;
				location.Y -= size.Height;

				if (location.X + size.Width > workingArea.Right)
				{
					location.X = workingArea.Right - size.Width;
				}
			}
			#endregion
			
			private AbstractMenu				menu;
		}
		#endregion
		
		#region FixMenuHost Class
		protected class FixMenuHost : IMenuHost
		{
			public FixMenuHost(AbstractMenu menu)
			{
				this.menu = menu;
			}
			
			
			#region IMenuHost Members
			public void GetMenuDisposition(Widget item, ref Size size, out Point location, out Animation animation)
			{
				//	Détermine la hauteur maximale disponible par rapport à la position
				//	actuelle :
				Point pos = Helpers.VisualTree.MapVisualToScreen(item, new Point(0, 0));
				Point hot = Helpers.VisualTree.MapVisualToScreen(item, new Point(0, 0));
				ScreenInfo screenInfo  = ScreenInfo.Find(hot);
				Rectangle workingArea = screenInfo.WorkingArea;

				this.menu.MaxSize = new Size(this.menu.MaxWidth, workingArea.Height);
				this.menu.AdjustSize();

				if (this.menu.PreferredHeight < pos.Y-workingArea.Bottom)
				{
					//	Il y a assez de place pour dérouler le menu vers le bas.
					size      = this.menu.PreferredSize;
					location  = pos;
					animation = Animation.RollDown;
				}
				else if (this.menu.PreferredHeight < workingArea.Top-(pos.Y+item.ActualHeight))
				{
					//	Il y a assez de place pour dérouler le menu vers le haut.
					pos.Y += item.ActualHeight;
					pos.Y += this.menu.PreferredHeight;

					size      = this.menu.PreferredSize;
					location  = pos;
					animation = Animation.RollUp;
				}
				else
				{
					//	Il faut dérouler le menu vers le bas, mais depuis en dessus du bouton.
					pos.Y = workingArea.Bottom+this.menu.PreferredHeight;

					size      = this.menu.PreferredSize;
					location  = pos;
					animation = Animation.RollDown;
				}
				
				location.X -= this.menu.MenuShadow.Left;
				location.Y -= size.Height;

				if (location.X + size.Width > workingArea.Right)
				{
					location.X = workingArea.Right - size.Width;
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

		System.Converter<string, string>		itemTextConverter;
		System.Converter<string, string>		listTextConverter;
		
		private Behaviors.SelectItemBehavior	selectItemBehavior;
		private ComboArrowMode					comboArrowMode		= ComboArrowMode.Open;
		private bool							isLiveUpdateEnabled	= true;
		
		protected AbstractMenu					menu;
		
		private ScrollList						scrollList;
		private int								selectedItemIndex;
		
		protected Button						button;
		protected Collections.StringCollection	items;
		protected double						defaultButtonWidth;
	}
}
