//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.Firebird;
using FirebirdSql.Data.Firebird.Services;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Impl�mentation de ISqlEngine pour Firebird.
	/// </summary>
	public class FirebirdServiceTools : IDbServiceTools, System.IDisposable
	{
		public FirebirdServiceTools(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		
		#region IServiceTools Members
		public void Backup(string file_name)
		{
			FbBackup backup = new FbBackup ();
			
			backup.BackupFiles.Add (new FbBackupFile (file_name, 2048));
			
			backup.ConnectionString = FirebirdAbstraction.MakeStandardConnectionString (fb.DbAccess, fb.MakeDbFileName (fb.DbAccess), fb.ServerType);
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
		
		public void Restore(string file_name)
		{
			FbRestore restore = new FbRestore();
			
			restore.ConnectionString = FirebirdAbstraction.MakeStandardConnectionString (fb.DbAccess, fb.MakeDbFileName (fb.DbAccess), fb.ServerType);
			restore.BackupFiles.Add (new FbBackupFile (file_name, 2048));
			
			restore.Verbose        = false;
			restore.PageSize       = 4096;
			restore.Options        = FbRestoreFlags.Create | FbRestoreFlags.Replace;
			restore.ServiceOutput += new ServiceOutputEventHandler (FirebirdServiceTools.ServiceOutput);

			restore.Execute();
		}
		#endregion
		
		private static void ServiceOutput(object sender, ServiceOutputEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.Message);
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}
		
		
		private FirebirdAbstraction		fb;
	}
}
