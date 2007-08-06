using System.Collections.Generic;

namespace Epsitec.App.Dolphin
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
			Epsitec.Common.Document.Engine.Initialize();

			string execPath = Epsitec.Common.Support.Globals.Directories.ExecutableRoot;

			Epsitec.Common.Support.ResourceManagerPool pool = new Epsitec.Common.Support.ResourceManagerPool("App.Dolphin");
			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths();

			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookSimply");

			DolphinApplication mainWindow = new DolphinApplication(pool);
			mainWindow.Show(null);
			mainWindow.Window.Run();
		}
	}
}