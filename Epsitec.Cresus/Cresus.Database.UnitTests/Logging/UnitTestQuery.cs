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
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new Query (null, new List<Parameter> (), new Result (new List<Table> ()), DateTime.Now, TimeSpan.Zero)
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new Query ("my source code", null, new Result (new List<Table> ()), DateTime.Now, TimeSpan.Zero)
			);
		}


		[TestMethod]
		public void ConstructorAndGettersTest()
		{
			var sourceCode = "My super SQL source code";
			var parameters = new List<Parameter> () { new Parameter ("parameter", "value") };

			var columns = new List<Column> () { new Column ("column") };
			var rows = new List<Row> () { new Row (new List<object> () { "result" }) };
			var tables = new List<Table> () { new Table ("table", columns, rows) };
			var result = new Result (tables);

			var startTime = DateTime.Now;
			var duration = TimeSpan.FromTicks (123456789);

			Query query = new Query (sourceCode, parameters, result, startTime, duration);

			Assert.AreEqual (sourceCode, query.SourceCode);
			CollectionAssert.AreEqual (parameters, query.Parameters);
			Assert.AreEqual (result, query.Result);
			Assert.AreEqual (startTime, query.StartTime);
			Assert.AreEqual (duration, query.Duration);
		}


	}


}
