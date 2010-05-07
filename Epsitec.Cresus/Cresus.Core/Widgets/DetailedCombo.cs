//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public Accessors.ComboInitializer ComboInitializer
		{
			// TODO: A supprimer
			get
			{
				return this.comboInitializer;
			}
			set
			{
				this.comboInitializer = value;
				this.CreateUIComboInitializer ();
			}
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
						Name = this.items.GetName (i),
						Text = this.items[i],
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}
				else
				{
					button = new RadioButton
					{
						Parent = this,
						Name = this.items.GetName (i),
						Text = this.items[i],
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}

				button.ActiveStateChanged += new EventHandler (this.HandleButtonActiveStateChanged);
			}
		}

		private void CreateUIComboInitializer()
		{
			// TODO: A supprimer
			//	Il faut appeler cette méthode après avoir défini la liste Items.
			//	Je ne veux pas le faire automatiquement après chaque Items.Add(), pour des raisons
			//	d'efficacité !
			this.Children.Clear ();

			int tabIndex = 1;

			foreach (var pair in this.comboInitializer.Content)
			{
				AbstractButton button;

				if (this.AllowMultipleSelection)
				{
					button = new CheckButton
					{
						Parent = this,
						Name = pair.Key,
						Text = pair.Value,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}
				else
				{
					button = new RadioButton
					{
						Parent = this,
						Name = pair.Key,
						Text = pair.Value,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};
				}

				button.ActiveStateChanged += new EventHandler (this.HandleButtonActiveStateChanged);
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

		#region INamedStringSelection Members

		public string SelectedName
		{
			get
			{
				int currentSelection = this.SelectedIndex;

				if (currentSelection != -1)
				{
					return this.items.GetName (currentSelection);
				}

				return null;
			}
			set
			{
				this.SelectedIndex = this.items.FindIndexByName (value);
			}
		}

		#endregion

		#region IStringSelection Members

		public int SelectedIndex
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
				int currentSelection = this.SelectedIndex;

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
				int currentSelection = this.SelectedIndex;

				if (currentSelection != -1)
				{
					return this.items[currentSelection];
				}

				return null;
			}
			set
			{
				this.SelectedIndex = this.items.FindIndexByValueExactMatch (value);
			}
		}

		public event EventHandler SelectedIndexChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectedIndexChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedIndexChanged", value);
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
				list.Add (this.items.GetName(sel));
			}

			foreach (AbstractButton button in this.Children)
			{
				button.ActiveState = (list.Contains (button.Name)) ? Common.Widgets.ActiveState.Yes : Common.Widgets.ActiveState.No;
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
			var e = new DependencyPropertyChangedEventArgs ("MultiSelection");

			if (handler != null)
			{
				handler (this, e);
			}
		}


		private void HandleButtonActiveStateChanged(object sender)
		{
			this.ButtonsToSelection ();
		}


		private const string MultiSelectionChangedEvent = "MultiSelectionChanged";

		private Common.Widgets.Collections.StringCollection	items;
		private readonly HashSet<int> selection;
		private Accessors.ComboInitializer comboInitializer;
		private bool ignoreChange;
	}
}
