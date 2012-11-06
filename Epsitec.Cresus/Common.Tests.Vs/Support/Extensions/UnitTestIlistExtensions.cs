using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Tests.Vs.Support.Extensions
{


	[TestClass]
	public sealed class UnitTestIlistExtensions
	{


		[TestMethod]
		public void InsertAtIndexArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => ((IList<int>) null).InsertAtIndex (0, 0)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => new List<int> ().InsertAtIndex (-1, 0)
			);
		}


		[TestMethod]
		public void InsertAtIndexTest()
		{
			var a1 = new List<int> () {};
			var e1 = new List<int> () { 1 };
			a1.InsertAtIndex (0, 1);
			CollectionAssert.AreEqual (e1, a1);		
		
			var a2 = new List<int> () { };
			var e2 = new List<int> () { 0, 0, 0, 1};
			a2.InsertAtIndex (3, 1);
			CollectionAssert.AreEqual (e2, a2);
				
			var a3 = new List<int> () { 0, 1, 2};
			var e3 = new List<int> () { 0, 1, 2, 3};
			a3.InsertAtIndex (3, 3);
			CollectionAssert.AreEqual (e3, a3);		
		
			var a4 = new List<int> () { 0, 1, 2};
			var e4 = new List<int> () { 0, 1, 2, 0, 0, 5};
			a4.InsertAtIndex (5, 5);
			CollectionAssert.AreEqual (e4, a4);		
		
			var a5 = new List<string> () { };
			var e5 = new List<string> () { null, null, "a" };
			a5.InsertAtIndex (2, "a");
			CollectionAssert.AreEqual (e5, a5);
			
			var a6 = new List<string> () { "a", "b", "c" };
			var e6 = new List<string> () { "a", "b", "c", null, "e" };
			a6.InsertAtIndex (4, "e");
			CollectionAssert.AreEqual (e6, a6);
		}


		[TestMethod]
		public void GetRandomElementArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => ((IList<int>) null).GetRandomElement ()
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => new List<int> ().GetRandomElement ()
			);
		}


		[TestMethod]
		public void GetRandomElementTest()
		{
			var elements = Enumerable.Range (0, 100).ToList ();

			for (int i = 0; i < 1000; i++)
			{
				int element = elements.GetRandomElement ();

				Assert.IsTrue (element >= 0);
				Assert.IsTrue (element <= 99);
			}
		}


		[TestMethod]
		public void GetRandomElementDistribution()
		{
			int nbElements = 100;
			int nbCalls = 1000000;
			
			var elements = Enumerable.Range (0, nbElements).ToList ();
			var counts = elements.ToDictionary (e => e, e => 0);

			for (int i = 0; i < nbCalls; i++)
			{
				var value = elements.GetRandomElement ();

				counts[value] += 1;
			}

			double expectedCount = (double) nbCalls / nbElements;

			foreach (int count in counts.Keys)
			{
				Assert.IsTrue (System.Math.Abs (counts[count] - expectedCount) / expectedCount < 0.05);
			}
		}


		[TestMethod]
		public void RemoveAllArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => ((IList<int>) null).RemoveAll (x => x == 0)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => ((IList<int>) new List<int> ()).RemoveAll (null)
			);
		}


		[TestMethod]
		public void RemoveAllTest()
		{
			this.RemoveAllTest
			(
				new List<int> ()  { },
				x => x == 0, 
				new List<int> () { }
			);
			
			this.RemoveAllTest
			(
				new List<int> () { 0, 1, 2 },
				x => x == 0,
				new List<int> () { 1, 2 }
			);

			this.RemoveAllTest
			(
				new List<int> () { 0, 1, 2, 3, 4 },
				x => x > 5,
				new List<int> () { 0, 1, 2, 3, 4 }
			);

			this.RemoveAllTest
			(
				new List<int> () { 2, 0, 2, 2, 2, 1, 2, 3, 4, 2 },
				x => x == 2,
				new List<int> () { 0, 1, 3, 4 }
			);

			this.RemoveAllTest
			(
				new List<int> () { 0, 0, 0, 0, 0 },
				x => x == 0,
				new List<int> () { }
			);
		}


		private void RemoveAllTest<T>(List<T> original, Func<T, bool> predicate, List<T> expected)
		{
			((IList<T>) original).RemoveAll (predicate);

			CollectionAssert.AreEqual (expected, original);
		}


	}


}
