using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture] public class DbRichCommandTest
	{
		[Test] public void Check01Select()
		{
			//	Tout ceci est provisoire !!! Les structures SQL ne devraient pas être exposées, seulement
			//	leur variante Db... neutre. Cela va certainement migrer dans DbInfrastructure.
			
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			ISqlBuilder sql_builder = infrastructure.SqlBuilder;
			ISqlEngine  sql_engine  = infrastructure.SqlEngine;
			
			DbTable db_table_a = infrastructure.CreateDbTable ("Personnes", DbElementCat.UserDataManaged);
			DbTable db_table_b = infrastructure.CreateDbTable ("Domiciles", DbElementCat.UserDataManaged);
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			DbType db_type_rev  = infrastructure.ResolveDbType (null, "CR_KeyRevisionType");
			
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Nom", db_type_name));
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Prenom", db_type_name));
			
			db_table_b.Columns.AddRange (infrastructure.CreateRefColumns ("Personne", "Personnes", DbKeyMatchMode.ExactIdRevision));
			db_table_b.Columns.Add (infrastructure.CreateColumn ("Ville", db_type_name));
			
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_a.Name, db_table_a.Columns.Count);
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_b.Name, db_table_b.Columns.Count);
			
			infrastructure.RegisterNewDbTable (null, db_table_a);
			infrastructure.RegisterNewDbTable (null, db_table_b);
			
			SqlSelect select_a = new SqlSelect ();
			SqlSelect select_b = new SqlSelect ();
			
			select_a.Fields.Add (SqlField.CreateAll ());
			select_b.Fields.Add (SqlField.CreateAll ());
			
			select_a.Tables.Add ("A", SqlField.CreateName (db_table_a.CreateSqlName ()));
			select_b.Tables.Add ("B", SqlField.CreateName (db_table_b.CreateSqlName ()));
			
			DbTransaction transaction;
			DbRichCommand command = new DbRichCommand ();
			
			using (transaction = infrastructure.BeginTransaction ())
			{
				sql_builder.SelectData (select_a);
				command.Commands.Add (sql_builder.Command);
				
				sql_builder.SelectData (select_b);
				command.Commands.Add (sql_builder.Command);
				
				command.Transaction = transaction.Transaction;
				command.Tables.Add (db_table_a);
				command.Tables.Add (db_table_b);
				
				sql_engine.Execute (command);
				transaction.Commit ();
			}
			
			System.Console.Out.WriteLine ("Tables : {0}", command.DataSet.Tables.Count);
			foreach (System.Data.DataTable table in command.DataSet.Tables)
			{
				System.Console.Out.WriteLine ("Table {0} has {1} columns:", table.TableName, table.Columns.Count);
				foreach (System.Data.DataColumn column in table.Columns)
				{
					System.Console.Out.WriteLine ("  {0} {1} {2}", column.ColumnName, column.Unique, column.DataType);
				}
			}
			
			System.Data.DataTable ado_table_a = command.DataSet.Tables["Personnes"];
			System.Data.DataTable ado_table_b = command.DataSet.Tables["Domiciles"];
			
			ado_table_a.Rows.Add (new object[] { 1, 0, 0, "Arnaud",   "Pierre" });
			ado_table_a.Rows.Add (new object[] { 2, 0, 0, "Dumoulin", "Denis" });
			ado_table_a.Rows.Add (new object[] { 3, 0, 0, "Roux",     "Daniel" });
			
			ado_table_b.Rows.Add (new object[] { 1, 0, 0, 1, 0, "Yverdon-les-Bains" });
			ado_table_b.Rows.Add (new object[] { 2, 0, 0, 2, 0, "Morges" });
			ado_table_b.Rows.Add (new object[] { 3, 0, 0, 2, 0, "Saverne" });
			ado_table_b.Rows.Add (new object[] { 4, 0, 0, 3, 0, "Crissier" });
			
			DbInfrastructureTest.DisplayDataSet (infrastructure, ado_table_a.TableName, ado_table_a);
			DbInfrastructureTest.DisplayDataSet (infrastructure, ado_table_b.TableName, ado_table_b);
			
			using (transaction = infrastructure.BeginTransaction ())
			{
				command.Transaction = transaction.Transaction;
				command.UpdateTables ();
				transaction.Commit ();
			}
			
			infrastructure.UnregisterDbTable (null, db_table_a);
			infrastructure.UnregisterDbTable (null, db_table_b);
		}
		
		[Test] public void Check02CreateEmptyDataSet()
		{
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird15\Data\Epsitec\FICHE.FIREBIRD");
			}
			catch {}
			
			DbAccess db_access = DbInfrastructure.CreateDbAccess ("fiche");
			DbInfrastructure infrastructure = new DbInfrastructure ();
			
			infrastructure.CreateDatabase (db_access);
			
			DbTable db_table_a = infrastructure.CreateDbTable ("Personnes", DbElementCat.UserDataManaged);
			DbTable db_table_b = infrastructure.CreateDbTable ("Domiciles", DbElementCat.UserDataManaged);
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			DbType db_type_rev  = infrastructure.ResolveDbType (null, "CR_KeyRevisionType");
			
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Nom", db_type_name));
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Prenom", db_type_name));
			
			db_table_b.Columns.AddRange (infrastructure.CreateRefColumns ("Personne", "Personnes", DbKeyMatchMode.ExactIdRevision));
			db_table_b.Columns.Add (infrastructure.CreateColumn ("Ville", db_type_name));
			
			infrastructure.RegisterNewDbTable (null, db_table_a);
			infrastructure.RegisterNewDbTable (null, db_table_b);
			
			DbRichCommand command = new DbRichCommand ();
			
			command.Tables.Add (db_table_a);
			command.Tables.Add (db_table_b);
			command.CreateEmptyDataSet (infrastructure);
			
			infrastructure.UnregisterDbTable (null, db_table_a);
			infrastructure.UnregisterDbTable (null, db_table_b);
		}
	}
}
