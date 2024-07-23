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


using Epsitec.Common.IO;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Debug
{
    /// <summary>
    /// The <c>ErrorLogger</c> class provides the tools to log errors and
    /// <see cref="System.Exception"/>s.
    /// </summary>
    public static class ErrorLogger
    {
        /// <summary>
        /// Builds a new <see cref="System.Exception"/> with <paramref name="message"/> as message,
        /// logs it to the default error log file and throws it.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <exception cref="System.Exception">Always.</exception>
        public static void LogAndThrowException(string message)
        {
            ErrorLogger.LogAndThrowException(
                new System.Exception(message),
                ErrorLogger.defaultErrorFile
            );
        }

        /// <summary>
        /// Builds a new <see cref="System.Exception"/> with <paramref name="message"/> as message,
        /// logs it to <paramref name="file"/> and throws it.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="file">The file where to log the error.</param>
        /// <exception cref="System.Exception">Always.</exception>
        public static void LogAndThrowException(string message, string file)
        {
            ErrorLogger.LogAndThrowException(new System.Exception(message), file);
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the default error log file and throws it.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log and throw.</param>
        /// <exception cref="System.Exception">Always.</exception>
        public static void LogAndThrowException(System.Exception exception)
        {
            ErrorLogger.LogAndThrowException(exception, ErrorLogger.defaultErrorFile);
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to <paramref name="file"/> and throws it.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log and throw.</param>
        /// <param name="file">The file where to log the error.</param>
        public static void LogAndThrowException(System.Exception exception, string file)
        {
            ErrorLogger.LogException(exception, file);
            throw exception;
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to the default error log file.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log.</param>
        public static void LogException(System.Exception exception)
        {
            ErrorLogger.LogException(exception, ErrorLogger.defaultErrorFile);
        }

        /// <summary>
        /// Logs <paramref name="exception"/> to file. The type of exception, its message,
        /// its source and its stack trace are written to the log file, recursively for the inner
        /// exception.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log.</param>
        /// <param name="file">The file where to log exception.</param>
        public static void LogException(System.Exception exception, string file)
        {
            string header = "========= New Exception =========";
            string error = exception.GetFullText();

            ErrorLogger.LogErrorMessage(header + "\n" + error, file);
        }

        /// <summary>
        /// Logs <paramref name="message"/> to the default error log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogErrorMessage(string message)
        {
            ErrorLogger.LogErrorMessage(message, ErrorLogger.defaultErrorFile);
        }

        /// <summary>
        /// Logs <paramref name="message"/> to <paramref name="file"/>.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="file">The file where to log message.</param>
        public static void LogErrorMessage(string message, string file)
        {
            Logger.Log(message, file);
        }

        /// <summary>
        /// The path of the default error file (UserAppData\Logs\logs.txt).
        /// </summary>
        /// <remarks>
        /// Cannot use <see cref="Epsitec.Common.Support.Globals.Directories.UserAppData"/> to build the path
        /// because Visual Studio cannot handle circular project references.
        /// </remarks>
        public static readonly string defaultErrorFile = string.Format(
            @"{0}\Logs\errors.txt",
            System.IO.Path.GetDirectoryName(Support.Globals.Directories.UserAppData)
        );
    }
}
