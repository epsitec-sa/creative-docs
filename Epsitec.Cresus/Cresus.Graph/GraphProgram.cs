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

		enum SplashPhase
		{
			FadeIn,
			Show,
			FadeOut,
		};
		
		static void SplashThread()
		{
			System.Diagnostics.Debug.WriteLine ("SlpashThread: setup");

			var splashPhase = SplashPhase.FadeIn;
			var splashForm  = new System.Windows.Forms.Form ()
			{
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None,
				ShowInTaskbar = false,
				TopMost = true,
				StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
			};

			var splashTimer = new System.Windows.Forms.Timer ()
			{
				Interval = 50,
			};

			int exStyle = Splash.Win32Api.GetWindowExStyle (splashForm.Handle) | Splash.Win32Api.Win32Const.WS_EX_LAYERED;
			Splash.Win32Api.SetWindowExStyle (splashForm.Handle, exStyle);

			string path = System.Windows.Forms.Application.StartupPath;

			int alpha = 0;
			var image = System.Drawing.Image.FromFile (@"F:\logo.png");

			splashTimer.Tick +=
				delegate
				{
					if (splashPhase == SplashPhase.FadeIn)
                    {
						if (alpha <= 100)
						{
							GraphProgram.SetAlpha (splashForm, image, alpha * 0.01);
							alpha += 5;
						}
                    }
					if (GraphProgram.IsRunning)
					{
						splashForm.Close ();
					}
				};

			GraphProgram.SetAlpha (splashForm, image, 0);
			splashTimer.Start ();

			System.Diagnostics.Debug.WriteLine ("SplashThread: start");

			System.Windows.Forms.Application.Run (splashForm);

			System.Diagnostics.Debug.WriteLine ("SplashThread: exit");
		}
#if false		
		public static void PremultiplyAlpha(BitmapData data)
		{
			int pixWidth = data.Width;
			int pixHeight = data.Height;
			int pixStride = data.Stride;

			var pixFormat = data.PixelFormat;
			var pixScan0 = data.Scan0;

			if (pixScan0 != System.IntPtr.Zero)
			{
				unsafe
				{
					byte* pixData = (byte*) pixScan0.ToPointer ();

					for (int y = 0; y < pixHeight; y++)
					{
						byte* row = pixData + pixStride * y;

						for (int x = 0; x < pixWidth; x++)
						{
							int a = row[3];
							int r = row[2];
							int g = row[1];
							int b = row[0];

							if ((a != 0) &&
								(a != 255))
							{
								r = r * a / 255;
								g = g * a / 255;
								b = b * a / 255;

								row[2] = (byte) r;
								row[1] = (byte) g;
								row[0] = (byte) b;
							}

							row[3] |= 0x40;

							row += 4;
						}
					}
				}
			}
		}
#endif

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

//			GraphProgram.IsRunning = true;

			GraphSerial.CheckLicense (GraphProgram.Application.Window);

			GraphProgram.Application.Window.Show ();
			GraphProgram.Application.Window.Run ();

			UI.ShutDown ();

			GraphProgram.Application.Dispose ();
			GraphProgram.Application = null;
		}

		private static void SetAlpha(System.Windows.Forms.Form form, System.Drawing.Image image, double alpha)
		{
			Splash.Win32Api.UpdateLayeredWindow (form.Handle, (System.Drawing.Bitmap) image, form.Left, form.Top, alpha);
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
