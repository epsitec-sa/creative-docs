using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestComparisonFieldField
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
				

		[TestMethod]
		public void BinaryComparisonFieldWithFieldConstructorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldField (leftField, op, rightField);
		}


		[TestMethod]
		public void BinaryComparisonFieldWithFieldConstructorArgumentCheck()
		{
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;
			Field leftField = new Field (Druid.FromLong (1));
			
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldField (null, op, rightField)
			);
					
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldField (leftField, op, null)
			);
		}


		[TestMethod]
		public void LeftTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (leftField, comparison.Left);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreEqual (op, comparison.Operator);
		}


		[TestMethod]
		public void RightTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (rightField, comparison.Right);
		}


		[TestMethod]
		public void GetFieldsTest()
		{
			foreach (Field field1 in ExpressionHelper.GetSampleFields ())
			{
				foreach (Field field2 in ExpressionHelper.GetSampleFields ())
				{
					ComparisonFieldField comparison = new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2);

					List<Druid> fields = comparison.GetFields ().ToList ();

					if (field1.FieldId == field2.FieldId)
					{
						Assert.IsTrue (fields.Count () == 1);
						Assert.AreEqual (field1.FieldId, fields.Single ());
					}
					else
					{
						Assert.IsTrue (fields.Count () == 2);
						CollectionAssert.Contains (fields, field1.FieldId);
						CollectionAssert.Contains (fields, field2.FieldId);
					}
				}
			}
		}


	}


}
