using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestBinaryComparison
	{


		// TODO Add tests for CreateSqlCondition(...)
		// TODO Add tests for CheckFields(...)


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			var left = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));
			var op = BinaryComparator.IsEqual;
			var right = new Constant (0);

			var comparison = new BinaryComparison (left, op, right);

			Assert.AreSame (left, comparison.Left);
			Assert.AreEqual (op, comparison.Operator);
			Assert.AreSame (right, comparison.Right);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			var right = new Constant (0);
			var op = BinaryComparator.IsEqual;
			var left = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryComparison (null, op, right)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new BinaryComparison (left, op, null)
			);
		}


	}


}
