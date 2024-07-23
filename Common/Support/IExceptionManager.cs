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
    /// The <c>IExceptionManager</c> interface provides a generic means of
    /// executing code which might throw exceptions and for which special
    /// handling is required (i.e. eat the exception and log the error).
    /// </summary>
    public interface IExceptionManager
    {
        /// <summary>
        /// Executes the specified function with special exception handling.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function to execute.</param>
        /// <param name="logSourceInfoGetter">The function to call in order
        /// to get source information if logging is required.</param>
        /// <returns>The result of the function execution.</returns>
        TResult Execute<TResult>(
            System.Func<TResult> func,
            System.Func<string> logSourceInfoGetter
        );
    }
}
