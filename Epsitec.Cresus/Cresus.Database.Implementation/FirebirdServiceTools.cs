//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

			path = this.GetBackupFilePath (path);
			
			backup.BackupFiles.Add (new FbBackupFile (path, 2048));

			backup.ConnectionString = FirebirdAbstraction.MakeConnectionString (this.fb.DbAccess, this.fb.GetDbFilePath (), this.fb.ServerType);
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

			path = this.GetBackupFilePath (path);

			restore.ConnectionString = FirebirdAbstraction.MakeConnectionString (this.fb.DbAccess, this.fb.GetDbFilePath (), this.fb.ServerType);
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

		private string GetBackupFilePath(string path)
		{
			if (System.IO.Path.IsPathRooted (path))
			{
				return path;
			}
			else
			{
				var dir = System.IO.Path.GetDirectoryName (this.fb.GetDbFilePath ());
				var rel = path;
				
				return System.IO.Path.Combine (dir, rel);
			}
		}

		public bool CheckExistence()
		{
			// TODO This method is not very reliable, as it could tell that the database does not
			// exists when the database exists but the login information is not valid. This might
			// be improved, but it doesn't seem to be an easy way to ask Firebird if a database
			// does exist or not.
			// Marc

			var dbAccess   = this.fb.DbAccess;
			var path       = this.fb.GetDbFilePath ();
			var serverType = this.fb.ServerType;

			string connectionString = FirebirdAbstraction.MakeConnectionString (dbAccess, path, serverType);
			
			bool databaseExists;

			try
			{
				using (FbConnection fbConnection = new FbConnection(connectionString))
				{
					fbConnection.Open ();
					fbConnection.Close ();
				}

				databaseExists = true;
			}
			catch
			{
				databaseExists = false;
			}

			return databaseExists;
		}
		
		public string GetDatabasePath()
		{
			return this.fb.GetDbFilePath ();
		}
		
		#endregion
		
		private static void ServiceOutput(object sender, ServiceOutputEventArgs e)
		{
//-			System.Diagnostics.Debug.WriteLine (e.Message);
		}
		
		
		private readonly FirebirdAbstraction	fb;
	}
}
