﻿//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class DetailedCombo : Widget, IMultipleSelection
	{
		public DetailedCombo()
		{
			this.items = new Common.Widgets.Collections.StringCollection (this);
			this.items.AcceptsRichText = true;

			this.selection = new HashSet<int> ();
		}

		public DetailedCombo(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Autorise ou non les sélections multiples, ce qui se traduit par des CheckButtons ou des RadioButtons.
		/// Dans l'implémentation actuelle, le setter doit être appelé avant le setter de ComboInitializer !
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [allow multiple selection]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowMultipleSelection
		{
			get;
			set;
		}

		/// <summary>
		/// Méthode de conversion d'un objet stocké dans Items.Value en une chaîne à afficher.
		/// </summary>
		/// <value>The value converter.</value>
		public System.Func<object, string> ValueToDescriptionConverter
		{
			get;
			set;
		}

	
		public void CreateUI()
		{
			//	Il faut appeler cette méthode après avoir défini la liste Items.
			//	Je ne veux pas le faire automatiquement après chaque Items.Add(), pour des raisons
			//	d'efficacité !
			this.Children.Clear ();

			int tabIndex = 1;

			for (int i = 0; i < this.items.Count; i++)
			{
				AbstractButton button;

				if (this.AllowMultipleSelection)
				{
					button = new CheckButton
					{
						Parent = this,
						Name = this.GetItemText (i),
						Text = this.GetItemText (i),
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}
				else
				{
					button = new RadioButton
					{
						Parent = this,
						Name = this.GetItemText (i),
						Text = this.GetItemText (i),
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}

				button.ActiveStateChanged += new EventHandler (this.HandleButtonActiveStateChanged);
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
				return this.ValueToDescriptionConverter (value);
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
				if (this.selection.Add (index))
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
				this.AddUserEventHandler (DetailedCombo.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (DetailedCombo.SelectedItemChangedEvent, value);
			}
		}

		#endregion


		private void SelectionToButtons()
		{
			//	Met à jour l'état des boutons en fonction de la séleciton.
			ICollection<int> sels = this.GetSortedSelection ();
			List<string> list = new List<string> ();

			foreach (int sel in sels)
			{
				list.Add (this.GetItemText (sel));
			}

			foreach (AbstractButton button in this.Children)
			{
				this.ignoreChange = true;
				button.ActiveState = (list.Contains (button.Name)) ? Common.Widgets.ActiveState.Yes : Common.Widgets.ActiveState.No;
				this.ignoreChange = false;
			}
		}

		private void ButtonsToSelection()
		{
			//	Met à jour la sélection en fonction de l'état des boutons.
			this.ClearSelection ();

			foreach (AbstractButton button in this.Children)
			{
				if (button.IsActive)
				{
					int index = this.items.Keys.ToList ().IndexOf (button.Name);

					if (index != -1)
					{
						this.AddSelection (Enumerable.Range (index, 1));
					}
				}
			}
		}


		protected void OnMultiSelectionChanged()
		{
			this.Invalidate ();

			var handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> (DetailedCombo.MultiSelectionChangedEvent);
			var e = new DependencyPropertyChangedEventArgs ("MultiSelectionChanged");

			if (handler != null)
			{
				handler (this, e);
			}
		}

		public event EventHandler MultiSelectionChanged
		{
			add
			{
				this.AddUserEventHandler (DetailedCombo.MultiSelectionChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (DetailedCombo.MultiSelectionChangedEvent, value);
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
		private Accessors.ComboInitializer comboInitializer;
		private bool ignoreChange;
	}
}
