using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using FirebirdSql.Data.FirebirdClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

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
		public void ModeTest()
		{
			TestLog testLog = new TestLog ();
			Assert.AreEqual (LogMode.Basic, testLog.Mode);

			testLog.Mode = LogMode.Extended;
			Assert.AreEqual (LogMode.Extended, testLog.Mode);

			testLog.Mode = LogMode.Basic;
			Assert.AreEqual (LogMode.Basic, testLog.Mode);
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
		public void AddEntryTest1()
		{
			TestLog testLog = new TestLog ();

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);
			
			foreach (var mode in new List<LogMode> () { LogMode.Basic, LogMode.Extended })
			{
				testLog.Clear ();
				testLog.Mode = mode;
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
			}
		}


		[TestMethod]
		public void AddEntryTest2()
		{
			TestLog testLog = new TestLog ();

			IDbCommand command = this.GetSampleCommand1 ();
			DateTime startTime = DateTime.Now;
			TimeSpan duration = TimeSpan.FromSeconds (123);
			object data = new object ();

			foreach (var mode in new List<LogMode> () { LogMode.Basic, LogMode.Extended })
			{
				testLog.Clear ();
				testLog.Mode = mode;
				testLog.AddEntry (command, startTime, duration, data);

				Assert.AreEqual (1, testLog.GetNbEntries ());

				Query entry = testLog.GetEntry (0);

				Assert.AreEqual (command.CommandText, entry.SourceCode);
				Assert.AreEqual (startTime, entry.StartTime);
				Assert.AreEqual (duration, entry.Duration);
				Assert.AreEqual (1, entry.Parameters.Count);
				Assert.AreEqual ("name", entry.Parameters[0].Name);
				Assert.AreEqual ("value", entry.Parameters[0].Value);

				switch (mode)
				{
					case LogMode.Basic:
						Assert.IsNull (entry.Result);
						break;

					case LogMode.Extended:

						Assert.IsNotNull (entry.Result);
						Assert.AreEqual (1, entry.Result.Tables.Count);
						Assert.AreEqual ("result", entry.Result.Tables[0].Name);
						Assert.AreEqual (1, entry.Result.Tables[0].Columns.Count);
						Assert.AreEqual ("result", entry.Result.Tables[0].Columns[0].Name);
						Assert.AreEqual (1, entry.Result.Tables[0].Rows.Count);
						Assert.AreEqual (1, entry.Result.Tables[0].Rows[0].Values.Count);
						Assert.AreEqual (data, entry.Result.Tables[0].Rows[0].Values[0]);

						break;

					default:
						Assert.Fail ();
						break;
				}
			}
		}


		[TestMethod]
		public void AddEntryTest3()
		{
			TestLog testLog = new TestLog ();

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

			foreach (var mode in new List<LogMode> () { LogMode.Basic, LogMode.Extended })
			{
				testLog.Clear ();
				testLog.Mode = mode;
				testLog.AddEntry (command, startTime, duration, data);

				Assert.AreEqual (1, testLog.GetNbEntries ());

				Query entry = testLog.GetEntry (0);

				Assert.AreEqual (command.CommandText, entry.SourceCode);
				Assert.AreEqual (startTime, entry.StartTime);
				Assert.AreEqual (duration, entry.Duration);
				Assert.AreEqual (1, entry.Parameters.Count);
				Assert.AreEqual ("name", entry.Parameters[0].Name);
				Assert.AreEqual ("value", entry.Parameters[0].Value);
				
				switch (mode)
				{
					case LogMode.Basic:
						Assert.IsNull (entry.Result);
						break;

					case LogMode.Extended:

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

						break;

					default:
						Assert.Fail ();
						break;
				}
			}
		}


		[TestMethod]
		public void AddEntryTest4()
		{
			TestLog testLog = new TestLog ();

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

			foreach (var mode in new List<LogMode> () { LogMode.Basic, LogMode.Extended })
			{
				testLog.Clear ();
				testLog.Mode = mode;
				testLog.AddEntry (command, startTime, duration, data);

				Assert.AreEqual (1, testLog.GetNbEntries ());

				Query entry = testLog.GetEntry (0);

				Assert.AreEqual (command.CommandText, entry.SourceCode);
				Assert.AreEqual (startTime, entry.StartTime);
				Assert.AreEqual (duration, entry.Duration);
				Assert.AreEqual (1, entry.Parameters.Count);
				Assert.AreEqual ("name", entry.Parameters[0].Name);
				Assert.AreEqual ("value", entry.Parameters[0].Value);

				switch (mode)
				{
					case LogMode.Basic:
						Assert.IsNull (entry.Result);
						break;

					case LogMode.Extended:

						Assert.IsNotNull (entry.Result);
						Assert.AreEqual (1, entry.Result.Tables.Count);
						Assert.AreEqual ("table", entry.Result.Tables[0].Name);
						Assert.AreEqual (2, entry.Result.Tables[0].Columns.Count);
						Assert.AreEqual ("column1", entry.Result.Tables[0].Columns[0].Name);
						Assert.AreEqual ("column2", entry.Result.Tables[0].Columns[1].Name);
						Assert.AreEqual (2, entry.Result.Tables[0].Rows.Count);
						CollectionAssert.AreEqual (table.Rows[0].ItemArray.ToList (), entry.Result.Tables[0].Rows[0].Values);
						CollectionAssert.AreEqual (table.Rows[1].ItemArray.ToList (), entry.Result.Tables[0].Rows[1].Values);

						break;

					default:
						Assert.Fail ();
						break;
				}
			}
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


			internal override void AddEntry(Query query)
			{
				this.log.Add (query);
			}


			private readonly List<Query> log;


		}


	}


}
