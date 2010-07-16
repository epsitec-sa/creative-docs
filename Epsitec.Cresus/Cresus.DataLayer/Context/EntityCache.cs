//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	internal class EntityCache
	{

		public EntityCache()
		{
			this.entityIdToEntity = new Dictionary<long, AbstractEntity> ();
			this.entityIdToEntityKey = new Dictionary<long, EntityKey> ();
			this.entityKeyToEntity = new Dictionary<EntityKey, AbstractEntity> ();
		}


		public void Add(AbstractEntity entity)
		{
			long id = entity.GetEntitySerialId ();

			this.entityIdToEntity[id] = entity;
		}


		public void Remove(AbstractEntity entity)
		{
			long id = entity.GetEntitySerialId ();

			this.entityIdToEntity.Remove (id);

			if (this.entityIdToEntityKey.ContainsKey (id))
			{
				EntityKey entityKey = this.entityIdToEntityKey[id];

				this.entityIdToEntityKey.Remove (id);
				this.entityKeyToEntity.Remove (entityKey);
			}
		}


		public void DefineRowKey(AbstractEntity entity, DbKey key)
		{
			long id = entity.GetEntitySerialId ();

			if (!this.entityIdToEntity.ContainsKey (id))
			{
				throw new System.Exception ("Entity is not yet defined cache");
			}

			EntityKey entityKey = this.GetEntityKey (entity, key);

			this.entityIdToEntityKey[id] = entityKey;
			this.entityKeyToEntity[entityKey] = entity;
		}


		public bool ContainsEntity(AbstractEntity entity)
		{
			long id = entity.GetEntitySerialId ();

			return this.entityIdToEntity.ContainsKey (id);
		}


		public EntityKey? GetEntityKey(AbstractEntity entity)
		{
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


		public AbstractEntity GetEntity(EntityKey entityKey)
		{
			if (this.entityKeyToEntity.ContainsKey (entityKey))
			{
				return this.entityKeyToEntity[entityKey];
			}
			else
			{
				return null;
			}
		}


		public IEnumerable<AbstractEntity> GetEntities()
		{
			return this.entityIdToEntity.Values;
		}


		private EntityKey GetEntityKey(AbstractEntity entity, DbKey key)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return new EntityKey (key, leafEntityId);
		}


		private readonly Dictionary<long, AbstractEntity> entityIdToEntity;


		private readonly Dictionary<long, EntityKey> entityIdToEntityKey;


		private readonly Dictionary<EntityKey, AbstractEntity> entityKeyToEntity;


	}


}
