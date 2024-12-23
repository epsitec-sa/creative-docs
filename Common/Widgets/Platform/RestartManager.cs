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


namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// The <c>RestartManager</c> class is used to handle automatic restart of
    /// an application, but also clean shutdown when the OS Restart Manager
    /// tells us to do so.
    /// </summary>
    internal static class RestartManager
    {
        // ******************************************************************
        // TODO bl-net8-cross
        // - see if we need this RestartManager at all
        // ******************************************************************
        internal static void Setup()
        {
            if (RestartManager.initialized)
            {
                return;
            }

            RestartManager.initialized = true;

            if (System.Environment.OSVersion.Version.Major > 5)
            {
                string commandLine = System.Environment.CommandLine;
                RestartManager.SetupVistaRestartManager(commandLine);
            }
            else
            {
                RestartManager.SetExceptionMode();
            }
        }

        internal static bool UseWindowsErrorReporting
        {
            get
            {
                return !System.Diagnostics.Debugger.IsAttached
                    && RestartManager.useWindowsErrorReporting;
            }
        }

        private static void SetExceptionMode()
        {
/*            if (Epsitec.Common.Debug.GeneralExceptionCatcher.IsActive)
            {
                //	Don't set the exception mode since we already have an exception
                //	handler in place.
            }
            else
            {
                try
                {
                    if (
                        System.AppDomain.CurrentDomain.GetData(
                            "System.Windows.Forms.Application.UnhandledExceptionMode"
                        ) == null
                    )
                    {
                        System.Windows.Forms.Application.SetUnhandledExceptionMode(
                            System.Windows.Forms.UnhandledExceptionMode.ThrowException
                        );
                    }
                }
                catch (System.InvalidOperationException)
                {
                    //	Never mind, if we cannot set the exception mode, assume the caller
                    //	did it before creating the first Form instances...
                }
            }
*/        }

        private static void SetupVistaRestartManager(string commandLine)
        {
            // bl-net8-cross
            /*
            RestartManager.SetExceptionMode();

            //	See http://www.danielmoth.com/Blog/2006/08/vista-registerapplicationrecoverycallb.html
            //	See http://www.danielmoth.com/Blog/2006/08/vista-application-recovery.html

            Win32Api.RegisterApplicationRestart(commandLine, 0);
            Win32Api.RegisterApplicationRecoveryCallback(
                RestartManager.RecoveryCallback,
                System.IntPtr.Zero,
                5 * 1000 * 1000 * 10,
                0
            );

            RestartManager.useWindowsErrorReporting = true;
            */
            throw new System.NotImplementedException();
        }

        private static int RecoveryCallback(System.IntPtr parameter)
        {
            // bl-net8-cross
            /*
            bool success = RestartManager.Recover();

            Win32Api.ApplicationRecoveryFinished(success);
            return 0;
            */
            throw new System.NotImplementedException();
        }

        private static bool Recover()
        {
            //	TODO: ...

            return true;
        }

        internal static bool ShouldCancelRecovery()
        {
            // bl-net8-cross
            /*
            bool canceled;
            Win32Api.ApplicationRecoveryInProgress(out canceled);
            return canceled;
            */
            throw new System.NotImplementedException();
        }

/*        internal static bool HandleWndProc(ref System.Windows.Forms.Message msg)
        {
            if (System.Environment.OSVersion.Version.Major > 5)
            {
                if (msg.Msg == Win32Const.WM_QUERYENDSESSION)
                {
                    return RestartManager.QueryEndSession(ref msg);
                }
                else if (msg.Msg == Win32Const.WM_ENDSESSION)
                {
                    return RestartManager.EndSession(ref msg);
                }
            }

            return false;
        }
*/
/*        private static bool QueryEndSession(ref System.Windows.Forms.Message msg)
        {
            int lParam = msg.LParam.ToInt32();

            if ((lParam & Win32Const.ENDSESSION_CLOSEAPP) != 0)
            {
                RestartManager.NotifyEndSessionPending();
                msg.Result = new System.IntPtr(1);
                return true;
            }

            return false;
        }

        private static bool EndSession(ref System.Windows.Forms.Message msg)
        {
            int wParam = msg.WParam.ToInt32();
            int lParam = msg.LParam.ToInt32();

            if ((lParam & Win32Const.ENDSESSION_CLOSEAPP) != 0)
            {
                if (wParam != 0)
                {
                    RestartManager.NotifySessionEnding();
                }

                msg.Result = new System.IntPtr(0);
                return true;
            }

            return false;
        }
*/
        private static void NotifyEndSessionPending()
        {
            //	TODO: ...
        }

        private static void NotifySessionEnding()
        {
            //System.Windows.Forms.Application.Exit();
        }

        private static bool initialized;
        private static bool useWindowsErrorReporting;
    }
}
