﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget permet d'éditer une enumération sous une forme très compacte (une seule ligne),
	/// avec un support complet de tous les modes de cardinalité.
	/// </summary>
	public class ItemPicketCombo : TextFieldCombo, IMultipleSelection
	{
		public ItemPicketCombo()
		{
			this.IsReadOnly = true;
			this.Cardinality = BusinessLogic.EnumValueCardinality.Any;
			this.MultipleSelectionTextSeparator = ", ";

			this.selection = new HashSet<int> ();
		}

		public ItemPicketCombo(Widget embedder)
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


		public BusinessLogic.EnumValueCardinality Cardinality
		{
			get;
			set;
		}


		public string MultipleSelectionTextSeparator
		{
			//	Choix du séparateur pour afficher plusieurs sélections dans la ligne éditable.
			//	Ce séparateur n'a qu'un rôle visuel.
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

			this.UpdateText ();

			if (dirty)
			{
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

			this.UpdateText ();

			if (dirty)
			{
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

			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			this.menu = this.CreateMenu ();

			if (this.menu == null)
			{
				return;
			}

			this.menu.AutoDispose = true;
			this.menu.ShowAsComboList (this, this.MapClientToScreen (new Point (0, 0)), this.Button, Common.Widgets.Behaviors.MenuBehavior.Animate.No);

			if (this.scrollList != null)
			{
				this.scrollList.ShowSelected (ScrollShowMode.Center);
			}

			this.menu.Accepted += this.HandleMenuAccepted;
			this.menu.Rejected += this.HandleMenuRejected;

			if (this.scrollList != null)
			{
				this.scrollList.SelectedItemChanged += this.HandleScrollListSelectedItemChanged;
				this.scrollList.SelectionActivated  += this.HandleScrollListSelectionActivated;
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
				this.scrollList.SelectionActivated  -= this.HandleScrollListSelectionActivated;
				this.scrollList.SelectedItemChanged -= this.HandleScrollListSelectedItemChanged;

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
				this.OnSelectedItemChanged ();
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
			//	est validée par un clic de souris.
			var sels = this.GetSortedSelection ();
			int index = this.MapComboListToIndex (sel);

			if (index >= 0)
			{
				if (this.Cardinality == BusinessLogic.EnumValueCardinality.Any)
				{
					if (sels.Contains (index))
					{
						this.RemoveSelection (new int[] { index });
					}
					else
					{
						this.AddSelection (new int[] { index });
					}
				}
				else if (this.Cardinality == BusinessLogic.EnumValueCardinality.ZeroOrOne)
				{
					if (index == 0)
					{
						this.ClearSelection ();
					}
					else
					{
						index--;

						if (sels.Contains (index))
						{
							this.RemoveSelection (new int[] { index });
						}
						else
						{
							this.ClearSelection ();
							this.AddSelection (new int[] { index });
						}
					}
				}
				else if (this.Cardinality == BusinessLogic.EnumValueCardinality.AtLeastOne)
				{
					if (sels.Contains (index))
					{
						if (this.SelectionCount > 1)
						{
							this.RemoveSelection (new int[] { index });
						}
					}
					else
					{
						this.AddSelection (new int[] { index });
					}
				}
				else if (this.Cardinality == BusinessLogic.EnumValueCardinality.ExactlyOne)
				{
					this.ClearSelection ();
					this.AddSelection (new int[] { index });
				}

				this.menu.Behavior.Accept ();
			}
		}

		protected override void CopyItemsToComboList(Common.Widgets.Collections.StringCollection list)
		{
			var sel = this.GetSortedSelection ();

			if (this.Cardinality == BusinessLogic.EnumValueCardinality.ZeroOrOne)
			{
				string icon;

				if (sel.Count == 0)
				{
					icon = "Button.RadioYes";
				}
				else
				{
					icon = "Button.RadioNo";
				}

				list.Add (null, string.Concat (Misc.GetResourceIconImageTag (icon, -4), " ", "Aucun"));
			}

			for (int i = 0; i < this.items.Count; i++)
			{
				string name = this.items.GetKey (i);
				string text = this.items[i];

				if (this.ListTextConverter != null)
				{
					text = this.ListTextConverter (text);
				}

				string icon;

				if (this.Cardinality == BusinessLogic.EnumValueCardinality.ExactlyOne ||
					this.Cardinality == BusinessLogic.EnumValueCardinality.ZeroOrOne)
				{
					if (sel.Contains (i))
					{
						icon = "Button.RadioYes";
					}
					else
					{
						icon = "Button.RadioNo";
					}
				}
				else
				{
					if (sel.Contains (i))
					{
						icon = "Button.CheckYes";
					}
					else
					{
						icon = "Button.CheckNo";
					}
				}

				list.Add (name, string.Concat (Misc.GetResourceIconImageTag (icon, -4), " ", text));
			}
		}

		public void UpdateText()
		{
			var sels = this.GetSortedSelection ();
			var list = new List<string> ();

			foreach (int sel in sels)
			{
				list.Add (this.items[sel]);
			}

			if (this.Cardinality == BusinessLogic.EnumValueCardinality.ZeroOrOne && list.Count == 0)
			{
				list.Add ("Aucun");
			}

			this.Text = Misc.Join (this.MultipleSelectionTextSeparator, list);
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

			var handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> (ItemPicketCombo.MultiSelectionChangedEvent);
			var e = new DependencyPropertyChangedEventArgs ("MultiSelection");

			if (handler != null)
			{
				handler (this, e);
			}
		}


		private void HandleScrollListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.

			this.ProcessComboActivatedIndex (this.scrollList.SelectedItemIndex);
		}

		private void HandleScrollListSelectedItemChanged(object sender)
		{
			//	L'utilisateur a simplement déplacé la souris dans la liste.
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
