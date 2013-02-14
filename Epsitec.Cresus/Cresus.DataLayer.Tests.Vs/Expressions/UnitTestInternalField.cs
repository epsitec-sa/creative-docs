using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestInternalField
	{


		// TODO Add tests for CreateSqlField(...)
		// TODO Add tests for CheckField(...)


		[TestMethod]
		public void CreateIdTest()
		{
			var entity = new NaturalPersonEntity();

			var field = InternalField.CreateId (entity);

			Assert.AreEqual (entity, field.Entity);
			Assert.AreEqual ("CR_ID", field.Name);
		}


		[TestMethod]
		public void CreateIdArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => InternalField.CreateId (null)
			);
		}


	}


}
