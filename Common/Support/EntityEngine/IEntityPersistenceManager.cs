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


namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>IEntityPersistenceManager</c> interface is used to map between
    /// entities and their persisted id (which could be a database key).
    /// </summary>
    public interface IEntityPersistenceManager
    {
        /// <summary>
        /// Gets the persisted id for the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The persisted id or <c>null</c>.</returns>
        string GetPersistedId(AbstractEntity entity);

        /// <summary>
        /// Gets the peristed entity for the specified id.
        /// </summary>
        /// <param name="id">The id (identifies the instance).</param>
        /// <returns>The persisted entity or <c>null</c>.</returns>
        AbstractEntity GetPersistedEntity(string id);
    }
}
