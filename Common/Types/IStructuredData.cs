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

namespace Epsitec.Common.Types
{
    using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;

    /// <summary>
    /// The <c>IStructuredData</c> interface provides a <see cref="Binding"/>
    /// compatible way of accessing structured data (i.e. records, graphs, etc.)
    /// </summary>
    public interface IStructuredData : IValueStore
    {
        /// <summary>
        /// Attaches a listener to the specified structured value.
        /// </summary>
        /// <param name="id">The identifier of the value.</param>
        /// <param name="handler">The handler which implements the listener.</param>
        void AttachListener(string id, PropertyChangedEventHandler handler);

        /// <summary>
        /// Sets the value. See <see cref="IValueStore.SetValue"/> for additional
        /// details (the default mode will be used).
        /// </summary>
        /// <param name="id">The identifier of the value.</param>
        /// <param name="value">The value.</param>
        void SetValue(string id, object value);

        /// <summary>
        /// Detaches a listener from the specified structured value.
        /// </summary>
        /// <param name="id">The identifier of the value.</param>
        /// <param name="handler">The handler which implements the listener.</param>
        void DetachListener(string id, PropertyChangedEventHandler handler);

        /// <summary>
        /// Gets the collection of identifiers used to define the structured values.
        /// </summary>
        /// <returns>The collection of identifiers.</returns>
        IEnumerable<string> GetValueIds();
    }
}
