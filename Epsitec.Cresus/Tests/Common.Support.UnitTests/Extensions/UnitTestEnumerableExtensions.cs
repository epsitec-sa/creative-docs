using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Support.UnitTests.Extensions
{


	[TestClass]
	public sealed  class UnitTestEnumerableExtensions
	{


		[TestMethod]
		public void AppendArgumentCheck()
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
		public void AppendTest()
		{
			Assert.IsTrue (Enumerable.Range (0, 11).SequenceEqual (Enumerable.Range (0, 10).Append (10)));
			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5).Concat (Enumerable.Range (6, 4))));

			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5, 6, 7, 8, 9)));
			Assert.IsTrue (Enumerable.Range (0, 10).SequenceEqual (Enumerable.Range (0, 5).Append (5, 6).Concat (Enumerable.Range (7, 3))));
		}


		[TestMethod]
		public void ShuffleArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IEnumerable<int>) null).Shuffle ()
			);
		}


		[TestMethod]
		public void ShuffleTest()
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
		public void ShuffleDistributionTest()
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
		public void SetEqualsArgumentCheck()
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
		public void SetEqualsTest()
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


	}


}
