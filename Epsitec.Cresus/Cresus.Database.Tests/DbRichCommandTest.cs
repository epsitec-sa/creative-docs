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
			
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[3].ParentTableName);
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[4].ParentTableName);
			
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_a.Name, db_table_a.Columns.Count);
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_b.Name, db_table_b.Columns.Count);
			
			infrastructure.RegisterNewDbTable (null, db_table_a);
			infrastructure.RegisterNewDbTable (null, db_table_b);
			
			infrastructure.RegisterColumnRelations (null, db_table_a);
			infrastructure.RegisterColumnRelations (null, db_table_b);
			
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
			
			Assertion.AssertEquals (2, ado_table_b.Rows[1]["Personne (ID)"]);
			Assertion.AssertEquals (2, ado_table_b.Rows[2]["Personne (ID)"]);
			
			ado_table_a.Rows[1]["CR_ID"] = 4;
			
			Assertion.AssertEquals (4, ado_table_b.Rows[1]["Personne (ID)"]);
			Assertion.AssertEquals (4, ado_table_b.Rows[2]["Personne (ID)"]);
		}
		
		[Test] public void Check02CreateEmptyDataSet()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			DbType db_type_rev  = infrastructure.ResolveDbType (null, "CR_KeyRevisionType");
			
			Assertion.AssertEquals (5, db_table_a.Columns.Count);
			Assertion.AssertEquals (6, db_table_b.Columns.Count);
			
			Assertion.AssertEquals ("Nom",    db_table_a.Columns[3].Name);
			Assertion.AssertEquals ("Prenom", db_table_a.Columns[4].Name);
			Assertion.AssertEquals (db_type_name.InternalKey,  db_table_a.Columns[3].Type.InternalKey);
			Assertion.AssertEquals (db_type_name.InternalKey,  db_table_a.Columns[4].Type.InternalKey);
			
			Assertion.AssertEquals ("Personne", db_table_b.Columns[3].Name);
			Assertion.AssertEquals (DbColumnClass.RefTupleId, db_table_b.Columns[3].ColumnClass);
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[3].ParentTableName);
			
			Assertion.AssertEquals ("Personne", db_table_b.Columns[4].Name);
			Assertion.AssertEquals (DbColumnClass.RefTupleRevision, db_table_b.Columns[4].ColumnClass);
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[4].ParentTableName);
			
			Assertion.AssertEquals ("Ville", db_table_b.Columns[5].Name);
			Assertion.AssertEquals (db_type_name.InternalKey, db_table_b.Columns[5].Type.InternalKey);
			
			DbRichCommand command = new DbRichCommand ();
			
			command.Tables.Add (db_table_a);
			command.Tables.Add (db_table_b);
			command.CreateEmptyDataSet (infrastructure);
			
			foreach (System.Data.DataRelation relation in command.DataSet.Relations)
			{
				for (int i = 0; i < relation.ChildColumns.Length; i++)
				{
					System.Console.Out.WriteLine ("{0}.{1} -> {2}.{3}", relation.ChildTable.TableName, relation.ChildColumns[i].ColumnName, relation.ParentTable.TableName, relation.ParentColumns[i].ColumnName);
				}
			}
		}
		
		[Test] public void Check03CreateNewRow()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			DbType db_type_rev  = infrastructure.ResolveDbType (null, "CR_KeyRevisionType");
			
			DbRichCommand command = new DbRichCommand ();
			
			System.Data.DataRow row_1;
			System.Data.DataRow row_2;
			System.Data.DataRow row_3;
			
			command.Tables.Add (db_table_a);
			command.Tables.Add (db_table_b);
			command.CreateEmptyDataSet (infrastructure);
			command.CreateNewRow ("Personnes", out row_1);
			command.CreateNewRow ("Personnes", out row_2);
			command.CreateNewRow ("Domiciles", out row_3);
			
			DbKey k1 = new DbKey (row_1);
			DbKey k2 = new DbKey (row_2);
			DbKey k3 = new DbKey (row_3);
			
			Assertion.AssertEquals (DbRowStatus.Live, k1.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k2.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k3.Status);
			
			Assertion.AssertEquals (k1.Id + 1, k2.Id);
			Assertion.AssertEquals (k1.Id + 2, k3.Id);
			Assertion.Assert (DbKey.CheckTemporaryId (k1.Id));
		}
		
		[Test] public void Check99UnregisterDbTables()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
			infrastructure.UnregisterDbTable (null, db_table_a);
			infrastructure.UnregisterDbTable (null, db_table_b);
		}
	}
}
