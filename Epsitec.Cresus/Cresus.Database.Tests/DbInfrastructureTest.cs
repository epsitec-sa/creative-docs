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
			
			infrastructure.Dispose ();
		}
		
		[Test] public void CheckAttachDatabase()
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.AttachDatabase (db_access);
			infrastructure.DisplayDataSet = new CallbackDisplayDataSet (this.DisplayDataSet);
			
			DbTable db_table = infrastructure.ResolveDbTable ("CR_TABLE_DEF");
			DbType  db_type1 = infrastructure.ResolveDbType (new DbKey (8));
			DbType  db_type2 = infrastructure.ResolveDbType (new DbKey (8));
			DbType  db_type3 = infrastructure.ResolveDbType ("CR.KeyId");
			DbType  db_type4 = infrastructure.ResolveDbType ("CR.Name");
			
			System.Console.Out.WriteLine ("Table {0} has {1} columns:", db_table.Name, db_table.Columns.Count);
			
			for (int i = 0; i < db_table.Columns.Count; i++)
			{
				DbColumn column = db_table.Columns[i];
				DbType   type   = column.Type;
				
				System.Console.Out.WriteLine ("{0}: {1}/{2} of type {3}.", i, column.Name, column.Caption, type.Name);
			}
			
			Assertion.AssertEquals (db_type1, db_type2);
			
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
