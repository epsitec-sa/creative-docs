using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryOperation
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		

		[TestMethod]
		public void UnaryOperationConstructorTest()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			new UnaryOperation (UnaryOperator.Not, expression);
		}


		[TestMethod]
		public void UnaryOperationConstructorTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new UnaryOperation (UnaryOperator.Not, null)
			);
		}


		[TestMethod]
		public void ExpressionTest()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

			Assert.AreSame (expression, operation.Expression);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

			Assert.AreEqual (UnaryOperator.Not, operation.Operator);
		}
		
		
		[TestMethod]
		public void GetFieldsTest()
		{
			foreach (Field field1 in ExpressionHelper.GetSampleFields ())
			{
				foreach (Field field2 in ExpressionHelper.GetSampleFields ())
				{
					UnaryOperation operation = new UnaryOperation (
						UnaryOperator.Not,
						new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2)
					);

					List<Druid> fields = operation.GetFields ().ToList ();

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
