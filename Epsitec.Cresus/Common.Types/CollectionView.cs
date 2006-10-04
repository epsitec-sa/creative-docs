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
		public CollectionView()
		{
		}



		#region ICollectionView Members

		public object CurrentItem
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public int CurrentPosition
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public Collections.ReadOnlyObservableList<CollectionViewGroup> Groups
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public Collections.ObservableList<AbstractGroupDescription> GroupDescriptions
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public Collections.ObservableList<SortDescription> SortDescriptions
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		public System.Collections.IEnumerable SourceCollection
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}
		
		public void Refresh()
		{
			throw new System.Exception ("The method or operation is not implemented.");
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
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion
	}
}
