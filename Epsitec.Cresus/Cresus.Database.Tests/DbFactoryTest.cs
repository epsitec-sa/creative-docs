using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbFactoryTest
	{
		[Test] public void CheckFindDbAbstractionAndOpenClose()
		{
			//	Détruit le fichier de test, pour contrôler la création
			//	et pour avoir un fichier vide pour les tests
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird15\Data\Epsitec\TEST.FIREBIRD");
			}
			catch {}

			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			
			Assert.IsNotNull (db_abstraction, "Could not instanciate Firebird abstraction");
			Assert.IsNotNull (db_abstraction.Factory, "Cannot retrieve the factory");
			
			Assert.IsFalse (db_abstraction.IsConnectionAlive, "Connection should not be alive");
			Assert.IsFalse (db_abstraction.IsConnectionOpen, "Connection should not be open");
			
			db_abstraction.Connection.Open ();
			
			Assert.IsTrue (db_abstraction.IsConnectionAlive, "Connection should be alive");
			Assert.IsTrue (db_abstraction.IsConnectionOpen, "Connection should be open");
			
			db_abstraction.Connection.Close ();
		}
		
		[Test] public void CheckNewDbCommand()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			Assert.IsNotNull (command);
		}
		
		[Test] public void CheckNewDataAdapter()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			
			db_abstraction.Connection.Open ();
			
			System.Data.IDbCommand   command = db_abstraction.NewDbCommand ();
			System.Data.IDataAdapter adapter = db_abstraction.NewDataAdapter (command);
			
			db_abstraction.Connection.Close ();
			
			Assert.IsNotNull (adapter);
		}
		
		[Test] public void CheckExecuteScalar()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
			command.CommandType = System.Data.CommandType.Text;
			command.CommandText = "SELECT CURRENT_TIMESTAMP FROM RDB$DATABASE;";
			
			object result = command.ExecuteScalar ();
			
			System.Console.Out.WriteLine ("Result: " + result.ToString () + ", type is " + result.GetType ().ToString ());
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void CheckExecuteReader()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			System.Data.IDbCommand command = db_abstraction.NewDbCommand ();
			
			command.Transaction = db_abstraction.BeginReadOnlyTransaction ();
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
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void CheckQueryUserTableNames()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			
			try
			{
				using (System.Data.IDbCommand command = db_abstraction.NewDbCommand ())
				{
					using (System.Data.IDbTransaction transaction = db_abstraction.BeginReadWriteTransaction ())
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
			
			foreach (string name in db_abstraction.QueryUserTableNames ())
			{
				Assert.AreEqual (-1, name.IndexOf (' '), string.Format ("Name contains white space: ({0})", name));
				System.Console.Out.WriteLine ("Table : " + name);
			}
		}
		
		[Test] public void CheckReadOnlyTransaction()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			string error_message = "";
			
			try
			{
				using (System.Data.IDbCommand command = db_abstraction.NewDbCommand ())
				{
					System.Data.IDbTransaction transaction = db_abstraction.BeginReadOnlyTransaction ();
					
					try
					{
						command.Transaction = transaction;
						command.CommandType = System.Data.CommandType.Text;
						command.CommandText = "CREATE TABLE X_TABLE_1(FIELD VARCHAR(50));";
						command.ExecuteNonQuery ();
						transaction.Commit ();
					}
					finally
					{
						transaction.Dispose ();
					}
				}
			}
			catch (System.Exception e)
			{
				error_message = e.Message;
			}
			
			System.Console.WriteLine (error_message);
			
			Assert.IsTrue (error_message.IndexOf ("attempted update during read-only transaction") > 0);
		}
		
		[Test] public void CheckDebugDumpRegisteredDbAbstractions ()
		{
			DbFactory.DebugDumpRegisteredDatabaseAbstractions ();
		}
		
		
		
		public static DbAccess CreateDbAccess(bool force_db_creation)
		{
			return new DbAccess ("Firebird", "test", "localhost", "sysdba", "masterkey", force_db_creation);
		}
		
		public static DbAccess CreateDbAccess(string name)
		{
			return new DbAccess ("Firebird", name, "localhost", "sysdba", "masterkey", false);
		}
		
		public static IDbAbstraction CreateDbAbstraction(bool force_db_creation)
		{
			IDbAbstraction db_abstraction = null;
			DbAccess db_access = DbFactoryTest.CreateDbAccess (force_db_creation);
			
			try
			{
				db_abstraction = DbFactory.CreateDatabaseAbstraction (db_access);
			}
			catch (Exceptions.ExistsException)
			{
				//	OK: la base existait déjà... on ne peut pas la créer !
				
				db_access.CreateDatabase = false;
				db_abstraction = DbFactory.CreateDatabaseAbstraction (db_access);
			}
			
			return db_abstraction;
		}
		
		public static IDbAbstractionFactory CreateDbAbstractionFactory()
		{
			return DbFactory.FindDatabaseAbstractionFactory ("Firebird");
		}
	}
}
