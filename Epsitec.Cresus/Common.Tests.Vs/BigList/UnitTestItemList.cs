//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Common.BigList;

namespace Epsitec.Common.Tests.Vs.BigList
{
	/// <summary>
	/// 
	/// </summary>
	[TestClass]
	public class UnitTestItemList
	{
		public UnitTestItemList()
		{
			this.provider1 = new SeqProvider (60, 30, 20);
			this.provider2 = new RangeProvider (1000000);
			this.mapper1   = new SeqMapper ();
			this.mapper2   = new ConstMapper (10);
			
			this.itemList1 = new ItemList<int> (this.provider1, this.mapper1);
			this.itemList1.Reset ();
		}


		[TestMethod]
		public void TestVisibleIndexAndVisibleOffset1()
		{
			this.itemList1.Reset ();
			this.itemList1.VisibleHeight = 60;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (30, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 2;

			Assert.AreEqual (2, this.itemList1.VisibleIndex);
			Assert.AreEqual (40, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (10, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 0;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);
		}

		[TestMethod]
		public void TestVisibleIndexAndVisibleOffset2()
		{
			this.itemList1.Reset ();
			this.itemList1.VisibleHeight = 100;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (60, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 2;

			Assert.AreEqual (2, this.itemList1.VisibleIndex);
			Assert.AreEqual (80, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (50, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 0;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (3, this.itemList1.VisibleCount);
		}

		[TestMethod]
		public void TestVisibleIndexAndVisibleOffset3()
		{
			this.itemList1.Reset ();
			this.itemList1.VisibleHeight = 30;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 2;

			Assert.AreEqual (2, this.itemList1.VisibleIndex);
			Assert.AreEqual (10, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.VisibleIndex = 0;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);
		}

		[TestMethod]
		public void TestMoveVisibleContent()
		{
			this.itemList1.Reset ();
			this.itemList1.VisibleHeight = 30;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (10);		//	cannot move further down !

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-10, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-20, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-30, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-40, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-50, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (-10, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (-20, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (-10);	//	cannot move further up !

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (-20, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (30);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-50, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);

			this.itemList1.MoveVisibleContent (1000);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);
		}

		[TestMethod]
		public void TestPerf()
		{
			long memory0 = System.GC.GetTotalMemory (true);

			ItemList<int> list = new ItemList<int> (this.provider2, this.mapper2);

			long memory1 = System.GC.GetTotalMemory (true);

			list.Reset ();
			list.VisibleHeight = 100;

			Assert.AreEqual (10, list.VisibleCount);

			list.VisibleIndex = 1000;

			Assert.AreEqual (1000, list.VisibleIndex);
			Assert.AreEqual (90, list.VisibleOffset);
			Assert.AreEqual (10, list.VisibleCount);

			list.VisibleIndex = 100*1000;

			Assert.AreEqual (100*1000, list.VisibleIndex);
			Assert.AreEqual (90, list.VisibleOffset);
			Assert.AreEqual (10, list.VisibleCount);

			list.VisibleIndex = 1000;

			Assert.AreEqual (1000, list.VisibleIndex);
			Assert.AreEqual (0, list.VisibleOffset);
			Assert.AreEqual (10, list.VisibleCount);

			list.VisibleIndex = 1000*1000;		//	overflow (items 0..999999)

			long memory2 = System.GC.GetTotalMemory (true);

			Assert.AreEqual (999999, list.VisibleIndex);
			Assert.AreEqual (90, list.VisibleOffset);
			Assert.AreEqual (10, list.VisibleCount);

			System.Diagnostics.Debug.WriteLine ("Before: {0:###,###,###}\n" +
												"After:  {1:###,###,###}\tdelta={2}\n" +
												"End:    {3:###,###,###}\tdelta={4}", memory0, memory1, memory1-memory0, memory2, memory2-memory1);
		}

		[TestMethod]
		public void TestSelection()
		{
			this.itemList1.Reset ();

			Assert.AreEqual (0, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);

			this.itemList1.Select (0, ItemSelection.Select);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.Select (1, ItemSelection.Select);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));

			this.itemList1.Select (0, ItemSelection.Deselect);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);

			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));

			this.itemList1.GetItemState (2);

			Assert.AreEqual (3, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
		}

		[TestMethod]
		public void TestExtraState()
		{
			this.itemList1.Reset ();

			Assert.AreEqual (0, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);

			var state1 = this.itemList1.GetItemState (0);
			var state2 = this.itemList1.GetItemState (0);

			Assert.AreNotSame (state1, state2);

			state1.Height = 100;

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0));

			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0));

			state1.Height = 999;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (999, this.itemList1.GetItemHeight (0));

			state1.Height = 1000;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (1000, this.itemList1.GetItemHeight (0));

			this.itemList1.SetItemHeight (0, 1001);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0));

			this.itemList1.Select (0, ItemSelection.Select);
			
			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0));
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemHeight (0, 100);
			
			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0));
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemState (0, state2);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.ExtraStateCount);
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0));
			Assert.IsFalse (this.itemList1.IsSelected (0));
		}

		private class SeqMapper : IItemDataMapper<int>
		{
			#region IItemDataMapper<int> Members

			public ItemData<int> Map(int value)
			{
				return new ItemData<int> (value)
				{
					Height = value,
				};
			}

			#endregion
		}

		private class SeqProvider : IItemDataProvider<int>
		{
			public SeqProvider(params int[] values)
			{
				this.list = new List<int> (values);
			}

			#region IItemDataProvider<int> Members

			public bool Resolve(int index, out int value)
			{
				if ((index < 0) ||
					(index > this.list.Count))
				{
					value = 0;
					return false;
				}

				value = this.list[index];

				return true;
			}

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			#endregion

			private readonly List<int> list;
		}

		private class ConstMapper : IItemDataMapper<int>
		{
			public ConstMapper(int height)
			{
				this.height = height;
			}

			#region IItemDataMapper<int> Members

			public ItemData<int> Map(int value)
			{
				return new ItemData<int> (value)
				{
					Height = this.height,
				};
			}

			#endregion

			private readonly int height;
		}

		private class RangeProvider : IItemDataProvider<int>
		{
			public RangeProvider(int num)
			{
				this.num = num;
			}

			#region IItemDataProvider<int> Members

			public bool Resolve(int index, out int value)
			{
				if ((index < 0) ||
					(index > this.num))
				{
					value = 0;
					return false;
				}

				value = index;

				return true;
			}

			public int Count
			{
				get
				{
					return this.num;
				}
			}

			#endregion

			private readonly int num;
		}



		private readonly ItemList<int>		itemList1;
		private readonly SeqProvider			provider1;
		private readonly RangeProvider			provider2;
		private readonly SeqMapper				mapper1;
		private readonly ConstMapper			mapper2;
	}
}
