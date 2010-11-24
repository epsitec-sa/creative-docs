using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestPriceCalculatorDimension
	{


		[TestMethod]
		public void NameTest()
		{
			List<string> names = new List<string> ()
			{
				"coucou",
				"blabla",
				"test",
			};

			List<string> values = new List<string> ()
			{
				"Albert",
				"Alfred",
				"Alfonse",
			};

			foreach (string name in names)
			{
				PriceCalculatorDimension<string> pcd = new PriceCalculatorDimension<string> (name, values);

				Assert.AreEqual (name, pcd.Name);
			}
		}

		[TestMethod]
		public void ValuesTest()
		{
			string name = "blabla";

			List<string> values = new List<string> ()
			{
				"Alain",
				"Bernard",
				"Christophe",
				"Daniel",
			}.OrderBy (v => v).ToList ();

			for (int i = 0; i < 10; i++)
			{
				PriceCalculatorDimension<string> pcd = new PriceCalculatorDimension<string> (name, values.Shuffle ());

				CollectionAssert.AreEqual (values, pcd.GenericValues.ToList ());
				CollectionAssert.AreEqual (values, pcd.Values.Cast<string> ().ToList ());
			}
		}


		[TestMethod]
		public void IsDefinedForExactValueTest()
		{
			string name = "blabla";

			List<string> values1 = new List<string> ()
			{
				"Alain",
				"Bernard",
				"Christophe",
				"Daniel",
			};

			List<string> values2 = new List<string> ()
			{
				"Bob",
				"Brad",
				"Ben",
				"Bert",
			};

			PriceCalculatorDimension<string> pcd = new PriceCalculatorDimension<string> (name, values1.Shuffle ());

			foreach (string value in values1)
			{
				Assert.IsTrue (pcd.IsDefinedForExactValue (value));
				Assert.IsTrue (pcd.IsDefinedForExactGenericValue (value));
			}

			foreach (string value in values2)
			{
				Assert.IsFalse (pcd.IsDefinedForExactValue (value));
				Assert.IsFalse (pcd.IsDefinedForExactGenericValue (value));
			}
		}


		[TestMethod]
		public void GetValueTest()
		{
			string name = "blabla";

			List<string> values = new List<string> ()
			{
				"Alain",
				"Bernard",
				"Christophe",
				"Daniel",
			};

			PriceCalculatorDimension<string> pcd = new PriceCalculatorDimension<string> (name, values.Shuffle ());

			foreach (string value in values)
			{
				Assert.AreEqual (value, pcd.GetGenericValue (value));
				Assert.AreEqual (value, pcd.GetValue (value));
			}

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => pcd.GetGenericValue ("coucou")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => pcd.GetValue ("coucou")
			);
		}


	}


}
