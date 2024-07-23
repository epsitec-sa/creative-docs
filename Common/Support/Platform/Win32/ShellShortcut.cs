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
    using Epsitec.Common.Widgets.Platform;
    using ProcessWindowStyle = System.Diagnostics.ProcessWindowStyle;

    internal sealed class ShellShortcut : System.IDisposable
    {
        // bl-net8-cross maybedelete IGNOREFILE
        public ShellShortcut(string path)
        {
            IPersistFile pf;

            this.link = (IShellLink)new ShellLink();
            this.path = path;

            if (System.IO.File.Exists(this.path))
            {
                try
                {
                    pf = (IPersistFile)this.link;
                    pf.Load(this.path, 0);
                    //this.link.Resolve (System.IntPtr.Zero, (ShellApi.SLR_FLAGS) (0x00010000*100) | ShellApi.SLR_FLAGS.SLR_NO_UI | ShellApi.SLR_FLAGS.SLR_NOSEARCH | ShellApi.SLR_FLAGS.SLR_NOUPDATE);
                }
                catch
                {
                    this.Dispose();
                }
            }
        }

        ~ShellShortcut()
        {
            this.Dispose(false);
        }

        public string Arguments
        {
            get
            {
                /*
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.INFOTIPSIZE
                );
                this.link.GetArguments(buffer, buffer.Capacity);
                return buffer.ToString();
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetArguments(value); }
        }

        public string Description
        {
            get
            {
                /*
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.INFOTIPSIZE
                );
                this.link.GetDescription(buffer, buffer.Capacity);
                return buffer.ToString();
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetDescription(value); }
        }

        public string WorkingDirectory
        {
            get
            {
                /*
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.MAX_PATH
                );
                this.link.GetWorkingDirectory(buffer, buffer.Capacity);
                return buffer.ToString();
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetWorkingDirectory(value); }
        }

        public string TargetPath
        {
            get
            {
                /*
                ShellApi.WIN32_FIND_DATAW fd = new ShellApi.WIN32_FIND_DATAW();
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.MAX_PATH
                );

                this.link.GetPath(
                    buffer,
                    buffer.Capacity,
                    out fd,
                    ShellApi.SLGP_FLAGS.SLGP_UNCPRIORITY
                );
                return buffer.ToString();
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetPath(value); }
        }

        public PidlHandle TargetPidl
        {
            get
            {
                if (this.link != null)
                {
                    System.IntPtr pidl;
                    this.link.GetIDList(out pidl);

                    return PidlHandle.Inherit(pidl);
                }
                else
                {
                    return null;
                }
            }
            set { this.link.SetIDList(value.Pidl); }
        }

        public string IconPath
        {
            get
            {
                /*
                int iconIndex;
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.MAX_PATH
                );
                this.link.GetIconLocation(buffer, buffer.Capacity, out iconIndex);
                return buffer.ToString();
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetIconLocation(value, this.IconIndex); }
        }

        public int IconIndex
        {
            get
            {
                /*
                int iconIndex;
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(
                    Win32Const.MAX_PATH
                );
                this.link.GetIconLocation(buffer, buffer.Capacity, out iconIndex);
                return iconIndex;
                */
                throw new System.NotImplementedException();
            }
            set { this.link.SetIconLocation(this.IconPath, value); }
        }

        public ProcessWindowStyle WindowStyle
        {
            get
            {
                /*
                int windowStyle;
                this.link.GetShowCmd(out windowStyle);

                switch (windowStyle)
                {
                    case Win32Const.SW_SHOWMINIMIZED:
                        return ProcessWindowStyle.Minimized;
                    case Win32Const.SW_SHOWMINNOACTIVE:
                        return ProcessWindowStyle.Minimized;
                    case Win32Const.SW_SHOWMAXIMIZED:
                        return ProcessWindowStyle.Maximized;
                    default:
                        return ProcessWindowStyle.Normal;
                }
                */
                throw new System.NotImplementedException();
            }
            set
            {
                /*
                int windowStyle;

                switch (value)
                {
                    case ProcessWindowStyle.Normal:
                        windowStyle = Win32Const.SW_SHOWNORMAL;
                        break;
                    case ProcessWindowStyle.Minimized:
                        windowStyle = Win32Const.SW_SHOWMINNOACTIVE;
                        break;
                    case ProcessWindowStyle.Maximized:
                        windowStyle = Win32Const.SW_SHOWMAXIMIZED;
                        break;

                    default:
                        throw new System.ArgumentException(
                            string.Format("Unsupported ProcessWindowStyle.{0}", value)
                        );
                }

                this.link.SetShowCmd(windowStyle);
                */
                throw new System.NotImplementedException();
            }
        }

        public void Save()
        {
            IPersistFile pf = (IPersistFile)this.link;
            pf.Save(this.path, true);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (this.link != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.link);
                this.link = null;
            }
        }

        private IShellLink link;
        private string path;
    }
}
