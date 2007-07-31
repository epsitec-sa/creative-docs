using Epsitec.Common.Designer;
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

			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookSimply");  // pour Dolphin !

			DesignerApplication designerMainWindow;

			designerMainWindow = new DesignerApplication (pool);
			designerMainWindow.Mode = DesignerMode.Dolphin;
			designerMainWindow.Standalone = true;
			designerMainWindow.Show (null);
			designerMainWindow.Window.Run ();
		}
	}
}