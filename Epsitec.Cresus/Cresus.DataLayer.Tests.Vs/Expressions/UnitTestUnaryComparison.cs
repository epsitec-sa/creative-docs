using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestUnaryComparison
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		
		
		[TestMethod]
		public void UnaryComparisonConstructorTest()
		{
			Field field = new Field (Druid.FromLong (1));
			
			new UnaryComparison (field, UnaryComparator.IsNull);
			new UnaryComparison (field, UnaryComparator.IsNotNull);
		}


		[TestMethod ]
		public void UnaryComparisonConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new UnaryComparison (null, UnaryComparator.IsNull)
			);
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
		public void GetFieldsTest()
		{
			foreach (Field field in ExpressionHelper.GetSampleFields ())
			{
				UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

				List<Druid> fields = comparison.GetFields ().ToList ();

				Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


	}


}
