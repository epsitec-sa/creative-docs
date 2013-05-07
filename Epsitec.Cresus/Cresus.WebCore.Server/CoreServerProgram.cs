//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using Epsitec.Cresus.WebCore.Server.Owin;
using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server
{

	
	public sealed class CoreServerProgram
	{


		public CoreServerProgram()
		{
			ConsoleCreator.RunWithConsole (() =>
			{
				this.SetupParameters ();
				this.SetupConfiguration ();
				this.SetupDatabaseClient ();
				this.Initialize
				(
					uiCulture: CoreServerProgram.uiCulture,
					clientDirectory: this.clientDirectory
				);
				this.Run
				(
					nGinxAutorun: CoreServerProgram.nGinxAutorun,
					nGinxPath: CoreServerProgram.nGinxPath,
					uiCulture: CoreServerProgram.uiCulture,
					uri: this.baseUri,
					nbCoreWorkers: this.nbCoreWorkers,
					backupDirectory: this.backupDirectory,
					backupInterval: this.backupInterval,
					backupStart: this.backupStart
				);
			}, 200);
		}

		private void SetupConfiguration()
		{
			var bannerMessage = new List<string> ();

			if (CoreContext.EnableTestEnvironment)
			{
				bannerMessage.Add ("Environnement de test destiné à la formation");
				Logger.LogToConsole ("Configuration: test environment");
			}

			if (CoreContext.EnableReadOnlyMode)
			{
				bannerMessage.Add ("Aucune modification n\\'est possible pour l\\'instant");
				Logger.LogToConsole ("Configuration: read-only mode");
			}

			var buffer = new System.Text.StringBuilder ();

			buffer.AppendLine ("var epsitecConfig = {");
			
			if (bannerMessage.Count == 0)
			{
				buffer.Append ("  splash: 'images/Static/logo.png'");
			}
			else
			{
				buffer.AppendLine ("  splash: 'images/Static/logo.png',");
			}

			if (bannerMessage.Count != 0)
			{
				var message = string.Join ("<br/>", bannerMessage);
				buffer.AppendLine ("  displayBannerMessage: true,");
				buffer.Append ("  bannerMessage: '" + message + "'");
			}

			var features = CoreContext.GetExperimentalFeatures ();

			foreach (var feature in features)
			{
				buffer.AppendLine (",");
				buffer.Append ("  feature" + feature + ": true");
			}

			buffer.AppendLine ();
			buffer.AppendLine ("};");

			System.IO.File.WriteAllText (System.IO.Path.Combine (this.clientDirectory.FullName, "config.js"),
										 buffer.ToString (), System.Text.Encoding.UTF8);
		}


		private void SetupParameters()
		{
			Logger.LogToConsole ("Reading configuration...");

			Logger.LogToConsole ("Nginx path: " + CoreServerProgram.nGinxPath.FullName);
			Logger.LogToConsole ("Nginx autorun: " + CoreServerProgram.nGinxAutorun);

			this.clientDirectory = this.SetupParameter
			(
				"clientDirectory",
				s => new DirectoryInfo (s),
				CoreServerProgram.defaultClientDirectory
			);
			Logger.LogToConsole ("Client directory: " + this.clientDirectory.FullName);
			
			this.baseUri = this.SetupParameter
			(
				"baseUri",
				s => new Uri (s),
				CoreServerProgram.defaultBaseUri
			);
			Logger.LogToConsole ("Base uri: " + this.baseUri);
			
			this.nbCoreWorkers = this.SetupParameter
			(
				"nbCoreWorkers",
				s => int.Parse (s),
				CoreServerProgram.defaultNbCoreWorkers
			);
			Logger.LogToConsole ("Number of CoreWorkers: " + this.nbCoreWorkers);

			this.backupDirectory = this.SetupParameter
			(
				"backupDirectory",
				s => new DirectoryInfo(s),
				CoreServerProgram.defaultBackupDirectory
			);
			Logger.LogToConsole ("Backup directory: " + this.backupDirectory);

			this.backupStart = this.SetupParameter
			(
				"backupStart",
				s => new Time (DateTime.Parse (s)),
				CoreServerProgram.defaultBackupStart
			);
			Logger.LogToConsole ("Backup start: " + this.backupStart);

			this.backupInterval = this.SetupParameter
			(
				"backupInterval",
				s => TimeSpan.Parse (s),
				CoreServerProgram.defaultBackupInterval
			);
			Logger.LogToConsole ("Backup interval: " + this.backupInterval);

			this.enableUserNotifications = this.SetupParameter
			(
				"enableUserNotifications",
				s => bool.Parse(s),
				CoreServerProgram.defaultEnableUserNotifications
			);
			Logger.LogToConsole ("Enable user notifications: " + this.enableUserNotifications);

			Logger.LogToConsole ("Configuration read");
		}


		private void SetupDatabaseClient()
		{
			CoreContext.EnableEmbeddedDatabaseClient (true);
		}


		private T SetupParameter<T>(string parameter, Func<string, T> converter, T defaultValue)
		{
			var value = ConfigurationManager.AppSettings[parameter];

			return value == null
				? defaultValue
				: converter (value);
		}


		private void Initialize(CultureInfo uiCulture, DirectoryInfo clientDirectory)
		{
			Logger.LogToConsole ("Setting up server...");

			Thread.CurrentThread.CurrentUICulture = uiCulture;
			
			TypeRosetta.InitializeResources ();
			Logger.LogToConsole ("Type resources initialized");

			CoreContext.ExecutePendingSetupFunctions ();
			Logger.LogToConsole ("Setup functions executed");

			IconManager.BuildIcons (clientDirectory.FullName);
			Logger.LogToConsole ("Icons built");

			Logger.LogToConsole ("Server set up");
		}


		private void Run(bool nGinxAutorun, FileInfo nGinxPath, Uri uri, int nbCoreWorkers, CultureInfo uiCulture, DirectoryInfo backupDirectory, TimeSpan backupInterval, Time? backupStart)
		{
			Logger.LogToConsole ("Launching server...");
			
			using (var owin = this.enableUserNotifications && CoreContext.HasExperimentalFeature("Notifications") ? new OwinServer () : null)
			using (var backupManager = new BackupManager (backupDirectory, backupInterval, backupStart))
			using (var nGinxServer = nGinxAutorun ? new NGinxServer (nGinxPath) : null)
			using (var coreServer = new CoreServer (nbCoreWorkers, uiCulture))
			using (var nancyServer = new NancyServer (coreServer, uri))
			{
				Logger.LogToConsole ("Server launched");
				Logger.LogToConsole ("Press [ENTER] to shut down");
				Console.ReadLine ();

				Logger.LogToConsole ("Shutting down server...");
			}

			Logger.LogToConsole ("Server shut down");
			Logger.LogToConsole ("Press [ENTER] to exit");
			Console.ReadLine ();
		}


		private DirectoryInfo clientDirectory;
		
		
		private Uri baseUri;
		
		
		private int nbCoreWorkers;


		private DirectoryInfo backupDirectory;


		private TimeSpan backupInterval;


		private Time? backupStart;

		private bool enableUserNotifications;

		private static readonly DirectoryInfo defaultClientDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");


		private static readonly Uri defaultBaseUri = new Uri ("http://localhost:12345/");


		private static readonly int defaultNbCoreWorkers = 3;


		private static readonly DirectoryInfo defaultBackupDirectory = new DirectoryInfo (AppDomain.CurrentDomain.BaseDirectory);


		private static readonly TimeSpan defaultBackupInterval = TimeSpan.FromDays (1);


		private static readonly Time? defaultBackupStart = null;

		
		private static readonly bool defaultEnableUserNotifications = false;


		private static readonly CultureInfo uiCulture = new CultureInfo ("fr-CH");


		private static readonly FileInfo nGinxPath = new FileInfo ("Nginx\\nginx.exe");


		private static readonly bool nGinxAutorun = true;


	}

}
