//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Services;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdServiceTools</c> class implements the <c>IDbServiceTools</c> interface
	/// for the Firebird engine.
	/// </summary>
	internal sealed class FirebirdServiceTools : IDbServiceTools
	{
		public FirebirdServiceTools(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		#region IServiceTools Members
		
		public void Backup(string path)
		{
			FbBackup backup = new FbBackup ();
			
			backup.BackupFiles.Add (new FbBackupFile (path, 2048));
			
			backup.ConnectionString = FirebirdAbstraction.MakeConnectionString (fb.DbAccess, fb.MakeDbFilePath (), fb.ServerType);
			backup.Options          = FbBackupFlags.IgnoreLimbo;
			backup.Verbose          = false;
			backup.ServiceOutput   += new ServiceOutputEventHandler (FirebirdServiceTools.ServiceOutput);
			
			System.Diagnostics.Debug.WriteLine ("Backup: running.");
			
			try
			{
				backup.Execute ();
			}
			finally
			{
				System.Diagnostics.Debug.WriteLine ("Backup: done.");
			}
		}
		
		public void Restore(string path)
		{
			FbRestore restore = new FbRestore();
			
			restore.ConnectionString = FirebirdAbstraction.MakeConnectionString (fb.DbAccess, fb.MakeDbFilePath (), fb.ServerType);
			restore.BackupFiles.Add (new FbBackupFile (path, 2048));
			
			restore.Verbose        = false;
			restore.PageSize       = 4096;
			restore.Options        = FbRestoreFlags.Create | FbRestoreFlags.Replace;
			restore.ServiceOutput += new ServiceOutputEventHandler (FirebirdServiceTools.ServiceOutput);

			restore.Execute();
		}
		
		public string GetDatabasePath()
		{
			return this.fb.MakeDbFilePath ();
		}
		
		#endregion
		
		private static void ServiceOutput(object sender, ServiceOutputEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.Message);
		}
		
		private FirebirdAbstraction		fb;
	}
}
