using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture] public class DbInfrastructureTest
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
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
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
			}
		}
		
		[Test] public void CheckAttachDatabase()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true))
			{
				Assertion.AssertNotNull (infrastructure);
				
				DbTable db_table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				DbType  db_type1 = infrastructure.ResolveDbType (null, "CR_Name");
				DbType  db_type2 = infrastructure.ResolveDbType (null, "CR_Name");
				DbType  db_type3 = infrastructure.ResolveDbType (null, "CR_KeyId");
				
				Assertion.AssertNotNull (db_table);
				Assertion.AssertNotNull (db_type1);
				
				Assertion.AssertEquals (db_type1, db_type2);
				
				Assertion.AssertEquals (8, db_table.Columns.Count);
				Assertion.AssertEquals ("CR_ID", db_table.Columns[0].Name);
				Assertion.AssertEquals (db_type1, db_table.Columns["CR_NAME"].Type);
			}
		}
		
		[Test] public void CheckCreateDbType()
		{
			//	Ce test ne marche que pour une base qui est propre (i.e. qui vient d'�tre
			//	cr��e par CheckCreateDatabase).
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbEnumValue[] values = new DbEnumValue[3];
				
				values[0] = new DbEnumValue (1, "M");		values[0].DefineAttributes ("capt=Monsieur");
				values[1] = new DbEnumValue (2, "Mme");		values[1].DefineAttributes ("capt=Madame");
				values[2] = new DbEnumValue (3, "Mlle");	values[2].DefineAttributes ("capt=Mademoiselle");
				
				DbTypeString db_type_str  = infrastructure.CreateDbType ("Nom", 40, false) as DbTypeString;
				DbTypeNum    db_type_num  = infrastructure.CreateDbType ("NUPO", new DbNumDef (4, 0, 1000, 9999)) as DbTypeNum;
				DbTypeEnum   db_type_enum = infrastructure.CreateDbType ("Titre", values) as DbTypeEnum;
				
				infrastructure.RegisterNewDbType (null, db_type_str);
				infrastructure.RegisterNewDbType (null, db_type_num);
				infrastructure.RegisterNewDbType (null, db_type_enum);
				
				DbType db_type_1 = infrastructure.ResolveDbType (null, "Nom");
				DbType db_type_2 = infrastructure.ResolveDbType (null, "NUPO");
				DbType db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assertion.AssertNotNull (db_type_1);
				Assertion.AssertNotNull (db_type_2);
				Assertion.AssertNotNull (db_type_3);
				
				Assertion.AssertEquals ("Nom",   db_type_1.Name);
				Assertion.AssertEquals ("NUPO",  db_type_2.Name);
				Assertion.AssertEquals ("Titre", db_type_3.Name);
				
				infrastructure.UnregisterDbType (null, db_type_1);
				infrastructure.UnregisterDbType (null, db_type_2);
				infrastructure.UnregisterDbType (null, db_type_3);
				
				db_type_1 = infrastructure.ResolveDbType (null, "Nom");
				db_type_2 = infrastructure.ResolveDbType (null, "NUPO");
				db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assertion.AssertNull (db_type_1);
				Assertion.AssertNull (db_type_2);
				Assertion.AssertNull (db_type_3);
			}
		}
		
		[Test] public void CheckCreateDbTable()
		{
			//	Ce test ne marche que pour une base qui est propre (i.e. qui vient d'�tre
			//	cr��e par CheckCreateDatabase).
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table1 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.UserDataManaged);
				infrastructure.RegisterNewDbTable (null, db_table1);
				
				DbTable db_table2 = infrastructure.ResolveDbTable (null, "SimpleTest");
				
				Assertion.AssertNotNull (db_table2);
				
				Assertion.AssertEquals (db_table1.Name,					db_table2.Name);
				Assertion.AssertEquals (db_table1.Category,				db_table2.Category);
				Assertion.AssertEquals (db_table1.PrimaryKeys.Count,	db_table2.PrimaryKeys.Count);
				Assertion.AssertEquals (db_table1.PrimaryKeys[0].Name,	db_table2.PrimaryKeys[0].Name);
				Assertion.AssertEquals (db_table1.Columns.Count,		db_table2.Columns.Count);
				
				Assertion.Assert (infrastructure.FindHighestRowRevision (null, "CR_TABLE_DEF", 1) >= 0);
				Assertion.Assert (infrastructure.FindHighestRowRevision (null, "CR_TABLE_DEF", 100) == -1);
			}
		}
		
		[Test] [ExpectedException (typeof (DbException))] public void CheckCreateDbTableEx1()
		{
			//	Ex�cuter deux fois une cr�ation de table va n�cessairement g�n�rer une exception.
			//	Il faut ex�cuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.UserDataManaged);
				infrastructure.RegisterNewDbTable (null, db_table);
			}
		}
		
		[Test] public void CheckFindDbTables()
		{
			//	Il faut ex�cuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable[] tables = infrastructure.FindDbTables (null, DbElementCat.Any);
				
				for (int i = 0; i < tables.Length; i++)
				{
					System.Console.Out.WriteLine ("Table {0} has {1} columns. Category is {2}.", tables[i].Name, tables[i].Columns.Count, tables[i].Category);
					for (int j = 0; j < tables[i].Columns.Count; j++)
					{
						System.Console.Out.WriteLine (" {0}: {1}, {2} (type {3}).", j, tables[i].Columns[j].Name, tables[i].Columns[j].Type.Name, tables[i].Columns[j].Type.SimpleType);
					}
				}
			}
		}
		
		[Test] public void CheckFindDbTypes()
		{
			//	Il faut ex�cuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbType[] types = infrastructure.FindDbTypes (null);
				
				for (int i = 0; i < types.Length; i++)
				{
					DbType type = types[i];
					
					System.Console.Out.Write ("Type {0}, {1}.", type.Name, type.SimpleType);
					
					if (type is DbTypeString)
					{
						DbTypeString type_string = type as DbTypeString;
						System.Console.Out.WriteLine ("  Length={0}, is fixed length={1}.", type_string.Length, type_string.IsFixedLength);
					}
					else if (type is DbTypeNum)
					{
						DbTypeNum type_num = type as DbTypeNum;
						System.Console.Out.WriteLine ("  Numeric type={0}, {1} digits, {2} shift.", type_num.NumDef.InternalRawType, type_num.NumDef.DigitPrecision, type_num.NumDef.DigitShift);
					}
					else
					{
						System.Console.Out.WriteLine ();
					}
				}
			}
		}
		
		[Test] public void CheckUnregisterDbTable()
		{
			//	Il faut ex�cuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table1 = infrastructure.ResolveDbTable (null, "SimpleTest");
				
				Assertion.AssertNotNull (db_table1);
				
				infrastructure.UnregisterDbTable (null, db_table1);
				
				DbTable db_table2 = infrastructure.ResolveDbTable (null, "SimpleTest");
				
				Assertion.AssertNull (db_table2);
			}
		}
		
		[Test] public void CheckRegisterDbTableSameAsUnregistered()
		{
			//	Il faut ex�cuter le test CheckUnregisterDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.UserDataManaged);
				infrastructure.RegisterNewDbTable (null, db_table);
				
				Assertion.AssertNotNull (infrastructure.ResolveDbTable (null, db_table.Name));
				Assertion.AssertEquals (12L, db_table.InternalKey.Id);
				Assertion.AssertEquals (0, db_table.InternalKey.Revision);
			}
		}
		
		[Test] [ExpectedException (typeof (DbException))] public void CheckUnregisterDbTableEx1()
		{
			//	Il faut ex�cuter le test CheckRegisterDbTableSameAsUnregistered avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.ResolveDbTable (null, "SimpleTest");
				
				Assertion.AssertNotNull (db_table);
				
				infrastructure.UnregisterDbTable (null, db_table);
				Assertion.AssertNull (infrastructure.ResolveDbTable (null, db_table.Name));
				infrastructure.UnregisterDbTable (null, db_table);
			}
		}
		
		
		#region Support Code
		internal static DbInfrastructure GetInfrastructureFromBase(string name, bool debug_attach)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess (name);
			
			if (debug_attach)
			{
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (DbInfrastructureTest.DisplayDataSet);
				infrastructure.AttachDatabase (db_access);
			}
			else
			{
				infrastructure.AttachDatabase (db_access);
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (DbInfrastructureTest.DisplayDataSet);
			}
			
			return infrastructure;
		}
		
		public static void DisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table)
		{
			DbInfrastructureTest.display.AddTable (name, table);
			DbInfrastructureTest.display.ShowWindow ();
		}
		#endregion
		
		static UserInterface.Debugging.DataSetDisplay	display = new UserInterface.Debugging.DataSetDisplay ();
	}
}
