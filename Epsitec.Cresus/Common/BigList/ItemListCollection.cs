//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public abstract class ItemListCollection : AbstractItemList, IEnumerable<IItemList>
	{
		public ItemListCollection(ItemCache cache, IList<ItemListMark> marks, ItemListSelection selection)
			: base (cache, marks, selection)
		{
			this.list = new List<IItemList> ();
		}

		
		#region IEnumerable<IItemList> Members

		public IEnumerator<IItemList> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		
		public abstract IItemList Create();

		public override void Reset()
		{
			this.list.ForEach (x => x.Reset ());
		}


		protected void Add(IItemList itemList)
		{
			this.list.Add (itemList);
		}

		protected void ForEach(System.Action<IItemList> action)
		{
			this.list.ForEach (action);
		}

		
		protected override void ClearActiveIndex()
		{
			base.ClearActiveIndex ();
			this.ForEach (x => x.ActiveIndex = -1);
		}

		protected override void SetActiveIndex(int index)
		{
			base.SetActiveIndex (index);
			this.ForEach (x => x.ActiveIndex = index);
		}

		protected override void ClearFocusedIndex()
		{
			base.ClearFocusedIndex ();
			this.ForEach (x => x.FocusedIndex = -1);
		}

		protected override void SetFocusedIndex(int index)
		{
			base.SetFocusedIndex (index);
			this.ForEach (x => x.FocusedIndex = index);
		}


		private readonly List<IItemList>		list;
	}
}
