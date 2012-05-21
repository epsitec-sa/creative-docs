//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{
	

	/// <summary>
	/// The <see cref="EntityCache"/> class is used to cache <see cref="AbstractEntity"/> in
	/// memory and to store associated data with them.
	/// </summary>
	internal sealed class EntityCache
	{


		/// <summary>
		/// Builds a new empty <see cref="EntityCache"/>.
		/// </summary>
		/// <param name="entityTypeEngine">The <see cref="EntityTypeEngine"/> used by this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityTypeEngine"/> is <c>null</c>.</exception>
		public EntityCache(EntityTypeEngine entityTypeEngine)
		{
			entityTypeEngine.ThrowIfNull ("entityTypeEngine");

			this.entityTypeEngine = entityTypeEngine;
			this.entityIdToEntity = new Dictionary<long, AbstractEntity> ();
			this.entityIdToEntityKey = new Dictionary<long, EntityKey> ();
			this.entityKeyToEntity = new Dictionary<EntityKey, AbstractEntity> ();
			this.entityTypeIdToEntityModificationEntryId = new Dictionary<Druid, long> ();
			this.entityIdToEntityModificationEntryId = new Dictionary<long, long> ();
		}
		

		private EntityTypeEngine EntityTypeEngine
		{
			get
			{
				return this.entityTypeEngine;
			}
		}
		

		/// <summary>
		/// Adds an <see cref="AbstractEntity"/> to this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to add.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public void Add(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			long id = entity.GetEntitySerialId ();

			this.entityIdToEntity[id] = entity;
		}


		/// <summary>
		/// Removes an <see cref="AbstractEntity"/> and its associated data from this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to remove.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public void Remove(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			long id = entity.GetEntitySerialId ();

			this.entityIdToEntity.Remove (id);
			
			if (this.entityIdToEntityKey.ContainsKey (id))
			{
				EntityKey entityKey = this.entityIdToEntityKey[id];

				this.entityIdToEntityKey.Remove (id);
				this.entityKeyToEntity.Remove (entityKey);
			}

			if (this.entityIdToEntityModificationEntryId.ContainsKey (id))
			{
				this.entityIdToEntityModificationEntryId.Remove (id);
			}
		}


		/// <summary>
		/// Associates <paramref name="entity"/> with its <see cref="DbKey"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to associate with the <see cref="DbKey"/>.</param>
		/// <param name="key">The <see cref="DbKey"/> to associate with the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="entity"/> is null.
		/// If <paramref name="entity"/>is not in the cache.
		/// If <paramref name="key"/> is empty.
		/// </exception>
		public void DefineRowKey(AbstractEntity entity, DbKey key)
		{
			entity.ThrowIfNull ("entity");
			key.ThrowIf (k => k.IsEmpty, "key cannot be empty");

			long id = entity.GetEntitySerialId ();

			if (!this.entityIdToEntity.ContainsKey (id))
			{
				throw new System.ArgumentException ("Entity is not yet defined cache");
			}

			EntityKey entityKey = new EntityKey (entity, key);
			EntityKey normalizedEntityKey = entityKey.GetNormalizedEntityKey (this.EntityTypeEngine);

			this.entityIdToEntityKey[id] = normalizedEntityKey;
			this.entityKeyToEntity[normalizedEntityKey] = entity;
		}


		/// <summary>
		/// Associates the given log id to the given entity type.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> that identifies the entity type.</param>
		/// <param name="logId">The log id that must be associated with the entity type.</param>
		public void DefineEntityModificationEntryId(Druid entityTypeId, long logId)
		{
			this.entityTypeIdToEntityModificationEntryId[entityTypeId] = logId;
		}


		/// <summary>
		/// Associates the given log id to the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to which associate the log id.</param>
		/// <param name="logId">The log id that must be associated with the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not defined in this instance.</exception>
		public void DefineEntityModificationEntryId(AbstractEntity entity, long logId)
		{
			entity.ThrowIfNull ("entity");

			long id = entity.GetEntitySerialId ();

			if (!this.entityIdToEntity.ContainsKey (id))
			{
				throw new System.ArgumentException ("Entity is not yet defined cache");
			}

			this.entityIdToEntityModificationEntryId[id] = logId;
		}


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is stored in this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose presence to check.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> is stored in the cache, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public bool ContainsEntity(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			long id = entity.GetEntitySerialId ();

			return this.entityIdToEntity.ContainsKey (id);
		}


		/// <summary>
		/// Gets the <see cref="EntityKey"/> associated to an <see cref="AbstractEntity"/> if there
		/// is any.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="EntityKey"/> to get.</param>
		/// <returns>The corresponding <see cref="EntityKey"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		public EntityKey? GetEntityKey(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			long id = entity.GetEntitySerialId ();

			if (this.entityIdToEntityKey.ContainsKey (id))
			{
				return this.entityIdToEntityKey[id];
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> corresponding to an <see cref="EntityKey"/> if there
		/// is any.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> whose <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The corresponding <see cref="AbstractEntity"/>.</returns>
		public AbstractEntity GetEntity(EntityKey entityKey)
		{
			EntityKey normalizedEntityKey = entityKey.GetNormalizedEntityKey (this.EntityTypeEngine);

			if (this.entityKeyToEntity.ContainsKey (normalizedEntityKey))
			{
				return this.entityKeyToEntity[normalizedEntityKey];
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Gets the log id currently associated with the given entity type.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> that defines the entity type.</param>
		/// <returns>The log is currently associated with the given entity type.</returns>
		public long? GetLogId(Druid entityTypeId)
		{
			if (this.entityTypeIdToEntityModificationEntryId.ContainsKey (entityTypeId))
			{
				return this.entityTypeIdToEntityModificationEntryId[entityTypeId];
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Gets the log id currently associated with the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose log id to get.</param>
		/// <returns>The log is currently associated with the given <see cref="AbstractEntity"/>.</returns>
		public long? GetLogId(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			long id = entity.GetEntitySerialId ();

			if (this.entityIdToEntityModificationEntryId.ContainsKey (id))
			{
				return this.entityIdToEntityModificationEntryId[id];
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Gets the smallest log id currently associated to an entity type known by this instance.
		/// </summary>
		/// <returns>The smallest log id.</returns>
		public long? GetMinimumLogId()
		{
			if (this.entityTypeIdToEntityModificationEntryId.Count == 0)
			{
				return null;
			}
			else
			{
				return this.entityTypeIdToEntityModificationEntryId.Values.Min ();
			}
		}


		/// <summary>
		/// Enumerates the <see cref="AbstractEntity"/> stored in this instance.
		/// </summary>
		/// <returns>The <see cref="AbstractEntity"/> stored in this instance.</returns>
		public IEnumerable<AbstractEntity> GetEntities()
		{
			return this.entityIdToEntity.Values;
		}


		/// <summary>
		/// Gets the sequence of <see cref="Druid"/> that defined all the entity types known by this
		/// instance.
		/// </summary>
		/// <returns>The sequence of <see cref="Druid"/>.</returns>
		public IEnumerable<Druid> GetEntityTypeIds()
		{
			return this.entityTypeIdToEntityModificationEntryId.Keys;
		}
		

		private readonly EntityTypeEngine entityTypeEngine;


		/// <summary>
		/// Maps the entity serial ids to the corresponding <see cref="AbstractEntity"/>.
		/// </summary>
		private readonly Dictionary<long, AbstractEntity> entityIdToEntity;


		/// <summary>
		/// Maps the entity serial ids to the corresponding <see cref="EntityKey"/>.
		/// </summary>
		private readonly Dictionary<long, EntityKey> entityIdToEntityKey;


		/// <summary>
		/// Maps the <see cref="EntityKey"/> to the corresponding <see cref="AbstractEntity"/>.
		/// </summary>
		private readonly Dictionary<EntityKey, AbstractEntity> entityKeyToEntity;


		/// <summary>
		/// Maps the <see cref="Druid"/> of the entity types known by this instance to their log id.
		/// </summary>
		private readonly Dictionary<Druid, long> entityTypeIdToEntityModificationEntryId;
		
		/// <summary>
		/// Maps the entity serial ids to their corresponding log sequence number.
		/// </summary>
		private readonly Dictionary<long, long> entityIdToEntityModificationEntryId;


	}


}
