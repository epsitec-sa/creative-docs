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
			List<string> invalidCodes = new List<string> () { null, "", };

			foreach (string invalidCode in invalidCodes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new CodeDimension (invalidCode)
				);
			}
		}


		[TestMethod]
		public void Constructor1Test()
		{
			string code = "code";

			CodeDimension dimension = new CodeDimension (code);

			Assert.AreEqual (code, dimension.Code);
		}


		[TestMethod]
		public void Constructor2ArgumentCheck()
		{
			string code = "code";

			List<string> values = new List<string> () { "a", "b" };

			List<string> invalidCodes = new List<string> () { null, "", };
			List<List<string>> invalidValues = new List<List<string>> () { null, new List<string> () { "0", "0" }, };

			foreach (string invalidCode in invalidCodes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new CodeDimension (invalidCode, values)
				);
			}

			foreach (List<string> invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new CodeDimension (code, invalidValue)
				);
			}
		}


		[TestMethod]
		public void Constructor2Test()
		{
			string code = "code";
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			Assert.AreEqual (code, dimension.Code);
			CollectionAssert.AreEqual (values, dimension.Values.ToList ());
		}


		[TestMethod]
		public void CodeTest()
		{
			string code = "code";
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);
			Assert.AreEqual (code, dimension.Code);

			code = "newCode";
			dimension.Code = code;
			Assert.AreEqual (code, dimension.Code);
		}


		[TestMethod]
		public void ValuesTest()
		{
			int initialLength = 3;
			int totalLentgh = 6;

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", values.Take (initialLength));

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

			CodeDimension dimension = new CodeDimension ("code", values.Take (initialLength));

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Blupi", };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Add (invalidValue)
				);
			}
		}


		[TestMethod]
		public void AddTest()
		{
			CodeDimension dimension = new CodeDimension ("code");

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Blupi", };
			List<int> invalidIndexes = new List<int> () { -1, 4 };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Insert (0, invalidValue)
				);
			}

			foreach (int invalidIndex in invalidIndexes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Insert (invalidIndex, "Duke")
				);
			}
		}


		[TestMethod]
		public void InsertTest()
		{
			CodeDimension dimension = new CodeDimension ("code");

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Duke", };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Remove (invalidValue)
				);
			}
		}


		[TestMethod]
		public void RemoveTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			
			CodeDimension dimension = new CodeDimension ("code", values);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<int> invalidIndexes = new List<int> () { -1, 4 };

			foreach (int invalidIndex in invalidIndexes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.RemoveAt (invalidIndex)
				);
			}
		}


		[TestMethod]
		public void RemoveAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", values);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Duke" };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Swap (invalidValue, "Albert")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Swap ("Albert", invalidValue)
				);
			}
		}


		[TestMethod]
		public void SwapTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", values);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<int> invalidIndexes = new List<int> () { -1, 4 };

			foreach (int invalidIndex in invalidIndexes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.SwapAt (invalidIndex, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.SwapAt (0, invalidIndex)
				);
			}
		}


		[TestMethod]
		public void SwapAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", "Duke", "Edgard", "Fluff", }.Shuffle ().ToList ();
			List<string> expected = values.ToList ();

			CodeDimension dimension = new CodeDimension ("code", values);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.Contains (invalidValue)
				);
			}
		}


		[TestMethod]
		public void ContainsTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", values1);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.IsValueRoundable (invalidValue)
				);
			}
		}


		[TestMethod]
		public void IsValueRoundableTest()
		{
			List<string> values1 = new List<string> () { "Albert", "Blupi", "Christophe", };
			List<string> values2 = new List<string> () { "Duke", "Edgar", "Fluff", };

			CodeDimension dimension = new CodeDimension ("code", values1);

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
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Duke"};

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.GetRoundedValue (invalidValue)
				);
			}
		}


		[TestMethod]
		public void GetRoundedValueTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", values);

			foreach (string value in values)
			{
				Assert.AreEqual (value, dimension.GetRoundedValue (value));
			}
		}


		[TestMethod]
		public void GetIndexOfArgumentCheck()
		{
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<string> invalidValues = new List<string> () { null, "", "Duke" };

			foreach (string invalidValue in invalidValues)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.GetIndexOf (invalidValue)
				);
			}
		}


		[TestMethod]
		public void GetIndexOfTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", values);

			foreach (string value in values)
			{
				Assert.AreEqual (values.IndexOf (value), dimension.GetIndexOf (value));
			}
		}


		[TestMethod]
		public void GetValueAtArgumentCheck()
		{
			string code = "code";

			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension (code, values);

			List<int> invalidIndexes = new List<int> () { -1, 4 };

			foreach (int invalidIndex in invalidIndexes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dimension.GetValueAt (invalidIndex)
				);
			}
		}


		[TestMethod]
		public void GetValueAtTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", };

			CodeDimension dimension = new CodeDimension ("code", values);

			for (int i = 0; i < values.Count; i++)
			{
				Assert.AreEqual (values[i], dimension.GetValueAt (i));
			}
		}


		[TestMethod]
		public void BuildCodeDimensionArgumentCheck()
		{
			string code = "code";
			string data = ";1;2";

			List<string> invalidCodes = new List<string> () { null, "", };
			List<string> invalidDatas = new List<string> () { null, "", "f", };

			foreach (string invalidCode in invalidCodes)
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => CodeDimension.BuildCodeDimension (invalidCode, data)
				);
			}

			foreach (string invalidData in invalidDatas)
			{
				ExceptionAssert.Throw<System.Exception>
				(
					() => CodeDimension.BuildCodeDimension (code, invalidData)
				);
			}
		}


		[TestMethod]
		public void GetStringDataAndBuildCodeDimensionTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", ";", ":" };

			CodeDimension dimension1 = new CodeDimension ("code", values);
			CodeDimension dimension2 = CodeDimension.BuildCodeDimension (dimension1.Code, dimension1.GetStringData ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


		[TestMethod]
		public void XmlExportArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => NumericDimension.XmlImport (null)
			);
		}


		[TestMethod]
		public void XmlImportExportTest()
		{
			List<string> values = new List<string> () { "Albert", "Blupi", "Christophe", ";", ":" };

			CodeDimension dimension1 = new CodeDimension ("code", values);
			CodeDimension dimension2 = (CodeDimension) AbstractDimension.XmlImport (dimension1.XmlExport ());

			Assert.AreEqual (dimension1.Code, dimension2.Code);
			CollectionAssert.AreEqual (dimension1.Values.ToList (), dimension2.Values.ToList ());
		}


	}


}
