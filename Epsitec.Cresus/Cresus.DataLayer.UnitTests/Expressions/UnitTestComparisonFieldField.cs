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
		public void BinaryComparisonFieldWithFieldConstructorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			new ComparisonFieldField (leftField, op, rightField);
		}


		[TestMethod]
		public void BinaryComparisonFieldWithFieldConstructorArgumentCheck()
		{
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;
			Field leftField = new Field (Druid.FromLong (1));
			
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldField (null, op, rightField)
			);
					
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ComparisonFieldField (leftField, op, null)
			);
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
		public void CreateDbConditionArgumentCheck()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

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
