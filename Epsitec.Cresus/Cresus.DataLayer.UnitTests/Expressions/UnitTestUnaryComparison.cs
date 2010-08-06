using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestUnaryComparison
	{
		
		
		[TestMethod]
		public void UnaryComparisonConstructorTest1()
		{
			Field field = new Field (Druid.FromLong (1));
			
			new UnaryComparison (field, UnaryComparator.IsNull);
			new UnaryComparison (field, UnaryComparator.IsNotNull);
		}


		[TestMethod ]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void UnaryComparisonConstructorTest2()
		{
			new UnaryComparison (null, UnaryComparator.IsNull);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void FieldTest()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

			Assert.AreSame (field, comparison.Field);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void OperatorTest()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison1 = new UnaryComparison (field, UnaryComparator.IsNull);
			UnaryComparison comparison2 = new UnaryComparison (field, UnaryComparator.IsNotNull);

			Assert.AreEqual (UnaryComparator.IsNull, comparison1.Operator);
			Assert.AreEqual (UnaryComparator.IsNotNull, comparison2.Operator);
		}


	}


}
