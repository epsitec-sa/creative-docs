//	Copyright © 2003-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe Win32Api exporte quelques fonctions de l'API Win32 utilisées
    /// par des couches très bas niveau.
    /// </summary>
    public class Win32Api
    {
        // ******************************************************************
        // TODO bl-net8-cross
        // implement Win32Api (stub)
        // ******************************************************************
        [DllImport("User32.dll")]
        internal static extern int SetWindowLong(System.IntPtr handle, int index, int value);

        [DllImport("User32.dll")]
        internal static extern int GetWindowLong(System.IntPtr handle, int index);

        [DllImport("User32.dll")]
        internal static extern int SetClassLong(System.IntPtr handle, int index, int value);

        [DllImport("User32.dll")]
        internal static extern int GetClassLong(System.IntPtr handle, int index);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool UpdateLayeredWindow(
            System.IntPtr handle,
            System.IntPtr dstDc,
            ref Win32Api.Point dst,
            ref Win32Api.Size size,
            System.IntPtr srcDc,
            ref Win32Api.Point src,
            int color,
            ref Win32Api.BlendFunction blend,
            int flags
        );

        [DllImport("User32.dll")]
        internal static extern System.IntPtr GetDC(System.IntPtr handle);

        [DllImport("User32.dll")]
        internal static extern int ReleaseDC(System.IntPtr handle, System.IntPtr dc);

        [DllImport("User32.dll")]
        internal static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        internal static extern void SendMessage(
            System.IntPtr handle,
            uint msg,
            System.IntPtr wParam,
            System.IntPtr lParam
        );

        [DllImport("User32.dll")]
        internal static extern bool PostMessage(
            System.IntPtr handle,
            uint msg,
            System.IntPtr wParam,
            System.IntPtr lParam
        );

        [DllImport("User32.dll")]
        internal static extern bool PostThreadMessage(
            int threadId,
            uint msg,
            System.IntPtr wParam,
            System.IntPtr lParam
        );

        [DllImport("User32.dll")]
        internal static extern int GetWindowThreadProcessId(System.IntPtr handle, out int pid);

        [DllImport("User32.dll")]
        internal static extern int GetSystemMetrics(int smIndex);

        [DllImport("User32.dll")]
        internal static extern System.IntPtr GetWindow(System.IntPtr handle, int direction);

        [DllImport("User32.dll")]
        internal static extern System.IntPtr FindWindow(string windowClass, string windowTitle);

        [DllImport("User32.dll")]
        internal static extern System.IntPtr GetDesktopWindow();

        [DllImport("User32.dll")]
        internal static extern bool GetGUIThreadInfo(int threadId, out GUIThreadInfo info);

        [DllImport("User32.dll")]
        internal static extern bool IsWindowVisible(System.IntPtr handle);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowEnabled(System.IntPtr handle);

        [DllImport("User32.dll")]
        internal static extern bool BringWindowToTop(System.IntPtr handle);

        [DllImport("User32.dll")]
        internal static extern bool GetIconInfo(System.IntPtr handle, out IconInfo info);

        [DllImport("User32.dll")]
        internal static extern System.IntPtr CreateIconIndirect(ref IconInfo info);

        [DllImport("User32.dll")]
        internal static extern bool DestroyIcon(System.IntPtr handle);

        [DllImport("User32.dll")]
        internal static extern bool ShowWindow(System.IntPtr handle, int cmdShow);

        [DllImport("User32.dll")]
        internal static extern bool MoveWindow(
            System.IntPtr handle,
            int x,
            int y,
            int width,
            int height,
            bool repaint
        );

        [DllImport("User32.dll")]
        internal static extern System.IntPtr GetActiveWindow();

        [DllImport("User32.dll")]
        internal static extern bool GetWindowPlacement(
            System.IntPtr handle,
            ref WindowPlacement placement
        );

        [DllImport("User32.dll")]
        internal static extern bool SetWindowPlacement(
            System.IntPtr handle,
            ref WindowPlacement placement
        );

        [DllImport("User32.dll")]
        internal static extern System.IntPtr SetParent(System.IntPtr child, System.IntPtr parent);

        [DllImport("User32.dll")]
        internal static extern bool IsIconic(System.IntPtr window);

        [DllImport("User32.dll")]
        internal static extern int GetKeyNameText(
            int param,
            [System.Runtime.InteropServices.Out] System.Text.StringBuilder buffer,
            int size
        );

        [DllImport("User32.dll")]
        internal static extern int MapVirtualKeyEx(int code, int mapType, System.IntPtr layout);

        [DllImport("User32.dll")]
        internal static extern System.IntPtr GetKeyboardLayout(int threadId);

        [DllImport("User32.dll")]
        internal static extern bool SetWindowPos(
            System.IntPtr hWnd,
            System.IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            System.IntPtr pvParam,
            int fWinIni
        );

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            out int pvParam,
            int fWinIni
        );

        [DllImport("Kernel32.dll")]
        internal static extern uint RegisterApplicationRestart(string pszCommandLine, int dwFlags);

        [DllImport("Kernel32.dll")]
        internal static extern uint RegisterApplicationRecoveryCallback(
            System.IntPtr pRecoveryCallback,
            System.IntPtr pvParameter,
            int dwPingInterval,
            int dwFlags
        );

        [DllImport("Kernel32.dll")]
        internal static extern uint ApplicationRecoveryInProgress(out bool pbCanceled);

        [DllImport("Kernel32.dll")]
        internal static extern uint ApplicationRecoveryFinished(bool bSuccess);

        [DllImport("Kernel32.dll")]
        internal static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern System.IntPtr CreateSemaphore(
            System.IntPtr securityAttributes,
            int initialCount,
            int maximumCount,
            string name
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern System.IntPtr OpenSemaphore(
            int desiredAccess,
            int inheritHandle,
            string name
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(System.IntPtr handle);

        internal static uint RegisterApplicationRecoveryCallback(
            ApplicationRecoveryCallback callback,
            System.IntPtr parameter,
            int pingInterval,
            int flags
        )
        {
            System.IntPtr callbackPointer = Marshal.GetFunctionPointerForDelegate(callback);
            return Win32Api.RegisterApplicationRecoveryCallback(
                callbackPointer,
                parameter,
                pingInterval,
                flags
            );
        }

        [DllImport("GDI32.dll")]
        internal static extern System.IntPtr CreateCompatibleDC(System.IntPtr dc);

        [DllImport("GDI32.dll")]
        internal static extern System.IntPtr SelectObject(
            System.IntPtr dc,
            System.IntPtr handleObject
        );

        [DllImport("GDI32.dll")]
        internal static extern System.IntPtr DeleteObject(System.IntPtr handleObject);

        [DllImport("GDI32.dll")]
        internal static extern bool DeleteDC(System.IntPtr dc);

        [DllImport("GDI32.dll")]
        internal static extern void SetStretchBltMode(System.IntPtr dc, int mode);

        [DllImport("GDI32.dll")]
        internal static extern void StretchBlt(
            System.IntPtr dc,
            int x,
            int y,
            int dx,
            int dy,
            System.IntPtr srcDc,
            int srcX,
            int srcY,
            int srcDx,
            int srcDy,
            int rop
        );

        internal delegate int ApplicationRecoveryCallback(System.IntPtr pvParameter);

        internal static bool GetKeyName(KeyCode key, out string name)
        {
            int scanCode = Win32Api.MapVirtualKeyEx((int)key, 0, Win32Api.GetKeyboardLayout(0));

            switch (key)
            {
                case KeyCode.Insert:
                case KeyCode.Delete:
                case KeyCode.Home:
                case KeyCode.End:
                case KeyCode.PageDown:
                case KeyCode.PageUp:
                case KeyCode.ArrowLeft:
                case KeyCode.ArrowRight:
                case KeyCode.ArrowUp:
                case KeyCode.ArrowDown:
                    scanCode |= 0x0100; //	ajoute 'extended bit'
                    break;
            }

            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            buffer.Length = 100;

            bool ok = Win32Api.GetKeyNameText(scanCode << 16, buffer, 99) != 0;

            name = buffer.ToString();

            return ok;
        }

/*        public class Win32HandleWrapper : System.Windows.Forms.IWin32Window
        {
            public Win32HandleWrapper(System.IntPtr handle)
            {
                this.handle = handle;
            }

            #region IWin32Window Members
            public System.IntPtr Handle
            {
                get { return this.handle; }
            }
            #endregion

            private System.IntPtr handle;
        }
*/
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Size
        {
            public int Width;
            public int Height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BlendFunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IconInfo
        {
            public byte FlagIcon;
            public byte r1,
                r2,
                r3;
            public int HotspotX;
            public int HotspotY;
            public System.IntPtr BitmapMask;
            public System.IntPtr BitmapColor;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WindowPlacement
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public Point MinPosition;
            public Point MaxPosition;
            public Rect NormalPosition;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WindowPos
        {
            public System.IntPtr WindowHandle;
            public System.IntPtr WindowHandleInsertAfter;
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public int Flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MinMaxInfo
        {
            public Point Reserved;
            public Size MaxSize;
            public Point MaxPosition;
            public Size MinTrackSize;
            public Size MaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GUIThreadInfo
        {
            public int Size;
            public int Flags;
            public System.IntPtr ActiveWindow;
            public System.IntPtr FocusWindow;
            public System.IntPtr CaptureWindow;
            public System.IntPtr MenuOwnerWindow;
            public System.IntPtr MoveSizeWindow;
            public System.IntPtr CaretWindow;
            public Rect Caret;
        }

        internal static int GetWindowStyle(System.IntPtr handle)
        {
            return Win32Api.GetWindowLong(handle, Win32Const.GWL_STYLE);
        }

        internal static int SetWindowStyle(System.IntPtr handle, int style)
        {
            return Win32Api.SetWindowLong(handle, Win32Const.GWL_STYLE, style);
        }

        internal static int GetWindowExStyle(System.IntPtr handle)
        {
            return Win32Api.GetWindowLong(handle, Win32Const.GWL_EXSTYLE);
        }

        internal static int SetWindowExStyle(System.IntPtr handle, int exStyle)
        {
            return Win32Api.SetWindowLong(handle, Win32Const.GWL_EXSTYLE, exStyle);
        }

/*        internal static bool UpdateLayeredWindow(
            System.IntPtr handle,
            System.Drawing.Bitmap bitmap,
            System.Drawing.Rectangle bounds,
            double alpha
        )
        {
            Win32Api.Point srcPoint = new Win32Api.Point();
            Win32Api.Point dstPoint = new Win32Api.Point();
            Win32Api.Size newSize = new Win32Api.Size();

            Win32Api.BlendFunction blendFunction = new Win32Api.BlendFunction();

            //	Get the screen DC, then create a memory based DC compatible with the screen DC.

            System.IntPtr screenDc = Win32Api.GetDC(System.IntPtr.Zero);
            System.IntPtr memoryDc = Win32Api.CreateCompatibleDC(screenDc);

            //	Get access to the bitmap handle contained in the Bitmap object, then select it.

            System.IntPtr memoryBitmap = bitmap.GetHbitmap(System.Drawing.Color.FromArgb(0));
            System.IntPtr oldBitmap = Win32Api.SelectObject(memoryDc, memoryBitmap);

            newSize.Width = bounds.Width;
            newSize.Height = bounds.Height;
            dstPoint.X = bounds.Left;
            dstPoint.Y = bounds.Top;
            srcPoint.X = 0;
            srcPoint.Y = 0;

            blendFunction.BlendOp = Win32Const.AC_SRC_OVER;
            blendFunction.BlendFlags = 0;
            blendFunction.SourceConstantAlpha = (byte)
                System.Math.Min(255, System.Math.Max(0, (int)(alpha * 255.9)));
            blendFunction.AlphaFormat = Win32Const.AC_SRC_ALPHA;

            int flags = Win32Const.ULW_ALPHA;
            bool res = false;

            res = Win32Api.UpdateLayeredWindow(
                handle,
                *//**//*screenDc,
                *//**//*ref dstPoint,
                *//**//*ref newSize,
                *//**//*memoryDc,
                *//**//*ref srcPoint,
                *//**//*0,
                *//**//*ref blendFunction,
                *//**//*flags
            );

            if (res == false)
            {
                System.Diagnostics.Debug.WriteLine(
                    "LastError = " + Marshal.GetLastWin32Error().ToString()
                );
            }

            Win32Api.SelectObject(memoryDc, oldBitmap);
            Win32Api.ReleaseDC(System.IntPtr.Zero, screenDc);
            Win32Api.DeleteObject(memoryBitmap);
            Win32Api.DeleteDC(memoryDc);

            return res;
        }
*/
        public static void GrabScreen(Drawing.Image bitmap, int x, int y)
        {
            /*
            System.Drawing.Bitmap native = bitmap.BitmapImage.NativeBitmap;
            System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(native);

            System.IntPtr bitmapDc = gfx.GetHdc();
            System.IntPtr desktopDc = Win32Api.GetDC(System.IntPtr.Zero);

            int dx = native.Width;
            int dy = native.Height;

            y = (int)(ScreenInfo.PrimaryHeight) - y - dy;

            Win32Api.SetStretchBltMode(bitmapDc, Win32Const.BLT_COLOR_ON_COLOR);
            Win32Api.StretchBlt(
                bitmapDc,
                0,
                0,
                dx,
                dy,
                desktopDc,
                x,
                y,
                dx,
                dy,
                Win32Const.ROP_SRC_COPY
            );

            gfx.ReleaseHdc(bitmapDc);
            Win32Api.ReleaseDC(System.IntPtr.Zero, desktopDc);
            gfx.Flush();
            gfx.Dispose();
            */
        }

        public static System.IntPtr FindThreadActiveWindowHandle(int thread)
        {
            GUIThreadInfo info = new GUIThreadInfo();

            info.Size = 48;

            Win32Api.GetGUIThreadInfo(thread, out info);

            return info.ActiveWindow;
        }
    }
}
