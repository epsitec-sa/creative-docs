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
	public sealed class UnitTestCodeDimension
	{


		[TestMethod]
		public void Constructor1ArgumentCheck()
		{
			Assert.Inconclusive ();
			
			//string name = "name";

			//List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (null, values)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension ("", values)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (name, null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (name, new List<string> ())
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (name, new List<string> () { null })
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (name, new List<string> () { "" })
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => new CodeDimension (name, new List<string> () { "." })
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

			CodeDimension dimension = new CodeDimension (code, name);

			Assert.AreEqual (code, dimension.Code);
			Assert.AreEqual (name, dimension.Name);
		}


		[TestMethod]
		public void Constructor2Test()
		{
			string code = "code";
			string name = "name";
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, name, values);

			Assert.AreEqual (code, dimension.Code);
			Assert.AreEqual (name, dimension.Name);
			CollectionAssert.AreEqual (values, dimension.Values.ToList ());
		}


		[TestMethod]
		public void CodeTest()
		{
			string code = "code";
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, "name", values);
			Assert.AreEqual (code, dimension.Code);

			code = "newCode";
			dimension.Code = code;
			Assert.AreEqual (code, dimension.Code);
		}


		[TestMethod]
		public void NameTest()
		{
			string name = "name";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", name, values);
			Assert.AreEqual (name, dimension.Name);

			name = "newName";
			dimension.Name = name;
			Assert.AreEqual (name, dimension.Name);
		}


		[TestMethod]
		public void ValuesTest()
		{
			int initialLength = 3;
			int totalLentgh = 6;

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", "name", values.Take (initialLength));

			CollectionAssert.AreEqual (values.Take (initialLength).ToList (), dimension.Values.ToList ());

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimension.Add (values[i]);

				CollectionAssert.AreEqual (values.Take (i + 1).ToList (), dimension.Values.ToList ());
			}

			for (int i = 0; i < initialLength; i++)
			{
				dimension.Remove (values[i]);

				CollectionAssert.AreEqual (values.Skip (i + 1).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void CountTest()
		{
			int initialLength = 3;
			int totalLentgh = 6;

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", "name", values.Take (initialLength));

			Assert.AreEqual (initialLength, dimension.Count);

			for (int i = initialLength; i < totalLentgh; i++)
			{
				dimension.Add (values[i]);

				Assert.AreEqual (i + 1, dimension.Count);
			}

			Assert.AreEqual (totalLentgh, dimension.Count);

			for (int i = 0; i < initialLength; i++)
			{
				dimension.Remove (values[i]);

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
			CodeDimension dimension = new CodeDimension ("code", "name");

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();

			Assert.IsFalse (dimension.Values.Any ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.Add (values[i]);

				CollectionAssert.AreEqual (values.Take (i + 1).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void InsertArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void InsertTest()
		{
			CodeDimension dimension = new CodeDimension ("code", "name");

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = new List<string> ();

			System.Random dice = new System.Random ();

			Assert.IsFalse (dimension.Values.Any ());

			for (int i = 0; i < values.Count; i++)
			{
				int index = dice.Next (0, dimension.Count + 1);
				string value = values[i];

				dimension.Insert (index, value);
				expected.Insert (index, value);

				CollectionAssert.AreEqual (expected, dimension.Values.ToList ());
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
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			
			CodeDimension dimension = new CodeDimension ("code", "name", values);

			CollectionAssert.AreEqual (values, dimension.Values.ToList ());

			for (int i = 0; i < values.Count; i++)
			{
				dimension.Remove (values[i]);

				CollectionAssert.AreEqual (values.Skip (i + 1).ToList (), dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void RemoveAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void RemoveAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			System.Random dice = new System.Random ();

			CollectionAssert.AreEqual (values, dimension.Values.ToList ());

			for (int i = 0; i < values.Count; i++)
			{
				int index = dice.Next (0, dimension.Count);

				dimension.RemoveAt (index);
				expected.RemoveAt (index);

				CollectionAssert.AreEqual (expected, dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void SwapArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void SwapTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			CollectionAssert.AreEqual (values, dimension.Values.ToList ());

			System.Random dice = new System.Random ();

			for (int i = 0; i < 10; i++)
			{
				int index1 = dice.Next (0, dimension.Count);
				int index2 = dice.Next (0, dimension.Count);

				string value1 = values[index1];
				string value2 = values[index2];

				expected[dimension.GetIndexOf (value1)] = value2;
				expected[dimension.GetIndexOf (value2)] = value1;
				dimension.Swap (value1, value2);

				CollectionAssert.AreEqual (expected, dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void SwapAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void SwapAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			CollectionAssert.AreEqual (values, dimension.Values.ToList ());

			System.Random dice = new System.Random ();

			for (int i = 0; i < 10; i++)
			{
				int index1 = dice.Next (0, dimension.Count);
				int index2 = dice.Next (0, dimension.Count);

				string value1 = expected[index1];
				string value2 = expected[index2];

				expected[index1] = value2;
				expected[index2] = value1;
				dimension.SwapAt (index1, index2);

				CollectionAssert.AreEqual (expected, dimension.Values.ToList ());
			}
		}


		[TestMethod]
		public void ContainsArgumentCheck()
		{
			Assert.Inconclusive ();
			
			//List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			//CodeDimension dimension = new CodeDimension ("name", values);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () => dimension.IsValueDefined (null)
			//);
		}


		[TestMethod]
		public void ContainsTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", "name", values1);

			foreach (string value in values1)
			{
				Assert.IsTrue (dimension.Contains (value));
			}

			foreach (string value in values2)
			{
				Assert.IsFalse (dimension.Contains (value));
			}
		}


		[TestMethod]
		public void IsValueRoundableArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void IsValueRoundableTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", "name", values1);

			foreach (string value in values1)
			{
				Assert.IsTrue (dimension.IsValueRoundable (value));
			}

			foreach (string value in values2)
			{
				Assert.IsFalse (dimension.IsValueRoundable (value));
			}
		}


		[TestMethod]
		public void GetRoundedValueArgumentCheck()
		{
			Assert.Inconclusive ();

			//List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			//List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			//CodeDimension dimension = new CodeDimension ("code", "name", values1);

			//ExceptionAssert.Throw<System.ArgumentNullException>
			//(
			//    () => dimension.GetRoundedValue (null)
			//);

			//foreach (string value in values2)
			//{
			//    ExceptionAssert.Throw<System.ArgumentException>
			//    (
			//        () => dimension.GetRoundedValue (value)
			//    );
			//}
		}


		[TestMethod]
		public void GetRoundedValueTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			foreach (string value in values)
			{
				Assert.AreEqual (value, dimension.GetRoundedValue (value));
			}
		}


		[TestMethod]
		public void GetIndexOfArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetIndexOfTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			foreach (string value in values)
			{
				Assert.AreEqual (values.IndexOf (value), dimension.GetIndexOf (value));
			}
		}


		[TestMethod]
		public void GetValueAtArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void GetValueAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", "name", values);

			for (int i = 0; i < values.Count; i++)
			{
				Assert.AreEqual (values[i], dimension.GetValueAt (i));
			}
		}


		[TestMethod]
		public void BuildCodeDimensionArgumentCheck()
		{
			Assert.Inconclusive ();

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => CodeDimension.BuildCodeDimension (null, "a;b")
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//  () => CodeDimension.BuildCodeDimension ("", "a;b")
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => CodeDimension.BuildCodeDimension ("name", null)
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => CodeDimension.BuildCodeDimension ("name", "")
			//);

			//ExceptionAssert.Throw<System.Exception>
			//(
			//    () => CodeDimension.BuildCodeDimension ("name", "True;")
			//);

			//ExceptionAssert.Throw<System.ArgumentException>
			//(
			//    () => CodeDimension.BuildCodeDimension ("name", "True;fdafda&;fdsafdas")
			//);
		}


		[TestMethod]
		public void GetStringDataAndBuildCodeDimensionTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension1 = new CodeDimension ("code", "name", values);
			CodeDimension dimension2 = CodeDimension.BuildCodeDimension (dimension1.Code, dimension1.Name, dimension1.GetStringData ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


		[TestMethod]
		public void XmlExportArgumentCheck()
		{
			Assert.Inconclusive ();
		}


		[TestMethod]
		public void XmlImportExportTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension1 = new CodeDimension ("code", "name", values);
			CodeDimension dimension2 = (CodeDimension) AbstractDimension.XmlImport (dimension1.XmlExport ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			Assert.AreEqual (dimension1.Name, dimension2.Name);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


	}


}
