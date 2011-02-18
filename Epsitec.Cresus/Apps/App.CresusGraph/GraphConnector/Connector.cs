//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>Connector</c> class is called by the native Win32 wrapper.
	/// </summary>
	public static class Connector
	{
		public static int SendData(System.IntPtr windowHandle, string path, string meta, string data)
		{
			System.Diagnostics.Debug.WriteLine ("WindowHandle : " + windowHandle.ToString ("X8"));
			System.Diagnostics.Debug.WriteLine ("Path : " + path ?? "<null>");
			System.Diagnostics.Debug.WriteLine ("Meta : " + meta ?? "<null>");
			System.Diagnostics.Debug.WriteLine ("Data : " + data ?? "<null>");

			var win32Window = new Win32Window (windowHandle);

			//	Bug in Compta 8.1.004.
			if ((data == null) &&
				(meta != null))
			{
				data = meta;
				meta = null;
			}

//-			System.Windows.Forms.MessageBox.Show (win32Window, "Hello !", "GraphConnector", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);

			if ((Connector.process == null) ||
				(Connector.process.HasExited))
			{
				try
				{
					var graphDirPath = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Graphe\Setup", "InstallDir", null);

					try
					{
						if (System.IO.Directory.Exists (@"S:\Epsitec.Cresus\Cresus.Graph\bin\Debug"))
						{
							graphDirPath = @"S:\Epsitec.Cresus\Cresus.Graph\bin\Debug";
						}
					}
					catch
					{
					}

					var graphExePath = System.IO.Path.Combine (graphDirPath, "Graph.exe");

					if (!System.IO.File.Exists (graphExePath))
                    {
						return -2;
                    }
					
					//	Launch "Crésus Graphe" and wait for it to have started its UI thread;
					//	from then on, we will be able to communicate with it using a named pipe.
					
					//	The communication is implemented by the ConnectorClient/ConnectorServer
					//	classes.
					
					var info = new System.Diagnostics.ProcessStartInfo (graphExePath, "-connector=SendData");
					Connector.process = System.Diagnostics.Process.Start (info);

//-					System.Diagnostics.Debug.WriteLine ("Process started : " + Connector.process.Id.ToString ());
					Connector.process.WaitForInputIdle ();
//-					System.Diagnostics.Debug.WriteLine ("Process ready and waiting for input");
				}
				catch
				{
					return -3;
				}
			}

			var client = new ConnectorClient (Connector.process);

			if (client.SendData (windowHandle, path, meta, data))
			{
				return 0;
			}
			else
			{
				return -1;
			}
		}


		private static System.Diagnostics.Process process;
		
		internal static readonly int MaxMessageSize = 128*1024*1024;	//	max. 100 MB while streaming
	}
}