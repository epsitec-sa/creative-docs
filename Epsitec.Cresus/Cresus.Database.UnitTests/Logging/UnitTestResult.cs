using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestResult
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Result (null)
			);
		}


		[TestMethod]
		public void ConstructorAndGettersTest()
		{
			List<Table> sampleTables = new List<Table> ()
			{
				this.GetSampleTable1 (),
				this.GetSampleTable2 (),
				this.GetSampleTable3 (),
			};

			for (int i = 0; i < sampleTables.Count; i++)
			{
				Result result = new Result (sampleTables.Take (i));

				CollectionAssert.AreEqual (sampleTables.Take (i).ToList (), result.Tables);
			}
		}


		public Table GetSampleTable1()
		{
			List<Column> columns = new List<Column> ();
			List<Row> rows = new List<Row> ();

			return new Table ("table1", columns, rows);
		}


		public Table GetSampleTable2()
		{
			List<Column> columns = new List<Column> ()
			{
				new Column ("First Name"),
				new Column ("Last Name"),
			};

			List<Row> rows = new List<Row> ()
			{
				new Row (new List<object> () { "Frodo", "Baggins", }),
				new Row (new List<object> () { "Sam", "Gamgee", }),
			};

			return new Table ("table2", columns, rows);
		}


		public Table GetSampleTable3()
		{
			List<Column> columns = new List<Column> ()
			{
				new Column ("Country"),
				new Column ("City"),
			};

			List<Row> rows = new List<Row> ()
			{
				new Row (new List<object> () { "Mordor", "Minas Morgul", }),
				new Row (new List<object> () { "Gondor", "Minias Tirith", }),
			};

			return new Table ("table3", columns, rows);
		}


	}


}
