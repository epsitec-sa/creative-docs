﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.IO;

using Epsitec.Cresus.Core.Server.NancyComponents;

using System;

using System.IO;

using System.Runtime.InteropServices;


namespace Epsitec.Cresus.Core.Server
{

	
	public sealed class CoreServerProgram
	{


		public CoreServerProgram()
		{
			CoreServerProgram.Initialize ();

			ConsoleCreator.RunWithConsole (() => CoreServerProgram.Run ());
		}


		private static void Initialize()
		{
			IconsBuilder.BuildIcons (CoreServerProgram.iconDirectory.FullName);
		}


		private static void Run()
		{
			var uri = CoreServerProgram.baseUri;
			var nbThreads = CoreServerProgram.nbThreads;
			
			Console.WriteLine ("Launching nancy server...");

			using (var nancyServer = new NancyServer (uri, nbThreads))
			{
				nancyServer.Start ();

				Console.WriteLine ("Nancy server running and listening to " + uri + "");
				Console.WriteLine ("Press [ENTER] to shut down");
				Console.ReadLine ();

				Console.WriteLine ("Shutting down nancy server...");

				nancyServer.Stop ();
			}

			Console.WriteLine ("Nancy server shut down");
			Console.WriteLine ("Press [ENTER] to exit");
			Console.ReadLine ();
		}


		private static readonly DirectoryInfo iconDirectory = new DirectoryInfo ("S:\\webcore\\");


		private static readonly Uri baseUri = new Uri ("http://localhost:12345/");


		private static readonly int nbThreads = 3;


	}


}
