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
			UI.Initialize ();

			using (Application application = new Application ())
			{
				application.SetupUI ();
				application.Window.Show ();
				application.Window.Run ();
			}

			UI.ShutDown ();	
		}

	}

}
