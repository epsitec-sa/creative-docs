//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemCache<T>
	{
		public ItemCache(int capacity)
		{
			this.capacity = capacity;
			
			this.states      = new IndexedArray<ushort> (this.capacity);
			this.extraStates = new IndexedStore<ItemState> (this.capacity);
			this.exclusion   = new ReadWriteLock ();
		}


		public int GetItemHeight(int index)
		{
			return this.GetItemState (index, fullState: false).Height;
		}

		public ItemState GetItemState(int index, bool fullState = false)
		{
			this.exclusion.EnterReadLock ();
			var state = this.GetItemStateLocked (index, fullState);
			this.exclusion.ExitReadLock ();

			return state;
		}

		public void SetItemState(int index, ItemState state)
		{
			this.exclusion.EnterWriteLock ();
			this.SetItemStateLocked (index, state);
			this.exclusion.ExitWriteLock ();
		}

		private ItemState GetItemStateLocked(int index, bool fullState)
		{
			var state = ItemState.FromCompactState (this.states[index]);

			if ((state.Partial && fullState) ||
				(state.Height == ItemState.MaxCompactHeight))
			{
				ItemState extra;

				if (this.extraStates.TryGetValue (index, out extra) == false)
				{
					//	TODO: build extra state
				}

				state.Apply (extra);
			}
			
			return state;
		}

		private void SetItemStateLocked(int index, ItemState state)
		{
			var compact = state.ToCompactState ();

			if (state.ComputePartialFlag ())
			{
				this.extraStates[index] = new ItemState (state);
			}
			else
			{
				this.extraStates.Remove (index);
			}

			this.states[index] = compact;
		}

		private readonly int						capacity;
		private readonly IndexedArray<ushort>		states;
		private readonly IndexedStore<ItemState>	extraStates;
		private readonly ReadWriteLock				exclusion;
	}

	internal class ReadWriteLock : System.Threading.ReaderWriterLockSlim
	{
		public ReadWriteLock()
			: base (System.Threading.LockRecursionPolicy.NoRecursion)
		{
		}
	};

	public class ItemCacheEntry<T>
	{
	}

	public class ItemData<T>
	{
	}

	internal class IndexedStore<TValue> : Dictionary<int, TValue>
	{
		public IndexedStore(int capacity)
			: base (capacity)
		{
		}
	}

	internal class IndexedArray<TValue> : List<TValue>
	{
		public IndexedArray(int capacity)
			: base (capacity)
		{
		}
	}
}
