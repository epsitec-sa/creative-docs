//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.NancyModules;

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
					nbThreads: CoreServerProgram.nbThreads,
					maxNbSessions: CoreServerProgram.maxNbSessions,
					sessionTimeout: CoreServerProgram.sessionTimeout,
					sessionCleanupInterval: CoreServerProgram.sessionCleanupInterval
				);
			});
		}


		private void Initialize()
		{
			Console.WriteLine ("Generating icons...");

			IconManager.BuildIcons (CoreServerProgram.iconDirectory.FullName);

			Console.WriteLine ("Icons generated");
		}


		private void Run(bool nGinxAutorun, FileInfo nGinxPath, Uri uri, int nbThreads, int maxNbSessions, TimeSpan sessionTimeout, TimeSpan sessionCleanupInterval)
		{		
			Console.WriteLine ("Launching server...");

			using (var nGinxServer = nGinxAutorun ? new NGinxServer (nGinxPath) : null)		
			using (var serverContext = new ServerContext (maxNbSessions, sessionTimeout, sessionCleanupInterval))
			using (var nancyServer = new NancyServer (serverContext, uri, nbThreads))
			{
				nancyServer.Start ();

				Dumper.Instance.IsEnabled = CoreServerProgram.enableDumper;

				Console.WriteLine ("Server launched");
				Console.WriteLine ("Press [ENTER] to shut down");
				Console.ReadLine ();

				Console.WriteLine ("Shutting down server...");

				nancyServer.Stop ();
			}

			Console.WriteLine ("Server shut down");
			Console.WriteLine ("Press [ENTER] to exit");
			Console.ReadLine ();
		}


		private static readonly FileInfo nGinxPath = new FileInfo ("C:\\nginx\\nginx.exe");


		private static readonly bool nGinxAutorun = true;


		private static readonly DirectoryInfo iconDirectory = new DirectoryInfo ("S:\\Epsitec.Cresus\\Cresus.WebCore.Client\\WebCore\\");


		private static readonly Uri baseUri = new Uri ("http://localhost:12345/");


		private static readonly int nbThreads = 3;


		private static readonly int maxNbSessions = 10;


		private static readonly TimeSpan sessionTimeout = TimeSpan.FromMinutes (5);


		private static readonly TimeSpan sessionCleanupInterval = TimeSpan.FromMinutes (1);


		private static readonly bool enableDumper = false;


	}

}
