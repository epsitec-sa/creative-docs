//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


namespace Epsitec.Cresus.WebCore.Server
{

	
	public sealed class CoreServerProgram
	{


		public CoreServerProgram()
		{
			ConsoleCreator.RunWithConsole (() =>
			{
				this.SetupParameters ();
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
					nbCoreWorkers: this.nbCoreWorkers
				);
			});
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

			Logger.LogToConsole ("Configuration read");
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
			CoreContext.ExecutePendingSetupFunctions ();

			IconManager.BuildIcons (clientDirectory.FullName);

			Logger.LogToConsole ("Server set up");
		}


		private void Run(bool nGinxAutorun, FileInfo nGinxPath, Uri uri, int nbCoreWorkers, CultureInfo uiCulture)
		{		
			Logger.LogToConsole ("Launching server...");

			using (var nGinxServer = nGinxAutorun ? new NGinxServer (nGinxPath) : null)
			using (var coreServer = new CoreServer (nbCoreWorkers, uiCulture))
			using (var nancyServer = new NancyServer (coreServer, uri))
			{
				Dumper.Instance.IsEnabled = CoreServerProgram.enableDumper;

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


		private static readonly DirectoryInfo defaultClientDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");


		private static readonly Uri defaultBaseUri = new Uri ("http://localhost:12345/");


		private static readonly int defaultNbCoreWorkers = 3;


		private static readonly CultureInfo uiCulture = new CultureInfo ("fr-CH");


		private static readonly FileInfo nGinxPath = new FileInfo ("Nginx\\nginx.exe");


		private static readonly bool nGinxAutorun = true;


		private static readonly bool enableDumper = false;


	}

}
