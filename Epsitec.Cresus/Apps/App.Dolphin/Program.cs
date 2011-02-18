//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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

			Epsitec.Common.Support.ResourceManagerPool pool = new Epsitec.Common.Support.ResourceManagerPool("App.Dolphin");
			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths();

			//	A cause des différents widgets de MyWidgets, il est important de ne pas changer
			//	de look. LookSimply affiche des choses simples avec des cadres noirs.
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookSimply");

			DolphinApplication mainWindow = new DolphinApplication(pool, args);
			mainWindow.Show(null);
			mainWindow.Window.Run();
		}
	}
}