using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestArrayEqualityComparer
	{


		[TestMethod]
		public void TestEquals()
		{
			List<double[]> arrays = new List<double[]> ();

			System.Random dice = new System.Random ();

			for (int i = 1; i < 10; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					arrays.Add (Enumerable.Range (0, i).Select (v => dice.NextDouble ()).Shuffle ().ToArray ());
				}
			}

			ArrayEqualityComparer<double> pcec = ArrayEqualityComparer<double>.Instance;

			foreach (double[] array1 in arrays)
			{
				foreach (double[] array2 in arrays)
				{
					// Actually, this test is too restrictive, since it could happen that two random
					// arrays could have the same value, even if the probability is low. We assume in
					// this test that we are lucky.
					// Marc

					Assert.AreEqual ((array1 == array2), pcec.Equals (array1, array2));
				}
			}

			Assert.IsFalse (pcec.Equals (arrays.First (), null));
			Assert.IsFalse (pcec.Equals (null, arrays.First ()));
			Assert.IsFalse (pcec.Equals (null, null));
		}


		[TestMethod]
		public void TestHashCode()
		{
			List<double[]> arrays = new List<double[]> ();

			System.Random dice = new System.Random ();

			for (int i = 1; i < 100; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					arrays.Add (Enumerable.Range (0, i).Select (v => dice.NextDouble ()).ToArray ());
				}
			}

			ArrayEqualityComparer<double> pcec = ArrayEqualityComparer<double>.Instance;

			foreach (double[] array1 in arrays)
			{
				foreach (double[] array2 in arrays)
				{
					// Actually, this test is too restrictive, as two different arrays could have
					// the same hash code. We assume in this test that we are lucky.
					// Marc

					Assert.AreEqual (pcec.Equals (array1, array2), (pcec.GetHashCode (array1) == pcec.GetHashCode (array2)));
				}
			}

			Assert.AreEqual (0, pcec.GetHashCode (null));
		}


	}


}
