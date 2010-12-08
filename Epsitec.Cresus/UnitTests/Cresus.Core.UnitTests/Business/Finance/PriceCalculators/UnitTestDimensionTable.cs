using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;

using System.Diagnostics;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.UnitTests.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestDimensionTable
	{


		//[TestMethod]
		//public void ConstructorArgumentCheck()
		//{
		//    ExceptionAssert.Throw<System.ArgumentNullException>
		//    (
		//        () => new DimensionTable (null)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => new DimensionTable (new AbstractDimension[0])
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => new DimensionTable (new AbstractDimension[] { null })
		//    );
		//}


		[TestMethod]
		public void DimensionsTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
                new NumericDimension ("2", "d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
                new NumericDimension ("3", "d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
            };

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			CollectionAssert.AreEqual (dimensions, table.Dimensions.ToList ());
		}


		[TestMethod]
		public void PossibleKeysTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
                new NumericDimension ("2", "d2", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
                new NumericDimension ("3", "d3", new decimal[] { 0, 1, 2 }, RoundingMode.Up),
            };

			List<string[]> expectedExactKeys = new List<string[]> ();

			for (decimal i = 0; i < 3; i++)
			{
				for (decimal j = 0; j < 3; j++)
				{
					for (decimal k = 0; k < 3; k++)
					{
						string si = InvariantConverter.ConvertToString (i);
						string sj = InvariantConverter.ConvertToString (j);
						string sk = InvariantConverter.ConvertToString (k);

						string[] expectedExactKey = new string[] { si, sj, sk };

						expectedExactKeys.Add (expectedExactKey);
					}
				}
			}

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			List<string[]> actualExactKeys = table.PossibleKeys.ToList ();

			Assert.AreEqual (expectedExactKeys.Count, actualExactKeys.Count);

			foreach (string[] expectedExactKey in expectedExactKeys)
			{
				Assert.IsTrue (actualExactKeys.Contains (expectedExactKey, new ArrayEqualityComparer ()));
			}
		}


		//[TestMethod]
		//public void IsValueDefinedArgumentCheck()
		//{
		//    NumericDimension d1 = new NumericDimension ("1", "d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
		//    NumericDimension d2 = new NumericDimension ("2", "d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

		//    DimensionTable table = new DimensionTable (d1, d2);

		//    ExceptionAssert.Throw<System.ArgumentNullException>
		//    (
		//        () => table.IsValueDefined (null)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsValueDefined (1m)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsValueDefined (1m, 0m)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsValueDefined (1m, null)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsValueDefined (1m, 1.5m)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsValueDefined (1m, 1f)
		//    );
		//}


		[TestMethod]
		public void IsValueDefinedTest()
		{
			NumericDimension d1 = new NumericDimension ("1", "d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			NumericDimension d2 = new NumericDimension ("2", "d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			DimensionTable table = new DimensionTable (d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.IsFalse (table.IsValueDefined (si, sj));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					table[si, sj] = i + j;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.IsTrue (table.IsValueDefined (si, sj));
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					table[si, sj] = null;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.IsFalse (table.IsValueDefined (si, sj));
				}
			}
		}


		//[TestMethod]
		//public void IsNearestValueDefinedArgumentCheck()
		//{
		//    NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
		//    NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

		//    DimensionTable table = new DimensionTable (d1, d2);

		//    ExceptionAssert.Throw<System.ArgumentNullException>
		//    (
		//        () => table.IsNearestValueDefined (null)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsNearestValueDefined (1m)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsNearestValueDefined (1m, 0m)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsNearestValueDefined (1m, null)
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () => table.IsNearestValueDefined (1m, 1f)
		//    );
		//}


		//[TestMethod]
		//public void IsNearestValueDefinedTest()
		//{
		//    NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
		//    NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

		//    DimensionTable table = new DimensionTable (d1, d2);

		//    for (decimal i = 1.5m; i < 3; i++)
		//    {
		//        for (decimal j = 1.5m; j < 3; j++)
		//        {
		//            Assert.IsFalse (table.IsNearestValueDefined (i, j));
		//        }
		//    }

		//    for (decimal i = 1; i < 4; i++)
		//    {
		//        for (decimal j = 1; j < 4; j++)
		//        {
		//            table[i, j] = i + j;
		//        }
		//    }

		//    for (decimal i = 1.5m; i < 3; i++)
		//    {
		//        for (decimal j = 1.5m; j < 3; j++)
		//        {
		//            Assert.IsTrue (table.IsNearestValueDefined (i, j));
		//        }
		//    }

		//    for (decimal i = 1; i < 4; i++)
		//    {
		//        for (decimal j = 1; j < 4; j++)
		//        {
		//            table[i, j] = null;
		//        }
		//    }

		//    for (decimal i = 1.5m; i < 3; i++)
		//    {
		//        for (decimal j = 1.5m; j < 3; j++)
		//        {
		//            Assert.IsFalse (table.IsNearestValueDefined (i, j));
		//        }
		//    }
		//}


		//[TestMethod]
		//public void GetValueTestArgumentCheck()
		//{
		//    NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
		//    NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

		//    DimensionTable table = new DimensionTable (d1, d2);

		//    ExceptionAssert.Throw<System.ArgumentNullException>
		//    (
		//        () =>
		//        {
		//            var v = table[null];
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            var v = table[1m];
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            var v = table[1m, null];
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            var v = table[1m, 0m];
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            var v = table[1m, 1f];
		//        }
		//    );
		//}


		//[TestMethod]
		//public void SetValueTestArgumentCheck()
		//{
		//    NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
		//    NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

		//    DimensionTable table = new DimensionTable (d1, d2);

		//    ExceptionAssert.Throw<System.ArgumentNullException>
		//    (
		//        () =>
		//        {
		//            table[null] = 0;
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            table[1m] = 0;
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            table[1m, null] = 0;
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            table[1m, 0m] = 0;
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            table[1m, 1f] = 0;
		//        }
		//    );

		//    ExceptionAssert.Throw<System.ArgumentException>
		//    (
		//        () =>
		//        {
		//            table[1m, 11.5m] = 0;
		//        }
		//    );
		//}


		[TestMethod]
		public void GetAndSetValueTest()
		{
			NumericDimension d1 = new NumericDimension ("1", "d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			NumericDimension d2 = new NumericDimension ("2", "d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			DimensionTable table = new DimensionTable (d1, d2);

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					table[si, sj] = i + j;
				}
			}

			for (decimal i = 1; i < 4; i++)
			{
				for (decimal j = 1; j < 4; j++)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.AreEqual (i + j, table[si, sj]);
				}
			}
		}


		[TestMethod]
		public void ImportArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => DimensionTable.XmlImport (null)
			);
		}


		[TestMethod]
		public void ImportExportTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
            {
                new NumericDimension ("1", "d1", new decimal[] { 0, 1, 2, 3 }, RoundingMode.Down),
                new NumericDimension ("2", "d2", new decimal[] { 0, 1, 2, 3 }, RoundingMode.Up),
                new CodeDimension ("3", "d3", new string[] {"0", "1", "2", "3"}),
                new CodeDimension ("4", "d4", new string[] {"0", "1", "2", "3"}),
            };

			DimensionTable table1 = new DimensionTable (dimensions.ToArray ());

			foreach (string i in dimensions[0].Values.Where (v => v != "0"))
			{
				foreach (string j in dimensions[1].Values.Take (2).Where (v => v != "1"))
				{
					foreach (string k in dimensions[2].Values.Where (v => v != "2"))
					{
						foreach (string l in dimensions[3].Values.Where (v => v != "3"))
						{
							string[] key = new string[] { i, j, k, l };

							table1[key] = (1000 * System.Int32.Parse (i)) + (100 * System.Int32.Parse (j)) + (10 * System.Int32.Parse (k)) + (1 * System.Int32.Parse (l));
						}
					}
				}
			}

			XElement xDimensionTable1 = table1.XmlExport ();

			DimensionTable table2 = DimensionTable.XmlImport (xDimensionTable1);

			XElement xDimensionTable2 = table2.XmlExport ();

			var dimensions1 = table1.Dimensions.ToList ();
			var dimensions2 = table2.Dimensions.ToList ();

			Assert.AreEqual (dimensions1.Count, dimensions2.Count);

			for (int i = 0; i < dimensions1.Count; i++)
			{
				Assert.AreEqual (dimensions1[i].Name, dimensions2[i].Name);
				CollectionAssert.AreEqual (dimensions1[i].Values.ToList (), dimensions2[i].Values.ToList ());

				Assert.AreEqual (dimensions1.GetType (), dimensions2.GetType ());

				if (dimensions1[i] is NumericDimension)
				{
					Assert.AreEqual (((NumericDimension) dimensions1[i]).RoundingMode, ((NumericDimension) dimensions2[i]).RoundingMode);
				}
			}

			var possibleKeys1 = table1.PossibleKeys.ToList ();
			var possibleKeys2 = table1.PossibleKeys.ToList ();

			Assert.AreEqual (possibleKeys1.Count, possibleKeys2.Count);

			for (int i = 0; i < possibleKeys1.Count; i++)
			{
				Assert.IsTrue (new ArrayEqualityComparer ().Equals (possibleKeys1[i], possibleKeys2[i]));
			}

			foreach (string[] key in possibleKeys1)
			{
				Assert.AreEqual (table1[key], table2[key]);
				//Assert.AreEqual (table1.IsNearestValueDefined (key), table2.IsNearestValueDefined (key));
				Assert.AreEqual (table1.IsValueDefined (key), table2.IsValueDefined (key));
			}

			Assert.AreEqual (xDimensionTable1.ToString (), xDimensionTable2.ToString ());
		}


		//[TestMethod]
		//public void StressTest1()
		//{
		//    List<NumericDimension> dimensions = new List<NumericDimension> ();

		//    foreach (int i in Enumerable.Range (0, 10))
		//    {
		//        string name = "d" + i;
		//        decimal[] values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).ToArray ();
		//        RoundingMode mode = RoundingMode.Down;

		//        NumericDimension dimension = new NumericDimension (name, values, mode);

		//        dimensions.Add (dimension);
		//    }

		//    DimensionTable table = new DimensionTable (dimensions.ToArray ());

		//    System.Random dice = new System.Random ();

		//    int nbTestValues = 100000;

		//    IDictionary<object[], decimal> data1 =
		//         Enumerable.Range (0, nbTestValues)
		//        .Select (d => Enumerable.Range (0, 10).Select (i => (object) System.Convert.ToDecimal (dice.Next (0, 10))).Shuffle ().ToArray ())
		//        .Distinct (new ArrayEqualityComparer ())
		//        .ToDictionary (e => e, e => (decimal) dice.NextDouble ());

		//    List<object[]> data2 =
		//         Enumerable.Range (0, nbTestValues)
		//        .Select (d => Enumerable.Range (0, 10).Select (i => (object) System.Convert.ToDecimal (9 * dice.NextDouble ())).Shuffle ().ToArray ())
		//        .ToList ();

		//    System.Console.WriteLine ("Generated " + nbTestValues + " test values.");

		//    Stopwatch watch = new Stopwatch ();
		//    watch.Start ();
		//    foreach (var item in data1)
		//    {
		//        Assert.IsFalse (table.IsValueDefined (item.Key));
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("IsValueDefined time: " + watch.ElapsedMilliseconds);
		//    watch.Restart ();

		//    foreach (var item in data1)
		//    {
		//        table[item.Key] = item.Value;
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("SetValue time: " + watch.ElapsedMilliseconds);
		//    watch.Restart ();

		//    foreach (var item in data1)
		//    {
		//        Assert.IsTrue (table.IsNearestValueDefined (item.Key));
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("IsNearestValueDefined time: " + watch.ElapsedMilliseconds);
		//    watch.Restart ();

		//    foreach (var item in data1)
		//    {
		//        Assert.AreEqual (item.Value, table[item.Key]);
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("GetValue defined time: " + watch.ElapsedMilliseconds);
		//    watch.Restart ();

		//    foreach (var item in data2)
		//    {
		//        if (table.IsNearestValueDefined (item))
		//        {
		//            Assert.IsNotNull (table[item]);
		//        }
		//        else
		//        {
		//            Assert.IsNull (table[item]);
		//        }
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("GetValue2 undefined time: " + watch.ElapsedMilliseconds);
		//}


		//[TestMethod]
		//public void StressTest2()
		//{

		//    List<AbstractDimension> dimensions = new List<AbstractDimension> ();

		//    int nbDimensions = 7;
		//    int nbValues = 10;

		//    foreach (int i in Enumerable.Range (0, nbDimensions))
		//    {
		//        string name = "d" + i;
		//        decimal[] values = Enumerable.Range (0, nbValues).Select (v => System.Convert.ToDecimal (v)).ToArray ();
		//        RoundingMode mode = RoundingMode.Down;

		//        NumericDimension dimension = new NumericDimension (name, values, mode);

		//        dimensions.Add (dimension);
		//    }

		//    DimensionTable table = new DimensionTable (dimensions.ToArray ());

		//    System.Console.WriteLine ("Generated table with " + nbDimensions + " dimensions with " + nbValues + " values.");

		//    Stopwatch watch = new Stopwatch ();

		//    watch.Start ();

		//    foreach (var item in table.PossibleKeys)
		//    {
		//        Assert.IsNotNull (item);
		//    }

		//    watch.Stop ();
		//    System.Console.WriteLine ("PossibleKeys time: " + watch.ElapsedMilliseconds);
		//}



	}


}
