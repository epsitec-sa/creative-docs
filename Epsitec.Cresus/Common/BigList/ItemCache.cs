//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>ItemCache</c> class is an abstract base for <see cref="ItemCache&lt;TData, TState&gt;"/>.
	/// The state is stored in a very compact representation if this is possible.
	/// </summary>
	public abstract class ItemCache
	{
		public ItemCache(int capacity, ItemListFeatures features)
		{
			this.capacity = capacity;
			this.features = features;
			this.states   = new IndexedArray<ushort> (this.capacity);
		}


		public int								BasicStateCount
		{
			get
			{
				return this.states.Count (x => x != 0);
			}
		}

		public ItemListFeatures					Features
		{
			get
			{
				return this.features;
			}
		}

		public int								ItemCount
		{
			get
			{
				var provider = this.GetDataProvider ();

				if (provider == null)
				{
					return 0;
				}
				else
				{
					return provider.Count;
				}
			}
		}


		public abstract void Reset();

		public abstract int GetExtraStateCount();
		
		public abstract ItemHeight GetItemHeight(int index);

		public abstract ItemData GetItemData(int index);

		public abstract ItemState GetItemState(int index, ItemStateDetails details);
		
		public abstract void SetItemState(int index, ItemState state, ItemStateDetails details);

		public abstract IItemDataProvider GetDataProvider();

		
		protected static readonly int			DefaultExtraCapacity = 1000;
		protected static readonly int			DefaultDataCapacity  = 1000;

		private readonly ItemListFeatures		features;
		private readonly int					capacity;
		protected readonly IndexedArray<ushort>	states;
	}
}
