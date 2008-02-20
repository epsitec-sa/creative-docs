//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Server
{
	/// <summary>
	/// Summary description for InitialDatabase.
	/// </summary>
	public sealed class InitialDatabase
	{
		public static void Create(WindowsService service)
		{
			Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ();
			Database.DbAccess         access         = WindowsService.CreateAccess ();
			
			infrastructure.CreateDatabase (access);
			
			service.EventLog.WriteEntry ("Created empty database.", System.Diagnostics.EventLogEntryType.Warning);
			
			infrastructure.LocalSettings.IsServer = true;
			infrastructure.LocalSettings.ClientId = 1;
			
			using (Database.DbTransaction transaction = infrastructure.BeginTransaction (Database.DbTransactionMode.ReadWrite))
			{
				infrastructure.LocalSettings.PersistToBase (transaction);
				transaction.Commit ();
			}
			
			infrastructure.Dispose ();
		}
	}
}
