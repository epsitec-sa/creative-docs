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
			
			//	Si on n'arrive pas � d�truire le fichier, c'est que le serveur Firebird a peut-�tre
			//	encore conserv� un handle ouvert; par exp�rience, cela peut prendre ~10s jusqu'� ce
			//	que la fermeture soit effective.
			
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.CreateDatabase (db_access);
			
			DbTable table;
			
			//	V�rifie que les tables principales ont �t� cr��es correctement :
			
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
			
			//	V�rifie que les statements UPDATE ... lors de la cr�ation ont bien pass�,
			//	puis v�rifie aussi que l'incr�ment de l'ID fonctionne correctement.
			
			table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
			
			Assertion.AssertEquals (6L, infrastructure.NewRowIdInTable (null, table.InternalKey, 2));
			Assertion.AssertEquals (8L, infrastructure.NewRowIdInTable (null, table.InternalKey, 0));
			Assertion.AssertEquals (8L, infrastructure.NewRowIdInTable (null, table.InternalKey, 0));
			
			infrastructure.Dispose ();
		}
		
		[Test] public void CheckAttachDatabase()
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.DisplayDataSet = new CallbackDisplayDataSet (this.DisplayDataSet);
			infrastructure.AttachDatabase (db_access);
			
			{
				DbTable db_table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				DbType  db_type1 = infrastructure.ResolveDbType (null, "CR.Name");
				DbType  db_type2 = infrastructure.ResolveDbType (null, "CR.Name");
				DbType  db_type3 = infrastructure.ResolveDbType (null, "CR.KeyId");
				
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
				DbTable db_table = infrastructure.CreateUserTable ("SimpleTest");
				
#if false
				DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
				DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
				DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
				
				column_c.DefineCategory (DbElementCat.UserDataManaged);
				column_d.DefineCategory (DbElementCat.UserDataManaged);
				column_e.DefineCategory (DbElementCat.UserDataManaged);
				
				db_table.Columns.AddRange (new DbColumn[] { column_c, column_d, column_e });
#endif
				
				infrastructure.RegisterNewDbTable (db_table);
				infrastructure.ResolveDbTable (null, db_table.Name);
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
