using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System;


namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestQuery
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentException>
			(
				() => new Query (-1, DateTime.Now, TimeSpan.Zero, null, new List<Parameter> (), new Result (new List<Table> ()))
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new Query (0, DateTime.Now, TimeSpan.Zero, null, new List<Parameter> (), new Result (new List<Table> ()))
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new Query (0, DateTime.Now, TimeSpan.Zero, "my source code", null, new Result (new List<Table> ()))
			);
		}


		[TestMethod]
		public void ConstructorAndGettersTest()
		{
			int number = 42;
			var sourceCode = "My super SQL source code";
			var parameters = new List<Parameter> () { new Parameter ("parameter", "value") };

			var columns = new List<Column> () { new Column ("column") };
			var rows = new List<Row> () { new Row (new List<object> () { "result" }) };
			var tables = new List<Table> () { new Table ("table", columns, rows) };
			var result = new Result (tables);
			var threadName = "myThread";
			var stackTrace = new System.Diagnostics.StackTrace (true);

			var startTime = DateTime.Now;
			var duration = TimeSpan.FromTicks (123456789);

			Query query = new Query (number, startTime, duration, sourceCode, parameters, result, threadName, stackTrace);

			Assert.AreEqual (number, query.Number);
			Assert.AreEqual (sourceCode, query.SourceCode);
			CollectionAssert.AreEqual (parameters, query.Parameters);
			Assert.AreEqual (result, query.Result);
			Assert.AreEqual (startTime, query.StartTime);
			Assert.AreEqual (duration, query.Duration);
			Assert.AreEqual (threadName, query.ThreadName);
			Assert.AreEqual (stackTrace, query.StackTrace);
		}


	}


}
