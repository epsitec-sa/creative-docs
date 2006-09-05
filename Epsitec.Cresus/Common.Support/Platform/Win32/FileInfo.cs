using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform.Win32
{
	public class FileInfo
	{
		public System.Drawing.Icon GetDesktopIcon()
		{
			IMalloc allocator = ShellFunctions.GetMalloc ();

			
			
			System.Runtime.InteropServices.Marshal.ReleaseComObject (allocator);
			
			return null;
		}
	}
}
