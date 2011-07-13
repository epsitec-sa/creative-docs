//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	internal static class CoreProgramOperations
	{
		public static void ProcessCommandLine(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					CoreProgramOperations.ProcessCommandLine (args[0]);
					break;
				case 2:
					CoreProgramOperations.ProcessCommandLine (args[0], args[1]);
					break;
			}
		}

		private static void ProcessCommandLine(string operation)
		{
			switch (operation)
			{
				case "-db-delete":
					CoreProgramOperations.ExecuteDatabaseDelete ();
					break;

				case "-js-server":
					CoreProgramServer.ExecuteServer ();
					break;
			}
		}

		private static void ProcessCommandLine(string operation, string arg1)
		{
			switch (operation)
			{
				case "-db-create-epsitec":
					CoreProgramOperations.ExecuteCreateEpsitecDatabase (arg1);
					break;

				case "-db-create-user":
					CoreProgramOperations.ExecuteCreateUserDatabase (arg1);
					break;

				case "-db-reload-epsitec":
					CoreProgramOperations.ExecuteReloadEpsitecData (arg1);
					break;

				case "-db-export":
					CoreProgramOperations.ExecuteDatabaseExport (arg1);
					break;
	
				case "-db-backup":
					CoreProgramOperations.ExecuteDatabaseBackup (arg1);
					break;

				case "-db-restore":
					CoreProgramOperations.ExecuteDatabaseRestore (arg1);
					break;
			}
		}

		private static void ExecuteCreateEpsitecDatabase(string path)
		{
			FileInfo file = new FileInfo (path);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportDatabase (file, dbAccess);
		}

		private static void ExecuteCreateUserDatabase(string path)
		{
			FileInfo file = new FileInfo (path);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.CreateUserDatabase (file, dbAccess);
		}

		private static void ExecuteReloadEpsitecData(string path)
		{
			FileInfo file = new FileInfo (path);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportSharedData (file, dbAccess);
		}

		private static void ExecuteDatabaseExport(string path)
		{
			FileInfo file = new FileInfo (path);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ExportDatabase (file, dbAccess, false);
		}

		private static void ExecuteDatabaseBackup(string path)
		{
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.BackupDatabase (path, dbAccess);
		}

		private static void ExecuteDatabaseRestore(string path)
		{
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.RestoreDatabase (path, dbAccess);
		}

		private static void ExecuteDatabaseDelete()
		{
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			Data.Infrastructure.DropDatabase (dbAccess);
		}
	}
}
