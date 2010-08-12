using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryComparison
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
		public void FieldTest()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

			Assert.AreSame (field, comparison.Field);
		}


		[TestMethod]
		public void OperatorTest()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison1 = new UnaryComparison (field, UnaryComparator.IsNull);
			UnaryComparison comparison2 = new UnaryComparison (field, UnaryComparator.IsNotNull);

			Assert.AreEqual (UnaryComparator.IsNull, comparison1.Operator);
			Assert.AreEqual (UnaryComparator.IsNotNull, comparison2.Operator);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest1()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				ExpressionConverter converter = new ExpressionConverter (dataContext);
				
				comparison.CreateDbCondition (converter, null);
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionTest2()
		{
			Field field = new Field (Druid.FromLong (1));

			UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

			comparison.CreateDbCondition (null, id => null);
		}


	}


}
