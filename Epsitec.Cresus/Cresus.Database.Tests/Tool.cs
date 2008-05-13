//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>Tool</c> class provides a few tools used by the tests.
	/// </summary>
	static class Tool
	{
		/// <summary>
		/// Deletes the database files on disk. This works for Firebird.
		/// </summary>
		/// <param name="name">The database name.</param>
		public static void DeleteDatabase(string name)
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess (name);
			string path = Epsitec.Common.Types.Collection.GetFirst (DbFactory.GetDatabaseFilePaths (access));

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (path);
				}
				catch
				{
				}
			}
		}

		public static void RestoreDatabase(string name, string backupPath)
		{
			Tool.DeleteDatabase (name);

			DbAccess access = DbInfrastructure.CreateDatabaseAccess (name);
			access.CreateDatabase = true;
			
			IDbServiceTools tools  = DbFactory.CreateDatabaseAbstraction (access).ServiceTools;
			string          path   = tools.GetDatabasePath ();

			Tool.DeleteDatabase (name);
			
			tools.Restore (backupPath);
		}
	}
}
