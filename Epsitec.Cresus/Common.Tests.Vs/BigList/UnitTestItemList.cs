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
		public void TestItemListMark()
		{
			var mark = new ItemListMark ()
			{
				Attachment = ItemListMarkAttachment.After,
				Breadth    = 1,
				Index      = 0,
			};

			this.itemList1.Reset ();
			this.itemList1.VisibleHeight = 40;
			this.itemList1.Marks.Clear ();
			this.itemList1.Marks.Add (mark);

			//	3 elements: 60 / 30 / 20

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsAfter);

			this.itemList1.MoveVisibleContent (-20);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-20, this.itemList1.VisibleOffset);
			Assert.AreEqual (1, this.itemList1.VisibleCount);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsAfter);
			Assert.AreEqual (40, this.itemList1.GetOffset (mark).Offset);

			this.itemList1.MoveVisibleContent (-10);

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (-30, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsAfter);
			Assert.AreEqual (30, this.itemList1.GetOffset (mark).Offset);

			this.itemList1.MoveVisibleContent (-30);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsAfter);
			Assert.AreEqual (0, this.itemList1.GetOffset (mark).Offset);

			this.itemList1.MoveVisibleContent (-1);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (-1, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsAfter);
			Assert.AreEqual (-1, this.itemList1.GetOffset (mark).Offset);

			this.itemList1.MoveVisibleContent (-1);

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (-2, this.itemList1.VisibleOffset);
			Assert.AreEqual (2, this.itemList1.VisibleCount);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsVisible);
			Assert.IsTrue (this.itemList1.GetOffset (mark).IsBefore);
			Assert.IsFalse (this.itemList1.GetOffset (mark).IsAfter);
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
		public void TestSelection1()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.SelectionMode = ItemSelectionMode.ExactlyOne;

			Assert.AreEqual (0, this.itemList1.SelectedItemCount);
			Assert.AreEqual (0, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());

			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Select));

			Assert.AreEqual (1, this.itemList1.SelectedItemCount);
			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.IsTrue (this.itemList1.IsSelected (0));

			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Select));

			Assert.AreEqual (1, this.itemList1.SelectedItemCount);
			Assert.AreEqual (3, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));

			Assert.IsFalse (this.itemList1.Select (0, ItemSelection.Deselect));

			Assert.AreEqual (1, this.itemList1.SelectedItemCount);
			Assert.AreEqual (3, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));

			this.itemList1.GetItemState (2);

			Assert.AreEqual (3, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
		}

		[TestMethod]
		public void TestSelection2()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.SelectionMode = ItemSelectionMode.Multiple;

			Assert.AreEqual (0, this.itemList1.SelectedItemCount);
			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Select));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Deselect));
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Toggle));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.AreEqual (1, this.itemList1.SelectedItemCount);
			
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Select));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
			Assert.AreEqual (2, this.itemList1.SelectedItemCount);
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Deselect));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.AreEqual (1, this.itemList1.SelectedItemCount);
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Toggle));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
			Assert.AreEqual (2, this.itemList1.SelectedItemCount);
		}

		[TestMethod]
		public void TestSelection3()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.SelectionMode = ItemSelectionMode.OneOrMore;

			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Select));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsFalse (this.itemList1.Select (0, ItemSelection.Deselect));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsFalse (this.itemList1.Select (0, ItemSelection.Toggle));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));

			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Select));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Deselect));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Toggle));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
		}


		[TestMethod]
		public void TestSelection4()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.SelectionMode = ItemSelectionMode.ZeroOrOne;

			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Select));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Deselect));
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (0, ItemSelection.Toggle));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));

			Assert.IsFalse (this.itemList1.Select (1, ItemSelection.Deselect));
			Assert.IsTrue (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));

			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Select));
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Deselect));
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsFalse (this.itemList1.IsSelected (1));
			Assert.IsTrue (this.itemList1.Select (1, ItemSelection.Toggle));
			Assert.IsFalse (this.itemList1.IsSelected (0));
			Assert.IsTrue (this.itemList1.IsSelected (1));
		}

		[TestMethod]
		public void TestExtraState1()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.EnableRowMargins = false;

			Assert.AreEqual (0, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());

			var state1 = this.itemList1.GetItemState (0);
			var state2 = this.itemList1.GetItemState (0);

			Assert.AreNotSame (state1, state2);

			state1.Height = 100;

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0).Height);

			state1.Height = 999;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (999, this.itemList1.GetItemHeight (0).Height);

			state1.Height = 1000;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1000, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.SetItemHeight (0, 1001);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.Select (0, ItemSelection.Select);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0).Height);
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemHeight (0, 100);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0).Height);
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemState (0, state2);

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0).Height);
			Assert.IsFalse (this.itemList1.IsSelected (0));
		}

		[TestMethod]
		public void TestExtraState2()
		{
			this.itemList1.Reset ();
			this.itemList1.Features.EnableRowMargins = true;

			Assert.AreEqual (0, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());

			var state1 = this.itemList1.GetItemState (0);
			var state2 = this.itemList1.GetItemState (0);

			Assert.AreNotSame (state1, state2);

			state1.Height = 100;

			Assert.AreEqual (1, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0).Height);

			state1.Height = 999;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (999, this.itemList1.GetItemHeight (0).Height);

			state1.Height = 1000;
			this.itemList1.SetItemState (0, state1);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1000, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.SetItemHeight (0, 1001);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0).Height);

			this.itemList1.Select (0, ItemSelection.Select);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (1, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (1001, this.itemList1.GetItemHeight (0).Height);
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemHeight (0, 100);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (100, this.itemList1.GetItemHeight (0).Height);
			Assert.IsTrue (this.itemList1.IsSelected (0));

			this.itemList1.SetItemState (0, state2);

			Assert.AreEqual (2, this.itemList1.Cache.BasicStateCount);
			Assert.AreEqual (0, this.itemList1.Cache.GetExtraStateCount ());
			Assert.AreEqual (60, this.itemList1.GetItemHeight (0).Height);
			Assert.IsFalse (this.itemList1.IsSelected (0));
		}

		[TestMethod]
		public void TestMargins1()
		{
			var provider = new SeqProvider (5520, 5530, 5210, 5810);
			var mapper   = new HeightMarginMapper ();

			var itemList = new ItemList<int> (provider, mapper);
			
			itemList.Reset ();
			itemList.Features.EnableRowMargins = true;

			Assert.AreEqual ( 5, itemList.GetItemHeight (0).MarginBefore);
			Assert.AreEqual (20, itemList.GetItemHeight (0).Height);
			Assert.AreEqual ( 3, itemList.GetItemHeight (0).MarginAfter);

			Assert.AreEqual ( 2, itemList.GetItemHeight (1).MarginBefore);
			Assert.AreEqual (30, itemList.GetItemHeight (1).Height);
			Assert.AreEqual ( 3, itemList.GetItemHeight (1).MarginAfter);

			Assert.AreEqual ( 2, itemList.GetItemHeight (2).MarginBefore);
			Assert.AreEqual (10, itemList.GetItemHeight (2).Height);
			Assert.AreEqual ( 4, itemList.GetItemHeight (2).MarginAfter);

			Assert.AreEqual ( 4, itemList.GetItemHeight (3).MarginBefore);
			Assert.AreEqual (10, itemList.GetItemHeight (3).Height);
			Assert.AreEqual ( 5, itemList.GetItemHeight (3).MarginAfter);
		}

		[TestMethod]
		public void TestMargins2()
		{
			var provider = new SeqProvider (5520, 5530, 5210, 5810);
			var mapper   = new HeightMarginMapper ();

			var itemList = new ItemList<int> (provider, mapper);
			
			itemList.Reset ();
			itemList.Features.EnableRowMargins = false;

			Assert.AreEqual ( 0, itemList.GetItemHeight (0).MarginBefore);
			Assert.AreEqual (20, itemList.GetItemHeight (0).Height);
			Assert.AreEqual ( 0, itemList.GetItemHeight (0).MarginAfter);

			Assert.AreEqual ( 0, itemList.GetItemHeight (1).MarginBefore);
			Assert.AreEqual (30, itemList.GetItemHeight (1).Height);
			Assert.AreEqual ( 0, itemList.GetItemHeight (1).MarginAfter);

			Assert.AreEqual ( 0, itemList.GetItemHeight (2).MarginBefore);
			Assert.AreEqual (10, itemList.GetItemHeight (2).Height);
			Assert.AreEqual ( 0, itemList.GetItemHeight (2).MarginAfter);

			Assert.AreEqual ( 0, itemList.GetItemHeight (3).MarginBefore);
			Assert.AreEqual (10, itemList.GetItemHeight (3).Height);
			Assert.AreEqual ( 0, itemList.GetItemHeight (3).MarginAfter);
		}

		private class SeqMapper : IItemDataMapper<int>
		{
			#region IItemDataMapper<int> Members

			public ItemData<int> Map(int value)
			{
				return new ItemData<int> (value,
					new ItemState
					{
						Height = value
					});
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
				return new ItemData<int> (value,
					new ItemState
					{
						Height = this.height
					});
			}

			#endregion

			private readonly int height;
		}

		private class HeightMarginMapper : IItemDataMapper<int>
		{
			public HeightMarginMapper()
			{
			}

			#region IItemDataMapper<int> Members

			public ItemData<int> Map(int value)
			{
				int height       = (value)        % 100;
				int marginBefore = (value / 100)  % 10;
				int marginAfter  = (value / 1000) % 10;

				return new ItemData<int> (value,
					new ItemState
					{
						Height       = height,
						MarginBefore = marginBefore,
						MarginAfter  = marginAfter,
					});
			}

			#endregion
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



		private readonly ItemList<int>			itemList1;
		private readonly SeqProvider			provider1;
		private readonly RangeProvider			provider2;
		private readonly SeqMapper				mapper1;
		private readonly ConstMapper			mapper2;
	}
}
