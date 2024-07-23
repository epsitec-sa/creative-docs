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


namespace Epsitec.Common.Types.Collections
{
    /// <summary>
    /// The <c>AbstractObservableList</c> class is used only to make a few
    /// non generic methods available to the generic <c>ObservableList</c>
    /// users, without having to specify a generic type parameter.
    /// </summary>
    public abstract class AbstractObservableList
    {
        /// <summary>
        /// Gets target for the specified collection changing event handler.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The target handler instance or <c>null</c>.</returns>
        public abstract object GetCollectionChangingTarget(int index);

        /// <summary>
        /// Gets target for the specified collection changed event handler.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The target handler instance or <c>null</c>.</returns>
        public abstract object GetCollectionChangedTarget(int index);
    }
}
