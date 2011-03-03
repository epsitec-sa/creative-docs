using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

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
	public sealed class UnitTestBinaryOperation
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void BinaryOperationConstructorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			new BinaryOperation (left, BinaryOperator.And, right);
			new BinaryOperation (left, BinaryOperator.Or, right);
		}


		[TestMethod]
		public void BinaryOperationConstructorArgumentCheck()
		{
			Field rightField = new Field (Druid.FromLong (1));
			Field leftField = new Field (Druid.FromLong (2));
			
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);
			
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryOperation (null, BinaryOperator.And, right)
			);
			
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryOperation (left, BinaryOperator.And, null)
			);
		}


		[TestMethod]
		public void LeftTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			Assert.AreSame(left, operation.Left);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation1 = new BinaryOperation (left, BinaryOperator.And, right);
			BinaryOperation operation2 = new BinaryOperation (left, BinaryOperator.Or, right);

			Assert.AreEqual (BinaryOperator.And, operation1.Operator);
			Assert.AreEqual (BinaryOperator.Or, operation2.Operator);
		}


		[TestMethod]
		public void RightTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			Assert.AreSame (right, operation.Right);
		}


		[TestMethod]
		public void GetFieldsTest()
		{
			foreach (Field field1 in ExpressionHelper.GetSampleFields ())
			{
				foreach (Field field2 in ExpressionHelper.GetSampleFields ())
				{
					BinaryOperation operation1 = new BinaryOperation (
						new UnaryComparison (field1, UnaryComparator.IsNull),
						BinaryOperator.And,
						new UnaryComparison (field2, UnaryComparator.IsNull)
					);

					List<Druid> actualFields1 = operation1.GetFields ().ToList ();
					List<Druid> expectedFields1 = new List<Druid> () { field1.FieldId, field2.FieldId, }.Distinct ().ToList ();

					Assert.IsTrue (actualFields1.Count == expectedFields1.Count);
					Assert.IsTrue (actualFields1.Except (expectedFields1).Count () == 0);

					foreach (Field field3 in ExpressionHelper.GetSampleFields ())
					{
						BinaryOperation operation2 = new BinaryOperation (
							new UnaryComparison (field1, UnaryComparator.IsNull),
							BinaryOperator.And,
							new ComparisonFieldField (field2, BinaryComparator.IsEqual, field3)
						);

						List<Druid> actualFields2 = operation2.GetFields ().ToList ();
						List<Druid> expectedFields2 = expectedFields1.Append (field3.FieldId).Distinct ().ToList ();

						Assert.IsTrue (actualFields2.Count == expectedFields2.Count);
						Assert.IsTrue (actualFields2.Except (expectedFields2).Count () == 0);

						foreach (Field field4 in ExpressionHelper.GetSampleFields ())
						{
							BinaryOperation operation3 = new BinaryOperation (
								new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2),
								BinaryOperator.And,
								new ComparisonFieldField (field3, BinaryComparator.IsEqual, field4)
							);

							List<Druid> actualFields3 = operation3.GetFields ().ToList ();
							List<Druid> expectedFields3 = expectedFields2.Append (field4.FieldId).Distinct ().ToList ();

							Assert.IsTrue (actualFields3.Count == expectedFields3.Count);
							Assert.IsTrue (actualFields3.Except (expectedFields3).Count () == 0);
						}
					}
				}
			}
		}


	}


}
