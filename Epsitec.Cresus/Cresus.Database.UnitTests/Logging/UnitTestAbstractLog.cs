using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using FirebirdSql.Data.FirebirdClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Data;

using System.Linq;



namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestAbstractLog
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void LogResultTest()
		{
			TestLog testLog = new TestLog ();
			Assert.IsFalse (testLog.LogResult);

			testLog.LogResult = true;
			Assert.IsTrue (testLog.LogResult);

			testLog.LogResult = false;
			Assert.IsFalse (testLog.LogResult);
		}


		[TestMethod]
		public void LogThreadNameTest()
		{
			TestLog testLog = new TestLog ();
			Assert.IsFalse (testLog.LogThreadName);

			testLog.LogThreadName = true;
			Assert.IsTrue (testLog.LogThreadName);

			testLog.LogThreadName = false;
			Assert.IsFalse (testLog.LogThreadName);
		}


		[TestMethod]
		public void LogStackTraceTest()
		{
			TestLog testLog = new TestLog ();
			Assert.IsFalse (testLog.LogStackTrace);

			testLog.LogStackTrace = true;
			Assert.IsTrue (testLog.LogStackTrace);

			testLog.LogStackTrace = false;
			Assert.IsFalse (testLog.LogStackTrace);
		}


		[TestMethod]
		public void TestQueryNumber()
		{
			TestLog testLog = new TestLog ();

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);

			for (int i = 0; i < 1000; i++)
			{
				testLog.AddEntry (command, startTime, duration);

				Assert.AreEqual (i+1, testLog.GetEntry (i).Number);
			}
		}


		[TestMethod]
		public void AddEntryArgumentCheck()
		{
			TestLog testLog = new TestLog ();

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => testLog.AddEntry (null, DateTime.Now, TimeSpan.Zero)
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => testLog.AddEntry (null, DateTime.Now, TimeSpan.Zero, new object ())
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => testLog.AddEntry (null, DateTime.Now, TimeSpan.Zero, new List<object> ())
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => testLog.AddEntry (null, DateTime.Now, TimeSpan.Zero, new DataSet ())
			);
		}


		[TestMethod]
		public void AddEntryNoDataTest()
		{
			TestLog testLog = new TestLog ()
			{
				LogResult = true,
				LogStackTrace = true,
				LogThreadName = true,
			};

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);

			testLog.AddEntry (command, startTime, duration);

			Assert.AreEqual (1, testLog.GetNbEntries ());

			Query entry = testLog.GetEntry (0);

			Assert.AreEqual (command.CommandText, entry.SourceCode);
			Assert.AreEqual (startTime, entry.StartTime);
			Assert.AreEqual (duration, entry.Duration);
			Assert.AreEqual (1, entry.Parameters.Count);
			Assert.AreEqual ("name", entry.Parameters[0].Name);
			Assert.AreEqual ("value", entry.Parameters[0].Value);
			Assert.IsNull (entry.Result);
			Assert.AreEqual (System.Threading.Thread.CurrentThread.Name, entry.ThreadName);

			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace (0, true);

			Assert.AreEqual (stackTrace.GetFrame (0).GetMethod (), entry.StackTrace.GetFrame (0).GetMethod ());
			CollectionAssert.AreEqual (stackTrace.GetFrames ().Skip (1).Select (f => f.ToString ()).ToList (), entry.StackTrace.GetFrames ().Skip (1).Select (sf => sf.ToString ()).ToList ());
		}


		[TestMethod]
		public void AddEntryWithObjectDataTest()
		{
			TestLog testLog = new TestLog ()
			{
				LogResult = true,
				LogThreadName = true,
				LogStackTrace = true,
			};

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);
			object data = new object ();

			testLog.AddEntry (command, startTime, duration, data);

			Assert.AreEqual (1, testLog.GetNbEntries ());

			Query entry = testLog.GetEntry (0);

			Assert.AreEqual (command.CommandText, entry.SourceCode);
			Assert.AreEqual (startTime, entry.StartTime);
			Assert.AreEqual (duration, entry.Duration);
			Assert.AreEqual (1, entry.Parameters.Count);
			Assert.AreEqual ("name", entry.Parameters[0].Name);
			Assert.AreEqual ("value", entry.Parameters[0].Value);
			Assert.IsNotNull (entry.Result);
			Assert.AreEqual (1, entry.Result.Tables.Count);
			Assert.AreEqual ("result", entry.Result.Tables[0].Name);
			Assert.AreEqual (1, entry.Result.Tables[0].Columns.Count);
			Assert.AreEqual ("result", entry.Result.Tables[0].Columns[0].Name);
			Assert.AreEqual (1, entry.Result.Tables[0].Rows.Count);
			Assert.AreEqual (1, entry.Result.Tables[0].Rows[0].Values.Count);
			Assert.AreEqual (data, entry.Result.Tables[0].Rows[0].Values[0]);
			Assert.AreEqual (System.Threading.Thread.CurrentThread.Name, entry.ThreadName);

			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace (0, true);

			Assert.AreEqual (stackTrace.GetFrame (0).GetMethod (), entry.StackTrace.GetFrame (0).GetMethod ());
			CollectionAssert.AreEqual (stackTrace.GetFrames ().Skip (1).Select (f => f.ToString ()).ToList (), entry.StackTrace.GetFrames ().Skip (1).Select (sf => sf.ToString ()).ToList ());
		}


		[TestMethod]
		public void AddEntryWithListDataTest()
		{
			TestLog testLog = new TestLog ()
			{
				LogResult = true,
				LogThreadName = true,
				LogStackTrace = true,
			};

			IDbCommand command = this.GetSampleCommand2 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);
			List<object> data = new List<object> ()
		    {
		        null,
		        "un",
		        2,
		        System.TimeSpan.FromDays(3),
		    };

			testLog.AddEntry (command, startTime, duration, data);

			Assert.AreEqual (1, testLog.GetNbEntries ());

			Query entry = testLog.GetEntry (0);

			Assert.AreEqual (command.CommandText, entry.SourceCode);
			Assert.AreEqual (startTime, entry.StartTime);
			Assert.AreEqual (duration, entry.Duration);
			Assert.AreEqual (1, entry.Parameters.Count);
			Assert.AreEqual ("name", entry.Parameters[0].Name);
			Assert.AreEqual ("value", entry.Parameters[0].Value);
			Assert.IsNotNull (entry.Result);
			Assert.AreEqual (1, entry.Result.Tables.Count);
			Assert.AreEqual ("result", entry.Result.Tables[0].Name);

			Assert.AreEqual (4, entry.Result.Tables[0].Columns.Count);
			for (int i = 0; i < 4; i++)
			{
				Assert.AreEqual ("name" + i, entry.Result.Tables[0].Columns[i].Name);
			}

			Assert.AreEqual (1, entry.Result.Tables[0].Rows.Count);
			CollectionAssert.AreEqual (data, entry.Result.Tables[0].Rows[0].Values);

			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace (0, true);

			Assert.AreEqual (stackTrace.GetFrame (0).GetMethod (), entry.StackTrace.GetFrame (0).GetMethod ());
			CollectionAssert.AreEqual (stackTrace.GetFrames ().Skip (1).Select (f => f.ToString ()).ToList (), entry.StackTrace.GetFrames ().Skip (1).Select (sf => sf.ToString ()).ToList ());
		}


		[TestMethod]
		public void AddEntryWithDataSetTest()
		{
			TestLog testLog = new TestLog ()
			{
				LogResult = true,
				LogThreadName = true,
				LogStackTrace = true,
			};

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);

			DataTable table = new DataTable ()
			{
				TableName = "table",
			};
			table.Columns.Add (new DataColumn ()
			{
				ColumnName = "column1",
			});
			table.Columns.Add (new DataColumn ()
			{
				ColumnName = "column2",
			});
			table.Rows.Add (new Row (new List<object> () { 1, 2 }));
			table.Rows.Add (new Row (new List<object> () { "un", "deux" }));

			DataSet data = new DataSet ();
			data.Tables.Add (table);

			testLog.AddEntry (command, startTime, duration, data);

			Assert.AreEqual (1, testLog.GetNbEntries ());

			Query entry = testLog.GetEntry (0);

			Assert.AreEqual (command.CommandText, entry.SourceCode);
			Assert.AreEqual (startTime, entry.StartTime);
			Assert.AreEqual (duration, entry.Duration);
			Assert.AreEqual (1, entry.Parameters.Count);
			Assert.AreEqual ("name", entry.Parameters[0].Name);
			Assert.AreEqual ("value", entry.Parameters[0].Value);
			Assert.IsNotNull (entry.Result);
			Assert.AreEqual (1, entry.Result.Tables.Count);
			Assert.AreEqual ("table", entry.Result.Tables[0].Name);
			Assert.AreEqual (2, entry.Result.Tables[0].Columns.Count);
			Assert.AreEqual ("column1", entry.Result.Tables[0].Columns[0].Name);
			Assert.AreEqual ("column2", entry.Result.Tables[0].Columns[1].Name);
			Assert.AreEqual (2, entry.Result.Tables[0].Rows.Count);
			CollectionAssert.AreEqual (table.Rows[0].ItemArray.ToList (), entry.Result.Tables[0].Rows[0].Values);
			CollectionAssert.AreEqual (table.Rows[1].ItemArray.ToList (), entry.Result.Tables[0].Rows[1].Values);

			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace (0, true);

			Assert.AreEqual (stackTrace.GetFrame (0).GetMethod (), entry.StackTrace.GetFrame (0).GetMethod ());
			CollectionAssert.AreEqual (stackTrace.GetFrames ().Skip (1).Select (f => f.ToString ()).ToList (), entry.StackTrace.GetFrames ().Skip (1).Select (sf => sf.ToString ()).ToList ());
		}


		[TestMethod]
		public void AddEntryOptionDisabledTest()
		{
			TestLog testLog = new TestLog ()
			{
				LogResult = false,
				LogThreadName = false,
				LogStackTrace = false,
			};

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);

			DataTable table = new DataTable ()
			{
				TableName = "table",
			};
			table.Columns.Add (new DataColumn ()
			{
				ColumnName = "column1",
			});
			table.Columns.Add (new DataColumn ()
			{
				ColumnName = "column2",
			});
			table.Rows.Add (new Row (new List<object> () { 1, 2 }));
			table.Rows.Add (new Row (new List<object> () { "un", "deux" }));

			DataSet data = new DataSet ();
			data.Tables.Add (table);

			testLog.AddEntry (command, startTime, duration, data);

			Assert.AreEqual (1, testLog.GetNbEntries ());

			Query entry = testLog.GetEntry (0);

			Assert.AreEqual (command.CommandText, entry.SourceCode);
			Assert.AreEqual (startTime, entry.StartTime);
			Assert.AreEqual (duration, entry.Duration);
			Assert.AreEqual (1, entry.Parameters.Count);
			Assert.AreEqual ("name", entry.Parameters[0].Name);
			Assert.AreEqual ("value", entry.Parameters[0].Value);
			Assert.IsNull (entry.Result);
			Assert.IsNull (entry.ThreadName);
			Assert.IsNull (entry.StackTrace);
		}


		private IDbCommand GetSampleCommand1()
		{
			FbCommand command = new FbCommand ("my super sql query");

			command.Parameters.Add (new FbParameter ()
			{
				Value = "value",
				ParameterName = "name",
				Direction = ParameterDirection.Input
			});

			return command;
		}


		private IDbCommand GetSampleCommand2()
		{
			FbCommand command = new FbCommand ("my super sql query");

			command.Parameters.Add (new FbParameter ()
			{
				Value = "value",
				ParameterName = "name",
				Direction = ParameterDirection.Input
			});

			for (int i = 0; i < 4; i++)
			{
				command.Parameters.Add (new FbParameter ()
				{
					ParameterName = "name" + i,
					Direction = ParameterDirection.Output
				});
			}

			return command;
		}


		private class TestLog : AbstractLog
		{


			public TestLog() : base ()
			{
				this.log = new List<Query> ();
			}


			public override void Clear()
			{
				this.log.Clear ();
			}


			public override int GetNbEntries()
			{
				return this.log.Count;
			}


			public override Query GetEntry(int index)
			{
				return this.log[index];
			}


			public override ReadOnlyCollection<Query> GetEntries(int index, int count)
			{
				return this.log.Skip (index).Take (count).ToList ().AsReadOnly ();
			}


			protected override void AddEntry(Query query)
			{
				this.log.Add (query);
			}


			protected override int GetNextNumber()
			{
				return this.log.Count + 1;
			}


			private readonly List<Query> log;


		}


	}


}
