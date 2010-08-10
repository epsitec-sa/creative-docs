using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestBinaryOperation
	{


		[TestMethod]
		public void BinaryOperationConstructorTest1()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			new BinaryOperation (left, BinaryOperator.And, right);
			new BinaryOperation (left, BinaryOperator.Or, right);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryOperationConstructorTest2()
		{
			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			new BinaryOperation (null, BinaryOperator.And, right);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryOperationConstructorTest3()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			new BinaryOperation (left, BinaryOperator.And, null);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			operation.CreateDbCondition (null);
		}


	}


}
