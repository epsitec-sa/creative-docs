using NUnit.Framework;
using System.Data;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class ISqlBuilderTest
	{
		[Test] public void Check01SqlBuilder()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			Assert.IsNotNull (sql_builder);
		}
		
		[Test] [ExpectedException (typeof (DbSyntaxException))] public void Check02InsertTableExPrimaryKey()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ("Test");
			SqlColumn sql_col_a = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sql_col_b = new SqlColumn ("B", DbRawType.Int64, Nullable.Yes);
			SqlColumn sql_col_c = new SqlColumn ("C", DbRawType.Int32);
			
			sql_table.Columns.Add (sql_col_a);
		// 	sql_table.Columns.Add (sql_col_b);		// volontairement omis pour provoquer l'exception !
			sql_table.Columns.Add (sql_col_c);
			
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_a, sql_col_b };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
		}
		
		[Test] public void Check03InsertTableFbProblem()
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
				while (reader.Read ())	//	<-- �a plantait avec Firebird.NET 1.5 Beta 1
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
			//command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void Check04InsertTable()
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
			//command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void Check05InsertTableColumns()
		{
			// doit �tre fait apr�s CheckInsertTable
			// et avant CheckRemoveTable
			// pour que la table existe d�j�

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
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);

			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check06RemoveTableColumns()
		{
			// doit �tre fait apr�s CheckInsertTable et apr�s CheckInsertTableColumns
			// et avant CheckRemoveTable
			// pour que la table existe d�j�

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
	
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, Nullable.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed2",   DbRawType.String,  50, false, Nullable.Yes);
			
			SqlColumn[] columns = { sql_col_1, sql_col_2, sql_col_3, sql_col_4 };
			sql_builder.RemoveTableColumns ("FbTestTable", columns);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);

			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		
		[Test] public void Check07InsertData()
		{
			// doit �tre fait apr�s CheckRemoveTableColumns
			// pour que la table existe dans le bon �tat

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;

			Collections.SqlFields fields = new Collections.SqlFields ();

			SqlField field;
			
			field = SqlField.CreateConstant (123, DbRawType.Int32);
			field.Alias = "Cr_ID";
			fields.Add (field);
			
			field = SqlField.CreateConstant (456, DbRawType.Int32);
			field.Alias = "Cr_REV";
			fields.Add (field);
			
			field = SqlField.CreateConstant ("Test � Copyright 2003", DbRawType.String);
			field.Alias = "StringDynamic";
			fields.Add (field);

			sql_builder.InsertData ("FbTestTable", fields);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check08RemoveData()
		{
			// doit �tre fait apr�s CheckInsertData
			// pour que la table existe dans le bon �tat

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;

			Collections.SqlFields conditions = new Collections.SqlFields ();

			SqlField field1, field2;
			
			field1 = SqlField.CreateConstant (123, DbRawType.Int32);
			field1.Alias = "Cr_ID";
			
			field2 = SqlField.CreateConstant (456, DbRawType.Int32);
			field2.Alias = "Cr_REV";
			
			//	d�fini la fonction Cr_ID == 123
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("Cr_ID"),
				field1);

			conditions.Add (SqlField.CreateFunction(sql_func));

			//	d�fini la fonction Cr_REV == 456
			sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("Cr_REV"),
				field1);

			conditions.Add (SqlField.CreateFunction(sql_func));

			sql_builder.RemoveData ("FbTestTable", conditions);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check09RemoveTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			sql_builder.RemoveTable ("FbTestTable");
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			command.ExecuteNonQuery ();
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check10InsertTableWithSqlEngine()
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
			
			sql_engine.Execute (command, command_type, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void Check11RemoveTableWithSqlEngine()
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
			
			sql_engine.Execute (command, command_type, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check12InsertArrayTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
			
			sql_builder.AutoClear = true;
			System.Data.IDbCommand command;

			// supprime la table si jamais
			
			sql_builder.RemoveTable ("FbTestArrayTable");
			
			command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
				
			command.Transaction = db_abstraction.BeginTransaction ();
			try
			{
				command.ExecuteNonQuery ();
				command.Transaction.Commit ();
				//command.Transaction.Dispose ();
				command.Dispose ();
			}
			catch
			{
				command.Transaction.Rollback ();
				//command.Transaction.Dispose ();
				command.Dispose ();
			}
			
			//	cr�e une table avec des champs binaires variables (BLOB)

			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("ArrayDynamic", DbRawType.ByteArray, 11000, false);
			SqlColumn sql_col_3 = new SqlColumn ("ArrayFixed",   DbRawType.ByteArray, 50, true, Nullable.Yes);	// les blobs fixes existent-ils ?
			
			sql_table.Name = "FbTestArrayTable";
			sql_table.Columns.Add (sql_col_1);
			sql_table.Columns.Add (sql_col_2);
			sql_table.Columns.Add (sql_col_3);
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1 };
			
			sql_builder.InsertTable (sql_table);
			
			command	= sql_builder.Command;
			
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();

			//	y �crit des donn�es

			byte[] insert_values = new byte[10000];
			for ( int i = 0 ; i < 10000 ; i++ )
			{
				insert_values[i] = (byte)(i);
			}

			Collections.SqlFields fields = new Collections.SqlFields ();

			SqlField field_ID, field_BLOB;
			
			field_ID = SqlField.CreateConstant (1, DbRawType.Int32);
			field_ID.Alias = "Cr_ID";
			fields.Add (field_ID);
			
			field_BLOB = SqlField.CreateConstant (insert_values, DbRawType.ByteArray);
			field_BLOB.Alias = "ArrayDynamic";
			fields.Add (field_BLOB);

			sql_builder.InsertData ("FbTestArrayTable", fields);
			
			command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();

			//	va relire les donn�es �crites dans le BLOB

			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateName ("Cr_ID"));
			sql_select.Fields.Add (SqlField.CreateName ("ArrayDynamic"));
			sql_select.Tables.Add (SqlField.CreateName ("FbTestArrayTable"));

			//	d�fini la fonction CR_ID == valeur
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName ("Cr_ID"),
				field_ID);

			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			//	lecture des r�sultats
			command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);

			DataSet data_set = new DataSet ();
			command.Transaction = db_abstraction.BeginTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			Assert.AreEqual (typeof (int),    data_set.Tables[0].Columns[0].DataType);
			Assert.AreEqual (typeof (object), data_set.Tables[0].Columns[1].DataType);
			
			//	r�cup�re l'objet de la premi�re ligne, seconde colonne
			
			DataTable  data_table  = data_set.Tables[0];
			DataColumn blob_column = data_table.Columns[1];
			System.Type blob_type  = blob_column.DataType;
			
			//	H�las, on n'a pas un type exact pour la colonne ici... J'aurais bien aim� que le
			//	provider retourne un type byte[] !
			
			Assert.AreEqual (typeof (object), blob_type);
//-			Assert.AreEqual (11000, blob_column.MaxLength);
			
			DataRow     data_row  = data_table.Rows[0];
			object	    blob      = data_row[1];
			System.Type data_type = blob.GetType ();
			
			Assert.AreEqual (typeof (byte[]), data_type);
			
			byte[] data_array = data_row[1] as byte[];
			
			Assert.IsNotNull (data_array);
			Assert.AreEqual (10000, data_array.Length);

			for (int i = 0; i < 10000; i++)
			{
				Assert.AreEqual (insert_values[i], data_array[i]);
			}
		}
	}
}
