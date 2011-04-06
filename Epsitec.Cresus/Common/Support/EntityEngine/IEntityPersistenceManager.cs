//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
