//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionView</c> class represents a view of a collection. Views implement
	/// grouping, sorting, filtering and the concept of a current item.
	/// </summary>
	public class CollectionView : ICollectionView, System.Collections.IEnumerable, INotifyCollectionChanged
	{
		public CollectionView(System.Collections.IList sourceList)
		{
			this.sourceList = sourceList;
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
				return this.currentPosition;
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
				return this.Items.Count == 0;
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
		/// A read-only collection of the top level groups or <c>null</c>
		/// if there are no groups configured for this view.
		/// </value>
		public Collections.ReadOnlyObservableList<CollectionViewGroup> Groups
		{
			get
			{
				if ((this.readOnlyGroups == null) &&
					(this.HasGroupDescriptions))
				{
					this.Refresh ();
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
				this.filter = value;
			}
		}


		/// <summary>
		/// Refreshes the contents of the view.
		/// </summary>
		public void Refresh()
		{
			//	TODO: add suspend/resume for all observable collections

			if ((this.HasFilter) ||
				(this.HasSortDescriptions))
			{
				this.sortedList = new List<object> ();
				this.FillItemsList ();
				this.SortItemsList ();
			}
			else
			{
				this.sortedList = null;
			}

			if (this.HasGroupDescriptions)
			{
				this.GroupItemsInList ();
			}
			else
			{
				this.readOnlyGroups = null;
			}
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
			//	TODO: ...
			
			return null;
		}

		public event Support.EventHandler CurrentChanged;

		public event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;

		#endregion

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

		#region INotifyCollectionChanged Members

		public event Support.EventHandler<CollectionChangedEventArgs> CollectionChanged;

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

		private void GroupItemsInList()
		{
			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;

			GroupDescription[] rules = this.groupDescriptions.ToArray ();

			System.Diagnostics.Debug.Assert (rules.Length > 0);

			GroupNode root = new GroupNode (null);

			foreach (object item in this.sortedList)
			{
				CollectionView.ProcessGroup (item, culture, rules, 0, root);
			}

			this.rootGroup = new CollectionViewGroup (null, null);

			this.PostProcessGroup (root, this.rootGroup);

			this.readOnlyGroups = new Collections.ReadOnlyObservableList<CollectionViewGroup> (this.rootGroup.GetSubgroups ());
		}

		private void PostProcessGroup(GroupNode node, CollectionViewGroup parentGroup)
		{
			if (node.HasSubnodes)
			{
				foreach (GroupNode subnode in node.Subnodes)
				{
					CollectionViewGroup group = new CollectionViewGroup (subnode.Name, parentGroup);
					this.PostProcessGroup (subnode, group);
					parentGroup.GetSubgroups ().Add (group);
				}
			}

			if (node.HasItems)
			{
				parentGroup.GetItems ().AddRange (node.Items);
			}
		}

		private static void ProcessGroup(object item, System.Globalization.CultureInfo culture, GroupDescription[] rules, int level, GroupNode node)
		{
			int nextLevel = level+1;
			string[] names = rules[level].GetGroupNamesForItem (item, culture);

			foreach (string name in names)
			{
				GroupNode subnode = node.GetSubnode (name);

				if (nextLevel < rules.Length)
				{
					CollectionView.ProcessGroup (item, culture, rules, nextLevel, subnode);
				}
				else
				{
					subnode.Add (item);
				}
			}
		}

		#region GroupNode Class

		private class GroupNode
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
				else if ((this.filter == null) || (this.filter (item)))
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
		
		
		private System.Collections.IList		sourceList;
		private List<object>					sortedList;
		private System.Type						itemType;
		private int								currentPosition;
		private object							currentItem;
		private CollectionViewGroup				rootGroup;
		private Collections.ReadOnlyObservableList<CollectionViewGroup> readOnlyGroups;
		private Collections.ObservableList<GroupDescription> groupDescriptions;
		private Collections.ObservableList<SortDescription> sortDescriptions;
		private System.Predicate<object>		filter;
	}
}
