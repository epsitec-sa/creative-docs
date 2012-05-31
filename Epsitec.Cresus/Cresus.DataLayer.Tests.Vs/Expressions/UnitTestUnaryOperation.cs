using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryOperation
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
			var field = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));
			var expression = new UnaryComparison (field, UnaryComparator.IsNull);

			var op = UnaryOperator.Not;

			var operation = new UnaryOperation (op, expression);

			Assert.AreSame (expression, operation.Expression);
			Assert.AreEqual (UnaryOperator.Not, operation.Operator);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new UnaryOperation (UnaryOperator.Not, null)
			);
		}


	}


}
