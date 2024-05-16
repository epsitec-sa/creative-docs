using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal abstract class SDLWindow : IDisposable
    {
        static void InitSDL()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                throw new SDLException(SDL_GetError());
            }
        }

        static void QuitSDL()
        {
            SDL_Quit();
        }

        internal SDLWindow(string windowTitle, int width, int height, SDL_WindowFlags flags)
        {
            Console.WriteLine("internal SDLWindow()");
            this.width = width;
            this.height = height;

            var window = SDL_CreateWindow(
                windowTitle,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                width,
                height,
                flags | SDL_WindowFlags.SDL_WINDOW_HIDDEN
            );
            if (window == IntPtr.Zero)
            {
                throw new SDLException(SDL_GetError());
            }
            this.window = window;
            this.UpdateWindowPosition();

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
            this.RecreateDrawingArea(width, height);
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public int WindowX
        {
            get { return this.x; }
        }

        public int WindowY
        {
            get { return this.y; }
        }

        public void SetPosition(int x, int y)
        {
            Console.WriteLine($"Set window position {x} {y}");
            SDL_SetWindowPosition(this.window, x, y);
            this.UpdateWindowPosition();
        }

        private void UpdateWindowPosition()
        {
            SDL_GetWindowPosition(this.window, out this.x, out this.y);
        }

        public void SetSize(int width, int height)
        {
            SDL_SetWindowSize(this.window, width, height);
        }

        public void SetBorder(bool border)
        {
            SDL_SetWindowBordered(this.window, SDLUtils.ToSDLBool(border));
        }

        public void SetResizable(bool resizable)
        {
            SDL_SetWindowResizable(this.window, SDLUtils.ToSDLBool(resizable));
        }

        public void SetFullscreen(bool fullscreen)
        {
            SDL_SetWindowFullscreen(
                this.window,
                fullscreen ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0
            );
        }

        public void SetTitle(string title)
        {
            SDL_SetWindowTitle(this.window, title);
        }

        public void SetWindowOpacity(float opacity)
        {
            SDL_SetWindowOpacity(this.window, opacity);
        }

        public void Show()
        {
            SDL_ShowWindow(this.window);
        }

        protected abstract void RecreateGraphicBuffer(
            IntPtr pixels,
            int width,
            int height,
            int stride
        );

        protected virtual void OnDraw() { }

        protected virtual void OnResize(int sx, int sy) { }

        public virtual void OnMouseButtonDown(int x, int y, int button) { }

        public virtual void OnMouseButtonUp(int x, int y, int button) { }

        public virtual void OnMouseMove(int x, int y) { }

        public void Run()
        {
            Console.WriteLine("SDLWindow run");
            while (true)
            {
                if (!this.ProcessEvents())
                {
                    break;
                }

                this.UpdateDrawing();
            }
            Console.WriteLine("SDLWindow run end");
        }

        private void UpdateDrawing()
        {
            this.OnDraw();

            var surfaceObj = (SDL_Surface)Marshal.PtrToStructure(this.surface, typeof(SDL_Surface));
            SDL_UpdateTexture(this.texture, IntPtr.Zero, surfaceObj.pixels, surfaceObj.pitch);

            if (SDL_SetRenderDrawColor(this.renderer, 0, 0, 0, 255) < 0)
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

        private bool ProcessEvents()
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                if (!this.HandleEvent(e))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Handle all sdl events
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if the application should continue normaly, false if it should terminate</returns>
        private bool HandleEvent(SDL_Event e)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    if (!this.HandleWindowEvent(e.window))
                    {
                        return false;
                    }
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    this.OnMouseButtonDown(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    this.OnMouseButtonUp(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    this.OnMouseMove(e.button.x, e.button.y);
                    break;
                default:
                    Console.WriteLine($"SDLWindow handle event {e.type}");
                    break;
            }
            return true;
        }

        /// <summary>
        /// Handle window manager related events (close, resize, maximized…)
        /// </summary>
        /// <param name="we"></param>
        /// <returns>true if the application should continue normaly, false if it should terminate</returns>
        private bool HandleWindowEvent(SDL_WindowEvent we)
        {
            switch (we.windowEvent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                    return false;
                case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                    this.RecreateDrawingArea(we.data1, we.data2);
                    this.OnResize(we.data1, we.data2);
                    return true;
                case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                    this.UpdateDrawing();
                    return true;
                default:
                    Console.WriteLine($"SDLWindow handle window event {we.windowEvent}");
                    return true;
            }
        }

        protected void CreateIconSurface(byte[] pixels, int width, int height)
        {
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

        private void RecreateDrawingArea(int width, int height)
        {
            this.DestroyDrawingArea();
            this.width = width;
            this.height = height;

            // bl-net8-cross
            // hardcoded pixelformat and stride value
            var pixelformat = SDL_PIXELFORMAT_ARGB8888;
            int stride = width * 4;

            this.surface = SDL_CreateRGBSurfaceWithFormat(0, width, height, 32, pixelformat);
            if (this.surface == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
            var surfaceObj = (SDL_Surface)Marshal.PtrToStructure(this.surface, typeof(SDL_Surface));
            this.RecreateGraphicBuffer(surfaceObj.pixels, width, height, stride);
            this.texture = SDL_CreateTexture(
                this.renderer,
                pixelformat,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                width,
                height
            );
            if (this.texture == 0)
            {
                throw new InvalidOperationException(SDL_GetError());
            }
        }

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

        public void Dispose()
        {
            Console.WriteLine("SDL destroy window");
            this.DestroyDrawingArea();
            this.DestroyIconSurface();
            SDL_DestroyRenderer(this.renderer);
            SDL_DestroyWindow(this.window);
        }

        private readonly IntPtr window;
        private readonly IntPtr renderer;
        private IntPtr surface;
        private IntPtr texture;

        private IntPtr iconSurface;
        private byte[] iconPixels;
        private GCHandle iconPixelsHandle;

        private int width;
        private int height;
        private int x;
        private int y;
    }
}
