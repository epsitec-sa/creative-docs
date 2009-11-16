//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Graph
{
	public static class Connector
	{
		public static int SendData(System.IntPtr windowHandle, string path, string meta, string data)
		{
			System.Diagnostics.Debug.WriteLine ("WindowHandle : " + windowHandle.ToString ("X8"));
			System.Diagnostics.Debug.WriteLine ("Path : " + path ?? "<null>");
			System.Diagnostics.Debug.WriteLine ("Meta : " + meta ?? "<null>");
			System.Diagnostics.Debug.WriteLine ("Data : " + data ?? "<null>");

			var win32Window = new Win32Window (windowHandle);

			System.Windows.Forms.MessageBox.Show (win32Window, "Hello !", "GraphConnector", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);

			return 0;
		}
	}
}