//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListSelection : System.IDisposable
	{
		public ItemListSelection(ItemCache cache)
		{
			this.cache = cache;
			this.cache.ResetFired += this.HandleCacheResetFired;
		}


		public int								SelectedItemCount
		{
			get
			{
				return this.selectedItemCount;
			}
		}

		
		public bool Select(int index, ItemSelection selection)
		{
			if (index == -1)
			{
				return false;
			}

			var e = new ItemListSelectionEventArgs ();

			if (this.Select (index, selection, e))
			{
				if (e.Count > 0)
				{
					this.OnSelectionChanged (e);
				}

				return true;
			}

			return false;
		}

		public bool IsSelected(int index)
		{
			if (index < 0)
			{
				return false;
			}
			else
			{
				return this.cache.GetItemState (index, ItemStateDetails.Flags).Selected;
			}
		}

		
		public IEnumerable<int> GetSelectedIndexes()
		{
			var flags = ItemStateDetails.Flags | ItemStateDetails.IgnoreNull;
			int count = this.cache.ItemCount;

			for (int index = 0; index < count; index++)
			{
				var state = this.cache.GetItemState (index, flags);

				if ((state != null) &&
					(state.Selected))
				{
					yield return index;
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
		}

		#endregion

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.cache.ResetFired -= this.HandleCacheResetFired;
			}
		}

		private bool Select(int index, ItemSelection selection, ItemListSelectionEventArgs e)
		{
			if (selection == ItemSelection.Toggle)
			{
				selection = this.IsSelected (index) ? ItemSelection.Deselect : ItemSelection.Select;
			}

			if (selection == ItemSelection.Deselect)
			{
				switch (this.cache.Features.SelectionMode)
				{
					case ItemSelectionMode.ExactlyOne:
					case ItemSelectionMode.None:
						return false;

					case ItemSelectionMode.Multiple:
					case ItemSelectionMode.ZeroOrOne:
						return this.DeselectOne (index, e);

					case ItemSelectionMode.OneOrMore:
						if (this.selectedItemCount < 2)
						{
							return false;
						}
						else
						{
							return this.DeselectOne (index, e);
						}
				}
			}

			if (selection == ItemSelection.Select)
			{
				switch (this.cache.Features.SelectionMode)
				{
					case ItemSelectionMode.ZeroOrOne:
					case ItemSelectionMode.ExactlyOne:
						if (this.SelectOne (index, e))
						{
							if (this.selectedItemCount > 1)
							{
								this.DeselectAllButOne (index, e);
							}
							return true;
						}
						return false;

					case ItemSelectionMode.OneOrMore:
					case ItemSelectionMode.Multiple:
						return this.SelectOne (index, e);

					case ItemSelectionMode.None:
						return false;
				}
			}

			throw new System.InvalidOperationException (string.Format ("Select does not understand {0}", selection.GetQualifiedName ()));
		}

		private bool DeselectAllButOne(int index, ItemListSelectionEventArgs e)
		{
			int changes = 0;
			int count   = this.cache.ItemCount;

			for (int i = 0; i < count; i++)
			{
				if (i == index)
				{
					continue;
				}

				if (this.ChangeFlagState (i, x => x.Select (ItemSelection.Deselect), ItemStateDetails.IgnoreNull))
				{
					e.Add (i, false);
					this.selectedItemCount--;
					changes++;
				}
			}

			return changes > 0;
		}

		private bool SelectOne(int index, ItemListSelectionEventArgs e)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Select)))
			{
				e.Add (index, true);
				this.selectedItemCount++;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool DeselectOne(int index, ItemListSelectionEventArgs e)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Deselect)))
			{
				e.Add (index, false);
				this.selectedItemCount--;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ChangeFlagState(int index, System.Action<ItemState> action, ItemStateDetails flags = ItemStateDetails.None)
		{
			flags &= ItemStateDetails.FlagMask;

			var state = this.cache.GetItemState (index, ItemStateDetails.Flags | flags);

			if (state == null)
			{
				return false;
			}

			var copy  = state.Clone ();

			action (copy);

			if (state.Equals (copy))
			{
				return false;
			}

			this.cache.SetItemState (index, copy, ItemStateDetails.Flags);

			return true;
		}

		private void OnSelectionChanged(ItemListSelectionEventArgs e)
		{
			var handler = this.SelectionChanged;
			handler.Raise (this, e);
		}

		private void HandleCacheResetFired(object sender)
		{
			this.selectedItemCount = 0;
		}


		public event EventHandler<ItemListSelectionEventArgs> SelectionChanged;

		
		private readonly ItemCache				cache;

		private int								selectedItemCount;
	}
}
