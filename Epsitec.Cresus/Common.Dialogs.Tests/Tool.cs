//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Dialogs
{
	class Tool
	{
		public static System.IDisposable InjectKey(System.Windows.Forms.Keys key)
		{
			return Tool.InjectKey (key, 1000);
		}
		
		public static System.IDisposable InjectKey(System.Windows.Forms.Keys key, int delay)
		{
			DoneNotifier notifier = new DoneNotifier ();

			if (Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment)
			{
				int pid = Win32Api.GetCurrentThreadId ();

				System.Diagnostics.Debug.WriteLine ("InjectKey starts a new thread.");

				System.Threading.Thread thread = new System.Threading.Thread (
					delegate ()
					{
						for (int i = 0; i < 10; i++)
						{
							System.Threading.Thread.Sleep (delay);
							System.IntPtr handle = Epsitec.Common.Widgets.Platform.Win32Api.FindThreadActiveWindowHandle (pid);

							if (notifier.Done)
							{
								break;
							}

							if (handle != System.IntPtr.Zero)
							{
								System.Diagnostics.Debug.WriteLine ("Posting key " + key.ToString () + " hexa " + ((int) key).ToString ("x") + " to PID " + pid.ToString ("x") + " on handle " + handle.ToString ());

								try
								{
									Win32Api.PostMessage (handle, 0x0100, (System.IntPtr) key, (System.IntPtr) 0x00010001);
									Win32Api.PostMessage (handle, 0x0102, (System.IntPtr) key, (System.IntPtr) 0x00010001);
									Win32Api.PostMessage (handle, 0x0101, (System.IntPtr) key, (System.IntPtr) (0xC001 << 16 | 0001));
								}
								catch (System.Exception ex)
								{
									System.Diagnostics.Debug.WriteLine ("Exception : " + ex.Message);
								}

								delay = 100;

								break;
							}
						}
					});

				thread.Start ();
			}

			return notifier;
		}

		private class DoneNotifier : System.IDisposable
		{
			public bool Done
			{
				get
				{
					return this.done;
				}
			}



			#region IDisposable Members

			public void Dispose()
			{
				this.done = true;
			}

			#endregion

			private bool done = false;
		}

		private static class Win32Api
		{
			[DllImport ("User32.dll")]
			internal extern static bool PostMessage(System.IntPtr handle, uint msg, System.IntPtr wParam, System.IntPtr lParam);
			[DllImport ("Kernel32.dll")]
			internal extern static int GetCurrentThreadId();
		}
	}
}
