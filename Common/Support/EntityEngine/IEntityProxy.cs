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



using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>IEntityProxy</c> interface is used by the <see cref="IValueStore"/>
    /// implementer to translate between an entity proxy and its real instance.
    /// </summary>
    public interface IEntityProxy
    {
        /// <summary>
        /// Gets the real instance to be used when reading on this proxy.
        /// </summary>
        /// <param name="store">The value store.</param>
        /// <param name="id">The value id.</param>
        /// <returns>The real instance to be used.</returns>
        object GetReadEntityValue(IValueStore store, string id);

        /// <summary>
        /// Gets the real instance to be used when writing on this proxy.
        /// </summary>
        /// <param name="store">The value store.</param>
        /// <param name="id">The value id.</param>
        /// <returns>The real instance to be used.</returns>
        object GetWriteEntityValue(IValueStore store, string id);

        /// <summary>
        /// Checks if the write to the specified entity value should proceed
        /// normally or be discarded completely.
        /// </summary>
        /// <param name="store">The value store.</param>
        /// <param name="id">The value id.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value should be discarded; otherwise, <c>false</c>.</returns>
        bool DiscardWriteEntityValue(IValueStore store, string id, ref object value);

        /// <summary>
        /// Promotes the proxy to its real instance.
        /// </summary>
        /// <returns>The real instance.</returns>
        object PromoteToRealInstance();
    }
}
