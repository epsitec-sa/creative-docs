using System;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform.SDLWrapper
{
    internal class SDLUtils
    {
        public static bool ToBool(SDL_bool value)
        {
            switch (value)
            {
                case SDL_bool.SDL_FALSE:
                    return false;
                case SDL_bool.SDL_TRUE:
                    return true;
                default:
                    throw new ArgumentException($"Invalid SDL_bool  {value}");
            }
        }

        public static SDL_bool ToSDLBool(bool value)
        {
            return value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        }
    }

    internal class SDLException : Exception
    {
        public SDLException(string message)
            : base(message) { }
    }
}
