using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestBinaryOperation
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


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
		public void CreateDbConditionTest1()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				ExpressionConverter converter = new ExpressionConverter (dataContext);

				operation.CreateDbCondition (converter, null);
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest2()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			operation.CreateDbCondition (null, id => null);
		}


	}


}
