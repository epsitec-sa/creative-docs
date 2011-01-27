//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		
		#region IDbServiceTools Members
		
		public void Backup(string path)
		{
			// NOTE Here we set the verbose mode to true, as this makes the Execute() method
			// synchronous. This is a bug in firebird, if the verbose property is set to false, then
			// the Execute() method becomes asynchronous and we have then no way of knowing when the
			// backup is finished.
			// Marc
			
			FbBackup backup = new FbBackup ();
			
			backup.BackupFiles.Add (new FbBackupFile (path, 2048));
			
			backup.ConnectionString = FirebirdAbstraction.MakeConnectionString (fb.DbAccess, fb.GetDbFilePath (), fb.ServerType);
			backup.Options          = FbBackupFlags.IgnoreLimbo;
			backup.Verbose          = true;
			backup.ServiceOutput   += FirebirdServiceTools.ServiceOutput;
			
			System.Diagnostics.Debug.WriteLine ("Backup: running; saving to " + path);
			
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
			// NOTE Here we set the verbose mode to true, as this makes the Execute() method
			// synchronous. This is a bug in firebird, if the verbose property is set to false, then
			// the Execute() method becomes asynchronous and we have then no way of knowing when the
			// restore is finished.
			// Marc
			
			FbRestore restore = new FbRestore();
			
			restore.ConnectionString = FirebirdAbstraction.MakeConnectionString (fb.DbAccess, fb.GetDbFilePath (), fb.ServerType);
			restore.BackupFiles.Add (new FbBackupFile (path, 2048));
			
			restore.Verbose        = true;
			restore.PageSize       = 4096;
			restore.Options        = FbRestoreFlags.Create | FbRestoreFlags.Replace;
			restore.ServiceOutput += FirebirdServiceTools.ServiceOutput;

			try
			{
				restore.Execute ();
			}
			finally
			{
				System.Diagnostics.Debug.WriteLine ("Restore: done.");
			}
		}
		
		public string GetDatabasePath()
		{
			return this.fb.GetDbFilePath ();
		}
		
		#endregion
		
		private static void ServiceOutput(object sender, ServiceOutputEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.Message);
		}
		
		private FirebirdAbstraction		fb;
	}
}
