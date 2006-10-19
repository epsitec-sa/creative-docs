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
		object CurrentItem
		{
			get;
		}

		int CurrentPosition
		{
			get;
		}

		/// <summary>
		/// Gets the top-level groups.
		/// </summary>
		/// <value>A read-only collection of the top-level groups, which has always
		/// at least one element.</value>
		Collections.ReadOnlyObservableList<object> Groups
		{
			get;
		}

		/// <summary>
		/// Gets a collection of group description objects that describe how the items
		/// in the collection are grouped in a view.
		/// </summary>
		/// <value>The collection of group descriptions.</value>
		Collections.ObservableList<AbstractGroupDescription> GroupDescriptions
		{
			get;
		}

		/// <summary>
		/// Gets a collection of <see cref="SortDescription"/> objects that describe how
		/// the items in a collection are sorted within the groups.
		/// </summary>
		Collections.ObservableList<SortDescription> SortDescriptions
		{
			get;
		}

		/// <summary>
		/// Gets the underlying unfiltered source collection.
		/// </summary>
		/// <value>The source collection.</value>
		System.Collections.IEnumerable SourceCollection
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

		void Refresh();

		event Support.EventHandler CurrentChanged;
		event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;
	}
}
