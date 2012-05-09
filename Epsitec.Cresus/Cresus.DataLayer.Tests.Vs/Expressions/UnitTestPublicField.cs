using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestPublicField
	{


		// TODO Add tests for CreateSqlField(...)
		// TODO Add tests for CheckField(...)
		
		
		[TestMethod]
		public void ConstructorTest()
		{
			var entity = new NaturalPersonEntity ();
			var druid = Druid.FromLong (1);

			var field = new PublicField (entity, druid);

			Assert.AreEqual (entity, field.Entity);
			Assert.AreEqual (druid, field.FieldId);
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new PublicField (null, Druid.FromLong (1))
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => new PublicField (new NaturalPersonEntity (), Druid.Empty)
			);
		}


	}


}
