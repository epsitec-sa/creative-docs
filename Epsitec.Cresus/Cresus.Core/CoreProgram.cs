﻿//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Splash;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.IO;

namespace Epsitec.Cresus.Core
{
	static class CoreProgram
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{		
			if (args.Length == 2 && args[0].Equals ("-db-create-epsitec"))
			{
				CoreProgram.ExecuteCreateEpsitecDatabase (args);
			}
			else if (args.Length == 2 && args[0].Equals ("-db-create-user"))
			{
				CoreProgram.ExecuteCreateUserDatabase (args);
			}
			else if (args.Length == 2 && args[0].Equals ("-db-reload-epsitec"))
			{
				CoreProgram.ExecuteReloadEpsitecData (args);
			}
			else if (args.Length == 2 && args[0].Equals ("-db-export"))
			{
				CoreProgram.ExecuteDatabaseExport (args);
			}
			else if (args.Length == 2 && args[0].Equals ("-db-backup"))
			{
				CoreProgram.ExecuteDatabaseBackup (args);
			}
			else if (args.Length == 2 && args[0].Equals ("-db-restore"))
			{
				CoreProgram.ExecuteDatabaseRestore (args);
			}
			else if (args.Length == 1 && args[0].Equals ("-db-delete"))
			{
				CoreProgram.ExecuteDatabaseDelete (args);
			}
			else
			{
				CoreProgram.ExecuteCoreProgram ();
			}
		}

		/// <summary>
		/// Gets the application object.
		/// </summary>
		/// <value>The application object.</value>
		public static CoreApplication Application
		{
			get
			{
				return CoreProgram.application;
			}
			internal set
			{
				if (CoreProgram.application == null)
				{
					CoreProgram.application = value;
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
		}

		private static void ExecuteCreateEpsitecDatabase(string[] args)
		{
			FileInfo file = new FileInfo (args[1]);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportDatabase (file, dbAccess);
		}

		private static void ExecuteCreateUserDatabase(string[] args)
		{
			FileInfo file = new FileInfo (args[1]);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.CreateUserDatabase (file, dbAccess);
		}

		private static void ExecuteReloadEpsitecData(string[] args)
		{
			FileInfo file = new FileInfo (args[1]);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ImportSharedData (file, dbAccess);
		}

		private static void ExecuteDatabaseExport(string[] args)
		{
			FileInfo file = new FileInfo (args[1]);
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.ExportDatabase  (file, dbAccess, false);
		}

		private static void ExecuteDatabaseBackup(string[] args)
		{
			string file = args[1];
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.BackupDatabase (file, dbAccess);
		}

		private static void ExecuteDatabaseRestore(string[] args)
		{
			string file = args[1];
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			CoreData.RestoreDatabase (file, dbAccess);
		}

		private static void ExecuteDatabaseDelete(string[] args)
		{
			DbAccess dbAccess = CoreData.GetDatabaseAccess ();

			Data.Infrastructure.DropDatabase (dbAccess);
		}
		
        private static void ExecuteCoreProgram()
		{
//-			Data.Test.Example1 ();
			using (var splash = new SplashScreen ("logo.png"))
			{
				CoreProgram.ExecuteCoreProgram (splash);
			}
		}

		private static void ExecuteCoreProgram(SplashScreen splash)
		{
			Epsitec.Common.Debug.GeneralExceptionCatcher.Setup ();

			UI.Initialize ();

			var app = new CoreApplication ();

			System.Diagnostics.Debug.Assert (app == CoreProgram.application);
			System.Diagnostics.Debug.Assert (app.ResourceManagerPool.PoolName == "Core");

			CoreApplication.LoadSettings ();

			app.DiscoverPlugIns ();
			app.CreatePlugIns ();
			app.SetupData ();
			app.CreateUI ();

			var user = app.UserManager.FindActiveUser ();

			if (app.UserManager.Authenticate (app, app.Data, user, softwareStartup: true))
			{
				app.Window.Show ();
				app.Window.Run ();

				CoreApplication.SaveSettings ();
			}

			UI.ShutDown ();

			app.Dispose ();
			CoreProgram.application = null;
		}

		private static CoreApplication application;
	}
}
