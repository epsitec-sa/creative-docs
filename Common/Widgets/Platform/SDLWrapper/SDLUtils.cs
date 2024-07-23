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
