using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbInfrastructureTest
	{
		[Test] public void CheckCreateDatabase()
		{
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird15\Data\Epsitec\FICHE.FIREBIRD");
			}
			catch {}
			
			//	Si on n'arrive pas à détruire le fichier, c'est que le serveur Firebird a peut-être
			//	encore conservé un handle ouvert; par expérience, cela peut prendre ~10s jusqu'à ce
			//	que la fermeture soit effective.
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
				
				infrastructure.CreateDatabase (db_access);
				
				DbTable table;
				
				//	Vérifie que les tables principales ont été créées correctement :
				
				table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				
				Assertion.AssertNotNull (table);
				Assertion.Assert (table.InternalKey.Id == 1);
				Assertion.Assert (table.Columns.Count == 8);
				
				table = infrastructure.ResolveDbTable (null, "CR_COLUMN_DEF");
				
				Assertion.AssertNotNull (table);
				Assertion.Assert (table.InternalKey.Id == 2);
				Assertion.Assert (table.Columns.Count == 9);
				
				table = infrastructure.ResolveDbTable (null, "CR_TYPE_DEF");
				
				Assertion.AssertNotNull (table);
				Assertion.Assert (table.InternalKey.Id == 3);
				Assertion.Assert (table.Columns.Count == 7);
				
				Assertion.AssertEquals (0, infrastructure.CountMatchingRows (null, "CR_COLUMN_DEF", "CR_NAME", DbSqlStandard.CreateSimpleSqlName ("MyColumn")));
				Assertion.AssertEquals (4, infrastructure.CountMatchingRows (null, "CR_COLUMN_DEF", "CR_NAME", "CR_INFO"));
				
				//	Vérifie que les statements UPDATE ... lors de la création ont bien passé,
				//	puis vérifie aussi que l'incrément de l'ID fonctionne correctement.
				
				table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				
				Assertion.AssertEquals (6L, infrastructure.NewRowIdInTable (null, table.InternalKey, 2));
				Assertion.AssertEquals (8L, infrastructure.NewRowIdInTable (null, table.InternalKey, 0));
				Assertion.AssertEquals (8L, infrastructure.NewRowIdInTable (null, table.InternalKey, 0));
			}
		}
		
		[Test] public void CheckAttachDatabase()
		{
			using (DbInfrastructure infrastructure = this.GetInfrastructureFromBase ("fiche", true))
			{
				Assertion.AssertNotNull (infrastructure);
				
				DbTable db_table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				DbType  db_type1 = infrastructure.ResolveDbType (null, "CR.Name");
				DbType  db_type2 = infrastructure.ResolveDbType (null, "CR.Name");
				DbType  db_type3 = infrastructure.ResolveDbType (null, "CR.KeyId");
				
				Assertion.AssertNotNull (db_table);
				Assertion.AssertNotNull (db_type1);
				
				Assertion.AssertEquals (db_type1, db_type2);
				
				Assertion.AssertEquals (8, db_table.Columns.Count);
				Assertion.AssertEquals ("CR_ID", db_table.Columns[0].Name);
				Assertion.AssertEquals (db_type1, db_table.Columns["CR_NAME"].Type);
			}
		}
		
		[Test] public void CheckCreateDbTable()
		{
			//	Ce test ne marche que pour une base qui est propre (i.e. qui vient d'être
			//	créée par CheckCreateDatabase).
			
			using (DbInfrastructure infrastructure = this.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table1 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.UserDataManaged);
				infrastructure.RegisterNewDbTable (db_table1);
				
				DbTable db_table2 = infrastructure.ResolveDbTable (null, "SimpleTest");
				
				Assertion.AssertNotNull (db_table2);
				
				Assertion.AssertEquals (db_table1.Name,					db_table2.Name);
				Assertion.AssertEquals (db_table1.Category,				db_table2.Category);
				Assertion.AssertEquals (db_table1.PrimaryKeys.Count,	db_table2.PrimaryKeys.Count);
				Assertion.AssertEquals (db_table1.PrimaryKeys[0].Name,	db_table2.PrimaryKeys[0].Name);
				Assertion.AssertEquals (db_table1.Columns.Count,		db_table2.Columns.Count);
			}
		}
		
		[Test] [ ExpectedException (typeof (DbException)) ] public void CheckCreateDbTableEx1()
		{
			//	Exécuter deux fois une création de table va nécessairement générer une exception.
			//	Il faut exécuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = this.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.UserDataManaged);
				infrastructure.RegisterNewDbTable (db_table);
				infrastructure.ResolveDbTable (null, db_table.Name);
			}
		}
		
		private DbInfrastructure GetInfrastructureFromBase(string name, bool debug_attach)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess (name);
			
			if (debug_attach)
			{
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (this.DisplayDataSet);
				infrastructure.AttachDatabase (db_access);
			}
			else
			{
				infrastructure.AttachDatabase (db_access);
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (this.DisplayDataSet);
			}
			
			return infrastructure;
		}
		
		private void DisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table)
		{
			this.display.AddTable (name, table);
			this.display.ShowWindow ();
		}
		
		UserInterface.Debugging.DataSetDisplay	display = new UserInterface.Debugging.DataSetDisplay ();
	}
}
