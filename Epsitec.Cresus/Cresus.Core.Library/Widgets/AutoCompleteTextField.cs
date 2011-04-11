//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public sealed class AutoCompleteTextField : TextFieldEx, Common.Widgets.Collections.IStringCollectionHost, Common.Support.Data.IKeyedStringSelection
	{
		public AutoCompleteTextField()
		{
			this.HintEditorMode = Widgets.HintEditorMode.DisplayMenuForSmallList;
			this.HintEditorSmallListLimit = 100;
			this.MenuButtonWidth = Library.UI.ComboButtonWidth-1;

			this.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
			this.DefocusAction = Common.Widgets.DefocusAction.AcceptEdition;

			this.items = new Common.Widgets.Collections.StringCollection (this);
			this.selectedRow = -1;
			this.ignoreChanges = new SafeCounter ();

			this.hintListIndex = new List<int> ();
			this.selectedHint = -1;

			this.hintWordSeparators = new List<string> ();

			this.IsFocusedChanged += this.HandleIsFocusedChanged;
			this.TextChanged += this.HandleTextChanged;
		}

		
		public HintEditorMode							HintEditorMode
		{
			get;
			set;
		}

		/// <summary>
		/// Lorsque HintEditorComboMenu = IfReasonable, détermine à partir de combien le combo-menu est caché.
		/// </summary>
		/// <value>The combo menu items limit.</value>
		public int										HintEditorSmallListLimit
		{
			get;
			set;
		}

		/// <summary>
		/// Méthode de comparaison d'un objet stocké dans Items.Value avec une chaîne partielle entrée par l'utilisateur.
		/// </summary>
		/// <value>The hint comparer.</value>
		public HintComparerPredicate<object, string>	HintComparer
		{
			get;
			set;
		}

		/// <summary>
		/// Méthode de conversion d'un objet stocké dans Items.Value en une chaîne à afficher.
		/// </summary>
		/// <value>The value converter.</value>
		public ValueToFormattedTextConverter				ValueToDescriptionConverter
		{
			get;
			set;
		}

		public System.Predicate<string>					ContentValidator
		{
			get;
			set;
		}

		public double									MenuButtonWidth
		{
			get;
			set;
		}



		public void RefreshTextBasedOnSelectedItem()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.Text = this.GetItemText (this.selectedRow);
			}
		}
		
		

		#region IStringCollectionHost Members

		public void NotifyStringCollectionChanged()
		{
		}

		public Common.Widgets.Collections.StringCollection Items
		{
			get
			{
				return this.items;
			}
		}

		#endregion

		#region IKeyedStringSelection Members

		public string SelectedKey
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return null;
				}

				return this.items.GetKey (this.selectedRow);
			}
			set
			{
				if (this.SelectedKey != value)
				{
					int index = -1;

					if (value != null)
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

		#endregion

		#region IStringSelection Members

		public int SelectedItemIndex
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.items.Count-1);
				}
				if (value != this.selectedRow)
				{
					this.selectedRow = value;

					this.RefreshTextBasedOnSelectedItem ();
					this.OnSelectedItemChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int index = this.SelectedItemIndex;
				
				if (index < 0)
				{
					return null;
				}

				return this.Items[index];
			}
			set
			{
				this.SelectedItemIndex = this.Items.IndexOf (value);
			}
		}

		public event EventHandler SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler(AutoCompleteTextField.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (AutoCompleteTextField.SelectedItemChangedEvent, value);
			}
		}

		#endregion


		private void UpdateHint(string typed)
		{
			if ((this.ValueToDescriptionConverter == null) || 
				(this.HintComparer == null))
			{
				return;
			}

			this.UpdateHintList (typed);
			this.SelectDefaultHint ();
			this.UseSelectedHint (SelectedHintMode.Searching);
			this.UpdateComboMenuVisibility ();
		}

		private void UpdateComboMenuVisibility()
		{
			if (this.hintListIndex.Count <= 1)
			{
				//	Zero or one item in the list: no need to display a menu !

				this.CloseComboMenu ();
			}
			else
			{
				if (this.IsComboMenuOpen)
				{
					this.UpdateComboMenuContent ();
				}
				else
				{
					this.OpenComboMenu (displayAllItems: false);
				}
			}
		}
		
		private void SelectDefaultHint()
		{
			if (this.hintListIndex.Count == 0)
			{
				this.selectedHint = -1;
			}
			else
			{
				this.selectedHint = 0;  // la première proposition
			}
		}
		
		private void UpdateHintList(string typed)
		{
			if (this.ValueToDescriptionConverter == null || this.HintComparer == null)
			{
				return;
			}

			this.hintListIndex.Clear ();

			string original = typed;
			if (this.HintComparisonConverter != null)
			{
				typed = this.HintComparisonConverter (typed);
			}

			if (!string.IsNullOrEmpty (typed))
			{
				List<int> list1 = new List<int> ();
				List<int> list2 = new List<int> ();

				for (int i=0; i<this.items.Count; i++)
				{
					var value = this.items.GetValue (i);

					string full = TextFormatter.FormatText (this.ValueToDescriptionConverter (value)).ToString ();
					if (full == original)  // trouvé exactement ?
					{
						list1.Clear ();
						list2.Clear ();

						list1.Add (i);  // met une seule proposition, la bonne
						break;
					}

					var result = this.HintComparer (value, typed);

					if (result == HintComparerResult.PrimaryMatch)
					{
						list1.Add (i);
					}

					if (result == HintComparerResult.SecondaryMatch)
					{
						list2.Add (i);
					}
				}

				this.hintListIndex.AddRange (list1);
				this.hintListIndex.AddRange (list2);
			}
		}


		private void HandleIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;

			if (focused)
			{
			}
			else
			{
				this.CloseComboMenu ();
			}
		}

		private void HandleTextChanged(object sender)
		{
			if (this.ContentValidator != null)
			{
				bool ok = this.ContentValidator (this.Text);
				this.SetError (!ok);
			}
		}


		private void OnSelectedItemChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (AutoCompleteTextField.SelectedItemChangedEvent);
			if (handler != null)
			{
				handler (this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.IsFocusedChanged -= this.HandleIsFocusedChanged;

				this.CloseComboMenu ();
			}

			base.Dispose (disposing);
		}


		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			// TODO: Ne fonctionne toujours pas.
			this.SelectAll ();
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void OnTextChanged()
		{
			if (this.ignoreChanges.Value > 0)
			{
				return;
			}

			this.UpdateHint (this.Text);
			base.OnTextChanged ();
		}

		protected override void OnAcceptingEdition(CancelEventArgs e)
		{
			//	OnAcceptingEdition est appelé pendant la phase d'acceptation; l'événement passe une instance de CancelEventArgs
			//	qui permet à ceux qui écoutent l'événement de faire un e.Cancel=true pour annuler l'opération en cours (donc
			//	refuser l'acceptation).
			if (string.IsNullOrEmpty (this.HintText) && this.ContentValidator != null)  // pas de texte souhaité et validateur existe ?
			{
				if (!this.ContentValidator (this.Text))  // valeur incorrecte ?
				{
					e.Cancel = true;
					return;
				}
			}

			base.OnAcceptingEdition (e);

#if false
			if (this.ContentValidator == null)
			{
				if (this.IsComboMenuOpen)
				{
					this.Text = this.HintText;
				}
			}
			else
			{
				//	Si un validateur existe, il faut accepter les valeurs 'non hint' éditées à la main,
				//	si elles sont conforme au validateur.
				if (string.IsNullOrEmpty (this.HintText))  // valeur éditée "à la main" ?
				{
					if (!this.ContentValidator (this.Text))  // valeur incorrecte ?
					{
						e.Cancel = true;
					}
				}
				else  // valeur choisie dans le menu (donc forcément ok) ?
				{
					if (this.IsComboMenuOpen)
					{
						this.Text = this.HintText;
					}
				}
			}
#endif
		}

		protected override void OnEditionAccepted()
		{
			//	OnEditionAccepted est appelé après que l'édition ait été validée et acceptée.
			if (!string.IsNullOrEmpty (this.HintText))
			{
				this.Text = this.HintText;
			}

			base.OnEditionAccepted ();

#if false
			if (this.ContentValidator == null)
			{
				this.Text = this.HintText;
			}
			else
			{
				//	Si un validateur existe, il faut accepter les valeurs 'non hint' éditées à la main,
				//	si elles sont conforme au validateur.
				if (string.IsNullOrEmpty (this.HintText))  // valeur éditée "à la main" ?
				{
					if (!this.ContentValidator (this.Text))
					{
						this.Text = null;
					}
				}
				else  // valeur choisie dans le menu ?
				{
					this.Text = this.HintText;
				}
			}
#endif

			this.CloseComboMenu ();
		}

		protected override void OnEditionRejected()
		{
			base.OnEditionRejected ();

			this.CloseComboMenu ();
		}



		protected override bool ProcessKeyDown(Message message, Point pos)
		{
			//	Gère les pressions de touches (en particulier les flèches haut
			//	et bas qui permettent de cycler le contenu).
			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:
					this.Navigate (-1);
					return true;

				case KeyCode.ArrowDown:
					this.Navigate (1);
					return true;

				case KeyCode.Escape:
					if (this.IsComboMenuOpen)
					{
						this.CloseComboMenu ();
					}
					break;
			}

			return base.ProcessKeyDown (message, pos);
		}

		private void Navigate(int dir)
		{
			//	Cherche le nom suivant ou précédent dans la comboList, même si elle
			//	n'est pas "déroulée".
			if (this.IsComboMenuOpen)
			{
				if (this.hintListIndex.Count != 0)  // pas de menu ?
				{
					int sel = this.selectedHint + dir;

					if (sel < 0)
					{
						sel = this.hintListIndex.Count-1;
					}

					if (sel >= this.hintListIndex.Count)
					{
						sel = 0;
					}

					this.selectedHint = sel;

					this.UpdateComboMenuSelection ();
					this.UseSelectedHint (SelectedHintMode.StartEdition);
				}
			}
			else
			{
				int sel = 0;

				this.UpdateHintList (this.Text);

				if (this.hintListIndex.Count != 0)
				{
					sel = this.hintListIndex[0] + dir;
				}

				this.hintListIndex.Clear ();
				for (int i = 0; i < this.items.Count; i++)
				{
					this.hintListIndex.Add (i);
				}

				if (sel < 0)
				{
					sel = this.hintListIndex.Count-1;
				}

				if (sel >= this.hintListIndex.Count)
				{
					sel = 0;
				}

				this.selectedHint = sel;
				this.UseSelectedHint (SelectedHintMode.StartEdition);

				this.hintListIndex.Clear ();
			}
		}


		private enum SelectedHintMode
		{
			Searching,
			StartEdition,
			AcceptEdition,
		}

		private void UseSelectedHint(SelectedHintMode selectedHintMode)
		{
			using (this.ignoreChanges.Enter ())
			{
				if (this.selectedHint >= 0 && this.selectedHint < this.hintListIndex.Count)
				{
					int i = this.hintListIndex[this.selectedHint];

					this.SetSilentSelectedItemIndex (i);  // (*)
					this.HintText = this.GetItemText (i);
					this.SetError (false);
				}
				else
				{
					this.SetSilentSelectedItemIndex (-1);  // (*)
					this.HintText = null;

					if (this.ContentValidator == null)
					{
						this.SetError (!string.IsNullOrEmpty (this.Text));
					}
					else
					{
						bool ok = this.ContentValidator (this.Text);
						this.SetError (!ok);
					}
				}

				if (selectedHintMode == SelectedHintMode.StartEdition)
				{
					this.StartEdition ();
					this.TextNavigator.TextLayout.Text = this.HintText;
					this.OnTextEdited ();
					this.SelectAll ();
				}

				if (selectedHintMode == SelectedHintMode.AcceptEdition)
				{
					this.StartEdition ();
					this.AcceptEdition ();
					this.SelectAll ();
				}
			}
		}

		// (*)	Il ne faut surtout pas utiliser SelectedItemIndex = i, car il ne faut pas modifier this.Text !

		private void SetSilentSelectedItemIndex(int index)
		{
			if (this.selectedRow != index)
			{
				this.selectedRow = index;
				this.OnSelectedItemChanged ();
			}
		}

		private string GetItemText(int index)
		{
			if (index == -1)
			{
				return null;
			}
			else
			{
				if (this.ValueToDescriptionConverter == null)
				{
					return TextFormatter.FormatText (this.items.GetValue (index)).ToString ();
				}
				else
				{
					object value = this.items.GetValue (index);
					return TextFormatter.FormatText (this.ValueToDescriptionConverter (value)).ToString ();
				}
			}
		}


		public void OpenComboMenu()
		{
			this.SelectAll ();
			this.Focus ();
			this.OpenComboMenu (displayAllItems: true);
		}

		private bool IsComboMenuOpen
		{
			get
			{
				return this.window != null;
			}
		}

		private void OpenComboMenu(bool displayAllItems)
		{
			if (this.IsComboMenuOpen)
			{
				if (displayAllItems)
				{
					this.CloseComboMenu ();

					if (this.displayAllItems)
					{
						//	The user clicked on the button to open the combo menu, while the
						//	combo menu was already showing all items; simply toggle from the
						//	visible combo to the hidden combo :

						return;
					}
				}
				else
				{
					return;
				}
			}

			this.displayAllItems = displayAllItems;

			if (this.displayAllItems)
			{
				this.UpdateHintListWithAllItems ();
			}
			else
			{
				switch (this.HintEditorMode)
				{
					case Widgets.HintEditorMode.InLine:
						return;
						
					case Widgets.HintEditorMode.DisplayMenuForSmallList:
						if (this.hintListIndex.Count >= this.HintEditorSmallListLimit)
						{
							return;
						}
						break;

					default:
						break;
				}
			}

			if (this.hintListIndex.Count == 0)
			{
				//	Don't open an empty menu:
				return;
			}

			this.CreateComboMenu ();
			this.UpdateComboMenuContent ();
			this.window.Show ();
		}

		private void CreateComboMenu()
		{
			this.window = new Common.Widgets.Window ();

			this.window.Owner = this.Window;
			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();
			this.window.DisableMouseActivation ();

			this.scrollList = new ScrollList ()
			{
				Parent = window.Root,
				ScrollListStyle = ScrollListStyle.Menu,
				AutomaticScrollEnable = false,
				Dock = DockStyle.Fill
			};

			this.scrollList.SelectionActivated += this.HandleScrollListSelectionActivated;
		}

		private void UpdateHintListWithAllItems()
		{
			this.UpdateHintList (this.Text);
			this.UpdateSelectedHint ();
			
			this.hintListIndex.Clear ();
			for (int i = 0; i < this.items.Count; i++)
			{
				this.hintListIndex.Add (i);
			}
		}

		private void UpdateSelectedHint()
		{
			if (this.hintListIndex.Count == 0)
			{
				this.selectedHint = -1;
			}
			else
			{
				this.selectedHint = this.hintListIndex[0];
			}
		}
		private void UpdateComboMenuContent()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			if (!this.displayAllItems)
			{
				if (this.HintEditorMode == Widgets.HintEditorMode.DisplayMenuForSmallList &&
					this.hintListIndex.Count >= this.HintEditorSmallListLimit)
				{
					this.CloseComboMenu ();
					return;
				}
			}

			this.UpdateScrollListContent ();

			Size bestSize = this.scrollList.GetBestFitSize ();
			Size size = new Size (this.ActualWidth, bestSize.Height);

			Point location;
			this.GetMenuDisposition (this.scrollList, ref size, out location);

			this.window.WindowLocation = location;
			this.window.WindowSize = size;
		}

		private void UpdateComboMenuSelection()
		{
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			using (this.ignoreChanges.Enter ())
			{
				this.scrollList.SelectedItemIndex = this.selectedHint;
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}
		}

		private void CloseComboMenu()
		{
			// TODO: Le menu n'est pas fermé lorsqu'on déplace la fenêtre !
			if (!this.IsComboMenuOpen)
			{
				return;
			}

			this.window.Close ();
			this.window = null;

			this.scrollList.SelectionActivated -= this.HandleScrollListSelectionActivated;
			this.scrollList = null;
		}


		private void UpdateScrollListContent()
		{
			this.scrollList.Items.Clear ();

			for (int index = 0; index < this.hintListIndex.Count; index++)
			{
				int i = this.hintListIndex[index];

				string key = this.items.GetKey (i);
				string text = this.GetItemText (i);

				this.scrollList.Items.Add (key, text);
			}

			using (this.ignoreChanges.Enter ())
			{
				this.scrollList.SelectedItemIndex = this.selectedHint;
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}

			Size size = this.scrollList.GetBestFitSizeBasedOnContent ();
			this.scrollList.RowHeight      = size.Height;
			this.scrollList.PreferredWidth = size.Width;
		}


		private void GetMenuDisposition(ScrollList scrollList, ref Size size, out Point location)
		{
			Point pos = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (this, new Point (0, 0));
			Point hot = Common.Widgets.Helpers.VisualTree.MapVisualToScreen (this, new Point (0, 0));
			ScreenInfo screenInfo  = ScreenInfo.Find (hot);
			Rectangle workingArea = screenInfo.WorkingArea;

			double maxHeight = pos.Y - workingArea.Bottom;

			if (maxHeight > size.Height || maxHeight > 100)
			{
				//	Il y a assez de place pour dérouler le menu vers le bas,
				//	mais il faudra peut-être le raccourcir un bout :
				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				AutoCompleteTextField.AdjustSize (scrollList);

				size.Height = scrollList.PreferredHeight;
				size.Width  = System.Math.Max (size.Width+this.MenuButtonWidth, scrollList.PreferredWidth);

				location = new Point(pos.X, pos.Y+1);
			}
			else
			{
				//	Il faut dérouler le menu vers le haut.
				pos.Y += this.ActualHeight-1;

				maxHeight = workingArea.Top - pos.Y;

				scrollList.MaxSize = new Size (scrollList.MaxWidth, maxHeight);
				AutoCompleteTextField.AdjustSize (scrollList);

				pos.Y += scrollList.PreferredHeight;

				size.Height = scrollList.PreferredHeight;
				size.Width  = System.Math.Max (size.Width+this.MenuButtonWidth, scrollList.PreferredWidth);

				location = pos;
			}

			location.Y -= size.Height;

			if (location.X + size.Width > workingArea.Right)
			{
				location.X = workingArea.Right - size.Width;
			}
		}

		private static void AdjustSize(ScrollList scrollList)
		{
			scrollList.PreferredSize = scrollList.GetBestFitSize ();
			Common.Widgets.Layouts.LayoutContext.SyncArrange (scrollList);
		}


		private void HandleScrollListSelectionActivated(object sender)
		{
			if (this.ignoreChanges.Value > 0)
			{
				return;
			}

			this.selectedHint = this.scrollList.SelectedItemIndex;
			
			this.UseSelectedHint (SelectedHintMode.AcceptEdition);

			this.CloseComboMenu ();
		}

		
		private const string SelectedItemChangedEvent = "SelectedItemChanged";


		private readonly Common.Widgets.Collections.StringCollection items;
		private readonly List<int> hintListIndex;
		private readonly List<string> hintWordSeparators;
		private readonly SafeCounter ignoreChanges;
		
		private int selectedRow;
		private int selectedHint;

		private Window window;
		private ScrollList scrollList;

		private bool displayAllItems;
	}
}
