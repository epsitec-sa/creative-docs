//	Copyright © 2008-2020, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;

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
    public static class ConsoleCreator
    {
        public static void Initialize()
        {
            ConsoleCreator.CreateConsole();
        }

        public static bool IsWin10 => (System.Environment.OSVersion.Version.Major >= 10);

        /// <summary>
        /// Creates a console, executes the given action and deletes the console afterwards. If the
        /// new console can't be created, an exception will be thrown and the action won't be
        /// executed.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="windowWidth">Width of the window (or zero).</param>
        public static void RunWithConsole(Action action, int windowWidth = 0)
        {
            if (ConsoleCreator.IsWin10)
            {
                ConsoleCreator.RunWithConsoleWin10(action, windowWidth);
            }
            else
            {
                ConsoleCreatorOld.RunWithConsole(action, windowWidth);
            }
        }

        public static void SetCursorPosition(int x, int y)
        {
            if (!ConsoleCreator.disableSetCursor)
            {
                System.Console.SetCursorPosition(x, y);
            }
        }

        /// <summary>
        /// Creates a console.
        /// </summary>
        /// <returns>return true for success and false for failure.</returns>
        private static bool CreateConsole()
        {
            return ConsoleCreator.IsWin10
                ? ConsoleCreator.CreateConsoleWin10()
                : ConsoleCreatorOld.CreateConsole();
        }

        /// <summary>
        /// Deletes a console.
        /// </summary>
        /// <returns>return true for success and false for failure.</returns>
        private static bool DeleteConsole()
        {
            return ConsoleCreator.IsWin10
                ? ConsoleCreator.DeleteConsoleWin10()
                : ConsoleCreatorOld.DeleteConsole();
        }

        private static void RunWithConsoleWin10(Action action, int windowWidth = 0)
        {
            bool success = false;

            try
            {
                success = ConsoleCreator.CreateConsole();

                if (!success)
                {
                    throw new Exception("The console could not be created.");
                }

                if (windowWidth > 0)
                {
                    if (WinConsole.IsConsoleRedirected == false)
                    {
                        try
                        {
                            System.Console.SetWindowSize(windowWidth, System.Console.WindowHeight);
                        }
                        catch (System.ArgumentOutOfRangeException ex)
                        {
                            var find = "maximum window size of ";
                            var message = ex.Message;
                            var pos = message.IndexOf(find);

                            System.Diagnostics.Trace.WriteLine($"RunWithConsole: {message}");

                            if (pos > 0)
                            {
                                message = message.Substring(pos + find.Length);
                                pos = message.IndexOf(' ');
                                if (pos > 0)
                                {
                                    var value = int.Parse(
                                        message.Substring(0, pos),
                                        System.Globalization.CultureInfo.InvariantCulture
                                    );
                                    windowWidth = value;
                                    System.Diagnostics.Trace.WriteLine($"Adjust to {windowWidth}");
                                    System.Console.SetWindowSize(
                                        windowWidth,
                                        System.Console.WindowHeight
                                    );
                                }
                            }
                        }

                        System.Console.SetBufferSize(windowWidth, System.Console.BufferHeight);
                    }
                }

                action();
            }
            finally
            {
                if (success)
                {
                    ConsoleCreator.DeleteConsole();
                }
            }
        }

        private static bool CreateConsoleWin10()
        {
            if (System.Threading.Interlocked.Increment(ref ConsoleCreator.counter) == 1)
            {
                ConsoleCreator.consoleAllocResult = WinConsole.Initialize(false);

                if (ConsoleCreator.consoleAllocResult)
                {
                    try
                    {
                        System.Console.WriteLine(
                            $"{System.DateTime.Now.ToShortDateString()} {System.DateTime.Now.ToLongTimeString()}"
                        );
                        System.Console.SetCursorPosition(0, 0);
                        System.Console.WriteLine("                   ");
                        System.Console.SetCursorPosition(0, 0);
                    }
                    catch (System.IO.IOException)
                    {
                        ConsoleCreator.disableSetCursor = true;
                    }
                }
            }

            return ConsoleCreator.consoleAllocResult;
        }

        private static bool DeleteConsoleWin10()
        {
            if (System.Threading.Interlocked.Decrement(ref ConsoleCreator.counter) == 0)
            {
                return WinConsole.Free();
            }
            else
            {
                return true;
            }
        }

        private static int counter;
        private static bool consoleAllocResult;
        private static bool disableSetCursor;
    }
}
