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

﻿using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal abstract class SDLWindow : IDisposable
    {
        internal SDLWindow(string windowTitle, int width, int height, SDL_WindowFlags flags)
        {
            var window = SDL_CreateWindow(
                windowTitle,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                width,
                height,
                flags | SDL_WindowFlags.SDL_WINDOW_HIDDEN
            );
            this.isVisible = false;
            if (window == IntPtr.Zero)
            {
                throw new SDLException(SDL_GetError());
            }
            this.window = window;
            this.windowID = SDL_GetWindowID(window);
            SDLWindowManager.AddWindow(this);
            this.UpdateWindowPosition();
            this.UpdateWindowSize();

            var renderer = SDL_CreateRenderer(
                window,
                -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED
                    | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (renderer == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    $"There was an issue creating the renderer. {SDL_GetError()}"
                );
            }
            this.renderer = renderer;
            this.RecreateDrawingArea();
        }

        ~SDLWindow()
        {
            this.Dispose();
        }

        /// <summary>
        /// Called on window creation and when the window size changes.
        /// </summary>
        /// <param name="pixels">A handle to the pixel buffer that can be used for drawing</param>
        /// <param name="width">Width of the pixel buffer</param>
        /// <param name="height">Height of the pixel buffer</param>
        /// <param name="stride">Length of one row of the pixel buffer in bytes</param>
        protected abstract void RecreateGraphicBuffer(
            IntPtr pixels,
            int width,
            int height,
            int stride
        );

        #region Window properies
        public int Width
        {
            get
            {
                this.RequireNotDisposed();
                return this.width;
            }
        }

        public int Height
        {
            get
            {
                this.RequireNotDisposed();
                return this.height;
            }
        }

        public int WindowX
        {
            get
            {
                this.RequireNotDisposed();
                return this.x;
            }
        }

        public int WindowY
        {
            get
            {
                this.RequireNotDisposed();
                return this.y;
            }
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
        }

        public bool IsFocused
        {
            get
            {
                this.RequireNotDisposed();
                SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(this.window);
                return flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);
            }
        }

        public bool IsMinimized
        {
            get
            {
                this.RequireNotDisposed();
                SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(this.window);
                return flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED);
            }
        }

        public bool IsMaximized
        {
            get
            {
                this.RequireNotDisposed();
                SDL_WindowFlags flags = (SDL_WindowFlags)SDL_GetWindowFlags(this.window);
                return flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_MAXIMIZED);
            }
        }

        public void SetPosition(int x, int y)
        {
            this.RequireNotDisposed();
            SDL_SetWindowPosition(this.window, x, y);
            this.UpdateWindowPosition();
        }

        public void CenterOnScreen()
        {
            this.RequireNotDisposed();
            SDL_SetWindowPosition(this.window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            this.UpdateWindowPosition();
        }

        public void SetSize(int width, int height)
        {
            this.RequireNotDisposed();
            SDL_SetWindowSize(this.window, width, height);
            this.UpdateWindowSize();
        }

        public void SetBorder(bool border)
        {
            this.RequireNotDisposed();
            SDL_SetWindowBordered(this.window, SDLUtils.ToSDLBool(border));
        }

        public void SetResizable(bool resizable)
        {
            this.RequireNotDisposed();
            SDL_SetWindowResizable(this.window, SDLUtils.ToSDLBool(resizable));
        }

        public void SetFullscreen(bool fullscreen)
        {
            this.RequireNotDisposed();
            SDL_SetWindowFullscreen(
                this.window,
                fullscreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0
            );
        }

        public void SetTitle(string title)
        {
            this.RequireNotDisposed();
            SDL_SetWindowTitle(this.window, title);
        }

        public void SetWindowOpacity(float opacity)
        {
            this.RequireNotDisposed();
            SDL_SetWindowOpacity(this.window, opacity);
        }

        /// <summary>
        /// Set the window icon from raw pixel data
        /// </summary>
        /// <param name="pixels">a buffer of pixels for the icon</param>
        /// <param name="width">Width of the icon</param>
        /// <param name="height">Height of the icon</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected void SetCustomIcon(byte[] pixels, int width, int height)
        {
            this.RequireNotDisposed();
            this.DestroyIconSurface();
            // bl-net8-cross
            // hardcoded pixelformat and stride value
            var pixelformat = SDL_PIXELFORMAT_ARGB8888;
            int stride = width * 4;

            this.iconPixels = pixels;
            this.iconPixelsHandle = GCHandle.Alloc(iconPixels, GCHandleType.Pinned);
            this.iconSurface = SDL_CreateRGBSurfaceWithFormatFrom(
                this.iconPixelsHandle.AddrOfPinnedObject(),
                width,
                height,
                32,
                stride,
                pixelformat
            );
            if (this.iconSurface == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
            SDL_SetWindowIcon(this.window, this.iconSurface);
        }

        #endregion

        #region Window events

        /// <summary>
        /// Called when the window needs to be redrawn.
        /// Drawing should be done in the pixelbuffer created by <c>RecreateGraphicBuffer</c>
        /// </summary>
        protected virtual void OnDraw() { }

        protected virtual void OnResize(int sx, int sy) { }

        public virtual void OnMouseButtonDown(int x, int y, int button) { }

        public virtual void OnMouseButtonUp(int x, int y, int button) { }

        public virtual void OnKeyDown(SDL_Keysym keysym) { }

        public virtual void OnKeyUp(SDL_Keysym keysym) { }

        public virtual void OnMouseMove(int x, int y) { }

        public virtual void OnMouseWheel(int wheelX, int wheelY) { }

        public virtual void OnUserEvent(int eventCode) { }

        protected virtual void OnFocusGained() { }

        protected virtual void OnFocusLost() { }

        protected virtual void OnWindowShown() { }

        protected virtual void OnWindowHidden() { }

        protected virtual void OnWindowClosed() { }

        #endregion

        #region Public methods

        public void Show()
        {
            this.RequireNotDisposed();
            SDL_ShowWindow(this.window);
            this.isVisible = true;
        }

        public void ShowWithoutFocus()
        {
            this.RequireNotDisposed();
            // Hack: SDL2 does not support window that can't be focused (which we need for the tooltip)
            // we get the currently focused window, show the new window and set the focus back on the other window
            // When SDL3 will be released, we might replace this hack by using SDL_SetWindowFocusable()
            // to disable input focus on tooltips
            IntPtr focusedWindow = SDL_GetMouseFocus();
            uint focusedWindowID = SDL_GetWindowID(focusedWindow);
            SDL_ShowWindow(this.window);
            SDL_RaiseWindow(focusedWindow);
            // With our hack, we generate a unfocused/focused event sequence on the focusedWindow
            // we filter these events to avoid unexpected things later on
            SDL_FilterEvents(
                (IntPtr _, IntPtr evPtr) =>
                {
                    SDL_Event ev = Marshal.PtrToStructure<SDL_Event>(evPtr);
                    if (
                        ev.type == SDL_EventType.SDL_WINDOWEVENT
                        && ev.window.windowID == focusedWindowID
                    )
                    {
                        // ignore event
                        return 0;
                    }
                    return 1;
                },
                IntPtr.Zero
            );
            this.isVisible = true;
        }

        public void Hide()
        {
            this.RequireNotDisposed();
            SDL_HideWindow(this.window);
            this.isVisible = false;
        }

        public void Focus()
        {
            this.RequireNotDisposed();
            SDL_RaiseWindow(this.window);
        }

        public void Maximize()
        {
            this.RequireNotDisposed();
            SDL_MaximizeWindow(this.window);
        }

        public void Minimize()
        {
            this.RequireNotDisposed();
            SDL_MinimizeWindow(this.window);
        }

        public void Restore()
        {
            this.RequireNotDisposed();
            SDL_RestoreWindow(this.window);
        }

        public void Flash()
        {
            this.RequireNotDisposed();
            SDL_FlashWindow(this.window, SDL_FlashOperation.SDL_FLASH_UNTIL_FOCUSED);
        }

        public void GenerateWindowCloseEvent()
        {
            SDL_Event ev = new SDL_Event();
            SDL_WindowEvent windowEvent = new SDL_WindowEvent();
            windowEvent.type = SDL_EventType.SDL_WINDOWEVENT;
            windowEvent.windowEvent = SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE;
            windowEvent.windowID = this.windowID;
            windowEvent.timestamp = SDL_GetTicks();
            ev.type = SDL_EventType.SDL_WINDOWEVENT;
            ev.window = windowEvent;
            SDL_PushEvent(ref ev);
        }

        #endregion

        #region Private methods

        internal int DisplayIndex
        {
            get
            {
                this.RequireNotDisposed();
                int index = SDL_GetWindowDisplayIndex(this.window);
                if (index < 0)
                {
                    throw new InvalidOperationException(SDL_GetError());
                }
                return index;
            }
        }

        internal void UpdateWindowPosition()
        {
            SDL_GetWindowPosition(this.window, out this.x, out this.y);
        }

        internal void UpdateWindowSize()
        {
            SDL_GetWindowSize(this.window, out this.width, out this.height);
        }

        internal void UpdateDrawing()
        {
            this.OnDraw();

            var surfaceObj = (SDL_Surface)Marshal.PtrToStructure(this.surface, typeof(SDL_Surface));
            SDL_UpdateTexture(this.texture, IntPtr.Zero, surfaceObj.pixels, surfaceObj.pitch);

            if (SDL_SetRenderDrawColor(this.renderer, 0, 0, 0, 0) < 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }

            if (SDL_RenderClear(this.renderer) < 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }

            SDL_RenderCopy(this.renderer, this.texture, IntPtr.Zero, IntPtr.Zero);

            SDL_RenderPresent(this.renderer);
        }

        /// <summary>
        /// Handle window manager related events (close, resize, maximized…)
        /// </summary>
        /// <param name="we"></param>
        /// <returns>true if the application should continue normaly, false if it should terminate</returns>
        internal void HandleWindowEvent(SDL_WindowEvent we)
        {
            switch (we.windowEvent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                    this.OnWindowShown();
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                    this.OnWindowHidden();
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                    this.OnWindowClosed();
                    this.Dispose();
                    return;
                case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                    this.x = we.data1;
                    this.y = we.data2;
                    break;
                // TODO  bl-net8-cross the resize events are only fired when we release the mouse
                // see how we could have a dynamic resize
                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                    this.width = we.data1;
                    this.height = we.data2;
                    this.RecreateDrawingArea();
                    this.OnResize(we.data1, we.data2);
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                    this.OnFocusGained();
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                    this.OnFocusLost();
                    break;
                case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                    this.UpdateDrawing();
                    break;
                default:
                    Console.WriteLine($"SDLWindow handle window event {we.windowEvent}");
                    break;
            }
        }

        private void RecreateDrawingArea()
        {
            this.DestroyDrawingArea();

            // bl-net8-cross
            // hardcoded pixelformat and stride value
            var pixelformat = SDL_PIXELFORMAT_ARGB8888;
            int stride = this.width * 4;

            this.surface = SDL_CreateRGBSurfaceWithFormat(
                0,
                this.width,
                this.height,
                32,
                pixelformat
            );
            if (this.surface == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
            var surfaceObj = (SDL_Surface)Marshal.PtrToStructure(this.surface, typeof(SDL_Surface));
            this.RecreateGraphicBuffer(surfaceObj.pixels, this.width, this.height, stride);
            this.texture = SDL_CreateTexture(
                this.renderer,
                pixelformat,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                this.width,
                this.height
            );
            if (this.texture == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
        }
        #endregion

        #region Memory management

        private void DestroyIconSurface()
        {
            this.iconPixels = null;
            if (this.iconPixelsHandle.IsAllocated)
            {
                this.iconPixelsHandle.Free();
            }
            if (this.iconSurface != IntPtr.Zero)
            {
                SDL_FreeSurface(this.iconSurface);
            }
            this.iconSurface = IntPtr.Zero;
        }

        private void DestroyDrawingArea()
        {
            if (this.surface != IntPtr.Zero)
            {
                SDL_FreeSurface(this.surface);
            }
            this.surface = IntPtr.Zero;
            if (this.texture != IntPtr.Zero)
            {
                SDL_DestroyTexture(this.texture);
            }
            this.texture = IntPtr.Zero;
        }

        private void RequireNotDisposed()
        {
            if (this.window == IntPtr.Zero)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        public void Dispose()
        {
            if (this.window == IntPtr.Zero)
            {
                return;
            }
            SDLWindowManager.RemoveWindow(this);
            this.DestroyDrawingArea();
            this.DestroyIconSurface();
            if (this.renderer != IntPtr.Zero)
            {
                SDL_DestroyRenderer(this.renderer);
            }
            this.renderer = IntPtr.Zero;
            SDL_DestroyWindow(this.window);
            this.window = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed
        {
            get { return this.window == IntPtr.Zero; }
        }
        #endregion

        internal readonly uint windowID;

        private IntPtr window;
        private IntPtr renderer;
        private IntPtr surface;
        private IntPtr texture;

        private IntPtr iconSurface;
        private byte[] iconPixels;
        private GCHandle iconPixelsHandle;

        private int width;
        private int height;
        private int x;
        private int y;
        private bool isVisible;
    }
}
