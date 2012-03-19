//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemCache<T> : ItemCache
	{
		public ItemCache(int capacity)
			: base (capacity)
		{
			this.exclusion   = new ReadWriteLock ();
			this.extraStates = new IndexedStore<ItemState> (ItemCache.DefaultExtraCapacity);
			this.data        = new IndexedStore<ItemData<T>> (ItemCache.DefaultDataCapacity);
		}


		public IItemDataProvider<T> DataProvider
		{
			get;
			set;
		}

		public IItemDataMapper<T> DataMapper
		{
			get;
			set;
		}

		public override int ExtraStateCount
		{
			get
			{
				return this.extraStates.Count;
			}
		}

		public override void Reset()
		{
			int count = this.DataProvider.Count;
			var array = new ushort[count];

			this.exclusion.EnterWriteLock ();

			this.states.Clear ();
			this.extraStates.Clear ();
			this.data.Clear ();
			this.states.AddRange (array);

			this.exclusion.ExitWriteLock ();
		}

		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The height of the item or zero if the item does not exist.</returns>
		public override int GetItemHeight(int index)
		{
			return this.GetItemState (index, ItemStateDetails.Full).Height;
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
		public override ItemState GetItemState(int index, ItemStateDetails details)
		{
			if ((index < 0) ||
				(index >= this.states.Count))
			{
				return ItemState.Empty;
			}

			this.exclusion.EnterReadLock ();
			var state = this.GetItemStateLocked (index, details);
			this.exclusion.ExitReadLock ();

			if (state == null)
			{
				//	Could not retrieve the state, because we never accessed it before and
				//	we have no data about it in the cache. Retrieve the data first.

				var data = this.GetItemData (index);

				if (data != null)
				{
					state = new ItemState ()
					{
						Height = data.Height
					};

					this.SetItemState (index, state, details);
				}
			}

			return state;
		}

		public override void SetItemState(int index, ItemState state, ItemStateDetails details)
		{
			if (details == ItemStateDetails.None)
			{
				return;
			}

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
			this.SetItemStateLocked (index, state, details);
			this.exclusion.ExitWriteLock ();
		}

		private ItemState GetItemStateLocked(int index, ItemStateDetails details)
		{
			var compact = this.states[index];

			if (compact == 0)
			{
				return null;
			}

			var state = ItemState.FromCompactState (compact);

			if (details.HasFlag (ItemStateDetails.Full))
			{
				if ((state.Partial) ||
					(state.Height+1 == ItemState.MaxCompactHeight))
				{
					ItemState extra;

					if (this.extraStates.TryGetValue (index, out extra) == false)
					{
						extra = new ItemState (state);
					}

					state.Apply (extra);
				}
			}

			return state;
		}

		private void SetItemStateLocked(int index, ItemState state, ItemStateDetails details)
		{
			var compact = state.ToCompactState ();

			if (details.HasFlag (ItemStateDetails.Full))
			{
				if (state.ComputePartialFlag ())
				{
					this.extraStates[index] = new ItemState (state);
				}
				else
				{
					this.extraStates.Remove (index);
				}
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
			var mapper   = this.DataMapper;

			if ((provider == null) ||
				(mapper == null))
			{
				return null;
			}

			T value;

			if (provider.Resolve (index, out value))
			{
				return mapper.Map (value);
			}

			return null;
		}

		private readonly IndexedStore<ItemState>	extraStates;
		private readonly ReadWriteLock				exclusion;
		private readonly IndexedStore<ItemData<T>>	data;
		private readonly IItemDataMapper<T>			mapper;
	}

	public class ItemCacheEntry<T>
	{
	}

	public class ItemData<T>
	{
		public int Height
		{
			get;
			set;
		}
	}

	public interface IItemDataProvider<T>
	{
		bool Resolve(int index, out T value);
		
		int Count
		{
			get;
		}
	}
	public interface IItemDataMapper<T>
	{
		ItemData<T> Map(T value);
	}
}
