using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
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
		public void CreateDbConditionTest1()
		{
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

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
			Field field = new Field (Druid.FromLong (1));
			UnaryComparison expression = new UnaryComparison (field, UnaryComparator.IsNull);

			UnaryOperation operation = new UnaryOperation (UnaryOperator.Not, expression);

			operation.CreateDbCondition (null, id => null);
		}


	}


}
