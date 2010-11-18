//	Copyright � 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


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
		/// <param name="entityContext">The <see cref="EntityContext"/> used by this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityContext"/> is <c>null</c>.</exception>
		public EntityCache(EntityContext entityContext)
		{
			entityContext.ThrowIfNull ("entityContext");

			this.EntityContext = entityContext;

			this.entityIdToEntity = new Dictionary<long, AbstractEntity> ();
			this.entityIdToEntityKey = new Dictionary<long, EntityKey> ();
			this.entityKeyToEntity = new Dictionary<EntityKey, AbstractEntity> ();
			this.entityIdToLogSequenceNumber = new Dictionary<long, long> ();
		}


		/// <summary>
		/// The <see cref="EntityContext"/> associated with this instance.
		/// </summary>
		private EntityContext EntityContext
		{
			get;
			set;
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

			if (this.entityIdToLogSequenceNumber.ContainsKey (id))
			{
				this.entityIdToLogSequenceNumber.Remove (id);
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
			EntityKey normalizedEntityKey = entityKey.GetNormalizedEntityKey (this.EntityContext);

			this.entityIdToEntityKey[id] = normalizedEntityKey;
			this.entityKeyToEntity[normalizedEntityKey] = entity;
		}


		/// <summary>
		/// Associates the given log sequence number to the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to associate with the log sequence number.</param>
		/// <param name="logSequenceNumber">The value of the log sequence number to associate with the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="entity"/> is null.
		/// If <paramref name="entity"/>is not in the cache.
		/// </exception>
		public void DefineLogSequenceNumber(AbstractEntity entity, long logSequenceNumber)
		{
			entity.ThrowIfNull ("entity");

			long id = entity.GetEntitySerialId ();

			if (!this.entityIdToEntity.ContainsKey (id))
			{
				throw new System.ArgumentException ("Entity is not yet defined cache");
			}

			this.entityIdToLogSequenceNumber[id] = logSequenceNumber;
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
			if (this.entityKeyToEntity.ContainsKey (entityKey))
			{
				EntityKey normalizedEntityKey = entityKey.GetNormalizedEntityKey (this.EntityContext);

				return this.entityKeyToEntity[normalizedEntityKey];
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Gets the log sequence number corresponding th the given <see cref="AbstractEntity"/> if
		/// there is any.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose log sequence number to retrieve.</param>
		/// <returns>The log sequence number.</returns>
		public long? GetLogSequenceNumber(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			long id = entity.GetEntitySerialId ();

			if (this.entityIdToLogSequenceNumber.ContainsKey (id))
			{
				return this.entityIdToLogSequenceNumber[id];
			}
			else
			{
				return null;
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
		/// Maps the entity serial ids to their corresponding log sequence number.
		/// </summary>
		private readonly Dictionary<long, long> entityIdToLogSequenceNumber;


	}


}
