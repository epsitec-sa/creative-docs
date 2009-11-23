//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Linq;
using System.Drawing.Imaging;

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
			UI.Initialize ();

			//Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookBlue");

			GraphProgram.Application = new GraphApplication ();

			System.Diagnostics.Debug.Assert (GraphProgram.Application.ResourceManagerPool.PoolName == "Core");

			GraphProgram.Application.SetupUI ();
			GraphProgram.Application.SetupDefaultDocument ();
			GraphProgram.Application.SetupConnectorServer ();

			splash.NotifyIsRunning ();
			
			GraphProgram.Application.Window.MakeActive ();
			GraphSerial.CheckLicense (GraphProgram.Application.Window);

			GraphProgram.Application.Window.Show ();
			GraphProgram.Application.Window.Run ();

			UI.ShutDown ();

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
