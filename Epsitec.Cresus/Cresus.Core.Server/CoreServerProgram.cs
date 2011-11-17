//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.IO;

using Epsitec.Cresus.Core.Server.CoreServer;
using Epsitec.Cresus.Core.Server.NancyModules;
using Epsitec.Cresus.Core.Server.NancyHosting;

using System;

using System.Collections.Generic;

using System.IO;

using System.Runtime.InteropServices;


namespace Epsitec.Cresus.Core.Server
{

	
	public sealed class CoreServerProgram
	{


		public CoreServerProgram()
		{
			ConsoleCreator.RunWithConsole (() =>
			{
				CoreServerProgram.Initialize ();
				CoreServerProgram.Run ();
			});
		}


		private static void Initialize()
		{
			Console.WriteLine ("Generating icons...");

			IconManager.BuildIcons (CoreServerProgram.iconDirectory.FullName);

			Console.WriteLine ("Icons generated");
		}


		private static void Run()
		{
			var uri = CoreServerProgram.baseUri;
			var nbThreads = CoreServerProgram.nbThreads;
			var maxNbSessions = CoreServerProgram.maxNbSessions;
			var sessionTimeout = CoreServerProgram.sessionTimeout;
			var sessionCleanupInterval = CoreServerProgram.sessionCleanupInterval;

			Console.WriteLine ("Launching server...");

			using (var serverContext = new ServerContext (maxNbSessions, sessionTimeout, sessionCleanupInterval))
			{
				using (var nancyServer = new NancyServer (serverContext, uri, nbThreads))
				{
					nancyServer.Start ();

					Console.WriteLine ("Server running and listening to " + uri + "");
					Console.WriteLine ("Press [ENTER] to shut down");
					Console.ReadLine ();

					Console.WriteLine ("Shutting down server...");

					nancyServer.Stop ();
				}
			}

			Console.WriteLine ("Server shut down");
			Console.WriteLine ("Press [ENTER] to exit");
			Console.ReadLine ();
		}


		private static readonly DirectoryInfo iconDirectory = new DirectoryInfo ("S:\\webcore\\");


		private static readonly Uri baseUri = new Uri ("http://localhost:12345/");


		private static readonly int nbThreads = 3;


		private static readonly int maxNbSessions = 10;


		private static readonly TimeSpan sessionTimeout = TimeSpan.FromMinutes (5);


		private static readonly TimeSpan sessionCleanupInterval = TimeSpan.FromMinutes (1);


	}


}
