//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	public abstract class VirtualList<THostEntity, T> : ObservableList<T>, ICollectionModificationCapabilities
		where T : AbstractEntity, new ()
	{
		protected VirtualList(THostEntity entity)
		{
			this.entity = entity;
			this.list.AddRange (this.GetItems ());
		}

		
		public abstract int						MaxCount
		{
			get;
		}

		
		#region ICollectionModificationCapabilities Members

		public bool CanInsert(int index)
		{
			if ((index < 0) || (index >= this.MaxCount))
			{
				return false;
			}
			else if (this.GetItems ().Count () == this.MaxCount)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool CanRemove(int index)
		{
			if ((index < 0) || (index >= this.GetItems ().Count ()))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool CanBeReordered
		{
			get
			{
				return true;
			}
		}

		#endregion

		public override void Insert(int index, T item)
		{
			if (this.Contains (item))
			{
				throw new System.InvalidOperationException ("Duplicate item");
			}

			this.Apply (list => list.Insert (index, item));
			base.Insert (index, item);
		}

		public override void RemoveAt(int index)
		{
			this.Apply (list => list.RemoveAt (index));
			base.RemoveAt (index);
		}

		public override void AddRange(IEnumerable<T> collection)
		{
			this.Apply (x => x.AddRange (collection));
			base.AddRange (collection);
		}

		public override void Sort(IComparer<T> comparer)
		{
			base.Sort (comparer);
			this.ReplaceItems (this.list);
		}

		public override void Sort(System.Comparison<T> comparison)
		{
			base.Sort (comparison);
			this.ReplaceItems (this.list);
		}

		public override void ReplaceWithRange(IEnumerable<T> collection)
		{
			base.ReplaceWithRange (collection);
			this.ReplaceItems (this.list);
		}

		public override T this[int index]
		{
		    get
		    {
		        return base[index];
		    }
		    set
		    {
		        this.Apply (list => list[index] = value);
		        base[index] = value;
		    }
		}

		public override void Clear()
		{
			this.Apply (list => list.Clear ());
			base.Clear ();
		}

		protected void Apply(System.Action<IList<T>> action)
		{
			var list = this.GetItems ().ToList ();
			action (list);
			this.ReplaceItems (list);
		}

		protected abstract IEnumerable<T> GetItems();

		protected abstract void ReplaceItems(IList<T> list);

		protected void SetField<TEntity>(TEntity entity, System.Action<TEntity, T> fieldSetter, System.Func<T> valueProvider, bool condition)
			where TEntity : AbstractEntity
		{
			if (condition)
			{
				fieldSetter (entity, valueProvider ());
			}
			else
			{
				fieldSetter (entity, EntityNullReferenceVirtualizer.CreateNullEntity<T> ());
			}
		}


		protected readonly THostEntity			entity;
	}
}
