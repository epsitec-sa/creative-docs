//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.BigList
{
    /// <summary>
    /// The <c>ItemCache</c> class maintains a collection of <see cref="ItemData&lt;TData&gt;"/>
    /// items. It fetches the data from a data provider and converts a raw data item into an
    /// item data record using a data mapper.
    /// </summary>
    /// <typeparam name="TData">The type of the raw data item.</typeparam>
    /// <typeparam name="TState">The type of the item state.</typeparam>
    public class ItemCache<TData, TState> : ItemCache
        where TState : ItemState, new()
    {
        public ItemCache(int capacity, ItemListFeatures features)
            : base(capacity, features)
        {
            this.extraStates = new IndexedStore<TState>(ItemCache.DefaultExtraCapacity);
            this.data = new IndexedStore<ItemData<TData>>(ItemCache.DefaultDataCapacity);
        }

        public IItemDataProvider<TData> DataProvider { get; set; }

        public IItemDataMapper<TData> DataMapper { get; set; }

        public override IItemDataProvider GetDataProvider()
        {
            return this.DataProvider;
        }

        public override int GetExtraStateCount()
        {
            return this.extraStates.Count;
        }

        public override void Reset()
        {
            int count = this.DataProvider.Count;
            var array = new ushort[count];

            this.states.Clear();
            this.extraStates.Clear();
            this.data.Clear();
            this.states.AddRange(array);

            this.OnResetFired();
        }

        /// <summary>
        /// Gets the height of the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The height of the item or zero if the item does not exist.</returns>
        public override ItemHeight GetItemHeight(int index)
        {
            int count = this.states.Count;

            if ((index < 0) || (index >= count))
            {
                return new ItemHeight();
            }

            if (this.Features.EnableRowMargins)
            {
                return this.GetItemHeightMergeMargins(index);
            }
            else
            {
                return this.GetItemHeightWithoutMargins(index);
            }
        }

        public override ItemData GetItemData(int index)
        {
            return this.GetItemDataExact(index);
        }

        public ItemData<TData> GetItemDataExact(int index)
        {
            var data = this.GetItemDataLocked(index);

            if (data == null)
            {
                data = this.FromDataProvider(index);

                if (data != null)
                {
                    this.data[index] = data;
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
            if ((index < 0) || (index >= this.states.Count))
            {
                return ItemState.Empty;
            }

            var state = this.GetItemStateLocked(index, details);

            if (state == null)
            {
                if (details.HasFlag(ItemStateDetails.IgnoreNull))
                {
                    return null;
                }

                //	Could not retrieve the state, because we never accessed it before and
                //	we have no data about it in the cache. Retrieve the data first.

                var data = this.GetItemData(index);

                if (data != null)
                {
                    state = data.CreateState<TState>();

                    this.SetItemState(index, state, details);
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

            if ((index < 0) || (index >= this.states.Count))
            {
                if (state.IsEmpty)
                {
                    return;
                }

                throw new System.IndexOutOfRangeException(
                    string.Format("Index {0} out of range", index)
                );
            }

            this.SetItemStateLocked(index, state as TState, details);
        }

        private ItemHeight GetItemHeightWithoutMargins(int index)
        {
            var state = this.GetItemState(index, ItemStateDetails.Full);

            return new ItemHeight(state.PaddingBefore + state.Height + state.PaddingAfter);
        }

        private ItemHeight GetItemHeightMergeMargins(int index)
        {
            int count = this.states.Count;
            var state = this.GetItemState(index, ItemStateDetails.Full);
            int height = 0;

            int marginBefore;
            int marginAfter;

            if (index == 0)
            {
                marginBefore = state.MarginBefore;
            }
            else
            {
                var stateBefore = this.GetItemState(index - 1, ItemStateDetails.Full);
                marginBefore = System.Math.Max(state.MarginBefore, stateBefore.MarginAfter) / 2;
            }

            height += state.PaddingBefore;
            height += state.Height;
            height += state.PaddingAfter;

            if (index < count - 1)
            {
                var stateAfter = this.GetItemState(index + 1, ItemStateDetails.Full);

                marginAfter = (System.Math.Max(state.MarginAfter, stateAfter.MarginBefore) + 1) / 2;
            }
            else
            {
                marginAfter = state.MarginAfter;
            }

            return new ItemHeight(height, marginBefore, marginAfter);
        }

        private TState GetItemStateLocked(int index, ItemStateDetails details)
        {
            var compact = this.states[index];

            if (compact == 0)
            {
                return null;
            }

            var state = ItemState.FromCompactState<TState>(compact);

            if (details.HasFlag(ItemStateDetails.Full))
            {
                if (state.Partial)
                {
                    TState extra;

                    if (this.extraStates.TryGetValue(index, out extra) == false)
                    {
                        //	Since we were not able to retrieve the full information, and we know
                        //	that there is more state than what we have at hand, force the caller
                        //	to retrieve the full state from the provider.

                        return null;
                    }

                    state.ApplyExtraState(extra);
                }
            }

            return state;
        }

        private void SetItemStateLocked(int index, TState state, ItemStateDetails details)
        {
            var compact = state == null ? ItemState.EmptyCompactState : state.ToCompactState();

            if (details.HasFlag(ItemStateDetails.Full))
            {
                if ((state != null) && (state.RequiresExtraState))
                {
                    this.extraStates[index] = state.Clone<TState>();
                }
                else
                {
                    this.extraStates.Remove(index);
                }
            }

            this.states[index] = compact;
        }

        private ItemData<TData> GetItemDataLocked(int index)
        {
            ItemData<TData> data;

            if (this.data.TryGetValue(index, out data))
            {
                return data;
            }

            return null;
        }

        private ItemData<TData> FromDataProvider(int index)
        {
            var provider = this.DataProvider;
            var mapper = this.DataMapper;

            if ((provider == null) || (mapper == null))
            {
                return null;
            }

            TData value;

            if (provider.Resolve(index, out value))
            {
                return mapper.Map(value);
            }

            return null;
        }

        private readonly IndexedStore<TState> extraStates;
        private readonly IndexedStore<ItemData<TData>> data;
    }
}
