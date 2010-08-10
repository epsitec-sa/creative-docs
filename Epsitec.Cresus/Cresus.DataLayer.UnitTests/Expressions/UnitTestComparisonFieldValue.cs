using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestComparisonFieldValue
	{
		
		
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
		public void CreateDbConditionTest()
		{
			Field left = new Field (Druid.FromLong (1));
			Constant right = new Constant (0);
			BinaryComparator op = BinaryComparator.IsEqual;

			var comparison = new ComparisonFieldValue (left, op, right);

			comparison.CreateDbCondition (null);
		}


	}


}
