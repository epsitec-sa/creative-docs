//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemCache<T> : ItemCache
	{
		public ItemCache(int capacity)
		{
			this.capacity = capacity;

			this.exclusion   = new ReadWriteLock ();
			this.states      = new IndexedArray<ushort> (this.capacity);
			this.extraStates = new IndexedStore<ItemState> (ItemCache.DefaultExtraCapacity);
			this.data        = new IndexedStore<ItemData<T>> (ItemCache.DefaultDataCapacity);
		}


		public IItemDataProvider<T> DataProvider
		{
			get;
			set;
		}


		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The height of the item or zero if the item does not exist.</returns>
		public int GetItemHeight(int index)
		{
			return this.GetItemState (index, fullState: false).Height;
		}

		public ItemData<T> GetItemData(int index)
		{
			this.exclusion.EnterReadLock ();
			var data = this.GetItemDataLocked (index);
			this.exclusion.ExitReadLock ();

			if (data == null)
			{
				data = this.FromDataProvider (index);

				if (data != null)
				{
					this.exclusion.EnterWriteLock ();
					this.data[index] = data;
					this.exclusion.ExitWriteLock ();
				}
			}
			
			return data;
		}

		/// <summary>
		/// Gets the state of the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="fullState">if set to <c>true</c>, retrieves the full state.</param>
		/// <returns></returns>
		public ItemState GetItemState(int index, bool fullState = false)
		{
			if ((index < 0) ||
				(index >= this.states.Count))
			{
				return ItemState.Empty;
			}

			this.exclusion.EnterReadLock ();
			var state = this.GetItemStateLocked (index, fullState);
			this.exclusion.ExitReadLock ();

			return state;
		}

		public void SetItemState(int index, ItemState state)
		{
			if ((index < 0) ||
				(index >= this.states.Count))
			{
				if (state.IsEmpty)
				{
					return;
				}

				throw new System.IndexOutOfRangeException (string.Format ("Index {0} out of range", index));
			}

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

		private ItemData<T> GetItemDataLocked(int index)
		{
			ItemData<T> data;

			if (this.data.TryGetValue (index, out data))
			{
				return data;
			}

			return null;
		}

		private ItemData<T> FromDataProvider(int index)
		{
			var provider = this.DataProvider;

			if (provider == null)
			{
				return null;
			}

			T value;

			if (provider.Resolve (index, out value))
			{
				//	TODO: allocate ItemData
			}

			return null;
		}

		private readonly int						capacity;
		private readonly IndexedArray<ushort>		states;
		private readonly IndexedStore<ItemState>	extraStates;
		private readonly ReadWriteLock				exclusion;
		private readonly IndexedStore<ItemData<T>>	data;
	}

	public class ItemCacheEntry<T>
	{
	}

	public class ItemData<T>
	{
	}

	public interface IItemDataProvider<T>
	{
		bool Resolve(int index, out T value);
	}
}
