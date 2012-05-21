//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform.Win32
{
	/// <summary>
	/// The <c>PidlHandle</c> class wraps a SHELL pointer to ID List so that
	/// we don't forget to free the associated memory.
	/// </summary>
	internal sealed class PidlHandle : Platform.FolderItemHandle
	{
		private PidlHandle()
		{
		}
		
		public PidlHandle(System.IntPtr pidl)
		{
			this.pidl = ShellApi.ILCombine (pidl, System.IntPtr.Zero);
		}

		public System.IntPtr					Pidl
		{
			get
			{
				return this.pidl;
			}
		}

		public void SetPidlCopy(System.IntPtr pidl)
		{
			this.pidl = ShellApi.ILCombine (pidl, System.IntPtr.Zero);
		}
		
		protected override void Dispose(bool disposing)
		{
			PidlHandle.FreePidl (this.pidl);
			this.pidl = System.IntPtr.Zero;
		}

		protected override bool InternalEquals(FolderItemHandle other)
		{
			PidlHandle that = other as PidlHandle;
			
			if (System.Object.ReferenceEquals (that, null))
			{
				return false;
			}

			if (this.pidl == that.pidl)
			{
				return true;
			}
			else
			{
				return ShellApi.ILIsEqual (this.pidl, that.pidl);
			}
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

		
		public static readonly PidlHandle VirtualDesktopHandle = new PidlHandle ();

		private System.IntPtr					pidl;
	}
}
