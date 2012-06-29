//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using System;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server
{

	
	public sealed class CoreServerProgram
	{


		public CoreServerProgram()
		{
			ConsoleCreator.RunWithConsole (() =>
			{
				this.Initialize ();
				this.Run
				(
					nGinxAutorun: CoreServerProgram.nGinxAutorun,
					nGinxPath: CoreServerProgram.nGinxPath,
					uri: CoreServerProgram.baseUri,
					nbCoreWorkers: CoreServerProgram.nbCoreWorkers
				);
			});
		}


		private void Initialize()
		{
			Logger.LogToConsole ("Generating icons...");

			IconManager.BuildIcons (CoreServerProgram.iconDirectory.FullName);

			Logger.LogToConsole ("Icons generated");
		}


		private void Run(bool nGinxAutorun, FileInfo nGinxPath, Uri uri, int nbCoreWorkers)
		{		
			Logger.LogToConsole ("Launching server...");

			using (var nGinxServer = nGinxAutorun ? new NGinxServer (nGinxPath) : null)
			using (var coreServer = new CoreServer (nbCoreWorkers))
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


		private static readonly FileInfo nGinxPath = new FileInfo ("C:\\nginx\\nginx.exe");


		private static readonly bool nGinxAutorun = true;


		private static readonly DirectoryInfo iconDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");


		private static readonly Uri baseUri = new Uri ("http://localhost:12345/");


		private static readonly int nbCoreWorkers = 3;


		private static readonly bool enableDumper = false;


	}

}
