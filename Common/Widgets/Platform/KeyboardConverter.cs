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

﻿using System.Collections.Generic;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform
{
    internal class KeyboardConverter
    {
        public static KeyCode ConvertKeycodeFromSDL(SDL_Keycode sdlKeycode)
        {
            KeyCode keycode;
            if (!KeyboardConverter.keycodeFromSDL.TryGetValue(sdlKeycode, out keycode))
            {
                return KeyCode.None;
            }
            return keycode;
        }

        public static ModifierKeys ConvertModifiersFromSDL(SDL_Keymod sdlKeymod)
        {
            ModifierKeys modifiers = ModifierKeys.None;
            if (sdlKeymod.HasFlag(SDL_Keymod.KMOD_SHIFT))
            {
                modifiers |= ModifierKeys.Shift;
            }
            if (sdlKeymod.HasFlag(SDL_Keymod.KMOD_CTRL))
            {
                modifiers |= ModifierKeys.Control;
            }
            if (sdlKeymod.HasFlag(SDL_Keymod.KMOD_ALT))
            {
                modifiers |= ModifierKeys.Alt;
            }
            return modifiers;
        }

        private static readonly Dictionary<SDL_Keycode, KeyCode> keycodeFromSDL = new Dictionary<
            SDL_Keycode,
            KeyCode
        >()
        {
            { SDL_Keycode.SDLK_UNKNOWN, KeyCode.None },
            { SDL_Keycode.SDLK_RETURN, KeyCode.Return },
            { SDL_Keycode.SDLK_ESCAPE, KeyCode.Escape },
            { SDL_Keycode.SDLK_BACKSPACE, KeyCode.Back },
            { SDL_Keycode.SDLK_TAB, KeyCode.Tab },
            { SDL_Keycode.SDLK_SPACE, KeyCode.Space },
            //{ SDL_Keycode.SDLK_EXCLAIM , KeyCode. },
            //{ SDL_Keycode.SDLK_QUOTEDBL , KeyCode. },
            //{ SDL_Keycode.SDLK_HASH , KeyCode. },
            //{ SDL_Keycode.SDLK_PERCENT , KeyCode. },
            //{ SDL_Keycode.SDLK_DOLLAR , KeyCode. },
            //{ SDL_Keycode.SDLK_AMPERSAND , KeyCode. },
            //{ SDL_Keycode.SDLK_QUOTE , KeyCode. },
            //{ SDL_Keycode.SDLK_LEFTPAREN , KeyCode. },
            //{ SDL_Keycode.SDLK_RIGHTPAREN , KeyCode. },
            //{ SDL_Keycode.SDLK_ASTERISK , KeyCode. },
            { SDL_Keycode.SDLK_PLUS, KeyCode.NumericAdd },
            { SDL_Keycode.SDLK_COMMA, KeyCode.Comma },
            { SDL_Keycode.SDLK_MINUS, KeyCode.NumericSubtract },
            { SDL_Keycode.SDLK_PERIOD, KeyCode.Dot },
            { SDL_Keycode.SDLK_SLASH, KeyCode.NumericDivide },
            { SDL_Keycode.SDLK_0, KeyCode.Digit0 },
            { SDL_Keycode.SDLK_1, KeyCode.Digit1 },
            { SDL_Keycode.SDLK_2, KeyCode.Digit2 },
            { SDL_Keycode.SDLK_3, KeyCode.Digit3 },
            { SDL_Keycode.SDLK_4, KeyCode.Digit4 },
            { SDL_Keycode.SDLK_5, KeyCode.Digit5 },
            { SDL_Keycode.SDLK_6, KeyCode.Digit6 },
            { SDL_Keycode.SDLK_7, KeyCode.Digit7 },
            { SDL_Keycode.SDLK_8, KeyCode.Digit8 },
            { SDL_Keycode.SDLK_9, KeyCode.Digit9 },
            //{ SDL_Keycode.SDLK_COLON , KeyCode. },
            //{ SDL_Keycode.SDLK_SEMICOLON , KeyCode. },
            //{ SDL_Keycode.SDLK_LESS , KeyCode. },
            //{ SDL_Keycode.SDLK_EQUALS , KeyCode. },
            //{ SDL_Keycode.SDLK_GREATER , KeyCode. },
            //{ SDL_Keycode.SDLK_QUESTION , KeyCode. },
            //{ SDL_Keycode.SDLK_AT , KeyCode. },
            //{ SDL_Keycode.SDLK_LEFTBRACKET , KeyCode. },
            { SDL_Keycode.SDLK_BACKSLASH, KeyCode.OemBackslash },
            //{ SDL_Keycode.SDLK_RIGHTBRACKET , KeyCode. },
            //{ SDL_Keycode.SDLK_CARET , KeyCode. },
            //{ SDL_Keycode.SDLK_UNDERSCORE , KeyCode. },
            //{ SDL_Keycode.SDLK_BACKQUOTE , KeyCode. },
            { SDL_Keycode.SDLK_a, KeyCode.AlphaA },
            { SDL_Keycode.SDLK_b, KeyCode.AlphaB },
            { SDL_Keycode.SDLK_c, KeyCode.AlphaC },
            { SDL_Keycode.SDLK_d, KeyCode.AlphaD },
            { SDL_Keycode.SDLK_e, KeyCode.AlphaE },
            { SDL_Keycode.SDLK_f, KeyCode.AlphaF },
            { SDL_Keycode.SDLK_g, KeyCode.AlphaG },
            { SDL_Keycode.SDLK_h, KeyCode.AlphaH },
            { SDL_Keycode.SDLK_i, KeyCode.AlphaI },
            { SDL_Keycode.SDLK_j, KeyCode.AlphaJ },
            { SDL_Keycode.SDLK_k, KeyCode.AlphaK },
            { SDL_Keycode.SDLK_l, KeyCode.AlphaL },
            { SDL_Keycode.SDLK_m, KeyCode.AlphaM },
            { SDL_Keycode.SDLK_n, KeyCode.AlphaN },
            { SDL_Keycode.SDLK_o, KeyCode.AlphaO },
            { SDL_Keycode.SDLK_p, KeyCode.AlphaP },
            { SDL_Keycode.SDLK_q, KeyCode.AlphaQ },
            { SDL_Keycode.SDLK_r, KeyCode.AlphaR },
            { SDL_Keycode.SDLK_s, KeyCode.AlphaS },
            { SDL_Keycode.SDLK_t, KeyCode.AlphaT },
            { SDL_Keycode.SDLK_u, KeyCode.AlphaU },
            { SDL_Keycode.SDLK_v, KeyCode.AlphaV },
            { SDL_Keycode.SDLK_w, KeyCode.AlphaW },
            { SDL_Keycode.SDLK_x, KeyCode.AlphaX },
            { SDL_Keycode.SDLK_y, KeyCode.AlphaY },
            { SDL_Keycode.SDLK_z, KeyCode.AlphaZ },
            { SDL_Keycode.SDLK_CAPSLOCK, KeyCode.CapsLock },
            { SDL_Keycode.SDLK_F1, KeyCode.FuncF1 },
            { SDL_Keycode.SDLK_F2, KeyCode.FuncF2 },
            { SDL_Keycode.SDLK_F3, KeyCode.FuncF3 },
            { SDL_Keycode.SDLK_F4, KeyCode.FuncF4 },
            { SDL_Keycode.SDLK_F5, KeyCode.FuncF5 },
            { SDL_Keycode.SDLK_F6, KeyCode.FuncF6 },
            { SDL_Keycode.SDLK_F7, KeyCode.FuncF7 },
            { SDL_Keycode.SDLK_F8, KeyCode.FuncF8 },
            { SDL_Keycode.SDLK_F9, KeyCode.FuncF9 },
            { SDL_Keycode.SDLK_F10, KeyCode.FuncF10 },
            { SDL_Keycode.SDLK_F11, KeyCode.FuncF11 },
            { SDL_Keycode.SDLK_F12, KeyCode.FuncF12 },
            //{ SDL_Keycode.SDLK_PRINTSCREEN , KeyCode. },
            { SDL_Keycode.SDLK_SCROLLLOCK, KeyCode.ScrollLock },
            { SDL_Keycode.SDLK_PAUSE, KeyCode.Pause },
            { SDL_Keycode.SDLK_INSERT, KeyCode.Insert },
            { SDL_Keycode.SDLK_HOME, KeyCode.Home },
            { SDL_Keycode.SDLK_PAGEUP, KeyCode.PageUp },
            { SDL_Keycode.SDLK_DELETE, KeyCode.Delete },
            { SDL_Keycode.SDLK_END, KeyCode.End },
            { SDL_Keycode.SDLK_PAGEDOWN, KeyCode.PageDown },
            { SDL_Keycode.SDLK_RIGHT, KeyCode.ArrowRight },
            { SDL_Keycode.SDLK_LEFT, KeyCode.ArrowLeft },
            { SDL_Keycode.SDLK_DOWN, KeyCode.ArrowDown },
            { SDL_Keycode.SDLK_UP, KeyCode.ArrowUp },
            //{ SDL_Keycode.SDLK_NUMLOCKCLEAR , KeyCode. },
            { SDL_Keycode.SDLK_KP_DIVIDE, KeyCode.NumericDivide },
            { SDL_Keycode.SDLK_KP_MULTIPLY, KeyCode.NumericMultiply },
            { SDL_Keycode.SDLK_KP_MINUS, KeyCode.NumericSubtract },
            { SDL_Keycode.SDLK_KP_PLUS, KeyCode.NumericAdd },
            { SDL_Keycode.SDLK_KP_ENTER, KeyCode.NumericEnter },
            { SDL_Keycode.SDLK_KP_1, KeyCode.Digit1 },
            { SDL_Keycode.SDLK_KP_2, KeyCode.Digit2 },
            { SDL_Keycode.SDLK_KP_3, KeyCode.Digit3 },
            { SDL_Keycode.SDLK_KP_4, KeyCode.Digit4 },
            { SDL_Keycode.SDLK_KP_5, KeyCode.Digit5 },
            { SDL_Keycode.SDLK_KP_6, KeyCode.Digit6 },
            { SDL_Keycode.SDLK_KP_7, KeyCode.Digit7 },
            { SDL_Keycode.SDLK_KP_8, KeyCode.Digit8 },
            { SDL_Keycode.SDLK_KP_9, KeyCode.Digit9 },
            { SDL_Keycode.SDLK_KP_0, KeyCode.Digit0 },
            { SDL_Keycode.SDLK_KP_PERIOD, KeyCode.Dot },
            //{ SDL_Keycode.SDLK_APPLICATION , KeyCode. },
            //{ SDL_Keycode.SDLK_POWER , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_EQUALS , KeyCode. },
            { SDL_Keycode.SDLK_F13, KeyCode.FuncF13 },
            { SDL_Keycode.SDLK_F14, KeyCode.FuncF14 },
            { SDL_Keycode.SDLK_F15, KeyCode.FuncF15 },
            { SDL_Keycode.SDLK_F16, KeyCode.FuncF16 },
            { SDL_Keycode.SDLK_F17, KeyCode.FuncF17 },
            { SDL_Keycode.SDLK_F18, KeyCode.FuncF18 },
            { SDL_Keycode.SDLK_F19, KeyCode.FuncF19 },
            { SDL_Keycode.SDLK_F20, KeyCode.FuncF20 },
            { SDL_Keycode.SDLK_F21, KeyCode.FuncF21 },
            { SDL_Keycode.SDLK_F22, KeyCode.FuncF22 },
            { SDL_Keycode.SDLK_F23, KeyCode.FuncF23 },
            { SDL_Keycode.SDLK_F24, KeyCode.FuncF24 },
            //{ SDL_Keycode.SDLK_EXECUTE , KeyCode. },
            //{ SDL_Keycode.SDLK_HELP , KeyCode. },
            //{ SDL_Keycode.SDLK_MENU , KeyCode. },
            //{ SDL_Keycode.SDLK_SELECT , KeyCode. },
            //{ SDL_Keycode.SDLK_STOP , KeyCode. },
            //{ SDL_Keycode.SDLK_AGAIN , KeyCode. },
            //{ SDL_Keycode.SDLK_UNDO , KeyCode. },
            //{ SDL_Keycode.SDLK_CUT , KeyCode. },
            //{ SDL_Keycode.SDLK_COPY , KeyCode. },
            //{ SDL_Keycode.SDLK_PASTE , KeyCode. },
            //{ SDL_Keycode.SDLK_FIND , KeyCode. },
            //{ SDL_Keycode.SDLK_MUTE , KeyCode. },
            //{ SDL_Keycode.SDLK_VOLUMEUP , KeyCode. },
            //{ SDL_Keycode.SDLK_VOLUMEDOWN , KeyCode. },
            { SDL_Keycode.SDLK_KP_COMMA, KeyCode.Comma },
            //{ SDL_Keycode.SDLK_KP_EQUALSAS400 , KeyCode. },
            //{ SDL_Keycode.SDLK_ALTERASE , KeyCode. },
            //{ SDL_Keycode.SDLK_SYSREQ , KeyCode. },
            //{ SDL_Keycode.SDLK_CANCEL , KeyCode. },
            { SDL_Keycode.SDLK_CLEAR, KeyCode.Clear },
            //{ SDL_Keycode.SDLK_PRIOR , KeyCode. },
            //{ SDL_Keycode.SDLK_RETURN2 , KeyCode. },
            //{ SDL_Keycode.SDLK_SEPARATOR , KeyCode. },
            //{ SDL_Keycode.SDLK_OUT , KeyCode. },
            //{ SDL_Keycode.SDLK_OPER , KeyCode. },
            //{ SDL_Keycode.SDLK_CLEARAGAIN , KeyCode. },
            //{ SDL_Keycode.SDLK_CRSEL , KeyCode. },
            //{ SDL_Keycode.SDLK_EXSEL , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_00 , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_000 , KeyCode. },
            //{ SDL_Keycode.SDLK_THOUSANDSSEPARATOR , KeyCode. },
            //{ SDL_Keycode.SDLK_DECIMALSEPARATOR , KeyCode. },
            //{ SDL_Keycode.SDLK_CURRENCYUNIT , KeyCode. },
            //{ SDL_Keycode.SDLK_CURRENCYSUBUNIT , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_LEFTPAREN , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_RIGHTPAREN , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_LEFTBRACE , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_RIGHTBRACE , KeyCode. },
            { SDL_Keycode.SDLK_KP_TAB, KeyCode.Tab },
            { SDL_Keycode.SDLK_KP_BACKSPACE, KeyCode.Back },
            { SDL_Keycode.SDLK_KP_A, KeyCode.AlphaA },
            { SDL_Keycode.SDLK_KP_B, KeyCode.AlphaB },
            { SDL_Keycode.SDLK_KP_C, KeyCode.AlphaC },
            { SDL_Keycode.SDLK_KP_D, KeyCode.AlphaD },
            { SDL_Keycode.SDLK_KP_E, KeyCode.AlphaE },
            { SDL_Keycode.SDLK_KP_F, KeyCode.AlphaF },
            //{ SDL_Keycode.SDLK_KP_XOR , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_POWER , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_PERCENT , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_LESS , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_GREATER , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_AMPERSAND , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_DBLAMPERSAND , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_VERTICALBAR , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_DBLVERTICALBAR , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_COLON , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_HASH , KeyCode. },
            { SDL_Keycode.SDLK_KP_SPACE, KeyCode.Space },
            //{ SDL_Keycode.SDLK_KP_AT , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_EXCLAM , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMSTORE , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMRECALL , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMCLEAR , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMADD , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMSUBTRACT , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMMULTIPLY , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_MEMDIVIDE , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_PLUSMINUS , KeyCode. },
            { SDL_Keycode.SDLK_KP_CLEAR, KeyCode.Clear },
            //{ SDL_Keycode.SDLK_KP_CLEARENTRY , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_BINARY , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_OCTAL , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_DECIMAL , KeyCode. },
            //{ SDL_Keycode.SDLK_KP_HEXADECIMAL , KeyCode. },
            { SDL_Keycode.SDLK_LCTRL, KeyCode.ControlKeyLeft },
            { SDL_Keycode.SDLK_LSHIFT, KeyCode.ShiftKeyLeft },
            { SDL_Keycode.SDLK_LALT, KeyCode.AltKeyLeft },
            //{ SDL_Keycode.SDLK_LGUI , KeyCode. },
            { SDL_Keycode.SDLK_RCTRL, KeyCode.ControlKeyRight },
            { SDL_Keycode.SDLK_RSHIFT, KeyCode.ShiftKeyRight },
            { SDL_Keycode.SDLK_RALT, KeyCode.AltKeyRight },
            //{ SDL_Keycode.SDLK_RGUI , KeyCode. },
            //{ SDL_Keycode.SDLK_MODE , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIONEXT , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOPREV , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOSTOP , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOPLAY , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOMUTE , KeyCode. },
            //{ SDL_Keycode.SDLK_MEDIASELECT , KeyCode. },
            //{ SDL_Keycode.SDLK_WWW , KeyCode. },
            //{ SDL_Keycode.SDLK_MAIL , KeyCode. },
            //{ SDL_Keycode.SDLK_CALCULATOR , KeyCode. },
            //{ SDL_Keycode.SDLK_COMPUTER , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_SEARCH , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_HOME , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_BACK , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_FORWARD , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_STOP , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_REFRESH , KeyCode. },
            //{ SDL_Keycode.SDLK_AC_BOOKMARKS , KeyCode. },
            //{ SDL_Keycode.SDLK_BRIGHTNESSDOWN , KeyCode. },
            //{ SDL_Keycode.SDLK_BRIGHTNESSUP , KeyCode. },
            //{ SDL_Keycode.SDLK_DISPLAYSWITCH , KeyCode. },
            //{ SDL_Keycode.SDLK_KBDILLUMTOGGLE , KeyCode. },
            //{ SDL_Keycode.SDLK_KBDILLUMDOWN , KeyCode. },
            //{ SDL_Keycode.SDLK_KBDILLUMUP , KeyCode. },
            //{ SDL_Keycode.SDLK_EJECT , KeyCode. },
            //{ SDL_Keycode.SDLK_SLEEP , KeyCode. },
            //{ SDL_Keycode.SDLK_APP1 , KeyCode. },
            //{ SDL_Keycode.SDLK_APP2 , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOREWIND , KeyCode. },
            //{ SDL_Keycode.SDLK_AUDIOFASTFORWARD , KeyCode. }
        };
    }
}
