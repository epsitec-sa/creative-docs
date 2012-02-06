//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
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
		/// <param name="collectionAccessor">The collection accessor.</param>
		/// <param name="item">The item.</param>
		/// <param name="filter">The filter used to identify compatible items in the collection.</param>
		public GroupedItemController(ICollectionAccessor collectionAccessor, AbstractEntity item, System.Predicate<AbstractEntity> filter)
		{
			this.collectionAccessor = collectionAccessor;
			this.item   = item;
			this.filter = filter ?? GroupedItemController.defaultFilter;
			
			var items = this.collectionAccessor.GetItemCollection ();
			
			System.Diagnostics.Debug.Assert (items.Contains (item));
			System.Diagnostics.Debug.Assert (filter (item));
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
		public void SetItemIndex(int newIndex)
		{
			if (this.collectionAccessor.IsReadOnly)
			{
				this.MoveItemInReadOnlyCollectionAccessor (newIndex);
			}

			var compatibleItems = this.CompatibleItems.ToList ();
			int oldIndex  = compatibleItems.IndexOf (this.item);

			if ((newIndex == oldIndex) ||
				(newIndex == oldIndex+1))
			{
				return;
			}

			//	Be careful: the caller is specifying an index relative to the CompatibleItems
			//	collection, not relative to all items. We have to map from the index in the
			//	filtered array to the one in the real array :

			var items      = this.collectionAccessor.GetItemCollection ();
			var collection = items as ISuspendCollectionChanged;

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
		}

		private IEnumerable<AbstractEntity> CompatibleItems
		{
			get
			{
				var items = this.collectionAccessor.GetItemCollection ();
				return items.Where (x => this.filter (x));
			}
		}
		
		private void MoveItemInReadOnlyCollectionAccessor(int newIndex)
		{
			var compatibleItems = this.CompatibleItems.ToList ();
			int itemCurrentIndex = compatibleItems.IndexOf (this.item);

			if ((itemCurrentIndex == newIndex) ||
				(itemCurrentIndex < 0))
			{
				return;
			}

			if (itemCurrentIndex < newIndex)
			{
				newIndex--;
			}

			compatibleItems.Remove (this.item);
			compatibleItems.Insert (newIndex, this.item);

			if (GroupedItemController.RenumberItemRanks (compatibleItems))
			{
				//	TODO: notify whomever to force Update...
			}
		}
		
		private void UpdateCollection(int newIndex, List<AbstractEntity> compatibleItems)
		{
			var items = this.collectionAccessor.GetItemCollection ();

			int itemCount    = compatibleItems.Count;
			int realOldIndex = items.IndexOf (this.item);
			int realNewIndex = newIndex >= itemCount ? -1 : items.IndexOf (compatibleItems[newIndex]);

			System.Diagnostics.Debug.Assert (realOldIndex >= 0);

			if (realOldIndex < realNewIndex)
			{
				realNewIndex--;
			}

			this.collectionAccessor.RemoveItem (this.item);

			if (realNewIndex < 0)
			{
				this.collectionAccessor.AddItem (this.item);
			}
			else
			{
				this.collectionAccessor.InsertItem (realNewIndex, this.item);
			}

			compatibleItems = this.CompatibleItems.ToList ();

			GroupedItemController.RenumberItemRanks (compatibleItems);
		}

		private static bool RenumberItemRanks(List<AbstractEntity> compatibleItems)
		{
			int changeCount = 0;

			for (int i = 0; i < compatibleItems.Count; i++)
			{
				IItemRank rankable = compatibleItems[i] as IItemRank;

				if ((rankable != null) &&
					(rankable.Rank.HasValue))
				{
					if (rankable.Rank != i)
					{
						rankable.Rank = i;
						changeCount++;
					}
				}
			}

			return changeCount > 0;
		}


		private static readonly System.Predicate<AbstractEntity> defaultFilter = x => true;

		private readonly ICollectionAccessor				collectionAccessor;
		private readonly AbstractEntity						item;
		private readonly System.Predicate<AbstractEntity>	filter;
	}
}
