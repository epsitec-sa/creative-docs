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
	public class SuperCombo : TextFieldCombo, IMultipleSelection
	{
		public SuperCombo()
		{
			this.AllowMultipleSelection = true;  //? provisoire
			this.MultipleSelectionTextSeparator = ", ";

			this.selection = new HashSet<int> ();
		}

		public SuperCombo(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: ...
			}

			base.Dispose (disposing);
		}


		public bool AllowMultipleSelection
		{
			get;
			set;
		}


		public string MultipleSelectionTextSeparator
		{
			get;
			set;
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
				this.UpdateText ();
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
				this.UpdateText ();
				this.OnMultiSelectionChanged ();
			}
		}

		public void ClearSelection()
		{
			if (this.selection.Count > 0)
			{
				this.selection.Clear ();

				this.UpdateText ();
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


		protected override void OpenCombo()
		{
			//	Rend la liste visible et démarre l'interaction.

			if (this.IsComboOpen)
			{
				return;
			}

			Common.Support.CancelEventArgs cancelEvent = new Common.Support.CancelEventArgs ();
			this.OnComboOpening (cancelEvent);

			if (cancelEvent.Cancel)
			{
				return;
			}

			if (this.AllowMultipleSelection)
			{
				this.InitializeSelections ();
			}

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			this.menu = this.CreateMenu ();

			if (this.menu == null)
			{
				return;
			}

			this.menu.AutoDispose = true;
			this.menu.ShowAsComboList (this, this.MapClientToScreen (new Point (0, 0)), this.Button);

			if (this.scrollList != null)
			{
				this.scrollList.SelectedIndex = this.MapIndexToComboList (this.SelectedIndex);
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}

			this.menu.Accepted += this.HandleMenuAccepted;
			this.menu.Rejected += this.HandleMenuRejected;

			if (this.scrollList != null)
			{
				this.scrollList.SelectedIndexChanged += this.HandleScrollerSelectedIndexChanged;
				this.scrollList.SelectionActivated   += this.HandleScrollListSelectionActivated;
			}

			this.StartEdition ();
			this.OnComboOpened ();
		}

		protected override void CloseCombo(CloseMode mode)
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
				this.scrollList.SelectedIndexChanged -= this.HandleScrollerSelectedIndexChanged;

				this.scrollList.Dispose ();
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
				case CloseMode.Reject:
					this.RejectEdition ();
					break;
				case CloseMode.Accept:
					this.AcceptEdition ();
					break;
			}

			this.OnComboClosed ();

			if (this.InitialText != this.Text)
			{
				this.OnSelectedIndexChanged ();
			}
		}

		protected override AbstractMenu CreateMenu()
		{
			TextFieldComboMenu menu = new TextFieldComboMenu ();

			menu.MinWidth = this.ActualWidth;

			this.scrollList = new ScrollList ();
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;

			menu.Contents = this.scrollList;

			//	Remplit la liste :

			this.CopyItemsToComboList (this.scrollList.Items);

			TextFieldCombo.AdjustScrollListWidth (this.scrollList);
			TextFieldCombo.AdjustComboSize (this, menu, true);

			return menu;
		}

		protected override void ProcessComboActivatedIndex(int sel)
		{
			//	Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			//	est validée par un clic de souris, au contraire de ProcessComboSelectedIndex
			//	qui est appelée à chaque changement "visuel".

			int index = this.MapComboListToIndex (sel);

			if (index >= 0)
			{
				if (this.AllowMultipleSelection)
				{
					this.InitialiseSelectedText (index);
				}
				else
				{
					this.SelectedIndex = index;
				}

				this.menu.Behavior.Accept ();
			}
		}

		protected override void ProcessComboSelectedIndex(int sel)
		{
			//	Met à jour le contenu de la combo en cas de changement de sélection
			//	dans la liste, pour autant qu'une telle mise à jour "live" ait été
			//	activée.

			if (this.IsLiveUpdateEnabled && !this.AllowMultipleSelection)
			{
				this.SelectedIndex = this.MapComboListToIndex (sel);
			}
		}

		protected override void CopyItemsToComboList(Common.Widgets.Collections.StringCollection list)
		{
			for (int i = 0; i < this.items.Count; i++)
			{
				string name = this.items.GetName (i);
				string text = this.items[i];

				if (this.ListTextConverter != null)
				{
					text = this.ListTextConverter (text);
				}

				if (this.IsSelectedItem (text))
				{
					text = string.Concat ("● ", text);  // TODO: Mettre un vrai "vu"
				}
				else
				{
					text = string.Concat ("   ", text);
				}

				list.Add (name, text);
			}
		}

		private bool IsSelectedItem(string text)
		{
			var words = Misc.Split (this.Text.Replace (",", " "), " ");

			foreach (var word in words)
			{
				if (word == text)
				{
					return true;
				}
			}

			return false;
		}

		private void InitializeSelections()
		{
			this.ClearSelection ();

			var words = Misc.Split (this.Text.Replace (",", " "), " ");

			foreach (var word in words)
			{
				int index = this.items.IndexOf (word);
				if (index != -1)
				{
					this.AddSelection (Enumerable.Range (index, 1));
				}
			}
		}

		private void InitialiseSelectedText(int index)
		{
			if ((this.IsItemSelected (index)))
			{
				this.RemoveSelection (Enumerable.Range (index, 1));
			}
			else
			{
				this.AddSelection (Enumerable.Range (index, 1));
			}

			this.UpdateText ();
		}

		private void UpdateText()
		{
			ICollection<int> sels = this.GetSortedSelection ();
			List<string> list = new List<string> ();

			foreach (int sel in sels)
			{
				list.Add (this.items[sel]);
			}

			this.Text = Misc.Combine (list, this.MultipleSelectionTextSeparator);
		}
		

		protected override void OnComboClosed()
		{
			base.OnComboClosed ();

			if (this.Window != null)
			{
				this.Window.RestoreLogicalFocus ();
			}
		}

		protected void OnMultiSelectionChanged()
		{
			this.Invalidate ();

			var handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> (SuperCombo.MultiSelectionChangedEvent);
			var e = new DependencyPropertyChangedEventArgs ("MultiSelection");

			if (handler != null)
			{
				handler (this, e);
			}
		}


		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.

			this.ProcessComboActivatedIndex (this.scrollList.SelectedIndex);
		}

		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			//	L'utilisateur a simplement déplacé la souris dans la liste.

			this.ProcessComboSelectedIndex (this.scrollList.SelectedIndex);
		}

		private void HandleMenuAccepted(object sender)
		{
			this.CloseCombo (CloseMode.Accept);
		}

		private void HandleMenuRejected(object sender)
		{
			this.CloseCombo (CloseMode.Reject);
		}



		private const string MultiSelectionChangedEvent = "MultiSelectionChanged";

		private readonly HashSet<int> selection;
		private ScrollList scrollList;
	}
}
