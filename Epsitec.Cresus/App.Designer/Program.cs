using Epsitec.Common.Designer;

using System.Collections.Generic;

namespace App.Designer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();

			List<string> paths = new List<string> (new string[]
			{
				@"S:\Epsitec.Cresus\Common.Dialogs\Resources",
				@"S:\Epsitec.Cresus\Common.Designer\Resources",
				@"S:\Epsitec.Cresus\Common.Document\Resources",
				@"S:\Epsitec.Cresus\Common.Support\Resources",
				@"S:\Epsitec.Cresus\Common.Types\Resources",
				@"S:\Epsitec.Cresus\Common.Widgets\Resources",
				@"S:\Epsitec.Cresus\App.DocumentEditor\Resources",
			});

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-no-default-paths":
						paths.Clear ();
						break;

					case "-add-path":
						paths.Add (args[++i]);
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Option {0} not supported", args[i]));
				}
			}
			
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths.ToArray ()));
			
			MainWindow designerMainWindow;
			
			designerMainWindow = new MainWindow ();
			designerMainWindow.Mode = DesignerMode.Build;
			designerMainWindow.Show (null);
			designerMainWindow.Window.WindowCloseClicked += delegate (object sender) { designerMainWindow.Window.Quit (); };
			designerMainWindow.Window.Run ();
		}
	}
}