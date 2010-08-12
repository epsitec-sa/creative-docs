using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestComparisonFieldField
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
		public void BinaryComparisonFieldWithFieldConstructorTest1()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldField (leftField, op, rightField);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryComparisonFieldWithFieldConstructorTest2()
		{
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldField (null, op, rightField);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void BinaryComparisonFieldWithFieldConstructorTest3()
		{
			Field leftField = new Field (Druid.FromLong (1));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldField (leftField, op, null);
		}


		[TestMethod]
		public void LeftTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (leftField, comparison.Left);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreEqual (op, comparison.Operator);
		}


		[TestMethod]
		public void RightTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (rightField, comparison.Right);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest1()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

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
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			comparison.CreateDbCondition (null, id => null);
		}


	}


}
