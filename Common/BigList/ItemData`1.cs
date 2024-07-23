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


namespace Epsitec.Common.BigList
{
    /// <summary>
    /// The <c>ItemData&lt;T&gt;</c> generic class stores a data item of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The native data type.</typeparam>
    public class ItemData<T> : ItemData
    {
        public ItemData(T data, ItemState state)
            : base(state)
        {
            this.data = data;
        }

        public T Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets the data stored in this instance.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <returns>
        /// The native data.
        /// </returns>
        /// <exception cref="System.ArgumentException">When the <typeparamref name="TData"/> does
        /// not match the real data type.</exception>
        public override TData GetData<TData>()
        {
            if (typeof(TData) == typeof(T))
            {
                return (TData)(object)this.data;
            }

            throw new System.ArgumentException(
                string.Format(
                    "Cannot cast type from {0} to {1}",
                    typeof(T).Name,
                    typeof(TData).Name
                )
            );
        }

        private readonly T data;
    }
}
