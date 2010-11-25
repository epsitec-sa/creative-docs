using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestNumericDimension
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			string name = "name";

			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			RoundingMode mode = RoundingMode.None;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new NumericDimension (null, values, mode)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new NumericDimension ("", values, mode)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new NumericDimension (name, null, mode)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new NumericDimension (name, new List<decimal> (), mode)
			);
		}


		[TestMethod]
		public void NameTest()
		{
			List<string> names = new List<string> () { "name", "coucou", "blabla", };

			List<decimal> values =  Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			foreach (string name in names)
			{
				NumericDimension dimension = new NumericDimension (name, values, RoundingMode.None);

				Assert.AreEqual (name, dimension.Name);
			}
		}


		[TestMethod]
		public void ValuesTest()
		{
			List<decimal> values =  Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ().OrderBy (v => v).ToList ();

			for (int i = 0; i < 10; i++)
			{
				NumericDimension dimension = new NumericDimension ("name", values.Shuffle (), RoundingMode.None);

				CollectionAssert.AreEqual (values, dimension.Values.Cast<decimal> ().ToList ());
			}
		}


		[TestMethod]
		public void RoundingModeTest()
		{
			List<decimal> values =  Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			List<RoundingMode> roundingModes = new List<RoundingMode> () { RoundingMode.Down, RoundingMode.Nearest, RoundingMode.Up, };

			foreach (RoundingMode roundingMode in roundingModes)
			{
				NumericDimension dimension = new NumericDimension ("name", values, roundingMode);

				Assert.AreEqual (roundingMode, dimension.RoundingMode);
			}
		}


		[TestMethod]
		public void IsValueDefinedArgumentCheck()
		{
			List<decimal> values =  Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension = new NumericDimension ("name", values, RoundingMode.None);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension.IsValueDefined (null)
			);
		}


		[TestMethod]
		public void IsValueDefinedTest()
		{
			List<decimal> values1 = Enumerable.Range (0, 11).Where (v => v % 2 == 0).Select (v => System.Convert.ToDecimal (v)).ToList ();
			List<decimal> values2 = Enumerable.Range (0, 10).Where (v => v % 2 == 1).Select (v => System.Convert.ToDecimal (v)).ToList ();

			RoundingMode mode = RoundingMode.None;

			NumericDimension dimension = new NumericDimension ("name", values1, mode);

			foreach (decimal value in values1)
			{
				Assert.IsTrue (dimension.IsValueDefined (value));
			}

			foreach (decimal value in values2)
			{
				Assert.IsFalse (dimension.IsValueDefined (value));
			}
		}


		[TestMethod]
		public void IsNearestValueDefinedArgumentCheck()
		{
			List<decimal> values = new List<decimal> () { 10, 20, 30, };

			NumericDimension dimension = new NumericDimension ("name", values, RoundingMode.None);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension.IsNearestValueDefined (null)
			);
		}


		[TestMethod]
		public void IsNearestValueDefinedTest()
		{
			List<decimal> values1 = Enumerable.Range (0, 11).Where (v => v % 2 == 0).Select (v => System.Convert.ToDecimal (v)).ToList ();
			List<decimal> values2 = Enumerable.Range (0, 10).Where (v => v % 2 == 1).Select (v => System.Convert.ToDecimal (v)).ToList ();
			List<decimal> values3 = new List<decimal>() { -1, 11 };

			NumericDimension dimension1 = new NumericDimension ("name", values1,  RoundingMode.Up);

			foreach (decimal value in values1)
			{
				Assert.IsTrue (dimension1.IsNearestValueDefined (value));
			}

			foreach (decimal value in values2)
			{
				Assert.IsTrue (dimension1.IsNearestValueDefined (value));
			}

			foreach (decimal value in values3)
			{
				Assert.IsFalse (dimension1.IsNearestValueDefined (value));
			}

			NumericDimension dimension2 = new NumericDimension ("name", values1, RoundingMode.None);

			foreach (decimal value in values1)
			{
				Assert.IsTrue (dimension2.IsNearestValueDefined (value));
			}

			foreach (decimal value in values2)
			{
				Assert.IsFalse (dimension2.IsNearestValueDefined (value));
			}

			foreach (decimal value in values3)
			{
				Assert.IsFalse (dimension2.IsNearestValueDefined (value));
			}
		}


		[TestMethod]
		public void GetNearestValueArgumentCheck()
		{
			List<decimal> values = new List<decimal> ()  { 10, 20, };

			NumericDimension dimension1 = new NumericDimension ("name", values, RoundingMode.Down);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension1.GetNearestValue (null)
			);
			
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => dimension1.GetNearestValue (9.9999999m)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => dimension1.GetNearestValue (20.000001m)
			);
			NumericDimension dimension2 = new NumericDimension ("name", values, RoundingMode.None);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension2.GetNearestValue (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => dimension2.GetNearestValue (9.9999999m)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => dimension2.GetNearestValue (15m)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => dimension2.GetNearestValue (20.000001m)
			);
		}


		[TestMethod]
		public void GetNearestValueTest()
		{
			List<decimal> values = new List<decimal> ()  { 10, 20, };

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

			this.GetValueTest (values, testDataNone, RoundingMode.None);
			this.GetValueTest (values, testDataDown, RoundingMode.Down);
			this.GetValueTest (values, testDataNearest, RoundingMode.Nearest);
			this.GetValueTest (values, testDataUp, RoundingMode.Up);
		}


		private void GetValueTest(List<decimal> values, Dictionary<decimal, decimal> testData, RoundingMode roundingMode)
		{
			NumericDimension dimension = new NumericDimension ("name", values, roundingMode);

			foreach (var item in testData)
			{
				Assert.AreEqual (item.Value, dimension.GetNearestValue (item.Key));
			}
		}


		[TestMethod]
		public void BuildNumericDimensionArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => NumericDimension.BuildNumericDimension (null, "a;b")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
			  () => NumericDimension.BuildNumericDimension ("", "a;b")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => NumericDimension.BuildNumericDimension ("name", null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => NumericDimension.BuildNumericDimension ("name", "")
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => NumericDimension.BuildNumericDimension ("name", "gfsgsd:1")
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => NumericDimension.BuildNumericDimension ("name", "ggfdgfd;")
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => NumericDimension.BuildNumericDimension ("name", "Up;")
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => NumericDimension.BuildNumericDimension ("name", "Up;1;fdfds")
			);
		}


		[TestMethod]
		public void GetStringDataAndNumericNumericDimensionTest()
		{
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension1 = new NumericDimension ("name", values, RoundingMode.Nearest);
			NumericDimension dimension2 = NumericDimension.BuildNumericDimension (dimension1.Name, dimension1.GetStringData ());

			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
			Assert.AreEqual (dimension1.RoundingMode, dimension2.RoundingMode);
		}


		[TestMethod]
		public void XmlImportExportTest()
		{
			List<decimal> values = Enumerable.Range (0, 11).Select (v => System.Convert.ToDecimal (v)).ToList ();

			NumericDimension dimension1 = new NumericDimension ("name", values, RoundingMode.Nearest);
			NumericDimension dimension2 = (NumericDimension) AbstractDimension.XmlImport (dimension1.XmlExport ());

			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
			Assert.AreEqual (dimension1.RoundingMode, dimension2.RoundingMode);
		}


	}


}
