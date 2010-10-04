using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryOperation
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
		public void CreateDbConditionTest1()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

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
