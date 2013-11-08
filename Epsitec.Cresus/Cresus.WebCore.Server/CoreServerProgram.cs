//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.Owin;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;


namespace Epsitec.Cresus.WebCore.Server
{
	/// <summary>
	/// This class is the main entry point of this dll. It manages the server for all its lifetime.
	/// </summary>
	public sealed class CoreServerProgram
	{
		/// <summary>
		/// Starts the server. This constructor won't return until the server has been shut down.
		/// </summary>
		public CoreServerProgram()
		{
			ConsoleCreator.RunWithConsole (
				() =>
				{
					this.SetupParameters ();
					this.SetupConfiguration ();
					this.SetupDatabaseClient ();
				
					this.Initialize (CoreServerProgram.uiCulture, this.clientDirectory);
				
					this.Run (
						enableUserNotifications: CoreContext.HasExperimentalFeature ("Notifications"),
						nGinxAutorun: CoreServerProgram.nGinxAutorun,
						nGinxPath: CoreServerProgram.nGinxPath,
						uiCulture: CoreServerProgram.uiCulture,
						nancyUri: this.nancyUri,
						owinUri: this.owinUri,
						nbCoreWorkers: this.nbCoreWorkers,
						backupDirectory: this.backupDirectory,
						backupInterval: this.backupInterval,
						backupStart: this.backupStart);
				}, 200);
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

			this.nancyUri = this.SetupParameter
			(
				"nancyUri",
				s => new Uri (s),
				CoreServerProgram.defaultNancyUri
			);
			Logger.LogToConsole ("Nancy uri: " + this.nancyUri);

			this.owinUri = this.SetupParameter
			(
				"owinUri",
				s => new Uri (s),
				CoreServerProgram.defaultOwinUri
			);
			Logger.LogToConsole ("Owin uri: " + this.owinUri);

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
				s => new DirectoryInfo (s),
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

			Logger.LogToConsole ("Configuration read");
		}

		private T SetupParameter<T>(string parameter, Func<string, T> converter, T defaultValue)
		{
			var value = ConfigurationManager.AppSettings[parameter];

			return value == null
				? defaultValue
				: converter (value);
		}

		private void SetupConfiguration()
		{
			var path = Path.Combine (this.clientDirectory.FullName, "config.js");
			var file = new FileInfo (path);
			var generator = new ConfigurationFileGenerator (file, "epsitecConfig");

			var bannerMessage = new List<string> ();

			if (CoreContext.EnableTestEnvironment)
			{
				bannerMessage.Add ("Environnement de test destiné à la formation.");
				Logger.LogToConsole ("Configuration: test environment");
			}

			if (CoreContext.EnableReadOnlyMode)
			{
				bannerMessage.Add ("En raison de travaux de maintenance, AIDER est en mode lecture seule. La consultation reste possible, mais aucune modification n\\'est possible pour l\\'instant.");
				Logger.LogToConsole ("Configuration: read-only mode");
			}

			if (bannerMessage.Count > 0)
			{
				generator.Set ("displayBannerMessage", true);
				generator.Set ("bannerMessage", string.Join ("<br/>", bannerMessage));
			}

			var splash = CoreContext.EnableTestEnvironment
				? "logo-test.png"
				: "logo.png";

			generator.Set ("splash", "images/Static/" + splash);

			var features = CoreContext.GetExperimentalFeatures ();

			foreach (var feature in features)
			{
				generator.Set ("feature" + feature, true);
				Logger.LogToConsole ("Feature " + feature + " enabled");
			}

			generator.Write ();
			Logger.LogToConsole ("Configuration file written to " + file.FullName);
		}

		private void SetupDatabaseClient()
		{
			CoreContext.EnableEmbeddedDatabaseClient (true);
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

		private void Run(bool enableUserNotifications, bool nGinxAutorun, FileInfo nGinxPath, Uri nancyUri, Uri owinUri, int nbCoreWorkers,
			/**/		 CultureInfo uiCulture, DirectoryInfo backupDirectory, TimeSpan backupInterval, Time? backupStart)
		{
			Logger.LogToConsole ("Launching server...");

			using (var backupManager = new BackupManager (backupDirectory, backupInterval, backupStart))
			using (var nGinxServer = nGinxAutorun ? new NGinxServer (nGinxPath) : null)
			{
				if (nGinxServer != null)
				{
					CoreApp.ExitCalled += o => nGinxServer.Dispose ();
				}

				using (var coreServer = new CoreServer (nbCoreWorkers, uiCulture))
				using (var owinServer = enableUserNotifications ? new OwinServer (owinUri, coreServer) : null)
				using (var nancyServer = new NancyServer (coreServer, nancyUri))
				{
					Logger.LogToConsole ("Server launched");
					Logger.LogToConsole ("Press [ENTER] to shut down");
					Console.ReadLine ();

					Logger.LogToConsole ("Shutting down server...");
				}
			}

			Logger.LogToConsole ("Server shut down");
			Logger.LogToConsole ("Press [ENTER] to exit");
			Console.ReadLine ();
		}

		private static readonly DirectoryInfo	defaultClientDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");
		private static readonly Uri				defaultNancyUri = new Uri ("http://localhost:12345/");
		private static readonly Uri				defaultOwinUri= new Uri ("http://localhost:9002/");
		private static readonly int				defaultNbCoreWorkers = 3;
		private static readonly DirectoryInfo	defaultBackupDirectory = new DirectoryInfo (AppDomain.CurrentDomain.BaseDirectory);
		private static readonly TimeSpan		defaultBackupInterval = TimeSpan.FromDays (1);
		private static readonly Time?			defaultBackupStart = null;
		private static readonly CultureInfo		uiCulture = new CultureInfo ("fr-CH");
		private static readonly FileInfo		nGinxPath = new FileInfo ("Nginx\\nginx.exe");
		private static readonly bool			nGinxAutorun = true;

		private DirectoryInfo					clientDirectory;
		private Uri								nancyUri;
		private Uri								owinUri;

		private int								nbCoreWorkers;

		private DirectoryInfo					backupDirectory;
		private TimeSpan						backupInterval;
		private Time?							backupStart;
	}
}
