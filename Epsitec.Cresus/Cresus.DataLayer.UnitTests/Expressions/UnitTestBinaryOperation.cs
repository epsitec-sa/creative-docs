using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
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
		public void CreateDbConditionTestArgumentCheck()
		{
			Field leftField = new Field (Druid.FromLong (1));
			UnaryComparison left = new UnaryComparison (leftField, UnaryComparator.IsNull);

			Field rightField = new Field (Druid.FromLong (1));
			UnaryComparison right = new UnaryComparison (rightField, UnaryComparator.IsNull);

			BinaryOperation operation = new BinaryOperation (left, BinaryOperator.And, right);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => operation.CreateDbCondition (converter, null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => operation.CreateDbCondition (null, id => null)
					);
				}
			}
		}


	}


}
