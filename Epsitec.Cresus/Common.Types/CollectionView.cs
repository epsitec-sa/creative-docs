//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
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

		public System.Collections.IEnumerable SourceCollection
		{
			get
			{
				return this.sourceCollection;
			}
		}

		public object CurrentItem
		{
			get
			{
				return this.currentItem;
			}
		}

		public int CurrentPosition
		{
			get
			{
				return this.currentPosition;
			}
		}

		public Collections.ReadOnlyObservableList<object> Groups
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

		public Collections.ObservableList<AbstractGroupDescription> GroupDescriptions
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

		public void Refresh()
		{
			if (this.groups == null)
			{
				this.groups = new Collections.ObservableList<object> ();
				this.readOnlyGroups = new Collections.ReadOnlyObservableList<object> (this.groups);
			}

			//	TODO: refresh the contents of the groups
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
		
		
		private System.Collections.IEnumerable sourceCollection;
		private int currentPosition;
		private object currentItem;
		private Collections.ObservableList<object> groups;
		private Collections.ReadOnlyObservableList<object> readOnlyGroups;
		private Collections.ObservableList<AbstractGroupDescription> groupDescriptions;
		private Collections.ObservableList<SortDescription> sortDescriptions;
	}
}
