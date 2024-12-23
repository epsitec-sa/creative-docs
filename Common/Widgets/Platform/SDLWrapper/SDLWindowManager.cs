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
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal class SDLWindowManager
    {
        private static uint USER_EVENT;
        private static bool initDone = false;

        static void Init()
        {
            if (SDLWindowManager.initDone)
            {
                return;
            }
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                throw new SDLException(SDL_GetError());
            }
            SDLWindowManager.USER_EVENT = SDL_RegisterEvents(1);
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
        /// Run the main application event loop.
        /// This function is blocking until all windows are closed.
        /// </summary>
        public static void RunMainApplicationEventLoop()
        {
            SDLWindowManager.Init();
            Console.WriteLine("SDLWindowManager run");
            while (true)
            {
                SDLWindowManager.DoUpdateStep();
                if (SDLWindowManager.openWindows.Count == 0)
                {
                    break;
                }
            }
            Console.WriteLine("SDLWindowManager run end");
            SDLWindowManager.QuitSDL();
        }

        /// <summary>
        /// Run the event loop until <paramref name="modalWindow"/> is closed.
        /// This function is blocking.
        /// </summary>
        /// <param name="modalWindow"></param>
        public static void RunModalWindowEventLoop(SDLWindow modalWindow)
        {
            while (SDLWindowManager.openWindows.ContainsKey(modalWindow.windowID))
            {
                SDLWindowManager.DoUpdateStep();
            }
        }

        public static void UpdateDrawings()
        {
            foreach (var window in SDLWindowManager.openWindows.Values.ToArray())
            {
                window.UpdateDrawing();
            }
        }

        public static void PushUserEvent(int eventCode, SDLWindow? window)
        {
            SDL_Event ev = new SDL_Event();
            ev.type = (SDL_EventType)SDLWindowManager.USER_EVENT;
            SDL_UserEvent userEvent = new SDL_UserEvent();
            userEvent.type = SDLWindowManager.USER_EVENT;
            userEvent.timestamp = SDL_GetTicks();
            userEvent.windowID = window == null ? 0 : window.windowID;
            userEvent.code = eventCode;
            ev.user = userEvent;
            SDL_PushEvent(ref ev);
        }

        public static void ProcessEvents()
        {
            // wait for first event
            SDL_WaitEvent(out SDL_Event e);
            SDLWindowManager.HandleEvent(e);
            // handle other events, if any
            while (SDL_PollEvent(out e) == 1)
            {
                SDLWindowManager.HandleEvent(e);
            }
        }

        private static void DoUpdateStep()
        {
            SDLWindowManager.ProcessEvents();
            SDLWindowManager.UpdateDrawings();
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
                    window = SDLWindowManager.GetWindowFromId(e.window.windowID);
                    window?.HandleWindowEvent(e.window);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    window = SDLWindowManager.GetWindowFromId(e.button.windowID);
                    window?.OnMouseButtonDown(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    window = SDLWindowManager.GetWindowFromId(e.button.windowID);
                    window?.OnMouseButtonUp(e.button.x, e.button.y, e.button.button);
                    break;
                case SDL_EventType.SDL_MOUSEMOTION:
                    window = SDLWindowManager.GetWindowFromId(e.motion.windowID);
                    window?.OnMouseMove(e.motion.x, e.motion.y);
                    break;
                case SDL_EventType.SDL_MOUSEWHEEL:
                    window = SDLWindowManager.GetWindowFromId(e.wheel.windowID);
                    window?.OnMouseWheel(e.wheel.x, e.wheel.y);
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    window = SDLWindowManager.GetWindowFromId(e.key.windowID);
                    window?.OnKeyDown(e.key.keysym);
                    break;
                case SDL_EventType.SDL_KEYUP:
                    window = SDLWindowManager.GetWindowFromId(e.key.windowID);
                    window?.OnKeyUp(e.key.keysym);
                    break;
                default:
                    if (e.type == (SDL_EventType)SDLWindowManager.USER_EVENT)
                    {
                        window =
                            SDLWindowManager.GetWindowFromId(e.user.windowID)
                            ?? SDLWindowManager.GetAnyWindow();
                        window?.OnUserEvent(e.user.code);
                        return;
                    }
                    Console.WriteLine($"SDLWindowManager handle event {e.type}");
                    break;
            }
        }

        private static SDLWindow GetWindowFromId(uint winId)
        {
            return openWindows.GetValueOrDefault(winId, null);
        }

        private static SDLWindow GetAnyWindow()
        {
            foreach (var window in SDLWindowManager.openWindows.Values.ToArray())
            {
                return window;
            }
            return null;
        }

        static void QuitSDL()
        {
            SDL_Quit();
        }

        private static Dictionary<uint, SDLWindow> openWindows = new Dictionary<uint, SDLWindow>();
    }
}
