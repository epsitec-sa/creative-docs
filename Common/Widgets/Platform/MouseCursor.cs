//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD
using System;
using System.Runtime.InteropServices;
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

        public void Use()
        {
            SDL_SetCursor(this.cursor);
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
            System.Console.WriteLine(
                $"Cursor from image {cursorImage.Width}x{cursorImage.Height} hot {xhot} {yhot}"
            );
            int width = (int)cursorImage.Width;
            int height = (int)cursorImage.Height;
            // bl-net8-cross
            // hardcoded pixelformat and stride value
            var pixelformat = SDL_PIXELFORMAT_ARGB8888;
            int stride = width * 4;

            var cursorPixelsHandle = GCHandle.Alloc(
                cursorImage.BitmapImage.GetPixelBuffer(),
                GCHandleType.Pinned
            );
            var cursorSurface = SDL_CreateRGBSurfaceWithFormatFrom(
                cursorPixelsHandle.AddrOfPinnedObject(),
                width,
                height,
                32,
                stride,
                pixelformat
            );
            if (cursorSurface == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }

            int xhotClamp = System.Math.Max(0, System.Math.Min(width - 1, xhot));
            int yhotClamp = System.Math.Max(0, System.Math.Min(height - 1, yhot));
            IntPtr cursor = SDL_CreateColorCursor(cursorSurface, xhotClamp, yhotClamp);
            if (cursor == IntPtr.Zero)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
            // cleanup
            cursorPixelsHandle.Free();
            SDL_FreeSurface(cursorSurface);
            return new MouseCursor(cursor);
        }

        public static MouseCursor FromImage(Drawing.Image cursorImage)
        {
            int x = 0;
            int y = 0;
            if (cursorImage is Drawing.Canvas)
            {
                var canvas = (Drawing.Canvas)cursorImage;
                x = (int)System.Math.Round(canvas.Origin.X);
                y = (int)System.Math.Round(canvas.Height - canvas.Origin.Y);
            }
            else if (cursorImage is Drawing.DynamicImage)
            {
                var dynImage = (Drawing.DynamicImage)cursorImage;
                x = (int)System.Math.Round(dynImage.Origin.X);
                y = (int)System.Math.Round(dynImage.Height - dynImage.Origin.Y);
            }

            return MouseCursor.FromImage(cursorImage, x, y);
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
