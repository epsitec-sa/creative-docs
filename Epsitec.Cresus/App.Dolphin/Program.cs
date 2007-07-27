using System.Collections.Generic;

namespace App.Dolphin
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize();

			string execPath = Epsitec.Common.Support.Globals.Directories.ExecutableRoot;
			List<string> paths;

			Epsitec.Common.Support.ResourceManagerPool pool = new Epsitec.Common.Support.ResourceManagerPool("App.Dolphin");
			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths();

			if (Epsitec.Common.Support.Globals.IsDebugBuild)
			{
				paths = new List<string> (new string[]
				{
					@"S:\Epsitec.Cresus\Common.Dialogs\Resources",
					@"S:\Epsitec.Cresus\Common.Support\Resources",
					@"S:\Epsitec.Cresus\Common.Types\Resources",
					@"S:\Epsitec.Cresus\Common.Widgets\Resources",
				});
			}
			else
			{
				paths = new List<string>();
				paths.Add(System.IO.Path.Combine(execPath, "Resources"));
			}
			
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath(string.Join(";", paths.ToArray()));
			
			Epsitec.App.Dolphin.DolphinApplication mainWindow;
			mainWindow = new Epsitec.App.Dolphin.DolphinApplication(pool);
			mainWindow.Show(null);
			mainWindow.Window.Run();
		}
	}
}