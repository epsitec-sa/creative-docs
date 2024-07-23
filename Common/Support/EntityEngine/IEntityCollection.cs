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


using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>IEntityCollection</c> interface is used by the generic
    /// <see cref="EntityCollection&lt;T&gt;"/> class to give access to
    /// the copy-on-write mechanisms.
    /// </summary>
    public interface IEntityCollection : INotifyCollectionChangedProvider, ISuspendCollectionChanged
    {
        /// <summary>
        /// Resets the collection to the unchanged copy on write state.
        /// </summary>
        void ResetCopyOnWrite();

        /// <summary>
        /// Copies the collection to a writable instance if the collection is
        /// still in the unchanged copy on write state.
        /// </summary>
        void CopyOnWrite();

        /// <summary>
        /// Gets a value indicating whether this collection will create a copy
        /// before being modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the collection has the copy on write state; otherwise, <c>false</c>.
        /// </value>
        bool HasCopyOnWriteState { get; }

        /// <summary>
        /// Gets the type of the items stored in this collection.
        /// </summary>
        /// <returns>The type of the items.</returns>
        System.Type GetItemType();

        /// <summary>
        /// Temporarily disables all change notifications. Any changes which
        /// happen until <c>Dispose</c> is called on the returned object will
        /// not generate events; they are simply lost.
        /// </summary>
        /// <returns>An object you will have to <c>Dispose</c> in order to re-enable
        /// the notifications.</returns>
        System.IDisposable DisableNotifications();
    }
}
