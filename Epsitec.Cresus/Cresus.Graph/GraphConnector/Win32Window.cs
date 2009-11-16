//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Graph
{
	class Win32Window : System.Windows.Forms.IWin32Window
	{
		public Win32Window(System.IntPtr handle)
		{
			this.handle = handle;
		}

		#region IWin32Window Members

		public System.IntPtr Handle
		{
			get
			{
				return this.handle;
			}
		}

		#endregion

		private readonly System.IntPtr handle;
	}
}
