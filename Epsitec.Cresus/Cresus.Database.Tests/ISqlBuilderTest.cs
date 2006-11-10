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
		
		[Test] [ExpectedException (typeof (Exceptions.SyntaxException))] public void Check02InsertTableExPrimaryKey()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ("Test");
			SqlColumn sql_col_a = new SqlColumn ("A", DbRawType.Int32);
			SqlColumn sql_col_b = new SqlColumn ("B", DbRawType.Int64, DbNullability.Yes);
			SqlColumn sql_col_c = new SqlColumn ("C", DbRawType.Int32);
			
			sql_table.Columns.Add (sql_col_a);
		//	sql_table.Columns.Add (sql_col_b);		// volontairement omis pour provoquer l'exception !
			sql_table.Columns.Add (sql_col_c);
			
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_a, sql_col_b };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
		}
		
#if false
		/* plus supporté depuis version 1.7 du provider .NET */
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
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
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
			//command.Transaction.Dispose ();
			command.Dispose ();
		}
#endif
		
		[Test] public void Check04InsertTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, DbNullability.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, DbNullability.Yes);
			
			sql_table.Name = "FbTestTable";
			sql_table.Columns.Add (sql_col_1);
			sql_table.Columns.Add (sql_col_2);
			sql_table.Columns.Add (sql_col_3);
			sql_table.Columns.Add (sql_col_4);
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1, sql_col_2 };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			string[] commands = command.CommandText.Split ('\n');
			
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			
			for (int i = 0; i < commands.Length-1; i++)
			{
				command.CommandText = commands[i];
				command.ExecuteNonQuery ();
			}
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}
		
		[Test] public void Check05InsertTableColumns()
		{
			//	doit être fait après CheckInsertTable
			//	et avant CheckRemoveTable
			//	pour que la table existe déjà

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
	
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, DbNullability.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed2",   DbRawType.String,  50, false, DbNullability.Yes);
			
			SqlColumn[] columns = { sql_col_1, sql_col_2, sql_col_3, sql_col_4 };
			sql_builder.InsertTableColumns ("FbTestTable", columns);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (-4, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check06RemoveTableColumns()
		{
			//	doit être fait après CheckInsertTable et après CheckInsertTableColumns
			//	et avant CheckRemoveTable
			//	pour que la table existe déjà

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
	
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, DbNullability.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed2",   DbRawType.String,  50, false, DbNullability.Yes);
			
			SqlColumn[] columns = { sql_col_1, sql_col_2, sql_col_3, sql_col_4 };
			sql_builder.RemoveTableColumns ("FbTestTable", columns);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (-4, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		
		[Test] public void Check07InsertData()
		{
			//	doit être fait après CheckRemoveTableColumns
			//	pour que la table existe dans le bon état

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
			
			field = SqlField.CreateConstant ("Test © Copyright 2003", DbRawType.String);
			field.Alias = "StringDynamic";
			fields.Add (field);

			sql_builder.InsertData ("FbTestTable", fields);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (1, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check07InsertDataMore()
		{
			//	doit être fait après CheckRemoveTableColumns
			//	pour que la table existe dans le bon état

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;

			Collections.SqlFields fields = new Collections.SqlFields ();

			SqlField field;
			
			field = SqlField.CreateConstant (200, DbRawType.Int32);
			field.Alias = "Cr_ID";
			fields.Add (field);
			
			field = SqlField.CreateConstant (0, DbRawType.Int32);
			field.Alias = "Cr_REV";
			fields.Add (field);
			
			field = SqlField.CreateConstant ("First line...", DbRawType.String);
			field.Alias = "StringDynamic";
			fields.Add (field);

			sql_builder.InsertData ("FbTestTable", fields);
			sql_builder.AppendMore ();
			
			fields.Clear ();
			field = SqlField.CreateConstant (201, DbRawType.Int32);
			field.Alias = "Cr_ID";
			fields.Add (field);
			
			field = SqlField.CreateConstant (0, DbRawType.Int32);
			field.Alias = "Cr_REV";
			fields.Add (field);
			
			field = SqlField.CreateConstant ("Second line...", DbRawType.String);
			field.Alias = "StringDynamic";
			fields.Add (field);
			
			sql_builder.InsertData ("FbTestTable", fields);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			
			int result;
			
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			IDbTransaction transaction = command.Transaction;
			
			Assert.AreEqual (2, result);
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check08RemoveData()
		{
			//	doit être fait après CheckInsertData
			//	pour que la table existe dans le bon état

			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;

			Collections.SqlFields conditions = new Collections.SqlFields ();

			SqlField field1, field2;
			
			field1 = SqlField.CreateConstant (123, DbRawType.Int32);
			field1.Alias = "Cr_ID";
			
			field2 = SqlField.CreateConstant (456, DbRawType.Int32);
			field2.Alias = "Cr_REV";
			
			//	défini la fonction Cr_ID == 123
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("Cr_ID"),
				field1);

			conditions.Add (SqlField.CreateFunction(sql_func));

			//	défini la fonction Cr_REV == 456
			sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName("Cr_REV"),
				field2);

			conditions.Add (SqlField.CreateFunction(sql_func));

			sql_builder.RemoveData ("FbTestTable", conditions);
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (1, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check09RemoveTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			
			sql_builder.RemoveTable ("FbTestTable");
			
			System.Data.IDbCommand command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			command.ExecuteNonQuery ();
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
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
			SqlColumn sql_col_3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, DbNullability.Yes);
			SqlColumn sql_col_4 = new SqlColumn ("StringFixed",   DbRawType.String,  50, false, DbNullability.Yes);
			
			sql_table.Name = "FbTestTable";
			sql_table.Columns.AddRange (new SqlColumn[] { sql_col_1, sql_col_2, sql_col_3, sql_col_4 });
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1, sql_col_2 };
			
			sql_builder.InsertTable (sql_table);
			
			System.Data.IDbCommand command      = sql_builder.Command;
			DbCommandType          command_type = sql_builder.CommandType;
			
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			System.Console.Out.WriteLine ("SQL Command Type: {0}", command_type.ToString ());
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, command_type, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (-2, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
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
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, command_type, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (-1, result);
			
			IDbTransaction transaction = command.Transaction;
			
			transaction.Commit ();
			transaction.Dispose ();
			command.Dispose ();
		}

		[Test] public void Check12InsertArrayTable()
		{
			IDbAbstraction  db_abstraction = DbFactoryTest.CreateDbAbstraction (false);
			ISqlBuilder     sql_builder    = db_abstraction.SqlBuilder;
			ISqlEngine		sql_engine     = db_abstraction.SqlEngine;
			
			sql_builder.AutoClear = true;
			System.Data.IDbCommand command;

			//	supprime la table si jamais
			
			sql_builder.RemoveTable ("FbTestArrayTable");
			
			command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
				
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			
			IDbTransaction transaction = command.Transaction;
			
			try
			{
				command.ExecuteNonQuery ();
				
				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
			catch
			{
				transaction.Rollback ();
				transaction.Dispose ();
				command.Dispose ();
			}
			
			//	crée une table avec des champs binaires variables (BLOB)

			SqlTable  sql_table = new SqlTable ();
			
			SqlColumn sql_col_1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
			SqlColumn sql_col_2 = new SqlColumn ("ArrayDynamic", DbRawType.ByteArray);
			SqlColumn sql_col_3 = new SqlColumn ("ArrayFixed",   DbRawType.ByteArray, DbNullability.Yes);	// les blobs fixes n'existent pas !
			
			sql_table.Name = "FbTestArrayTable";
			sql_table.Columns.Add (sql_col_1);
			sql_table.Columns.Add (sql_col_2);
			sql_table.Columns.Add (sql_col_3);
			sql_table.PrimaryKey = new SqlColumn[] { sql_col_1 };
			
			sql_builder.InsertTable (sql_table);
			
			command	= sql_builder.Command;
			
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
			
			int result;
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (-2, result);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();

			//	y écrit des données

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
			
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out result);
			
			Assert.AreEqual (1, result);
			
			command.Transaction.Commit ();
			//command.Transaction.Dispose ();
			command.Dispose ();

			//	va relire les données écrites dans le BLOB

			SqlSelect sql_select = new SqlSelect ();

			sql_select.Fields.Add (SqlField.CreateName ("Cr_ID"));
			sql_select.Fields.Add (SqlField.CreateName ("ArrayDynamic"));
			sql_select.Tables.Add (SqlField.CreateName ("FbTestArrayTable"));

			//	défini la fonction CR_ID == valeur
			SqlFunction sql_func = new SqlFunction (SqlFunctionType.CompareEqual, 
				SqlField.CreateName ("Cr_ID"),
				field_ID);

			sql_select.Conditions.Add (SqlField.CreateFunction(sql_func));

			//	construit la commande d'extraction
			sql_builder.SelectData (sql_select);

			//	lecture des résultats
			command = sql_builder.Command;
			System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);

			DataSet data_set = new DataSet ();
			command.Transaction = db_abstraction.BeginReadWriteTransaction ();
			sql_engine.Execute (command, sql_builder.CommandType, sql_builder.CommandCount, out data_set);

			Assert.AreEqual (typeof (int),    data_set.Tables[0].Columns[0].DataType);
			Assert.AreEqual (typeof (byte[]), data_set.Tables[0].Columns[1].DataType);
			
			//	récupère l'objet de la première ligne, seconde colonne
			
			DataTable  data_table  = data_set.Tables[0];
			DataColumn blob_column = data_table.Columns[1];
			System.Type blob_type  = blob_column.DataType;

			//	------------- Commentaire plus valable depuis .NET provider 1.7 RC 1. ---------------
			//	Hélas, on n'a pas un type exact pour la colonne ici... J'aurais bien aimé que le
			//	provider retourne un type byte[] ! Mais System.Array est déjà une bonne approximation.
			//	-------------------------------------------------------------------------------------
			
			Assert.AreEqual (typeof (byte[]), blob_type);
			
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
