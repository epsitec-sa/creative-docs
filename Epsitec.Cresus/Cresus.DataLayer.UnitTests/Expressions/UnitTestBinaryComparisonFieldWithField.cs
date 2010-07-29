using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestComparisonFieldField
	{
		

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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void LeftTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (leftField, comparison.Left);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void OperatorTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreEqual (op, comparison.Operator);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void RightTest()
		{
			Field leftField = new Field (Druid.FromLong (1));
			Field rightField = new Field (Druid.FromLong (2));
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldField (leftField, op, rightField);

			Assert.AreSame (rightField, comparison.Right);
		}


	}


}
