using System;
using Epsitec.Common.Designer;

namespace App.Designer
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
				@"S:\Epsitec.Cresus\Common.Widgets\Resources",
				@"S:\Epsitec.Cresus\Common.Types\Resources",
				@"S:\Epsitec.Cresus\Common.Document\Resources",
				@"S:\Epsitec.Cresus\Common.Designer\Resources",
				@"S:\Epsitec.Cresus\App.DocumentEditor\Resources",
			};
			
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths));
			
			MainWindow designerMainWindow;
			
			designerMainWindow = new MainWindow ();
			designerMainWindow.Mode = DesignerMode.Build;
			designerMainWindow.Show (null);
			designerMainWindow.Window.WindowCloseClicked += delegate (object sender) { designerMainWindow.Window.Quit (); };
			designerMainWindow.Window.Run ();
		}
	}
}