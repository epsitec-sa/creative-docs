using Epsitec.Common.Support;
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


		[TestMethod]
		public void Constructor2ArgumentCheck()
		{
			Assert.Inconclusive ();
			
			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () => new DimensionTable (null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new DimensionTable (new AbstractDimension[0])
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new DimensionTable (new AbstractDimension[] { null })
			//);
		}


		[TestMethod]
		public void Constructor1Test()
		{
			DimensionTable dimensionTable = new DimensionTable ();

			Assert.IsFalse (dimensionTable.Dimensions.Any ());
			this.CheckDimensionTableIsEmpty (dimensionTable);
		}


		[TestMethod]
		public void Constructor2Test()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			};

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());

			CollectionAssert.AreEqual (dimensions, dimensionTable.Dimensions.ToList ());
			this.CheckDimensionTableIsEmpty (dimensionTable);
		}


		[TestMethod]
		public void DimensionsTest()
		{
			int initialLength = 3;
			int totalLentgh = 6;
			
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
				new NumericDimension ("4", "d4", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("5", "d5", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("6", "d6", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.Take (initialLength).ToArray ());

			CollectionAssert.AreEqual (dimensions.Take (initialLength).ToList (), dimensionTable.Dimensions.ToList ());

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimensionTable.AddDimension (dimensions[i]);

				CollectionAssert.AreEqual (dimensions.Take (i + 1).ToList (), dimensionTable.Dimensions.ToList ());
			}

			for (int i = 0; i < initialLength; i++)
			{
				dimensionTable.RemoveDimension (dimensions[i]);

				CollectionAssert.AreEqual (dimensions.Skip (i + 1).ToList (), dimensionTable.Dimensions.ToList ());
			}
		}


		[TestMethod]
		public void DimensionsCountTest()
		{
			int initialLength = 3;
			int totalLentgh = 6;

			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
				new NumericDimension ("4", "d4", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("5", "d5", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("6", "d6", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.Take (initialLength).ToArray ());

			CollectionAssert.AreEqual (dimensions.Take (initialLength).ToList (), dimensionTable.Dimensions.ToList ());

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimensionTable.AddDimension (dimensions[i]);

				Assert.AreEqual (i + 1, dimensionTable.DimensionCount);
			}

			for (int i = 0; i < initialLength; i++)
			{
				dimensionTable.RemoveDimension (dimensions[i]);

				Assert.AreEqual (totalLentgh - i - 1, dimensionTable.DimensionCount);
			}
		}


		[TestMethod]
		public void KeysTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
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

			List<string[]> actualExactKeys = table.Keys.ToList ();

			Assert.AreEqual (expectedExactKeys.Count, actualExactKeys.Count);

			foreach (string[] expectedExactKey in expectedExactKeys)
			{
				Assert.IsTrue (actualExactKeys.Contains (expectedExactKey, new ArrayEqualityComparer<string> ()));
			}
		}


		[TestMethod]
		public void DefinedEntriesTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());

			Dictionary<string[], decimal> expectedEntries = new Dictionary<string[], decimal> (new ArrayEqualityComparer<string> ());

			System.Random dice = new System.Random ();

			for (decimal i = 0; i < 3; i++)
			{
				for (decimal j = 0; j < 3; j++)
				{
					for (decimal k = 0; k < 3; k++)
					{
						string si = InvariantConverter.ConvertToString (i);
						string sj = InvariantConverter.ConvertToString (j);
						string sk = InvariantConverter.ConvertToString (k);

						string[] key = new string[] { si, sj, sk };
						decimal value = (decimal) dice.NextDouble ();

						dimensionTable[key.ToArray()] = value;
						expectedEntries[key.ToArray()] = value;

						int index1 = dice.Next (0, dimensions.Count);
						int index2 = dice.Next (0, dimensions.Count);

						for (int nb = 0; nb < 2; nb++)
						{
							Assert.AreEqual (expectedEntries.Count, dimensionTable.DefinedEntries.Count ());

							foreach (var entry in dimensionTable.DefinedEntries)
							{
								Assert.AreEqual (expectedEntries[entry.Key], entry.Value);
							}

							expectedEntries = expectedEntries.ToDictionary (
								kvp =>
								{
									string value1 = kvp.Key[index1];
									string value2 = kvp.Key[index2];

									string[] swappedKey = kvp.Key.ToArray();

									swappedKey[index1] = value2;
									swappedKey[index2] = value1;

									return swappedKey;
								},
								kvp => kvp.Value,
								new ArrayEqualityComparer<string> ()
							);

							dimensionTable.SwapDimensionsAt (index1, index2);
						}			
					}
				}
			}
		}
		

		[TestMethod]
		public void AddDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void AddDimensionTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			DimensionTable dimensionTable = new DimensionTable ();
			Assert.IsFalse (dimensionTable.Dimensions.Any ());

			
			for (int i = 0; i < dimensions.Count; i++)
			{
				this.PopulateDimensionTable (dimensionTable);

				dimensionTable.AddDimension (dimensions[i]);

				CollectionAssert.AreEqual (dimensions.Take (i + 1).ToList (), dimensionTable.Dimensions.ToList ());
				this.CheckDimensionTableIsEmpty (dimensionTable);
			}
		}


		[TestMethod]
		public void InsertDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void InsertDimensionTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			List<AbstractDimension> expected = new List<AbstractDimension> ();

			System.Random dice = new System.Random ();

			DimensionTable dimensionTable = new DimensionTable ();
			Assert.IsFalse (dimensionTable.Dimensions.Any ());
			
			for (int i = 0; i < dimensions.Count; i++)
			{
				this.PopulateDimensionTable (dimensionTable);

				int index = dice.Next (0, dimensionTable.DimensionCount + 1);
				AbstractDimension value = dimensions[i];

				dimensionTable.InsertDimension (index, value);
				expected.Insert (index, value);

				CollectionAssert.AreEqual (expected, dimensionTable.Dimensions.ToList ());
				this.CheckDimensionTableIsEmpty (dimensionTable);
			}
		}


		[TestMethod]
		public void RemoveDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void RemoveDimensionTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			CollectionAssert.AreEqual (dimensions, dimensionTable.Dimensions.ToList ());

			for (int i = 0; i < dimensions.Count; i++)
			{
				this.PopulateDimensionTable (dimensionTable);

				dimensionTable.RemoveDimension (dimensions[i]);

				CollectionAssert.AreEqual (dimensions.Skip (i + 1).ToList (), dimensionTable.Dimensions.ToList ());
				this.CheckDimensionTableIsEmpty (dimensionTable);
			}
		}


		[TestMethod]
		public void RemoveDimensionAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void RemoveDimensionAtTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			List<AbstractDimension> expected = dimensions.ToList ();

			System.Random dice = new System.Random ();

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());

			for (int i = 0; i < dimensions.Count; i++)
			{
				this.PopulateDimensionTable (dimensionTable);

				int index = dice.Next (0, dimensionTable.DimensionCount);

				dimensionTable.RemoveDimensionAt (index);
				expected.RemoveAt (index);

				CollectionAssert.AreEqual (expected, dimensionTable.Dimensions.ToList ());
				this.CheckDimensionTableIsEmpty (dimensionTable);
			}
		}


		[TestMethod]
		public void SwapDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void SwapDimensionTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			List<AbstractDimension> expectedDimensions = dimensions.ToList ();

			System.Random dice = new System.Random ();

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			this.PopulateDimensionTable (dimensionTable);

			var expectedValues = dimensionTable.Keys.ToDictionary (k => k, k => dimensionTable[k], new ArrayEqualityComparer<string> ());

			for (int i = 0; i < 10; i++)
			{
				int index1 = dice.Next (0, dimensions.Count);
				int index2 = dice.Next (0, dimensions.Count);

				AbstractDimension value1 = dimensions[index1];
				AbstractDimension value2 = dimensions[index2];

				int indexA = dimensionTable.GetIndexOfDimension (value1);
				int indexB = dimensionTable.GetIndexOfDimension (value2);
				
				expectedDimensions[indexA] = value2;
				expectedDimensions[indexB] = value1;

				dimensionTable.SwapDimensions (value1, value2);

				expectedValues = expectedValues.ToDictionary (
					kvp =>
					{
						string[] key = kvp.Key.ToArray ();

						key[indexA] = kvp.Key[indexB];
						key[indexB] = kvp.Key[indexA];

						return key;
					},
					kvp => kvp.Value,
					new ArrayEqualityComparer<string> ()
				);

				CollectionAssert.AreEqual (expectedDimensions, dimensionTable.Dimensions.ToList ());
				Assert.AreEqual (expectedValues.Count, dimensionTable.Keys.Count ());
				foreach (string[] key in dimensionTable.Keys)
				{
					Assert.AreEqual (expectedValues[key], dimensionTable[key]);
				}
			}
		}


		[TestMethod]
		public void SwapDimensionAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void SwapDimensionAtTest()
		{
			List<AbstractDimension> dimensions = new List<AbstractDimension> ()
			{
			    new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("2", "d2", new string[] { "0", "1", "2" }),
			    new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
			    new CodeDimension ("4", "d4", new string[] { "0", "1", "2" }),
			};

			List<AbstractDimension> expectedDimensions = dimensions.ToList ();

			System.Random dice = new System.Random ();

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			this.PopulateDimensionTable (dimensionTable);

			var expectedValues = dimensionTable.Keys.ToDictionary (k => k, k => dimensionTable[k], new ArrayEqualityComparer<string> ());

			for (int i = 0; i < 10; i++)
			{
				int index1 = dice.Next (0, dimensions.Count);
				int index2 = dice.Next (0, dimensions.Count);

				AbstractDimension value1 = expectedDimensions[index1];
				AbstractDimension value2 = expectedDimensions[index2];

				expectedDimensions[index1] = value2;
				expectedDimensions[index2] = value1;

				dimensionTable.SwapDimensionsAt (index1, index2);

				expectedValues = expectedValues.ToDictionary (
					kvp =>
					{
						string[] key = kvp.Key.ToArray ();

						key[index1] = kvp.Key[index2];
						key[index2] = kvp.Key[index1];

						return key;
					},
					kvp => kvp.Value,
					new ArrayEqualityComparer<string> ()
				);

				CollectionAssert.AreEqual (expectedDimensions, dimensionTable.Dimensions.ToList ());
				Assert.AreEqual (expectedValues.Count, dimensionTable.Keys.Count ());
				foreach (string[] key in dimensionTable.Keys)
				{
					Assert.AreEqual (expectedValues[key], dimensionTable[key]);
				}
			}
		}


		[TestMethod]
		public void ContainsDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void ContainsDimensionTest()
		{
			List<NumericDimension> dimensions1 = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			List<NumericDimension> dimensions2 = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions1.ToArray ());

			foreach (AbstractDimension dimension in dimensions1)
			{
				Assert.IsTrue (dimensionTable.ContainsDimension (dimension));
			}

			foreach (AbstractDimension dimension in dimensions2)
			{
				Assert.IsFalse (dimensionTable.ContainsDimension (dimension));
			}
		}


		[TestMethod]
		public void GetIndexOfDimensionArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetIndexOfDimensionTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());

			foreach (AbstractDimension dimension in dimensions)
			{
				Assert.AreEqual (dimensions.IndexOf (dimension), dimensionTable.GetIndexOfDimension (dimension));
			}
		}


		[TestMethod]
		public void GetDimensionAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetDimensionAtTest()
		{
			List<NumericDimension> dimensions = new List<NumericDimension> ()
            {
                new NumericDimension ("1", "d1", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
                new NumericDimension ("3", "d3", RoundingMode.Up, new decimal[] { 0, 1, 2 }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());

			for (int i = 0; i < dimensions.Count; i++)
			{
				Assert.AreEqual (dimensions[i], dimensionTable.GetDimensionAt (i));
			}
		}


		[TestMethod]
		public void AddValueToDimensionTest()
		{
			List<CodeDimension> dimensions = new List<CodeDimension> ()
            {
                new CodeDimension ("1", "d1", new string[] { "1", "2", "3" }),
                new CodeDimension ("2", "d2", new string[] { "1", "2", "3" }),
                new CodeDimension ("3", "d3", new string[] { "1", "2", "3" }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			this.PopulateDimensionTable (dimensionTable);

			System.Random dice = new System.Random ();

			for (int i = 0; i < 10; i++)
			{
				var dump = dimensionTable.Keys.ToDictionary (k => k, k => dimensionTable[k]);

				int indexDimension = dice.Next (0, dimensions.Count);
				CodeDimension dimension = (CodeDimension) dimensions[indexDimension];

				int indexValue = dice.Next (0, dimension.Count + 1);
				string value = InvariantConverter.ConvertToString (dimension.Count + 1);

				dimension.Insert (indexValue, value);

				foreach (var item in dump)
				{
					Assert.AreEqual (item.Value, dimensionTable[item.Key]);
				}

				foreach (var key in dimensionTable.Keys.Where (k => k[indexDimension] == value))
				{
					Assert.IsNull (dimensionTable[key]);
					Assert.IsFalse (dimensionTable.IsValueDefined (key));
				}
			}
		}


		[TestMethod]
		public void RemoveValueFromDimensionTest()
		{
			List<CodeDimension> dimensions = new List<CodeDimension> ()
            {
                new CodeDimension ("1", "d1", new string[] { "1", "2", "3" }),
                new CodeDimension ("2", "d2", new string[] { "1", "2", "3" }),
                new CodeDimension ("3", "d3", new string[] { "1", "2", "3" }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			this.PopulateDimensionTable (dimensionTable);

			System.Random dice = new System.Random ();

			for (int i = 0; i < 10; i++)
			{
				var dump = dimensionTable.Keys.ToDictionary (k => k, k => dimensionTable[k]);

				int indexDimension = dice.Next (0, dimensions.Count);
				CodeDimension dimension = (CodeDimension) dimensions[indexDimension];

				if (dimension.Count > 0)
				{
					int indexValue = dice.Next (0, dimension.Count);
					string value = dimension.GetValueAt (indexValue);

					dimension.RemoveAt (indexValue);

					foreach (var item in dump)
					{
						if (item.Key[indexDimension] == value)
						{
							Assert.IsFalse (dimensionTable.IsKeyValid (item.Key));
						}
						else
						{
							Assert.AreEqual (item.Value, dimensionTable[item.Key]);
						}
					}

					Assert.IsFalse (dimensionTable.Keys.Any (k => k[indexDimension] == value));
				}
			}
		}


		[TestMethod]
		public void SwapValuesInDimensionTest()
		{
			List<CodeDimension> dimensions = new List<CodeDimension> ()
            {
                new CodeDimension ("1", "d1", new string[] { "1", "2", "3" }),
                new CodeDimension ("2", "d2", new string[] { "1", "2", "3" }),
                new CodeDimension ("3", "d3", new string[] { "1", "2", "3" }),
            };

			DimensionTable dimensionTable = new DimensionTable (dimensions.ToArray ());
			this.PopulateDimensionTable (dimensionTable);

			var dump = dimensionTable.Keys.ToDictionary (k => k, k => dimensionTable[k]);

			System.Random dice = new System.Random ();

			for (int i = 0; i < 10; i++)
			{
				CodeDimension dimension = (CodeDimension) dimensions[dice.Next (0, dimensions.Count)];
				
				int index1 = dice.Next (0, dimension.Count);
				int index2 = dice.Next (0, dimension.Count);

				dimension.SwapAt (index1, index2);

				Assert.AreEqual (dump.Count, dimensionTable.Keys.Count ());
				foreach (var item in dump)
				{
					Assert.AreEqual (item.Value, dimensionTable[item.Key]);
				}
			}
		}


		[TestMethod]
		public void IsValueDefinedArgumentCheck()
		{
			Assert.Fail ();
			
			//NumericDimension d1 = new NumericDimension ("1", "d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			//NumericDimension d2 = new NumericDimension ("2", "d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			//DimensionTable table = new DimensionTable (d1, d2);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () => table.IsValueDefined (null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => table.IsValueDefined (1m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => table.IsValueDefined (1m, 0m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => table.IsValueDefined (1m, null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => table.IsValueDefined (1m, 1.5m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => table.IsValueDefined (1m, 1f)
			//);
		}


		[TestMethod]
		public void IsValueDefinedTest()
		{
			NumericDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			NumericDimension d2 = new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 1, 2, 3 });

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


		[TestMethod]
		public void IsKeyRoundableArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void IsKeyRoundableTest()
		{
			NumericDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			NumericDimension d2 = new NumericDimension ("2", "d2", RoundingMode.None, new decimal[] { 1, 2, 3 });

			DimensionTable dimensionTable = new DimensionTable (d1, d2);

			foreach (string[] key in dimensionTable.Keys)
			{
				Assert.IsTrue (dimensionTable.IsKeyRoundable (key));
			}

			for (decimal i = 1.5m; i < 3; i++)
			{
				foreach (decimal j in d2.DecimalValues)
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.IsTrue (dimensionTable.IsKeyRoundable (si, sj));
				}

				foreach (decimal j in new List<decimal> () { 0m, 0.99999m, 1.00001m, 3.00001m, 4m, })
				{
					string si = InvariantConverter.ConvertToString (i);
					string sj = InvariantConverter.ConvertToString (j);

					Assert.IsFalse (dimensionTable.IsKeyRoundable (si, sj));
				}
			}
		}


		[TestMethod]
		public void GetRoundedKeyArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetRoundedKeyRoundableTest()
		{
			AbstractDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			AbstractDimension d2 = new NumericDimension ("2", "d2", RoundingMode.None, new decimal[] { 1, 2, 3 });
			AbstractDimension d3 = new CodeDimension ("3", "d3", new string[] { "1", "2", "3" });

			DimensionTable dimensionTable = new DimensionTable (d1, d2, d3);

			Dictionary<string[], string[]> data = new Dictionary<string[], string[]> ()
			{
				{ new string[] { "1", "2", "3", }, new string[] { "1", "2", "3", } },
				{ new string[] { "1.5", "1", "3", }, new string[] { "1", "1", "3", } },
				{ new string[] { "1.999", "3", "2", }, new string[] { "1", "3", "2", } },
				{ new string[] { "2.111", "2", "1", }, new string[] { "2", "2", "1", } },
			};

			ArrayEqualityComparer<string> aec = new ArrayEqualityComparer<string> ();

			foreach (var item in data)
			{
				Assert.IsTrue (aec.Equals (item.Value, dimensionTable.GetRoundedKey (item.Key)));
			}
		}


		[TestMethod]
		public void GetKeyFromIndexesArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetKeyFromIndexesTest()
		{
			AbstractDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			AbstractDimension d2 = new NumericDimension ("2", "d2", RoundingMode.None, new decimal[] { 1, 2, 3 });
			AbstractDimension d3 = new CodeDimension ("3", "d3", new string[] { "1", "2", "3" });

			DimensionTable dimensionTable = new DimensionTable (d1, d2, d3);

			Dictionary<int[], string[]> data = new Dictionary<int[], string[]> ()
			{
				{ new int[] { 0, 1, 2, }, new string[] { "1", "2", "3", } },
				{ new int[] { 0, 0, 2, }, new string[] { "1", "1", "3", } },
				{ new int[] { 0, 2, 1, }, new string[] { "1", "3", "2", } },
				{ new int[] { 1, 1, 0, }, new string[] { "2", "2", "1", } },
			};

			ArrayEqualityComparer<string> aec = new ArrayEqualityComparer<string> ();

			foreach (var item in data)
			{
				Assert.IsTrue (aec.Equals (item.Value, dimensionTable.GetKeyFromIndexes (item.Key)));
			}
		}


		[TestMethod]
		public void GetIndexesFromKeyArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetIndexesFromKeyTest()
		{
			AbstractDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			AbstractDimension d2 = new NumericDimension ("2", "d2", RoundingMode.None, new decimal[] { 1, 2, 3 });
			AbstractDimension d3 = new CodeDimension ("3", "d3", new string[] { "1", "2", "3" });

			DimensionTable dimensionTable = new DimensionTable (d1, d2, d3);

			Dictionary<string[], int[]> data = new Dictionary<string[], int[]> ()
			{
				{ new string[] { "1", "2", "3", }, new int[] { 0, 1, 2, } },
				{ new string[] { "1", "1", "3", }, new int[] { 0, 0, 2, } },
				{ new string[] { "1", "3", "2", }, new int[] { 0, 2, 1, } },
				{ new string[] { "2", "2", "1", }, new int[] { 1, 1, 0, } },
			};

			ArrayEqualityComparer<int> aec = new ArrayEqualityComparer<int> ();

			foreach (var item in data)
			{
				Assert.IsTrue (aec.Equals (item.Value, dimensionTable.GetIndexesFromKey (item.Key)));
			}
		}


		[TestMethod]
		public void GetValueArgumentCheck()
		{
			Assert.Fail ();
			
			//NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			//NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			//DimensionTable table = new DimensionTable (d1, d2);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () =>
			//    {
			//        var v = table[null];
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        var v = table[1m];
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        var v = table[1m, null];
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        var v = table[1m, 0m];
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        var v = table[1m, 1f];
			//    }
			//);
		}


		[TestMethod]
		public void SetValueArgumentCheck()
		{
			Assert.Inconclusive ();
			
			//NumericDimension d1 = new NumericDimension ("d1", new decimal[] { 1, 2, 3 }, RoundingMode.Down);
			//NumericDimension d2 = new NumericDimension ("d2", new decimal[] { 1, 2, 3 }, RoundingMode.Up);

			//DimensionTable table = new DimensionTable (d1, d2);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () =>
			//    {
			//        table[null] = 0;
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        table[1m] = 0;
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        table[1m, null] = 0;
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        table[1m, 0m] = 0;
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        table[1m, 1f] = 0;
			//    }
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () =>
			//    {
			//        table[1m, 11.5m] = 0;
			//    }
			//);
		}


		[TestMethod]
		public void GetAndSetValueTest()
		{
			NumericDimension d1 = new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 1, 2, 3 });
			NumericDimension d2 = new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 1, 2, 3 });

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
                new NumericDimension ("1", "d1", RoundingMode.Down, new decimal[] { 0, 1, 2, 3 }),
                new NumericDimension ("2", "d2", RoundingMode.Up, new decimal[] { 0, 1, 2, 3 }),
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

			var possibleKeys1 = table1.Keys.ToList ();
			var possibleKeys2 = table1.Keys.ToList ();

			Assert.AreEqual (possibleKeys1.Count, possibleKeys2.Count);

			for (int i = 0; i < possibleKeys1.Count; i++)
			{
				Assert.IsTrue (new ArrayEqualityComparer<string> ().Equals (possibleKeys1[i], possibleKeys2[i]));
			}

			foreach (string[] key in possibleKeys1)
			{
				Assert.AreEqual (table1[key], table2[key]);
				Assert.AreEqual (table1.IsValueDefined (key), table2.IsValueDefined (key));
			}

			Assert.AreEqual (xDimensionTable1.ToString (), xDimensionTable2.ToString ());
		}


		private void PopulateDimensionTable(DimensionTable dimensionTable)
		{
			System.Random dice = new System.Random();

			foreach (var key in dimensionTable.Keys)
			{
				dimensionTable[key] = System.Convert.ToDecimal (dice.NextDouble ());
			}
		}


		private void CheckDimensionTableIsEmpty(DimensionTable dimensionTable)
		{
			foreach (var key in dimensionTable.Keys)
			{
				Assert.IsNull (dimensionTable[key]);
			}
		}


	}


}
