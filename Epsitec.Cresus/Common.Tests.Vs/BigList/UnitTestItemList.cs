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

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestMethod1()
		{
			this.itemList1.VisibleHeight = 60;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
			
			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (30, this.itemList1.VisibleOffset);

			this.itemList1.VisibleIndex = 2;

			Assert.AreEqual (2, this.itemList1.VisibleIndex);
			Assert.AreEqual (40, this.itemList1.VisibleOffset);

			this.itemList1.VisibleIndex = 1;

			Assert.AreEqual (1, this.itemList1.VisibleIndex);
			Assert.AreEqual (10, this.itemList1.VisibleOffset);

			this.itemList1.VisibleIndex = 0;

			Assert.AreEqual (0, this.itemList1.VisibleIndex);
			Assert.AreEqual (0, this.itemList1.VisibleOffset);
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
