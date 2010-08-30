//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>GroupedItemController</c> class provides the basic functions
	/// needed to manipulate a list of items belonging to the same logical group
	/// in the user interface (e.g. all mail contacts in a person's contact list).
	/// </summary>
	public class GroupedItemController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupedItemController"/> class.
		/// </summary>
		/// <param name="items">The collection of items.</param>
		/// <param name="item">The item.</param>
		/// <param name="filter">The filter used to identify compatible items in the collection.</param>
		public GroupedItemController(System.Collections.IList items, AbstractEntity item, System.Predicate<AbstractEntity> filter)
		{
			System.Diagnostics.Debug.Assert (items.Contains (item));
			System.Diagnostics.Debug.Assert (filter (item));

			this.items  = items;
			this.item   = item;
			this.filter = filter;
		}

		private IEnumerable<AbstractEntity> CompatibleItems
		{
			get
			{
				return this.items.Cast<AbstractEntity> ().Where (x => this.filter (x));
			}
		}

		
		public int GetItemCount()
		{
			return this.CompatibleItems.Count ();
		}

		/// <summary>
		/// Gets the index of the item in the filtered collection.
		/// </summary>
		/// <returns>The index of the item.</returns>
		public int GetItemIndex()
		{
			return this.CompatibleItems.ToList ().IndexOf (this.item);
		}

		/// <summary>
		/// Sets the index of the item in the filtered collection.
		/// </summary>
		/// <param name="newIndex">The new index.</param>
		/// <returns><c>true</c> if the item changed its position in the collection; otherwise, <c>false</c>.</returns>
		public bool SetItemIndex(int newIndex)
		{
			if (this.items.IsReadOnly)
			{
				return false;
			}

			var compatibleItems = this.CompatibleItems.ToList ();

			int oldIndex  = compatibleItems.IndexOf (this.item);

			if ((newIndex == oldIndex) ||
				(newIndex == oldIndex+1))
			{
				return false;
			}

			//	Be careful: the caller is specifying an index relative to the CompatibleItems
			//	collection, not relative to all items. We have to map from the index in the
			//	filtered array to the one in the real array :

			var collection = this.items as IEntityCollection;

			if (collection == null)
			{
				this.UpdateCollection (newIndex, compatibleItems);
			}
			else
			{
				using (collection.SuspendNotifications ())
				{
					this.UpdateCollection (newIndex, compatibleItems);
				}
			}

			return true;
		}

		private void UpdateCollection(int newIndex, List<AbstractEntity> compatibleItems)
		{
			int itemCount    = compatibleItems.Count;
			int realOldIndex = this.items.IndexOf (this.item);
			int realNewIndex = newIndex == itemCount ? this.items.Count : this.items.IndexOf (compatibleItems[newIndex]);

			System.Diagnostics.Debug.Assert (realOldIndex >= 0);

			var item = this.items[realOldIndex];

			this.items.Insert (realNewIndex, item);

			if (realNewIndex < realOldIndex)
			{
				this.items.RemoveAt (realOldIndex+1);
			}
			else
			{
				this.items.RemoveAt (realOldIndex);
			}

			compatibleItems = this.CompatibleItems.ToList ();

			GroupedItemController.RenumberItemRanks (compatibleItems);
		}

		private static void RenumberItemRanks(List<AbstractEntity> compatibleItems)
		{
			for (int i = 0; i < compatibleItems.Count; i++)
			{
				IItemRank rankable = compatibleItems[i] as IItemRank;

				if ((rankable != null) &&
					(rankable.Rank.HasValue))
				{
					rankable.Rank = i;
				}
			}
		}

		
		readonly System.Collections.IList items;
		readonly AbstractEntity item;
		readonly System.Predicate<AbstractEntity> filter;
	}
}
