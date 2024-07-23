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


using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
    /// <summary>
    /// The <c>Clipboard</c> class is used to read data in the "HTML Format"
    /// without having .NET cripple the data while doing so.
    /// </summary>
    public static class Clipboard
    {
        internal static void Initialize()
        {
            if (Clipboard.viewerHelper == null)
            {
                Clipboard.viewerHelper = new ClipboardViewerHelper();
            }
        }

        public static byte[] ReadHtmlFormat()
        {
            Data data = new Data();
            int size = Clipboard.ReadHtmlFromClipboard(ref data);

            if (size < 0)
            {
                return null;
            }
            else
            {
                byte[] buffer = new byte[size];

                Clipboard.CopyClipboardData(ref data, buffer, size);
                Clipboard.FreeClipboardData(ref data);

                System.Diagnostics.Debug.Assert(data.DataPtr == System.IntPtr.Zero);
                System.Diagnostics.Debug.Assert(data.Size == 0);

                return buffer;
            }
        }

        #region Native Interface to Clipboard.Win32 DLL

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr SetClipboardViewer(System.IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(
            System.IntPtr hWndRemove,
            System.IntPtr hWndNewNext
        );

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(
            System.IntPtr hwnd,
            int wMsg,
            System.IntPtr wParam,
            System.IntPtr lParam
        );

        [DllImport("Clipboard.Win32.dll")]
        private static extern int ReadHtmlFromClipboard(ref Data data);

        [DllImport("Clipboard.Win32.dll")]
        private static extern int CopyClipboardData(ref Data data, byte[] buffer, int size);

        [DllImport("Clipboard.Win32.dll")]
        private static extern int FreeClipboardData(ref Data data);

        private struct Data
        {
            public System.IntPtr DataPtr;
            public int Size;
        }

        #endregion


        #region ClipboardViewerHelper Class

        private class ClipboardViewerHelper : System.Windows.Forms.Form
        {
            public ClipboardViewerHelper()
            {
                this.handle = this.Handle;
                this.nextViewer = Clipboard.SetClipboardViewer(this.handle);
            }

            protected override void Dispose(bool disposing)
            {
                Clipboard.ChangeClipboardChain(this.handle, this.nextViewer);

                base.Dispose(disposing);
            }

            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                switch (m.Msg)
                {
                    //	The WM_DRAWCLIPBOARD message is sent to the first window
                    //	in the clipboard viewer chain when the content of the
                    //	clipboard changes. This enables a clipboard viewer
                    //	window to display the new content of the clipboard.

                    case WM_DRAWCLIPBOARD:
                        this.GetClipboardData();
                        Clipboard.SendMessage(this.nextViewer, m.Msg, m.WParam, m.LParam);
                        break;

                    //	The WM_CHANGECBCHAIN message is sent to the first window
                    //	in the clipboard viewer chain when a window is being
                    //	removed from the chain.

                    case WM_CHANGECBCHAIN:
                        if (m.WParam == this.nextViewer)
                        {
                            //	The next viewer in the change changed; update the
                            //	list so that we notify the new next handler :

                            this.nextViewer = m.LParam;
                        }
                        else
                        {
                            Clipboard.SendMessage(this.nextViewer, m.Msg, m.WParam, m.LParam);
                        }
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            private void GetClipboardData()
            {
                this.data = Epsitec.Common.Support.Clipboard.GetData();

                var handler = Clipboard.DataChanged;

                if (handler != null)
                {
                    handler(this, new ClipboardDataChangedEventArgs(this.data));
                }
            }

            private const int WM_DESTROYCLIPBOARD = 0x0307;
            private const int WM_DRAWCLIPBOARD = 0x0308;
            private const int WM_CHANGECBCHAIN = 0x030D;

            private System.IntPtr nextViewer;
            private System.IntPtr handle;
            private ClipboardReadData data;
        }

        #endregion


        internal static event EventHandler<ClipboardDataChangedEventArgs> DataChanged;

        private static ClipboardViewerHelper viewerHelper;
    }
}
