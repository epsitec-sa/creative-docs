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
    /// The <c>ItemData</c> class is an abstract base class for <see cref="ItemData&lt;T&gt;"/>.
    /// It is used to cache data which is displayed in a big list, and store its state.
    /// </summary>
    public abstract class ItemData
    {
        protected ItemData(ItemState state)
        {
            this.state = state;
        }

        /// <summary>
        /// Gets the data stored in this instance.
        /// </summary>
        /// <exception cref="System.ArgumentException">When the <typeparamref name="TData"/> does
        /// not match the real data type.</exception>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <returns>The native data.</returns>
        public abstract TData GetData<TData>();

        public TState CreateState<TState>()
            where TState : ItemState, new()
        {
            return this.InitializeState(new TState()) as TState;
        }

        protected virtual ItemState InitializeState(ItemState state)
        {
            state.CopyFrom(this.state);

            return state;
        }

        protected ItemState state;
    }
}
