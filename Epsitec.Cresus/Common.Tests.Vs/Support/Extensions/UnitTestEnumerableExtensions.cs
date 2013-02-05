//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Tests.Vs.Support.Extensions
{
	[TestClass]
	public sealed  class UnitTestEnumerableExtensions
	{
		[TestMethod]
		public void TestIndexOfArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<object>) null).IndexOf (new object (), object.Equals)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new List<object> ().IndexOf (new object (), (System.Func<object, object, bool>) null)
			);
		}


		[TestMethod]
		public void TestIndexOf()
		{
			List<int> sequence = Enumerable.Range (0, 100).Shuffle ().ToList ();

			for (int i = 0; i < sequence.Count; i++)
			{
				int expected = sequence.IndexOf (i);
				int actual = sequence.IndexOf (i, (e1, e2) => e1 == e2);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void TestAppendArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).Append (0)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
			  () => new List<int> ().Append (null)
			);
		}


		[TestMethod]
		public void TestAppend()
		{
			Assert.IsTrue (Enumerable.Range (0, 11).SequenceEqual (Enumerable.Range (0, 10).Append (10)));
			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5).Concat (Enumerable.Range (6, 4))));

			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5, 6, 7, 8, 9)));
			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5, 6).Concat (Enumerable.Range (7, 3))));
		}


		[TestMethod]
		public void TestShuffleArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).Shuffle ()
			);
		}


		[TestMethod]
		public void TestShuffle()
		{
			List<int> sequence = Enumerable.Range (0, 100).ToList ();

			for (int i = 0; i < 100; i++)
			{
				List<int> shuffledSequence = sequence.Shuffle ().ToList ();

				CollectionAssert.AreNotEqual (sequence, shuffledSequence);
				CollectionAssert.IsSubsetOf (sequence, shuffledSequence);
				CollectionAssert.IsSubsetOf (shuffledSequence, sequence);
			}
		}


		[TestMethod]
		public void TestShuffleDistribution()
		{
			int sequenceLength = 100;
			int nbSequences = 10000;

			List<int> sequence = Enumerable.Range (0, sequenceLength).ToList ();

			List<List<int>> shuffledSequences = new List<List<int>> ();

			for (int i = 0; i < nbSequences; i++)
			{
				List<int> shuffledSequence = sequence.Shuffle ().ToList ();

				shuffledSequences.Add (shuffledSequence);
			}

			for (int i = 0; i < sequenceLength; i++)
			{
				double sum = 0;

				for (int j = 0; j < nbSequences; j++)
				{
					sum += shuffledSequences[j][i];
				}

				double expected = sequence.Average();
				double average = (sum / nbSequences);

				Assert.IsTrue (System.Math.Abs (expected - average) < 1);
			}
		}


		[TestMethod]
		public void TestSetEqualsArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).SetEquals (new List<int> ())
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new List<int> ().SetEquals (null)
			);
		}


		[TestMethod]
		public void TestSetEquals()
		{
			Assert.IsTrue (new List<int> ().SetEquals (new List<int> ()));

			for (int i = 0; i < 100; i++)
			{
				List<int> set1 = Enumerable.Range (0, 100).Shuffle ().ToList ();
				List<int> set2 = Enumerable.Range (0, 100).Shuffle ().ToList ();

				Assert.IsTrue (set1.SetEquals (set2));

				if (set1[0] != set2[0])
				{
					Assert.IsFalse (set1.Skip (1).SetEquals (set2.Skip (1)));
				}

				Assert.IsTrue (set1.Append (set1[0]).SetEquals (set2.Append (set2[0])));
			}
		}


		[TestMethod]
		public void TestSplitArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).Split (i => i == 0)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).Split (null)
			);
		}


		[TestMethod]
		public void TestSplit()
		{
			var result1 = Enumerable.Range (0, 10).Split (i => i < 5);

			CollectionAssert.AreEqual (Enumerable.Range (5, 5).ToList (), result1.Item1.ToList ());
			CollectionAssert.AreEqual (Enumerable.Range (0, 5).ToList (), result1.Item2.ToList ());

			var result2 = Enumerable.Range (0, 10).Split (i => i < 10);

			CollectionAssert.AreEqual (Enumerable.Range (0, 0).ToList (), result2.Item1.ToList ());
			CollectionAssert.AreEqual (Enumerable.Range (0, 10).ToList (), result2.Item2.ToList ());

			var result3 = Enumerable.Range (0, 10).Split (i => i < 0);

			CollectionAssert.AreEqual (Enumerable.Range (0, 10).ToList (), result3.Item1.ToList ());
			CollectionAssert.AreEqual (Enumerable.Range (0, 0).ToList (), result3.Item2.ToList ());

			var result4 = Enumerable.Range (0, 0).Split (i => i < 0);

			CollectionAssert.AreEqual (Enumerable.Range (0, 0).ToList (), result4.Item1.ToList ());
			CollectionAssert.AreEqual (Enumerable.Range (0, 0).ToList (), result4.Item2.ToList ());
		}


		[TestMethod]
		public void TestAsReadOnlyCollectionArgument()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).AsReadOnlyCollection ()
			);
		}


		[TestMethod]
		public void TestAsReadOnlyCollection()
		{
			for (int i = 0; i < 25; i++)
			{
				var sequence = Enumerable.Range (0, 25).Shuffle ().ToList ();
				var readOnlySequence = sequence.Select (e => e).AsReadOnlyCollection ();

				Assert.IsTrue (((ICollection<int>) readOnlySequence).IsReadOnly);
				CollectionAssert.AreEqual (sequence, readOnlySequence);
			}
		}


		[TestMethod]
		public void TestCombineToTuples()
		{
			string[] a = new string[] { "A", "B", "C" };
			int[]    b = new int[] { 1, 2 };

			var r1 = a.CombineToTuples (b).Select (x => x.Item1 + "." + x.Item2.ToString (System.Globalization.CultureInfo.InvariantCulture)).ToArray ();
			var r2 = b.CombineToTuples (a).Select (x => x.Item2 + "." + x.Item1.ToString (System.Globalization.CultureInfo.InvariantCulture)).ToArray ();

			Assert.AreEqual ("A.1 B.2 C.0", string.Join (" ", r1));
			Assert.AreEqual ("A.1 B.2 C.0", string.Join (" ", r2));
		}
	}
}
