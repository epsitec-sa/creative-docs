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
			
			infrastructure.Dispose ();
		}
		
		[Test] public void CheckAttachDatabase()
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbFactoryTest.CreateDbAccess ("fiche");
			
			infrastructure.AttachDatabase (db_access);
			infrastructure.DebugDisplayDataSet = new CallbackDebugDisplayDataSet (this.DebugDisplay);
			
			DbTable db_table = infrastructure.ReadDbTableMeta ("CR_TABLE_DEF");
			DbType  db_type  = infrastructure.ResolveType (new DbKey (8));
			
			infrastructure.Dispose ();
		}
		
		private void DebugDisplay(DbInfrastructure infrastructure, string name, System.Data.DataTable table)
		{
			this.display.AddTable (name, table);
			this.display.ShowWindow ();
		}
		
		UserInterface.Debugging.DataSetDisplay	display = new UserInterface.Debugging.DataSetDisplay ();
	}
}
