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
			this.provider1 = new Provider (60, 30, 20);
			this.mapper1   = new Mapper ();
			
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


		private class Mapper : IItemDataMapper<int>
		{
			#region IItemDataMapper<int> Members

			public ItemData<int> Map(int value)
			{
				return new ItemData<int> ()
				{
					Height = value,
				};
			}

			#endregion
		}

		private class Provider : IItemDataProvider<int>
		{
			public Provider(params int[] values)
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



		private readonly ItemList<int>			itemList1;
		private readonly Provider				provider1;
		private readonly Mapper					mapper1;
	}
}
