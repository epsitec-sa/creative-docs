using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestCollectionField
	{


		// TODO Add tests for CreateSqlField(...)
		// TODO Add tests for CheckField(...)
		
		
		[TestMethod]
		public void ConstructorTest()
		{
			var source = new NaturalPersonEntity ();
			var fieldId = Druid.FromLong (1);
			var target = new NaturalPersonEntity ();

			var field = CollectionField.CreateRank (source, fieldId, target);

			Assert.AreEqual (source, field.Entity);
			Assert.AreEqual (fieldId, field.FieldId);
			Assert.AreEqual (target, field.Target);
			Assert.AreEqual ("CR_RANK", field.ColumnName);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => CollectionField.CreateRank (null, Druid.FromLong (1), new NaturalPersonEntity ())
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => CollectionField.CreateRank (new NaturalPersonEntity (), Druid.Empty, new NaturalPersonEntity ())
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => CollectionField.CreateRank (new NaturalPersonEntity (), Druid.FromLong (1), null)
			);
		}


	}


}
