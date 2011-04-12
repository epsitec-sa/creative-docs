using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Common.Tests.Vs.Support.Extensions
{


	[TestClass]
	public sealed class UnitTestIlistExtensions
	{



		[TestMethod]
		public void GetRandomElementArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((IList<int>) null).GetRandomElement ()
			);

			ExceptionAssert.Throw<System.ArgumentException>
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


	}


}
