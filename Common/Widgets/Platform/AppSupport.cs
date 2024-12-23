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
    internal static class AppSupport
    {
        /// <summary>
        /// Creates the semaphore and returns the start status.
        /// </summary>
        /// <param name="semaphoreName">Name of the semaphore.</param>
        /// <returns>The application start status.</returns>
        public static ApplicationStartStatus CreateSemaphore(string semaphoreName)
        {
            // bl-net8-cross
            /*
            AppSupport.ValidateSemaphoreName(semaphoreName);

            var result = Win32Api.CreateSemaphore(System.IntPtr.Zero, 0, 1, semaphoreName);
            int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            if (result == System.IntPtr.Zero)
            {
                throw new System.Exception(
                    string.Format(
                        "Could not create semaphore '{0}' : error {1} (0x{1:X8})",
                        semaphoreName,
                        error
                    )
                );
            }

            if (error == Win32Const.ERROR_ALREADY_EXISTS)
            {
                return ApplicationStartStatus.RunningConcurrently;
            }
            else
            {
                return ApplicationStartStatus.RunningAlone;
            }
            */
            return ApplicationStartStatus.RunningAlone;
        }

        /// <summary>
        /// Determines whether an application is running based on its semaphore
        /// name.
        /// </summary>
        /// <param name="semaphoreName">Name of the semaphore.</param>
        /// <returns>
        /// 	<c>true</c> if the application is running; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRunning(string semaphoreName)
        {
            // bl-net8-cross
            /*
            AppSupport.ValidateSemaphoreName(semaphoreName);

            var result = Win32Api.OpenSemaphore(0, 0, semaphoreName);

            if (result == System.IntPtr.Zero)
            {
                return false;
            }
            else
            {
                Win32Api.CloseHandle(result);
                return true;
            }
            */
            throw new System.NotImplementedException();
        }

        private static void ValidateSemaphoreName(string semaphoreName)
        {
            if (System.String.IsNullOrEmpty(semaphoreName))
            {
                throw new System.ArgumentException(
                    "Semaphore name is null or empty",
                    "semaphoreName"
                );
            }
            if (semaphoreName.Contains("\\"))
            {
                throw new System.ArgumentException("Semaphore name is not valid", "semaphoreName");
            }
        }
    }
}
