using System;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal class SDLWindowManager
    {
        static void Init()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                throw new SDLException(SDL_GetError());
            }
        }

        public static void AddWindow(SDLWindow window)
        {
            openWindows[window.windowID] = window;
        }

        public static void RemoveWindow(SDLWindow window)
        {
            openWindows.Remove(window.windowID);
        }

        /// <summary>
        /// Run the application event loop.
        /// This function is blocking until all windows are closed.
        /// </summary>
        public static void RunApplicationEventLoop()
        {
            Console.WriteLine("SDLWindowManager run");
            while (true)
            {
                SDLWindowManager.ProcessEvents();

                SDLWindowManager.UpdateDrawings();
                if (openWindows.Count == 0)
                {
                    break;
                }
            }
            Console.WriteLine("SDLWindowManager run end");
        }

        public static void UpdateDrawings()
        {
            foreach (var (_, window) in SDLWindowManager.openWindows)
            {
                window.UpdateDrawing();
            }
        }

        private static void ProcessEvents()
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                SDLWindowManager.HandleEvent(e);
            }
        }

        /// <summary>
        /// Handle all sdl events
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if the application should continue normaly, false if it should terminate</returns>
        private static void HandleEvent(SDL_Event e)
        {
            SDLWindow window;
            switch (e.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    window = openWindows[e.window.windowID];
                    window.HandleWindowEvent(e.window);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    window = openWindows[e.button.windowID];
                    window.OnMouseButtonDown(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    window = openWindows[e.button.windowID];
                    window.OnMouseButtonUp(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    window = openWindows[e.motion.windowID];
                    window.OnMouseMove(e.motion.x, e.motion.y);
                    break;
                default:
                    Console.WriteLine($"SDLWindow handle event {e.type}");
                    break;
            }
        }

        static void QuitSDL()
        {
            SDL_Quit();
        }

        private static Dictionary<uint, SDLWindow> openWindows = new Dictionary<uint, SDLWindow>();
    }
}
