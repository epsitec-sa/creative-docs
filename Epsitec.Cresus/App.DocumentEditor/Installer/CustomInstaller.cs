//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.App.DocumentEditor.Installer
{
	using Win32HandleWrapper = Epsitec.Common.Widgets.Platform.Win32Api.Win32HandleWrapper;
	
	/// <summary>
	/// Summary description for CustomInstaller.
	/// </summary>
	[System.ComponentModel.RunInstaller(true)]
	public sealed class CustomInstaller : System.Configuration.Install.Installer
	{
		[DllImport ("User32.dll")]	private extern static System.IntPtr FindWindow(string window_class, string window_title);
		
		public CustomInstaller()
		{
			this.Installers.Add (new Serial ());
		}
		
		public class Serial : System.Configuration.Install.Installer
		{
			public override void Install(System.Collections.IDictionary state)
			{
				base.Install (state);
				
				string path = System.Windows.Forms.Application.CommonAppDataPath;
				string key  = null;
				string app  = "Application Data";
				
				int pos = path.LastIndexOf (app);
				
				if (pos > 0)
				{
					path = path.Substring (0, pos + app.Length);
					path = System.IO.Path.Combine (path, "Crésus Documents");
				}
				
				System.IO.Directory.CreateDirectory (path);
				
				path = System.IO.Path.Combine (path, "serial.info");
				
				if (System.IO.File.Exists (path))
				{
					using (System.IO.StreamReader reader = System.IO.File.OpenText (path))
					{
						key = reader.ReadLine ();
					}
				}
				
				if (key != null)
				{
					if (SerialAlgorithm.TestSerial (key))
					{
						return;
					}
				}
				
				SerialInput form = new SerialInput ();
				
				//	Compliqué : il faut trouver le handle de la fenêtre de l'installateur,
				//	mais comme ce n'est pas un processus .NET, c'est pas simple. Heureusement
				//	que l'on peut user d'asctuce :
				
				System.IntPtr      handle  = CustomInstaller.FindWindow ("MsiDialogCloseClass", "Crésus Documents");
				Win32HandleWrapper wrapper = new Win32HandleWrapper (handle);
				
				form.ShowDialog (wrapper);
				
				if (! form.SerialOk)
				{
					throw new System.Configuration.Install.InstallException ("L'installation a été interrompue...");
				}
				
				key = form.SerialKey;
				
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter (path,false))
				{
					writer.WriteLine (key);
				}
			}
			
			public override void Uninstall(System.Collections.IDictionary state)
			{
				base.Uninstall (state);
			}

			public override void Commit(System.Collections.IDictionary state)
			{
				base.Commit (state);
			}

			public override void Rollback(System.Collections.IDictionary state)
			{
				base.Rollback (state);
			}
		}
	}
}
