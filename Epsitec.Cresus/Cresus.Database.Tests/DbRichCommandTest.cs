using NUnit.Framework;
using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database
{
#if false
	
	[TestFixture] public class DbRichCommandTest
	{
		[Test]
		public void Check01Select()
		{
			//	Tout ceci est provisoire !!! Les structures SQL ne devraient pas �tre expos�es, seulement
			//	leur variante Db... neutre. Cela va certainement migrer dans DbInfrastructure.
			
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			ISqlBuilder sql_builder = infrastructure.DefaultSqlBuilder;
			ISqlEngine  sql_engine  = infrastructure.DefaultSqlEngine;
			
			DbTable db_table_a = infrastructure.CreateDbTable ("Personnes", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);
			DbTable db_table_b = infrastructure.CreateDbTable ("Domiciles", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges);
			
			DbTypeDef db_type_name = infrastructure.ResolveDbType (Tags.TypeName);
			DbTypeDef db_type_id   = infrastructure.ResolveDbType (Tags.TypeKeyId);
			DbTypeDef db_type_npa  = infrastructure.ResolveDbType (Tags.TypeKeyStatus);

			db_table_a.Columns.Add (DbTable.CreateUserDataColumn ("Nom", db_type_name));
			db_table_a.Columns.Add (DbTable.CreateUserDataColumn ("Prenom", db_type_name));

			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				db_table_b.Columns.Add (DbTable.CreateRefColumn (transaction, infrastructure, "Personne", "Personnes", DbNullability.Yes));
				db_table_b.Columns.Add (DbTable.CreateUserDataColumn ("Ville", db_type_name));
				db_table_b.Columns.Add (DbTable.CreateUserDataColumn ("NPA", db_type_npa));
				
				transaction.Commit ();
			}
			
			Assert.AreEqual ("Personnes", db_table_b.Columns[3].TargetTableName);
			
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_a.Name, db_table_a.Columns.Count);
			System.Console.Out.WriteLine ("Table {0} has {1} columns.", db_table_b.Name, db_table_b.Columns.Count);

			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				infrastructure.RegisterNewDbTable (transaction, db_table_a);
				infrastructure.RegisterNewDbTable (transaction, db_table_b);

				infrastructure.RegisterColumnRelations (transaction, db_table_a);
				infrastructure.RegisterColumnRelations (transaction, db_table_b);
				
				transaction.Commit ();
			}
			
			DbSelectCondition condition_a = new DbSelectCondition (DbSelectRevision.LiveActive);
			DbSelectCondition condition_b = new DbSelectCondition (DbSelectRevision.LiveActive);

			DbTable[] tables = new DbTable[] { db_table_a, db_table_b };
			DbSelectCondition[] conditions = new DbSelectCondition[] { condition_a, condition_b };

			DbRichCommand command;

			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command = DbRichCommand.CreateFromTables (infrastructure, transaction, tables, conditions);
				
				transaction.Commit ();
			}
			
			System.Console.Out.WriteLine ("Tables : {0}", command.DataSet.Tables.Count);
			
			Assert.AreEqual (2, command.DataSet.Tables.Count);
			Assert.IsTrue (command.DataSet.Tables["Personnes"].Columns[Tags.ColumnId].Unique);
			Assert.IsTrue (command.DataSet.Tables["Domiciles"].Columns[Tags.ColumnId].Unique);
			
			foreach (System.Data.DataTable table in command.DataSet.Tables)
			{
				System.Console.Out.WriteLine ("Table {0} has {1} columns:", table.TableName, table.Columns.Count);
				foreach (System.Data.DataColumn column in table.Columns)
				{
					System.Console.Out.WriteLine ("  {0} {1} {2}", column.ColumnName, column.Unique ? "- unique -" : "-", column.DataType);
				}
			}
			
			System.Data.DataTable ado_table_a = command.DataSet.Tables["Personnes"];
			System.Data.DataTable ado_table_b = command.DataSet.Tables["Domiciles"];
			
			System.Data.DataRow row_p1, row_p2, row_p3;
			System.Data.DataRow row_d1, row_d2, row_d3, row_d4;

			row_p1 = command.CreateRow ("Personnes");
			row_p2 = command.CreateRow ("Personnes");
			row_p3 = command.CreateRow ("Personnes");

			row_d1 = command.CreateRow ("Domiciles");
			row_d2 = command.CreateRow ("Domiciles");
			row_d3 = command.CreateRow ("Domiciles");
			row_d4 = command.CreateRow ("Domiciles");
			
			row_p1.BeginEdit (); row_p1["Nom"] = "Arnaud";   row_p1["Prenom"] = "Pierre"; row_p1.EndEdit ();
			row_p2.BeginEdit (); row_p2["Nom"] = "Dumoulin"; row_p2["Prenom"] = "Denis";  row_p2.EndEdit ();
			row_p3.BeginEdit (); row_p3["Nom"] = "Roux";     row_p3["Prenom"] = "Daniel"; row_p3.EndEdit ();
			
			row_d1.BeginEdit (); row_d1["Ville"] = "Yverdon";  row_d1["NPA"] = 1400; row_d1["Personne"] = row_p1["CR_ID"]; row_d1.EndEdit ();
			row_d2.BeginEdit (); row_d2["Ville"] = "Morges";   row_d2["NPA"] = 1110; row_d2["Personne"] = row_p2["CR_ID"]; row_d2.EndEdit ();
			row_d3.BeginEdit (); row_d3["Ville"] = "Saverne";  row_d3["NPA"] = 9999; row_d3["Personne"] = row_p2["CR_ID"]; row_d3.EndEdit ();
			row_d4.BeginEdit (); row_d4["Ville"] = "Crissier"; row_d4["NPA"] = 1023; row_d4["Personne"] = row_p3["CR_ID"]; row_d4.EndEdit ();
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.SaveTables (transaction);
				transaction.Commit ();
			}
			
			Assert.AreEqual (1000000000002L, ado_table_a.Rows[1]["CR_ID"]);
			Assert.AreEqual (1000000000002L, ado_table_b.Rows[1]["Personne"]);
			
			ado_table_a.Rows[1]["CR_ID"] = 100;
			
			Assert.AreEqual (100, ado_table_b.Rows[1]["Personne"]);
			
			infrastructure.Dispose ();
		}

		[Test]
		public void Check02CreateEmptyDataSet()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable ("Domiciles");
			
			DbTypeDef db_type_name = infrastructure.ResolveDbType (Tags.TypeName);
			DbTypeDef db_type_id   = infrastructure.ResolveDbType (Tags.TypeKeyId);
			
			Assert.AreEqual (5, db_table_a.Columns.Count);
			Assert.AreEqual (6, db_table_b.Columns.Count);
			
			Assert.AreEqual ("Nom",    db_table_a.Columns[3].Name);
			Assert.AreEqual ("Prenom", db_table_a.Columns[4].Name);
			Assert.AreEqual (db_type_name.Key,  db_table_a.Columns[3].Type.Key);
			
			Assert.AreEqual ("Personne", db_table_b.Columns[3].Name);
			Assert.AreEqual (DbColumnClass.RefId, db_table_b.Columns[3].ColumnClass);
			Assert.AreEqual ("Personnes", db_table_b.Columns[3].TargetTableName);
			
			Assert.AreEqual ("Ville", db_table_b.Columns[4].Name);
			Assert.AreEqual (db_type_name.Key, db_table_b.Columns[4].Type.Key);
			
			DbRichCommand command = DbRichCommand.CreateFromTables (infrastructure, null, db_table_a, db_table_b);
			
			foreach (System.Data.DataRelation relation in command.DataSet.Relations)
			{
				for (int i = 0; i < relation.ChildColumns.Length; i++)
				{
					System.Console.Out.WriteLine ("{0}.{1} -> {2}.{3}", relation.ChildTable.TableName, relation.ChildColumns[i].ColumnName, relation.ParentTable.TableName, relation.ParentColumns[i].ColumnName);
				}
			}
			
			infrastructure.Dispose ();
		}
		
#if false		
		[Test] public void Check03CreateNewRow()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable ("Domiciles");
			
			System.Data.DataRow row_1;
			System.Data.DataRow row_2;
			System.Data.DataRow row_3;
			
			DbRichCommand command = DbRichCommand.CreateFromTables (infrastructure, null, db_table_a, db_table_b);
			
			command.CreateNewRow ("Personnes", out row_1); row_1["Nom"] = "Toto"; row_1["Prenom"] = "Foo";
			command.CreateNewRow ("Personnes", out row_2); row_2["Nom"] = "Titi"; row_2["Prenom"] = "Bar";
			command.CreateNewRow ("Domiciles", out row_3); row_3["Ville"] = "New York"; row_3["NPA"] = 12345;
			
			DbKey k1 = new DbKey (row_1);
			DbKey k2 = new DbKey (row_2);
			DbKey k3 = new DbKey (row_3);
			
			Assert.AreEqual (DbRowStatus.Live, k1.Status);
			Assert.AreEqual (DbRowStatus.Live, k2.Status);
			Assert.AreEqual (DbRowStatus.Live, k3.Status);
			
			Assert.AreEqual (k1.Id + 1, k2.Id.Value);
			Assert.AreEqual (k1.Id + 2, k3.Id.Value);
			Assert.IsTrue (DbKey.CheckTemporaryId (k1.Id));
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateLogIds ();
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Rollback ();
			}
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check04CreateNewRowBatch()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			
			DbRichCommand command = DbRichCommand.CreateFromTables (infrastructure, null, db_table_a);
			
			int num = 1000;
			
			System.Diagnostics.Debug.WriteLine ("Creating " + num.ToString() + " rows.");
			for (int i = 0; i < num; i++)
			{
				System.Data.DataRow row;
				command.CreateNewRow ("Personnes", out row);
				row["Nom"] = "Toto" + i.ToString ();
				row["Prenom"] = "Foo" + i.ToString ();
			}
			System.Diagnostics.Debug.WriteLine ("-> created in DataTable");
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateLogIds ();
				command.UpdateRealIds (transaction);
				System.Diagnostics.Debug.WriteLine ("-> using real IDs");
				command.UpdateTables (transaction);
				System.Diagnostics.Debug.WriteLine ("-> sent to database");
				transaction.Rollback ();
				System.Diagnostics.Debug.WriteLine ("-> rolled back");
			}
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check05CreateNewRowAndDelete()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable ("Domiciles");
			
			System.Data.DataRow row_1;
			System.Data.DataRow row_2;
			System.Data.DataRow row_3;
			
			DbRichCommand command = DbRichCommand.CreateFromTables (infrastructure, null, db_table_a, db_table_b);
			
			command.CreateNewRow ("Personnes", out row_1); row_1["Nom"] = "Toto"; row_1["Prenom"] = "Foo";
			command.CreateNewRow ("Personnes", out row_2); row_2["Nom"] = "Titi"; row_2["Prenom"] = "Bar";
			command.CreateNewRow ("Domiciles", out row_3); row_3["Ville"] = "New York"; row_3["NPA"] = 12345;
			
			DbKey k1 = new DbKey (row_1);
			DbKey k2 = new DbKey (row_2);
			DbKey k3 = new DbKey (row_3);
			
			Assert.AreEqual (DbRowStatus.Live, k1.Status);
			Assert.AreEqual (DbRowStatus.Live, k2.Status);
			Assert.AreEqual (DbRowStatus.Live, k3.Status);
			
			Assert.AreEqual (k1.Id + 1, k2.Id.Value);
			Assert.AreEqual (k1.Id + 2, k3.Id.Value);
			Assert.IsTrue (DbKey.CheckTemporaryId (k1.Id));
			
			command.DeleteExistingRow (row_2);
			command.DeleteExistingRow (row_3);
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateLogIds ();
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Rollback ();
			}
			
			infrastructure.Dispose ();
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void Check06InternalFillDataSetEx()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			DbRichCommand    command        = new DbRichCommand (infrastructure);
			
			command.InternalFillDataSet (DbAccess.Empty, null, null);
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check07ReplaceTables()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			
			DbRichCommand command = DbRichCommand.CreateFromTables (infrastructure, null, db_table_a);
			
			System.Data.DataRow row;
			
			command.CreateNewRow ("Personnes", out row);
			
			row.BeginEdit ();
			row["Prenom"] = "Albert";
			row["Nom"]    = "Einstein";
			row.EndEdit ();
			
			command.CreateNewRow ("Personnes", out row);
			
			row.BeginEdit ();
			row["Prenom"] = "Jean";
			row["Nom"]    = "Dupont";
			row.EndEdit ();
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateLogIds ();
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Commit ();
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("A: Inserted {0}, Updated {1}, Deleted {2}.", command.ReplaceStatisticsInsertCount, command.ReplaceStatisticsUpdateCount, command.ReplaceStatisticsDeleteCount));
			
			row = command.DataSet.Tables[0].Rows[1];
			
			row.BeginEdit ();
			row["Nom"] = "Dupond";
			row.EndEdit ();
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.ReplaceTables (transaction);
				transaction.Commit ();
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("B: Inserted {0}, Updated {1}, Deleted {2}.", command.ReplaceStatisticsInsertCount, command.ReplaceStatisticsUpdateCount, command.ReplaceStatisticsDeleteCount));
			
			command.DataSet.Tables[0].Rows.Clear ();
			command.DataSet.Tables[0].Rows.Add (new object[] { 1000000000003L, 0, 123456L, "Walz", "Michael" });
			command.DataSet.Tables[0].Rows.Add (new object[] { 1000000000004L, 0, 123456L, "Rabout", "Yves" });
			command.DataSet.Tables[0].Rows.Add (new object[] { 1000000010000L, 0, 123456L, "Alleyn", "Christian" });
			command.DataSet.Tables[0].Rows.Add (new object[] { 1000000010001L, 0, 123456L, "Dieperink", "Alwin" });
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.ReplaceTables (transaction);
				transaction.Commit ();
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("C: Inserted {0}, Updated {1}, Deleted {2}.", command.ReplaceStatisticsInsertCount, command.ReplaceStatisticsUpdateCount, command.ReplaceStatisticsDeleteCount));
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.ReplaceTables (transaction);
				transaction.Commit ();
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("D: Inserted {0}, Updated {1}, Deleted {2}.", command.ReplaceStatisticsInsertCount, command.ReplaceStatisticsUpdateCount, command.ReplaceStatisticsDeleteCount));
			
			command.DataSet.Tables[0].Rows[1].BeginEdit ();
			command.DataSet.Tables[0].Rows[1]["Nom"] = "Raboud";
			command.DataSet.Tables[0].Rows[1].EndEdit ();
			command.DataSet.Tables[0].Rows[2].Delete ();
			command.DataSet.Tables[0].Rows[3].Delete ();
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.ReplaceTables (transaction);
				transaction.Commit ();
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("E: Inserted {0}, Updated {1}, Deleted {2}.", command.ReplaceStatisticsInsertCount, command.ReplaceStatisticsUpdateCount, command.ReplaceStatisticsDeleteCount));
			
			infrastructure.Dispose ();
			
			Assert.AreEqual (2, command.ReplaceStatisticsInsertCount);
			Assert.AreEqual (4, command.ReplaceStatisticsUpdateCount);
			Assert.AreEqual (2, command.ReplaceStatisticsDeleteCount);
		}
		
		[Test] public void Check99UnregisterDbTables()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable ("Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable ("Domiciles");
			
			infrastructure.UnregisterDbTable (, db_table_a);
			infrastructure.UnregisterDbTable (, db_table_b);
			
			infrastructure.Dispose ();
		}
#endif
	}
#endif
}
