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
			else if (this.list.Count == this.MaxCount)
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
			if ((index < 0) || (index >= this.list.Count))
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
			var list = new List<T>(this.list);
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
				// Originaly, we would put an entity created by the EntityNullReferenceVirtualizer,
				// but this would cause problems. For more details, you can go look on the bug #2242
				// in Mantis.
				// The only problem with using null, is that if you access the entity via the
				// property and not the list, you will get null, and you don't expect that because
				// of the EntityNullReferenceVirtualizer. But in practice, this is not a problem in
				// WebCore because never do this kind of stuff.

				fieldSetter (entity, null);
			}
		}


		protected readonly THostEntity			entity;
	}
}
