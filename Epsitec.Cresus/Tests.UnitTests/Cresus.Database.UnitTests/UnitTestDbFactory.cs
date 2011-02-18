using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbFactory
	{

		// TODO Investigate those tests and rename this class to UnitTestDbAbstraction?
		// Marc

		// TODO Do not use IDbAbstractionHelper in this test? Because IDbAbstraction is precisely
		// what we want to test here.
		// Marc


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			IDbAbstractionHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void FindDbAbstractionAndOpenCloseTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				Assert.IsNotNull (dbAbstraction);
				Assert.IsNotNull (dbAbstraction.Factory);

				Assert.IsFalse (dbAbstraction.IsConnectionAlive);
				Assert.IsFalse (dbAbstraction.IsConnectionOpen);

				dbAbstraction.Connection.Open ();

				Assert.IsTrue (dbAbstraction.IsConnectionAlive);
				Assert.IsTrue (dbAbstraction.IsConnectionOpen);

				dbAbstraction.Connection.Close ();
			}
		}


		[TestMethod]
		public void NewDbCommandTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				IDbCommand command = dbAbstraction.NewDbCommand ();

				Assert.IsNotNull (command);
			}
		}


		[TestMethod]
		public void NewDataAdapterTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				IDbCommand command = dbAbstraction.NewDbCommand ();
				IDataAdapter adapter = dbAbstraction.NewDataAdapter (command);

				dbAbstraction.Connection.Close ();

				Assert.IsNotNull (adapter);
			}
		}


		[TestMethod]
		public void ExecuteScalarTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				IDbCommand command = dbAbstraction.NewDbCommand ();

				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();
				command.CommandType = CommandType.Text;
				command.CommandText = "SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE;";

				object result = command.ExecuteScalar ();

				System.Console.Out.WriteLine ("Result: " + result.ToString () + ", type is " + result.GetType ().ToString ());

				command.Transaction.Commit ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void ExecuteReaderTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				IDbCommand command = dbAbstraction.NewDbCommand ();

				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();
				command.CommandType = CommandType.Text;
				command.CommandText = "SELECT * FROM RDB$DATABASE;";

				IDataAdapter adapter = dbAbstraction.NewDataAdapter (command);

				DataSet dataSet = new DataSet ();
				int rows = adapter.Fill (dataSet);

				System.Console.Out.WriteLine ("Rows returned: " + rows.ToString ());

				foreach (DataTable table in dataSet.Tables)
				{
					foreach (DataColumn column in table.Columns)
					{
						System.Console.Out.WriteLine ("Col: " + column.ColumnName + ", type " + column.DataType.ToString ());
					}
				}

				dataSet.Dispose ();
				command.Transaction.Commit ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void QueryUserTableNamesTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				using (IDbCommand command = dbAbstraction.NewDbCommand ())
				{
					using (IDbTransaction transaction = dbAbstraction.BeginReadWriteTransaction ())
					{
						command.Transaction = transaction;
						command.CommandType = CommandType.Text;
						command.CommandText = "CREATE TABLE TestTable(MyField VARCHAR(50));";
						command.ExecuteNonQuery ();
						transaction.Commit ();
					}
				}

				foreach (string name in dbAbstraction.QueryUserTableNames ())
				{
					Assert.AreEqual (-1, name.IndexOf (' '));
					System.Console.Out.WriteLine ("Table : " + name);
				}
			}
		}


		[TestMethod]
		public void ReadOnlyTransactionTest()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				dbAbstraction.Connection.Open ();

				using (IDbCommand command = dbAbstraction.NewDbCommand ())
				{
					using (IDbTransaction transaction = dbAbstraction.BeginReadOnlyTransaction ())
					{
						command.Transaction = transaction;
						command.CommandType = CommandType.Text;
						command.CommandText = "CREATE TABLE X_TABLE_1(FIELD VARCHAR(50));";

						ExceptionAssert.Throw
						(
							() => command.ExecuteNonQuery ()
						);

						transaction.Commit ();
					}
				}
			}
		}


	}


}
