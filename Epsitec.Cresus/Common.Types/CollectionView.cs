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

		public void Refresh()
		{
			if (this.groups == null)
			{
				this.groups = new Collections.ObservableList<object> ();
				this.readOnlyGroups = new Collections.ReadOnlyObservableList<object> (this.groups);
			}

			List<object> list = new List<object> ();
			
			System.Type dataType = null;

			foreach (object item in this.sourceCollection)
			{
				list.Add (item);

				if (item == null)
				{
					throw new System.InvalidOperationException ("Source collection contains null items");
				}
				else
				{
					System.Type itemType = item.GetType ();
					
					if (dataType == null)
					{
						dataType = itemType;
					}
					else
					{
						if (dataType != itemType)
						{
							throw new System.InvalidOperationException (string.Format ("Source collection not orthogonal; found type {0} and {1}", dataType.Name, itemType.Name));
						}
					}
				}
			}

			if ((this.sortDescriptions != null) &&
				(dataType != null))
			{
				System.Comparison<object>[] comparisons = new System.Comparison<object> [this.sortDescriptions.Count];

				for (int i = 0; i < this.sortDescriptions.Count; i++)
				{
					comparisons[i] = CollectionView.CreateComparison (dataType, this.sortDescriptions[i]);
				}

				if (comparisons.Length == 1)
				{
					list.Sort (comparisons[0]);
				}
				else if (comparisons.Length > 1)
				{
					list.Sort
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

			this.sortedItems = list;
			
			//	TODO: add suspend/resume
			
			this.groups.Clear ();
			this.groups.AddRange (list);
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
		private int								currentPosition;
		private object							currentItem;
		private ObjectList						groups;
		private ReadOnlyObjectList				readOnlyGroups;
		private GroupDescriptionList			groupDescriptions;
		private SortDescriptionList				sortDescriptions;
	}
}
