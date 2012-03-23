//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public abstract class ItemCache
	{
		public ItemCache(int capacity, ItemListFeatures features)
		{
			this.capacity = capacity;
			this.features = features;
			this.states   = new IndexedArray<ushort> (this.capacity);
		}

		
		public abstract int						ExtraStateCount
		{
			get;
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

		
		public abstract void Reset();
		
		public abstract ItemHeight GetItemHeight(int index);

		public abstract ItemData GetItemData(int index);

		public abstract ItemState GetItemState(int index, ItemStateDetails details);
		
		public abstract void SetItemState(int index, ItemState state, ItemStateDetails details);

		
		protected static readonly int			DefaultExtraCapacity = 1000;
		protected static readonly int			DefaultDataCapacity  = 1000;

		private readonly ItemListFeatures		features;
		private readonly int					capacity;
		protected readonly IndexedArray<ushort>	states;
	}
}
