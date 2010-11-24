using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;
using System.Diagnostics;

using System.Xml;
using System.Text;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public class UnitTestDimensionTable
	{


		[TestMethod]
		public void DimensionsTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
			{
				new NumericDimension ("d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new NumericDimension ("d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new NumericDimension ("d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
			};

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			CollectionAssert.AreEqual (dimensions, table.Dimensions.ToList ());
		}


		[TestMethod]
		public void PossibleKeysTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
			{
				new NumericDimension ("d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new NumericDimension ("d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new NumericDimension ("d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
			};

			List<object[]> expectedExactKeys = new List<object[]> ();

			for (decimal i = 0; i < 3; i++)
			{
				for (decimal j = 0; j < 3; j++)
				{
					for (decimal k = 0; k < 3; k++)
					{
						object[] expectedExactKey = new object[] { i, j, k};

						expectedExactKeys.Add (expectedExactKey);
					}
				}
			}

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			List<object[]> actualExactKeys = table.PossibleKeys.ToList ();

			for (int i = 0; i <expectedExactKeys.Count; i++)
			{
				System.Diagnostics.Debug.WriteLine (i + ":\t" + string.Join (",", expectedExactKeys[i].Select (e => e.ToString ()).ToArray ()) + "\t" + string.Join (",", actualExactKeys[i].Select (e => e.ToString ()).ToArray ()));
			}

			Assert.AreEqual (expectedExactKeys.Count, actualExactKeys.Count);

			foreach (object[] expectedExactKey in expectedExactKeys)
			{
				Assert.IsTrue (actualExactKeys.Contains (expectedExactKey, new ArrayEqualityComparer ()));
			}
		}


		[TestMethod]
		public void IsValueDefinedTest()
		{
			NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			DimensionTable table = new DimensionTable (d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.IsFalse (table.IsValueDefined (i, j));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					table[i, j] = i + j;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.IsTrue (table.IsValueDefined (i, j));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					table[i, j] = null;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.IsFalse (table.IsValueDefined (i, j));
				}
			}
		}


		[TestMethod]
		public void IsNearestValueDefinedTest()
		{
			NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			DimensionTable table = new DimensionTable (d1, d2);

			for (decimal i = 1.5m; i < 3; i++)
			{
				for (decimal j = 1.5m; j < 3; j++)
				{
					Assert.IsFalse (table.IsNearestValueDefined (i, j));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					table[i, j] = i + j;
				}
			}

			for (decimal i = 1.5m; i < 3; i++)
			{
				for (decimal j = 1.5m; j < 3; j++)
				{
					Assert.IsTrue (table.IsNearestValueDefined (i, j));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					table[i, j] = null;
				}
			}

			for (decimal i = 1.5m; i < 3; i++)
			{
				for (decimal j = 1.5m; j < 3; j++)
				{
					Assert.IsFalse (table.IsNearestValueDefined (i, j));
				}
			}
		}


		[TestMethod]
		public void GetAndSetValueTest()
		{
			NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			DimensionTable table = new DimensionTable (d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					table[i, j] = i + j;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.AreEqual (i + j, table[i, j]);
				}
			}
		}


		//[TestMethod]
		//public void ImportExportTest()
		//{
		//    List<PriceCalculatorNumericDimension> dimensions = new List<PriceCalculatorNumericDimension> ()
		//    {
		//        new PriceCalculatorNumericDimension ("d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
		//        new PriceCalculatorNumericDimension ("d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
		//        new PriceCalculatorNumericDimension ("d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
		//    };

		//    PriceCalculatorTable table = new PriceCalculatorTable (1.2345m, dimensions.ToArray ());

		//    for (decimal i = 0; i < 3; i++)
		//    {
		//        for (decimal j = 0; j < 3; j++)
		//        {
		//            for (decimal k = 0; k < 3; k++)
		//            {
		//                object[] key = new object[] { i, j, k };

		//                table[key] = (100 * i) + (10 * j) + (1 * k);
		//            }
		//        }
		//    }

		//    StringBuilder data = new StringBuilder ();

		//    XmlWriterSettings settings = new XmlWriterSettings ()
		//    {
		//        Indent = true,
		//    };

		//    using (XmlWriter xmlWriter = XmlWriter.Create (data, settings))
		//    {
		//        table.Export (xmlWriter);
		//    }

		//    System.Console.WriteLine (data.ToString ());
		//}


		[TestMethod]
		public void StressTest1()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ();

			foreach (int i in Enumerable.Range (0, 10))
			{
				string name = "d" + i;
				decimal[] values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).ToArray ();
				RoundingMode mode = RoundingMode.Down;

				NumericDimension dimension = new NumericDimension (name, values, mode);

				dimensions.Add (dimension);
			}

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			System.Random dice = new System.Random ();

			int nbTestValues = 100000;

			IDictionary<object[], decimal> data1 =
				 Enumerable.Range (0, nbTestValues)
				.Select (d => Enumerable.Range (0, 10).Select (i => (object) System.Convert.ToDecimal (dice.Next (0, 10))).Shuffle ().ToArray ())
				.Distinct (new ArrayEqualityComparer ())
				.ToDictionary (e => e, e => (decimal) dice.NextDouble ());

			List<object[]> data2 =
				 Enumerable.Range (0, nbTestValues)
				.Select (d => Enumerable.Range (0, 10).Select (i => (object) System.Convert.ToDecimal (9 * dice.NextDouble ())).Shuffle ().ToArray ())
				.ToList ();

			System.Console.WriteLine ("Generated " + nbTestValues + " test values.");
			
			Stopwatch watch = new Stopwatch ();
			watch.Start ();
			foreach (var item in data1)
			{
				Assert.IsFalse (table.IsValueDefined (item.Key));
			}

			watch.Stop ();
			System.Console.WriteLine ("IsValueDefined time: " + watch.ElapsedMilliseconds);
			watch.Restart ();

			foreach (var item in data1)
			{
				table[item.Key] = item.Value;
			}

			watch.Stop ();
			System.Console.WriteLine ("SetValue time: " + watch.ElapsedMilliseconds);
			watch.Restart ();

			foreach (var item in data1)
			{
				Assert.IsTrue (table.IsNearestValueDefined (item.Key));
			}

			watch.Stop ();
			System.Console.WriteLine ("IsNearestValueDefined time: " + watch.ElapsedMilliseconds);
			watch.Restart ();

			foreach (var item in data1)
			{
				Assert.AreEqual (item.Value, table[item.Key]);
			}

			watch.Stop ();
			System.Console.WriteLine ("GetValue defined time: " + watch.ElapsedMilliseconds);
			watch.Restart ();

			foreach (var item in data2)
			{
				if (table.IsNearestValueDefined (item))
				{
					Assert.IsNotNull (table[item]);
				}
				else
				{
					Assert.IsNull (table[item]);
				}
			}

			watch.Stop ();
			System.Console.WriteLine ("GetValue2 undefined time: " + watch.ElapsedMilliseconds);
		}


		[TestMethod]
		public void StressTest2()
		{

			List<AbstractDimension> dimensions = new List<AbstractDimension> ();

			int nbDimensions = 7;
			int nbValues = 10;
				
			foreach (int i in Enumerable.Range (0, nbDimensions))
			{
				string name = "d" + i;
				decimal[] values = Enumerable.Range (0, nbValues).Select (v => System.Convert.ToDecimal (v)).ToArray ();
				RoundingMode mode = RoundingMode.Down;

				NumericDimension dimension = new NumericDimension (name, values, mode);

				dimensions.Add (dimension);
			}

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			System.Console.WriteLine ("Generated table with " + nbDimensions + " dimensions with " + nbValues + " values.");

			Stopwatch watch = new Stopwatch ();

			watch.Start ();

			foreach (var item in table.PossibleKeys)
			{
				Assert.IsNotNull (item);
			}

			watch.Stop ();
			System.Console.WriteLine ("PossibleKeys time: " + watch.ElapsedMilliseconds);
		}



	}


}
