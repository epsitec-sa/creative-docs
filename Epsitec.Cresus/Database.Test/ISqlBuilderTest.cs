using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class ISqlBuilderTest
	{
		[SetUp] public void LoadAssemblies()
		{
			DbFactory.Initialise ();
		}
		
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
			
			System.Data.IDataReader reader;
			command.Transaction = db_abstraction.BeginTransaction ();
			reader = command.ExecuteReader ();
			int result = 0;
			for (;;)
			{
				System.Console.Out.WriteLine ("Result {0}", result++);
//				while (reader.Read ())
//				{
//					System.Console.Out.WriteLine ("{0} columns found.", reader.FieldCount);
//				}
				if (reader.NextResult () == false)
				{
					break;
				}
			}
			reader.Close ();
			
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
	}
}
