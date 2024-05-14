using System;
using System.Runtime.InteropServices;
using Epsitec.Common.Drawing;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal class SDLException : Exception
    {
        public SDLException(string message)
            : base(message) { }
    }

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

        internal SDLWindow(string windowTitle, int width, int height)
        {
            Console.WriteLine("internal SDLWindow()");
            this.width = width;
            this.height = height;

            var window = SDL_CreateWindow(
                windowTitle,
                SDL_WINDOWPOS_UNDEFINED,
                SDL_WINDOWPOS_UNDEFINED,
                width,
                height,
                SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE
            );
            if (window == IntPtr.Zero)
            {
                throw new SDLException(SDL_GetError());
            }
            this.window = window;

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

            // create rect with same dimensions
            //var rect = new SDL_Rect
            //{
            //    x = 0,
            //    y = 0,
            //    w = this.width,
            //    h = this.height
            //};
            //SDL_RenderFillRect(renderer, ref rect);

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
                    this.OnMouseButtonDown(e.button.x, FlipY(e.button.y), e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    this.OnMouseButtonUp(e.button.x, FlipY(e.button.y), e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    this.OnMouseMove(e.button.x, FlipY(e.button.y));
                    break;
                default:
                    Console.WriteLine($"SDLWindow handle event {e.type}");
                    break;
            }
            return true;
        }

        private int FlipY(int y)
        {
            return this.height - y;
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
                default:
                    return true;
            }
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
            SDL_DestroyRenderer(this.renderer);
            SDL_DestroyWindow(this.window);
        }

        private readonly IntPtr window;
        private readonly IntPtr renderer;
        private IntPtr surface;
        private IntPtr texture;

        private int width;
        private int height;
    }
}
