using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestBinaryOperation
	{


		// TODO Add tests for CreateSqlCondition(...)
		// TODO Add tests for CheckFields(...)


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			var leftField = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));
			var left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			var op = BinaryOperator.And;

			var rightField = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));
			var right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			var operation = new BinaryOperation (left, op, right);

			Assert.AreSame (left, operation.Left);
			Assert.AreEqual (op, operation.Operator);
			Assert.AreSame (right, operation.Right);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			var rightField = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));
			var leftField = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));

			var right = new UnaryComparison (rightField, UnaryComparator.IsNull);
			var left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryOperation (null, BinaryOperator.And, right)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryOperation (left, BinaryOperator.And, null)
			);
		}


	}


}
