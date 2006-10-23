//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionView</c> class represents a view of a collection. Views implement
	/// grouping, sorting, filtering and the concept of a current item.
	/// </summary>
	public class CollectionView : ICollectionView, System.Collections.IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged, System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionView"/> class.
		/// </summary>
		/// <param name="sourceList">The source list.</param>
		public CollectionView(System.Collections.IList sourceList)
		{
			this.sourceList = sourceList;
			this.rootGroup = new CollectionViewGroup (null, null);
			this.itemCount = this.sourceList == null ? 0 : this.sourceList.Count;

			INotifyCollectionChanged changed = this.sourceList as INotifyCollectionChanged;
			
			if (changed != null)
			{
				//	TODO: use a weak delegate in order to avoid trouble if nobody disposes the view
				
				changed.CollectionChanged += this.HandleCollectionChanged;
				this.hasDynamicSource = true;
			}

			this.SetCurrentToPosition (0);
		}

		#region ICollectionView Members

		/// <summary>
		/// Gets the underlying (unfiltered and unsorted) source collection.
		/// </summary>
		/// <value>The source collection.</value>
		public System.Collections.IEnumerable	SourceCollection
		{
			get
			{
				return this.sourceList;
			}
		}

		public object							CurrentItem
		{
			get
			{
				return this.currentItem;
			}
		}

		public int								CurrentPosition
		{
			get
			{
				int position = this.currentPosition;
				int count    = this.Count;

				if (position > count)
				{
					position = count;
				}
				
				return position;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <c>CurrentItem</c> of this view is before
		/// the first item.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <c>CurrentItem</c> is before the first item; otherwise, <c>false</c>.
		/// </value>
		public bool								IsCurrentBeforeFirst
		{
			get
			{
				return this.currentPosition < 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <c>CurrentItem</c> of this view is after
		/// the last item.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <c>CurrentItem</c> is after the last item; otherwise, <c>false</c>.
		/// </value>
		public bool								IsCurrentAfterLast
		{
			get
			{
				return this.currentPosition == System.Int32.MaxValue;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this (filtered) view is empty.
		/// </summary>
		/// <value><c>true</c> if this view is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		/// <summary>
		/// Gets the items. The collection might be sorted and filtered,
		/// depending on the settings.
		/// </summary>
		/// <value>A read-only collection of the items.</value>
		public System.Collections.IList			Items
		{
			get
			{
				this.RefreshWhenInvalidated ();

				if (this.sortedList != null)
				{
					return this.sortedList;
				}
				else if (this.sourceList != null)
				{
					return this.sourceList;
				}
				else
				{
					return Collections.EmptyList<object>.Instance;
				}
			}
		}

		/// <summary>
		/// Gets the top level groups.
		/// </summary>
		/// <value>
		/// A read-only collection of the top level groups; it is empty
		/// if there are no groups configured for this view.
		/// </value>
		public Collections.ReadOnlyObservableList<CollectionViewGroup> Groups
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.dirtyGroups == false, "Dirty groups");
				
				if (this.readOnlyGroups == null)
				{
					this.readOnlyGroups = new Collections.ReadOnlyObservableList<CollectionViewGroup> (this.rootGroup.GetSubgroups ());
				}

				return this.readOnlyGroups;
			}
		}

		/// <summary>
		/// Gets a collection of group description objects that describe how
		/// the items in the collection are grouped in a view.
		/// </summary>
		/// <value>The collection of group descriptions.</value>
		public Collections.ObservableList<GroupDescription> GroupDescriptions
		{
			get
			{
				if (this.groupDescriptions == null)
				{
					this.groupDescriptions = new Collections.ObservableList<GroupDescription> ();
					this.groupDescriptions.CollectionChanged += this.HandleGroupDescriptionsCollectionChanged;
				}
				
				return this.groupDescriptions;
			}
		}

		/// <summary>
		/// Gets a collection of <see cref="SortDescription"/> objects that
		/// describe how the items in a collection are sorted within the groups.
		/// </summary>
		/// <value>The sort descriptions.</value>
		public Collections.ObservableList<SortDescription> SortDescriptions
		{
			get
			{
				if (this.sortDescriptions == null)
				{
					this.sortDescriptions = new Collections.ObservableList<SortDescription> ();
					this.sortDescriptions.CollectionChanged += this.HandleSortDescriptionsCollectionChanged;
				}

				return this.sortDescriptions;
			}
		}

		/// <summary>
		/// Gets or sets a callback used to determine if an item is suitable for
		/// inclusion in the view.
		/// </summary>
		/// <value>
		/// A method used to determine if an item is suitable for inclusion in the view.
		/// </value>
		public System.Predicate<object>			Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				if (this.filter != value)
				{
					this.filter = value;
					
					this.InvalidateSortedList ();
				}
			}
		}


		/// <summary>
		/// Refreshes the contents of the view.
		/// </summary>
		public void Refresh()
		{
			this.dirtyGroups = true;
			this.dirtySortedList = true;
			
			this.RefreshContents ();
		}

		/// <summary>
		/// Enter a defer cycle which can be used to do multiple changes to
		/// the underlying source collection or to the view itself without
		/// having multiple refreshes.
		/// </summary>
		/// <returns>
		/// An instance of <c>System.IDisposable</c> which must be
		/// disposed of at the end of the modifications to exit the deferred
		/// refresh mode.
		/// </returns>
		public System.IDisposable DeferRefresh()
		{
			return new DeferManager (this);
		}

		/// <summary>
		/// Sets the specified item as the <see cref="CurrentItem"/> in the view.
		/// </summary>
		/// <param name="item">The item to set as the <see cref="CurrentItem"/>.</param>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentTo(object item)
		{
			this.VerfiyRefreshNotDeferred ();

			if (object.Equals (this.CurrentItem, item))
			{
				return this.IsCurrentInView;
			}

			int position = -1;
			
			if (this.PassesFilter (item))
			{
				position = this.Items.IndexOf (item);
			}

			return this.MoveCurrentToPosition (position);
		}

		/// <summary>
		/// Sets the item at the specified position to be the <see cref="CurrentItem"/> in the view.
		/// </summary>
		/// <param name="position">The position to set the <see cref="CurrentItem"/> to.</param>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentToPosition(int position)
		{
			this.VerfiyRefreshNotDeferred ();

			if ((position < -1) ||
				(position > this.Count))
			{
				throw new System.ArgumentOutOfRangeException ("position");
			}

			if ((this.CurrentPosition != position) &&
				(this.AcceptsChangingCurrentPosition ()))
			{
				bool ok = this.SetCurrentToPosition (position);
				this.OnCurrentChanged ();
				return ok;
			}
			else
			{
				return this.IsCurrentInView;
			}
		}

		/// <summary>
		/// Sets the first item in the view as the <see cref="CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentToFirst()
		{
			this.VerfiyRefreshNotDeferred ();
			return this.MoveCurrentToPosition (0);
		}

		/// <summary>
		/// Sets the last item in the view as the <see cref="CurrentItem"/>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentToLast()
		{
			this.VerfiyRefreshNotDeferred ();
			return this.MoveCurrentToPosition (this.Count-1);
		}

		/// <summary>
		/// Sets the item after the <see cref="CurrentItem"/> as the <c>CurrentItem</c>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentToNext()
		{
			this.VerfiyRefreshNotDeferred ();
			return this.IsCurrentAfterLast ? false : this.MoveCurrentToPosition (this.currentPosition+1);
		}

		/// <summary>
		/// Sets the item before the <see cref="CurrentItem"/> as the <c>CurrentItem</c>.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the resulting <c>CurrentItem</c> is within the view; otherwise, <c>false</c>.
		/// </returns>
		public bool MoveCurrentToPrevious()
		{
			this.VerfiyRefreshNotDeferred ();
			return this.IsCurrentBeforeFirst ? false : this.MoveCurrentToPosition (System.Math.Min (this.Count, this.currentPosition)-1);
		}
		
		public event Support.EventHandler CurrentChanged;

		public event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;

		#endregion

		/// <summary>
		/// Gets the number of items in the view. This is a cached value which
		/// might be incorrect when changes are deferred.
		/// </summary>
		/// <value>The number of items.</value>
		public int								Count
		{
			get
			{
				return this.itemCount;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this view has sort descriptions.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this view has sort descriptions; otherwise, <c>false</c>.
		/// </value>
		public bool								HasSortDescriptions
		{
			get
			{
				return (this.sortDescriptions != null)
					&& (this.sortDescriptions.Count > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this view has group descriptions.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this view has group descriptions; otherwise, <c>false</c>.
		/// </value>
		public bool								HasGroupDescriptions
		{
			get
			{
				return (this.groupDescriptions != null)
				    && (this.groupDescriptions.Count > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this view has a filter.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this view has a filter; otherwise, <c>false</c>.
		/// </value>
		public bool								HasFilter
		{
			get
			{
				return this.filter != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the refresh operations of this view are deferred.
		/// </summary>
		/// <value><c>true</c> if the refresh operations are deferred; otherwise, <c>false</c>.</value>
		public bool								IsRefreshDeferred
		{
			get
			{
				return this.deferCounter > 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <c>CurrentItem</c> belongs to this view.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <c>CurrentItem</c> belongs to this view; otherwise, <c>false</c>.
		/// </value>
		protected bool							IsCurrentInView
		{
			get
			{
				return (this.currentPosition >= 0)
				    && (this.currentPosition < this.Count);
			}
		}

		#region INotifyCollectionChanged Members

		public event Support.EventHandler<CollectionChangedEventArgs> CollectionChanged;

		#endregion

		#region INotifyPropertyChanged Members

		public event Support.EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			foreach (object group in this.Groups)
			{
				yield return group;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion
		
		/// <summary>
		/// Returns a value indicating whether the specified item is valid
		/// with respect to the filter.
		/// </summary>
		/// <param name="item">The item to test.</param>
		/// <returns>
		/// 	<c>true</c> if the item is valid with respect to the filter; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool PassesFilter(object item)
		{
			if (this.filter != null)
			{
				return this.filter (item);
			}
			else
			{
				return true;
			}
		}

		protected bool AcceptsChangingCurrentPosition()
		{
			CurrentChangingEventArgs e = new CurrentChangingEventArgs (true);
			this.OnCurrentChanging (e);
			return e.Cancel == false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.hasDynamicSource)
				{
					INotifyCollectionChanged changed = this.sourceList as INotifyCollectionChanged;

					if (changed != null)
					{
						changed.CollectionChanged -= this.HandleCollectionChanged;
						this.hasDynamicSource = false;
					}
				}
			}
		}
		
		protected virtual void OnCurrentChanged()
		{
			if (this.CurrentChanged != null)
			{
				this.CurrentChanged (this);
			}
		}

		protected virtual void OnCurrentChanging(CurrentChangingEventArgs e)
		{
			if (this.CurrentChanging != null)
			{
				this.CurrentChanging (this, e);
			}
		}

		private void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.InvalidateSortedList ();
		}
		
		private void HandleGroupDescriptionsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.InvalidateGroups ();
		}

		private void HandleSortDescriptionsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.InvalidateSortedList ();
		}

		private void GroupItemsInList()
		{
			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

			GroupDescription[] rules = this.groupDescriptions.ToArray ();

			System.Diagnostics.Debug.Assert (rules.Length > 0);

			GroupNode root = new GroupNode (null);

			foreach (object item in this.sortedList)
			{
				CollectionView.ClassifyItemIntoGroups (item, culture, rules, 0, root);
			}

			CollectionView.GenerateSubgroups (root, this.rootGroup);
		}

		private static void ClassifyItemIntoGroups(object item, System.Globalization.CultureInfo culture, GroupDescription[] rules, int level, GroupNode node)
		{
			//	Apply the specified level in the grouping rules to the item. When
			//	we reach the last rule, we record the item in the group nodes tree.

			int nextLevel = level+1;
			
			//	An item may have one or more grouping names. If an item has more than
			//	one grouping name, then it will appear within different groups.
			
			foreach (string name in rules[level].GetGroupNamesForItem (item, culture))
			{
				GroupNode subnode = node.GetSubnode (name);

				if (nextLevel < rules.Length)
				{
					CollectionView.ClassifyItemIntoGroups (item, culture, rules, nextLevel, subnode);
				}
				else
				{
					subnode.Add (item);
				}
			}
		}

		private static void GenerateSubgroups(GroupNode node, CollectionViewGroup parentGroup)
		{
			//	Generate the subgroup collections based on the group node tree.

			parentGroup.ClearSubgroups ();
			parentGroup.ClearItems ();
			
			if (node.HasSubnodes)
			{
				List<CollectionViewGroup> groups = new List<CollectionViewGroup> ();
				
				foreach (GroupNode subnode in node.Subnodes)
				{
					CollectionViewGroup group = new CollectionViewGroup (subnode.Name, parentGroup);
					CollectionView.GenerateSubgroups (subnode, group);
					groups.Add (group);
				}

				//	Add all subgroups in one single insertion in order to avoid
				//	multiple change events in the parent group.
				
				parentGroup.GetSubgroups ().AddRange (groups);
			}

			if (node.HasItems)
			{
				parentGroup.GetItems ().AddRange (node.Items);
			}
		}

		private void SortItemsList()
		{
			if ((this.sortDescriptions != null) &&
				(this.itemType != null))
			{
				System.Comparison<object>[] comparisons = new System.Comparison<object>[this.sortDescriptions.Count];

				for (int i = 0; i < this.sortDescriptions.Count; i++)
				{
					comparisons[i] = CollectionView.CreateComparison (this.itemType, this.sortDescriptions[i]);
				}

				if (comparisons.Length == 1)
				{
					this.sortedList.Sort (comparisons[0]);
				}
				else if (comparisons.Length > 1)
				{
					this.sortedList.Sort
						(
							delegate (object x, object y)
							{
								for (int i = 0; i < comparisons.Length; i++)
								{
									int result = comparisons[i] (x, y);

									if (result != 0)
									{
										return result;
									}
								}

								return 0;
							}
						);
				}
			}
		}

		private void FillItemsList()
		{
			this.itemType = null;

			foreach (object item in this.sourceList)
			{
				if (item == null)
				{
					throw new System.InvalidOperationException ("Source collection contains null items");
				}
				else if (this.PassesFilter (item))
				{
					this.sortedList.Add (item);

					System.Type itemType = item.GetType ();

					if (this.itemType == null)
					{
						this.itemType = itemType;
					}
					else
					{
						if (this.itemType != itemType)
						{
							throw new System.InvalidOperationException (string.Format ("Source collection not orthogonal; found type {0} and {1}", this.itemType.Name, itemType.Name));
						}
					}
				}
			}
		}

		private void BeginDefer()
		{
			System.Threading.Interlocked.Increment (ref this.deferCounter);
		}

		private void EndDefer()
		{
			if (System.Threading.Interlocked.Decrement (ref this.deferCounter) == 0)
			{
				this.RefreshContents ();
			}
		}

		private void VerfiyRefreshNotDeferred()
		{
			if (this.IsRefreshDeferred)
			{
				throw new System.InvalidOperationException ("Invalid operation while refresh is deferred");
			}
		}

		private void InvalidateSortedList()
		{
			this.dirtySortedList = true;
			this.dirtyGroups = true;

			this.RefreshWhenInvalidated ();
		}

		private void InvalidateGroups()
		{
			this.dirtyGroups = true;
			this.RefreshWhenInvalidated ();
		}

		private void RefreshWhenInvalidated()
		{
			if (this.deferCounter == 0)
			{
				this.RefreshContents ();
			}
		}

		private void RefreshContents()
		{
			if ((this.dirtyGroups == false) &&
				(this.dirtySortedList == false))
			{
				return;
			}

			object currentItem     = this.currentItem;
			int    currentPosition = this.currentPosition;
			
			bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
			bool isCurrentAfterLast   = this.IsCurrentAfterLast;

			this.OnCurrentChanging (new CurrentChangingEventArgs ());
			
			//	Rebuild the filtered/sorted list :
			
			if (this.dirtySortedList)
			{
				if ((this.HasFilter) ||
					(this.HasSortDescriptions))
				{
					if (this.sortedList == null)
					{
						this.sortedList = new List<object> ();
					}
					else
					{
						this.sortedList.Clear ();
					}
					
					this.FillItemsList ();
					this.SortItemsList ();
					
					this.itemCount = this.sortedList.Count;
				}
				else
				{
					this.sortedList = null;
					this.itemCount  = this.sourceList == null ? 0 : this.sourceList.Count;
				}

				this.dirtySortedList = false;
			}

			//	Rebuild the groups :

			if (this.dirtyGroups)
			{
				if (this.HasGroupDescriptions)
				{
					this.GroupItemsInList ();
				}
				else
				{
					this.rootGroup.ClearSubgroups ();
				}
				
				this.dirtyGroups = false;
			}

			//	Update the current item since the contents might have moved :

			if ((this.IsEmpty) ||
				(isCurrentBeforeFirst))
			{
				this.SetCurrentToPosition (-1);
			}
			else if (isCurrentAfterLast)
			{
				this.SetCurrentToPosition (System.Int32.MaxValue);
			}
			else if (currentItem != null)
			{
				int position = this.Items.IndexOf (currentItem);
				
				if (position < 0)
				{
					position = System.Math.Min (this.Count-1, currentPosition);
				}

				this.SetCurrentToPosition (position);
			}

			this.OnCurrentChanged ();

			if (this.IsCurrentAfterLast != isCurrentAfterLast)
			{
				this.OnPropertyChanged ("IsCurrentAfterLast");
			}
			if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
			{
				this.OnPropertyChanged ("IsCurrentBeforeFirst");
			}
			if (this.CurrentPosition != currentPosition)
			{
				this.OnPropertyChanged ("CurrentPosition");
			}
			if (this.CurrentItem != currentItem)
			{
				this.OnPropertyChanged ("CurrentItem");
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged (new DependencyPropertyChangedEventArgs (propertyName));
		}

		protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged (this, e);
			}
		}

		private bool SetCurrentToPosition(int position)
		{
			if ((position < 0) ||
				(this.IsEmpty))
			{
				this.currentPosition = -1;
				this.currentItem     = null;
				return false;
			}
			else if (position >= this.Count)
			{
				this.currentPosition = System.Int32.MaxValue;
				this.currentItem     = null;
				return false;
			}
			else
			{
				this.currentPosition = position;
				this.currentItem     = this.Items[position];
				return true;
			}
		}

		private static System.Comparison<object> CreateComparison(System.Type type, SortDescription sort)
		{
			string propertyName = sort.PropertyName;

			Support.PropertyComparer comparer = Support.DynamicCodeFactory.CreatePropertyComparer (type, propertyName);

			switch (sort.Direction)
			{
				case ListSortDirection.Ascending:
					return delegate (object x, object y)
					{
						return comparer (x, y);
					};

				case ListSortDirection.Descending:
					return delegate (object x, object y)
					{
						return -comparer (x, y);
					};
			}

			throw new System.ArgumentException ("Invalid sort direction");
		}


		#region GroupNode Class

		private sealed class GroupNode
		{
			public GroupNode(string name)
			{
				this.name = name;
			}

			public bool HasItems
			{
				get
				{
					return (this.leaves != null) && (this.leaves.Count > 0);
				}
			}

			public bool HasSubnodes
			{
				get
				{
					return (this.subnodes != null) && (this.subnodes.Count > 0);
				}
			}

			public IEnumerable<object> Items
			{
				get
				{
					if (this.leaves == null)
					{
						return Collections.EmptyEnumerable<object>.Instance;
					}
					else
					{
						return this.leaves;
					}
				}
			}

			public IEnumerable<GroupNode> Subnodes
			{
				get
				{
					if (this.subnodes == null)
					{
						return Collections.EmptyEnumerable<GroupNode>.Instance;
					}
					else
					{
						return this.subnodes.Values;
					}
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public GroupNode GetSubnode(string name)
			{
				if (this.subnodes == null)
				{
					this.subnodes = new Dictionary<string, GroupNode> ();
				}

				GroupNode subnode;

				if (this.subnodes.TryGetValue (name, out subnode))
				{
					return subnode;
				}

				subnode = new GroupNode (name);
				this.subnodes[name] = subnode;
				return subnode;
			}

			public void Add(object item)
			{
				if (this.leaves == null)
				{
					this.leaves = new List<object> ();
				}

				this.leaves.Add (item);
			}

			private string name;
			private Dictionary<string, GroupNode> subnodes;
			private List<object> leaves;
		}

		#endregion

		#region DeferManager Class

		private sealed class DeferManager : System.IDisposable
		{
			public DeferManager(CollectionView view)
			{
				this.view = view;
				this.view.BeginDefer ();
			}

			#region IDisposable Members

			void System.IDisposable.Dispose()
			{
				if (this.view != null)
				{
					CollectionView view = this.view;
					this.view = null;
					view.EndDefer ();
				}
			}

			#endregion

			private CollectionView view;
		}

		#endregion

		
		private System.Collections.IList		sourceList;
		private List<object>					sortedList;
		private System.Type						itemType;
		private int								itemCount;
		private bool							dirtyGroups;
		private bool							dirtySortedList;
		private bool							hasDynamicSource;
		private int								deferCounter;
		private int								currentPosition;
		private object							currentItem;
		private CollectionViewGroup				rootGroup;
		private Collections.ReadOnlyObservableList<CollectionViewGroup> readOnlyGroups;
		private Collections.ObservableList<GroupDescription> groupDescriptions;
		private Collections.ObservableList<SortDescription> sortDescriptions;
		private System.Predicate<object>		filter;
	}
}
