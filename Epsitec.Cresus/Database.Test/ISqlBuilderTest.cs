using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class ISqlBuilderTest
	{
		[Test] public void CheckSqlBuilder()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			Assertion.AssertNotNull (sql_builder);
		}
		
		[Test] [ExpectedException (typeof (DbSyntaxException))] public void CheckInsertTableExPrimaryKey()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ("Test");
			SqlColumn sql_col_a = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sql_col_b = new SqlColumn ("B", DbRawType.Int64, Nullable.Yes);
			SqlColumn sql_col_c = new SqlColumn ("C", DbRawType.Int32);
			
			sql_table.Columns.Add (sql_col_a);
			sql_table.Columns.Add (sql_col_c);
			
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_a, sql_col_b };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
		}
		
		[Test] public void CheckInsertTableFbProblem()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, Nullable.Yes);
			
			sql_table.Name = "FbTestTable";
			sql_table.Columns.Add (sql_col_1);
			sql_table.Columns.Add (sql_col_2);
			sql_table.Columns.Add (sql_col_3);
			sql_table.Columns.Add (sql_col_4);
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1, sql_col_2 };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			System.Data.IDataReader reader;
			command.Transaction = db_abstraction.BeginTransaction ();
			reader = command.ExecuteReader ();
			int result = 0;
			for (;;)
			{
				System.Console.Out.WriteLine ("Result {0}", result++);
				while (reader.Read ())	//	<-- ça plantait avec Firebird.NET 1.5 Beta 1
				{
					System.Console.Out.WriteLine ("{0} columns found.", reader.FieldCount);
				}
				if (reader.NextResult () == false)
				{
					break;
				}
			}
			reader.Close ();
			
			command.Transaction.Rollback ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void CheckInsertTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, Nullable.Yes);
			
			sql_table.Name = "FbTestTable";
			sql_table.Columns.Add (sql_col_1);
			sql_table.Columns.Add (sql_col_2);
			sql_table.Columns.Add (sql_col_3);
			sql_table.Columns.Add (sql_col_4);
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1, sql_col_2 };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			
			using (System.Data.IDataReader reader = command.ExecuteReader ())
			{
				int n = 1;
				while (reader.NextResult ()) n++;
				System.Console.Out.WriteLine ("Executed {0} commands", n);
			}
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void CheckInsertTableColumns()
		{
			// doit être fait après CheckInsertTable
			// et avant CheckRemoveTable
			// pour que la table existe déjà

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
	
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed2",   DbRawType.String,  50, false, Nullable.Yes);
			
			SqlColumn[] columns = { sql_col_1, sql_col_2, sql_col_3, sql_col_4 };
			sql_builder.InsertTableColumns ("FbTestTable", columns);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute(command, sql_builder.CommandType);

			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void CheckRemoveTableColumns()
		{
			// doit être fait après CheckInsertTable et après CheckInsertTableColumns
			// et avant CheckRemoveTable
			// pour que la table existe déjà

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
	
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, true, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, Nullable.Yes);
			
			SqlColumn[] columns = { sql_col_1, sql_col_2, sql_col_3, sql_col_4 };
			sql_builder.RemoveTableColumns ("FbTestTable", columns);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute(command, sql_builder.CommandType);

			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}

		
		[Test] public void CheckInsertData()
		{
			// doit être fait après CheckRemoveTableColumns
			// pour que la table existe dans le bon état

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;

			SqlFieldCollection fields = new SqlFieldCollection ();

			SqlField field;
			
			field = SqlField.CreateConstant (123, DbRawType.Int32);
			field.Alias = "Cr_ID";
			fields.Add (field);
			
			field = SqlField.CreateConstant (456, DbRawType.Int32);
			field.Alias = "Cr_REV";
			fields.Add (field);
			
			field = SqlField.CreateConstant ("Test © Copyright 2003", DbRawType.String);
			field.Alias = "StringDynamic";
			fields.Add (field);
			
			sql_builder.InsertData("FbTestTable", fields);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute(command, sql_builder.CommandType);
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void CheckRemoveTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			sql_builder.RemoveTable ("FbTestTable");
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			command.ExecuteNonQuery ();
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void CheckInsertTableWithSqlEngine()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
			
			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, Nullable.Yes);
			
			sql_table.Name = "FbTestTable";
			sql_table.Columns.AddRange (new SqlColumn[] { sql_col_1, sql_col_2, sql_col_3, sql_col_4 });
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1, sql_col_2 };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command      = sql_builder.Command;
			DbCommandType          command_type = sql_builder.CommandType;
			
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			System.Console.Out.WriteLine ("SQL Command Type: {0}", command_type.ToString ());
			
			command.Transaction = db_abstraction.BeginTransaction ();
			
			sql_engine.Execute (command, command_type);
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void CheckRemoveTableWithSqlEngine()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
			
			sql_builder.RemoveTable ("FbTestTable");
			
			System.Data.IDbCommand command      = sql_builder.Command;
			DbCommandType          command_type = sql_builder.CommandType;
			
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			System.Console.Out.WriteLine ("SQL Command Type: {0}", command_type.ToString ());
			
			command.Transaction = db_abstraction.BeginTransaction ();
			
			sql_engine.Execute (command, command_type);
			
			command.Transaction.Commit ();
			command.Transaction.Dispose ();
			command.Dispose ();
		}
	}
}
