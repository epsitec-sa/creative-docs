//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

namespace Epsitec.Cresus.Graph
{
	static class GraphProgram
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			using (var splash = new Splash.SplashScreen ("logo.png"))
			{
				GraphProgram.ExecuteCoreProgram (splash);
			}
		}

		
		static void ExecuteCoreProgram(Splash.SplashScreen splash)
		{
			Epsitec.Common.Debug.GeneralExceptionCatcher.Setup ();

			Services.Initialize ();

			//Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookBlue");

			GraphProgram.Application = new GraphApplication ();

			System.Diagnostics.Debug.Assert (GraphProgram.Application.ResourceManagerPool.PoolName == "Core");

			GraphProgram.Application.SetupUI ();
			GraphProgram.Application.SetupDefaultDocument ();

			splash.NotifyIsRunning ();
			
			GraphProgram.Application.Window.MakeActive ();
			
			GraphUpdate.CheckUpdate ();
			GraphSerial.CheckLicense (GraphProgram.Application.Window);

			GraphProgram.Application.ProcessCommandLine ();

			GraphProgram.Application.Window.Show ();
			GraphProgram.Application.Window.Run ();

			Services.ShutDown ();

			GraphProgram.Application.Dispose ();
			GraphProgram.Application = null;
		}


		public static GraphApplication Application
		{
			get;
			private set;
		}
	}
}
