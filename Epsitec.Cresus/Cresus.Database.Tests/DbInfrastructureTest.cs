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
			
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.CreateDatabase (db_access);
			
			DbTable table;
			
			//	Vérifie que les tables principales ont été créées correctement :
			
			table = infrastructure.ResolveDbTable ("CR_TABLE_DEF");
			
			Assertion.AssertNotNull (table);
			Assertion.Assert (table.InternalKey.Id == 1);
			Assertion.Assert (table.Columns.Count == 8);
			
			table = infrastructure.ResolveDbTable ("CR_COLUMN_DEF");
			
			Assertion.AssertNotNull (table);
			Assertion.Assert (table.InternalKey.Id == 2);
			Assertion.Assert (table.Columns.Count == 9);
			
			table = infrastructure.ResolveDbTable ("CR_TYPE_DEF");
			
			Assertion.AssertNotNull (table);
			Assertion.Assert (table.InternalKey.Id == 3);
			Assertion.Assert (table.Columns.Count == 7);
			
			//	Vérifie que les statements UPDATE ... lors de la création ont bien passé,
			//	puis vérifie aussi que l'incrément de l'ID fonctionne correctement.
			
			table = infrastructure.ResolveDbTable ("CR_TABLE_DEF");
			
			Assertion.AssertEquals ( 8L, infrastructure.NewRowIdInTable (table.InternalKey, 2));
			Assertion.AssertEquals (10L, infrastructure.NewRowIdInTable (table.InternalKey, 0));
			Assertion.AssertEquals (10L, infrastructure.NewRowIdInTable (table.InternalKey, 0));
			
			infrastructure.Dispose ();
		}
		
		[Test] public void CheckAttachDatabase()
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.DisplayDataSet = new CallbackDisplayDataSet (this.DisplayDataSet);
			infrastructure.AttachDatabase (db_access);
			
			{
				DbTable db_table = infrastructure.ResolveDbTable ("CR_TABLE_DEF");
				DbType  db_type1 = infrastructure.ResolveDbType ("CR.Name");
				DbType  db_type2 = infrastructure.ResolveDbType ("CR.Name");
				DbType  db_type3 = infrastructure.ResolveDbType ("CR.KeyId");
				
				System.Console.Out.WriteLine ("Table {0} has {1} columns:", db_table.Name, db_table.Columns.Count);
				
				for (int i = 0; i < db_table.Columns.Count; i++)
				{
					DbColumn column = db_table.Columns[i];
					DbType   type   = column.Type;
					
					System.Console.Out.WriteLine ("{0}: {1}/{2} of type {3}.", i, column.Name, column.Caption, type.Name);
				}
				
				Assertion.AssertEquals (db_type1, db_type2);
			}	
			{
				DbColumn column_a = new DbColumn ("CR_ID",  DbNumDef.FromRawType (DbRawType.Int64));
				DbColumn column_b = new DbColumn ("CR_REV", DbNumDef.FromRawType (DbRawType.Int32));
				DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
				DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
				DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
				
				DbTable db_table = new DbTable ("Test");
				
				db_table.PrimaryKey = new DbColumn[] { column_a, column_b };
				db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
				
				infrastructure.RegisterNewDbTable (db_table);
			}
#if false			
			DbTypeEnum db_type_enum = db_type1 as DbTypeEnum;
			DbEnumValue[] enum_values = new DbEnumValue[db_type_enum.Count];
			db_type_enum.CopyTo (enum_values, 0);
			System.Array.Sort (enum_values, DbEnumValue.IdComparer);
			System.Console.Out.WriteLine ("Sorted by Id :");
			foreach (DbEnumValue v in enum_values)
			{
				System.Console.Out.Write (v.Name+" ");
			}
			System.Console.Out.WriteLine ();
			System.Array.Sort (enum_values, DbEnumValue.NameComparer);
			System.Console.Out.WriteLine ("Sorted by Name :");
			foreach (DbEnumValue v in enum_values)
			{
				System.Console.Out.Write (v.Name+" ");
			}
			System.Console.Out.WriteLine ();
			System.Array.Sort (enum_values, DbEnumValue.RankComparer);
			System.Console.Out.WriteLine ("Sorted by Rank :");
			foreach (DbEnumValue v in enum_values)
			{
				System.Console.Out.Write (v.Name+" ");
			}
			System.Console.Out.WriteLine ();
#endif
			
			
			infrastructure.Dispose ();
		}
		
		private void DisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table)
		{
			this.display.AddTable (name, table);
			this.display.ShowWindow ();
		}
		
		UserInterface.Debugging.DataSetDisplay	display = new UserInterface.Debugging.DataSetDisplay ();
	}
}
