//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ICollectionView</c> interface enables collections to have functionalities
	/// such as current record management, sorting, filtering and grouping.
	/// </summary>
	public interface ICollectionView : INotifyCollectionChanged, INotifyCurrentChanged
	{
		/// <summary>
		/// Gets the underlying (unfiltered and unsorted) source collection.
		/// </summary>
		/// <value>The source collection.</value>
		System.Collections.IEnumerable SourceCollection
		{
			get;
		}

		object CurrentItem
		{
			get;
		}

		int CurrentPosition
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the <c>CurrentItem</c> of this view is before
		/// the first item.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <c>CurrentItem</c> is before the first item; otherwise, <c>false</c>.
		/// </value>
		bool IsCurrentBeforeFirst
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the <c>CurrentItem</c> of this view is after
		/// the last item.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <c>CurrentItem</c> is after the last item; otherwise, <c>false</c>.
		/// </value>
		bool IsCurrentAfterLast
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this (filtered) view is empty.
		/// </summary>
		/// <value><c>true</c> if this view is empty; otherwise, <c>false</c>.</value>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// Gets the top level groups.
		/// </summary>
		/// <value>
		/// A read-only collection of the top level groups; it is empty
		/// if there are no groups configured for this view.
		/// </value>
		Collections.ReadOnlyList<CollectionViewGroup> Groups
		{
			get;
		}

		/// <summary>
		/// Gets the items. The collection will be sorted and filtered,
		/// depending on the settings.
		/// </summary>
		/// <value>A read-only collection of the items.</value>
		System.Collections.IList Items
		{
			get;
		}

		/// <summary>
		/// Gets the items synchronization object. Locking on this object ensures
		/// that nobody asynchronously modifies the <c>Items</c> or <c>Groups</c>
		/// collections.
		/// </summary>
		/// <value>The items synchronization object.</value>
		object ItemsSyncRoot
		{
			get;
		}

		/// <summary>
		/// Gets a collection of group description objects that describe how
		/// the items in the collection are grouped in a view.
		/// </summary>
		/// <value>The collection of group descriptions.</value>
		Collections.ObservableList<GroupDescription> GroupDescriptions
		{
			get;
		}

		/// <summary>
		/// Gets a collection of <see cref="SortDescription"/> objects that
		/// describe how the items in a collection are sorted within the groups.
		/// </summary>
		/// <value>The sort descriptions.</value>
		Collections.ObservableList<SortDescription> SortDescriptions
		{
			get;
		}

		/// <summary>
		/// Gets or sets a callback used to determine if an item is suitable for
		/// inclusion in the view.
		/// </summary>
		/// <value>
		/// A method used to determine if an item is suitable for inclusion in the view.
		/// </value>
		System.Predicate<object> Filter
		{
			get;
			set;
		}

		/// <summary>
		/// Sets the specified item as the <see cref="CurrentItem"/> in the view.
		/// </summary>
		/// <param name="item">The item to set as the <see cref="CurrentItem"/>.</param>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentTo(object item);

		/// <summary>
		/// Sets the item at the specified position to be the <see cref="CurrentItem"/> in the view.
		/// </summary>
		/// <param name="position">The position to set the <see cref="CurrentItem"/> to.</param>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentToPosition(int position);

		/// <summary>
		/// Sets the first item in the view as the <see cref="CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentToFirst();

		/// <summary>
		/// Sets the last item in the view as the <see cref="CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentToLast();

		/// <summary>
		/// Sets the item after the <see cref="CurrentItem"/> as the <c>CurrentItem</c>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentToNext();

		/// <summary>
		/// Sets the item before the <see cref="CurrentItem"/> as the <c>CurrentItem</c>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		bool MoveCurrentToPrevious();

		/// <summary>
		/// Refreshes the contents of the view.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Enter a defer cycle which can be used to do multiple changes to
		/// the underlying source collection or to the view itself without
		/// having multiple refreshes.
		/// </summary>
		/// <returns>An instance of <c>System.IDisposable</c> which must be
		/// disposed of at the end of the modifications to exit the deferred
		/// refresh mode.</returns>
		System.IDisposable DeferRefresh();
	}
}
