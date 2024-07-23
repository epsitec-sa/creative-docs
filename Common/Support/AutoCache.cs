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


using System.Collections.Generic;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>AutoCache</c> class caches results of function calls. That is, when requested for a
    /// result, it computes it, caches it and never computes it back.
    /// </summary>
    /// <remarks>
    /// There are some restrictions with the functions that can be used with this class.
    /// - A function must be stable, that is, it must have the same result if called several times
    ///   with the same argument.
    /// - It might only have a single input argument and a single result.
    /// </remarks>
    /// <typeparam name="TKey">The type of the function argument.</typeparam>
    /// <typeparam name="TValue">The type of the function result.</typeparam>
    public sealed class AutoCache<TKey, TValue>
    {
        /*
         * As the Dictionary<TKey,TValue> class cannot contain a value for the null key, we have to
         * work around this with the fields resultOfCallWithNull and resultOfCallWithNullComputed
         * and the function GetResultForNullArgument in order to store the result of a call to the
         * computer function with a null argument.
         * Marc
         */


        /// <summary>
        /// Builds a new instance of the <c>AutoCache</c> class.
        /// </summary>
        /// <param name="computer">The function whose results must be cached.</param>
        /// <exception cref=""
        public AutoCache(System.Func<TKey, TValue> computer)
        {
            computer.ThrowIfNull("computer");

            this.resultOfCallWithNull = default(TValue);
            this.resultOfCallWithNullComputed = false;

            this.computer = computer;
            this.cache = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Computes the result of the computer function if it has not been computed yet and gives
        /// the result back.
        /// </summary>
        /// <param name="key">The value that must be passed as an argument to the computer function.</param>
        /// <returns>The (possibly cached) result of the computer function.</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    return this.GetResultForNullArgument(key);
                }
                else
                {
                    return this.GetResultForRegularArgument(key);
                }
            }
        }

        /// <summary>
        /// Clears the cache used by this instance.
        /// </summary>
        public void Clear()
        {
            this.cache.Clear();

            this.resultOfCallWithNull = default(TValue);
            this.resultOfCallWithNullComputed = false;
        }

        private TValue GetResultForNullArgument(TKey key)
        {
            if (!this.resultOfCallWithNullComputed)
            {
                this.resultOfCallWithNull = this.computer(key);
                this.resultOfCallWithNullComputed = true;
            }
            return this.resultOfCallWithNull;
        }

        private TValue GetResultForRegularArgument(TKey key)
        {
            TValue result = default(TValue);
            bool done = this.cache.TryGetValue(key, out result);

            if (!done)
            {
                result = this.computer(key);

                this.cache[key] = result;
            }

            return result;
        }

        private readonly System.Func<TKey, TValue> computer;
        private readonly IDictionary<TKey, TValue> cache;

        private TValue resultOfCallWithNull;
        private bool resultOfCallWithNullComputed;
    }
}
