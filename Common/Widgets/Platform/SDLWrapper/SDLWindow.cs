using System;
using SDL2;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal class SDLException : Exception
    {
        public SDLException(string message)
            : base(message) { }
    }

    internal class SDLWindow : IDisposable
    {
        static void InitSDL()
        {
            // Initilizes SDL.
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new SDLException(SDL.SDL_GetError());
            }
        }

        static void QuitSDL()
        {
            SDL.SDL_Quit();
        }

        internal SDLWindow(string windowTitle, int width, int height)
        {
            // Create a new window given a title, size, and passes it a flag indicating it should be shown.
            var window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                width,
                height,
                SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN
            );
            if (window == IntPtr.Zero)
            {
                throw new SDLException(SDL.SDL_GetError());
            }
            this.window = window;

            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            var renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED
                    | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (renderer == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    $"There was an issue creating the renderer. {SDL.SDL_GetError()}"
                );
            }
            this.renderer = renderer;
        }

        public void Show()
        {
            SDL.SDL_ShowWindow(this.window);
        }

        public void Run()
        {
            var running = true;

            Console.WriteLine("SDLWindow run");
            // Main loop for the program
            while (running)
            {
                if (!this.ProcessEvents())
                {
                    break;
                }

                // Sets the color that the screen will be cleared with.
                if (SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255) < 0)
                {
                    throw new InvalidOperationException(
                        $"There was an issue with setting the render draw color. {SDL.SDL_GetError()}"
                    );
                }

                // Clears the current render surface.
                if (SDL.SDL_RenderClear(renderer) < 0)
                {
                    throw new InvalidOperationException(
                        $"There was an issue with clearing the render surface. {SDL.SDL_GetError()}"
                    );
                }

                // Sets the color that the screen will be cleared with.
                if (SDL.SDL_SetRenderDrawColor(renderer, 250, 12, 50, 255) < 0)
                {
                    throw new InvalidOperationException(
                        $"There was an issue with setting the render draw color. {SDL.SDL_GetError()}"
                    );
                }

                var rect = new SDL.SDL_Rect
                {
                    x = rx,
                    y = ry,
                    w = 10,
                    h = 10
                };
                SDL.SDL_RenderFillRect(renderer, ref rect);

                // Switches out the currently presented render surface with the one we just did work on.
                SDL.SDL_RenderPresent(renderer);
            }
            Console.WriteLine("SDLWindow run end");
        }

        private bool ProcessEvents()
        {
            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                if (!this.HandleEvent(e))
                {
                    return false;
                }
            }
            return true;
        }

        private bool HandleEvent(SDL.SDL_Event e)
        {
            Console.WriteLine($"SDLWindow handle event {e.type}");
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE)
                    {
                        return false;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    rx = e.button.x;
                    ry = e.button.y;
                    break;
            }
            return true;
        }

        public void Dispose()
        {
            Console.WriteLine("SDL destroy window");
            // Clean up the resources that were created.
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
        }

        private IntPtr window;
        private IntPtr renderer;
        private int rx = 0; // TEMP
        private int ry = 0; // TEMP
    }
}
