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


using Epsitec.Common.Types.Exceptions;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>IReadOnlyExtensions</c> class contains some helper methods related to
    /// <see cref="IReadOnly"/> instances.
    /// </summary>
    public static class IReadOnlyExtensions
    {
        /// <summary>
        /// Asserts that <paramref name="obj"/> is in read only mode.
        /// </summary>
        /// <param name="obj">The object whose state to check.</param>
        /// <exception cref="ReadOnlyException">If <paramref name="obj"/> is not in read only mode.</exception>
        public static void ThrowIfReadWrite(this IReadOnly obj)
        {
            if (!obj.IsReadOnly)
            {
                string message = "Object " + obj + "  is not read only.";

                throw new ReadOnlyException(obj, message);
            }
        }

        /// <summary>
        /// Asserts that <paramref name="obj"/> is not in read only mode.
        /// </summary>
        /// <param name="obj">The object whose state to check.</param>
        /// <exception cref="ReadOnlyException">If <paramref name="obj"/> is in read only mode.</exception>
        public static void ThrowIfReadOnly(this IReadOnly obj)
        {
            if (obj.IsReadOnly)
            {
                string message = string.Format("Object {0} is read only", obj);

                throw new ReadOnlyException(obj, message);
            }
        }
    }
}
