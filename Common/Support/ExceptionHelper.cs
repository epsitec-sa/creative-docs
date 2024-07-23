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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>ExceptionHelper</c> class provides support for managing
    /// exceptions.
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Preserves the full stack trace for the given exception, when it is
        /// either being rethrown using <code>throw;</code> or <code>throw ex;</code>.
        /// </summary>
        /// <param name="ex">The exception which will be rethrown.</param>
        public static void PreserveStackTrace(System.Exception ex)
        {
            System.Type exceptionType = typeof(System.Exception);
            System.Reflection.BindingFlags flags =
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.MethodInfo preserve = exceptionType.GetMethod(
                "InternalPreserveStackTrace",
                flags
            );

            if (preserve != null)
            {
                preserve.Invoke(ex, null);
            }
        }
    }
}
