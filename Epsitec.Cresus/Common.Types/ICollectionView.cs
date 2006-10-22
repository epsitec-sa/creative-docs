//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ICollectionView</c> interface enables collections to have functionalities
	/// such as current record management, sorting, filtering and grouping.
	/// </summary>
	public interface ICollectionView
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
		/// A read-only collection of the top level groups or <c>null</c>
		/// if there are no groups configured for this view.
		/// </value>
		Collections.ReadOnlyObservableList<CollectionViewGroup> Groups
		{
			get;
		}

		/// <summary>
		/// Gets the items. The collection might be sorted and filtered,
		/// depending on the settings.
		/// </summary>
		/// <value>A read-only collection of the items.</value>
		System.Collections.IList Items
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

		event Support.EventHandler CurrentChanged;
		event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;
	}
}
