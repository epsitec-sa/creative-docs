//	Copyright © 2008-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;

using System.Runtime.InteropServices;

namespace Epsitec.Common.IO
{


    /// <summary>
    /// Little helper class that allows windows application to create a console that can be used
    /// for output and input.
    /// </summary>
    /// <remarks>
    /// Note that a process can have at most one console, so you can't create a console twice and
    /// trying to do so might be problematic.
    /// </remarks>
    public static class ConsoleCreatorOld
    {
        public static void Initialize()
        {
            ConsoleCreatorOld.CreateConsole ();
        }

        /// <summary>
        /// Creates a console, executes the given action and deletes the console afterwards. If the
        /// new console can't be created, an exception will be thrown and the action won't be
        /// executed.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="windowWidth">Width of the window (or zero).</param>
        public static void RunWithConsole(Action action, int windowWidth = 0)
        {
            bool success = false;

            try
            {
                success = ConsoleCreatorOld.CreateConsole ();

                if (!success)
                {
                    throw new Exception ("The console could not be created.");
                }

                if (windowWidth > 0)
                {
                    if (ConsoleCreatorOld.IsOutputRedirected == false)
                    {
                        try
                        {
                            System.Console.SetWindowSize (windowWidth, System.Console.WindowHeight);
                        }
                        catch (System.ArgumentOutOfRangeException ex)
                        {
                            var find = "maximum window size of ";
                            var message = ex.Message;
                            var pos = message.IndexOf (find);

                            System.Diagnostics.Trace.WriteLine ($"RunWithConsole: {message}");

                            if (pos > 0)
                            {
                                message = message.Substring (pos + find.Length);
                                pos = message.IndexOf (' ');
                                if (pos > 0)
                                {
                                    var value = int.Parse (message.Substring (0, pos), System.Globalization.CultureInfo.InvariantCulture);
                                    windowWidth = value;
                                    System.Diagnostics.Trace.WriteLine ($"Adjust to {windowWidth}");
                                    System.Console.SetWindowSize (windowWidth, System.Console.WindowHeight);
                                }
                            }
                        }
                        System.Console.SetBufferSize (windowWidth, System.Console.BufferHeight);
                    }
                }

                action ();
            }
            finally
            {
                if (success)
                {
                    ConsoleCreatorOld.DeleteConsole ();
                }
            }
        }

        /// <summary>
        /// Creates a console.
        /// </summary>
        /// <returns>return true for success and false for failure.</returns>
        internal static bool CreateConsole()
        {
            if (System.Threading.Interlocked.Increment (ref ConsoleCreatorOld.counter) == 1)
            {
                ConsoleCreatorOld.consoleAllocResult = ConsoleCreatorOld.AllocConsole ();

                var defaultHandle = new IntPtr (7);
                var currentHandle = ConsoleCreatorOld.GetStdHandle (ConsoleCreatorOld.StdOutputHandle);

                if (currentHandle != defaultHandle)
                {
                    //  Visual Studio has redirected our output, apparently, so we
                    //  need to restore a valid handle here in order to be able to
                    //  use SetCursorPosition, etc.

                    ConsoleCreatorOld.SetStdHandle (StdOutputHandle, defaultHandle);

                    System.Diagnostics.Trace.WriteLine ("AllocConsole: restored output to console");
                }

                var writer = new System.IO.StreamWriter (Console.OpenStandardOutput (), System.Text.Encoding.GetEncoding (850))
                {
                    AutoFlush = true
                };

                System.Diagnostics.Trace.WriteLine ("Console created");

                Console.SetOut (writer);

                System.Console.WriteLine ($"{System.DateTime.Now.ToShortDateString ()} {System.DateTime.Now.ToLongTimeString ()}");
                System.Console.SetCursorPosition (0, 0);
                System.Console.WriteLine ("                   ");
                System.Console.SetCursorPosition (0, 0);
            }

            return ConsoleCreatorOld.consoleAllocResult;
        }

        /// <summary>
        /// Deletes a console.
        /// </summary>
        /// <returns>return true for success and false for failure.</returns>
        internal static bool DeleteConsole()
        {
            if (System.Threading.Interlocked.Decrement (ref ConsoleCreatorOld.counter) == 0)
            {
                return ConsoleCreatorOld.FreeConsole ();
            }

            return true;
        }


        private static int counter;
        private static bool consoleAllocResult;



        private const uint StdOutputHandle = 0xFFFFFFF5;
        [DllImport ("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport ("kernel32.dll")]
        private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);

        [DllImport ("kernel32.dll", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport ("kernel32.dll", SetLastError = true)]
        [return: MarshalAs (UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        public static bool IsOutputRedirected => FileType.Char != GetFileType (GetStdHandle (StdHandle.Stdout));

        public static bool IsInputRedirected => FileType.Char != GetFileType (GetStdHandle (StdHandle.Stdin));

        public static bool IsErrorRedirected => FileType.Char != GetFileType (GetStdHandle (StdHandle.Stderr));

        // P/Invoke:
        private enum FileType { Unknown, Disk, Char, Pipe };
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };

        [DllImport ("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);

        [DllImport ("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
    }
}
