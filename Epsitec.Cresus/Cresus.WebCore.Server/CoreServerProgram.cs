//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using System;

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
				this.Initialize
				(
					uiCulture: CoreServerProgram.uiCulture
				);
				this.Run
				(
					nGinxAutorun: CoreServerProgram.nGinxAutorun,
					nGinxPath: CoreServerProgram.nGinxPath,
					uiCulture: CoreServerProgram.uiCulture,
					uri: CoreServerProgram.baseUri,
					nbCoreWorkers: CoreServerProgram.nbCoreWorkers
				);
			});
		}


		private void Initialize(CultureInfo uiCulture)
		{
			Logger.LogToConsole ("Setting up server...");

			Thread.CurrentThread.CurrentUICulture = uiCulture;
			
			TypeRosetta.InitializeResources ();
			CoreContext.ExecutePendingSetupFunctions ();

			IconManager.BuildIcons (CoreServerProgram.iconDirectory.FullName);

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


		private static readonly CultureInfo uiCulture = new CultureInfo ("fr-CH");


		private static readonly FileInfo nGinxPath = new FileInfo ("Nginx\\nginx.exe");


		private static readonly bool nGinxAutorun = true;


		private static readonly DirectoryInfo iconDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");


		private static readonly Uri baseUri = new Uri ("http://localhost:12345/");


		private static readonly int nbCoreWorkers = 3;


		private static readonly bool enableDumper = false;


	}

}
