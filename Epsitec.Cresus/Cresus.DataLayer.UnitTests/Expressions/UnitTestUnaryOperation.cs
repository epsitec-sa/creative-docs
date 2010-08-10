using Epsitec.Common.Support;
using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestUnaryOperation
	{


		[TestMethod]
		public void UnaryOperationConstructorTest1()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			new UnaryOperation (UnaryOperator.Not, expression);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void UnaryOperationConstructorTest2()
		{
			new UnaryOperation (UnaryOperator.Not, null);
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
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

			operation.CreateDbCondition (null);
		}


	}


}
