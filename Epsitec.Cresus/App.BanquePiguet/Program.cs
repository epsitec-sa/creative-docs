//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Cresus.Core;


namespace Epsitec.App.BanquePiguet
{

	static class Program
	{

		[System.STAThread]
		static void Main()
		{
			try
			{
				string path = System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.UserAppData, "test.log");
				System.IO.File.WriteAllText (path, "Ca marche !\r\n...");

				UI.Initialize ();

				using (Application application = new Application ())
				{
					application.SetupUI ();
					application.Window.Show ();
					application.Window.Run ();
				}

				UI.ShutDown ();
			}
			catch (System.Exception ex)
			{
				while (ex != null)
				{
					System.Diagnostics.Debug.WriteLine (ex.Message);
					System.Diagnostics.Debug.WriteLine (ex.StackTrace);
					ex = ex.InnerException;
				}
			}
		}

	}

}
