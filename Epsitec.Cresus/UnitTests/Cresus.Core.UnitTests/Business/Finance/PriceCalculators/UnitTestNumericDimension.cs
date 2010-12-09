using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.UnitTests.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestNumericDimension
	{


		[TestMethod]
		public void Constructor1ArgumentCheck()
		{
			Assert.Inconclusive ();

			//string name = "name";

			//List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			//RoundingMode mode = RoundingMode.None;

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => new NumericDimension (null, values, mode)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => new NumericDimension ("", values, mode)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => new NumericDimension (name, null, mode)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => new NumericDimension (name, new List<decimal> (), mode)
			//);
		}


		[TestMethod]
		public void Constructor2ArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void Constructor1Test()
		{
			string code = "code";
			string name = "name";
			RoundingMode roundingMode = RoundingMode.Nearest;

			NumericDimension dimension = new NumericDimension (code, name, roundingMode);

			Assert.AreEqual (code, dimension.Code);
			Assert.AreEqual (name, dimension.Name);
			Assert.AreEqual (roundingMode, dimension.RoundingMode);
		}


		[TestMethod]
		public void Constructor2Test()
		{
			string code = "code";
			string name = "name";
			RoundingMode roundingMode = RoundingMode.Nearest;
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension = new NumericDimension (code, name, roundingMode, values);

			Assert.AreEqual (code, dimension.Code);
			Assert.AreEqual (name, dimension.Name);
			Assert.AreEqual (roundingMode, dimension.RoundingMode);
			CollectionAssert.AreEqual (values, dimension.DecimalValues.ToList ());
		}


		[TestMethod]
		public void CodeTest()
		{
			string code = "code";
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension = new NumericDimension (code, "name", RoundingMode.None, values);
			Assert.AreEqual (code, dimension.Code);

			code = "newCode";
			dimension.Code = code;
			Assert.AreEqual (code, dimension.Code);
		}


		[TestMethod]
		public void NameTest()
		{
			string name = "name";

			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension = new NumericDimension ("code", name, RoundingMode.None, values);
			Assert.AreEqual (name, dimension.Name);

			name = "newName";
			dimension.Name = name;
			Assert.AreEqual (name, dimension.Name);
		}


		[TestMethod]
		public void RoundingModeTest()
		{
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			List<RoundingMode> roundingModes = new List<RoundingMode> () { RoundingMode.Down, RoundingMode.Nearest, RoundingMode.Up, RoundingMode.None, };

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.Up, values);

			Assert.AreEqual (RoundingMode.Up, dimension.RoundingMode);

			foreach (RoundingMode roundingMode in roundingModes)
			{
				dimension.RoundingMode = roundingMode;

				Assert.AreEqual (roundingMode, dimension.RoundingMode);
			}
		}


		[TestMethod]
		public void ValuesAndDecimalValuesTest()
		{
			int initialLength = 5;
			int totalLentgh = 10;

			List<decimal> values = Enumerable.Range (0, totalLentgh).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values.Take (initialLength));

			List<decimal> expected = values.Take (initialLength).OrderBy (v => v).ToList ();

			CollectionAssert.AreEqual (expected, dimension.DecimalValues.ToList ());
			CollectionAssert.AreEqual (expected, dimension.Values.Select (v => InvariantConverter.ConvertFromString<decimal> (v)).ToList ());

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimension.AddDecimal (values[i]);

				expected = values.Take (i + 1).OrderBy (v => v).ToList ();

				CollectionAssert.AreEqual (expected, dimension.DecimalValues.ToList ());
				CollectionAssert.AreEqual (expected, dimension.Values.Select (v => InvariantConverter.ConvertFromString<decimal> (v)).ToList ());
			}

			for (int i = 0; i < initialLength; i++)
			{
				dimension.RemoveDecimal (values[i]);

				expected = values.Skip (i + 1).OrderBy (v => v).ToList ();

				CollectionAssert.AreEqual (expected, dimension.DecimalValues.ToList ());
				CollectionAssert.AreEqual (expected, dimension.Values.Select (v => InvariantConverter.ConvertFromString<decimal> (v)).ToList ());
			}
		}


		[TestMethod]
		public void CountTest()
		{
			int initialLength = 5;
			int totalLentgh = 10;

			List<decimal> values = Enumerable.Range (0, totalLentgh).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values.Take (initialLength));

			Assert.AreEqual (initialLength, dimension.Count);

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimension.AddDecimal (values[i]);

				Assert.AreEqual (i + 1, dimension.Count);
			}

			Assert.AreEqual (totalLentgh, dimension.Count);

			for (int i = 0; i < initialLength; i++)
			{
				dimension.RemoveDecimal (values[i]);

				Assert.AreEqual (totalLentgh - i - 1, dimension.Count);
			}
		}


		[TestMethod]
		public void AddArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void AddTest()
		{
			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None);

			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			Assert.IsFalse (dimension.Values.Any ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.Add (InvariantConverter.ConvertToString (values[i]));

				CollectionAssert.AreEqual (values.Take (i + 1).OrderBy (v => v).Select (v => InvariantConverter.ConvertToString (v)).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void AddDecimalArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void AddDecimalTest()
		{
			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None);

			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			Assert.IsFalse (dimension.Values.Any ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.AddDecimal (values[i]);

				CollectionAssert.AreEqual (values.Take (i + 1).OrderBy (v => v).ToList (), dimension.DecimalValues.ToList ());
			}
		}


		[TestMethod]
		public void RemoveArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void RemoveTest()
		{
			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();
			
			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values);

			CollectionAssert.AreEqual (values.OrderBy (v => v).Select (v => InvariantConverter.ConvertToString (v)).ToList (), dimension.Values.ToList ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.Remove (InvariantConverter.ConvertToString (values[i]));

				CollectionAssert.AreEqual (values.Skip (i + 1).OrderBy (v => v).Select (v => InvariantConverter.ConvertToString (v)).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void RemoveDecimalArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void RemoveDecimalTest()
		{
			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values);

			CollectionAssert.AreEqual (values.OrderBy (v => v).ToList (), dimension.DecimalValues.ToList ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.RemoveDecimal (values[i]);

				CollectionAssert.AreEqual (values.Skip (i + 1).OrderBy (v => v).ToList (), dimension.DecimalValues.ToList ());
			}
		}


		[TestMethod]
		public void ContainsArgumentCheck()
		{
			Assert.Inconclusive ();
			
			//List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			//NumericDimension dimension = new NumericDimension ("name", values, RoundingMode.None);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//  () => dimension.IsValueDefined (null)
			//);
		}


		[TestMethod]
		public void ContainsDecimalArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void ContainsAndContainsDecimalTest()
		{
			List<decimal> values1 = Enumerable.Range (0, 11).Where (v => v % 2 == 0).Select (v => System.Convert.ToDecimal (v)).ToList ();
			List<decimal> values2 = Enumerable.Range (0, 10).Where (v => v % 2 == 1).Select (v => System.Convert.ToDecimal (v)).ToList ();

			RoundingMode mode = RoundingMode.None;

			NumericDimension dimension = new NumericDimension ("code", "name", mode, values1);

			foreach (decimal value in values1)
			{
				Assert.IsTrue (dimension.Contains (InvariantConverter.ConvertToString (value)));
				Assert.IsTrue (dimension.ContainsDecimal (value));
			}

			foreach (decimal value in values2)
			{
				Assert.IsFalse (dimension.Contains (InvariantConverter.ConvertToString (value)));
				Assert.IsFalse (dimension.ContainsDecimal (value));
			}
		}


		[TestMethod]
		public void IsValueRoundableArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void IsDecimalValueRoundableArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void IsValueRoundableAndIsDecimalValueRoundableTest()
		{
			List<decimal> values = new List<decimal> () { 1, 2, 3, 4, 5 };

			var expectedResultsNone = new Dictionary<decimal, bool> ()
			{
				{0, false},
				{0.99999m, false},
				{1, true},
				{1.00001m, false},
				{3, true},
				{4.99999m, false},
				{5, true},
				{500001m, false},
				{6, false},
			};

			var expectedResultsDown = new Dictionary<decimal, bool> ()
			{
				{0, false},
				{0.99999m, false},
				{1, true},
				{1.00001m, true},
				{3, true},
				{4.99999m, true},
				{5, true},
				{500001m, false},
				{6, false},
			};

			var expectedResultsNearest = new Dictionary<decimal, bool> ()
			{
				{0, false},
				{0.99999m, false},
				{1, true},
				{1.00001m, true},
				{3, true},
				{4.99999m, true},
				{5, true},
				{500001m, false},
				{6, false},
			};

			var expectedResultsUp = new Dictionary<decimal, bool> ()
			{
				{0, false},
				{0.99999m, false},
				{1, true},
				{1.00001m, true},
				{3, true},
				{4.99999m, true},
				{5, true},
				{500001m, false},
				{6, false},
			};

			this.IsValueRoundableAndIsDecimalValueRoundableTestHelper (RoundingMode.None, values, expectedResultsNone);
			this.IsValueRoundableAndIsDecimalValueRoundableTestHelper (RoundingMode.Down, values, expectedResultsDown);
			this.IsValueRoundableAndIsDecimalValueRoundableTestHelper (RoundingMode.Nearest, values, expectedResultsNearest);
			this.IsValueRoundableAndIsDecimalValueRoundableTestHelper (RoundingMode.Up, values, expectedResultsUp);
		}


		private void IsValueRoundableAndIsDecimalValueRoundableTestHelper(RoundingMode roundingMode, List<decimal> values, Dictionary<decimal, bool> expectedResults)
		{
			NumericDimension dimension = new NumericDimension ("code", "name", roundingMode, values);

			foreach (var item in expectedResults)
			{
				Assert.AreEqual (item.Value, dimension.IsDecimalValueRoundable (item.Key));
				Assert.AreEqual (item.Value, dimension.IsValueRoundable (InvariantConverter.ConvertToString<decimal> (item.Key)));
			}
		}


		[TestMethod]
		public void GetRoundedValueArgumentCheck()
		{
			Assert.Inconclusive ();

			//List<decimal> values = new List<decimal> () { 10, 20, };

			//NumericDimension dimension1 = new NumericDimension ("name", values, RoundingMode.Down);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//  () => dimension1.GetNearestValue (null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => dimension1.GetNearestValue (9.9999999m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => dimension1.GetNearestValue (20.000001m)
			//);
			//NumericDimension dimension2 = new NumericDimension ("name", values, RoundingMode.None);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//  () => dimension2.GetNearestValue (null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => dimension2.GetNearestValue (9.9999999m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => dimension2.GetNearestValue (15m)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => dimension2.GetNearestValue (20.000001m)
			//);
		}


		[TestMethod]
		public void GetRoundedDecimalValueArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetRoundedValueAndGetRoundedDecimalValueTest()
		{
			List<decimal> values = new List<decimal> () { 10, 20, };

			Dictionary<decimal, decimal> testDataNone = new Dictionary<decimal, decimal> ()
      {
        {10, 10},
        {20, 20},
      };

			Dictionary<decimal, decimal> testDataDown = new Dictionary<decimal, decimal> ()
      {
        {10, 10},
        {10.0000001m, 10},
        {15, 10},
        {19.9999999m, 10},
        {20, 20},
      };

			Dictionary<decimal, decimal> testDataNearest = new Dictionary<decimal, decimal> ()
      {
        {10, 10},
        {10.0000001m, 10},
        {15, 20},
        {19.9999999m, 20},
        {20, 20},
      };

			Dictionary<decimal, decimal> testDataUp = new Dictionary<decimal, decimal> ()
      {
        {10, 10},
        {10.0000001m, 20},
        {15, 20},
        {19.9999999m, 20},
        {20, 20},
      };

			this.GetRoundedValueAndGetRoundedDecimalValueTestHelper (values, testDataNone, RoundingMode.None);
			this.GetRoundedValueAndGetRoundedDecimalValueTestHelper (values, testDataDown, RoundingMode.Down);
			this.GetRoundedValueAndGetRoundedDecimalValueTestHelper (values, testDataNearest, RoundingMode.Nearest);
			this.GetRoundedValueAndGetRoundedDecimalValueTestHelper (values, testDataUp, RoundingMode.Up);
		}


		private void GetRoundedValueAndGetRoundedDecimalValueTestHelper(List<decimal> values, Dictionary<decimal, decimal> testData, RoundingMode roundingMode)
		{
			NumericDimension dimension = new NumericDimension ("code", "name", roundingMode, values);

			foreach (var item in testData)
			{
				Assert.AreEqual (item.Value, dimension.GetRoundedDecimalValue (item.Key));
				Assert.AreEqual (InvariantConverter.ConvertToString (item.Value), dimension.GetRoundedValue (InvariantConverter.ConvertToString (item.Key)));
			}
		}


		[TestMethod]
		public void GetIndexOfArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetIndexOfDecimalArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetIndexOfAndGetIndexOfDecimalTest()
		{
			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values);

			foreach (decimal value in values)
			{
				Assert.AreEqual ((int) value, dimension.GetIndexOf (InvariantConverter.ConvertToString (value)));
				Assert.AreEqual ((int) value, dimension.GetIndexOfDecimal (value));
			}
		}


		[TestMethod]
		public void GetValueAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetDecimalValueAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetValueAtAndGetDecimalValueAtTest()
		{
			List<decimal> values = Enumerable.Range (0, 10).Select (v => System.Convert.ToDecimal (v)).Shuffle ().ToList ();

			NumericDimension dimension = new NumericDimension ("code", "name", RoundingMode.None, values);

			foreach (decimal value in values)
			{
				Assert.AreEqual (InvariantConverter.ConvertToString(value), dimension.GetValueAt((int) value));
				Assert.AreEqual (value, dimension.GetDecimalValueAt ((int) value));
			}
		}


		[TestMethod]
		public void BuildNumericDimensionArgumentCheck()
		{
			Assert.Inconclusive ();

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => NumericDimension.BuildNumericDimension (null, "a;b")
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			// () => NumericDimension.BuildNumericDimension ("", "a;b")
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", "")
			//);

			//ExceptionAssert.Throw<System.Exception>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", "gfsgsd:1")
			//);

			//ExceptionAssert.Throw<System.Exception>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", "ggfdgfd;")
			//);

			//ExceptionAssert.Throw<System.Exception>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", "Up;")
			//);

			//ExceptionAssert.Throw<System.Exception>
			//(
			//  () => NumericDimension.BuildNumericDimension ("name", "Up;1;fdfds")
			//);
		}


		[TestMethod]
		public void GetStringDataAndBuildNumericNumericDimensionTest()
		{
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension1 = new NumericDimension ("code", "name", RoundingMode.Nearest, values);
			NumericDimension dimension2 = NumericDimension.BuildNumericDimension (dimension1.Code, dimension1.Name, dimension1.GetStringData ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.DecimalValues.ToList (), dimension2.DecimalValues.ToList ());
			Assert.AreEqual (dimension1.RoundingMode, dimension2.RoundingMode);
		}


		[TestMethod]
		public void XmlImportArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void XmlImportExportTest()
		{
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension1 = new NumericDimension ("code", "name", RoundingMode.Nearest, values);
			NumericDimension dimension2 = (NumericDimension) AbstractDimension.XmlImport (dimension1.XmlExport ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.DecimalValues.ToList (), dimension2.DecimalValues.ToList ());
			Assert.AreEqual (dimension1.RoundingMode, dimension2.RoundingMode);
		}


	}


}
