//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.Firebird;
using FirebirdSql.Data.Firebird.Services;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlEngine pour Firebird.
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
			//	TODO: vérifier l'implémentation du backup vers un fichier
			
			FbBackup backup = new FbBackup ();
			
			backup.Parameters.Database = this.fb.DbAccess.Database;
			backup.Parameters.DataSource = this.fb.DbAccess.Server;
			backup.Parameters.UserName = this.fb.DbAccess.LoginName;
			backup.Parameters.UserPassword = this.fb.DbAccess.LoginPassword;
			backup.Options = FbBackupFlags.IgnoreLimbo;
			backup.BackupFiles.Add (new FbBackupFile (file_name, 2048));
			backup.Verbose = true;
			backup.Start ();
			backup.Close ();
			
		}
		#endregion
		
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
