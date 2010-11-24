using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;
using System.Diagnostics;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public class UnitTestPriceCalculatorTable
	{


		[TestMethod]
		public void DimensionsTest()
		{
			List<PriceCalculatorNumericDimension> dimensions = new List<PriceCalculatorNumericDimension> ()
			{
				new PriceCalculatorNumericDimension ("d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new PriceCalculatorNumericDimension ("d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new PriceCalculatorNumericDimension ("d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
			};

			PriceCalculatorTable table = new PriceCalculatorTable (1.2345m, dimensions.ToArray ());

			CollectionAssert.AreEqual (dimensions, table.Dimensions.ToList ());
		}


		[TestMethod]
		public void ExactKeysTest()
		{
			List<PriceCalculatorNumericDimension> dimensions = new List<PriceCalculatorNumericDimension> ()
			{
				new PriceCalculatorNumericDimension ("d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new PriceCalculatorNumericDimension ("d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
				new PriceCalculatorNumericDimension ("d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
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

			PriceCalculatorTable table = new PriceCalculatorTable (1.2345m, dimensions.ToArray ());

			List<object[]> actualExactKeys = table.ExactKeys.ToList ();

			for (int i = 0; i <expectedExactKeys.Count; i++)
			{
				System.Diagnostics.Debug.WriteLine (i + ":\t" + string.Join (",", expectedExactKeys[i].Select (e => e.ToString ()).ToArray ()) + "\t" + string.Join (",", actualExactKeys[i].Select (e => e.ToString ()).ToArray ()));
			}

			Assert.AreEqual (expectedExactKeys.Count, actualExactKeys.Count);

			foreach (object[] expectedExactKey in expectedExactKeys)
			{
				Assert.IsTrue (actualExactKeys.Contains (expectedExactKey, new PriceCalculatorEqualityComparer ()));
			}
		}


		[TestMethod]
		public void IsExactDefinedTest()
		{
			PriceCalculatorNumericDimension d1 = new PriceCalculatorNumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			PriceCalculatorNumericDimension d2 = new PriceCalculatorNumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			PriceCalculatorTable table = new PriceCalculatorTable (1.12345m, d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.IsFalse (table.IsExactValueDefined (i, j));
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
					Assert.IsTrue (table.IsExactValueDefined (i, j));
				}
			}

			table.Clear ();

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.IsFalse (table.IsExactValueDefined (i, j));
				}
			}
		}


		[TestMethod]
		public void IsNearestValueDefinedTest()
		{
			PriceCalculatorNumericDimension d1 = new PriceCalculatorNumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			PriceCalculatorNumericDimension d2 = new PriceCalculatorNumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			PriceCalculatorTable table = new PriceCalculatorTable (1.12345m, d1, d2);

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

			table.Clear ();

			for (decimal i = 1.5m; i < 3; i++)
			{
				for (decimal j = 1.5m; j < 3; j++)
				{
					Assert.IsFalse (table.IsNearestValueDefined (i, j));
				}
			}
		}


		[TestMethod]
		public void DefaultValueTest()
		{
			PriceCalculatorNumericDimension d1 = new PriceCalculatorNumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			PriceCalculatorNumericDimension d2 = new PriceCalculatorNumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			PriceCalculatorTable table = new PriceCalculatorTable (1.12345m, d1, d2);

			Assert.AreEqual (1.12345m, table.DefaultValue);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.AreEqual (table.DefaultValue, table[i, j]);
				}
			}
		}


		[TestMethod]
		public void GetAndSetValueTest()
		{
			PriceCalculatorNumericDimension d1 = new PriceCalculatorNumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			PriceCalculatorNumericDimension d2 = new PriceCalculatorNumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			PriceCalculatorTable table = new PriceCalculatorTable (1.12345m, d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					Assert.AreEqual (table.DefaultValue, table[i, j]);
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
					Assert.AreEqual (i + j, table[i, j]);
				}
			}
		}


		[TestMethod]
		public void StressTest1()
		{
			List<PriceCalculatorNumericDimension> dimensions = new List<PriceCalculatorNumericDimension> ();

			foreach (int i in Enumerable.Range (0, 10))
			{
				string name = "d" + i;
				decimal[] values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).ToArray ();
				RoundingMode mode = RoundingMode.Down;

				PriceCalculatorNumericDimension dimension = new PriceCalculatorNumericDimension (name, values, mode);

				dimensions.Add (dimension);
			}

			PriceCalculatorTable table = new PriceCalculatorTable (1.2345m, dimensions.ToArray ());

			System.Random dice = new System.Random ();

			int nbTestValues = 100000;

			IDictionary<object[], decimal> data1 =
				 Enumerable.Range (0, nbTestValues)
				.Select (d => Enumerable.Range (0, 10).Select (i => (object) System.Convert.ToDecimal (dice.Next (0, 10))).Shuffle ().ToArray ())
				.Distinct (new PriceCalculatorEqualityComparer ())
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
				Assert.IsFalse (table.IsExactValueDefined (item.Key));
			}

			watch.Stop ();
			System.Console.WriteLine ("IsExactValueDefined time: " + watch.ElapsedMilliseconds);
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
				Assert.IsTrue (table.IsExactValueDefined (item.Key));
			}

			watch.Stop ();
			System.Console.WriteLine ("IsExactValueDefined time: " + watch.ElapsedMilliseconds);
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
				Assert.IsNotNull (table[item]);
			}

			watch.Stop ();
			System.Console.WriteLine ("GetValue2 undefined time: " + watch.ElapsedMilliseconds);
		}


		[TestMethod]
		public void StressTest2()
		{

			List<PriceCalculatorDimension> dimensions = new List<PriceCalculatorDimension> ();

			int nbDimensions = 8;
			int nbValues = 10;
				
			foreach (int i in Enumerable.Range (0, nbDimensions))
			{
				string name = "d" + i;
				decimal[] values = Enumerable.Range (0, nbValues).Select (v => System.Convert.ToDecimal (v)).ToArray ();
				RoundingMode mode = RoundingMode.Down;

				PriceCalculatorNumericDimension dimension = new PriceCalculatorNumericDimension (name, values, mode);

				dimensions.Add (dimension);
			}

			PriceCalculatorTable table = new PriceCalculatorTable (1.2345m, dimensions.ToArray ());

			System.Console.WriteLine ("Generated table with " + nbDimensions + " dimensions with " + nbValues + " values.");

			Stopwatch watch = new Stopwatch ();

			watch.Start ();

			foreach (var item in table.ExactKeys)
			{
				Assert.IsNotNull (item);
			}

			watch.Stop ();
			System.Console.WriteLine ("ExactKeys time: " + watch.ElapsedMilliseconds);
		}



	}


}
