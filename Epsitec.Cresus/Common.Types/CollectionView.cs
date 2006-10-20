//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using SortDescriptionList=Collections.ObservableList<SortDescription>;
	using GroupDescriptionList=Collections.ObservableList<AbstractGroupDescription>;
	using ObjectList=Collections.ObservableList<object>;
	using ReadOnlyObjectList=Collections.ReadOnlyObservableList<object>;
	
	/// <summary>
	/// The <c>CollectionView</c> class represents a view of a collection. Views implement
	/// grouping, sorting, filtering and the concept of a current item.
	/// </summary>
	public class CollectionView : ICollectionView, System.Collections.IEnumerable, INotifyCollectionChanged
	{
		public CollectionView(System.Collections.IEnumerable sourceCollection)
		{
			this.sourceCollection = sourceCollection;
		}

		#region ICollectionView Members

		public System.Collections.IEnumerable	SourceCollection
		{
			get
			{
				return this.sourceCollection;
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

		public ReadOnlyObjectList				Groups
		{
			get
			{
				if (this.readOnlyGroups == null)
				{
					this.Refresh ();
				}

				return this.readOnlyGroups;
			}
		}

		public GroupDescriptionList				GroupDescriptions
		{
			get
			{
				if (this.groupDescriptions == null)
				{
					this.groupDescriptions = new Collections.ObservableList<AbstractGroupDescription> ();
				}
				
				return this.groupDescriptions;
			}
		}

		public SortDescriptionList				SortDescriptions
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

		public void Refresh()
		{
			//	TODO: add suspend/resume for all observable collections

			if (this.groups == null)
			{
				this.groups = new Collections.ObservableList<object> ();
				this.readOnlyGroups = new Collections.ReadOnlyObservableList<object> (this.groups);
			}
			else
			{
				this.groups.Clear ();
			}

			if (this.sortedItems == null)
			{
				this.sortedItems = new List<object> ();
			}
			else
			{
				this.sortedItems.Clear ();
			}
			
			this.FillItemsList ();
			this.SortItemsList ();
			
			this.GroupItemsInList ();

			this.groups.AddRange (this.sortedItems);
		}

		private void GroupItemsInList()
		{
			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
			
			AbstractGroupDescription[] rules = this.groupDescriptions.ToArray ();

			if (rules.Length > 0)
			{
				GroupNode root = new GroupNode (null);

				foreach (object item in this.sortedItems)
				{
					CollectionView.ProcessGroup (item, culture, rules, 0, root);
				}

				this.PostProcessGroup (rules.Length, root, this.groups);
			}
		}

		private void PostProcessGroup(int depth, GroupNode node, Collections.ObservableList<object> groups)
		{
			if (depth == 1)
			{
				groups.AddRange (node.Items);
			}
			else
			{
				CollectionViewGroup group = new CollectionViewGroup (node.Name);

				foreach (GroupNode subnode in node.Subnodes)
				{
					this.PostProcessGroup (depth-1, subnode, group.GetItems ());
				}

				groups.Add (group);
			}
		}

		private static void ProcessGroup(object item, System.Globalization.CultureInfo culture, AbstractGroupDescription[] rules, int level, GroupNode node)
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

		private class GroupNode
		{
			public GroupNode(string name)
			{
				this.name = name;
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
					this.sortedItems.Sort (comparisons[0]);
				}
				else if (comparisons.Length > 1)
				{
					this.sortedItems.Sort
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
			
			foreach (object item in this.sourceCollection)
			{
				if (item == null)
				{
					throw new System.InvalidOperationException ("Source collection contains null items");
				}
				else if ((this.filter == null) || (this.filter (item)))
				{
					this.sortedItems.Add (item);

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

		public event Support.EventHandler CurrentChanged;

		public event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;

		#endregion

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
		
		
		private System.Collections.IEnumerable	sourceCollection;
		private List<object>					sortedItems;
		private System.Type						itemType;
		private int								currentPosition;
		private object							currentItem;
		private ObjectList						groups;
		private ReadOnlyObjectList				readOnlyGroups;
		private GroupDescriptionList			groupDescriptions;
		private SortDescriptionList				sortDescriptions;
		private System.Predicate<object>		filter;
	}
}
