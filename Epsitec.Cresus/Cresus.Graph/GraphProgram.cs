//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Linq;

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
			System.Diagnostics.Debug.WriteLine ("Main()");
			GraphProgram.LaunchSplashThread ();
			GraphProgram.ExecuteCoreProgram ();
		}


		static void LaunchSplashThread()
		{
			var thread = new System.Threading.Thread (GraphProgram.SplashThread)
			{
				Name = "SplashThread",
				Priority = System.Threading.ThreadPriority.BelowNormal,
			};

			thread.Start ();
		}

		static void SplashThread()
		{
			var splashForm = new System.Windows.Forms.Form ();
			var splashTimer = new System.Windows.Forms.Timer ()
			{
				Interval = 50,
			};

			splashTimer.Tick +=
				delegate
				{
					if (GraphProgram.IsRunning)
					{
						splashForm.Close ();
					}
				};
			
			splashTimer.Start ();

			System.Diagnostics.Debug.WriteLine ("SplashThread: start");

			System.Windows.Forms.Application.Run (splashForm);

			System.Diagnostics.Debug.WriteLine ("SplashThread: exit");
		}

		static void ExecuteCoreProgram()
		{
			System.Diagnostics.Debug.WriteLine ("ExecuteCoreProgram()");
			UI.Initialize ();

			//Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookBlue");

			GraphProgram.Application = new GraphApplication ();

			System.Diagnostics.Debug.Assert (GraphProgram.Application.ResourceManagerPool.PoolName == "Core");

			GraphProgram.Application.SetupUI ();
			GraphProgram.Application.SetupDefaultDocument ();
			GraphProgram.Application.SetupConnectorServer ();

			GraphProgram.IsRunning = true;

			GraphSerial.CheckLicense (GraphProgram.Application.Window);

			GraphProgram.Application.Window.Show ();
			GraphProgram.Application.Window.Run ();

			UI.ShutDown ();

			GraphProgram.Application.Dispose ();
			GraphProgram.Application = null;
		}


		public static bool IsRunning
		{
			get
			{
				return GraphProgram.isRunning;
			}
			set
			{
				GraphProgram.isRunning = value;
			}
		}

		public static GraphApplication Application
		{
			get;
			private set;
		}


		private static volatile bool isRunning;
	}
}
