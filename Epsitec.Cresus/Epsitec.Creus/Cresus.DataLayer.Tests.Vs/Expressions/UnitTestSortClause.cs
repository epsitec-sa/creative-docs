using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestSortClause
	{


		// TODO Add tests for CreateSqlField(...)
		// TODO Add tests for CheckField(...)
		

		[TestMethod]
		public void ConstructorTest()
		{
			foreach (SortOrder sortOrder in Enum.GetValues (typeof (SortOrder)))
			{
				var field = new PublicField (new NaturalPersonEntity (), Druid.FromLong (1));

				var sortClause = new SortClause (field, sortOrder);

				Assert.AreSame (field, sortClause.Field);
				Assert.AreEqual (sortOrder, sortClause.SortOrder);
			}
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new SortClause (null, SortOrder.Ascending)
			);
		}


	}


}
