using System;
using Epsitec.Common.Designer;

namespace App.Tester
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();

			string[] paths = new string[]
			{
				@"S:\Epsitec.Cresus\Common.Dialogs\Resources",
				@"S:\Epsitec.Cresus\Common.Designer\Resources",
				@"S:\Epsitec.Cresus\Common.Document\Resources",
				@"S:\Epsitec.Cresus\Common.Support\Resources",
				@"S:\Epsitec.Cresus\Common.Types\Resources",
				@"S:\Epsitec.Cresus\Common.Widgets\Resources",

				@"S:\Epsitec.Cresus\App.DocumentEditor\Resources",
			};
			
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths));
			
			MainWindow testerMainWindow;
			
			testerMainWindow = new MainWindow ();
			if (!testerMainWindow.InitDb ())
			{
				return;
			}
			testerMainWindow.Show (null);
			testerMainWindow.Window.WindowCloseClicked += delegate (object sender) { testerMainWindow.Window.Quit (); };
			testerMainWindow.Window.Run ();
		}
	}
}