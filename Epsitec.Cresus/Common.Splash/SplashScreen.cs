//	Copyright © 2009-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Splash
{
	/// <summary>
	/// The <c>SplashScreen</c> class implements a fade-in/fade-out splash screen
	/// implemented based on a fixed PNG bitmap, which may contain an alpha channel.
	/// </summary>
	public class SplashScreen : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SplashScreen"/> class.
		/// </summary>
		/// <param name="logoFileName">Name of the logo file, which must be
		/// located in the same folder as the executable.</param>
		public SplashScreen(string logoFileName)
		{
			this.logoFileName = logoFileName;
			this.logoFilePath = System.IO.Path.Combine (SplashScreen.GetStartupPath (), logoFileName);
			this.phase = SplashPhase.FadeIn;

			this.LaunchSplashThread ();
		}

		private static string GetStartupPath()
		{
			string path = System.Windows.Forms.Application.StartupPath;

			foreach (var suffix in new string[] { @"\bin\Debug", @"\bin\Release" })
			{
				if (path.EndsWith (suffix))
				{
					return path.Substring (0, path.Length - suffix.Length);
				}
			}
			
			return path;
		}

		public SplashPhase Phase
		{
			get
			{
				lock (this)
				{
					return this.phase;
				}
			}
			private set
			{
				lock (this)
				{
					if (this.phase != SplashPhase.FadeOut)
					{
						this.phase = value;
					}
				}
			}
		}

		public void NotifyIsRunning()
		{
			this.Phase = SplashPhase.FadeOut;
		}

		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
			this.Phase = SplashPhase.FadeOut;
			this.splashThread.Join ();
		}

		#endregion
		
		private void LaunchSplashThread()
		{
			this.splashThread = new System.Threading.Thread (this.SplashThreadBody)
			{
				Name = "SplashThread",
				Priority = System.Threading.ThreadPriority.BelowNormal,
			};

			this.splashThread.Start ();
		}

		private void SplashThreadBody()
		{
			if ((string.IsNullOrEmpty (this.logoFilePath)) ||
				(!System.IO.File.Exists (this.logoFilePath)))
			{
				return;
			}

			this.splashForm = new System.Windows.Forms.Form ()
			{
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None,
				ShowInTaskbar = false,
				TopMost = true,
				StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
				ControlBox = false,
			};

			this.splashTimer = new System.Windows.Forms.Timer ()
			{
				Interval = 50,
			};

			this.splashForm.FormClosing +=
				(sender, e) =>
				{
					if (e.CloseReason == System.Windows.Forms.CloseReason.UserClosing)
					{
						e.Cancel = true;
					}
				};

			var handle = this.splashForm.Handle;

			Win32Api.SetWindowExStyle (handle, Win32Api.GetWindowExStyle (handle) | Win32Api.Win32Const.WS_EX_LAYERED);

			var image = System.Drawing.Image.FromFile (this.logoFilePath) as System.Drawing.Bitmap;
			var watch = new System.Diagnostics.Stopwatch ();

			splashTimer.Tick +=
				delegate
				{
					int alpha;

					switch (this.Phase)
					{
						case SplashPhase.FadeIn:
							alpha = (int) (watch.ElapsedMilliseconds);

							if (alpha <= 1000)
							{
								Win32Api.UpdateLayeredWindow (handle, image, this.splashForm.Left, this.splashForm.Top, alpha * 0.001);
							}
							else
							{
								this.Phase = SplashPhase.Show;
								watch.Reset ();
							}
							break;

						case SplashPhase.FadeOut:
							if (!watch.IsRunning)
                            {
								watch.Start ();
                            }

							alpha = (int) (watch.ElapsedMilliseconds) * 3;

							if (alpha <= 1000)
							{
								Win32Api.UpdateLayeredWindow (handle, image, this.splashForm.Left, this.splashForm.Top, 1.0  - alpha * 0.001);
							}
							else
							{
								Win32Api.UpdateLayeredWindow (handle, image, this.splashForm.Left, this.splashForm.Top, 0);
								this.splashTimer.Dispose ();
								this.splashForm.Dispose ();
							}
							break;
					}
				};

			watch.Start ();
			this.splashTimer.Start ();

			System.Windows.Forms.Application.Run (splashForm);
		}


		private readonly string logoFileName;
		private readonly string logoFilePath;
		private System.Windows.Forms.Form  splashForm;
		private System.Windows.Forms.Timer splashTimer;
		private System.Threading.Thread splashThread;
		private SplashPhase phase;
	}
}
