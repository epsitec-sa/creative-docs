using NUnit.Framework;
using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database
{
	[TestFixture] public class DbRichCommandTest
	{
		[Test] public void Check01Select()
		{
			//	Tout ceci est provisoire !!! Les structures SQL ne devraient pas �tre expos�es, seulement
			//	leur variante Db... neutre. Cela va certainement migrer dans DbInfrastructure.
			
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			ISqlBuilder sql_builder = infrastructure.SqlBuilder;
			ISqlEngine  sql_engine  = infrastructure.SqlEngine;
			
			DbTable db_table_a = infrastructure.CreateDbTable ("Personnes", DbElementCat.UserDataManaged, DbRevisionMode.Disabled);
			DbTable db_table_b = infrastructure.CreateDbTable ("Domiciles", DbElementCat.UserDataManaged, DbRevisionMode.Disabled);
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			DbType db_type_npa  = infrastructure.ResolveDbType (null, "CR_KeyStatusType");
			
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Nom", db_type_name));
			db_table_a.Columns.Add (infrastructure.CreateColumn ("Prenom", db_type_name));
			
			db_table_b.Columns.AddRange (infrastructure.CreateRefColumns ("Personne", "Personnes", Nullable.Yes));
			db_table_b.Columns.Add (infrastructure.CreateColumn ("Ville", db_type_name));
			db_table_b.Columns.Add (infrastructure.CreateColumn ("NPA", db_type_npa, Nullable.No));
			
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[2].ParentTableName);
			
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
			
			DbRichCommand command = new DbRichCommand (infrastructure);
			
			sql_builder.SelectData (select_a);
			System.Data.IDbCommand command_a = sql_builder.Command;
			
			sql_builder.SelectData (select_b);
			System.Data.IDbCommand command_b = sql_builder.Command;
			
			command.Commands.Add (command_a);
			command.Commands.Add (command_b);
			
			command.Tables.Add (db_table_a);
			command.Tables.Add (db_table_b);
			
			infrastructure.Execute (null, command);
			
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
			
			System.Data.DataRow row_p1, row_p2, row_p3;
			System.Data.DataRow row_d1, row_d2, row_d3, row_d4;
			
			command.CreateNewRow ("Personnes", out row_p1);
			command.CreateNewRow ("Personnes", out row_p2);
			command.CreateNewRow ("Personnes", out row_p3);
			command.CreateNewRow ("Domiciles", out row_d1);
			command.CreateNewRow ("Domiciles", out row_d2);
			command.CreateNewRow ("Domiciles", out row_d3);
			command.CreateNewRow ("Domiciles", out row_d4);
			
			row_p1.BeginEdit (); row_p1["Nom"] = "Arnaud";   row_p1["Prenom"] = "Pierre"; row_p1.EndEdit ();
			row_p2.BeginEdit (); row_p2["Nom"] = "Dumoulin"; row_p2["Prenom"] = "Denis";  row_p2.EndEdit ();
			row_p3.BeginEdit (); row_p3["Nom"] = "Roux";     row_p3["Prenom"] = "Daniel"; row_p3.EndEdit ();
			
			row_d1.BeginEdit (); row_d1["Ville"] = "Yverdon";  row_d1["NPA"] = 1400; row_d1["Personne"] = row_p1["CR_ID"]; row_d1.EndEdit ();
			row_d2.BeginEdit (); row_d2["Ville"] = "Morges";   row_d2["NPA"] = 1110; row_d2["Personne"] = row_p2["CR_ID"]; row_d2.EndEdit ();
			row_d3.BeginEdit (); row_d3["Ville"] = "Saverne";  row_d3["NPA"] = 9999; row_d3["Personne"] = row_p2["CR_ID"]; row_d3.EndEdit ();
			row_d4.BeginEdit (); row_d4["Ville"] = "Crissier"; row_d4["NPA"] = 1023; row_d4["Personne"] = row_p3["CR_ID"]; row_d4.EndEdit ();
			
			DbInfrastructureTest.DisplayDataSet (infrastructure, ado_table_a.TableName, ado_table_a);
			DbInfrastructureTest.DisplayDataSet (infrastructure, ado_table_b.TableName, ado_table_b);
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Commit ();
			}
			
			Assertion.AssertEquals (1, ado_table_a.Rows[1]["CR_ID"]);
			Assertion.AssertEquals (1, ado_table_b.Rows[1]["Personne"]);
			
			ado_table_a.Rows[1]["CR_ID"] = 100;
			
			Assertion.AssertEquals (100, ado_table_b.Rows[1]["Personne"]);
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check02CreateEmptyDataSet()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
			DbType db_type_name = infrastructure.ResolveDbType (null, "CR_NameType");
			DbType db_type_id   = infrastructure.ResolveDbType (null, "CR_KeyIdType");
			
			Assertion.AssertEquals (4, db_table_a.Columns.Count);
			Assertion.AssertEquals (5, db_table_b.Columns.Count);
			
			Assertion.AssertEquals ("Nom",    db_table_a.Columns[2].Name);
			Assertion.AssertEquals ("Prenom", db_table_a.Columns[3].Name);
			Assertion.AssertEquals (db_type_name.InternalKey,  db_table_a.Columns[2].Type.InternalKey);
			
			Assertion.AssertEquals ("Personne", db_table_b.Columns[2].Name);
			Assertion.AssertEquals (DbColumnClass.RefId, db_table_b.Columns[2].ColumnClass);
			Assertion.AssertEquals ("Personnes", db_table_b.Columns[2].ParentTableName);
			
			Assertion.AssertEquals ("Ville", db_table_b.Columns[3].Name);
			Assertion.AssertEquals (db_type_name.InternalKey, db_table_b.Columns[3].Type.InternalKey);
			
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
		
		[Test] public void Check03CreateNewRow()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
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
			
			Assertion.AssertEquals (DbRowStatus.Live, k1.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k2.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k3.Status);
			
			Assertion.AssertEquals (k1.Id + 1, k2.Id);
			Assertion.AssertEquals (k1.Id + 2, k3.Id);
			Assertion.Assert (DbKey.CheckTemporaryId (k1.Id));
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Rollback ();
			}
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check04CreateNewRowBatch()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			
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
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
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
			
			Assertion.AssertEquals (DbRowStatus.Live, k1.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k2.Status);
			Assertion.AssertEquals (DbRowStatus.Live, k3.Status);
			
			Assertion.AssertEquals (k1.Id + 1, k2.Id);
			Assertion.AssertEquals (k1.Id + 2, k3.Id);
			Assertion.Assert (DbKey.CheckTemporaryId (k1.Id));
			
			command.DeleteRow (row_2);
			command.DeleteRow (row_3);
			
			using (DbTransaction transaction = infrastructure.BeginTransaction ())
			{
				command.UpdateRealIds (transaction);
				command.UpdateTables (transaction);
				transaction.Rollback ();
			}
			
			infrastructure.Dispose ();
		}
		
#if false
		[Test] public void XxxTest()
		{
			//	Test synth�tique pour soumission du probl�me � Carlos Guzman Alvarez.
			
			FbConnection connection = new FbConnection (@"User=sysdba;Password=masterkey;" +
				/**/									@"DataSource=localhost;Database=C:\Program Files\Firebird15\Data\Epsitec\fiche.firebird;Port=3050;Dialect=3;Packet Size=8192;" +
				/**/									@"ServerType=1;Charset=UNICODE_FSS;Role=;Pooling=true;Connection Lifetime=10;");
			
#if false
			connection.Open();
           
			string sql = "insert into U_DOMICILES_35 (CR_ID, CR_REV, CR_STAT, U_VILLE, U_NPA) values (@CR_ID, @CR_REV, @CR_STAT, @U_VILLE, @U_NPA)";

			FbCommand cmd = new FbCommand(sql, connection);
			cmd.Parameters.Add("@CR_ID", FbDbType.BigInt).Value = 1;
			cmd.Parameters.Add("@CR_REV", FbDbType.Integer).Value = 2;
			cmd.Parameters.Add("@CR_STAT", FbDbType.SmallInt).Value = 3;
			cmd.Parameters.Add("@U_VILLE", FbDbType.VarChar, 40).Value = "aa";
			cmd.Parameters.Add("@U_NPA", FbDbType.Integer).Value = null;

			cmd.ExecuteNonQuery();

			connection.Close();

#else
			FbCommand    command    = connection.CreateCommand ();
			
			command.CommandType = System.Data.CommandType.Text;
			command.CommandText = "SELECT * FROM U_DOMICILES_35 WHERE 0=1;";
			
			FbDataAdapter    adapter = new FbDataAdapter (command);
			FbCommandBuilder builder = new FbCommandBuilder (adapter);
			
			connection.Open ();
			
			FbTransaction transaction = connection.BeginTransaction (System.Data.IsolationLevel.RepeatableRead);
			
			System.Data.DataSet set = new System.Data.DataSet ();
			command.Transaction = transaction;
			adapter.Fill (set);
			transaction.Commit ();
			
			System.Data.DataTable table = set.Tables[0];
			System.Data.DataRow   row   = table.NewRow ();
			
			row["CR_ID"] = 1000;
			row["CR_REV"] = 0;
			row["CR_STAT"] = 0;
			row["REF_PERSONNE_ID"] = System.DBNull.Value;
			row["REF_PERSONNE_REV"] = System.DBNull.Value;
			row["U_VILLE"] = "Yverdon";
			row["U_NPA"] = System.DBNull.Value;		//	<- devrait �tre rejet�
			
			table.Rows.Add (row);
			
			transaction = connection.BeginTransaction (System.Data.IsolationLevel.RepeatableRead);
			command.Transaction = transaction;
			adapter.Update (set);
			transaction.Rollback ();

			connection.Close ();
#endif
		}
#endif
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInternalFillDataSetEx()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			DbRichCommand    command        = new DbRichCommand (infrastructure);
			
			command.InternalFillDataSet (DbAccess.Empty, null, null);
			
			infrastructure.Dispose ();
		}
		
		[Test] public void Check99UnregisterDbTables()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			
			DbTable db_table_a = infrastructure.ResolveDbTable (null, "Personnes");
			DbTable db_table_b = infrastructure.ResolveDbTable (null, "Domiciles");
			
			infrastructure.UnregisterDbTable (null, db_table_a);
			infrastructure.UnregisterDbTable (null, db_table_b);
			
			infrastructure.Dispose ();
		}
	}
}
