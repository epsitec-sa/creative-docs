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
			System.Data.DataSet data = infrastructure.ReadDbTableMeta ("CR_TABLE_DEF");
			new Epsitec.Cresus.UserInterface.DataSetDisplay (data);
			infrastructure.Dispose ();
		}
	}
}
