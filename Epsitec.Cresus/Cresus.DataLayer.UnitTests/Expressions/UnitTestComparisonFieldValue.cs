using Epsitec.Common.Support;

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
		public void BinaryComparisonFieldWithValueConstructorTest1()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldValue (left, op, right);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryComparisonFieldWithValueConstructorTest2()
		{
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldValue (null, op, right);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryComparisonFieldWithValueConstructorTest3()
		{
			Field left = new Field (Druid.FromLong (1));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldValue (left, op, null);
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
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest1()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				ExpressionConverter converter = new ExpressionConverter (dataContext);

				comparison.CreateDbCondition (converter, null);
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest2()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			comparison.CreateDbCondition (null, id => null);
		}


	}


}
