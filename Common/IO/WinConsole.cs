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

﻿//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Epsitec.Common.IO
{
    static class WinConsole
    {
        public static bool Initialize(bool alwaysCreateNewConsole = true)
        {
            bool consoleAttached = true;

            if (
                alwaysCreateNewConsole
                || (
                    WinConsole.AttachConsole(ATTACH_PARRENT) == 0
                    && Marshal.GetLastWin32Error() != ERROR_ACCESS_DENIED
                )
            )
            {
                consoleAttached = AllocConsole() != 0;

                if (consoleAttached)
                {
                    System.Diagnostics.Trace.WriteLine(
                        "AllocConsole: successfully created console"
                    );

                    WinConsole.SetupOutputStream();
                    WinConsole.SetupInputStream();

                    WinConsole.IsConsoleRedirected = true;
                }
            }

            return consoleAttached;
        }

        public static bool Free() => WinConsole.FreeConsole();

        public static bool IsConsoleRedirected { get; private set; }

        private static void SetupOutputStream()
        {
            var fs = WinConsole.CreateFileStream(
                "CONOUT$",
                GENERIC_WRITE,
                FILE_SHARE_WRITE,
                FileAccess.Write
            );

            if (fs != null)
            {
                var writer = new StreamWriter(fs, System.Text.Encoding.GetEncoding(850))
                {
                    AutoFlush = true
                };

                Console.SetOut(writer);
                Console.SetError(writer);
            }
        }

        private static void SetupInputStream()
        {
            var fs = WinConsole.CreateFileStream(
                "CONIN$",
                GENERIC_READ,
                FILE_SHARE_READ,
                FileAccess.Read
            );

            if (fs != null)
            {
                Console.SetIn(new StreamReader(fs));
            }
        }

        private static FileStream CreateFileStream(
            string name,
            uint win32DesiredAccess,
            uint win32ShareMode,
            FileAccess dotNetFileAccess
        )
        {
            var handle = WinConsole.CreateFileW(
                name,
                win32DesiredAccess,
                win32ShareMode,
                IntPtr.Zero,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            var file = new SafeFileHandle(handle, true);

            if (file.IsInvalid)
            {
                return null;
            }
            else
            {
                return new FileStream(file, dotNetFileAccess);
            }
        }

        #region Win API Functions and Constants

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport(
            "kernel32.dll",
            EntryPoint = "AttachConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall
        )]
        private static extern UInt32 AttachConsole(UInt32 dwProcessId);

        [DllImport(
            "kernel32.dll",
            EntryPoint = "CreateFileW",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall
        )]
        private static extern IntPtr CreateFileW(
            string lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 FILE_SHARE_READ = 0x00000001;
        private const UInt32 FILE_SHARE_WRITE = 0x00000002;
        private const UInt32 OPEN_EXISTING = 0x00000003;
        private const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        private const UInt32 ERROR_ACCESS_DENIED = 5;

        private const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;

        #endregion
    }
}
