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
    /// The <c>INotifyCollectionChangedProvider</c> interface gives access to
    /// the <see cref="INotifyCollectionChanged"/> interface.
    /// </summary>
    public interface INotifyCollectionChangedProvider
    {
        /// <summary>
        /// Gets the <see cref="INotifyCollectionChanged"/> interface which can
        /// be used to get the <c>CollectionChanged</c> events for the source.
        /// </summary>
        /// <returns>The <see cref="INotifyCollectionChanged"/> interface.</returns>
        INotifyCollectionChanged GetNotifyCollectionChangedSource();
    }
}
