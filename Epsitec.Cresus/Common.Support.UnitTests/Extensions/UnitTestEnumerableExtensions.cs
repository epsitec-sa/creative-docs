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
				List<int> shuffled = sequence.Shuffle ().ToList ();

				CollectionAssert.AreNotEqual (sequence, shuffled);
			}
		}

	}


}
