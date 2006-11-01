//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>AbstractList</c> class is the base class for all collections used
	/// in the database layer.
	/// </summary>
	public class AbstractList<T> : ObservableList<T> where T : class
	{
		public AbstractList()
		{
		}

		public T this[string name]
		{
			get
			{
				int index = this.IndexOf (name);
				
				if (index < 0)
				{
					return null;
				}
				else
				{
					return this[index];
				}
			}
		}
		
		public void Remove(string name)
		{
			int index = this.IndexOf (name);
			
			if (index != -1)
			{
				this.RemoveAt (index);
			}
		}
		
		public bool Contains(string name)
		{
			return this.IndexOf (name) != -1;
		}

		public int IndexOf(string name)
		{
			return this.IndexOf (name, 0);
		}

		public virtual int IndexOf(string name, int start)
		{
			//	Cette méthode retourne toujours -1, car on ne sait pas comment chercher
			//	selon un nom. Par contre, les classes qui héritent de AbstractList
			//	fournissent leur propre implémentation.

			return -1;
		}


		protected override void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			base.OnCollectionChanged (e);

			switch (e.Action)
			{
				case CollectionChangedAction.Add:
					foreach (T item in e.NewItems)
					{
						this.NotifyInsertion (item);
					}
					break;

				case CollectionChangedAction.Move:
					break;

				case CollectionChangedAction.Remove:
					foreach (T item in e.OldItems)
					{
						this.NotifyRemoval (item);
					}
					break;

				case CollectionChangedAction.Replace:
					foreach (T item in e.OldItems)
					{
						this.NotifyRemoval (item);
					}
					foreach (T item in e.NewItems)
					{
						this.NotifyInsertion (item);
					}
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Action.{0} not implemented", e.Action));
			}
		}

		protected virtual void NotifyInsertion(T item)
		{
			if (this.ItemInserted != null)
			{
				this.ItemInserted (this, new ValueEventArgs (item));
			}
		}
		
		protected virtual void NotifyRemoval(T item)
		{
			if (this.ItemRemoved != null)
			{
				this.ItemRemoved (this, new ValueEventArgs (item));
			}
		}


		public event EventHandler<ValueEventArgs> ItemInserted;
		public event EventHandler<ValueEventArgs> ItemRemoved;
	}
}
