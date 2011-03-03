using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestField
	{


		[TestMethod]
		public void FieldConstructorTest()
		{
			foreach (Druid druid in this.GetSampleData ())
			{
				Field field = new Field (druid);

				Assert.AreEqual (druid, field.FieldId);
			}
		}


		private IEnumerable<Druid> GetSampleData()
		{
			for (int i = 0; i < 100; i++)
			{
				yield return Druid.FromLong (i);	
			}
		}


	}


}
