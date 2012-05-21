using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryComparison
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
			var field = new PublicField (new NaturalPersonEntity (), Druid.FromLong (1));
			var op = UnaryComparator.IsNull;

			var comparison = new UnaryComparison (field, op);

			Assert.AreEqual (op, comparison.Operator);
			Assert.AreSame (field, comparison.Field);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new UnaryComparison (null, UnaryComparator.IsNull)
			);
		}


	}


}
