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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>Accessor</c> class is the base class for <see cref="Accessor{TResult}"/>.
    /// </summary>
    public abstract class Accessor
    {
        /// <summary>
        /// Creates an accessor based on two functions: the first returns the source object
        /// and the second returns the value based on the source.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sourceValueFunction">The function which returns the source object.</param>
        /// <param name="resultValueFunction">The function which returns the value based on the source.</param>
        /// <returns>The accessor.</returns>
        public static Accessor<TResult> Create<T, TResult>(
            System.Func<T> sourceValueFunction,
            System.Func<T, TResult> resultValueFunction
        )
            where T : new()
        {
            return new Accessor<TResult>(() => resultValueFunction(sourceValueFunction()));
        }
    }
}
