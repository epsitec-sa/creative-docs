//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;

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
			UI.Initialize ();
			
			string execPath = Epsitec.Common.Support.Globals.Directories.ExecutableRoot;
			List<string> paths;

#if false
			paths = new List<string> ();
			paths.Add (System.IO.Path.Combine (execPath, "Resources"));

			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths.ToArray ()));
#endif

			CoreApplication application = new CoreApplication ();
			
			application.ResourceManagerPool.DefaultPrefix = "file";
			application.ResourceManagerPool.SetupDefaultRootPaths ();
			application.ResourceManagerPool.ScanForAllModules ();

			application.CreateUserInterface ();

			application.Window.Show ();
			application.Window.Run ();
		}
	}
}
