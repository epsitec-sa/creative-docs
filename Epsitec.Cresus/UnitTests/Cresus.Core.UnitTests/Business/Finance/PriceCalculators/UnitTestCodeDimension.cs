using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	[TestClass]
	public sealed class UnitTestCodeDimension
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			string name = "name";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (null, false, values)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension ("", false, values)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (name, false, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (name, false, new List<string> ())
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (name, false, new List<string> () { null })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (name, false, new List<string> () { "" })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CodeDimension (name, false, new List<string> () { "." })
			);
		}


		[TestMethod]
		public void NameTest()
		{
			List<string> names = new List<string> () {  "name",  "coucou", "blabla",  };

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			foreach (string name in names)
			{
				CodeDimension dimension = new CodeDimension (name, false, values);

				Assert.AreEqual (name, dimension.Name);
			}
		}


		[TestMethod]
		public void IsNullableTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			Assert.AreEqual (false, new CodeDimension ("name", false, values).IsNullable);
			Assert.AreEqual (true, new CodeDimension ("name", true, values).IsNullable);
		}


		[TestMethod]
		public void ValuesTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", }.OrderBy (v => v).ToList ();

			for (int i = 0; i < 10; i++)
			{
				CodeDimension dimension = new CodeDimension ("name", false, values.Shuffle ());

				CollectionAssert.AreEqual (values, dimension.Values.Cast<string> ().ToList ());
			}

			for (int i = 0; i < 10; i++)
			{
				CodeDimension dimension = new CodeDimension ("name", true, values.Shuffle ());

				CollectionAssert.AreEqual (values.Append (CodeDimension.NullValue).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void IsValueDefinedArgumentCheck()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("name", false, values);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension.IsValueDefined (null)
			);
		}


		[TestMethod]
		public void IsValueDefinedTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("name", false, values1);

			foreach (string value in values1)
			{
				Assert.IsTrue (dimension.IsValueDefined (value));
			}

			foreach (string value in values2)
			{
				Assert.IsFalse (dimension.IsValueDefined (value));
			}

			Assert.IsFalse (dimension.IsValueDefined (CodeDimension.NullValue));
			Assert.IsTrue (new CodeDimension ("name", true, values1).IsValueDefined (CodeDimension.NullValue));
		}


		[TestMethod]
		public void IsNearestValueDefinedArgumentCheck()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("name", false, values);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension.IsNearestValueDefined (null)
			);
		}


		[TestMethod]
		public void IsNearestValueDefinedTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("name", false, values1);

			foreach (string value in values1)
			{
				Assert.IsTrue (dimension.IsNearestValueDefined (value));
			}

			foreach (string value in values2)
			{
				Assert.IsFalse (dimension.IsNearestValueDefined (value));
			}

			Assert.IsFalse (dimension.IsNearestValueDefined (CodeDimension.NullValue));
			Assert.IsTrue (new CodeDimension ("name", true, values1).IsNearestValueDefined (CodeDimension.NullValue));
		}


		[TestMethod]
		public void GetNearestValueArgumentCheck()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("name", false, values1);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => dimension.GetNearestValue (null)
			);

			foreach (string value in values2)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.GetNearestValue (value)
				);
			}

			ExceptionAssert.Throw<System.ArgumentException>
			(
			   () => dimension.GetNearestValue (CodeDimension.NullValue)
			);
		}


		[TestMethod]
		public void GetNearestValueTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("name", false, values);

			foreach (string value in values)
			{
				Assert.AreEqual (value, dimension.GetNearestValue (value));
			}

			Assert.AreEqual (CodeDimension.NullValue, new CodeDimension ("name", true, values).GetNearestValue (CodeDimension.NullValue));
		}


		[TestMethod]
		public void BuildCodeDimensionArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => CodeDimension.BuildCodeDimension (null, "a;b")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
			  () => CodeDimension.BuildCodeDimension ("", "a;b")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => CodeDimension.BuildCodeDimension ("name", null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => CodeDimension.BuildCodeDimension ("name", "")
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => CodeDimension.BuildCodeDimension ("name", "True;")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => CodeDimension.BuildCodeDimension ("name", "True;fdafda&;fdsafdas")
			);
		}


		[TestMethod]
		public void GetStringDataAndBuildCodeDimensionTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension1 = new CodeDimension ("name", false, values);
			CodeDimension dimension2 = CodeDimension.BuildCodeDimension (dimension1.Name, dimension1.GetStringData ());

			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


		[TestMethod]
		public void XmlImportExportTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension1 = new CodeDimension ("name", false, values);
			CodeDimension dimension2 = (CodeDimension) AbstractDimension.XmlImport (dimension1.XmlExport ());

			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


	}


}
