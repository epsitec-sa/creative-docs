using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestComparisonFieldValue
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
		public void BinaryComparisonFieldWithValueConstructorTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldValue (left, op, right);
		}


		[TestMethod]
		public void BinaryComparisonFieldWithValueConstructorTestArgumentCheck()
		{
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;
			Field left = new Field (Druid.FromLong (1));

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldValue (null, op, right)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldValue (left, op, null)
			);
		}


		[TestMethod]
		public void EscapeTest()
		{
			
		}


		[TestMethod]
		public void LeftTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreSame (left, comparison.Left);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreEqual (op, comparison.Operator);
		}


		[TestMethod]
		public void RightTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			Assert.AreSame (right, comparison.Right);
		}


		[TestMethod]
		public void CreateDbConditionArgumentCheck()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				ExpressionConverter converter = new ExpressionConverter (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => comparison.CreateDbCondition (converter, null)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => comparison.CreateDbCondition (null, id => null)
				);
			}
		}


	}


}
