//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListSelection
	{
		public ItemListSelection(ItemCache cache)
		{
			this.cache = cache;
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
						return this.DeselectOne (index);

					case ItemSelectionMode.OneOrMore:
						if (this.selectedItemCount < 2)
						{
							return false;
						}
						else
						{
							return this.DeselectOne (index);
						}
				}
			}

			if (selection == ItemSelection.Select)
			{
				switch (this.cache.Features.SelectionMode)
				{
					case ItemSelectionMode.ZeroOrOne:
					case ItemSelectionMode.ExactlyOne:
						if (this.SelectOne (index))
						{
							if (this.selectedItemCount > 1)
							{
								this.DeselectAllButOne (index);
							}
							return true;
						}
						return false;

					case ItemSelectionMode.OneOrMore:
					case ItemSelectionMode.Multiple:
						return this.SelectOne (index);

					case ItemSelectionMode.None:
						return false;
				}
			}

			throw new System.InvalidOperationException (string.Format ("Select does not understand {0}", selection.GetQualifiedName ()));
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


		private bool DeselectAllButOne(int index)
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
					this.selectedItemCount--;
					changes++;
				}
			}

			return changes > 0;
		}

		private bool SelectOne(int index)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Select)))
			{
				this.selectedItemCount++;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool DeselectOne(int index)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Deselect)))
			{
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

		
		private readonly ItemCache				cache;

		private int								selectedItemCount;
	}
}
