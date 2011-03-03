using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestComparisonFieldValue
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		
		
		[TestMethod]
		public void BinaryComparisonFieldWithValueConstructorTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldValue (left, op, right);
		}


		[TestMethod]
		public void BinaryComparisonFieldWithValueConstructorTestArgumentCheck()
		{
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;
			Field left = new Field (Druid.FromLong (1));

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldValue (null, op, right)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldValue (left, op, null)
			);
		}


		[TestMethod]
		public void LeftTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreSame (left, comparison.Left);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreEqual (op, comparison.Operator);
		}


		[TestMethod]
		public void RightTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreSame (right, comparison.Right);
		}


		[TestMethod]
		public void GetFieldsTest()
		{
			foreach (Field field in ExpressionHelper.GetSampleFields ())
			{
				ComparisonFieldValue comparison = new ComparisonFieldValue (field, BinaryComparator.IsEqual, new Constant (0));

				List<Druid> fields = comparison.GetFields ().ToList ();

				Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


	}


}
