//	Copyright © 2008-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget permet d'éditer une enumération sous une forme détaillée (plusieurs lignes de CheckButton
	/// ou RadioButton), avec un support complet de tous les modes de cardinalité.
	/// </summary>
	public class ItemPickerButtons : Widget, IMultipleSelection, IItemPicker
	{
		public ItemPickerButtons()
		{
			this.Cardinality = EnumValueCardinality.ExactlyOne;

			this.items = new Common.Widgets.Collections.StringCollection (this);
			this.items.AcceptsRichText = true;

			this.selection = new HashSet<int> ();
		}

		public ItemPickerButtons(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		#region IItemPicker Members

		//	Cette interface met en commun tout ce qu'il faut pour unifier ItemPicker et ItemPickerCombo.
		//	Comme ces 2 widgets ont des héritages différents, il est nécessaire de procéder ainsi, en
		//	redéfinissant de façon redondante les méthodes communes de provenances diverses.

		public void IPRefreshContents()
		{
			this.RefreshContents ();
		}

		public void IPUpdateText()
		{
		}

		public void IPClearSelection()
		{
			this.ClearSelection ();
		}

		public void IPAddSelection(int selectedIndex)
		{
			this.AddSelection (selectedIndex);
		}

		public void IPAddSelection(IEnumerable<int> selection)
		{
			this.AddSelection (selection);
		}

		public ICollection<int> IPGetSortedSelection()
		{
			return GetSortedSelection ();
		}

		public event EventHandler IPSelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler (ItemPickerButtons.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemPickerButtons.SelectedItemChangedEvent, value);
			}
		}

		public int IPSelectedItemIndex
		{
			get
			{
				return this.SelectedItemIndex;
			}
			set
			{
				this.SelectedItemIndex = value;
			}
		}

		#endregion

	
		public EnumValueCardinality Cardinality
		{
			// TODO: Le mode EnumValueCardinality.AtLeastOne n'est pas encore supporté !
			get;
			set;
		}

		/// <summary>
		/// Méthode de conversion d'un objet stocké dans Items.Value en une chaîne à afficher.
		/// </summary>
		/// <value>The value converter.</value>
		public ValueToFormattedTextConverter ValueToDescriptionConverter
		{
			get;
			set;
		}

	
		public void RefreshContents()
		{
			//	Il faut appeler cette méthode après avoir défini la liste Items.
			//	Je ne veux pas le faire automatiquement après chaque Items.Add(), pour des raisons
			//	d'efficacité !
			this.Children.Clear ();

			AbstractButton button;
			int tabIndex = 1;

			if (this.Cardinality == EnumValueCardinality.ZeroOrOne)
			{
				button = new RadioButton
				{
					Parent   = this,
					Index    = -1,
					Text     = "Aucun",
					Dock     = DockStyle.Top,
					TabIndex = tabIndex++,
				};
			}

			for (int i = 0; i < this.items.Count; i++)
			{
				if (this.Cardinality == EnumValueCardinality.ExactlyOne ||
					this.Cardinality == EnumValueCardinality.ZeroOrOne)
				{
					button = new RadioButton
					{
						Parent   = this,
						Index    = i,
						Text     = this.GetItemText (i),
						Dock     = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}
				else
				{
					button = new CheckButton
					{
						Parent   = this,
						Index    = i,
						Text     = this.GetItemText (i),
						Dock     = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}

				button.ActiveStateChanged += this.HandleButtonActiveStateChanged;
			}
		}

		private string GetItemText(int index)
		{
			if (this.ValueToDescriptionConverter == null)
			{
				return null;
			}
			else
			{
				object value = this.items.GetValue (index);
				return this.ValueToDescriptionConverter (value).ToString ();
			}
		}


		public void AddSelection(int selectedIndex)
		{
			if (selectedIndex > -1)
			{
				this.AddSelection (new int[] { selectedIndex });
			}
		}


		#region IMultipleSelection Members

		public int SelectionCount
		{
			get
			{
				return this.selection.Count;
			}
		}

		public void AddSelection(IEnumerable<int> selection)
		{
			bool dirty = false;

			foreach (var index in selection)
			{
				if (index > -1)
				{
					if (this.selection.Add (index))
					{
						dirty = true;
					}
				}
			}

			if (dirty)
			{
				this.SelectionToButtons ();
				this.OnMultiSelectionChanged ();
			}
		}

		public void RemoveSelection(IEnumerable<int> selection)
		{
			bool dirty = false;

			foreach (var index in selection)
			{
				if (this.selection.Remove (index))
				{
					dirty = true;
				}
			}

			if (dirty)
			{
				this.SelectionToButtons ();
				this.OnMultiSelectionChanged ();
			}
		}

		public void ClearSelection()
		{
			if (this.selection.Count > 0)
			{
				this.selection.Clear ();

				this.SelectionToButtons ();
				this.OnMultiSelectionChanged ();
			}
		}

		public ICollection<int> GetSortedSelection()
		{
			return this.selection.OrderBy (x => x).ToList ().AsReadOnly ();
		}

		public bool IsItemSelected(int index)
		{
			if (this.selection.Contains (index))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IStringCollectionHost Members

		public void NotifyStringCollectionChanged()
		{
			this.ClearSelection ();
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
				int currentSelection = this.SelectedItemIndex;

				if (currentSelection != -1)
				{
					return this.items.GetKey (currentSelection);
				}

				return null;
			}
			set
			{
				this.SelectedItemIndex = this.items.FindIndexByKey (value);
			}
		}

		#endregion

		#region IStringSelection Members

		public int SelectedItemIndex
		{
			get
			{
				ICollection<int> sels = this.GetSortedSelection ();
				if (sels.Count == 1)
				{
					return sels.First ();
				}

				return -1;
			}
			set
			{
				int currentSelection = this.SelectedItemIndex;

				if (currentSelection != value)
				{
					this.ClearSelection ();

					if (value != -1)
					{
						this.AddSelection (Enumerable.Range (value, 1));
					}
					
					this.OnSelectedItemChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int currentSelection = this.SelectedItemIndex;

				if (currentSelection != -1)
				{
					return this.items[currentSelection];
				}

				return null;
			}
			set
			{
				this.SelectedItemIndex = this.items.FindIndexByValueExactMatch (value);
			}
		}

		public event EventHandler SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler (ItemPickerButtons.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemPickerButtons.SelectedItemChangedEvent, value);
			}
		}

		#endregion


		private void SelectionToButtons()
		{
			//	Met à jour l'état des boutons en fonction de la séleciton.
			var sels = this.GetSortedSelection ();
			
			foreach (AbstractButton button in this.Children)
			{
				this.ignoreChange = true;
				button.ActiveState = sels.Contains (button.Index) ? Common.Widgets.ActiveState.Yes : Common.Widgets.ActiveState.No;
				this.ignoreChange = false;
			}
		}

		private void ButtonsToSelection()
		{
			//	Met à jour la sélection en fonction de l'état des boutons.
			this.selection.Clear ();

			foreach (AbstractButton button in this.Children)
			{
				if (button.IsActive)
				{
					int index = button.Index;

					if (index != -1)
					{
						this.selection.Add (index);
					}
				}
			}

			this.OnMultiSelectionChanged ();
		}


		protected void OnMultiSelectionChanged()
		{
			this.Invalidate ();

			var handler = (EventHandler) this.GetUserEventHandler (ItemPickerButtons.MultiSelectionChangedEvent);

			if (handler != null)
			{
				handler (this);
			}
			
			this.OnSelectedItemChanged ();
		}

		protected void OnSelectedItemChanged()
		{
			this.Invalidate ();

			var handler = (EventHandler) this.GetUserEventHandler (ItemPickerButtons.SelectedItemChangedEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler MultiSelectionChanged
		{
			add
			{
				this.AddUserEventHandler (ItemPickerButtons.MultiSelectionChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemPickerButtons.MultiSelectionChangedEvent, value);
			}
		}


		private void HandleButtonActiveStateChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.ButtonsToSelection ();
		}


		private const string MultiSelectionChangedEvent = "MultiSelectionChanged";
		private const string SelectedItemChangedEvent = "SelectedItemChanged";

		private Common.Widgets.Collections.StringCollection	items;
		private readonly HashSet<int> selection;
		private bool ignoreChange;
	}
}
