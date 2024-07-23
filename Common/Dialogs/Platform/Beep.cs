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

using System.Runtime.InteropServices;

namespace Epsitec.Common.Dialogs.Platform
{
    /// <summary>
    /// Summary description for Beep.
    /// </summary>
    public class Beep
    {
        public enum MessageType
        {
            Default = -1,
            Ok = 0x00000000,
            Error = 0x00000010,
            Question = 0x00000020,
            Warning = 0x00000030,
            Information = 0x00000040,
        }

        [DllImport("User32.dll", SetLastError = true, EntryPoint = "MessageBeep")]
        private static extern bool Win32MessageBeep(int beepType);

        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        private static extern void Win32Beep(int f, int d);

        public static void MessageBeep(MessageType type)
        {
            Beep.Win32MessageBeep((int)type);
        }
    }
}
