//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD
using System;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe MouseCursor décrit un curseur de souris.
    /// </summary>
    public sealed class MouseCursor : System.IDisposable
    {
        private MouseCursor(IntPtr cursor)
        {
            this.cursor = cursor;
        }

        public static MouseCursor FromSystemCursor(SDL_SystemCursor systemCursor)
        {
            IntPtr cursor = SDL_CreateSystemCursor(systemCursor);
            if (cursor == IntPtr.Zero)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
            return new MouseCursor(cursor);
        }

        public static MouseCursor FromImage(Drawing.Image cursorImage, int xhot, int yhot)
        {
            /*
            Drawing.Image image = Drawing.Bitmap.CopyImage(cursorImage);
            System.Drawing.Bitmap bitmap = image.BitmapImage.NativeBitmap;
            System.IntPtr orgHandle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon();
            Win32Api.IconInfo iconInfo = new Win32Api.IconInfo();

            if (orgHandle == System.IntPtr.Zero)
            {
                throw new System.NullReferenceException("FromImage cannot derive bitmap handle.");
            }

            Win32Api.GetIconInfo(orgHandle, out iconInfo);

            if (
                (iconInfo.BitmapColor == System.IntPtr.Zero)
                || (iconInfo.BitmapMask == System.IntPtr.Zero)
            )
            {
                throw new System.NullReferenceException("FromImage got empty IconInfo.");
            }

            iconInfo.FlagIcon = 0;
            iconInfo.HotspotX = xhot;
            iconInfo.HotspotY = (int)(image.Height) - yhot;

            System.IntPtr newHandle = Win32Api.CreateIconIndirect(ref iconInfo);
            System.Windows.Forms.Cursor winCursor = new System.Windows.Forms.Cursor(newHandle);

            Win32Api.DeleteObject(iconInfo.BitmapColor);
            Win32Api.DeleteObject(iconInfo.BitmapMask);
            Win32Api.DestroyIcon(orgHandle);
            image.Dispose();

            return new MouseCursor(winCursor, newHandle);
            */
            throw new System.NotImplementedException();
            return null;
        }

        public static MouseCursor FromImage(Drawing.Image cursorImage)
        {
            /*
            Drawing.Image image = Drawing.Bitmap.CopyImage(cursorImage);
            System.Drawing.Bitmap bitmap = image.BitmapImage.NativeBitmap;
            System.IntPtr orgHandle = bitmap == null ? System.IntPtr.Zero : bitmap.GetHicon();
            Win32Api.IconInfo iconInfo = new Win32Api.IconInfo();

            if (orgHandle == System.IntPtr.Zero)
            {
                throw new System.NullReferenceException("FromImage cannot derive bitmap handle.");
            }

            Win32Api.GetIconInfo(orgHandle, out iconInfo);

            if (
                (iconInfo.BitmapColor == System.IntPtr.Zero)
                || (iconInfo.BitmapMask == System.IntPtr.Zero)
            )
            {
                throw new System.NullReferenceException("FromImage got empty IconInfo.");
            }

            double ox = image.Origin.X;
            double oy = image.Height - image.Origin.Y;

            iconInfo.FlagIcon = 0;
            iconInfo.HotspotX = (int)System.Math.Floor(ox + 0.5);
            ;
            iconInfo.HotspotY = (int)System.Math.Floor(oy + 0.5);
            ;

            System.IntPtr newHandle = Win32Api.CreateIconIndirect(ref iconInfo);
            System.Windows.Forms.Cursor winCursor = new System.Windows.Forms.Cursor(newHandle);

            Win32Api.DeleteObject(iconInfo.BitmapColor);
            Win32Api.DeleteObject(iconInfo.BitmapMask);
            Win32Api.DestroyIcon(orgHandle);
            image.Dispose();

            return new MouseCursor(winCursor, newHandle);
            */
            throw new System.NotImplementedException();
            return null;
        }

        public static void Hide()
        {
            if (SDL_ShowCursor(SDL_DISABLE) < 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
        }

        public static void Show()
        {
            if (SDL_ShowCursor(SDL_ENABLE) < 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (this.cursor != System.IntPtr.Zero)
            {
                SDL_FreeCursor(this.cursor);
                this.cursor = System.IntPtr.Zero;
            }
        }
        #endregion

        public static MouseCursor Default
        {
            get { return MouseCursor.asArrow; }
        }

        public static MouseCursor AsArrow
        {
            get { return MouseCursor.asArrow; }
        }

        public static MouseCursor AsHand
        {
            get { return MouseCursor.asHand; }
        }

        public static MouseCursor AsIBeam
        {
            get { return MouseCursor.asIBeam; }
        }

        public static MouseCursor AsCross
        {
            get { return MouseCursor.asCross; }
        }

        public static MouseCursor AsWait
        {
            get { return MouseCursor.asWait; }
        }

        public static MouseCursor AsNo
        {
            get { return MouseCursor.asNo; }
        }

        public static MouseCursor AsSizeAll
        {
            get { return MouseCursor.asSizeAll; }
        }

        public static MouseCursor AsSizeNESW
        {
            get { return MouseCursor.asSizeNesw; }
        }

        public static MouseCursor AsSizeNS
        {
            get { return MouseCursor.asSizeNs; }
        }

        public static MouseCursor AsSizeNWSE
        {
            get { return MouseCursor.asSizeNwse; }
        }

        public static MouseCursor AsSizeWE
        {
            get { return MouseCursor.asSizeWe; }
        }

        private System.IntPtr cursor;

        private static readonly MouseCursor asArrow = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW
        );
        private static readonly MouseCursor asHand = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND
        );
        private static readonly MouseCursor asIBeam = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM
        );
        private static readonly MouseCursor asWait = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAITARROW
        );
        private static readonly MouseCursor asCross = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR
        );
        private static readonly MouseCursor asNo = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO
        );
        private static readonly MouseCursor asSizeAll = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL
        );
        private static readonly MouseCursor asSizeNesw = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW
        );
        private static readonly MouseCursor asSizeNs = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS
        );
        private static readonly MouseCursor asSizeNwse = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE
        );
        private static readonly MouseCursor asSizeWe = MouseCursor.FromSystemCursor(
            SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE
        );
    }
}
