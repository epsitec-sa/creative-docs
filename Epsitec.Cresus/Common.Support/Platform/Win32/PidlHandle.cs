using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform.Win32
{
	internal class PidlHandle : Platform.FolderItemHandle
	{
		private PidlHandle()
		{
		}
		
		public PidlHandle(System.IntPtr pidl)
		{
			this.pidl = ShellApi.ILCombine (pidl, System.IntPtr.Zero);
		}

		public System.IntPtr Pidl
		{
			get
			{
				return this.pidl;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			PidlHandle.FreePidl (this.pidl);
			this.pidl = System.IntPtr.Zero;
		}

		public static PidlHandle Inherit(System.IntPtr pidl)
		{
			PidlHandle handle = new PidlHandle ();
			handle.pidl = pidl;
			return handle;
		}
		
		public static void FreePidl(System.IntPtr pidl)
		{
			if (pidl != System.IntPtr.Zero)
			{
				System.Runtime.InteropServices.Marshal.FreeCoTaskMem (pidl);
			}
		}

		System.IntPtr pidl;
	}
}
