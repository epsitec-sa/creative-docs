using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbFactoryTest
	{
		[SetUp] public void LoadAssemblies()
		{
			DbFactory.Initialise ();
		}
		
		[Test] public void CheckFindDbAbstractionAndOpenClose()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			
			Assertion.AssertNotNull ("Could not instanciate Firebird abstraction", db_abstraction);
			Assertion.AssertNotNull ("Cannot retrieve the factory", db_abstraction.Factory);
			
			Assertion.AssertEquals ("Connection should not be alive", db_abstraction.IsConnectionAlive, false);
			Assertion.AssertEquals ("Connection should not be open", db_abstraction.IsConnectionOpen, false);
			
			db_abstraction.Connection.Open ();
			
			Assertion.AssertEquals ("Connection should be alive", db_abstraction.IsConnectionAlive, true);
			Assertion.AssertEquals ("Connection should be open", db_abstraction.IsConnectionOpen, true);
			
			db_abstraction.Connection.Close ();
		}
		
		[Test] public void CheckNewDbCommand()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			Assertion.AssertNotNull (command);
		}
		
		[Test] public void CheckNewDataAdapter()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			
			db_abstraction.Connection.Open ();
			
			System.Data.IDbCommand   command = db_abstraction.NewDbCommand ();
			System.Data.IDataAdapter adapter = db_abstraction.NewDataAdapter (command);
			
			db_abstraction.Connection.Close ();
			
			Assertion.AssertNotNull (adapter);
		}
		
		[Test] public void CheckExecuteScalar()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			command.Transaction = db_abstraction.BeginTransaction ();
			command.CommandType = System.Data.CommandType.Text;
			command.CommandText = "SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE;";
			
			object result = command.ExecuteScalar ();
			
			System.Console.Out.WriteLine ("Result: " + result.ToString () + ", type is " + result.GetType ().ToString ());
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void CheckExecuteReader()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			command.Transaction = db_abstraction.BeginTransaction ();
			command.CommandType = System.Data.CommandType.Text;
			command.CommandText = "SELECT * FROM RDB$DATABASE;";
			
			System.Data.IDataAdapter adapter = db_abstraction.NewDataAdapter (command);
			
			System.Data.DataSet data_set = new System.Data.DataSet ();
			int rows = adapter.Fill (data_set);
			
			System.Console.Out.WriteLine ("Rows returned: " + rows.ToString ());
			
			foreach (System.Data.DataTable table in data_set.Tables)
			{
				foreach (System.Data.DataColumn column in table.Columns)
				{
					System.Console.Out.WriteLine ("Col: " + column.ColumnName + ", type " + column.DataType.ToString ());
				}
			}
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void CheckUserTableNames()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			
			try
			{
				using (System.Data.IDbCommand command = db_abstraction.NewDbCommand ())
				{
					using (System.Data.IDbTransaction transaction = db_abstraction.BeginTransaction ())
					{
						command.Transaction = transaction;
						command.CommandType = System.Data.CommandType.Text;
						command.CommandText = "CREATE TABLE TestTable(MyField VARCHAR(50));";
						command.ExecuteNonQuery ();
						transaction.Commit ();
					}
				}
			}
			catch (System.Exception e)
			{
				System.Console.WriteLine (e.Message);
			}
			
			foreach (string name in db_abstraction.UserTableNames)
			{
				Assertion.Assert (string.Format ("Name contains white space: ({0})", name), name.IndexOf (' ') == -1);
				System.Console.Out.WriteLine ("Table : " + name);
			}
		}
		
		[Test] public void CheckDebugDumpRegisteredDbAbstractions ()
		{
			DbFactory.DebugDumpRegisteredDbAbstractions ();
		}
		
		
		
		public static DbAccess CreateDbAccess(bool force_db_creation)
		{
			DbAccess db_access = new DbAccess ();
			
			db_access.Provider		= "Firebird";
			db_access.LoginName		= "sysdba";
			db_access.LoginPassword = "masterkey";
			db_access.Database		= "test";
			db_access.Server		= "localhost";
			db_access.Create		= force_db_creation;
			
			return db_access;
		}
		
		public static IDbAbstraction CreateDbAbstraction(bool force_db_creation)
		{
			IDbAbstraction db_abstraction = null;
			DbAccess db_access = DbFactoryTest.CreateDbAccess (force_db_creation);
			
			try
			{
				db_abstraction = DbFactory.FindDbAbstraction (db_access);
			}
			catch (DbExistsException ex)
			{
				Assertion.AssertNotNull (ex);
				
				//	OK: la base existait déjà... on ne peut pas la créer !
				
				db_access.Create = false;
				db_abstraction = DbFactory.FindDbAbstraction (db_access);
			}
			
			return db_abstraction;
		}
		
		public static IDbAbstractionFactory CreateDbAbstractionFactory()
		{
			return DbFactory.FindDbAbstractionFactory ("Firebird");
		}
	}
}
