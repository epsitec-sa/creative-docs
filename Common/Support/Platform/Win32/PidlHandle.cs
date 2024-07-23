/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace Epsitec.Common.Support.Platform.Win32
{
    /// <summary>
    /// The <c>PidlHandle</c> class wraps a SHELL pointer to ID List so that
    /// we don't forget to free the associated memory.
    /// </summary>
    internal sealed class PidlHandle : Platform.FolderItemHandle
    {
        private PidlHandle() { }

        public PidlHandle(System.IntPtr pidl)
        {
            this.pidl = ShellApi.ILCombine(pidl, System.IntPtr.Zero);
        }

        public System.IntPtr Pidl
        {
            get { return this.pidl; }
        }

        public void SetPidlCopy(System.IntPtr pidl)
        {
            this.pidl = ShellApi.ILCombine(pidl, System.IntPtr.Zero);
        }

        protected override void Dispose(bool disposing)
        {
            PidlHandle.FreePidl(this.pidl);
            this.pidl = System.IntPtr.Zero;
        }

        protected override bool InternalEquals(FolderItemHandle other)
        {
            PidlHandle that = other as PidlHandle;

            if (System.Object.ReferenceEquals(that, null))
            {
                return false;
            }

            if (this.pidl == that.pidl)
            {
                return true;
            }
            else
            {
                return ShellApi.ILIsEqual(this.pidl, that.pidl);
            }
        }

        public static PidlHandle Inherit(System.IntPtr pidl)
        {
            PidlHandle handle = new PidlHandle();
            handle.pidl = pidl;
            return handle;
        }

        public static void FreePidl(System.IntPtr pidl)
        {
            if (pidl != System.IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pidl);
            }
        }

        public static readonly PidlHandle VirtualDesktopHandle = new PidlHandle();

        private System.IntPtr pidl;
    }
}
