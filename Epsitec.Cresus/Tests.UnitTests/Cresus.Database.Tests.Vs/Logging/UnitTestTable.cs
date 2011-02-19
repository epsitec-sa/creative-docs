using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestTable
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Table (null, new List<Column> (), new List<Row> ())
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Table ("name", null, new List<Row> ())
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Table ("name", new List<Column> (), null)
			);
		}


		[TestMethod]
		public void ConstructorAndGettersTest()
		{
			string sampleName = "MiddleEarth";

			List<Column> sampleColums = new List<Column> ()
			{
				new Column ("Name"), new Column ("Country"), new Column ("Evilness"),
			};

			List<Row> sampleRows = new List<Row> ()
			{
				new Row (new List<object> () { "Frodo", "Shire", "Unevil" }),
				new Row (new List<object> () { "Golum", null, "Somewhat evil" }),
				new Row (new List<object> () { "Sauron", "Mordor", "Mega evil" }),
			};

			Table table = new Table (sampleName, sampleColums, sampleRows);

			Assert.AreEqual (sampleName, table.Name);

			CollectionAssert.AreEqual (sampleColums, table.Columns);
			CollectionAssert.AreEqual (sampleRows, table.Rows);
		}


	}


}
