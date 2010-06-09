//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataContext</c> class provides a context in which entities can
	/// be loaded from the database, modified and then saved back.
	/// </summary>
	public partial class DataContext
	{
		/// <summary>
		/// The <c>EntityDataCache</c> class provides a centralized cache for
		/// maintaining relations between entity instances in memory and in the
		/// database.
		/// </summary>
		class EntityDataCache
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="EntityDataCache"/> class.
			/// </summary>
			public EntityDataCache()
			{
				this.idToEntityMapping = new Dictionary<long, EntityDataMapping> ();
				this.lookup = new Dictionary<EntityDataMapping, EntityDataMapping> ();
				this.mappings = new List<EntityDataMapping> ();
				this.entities = new HashSet<AbstractEntity> ();
			}

			/// <summary>
			/// Finds the entity mapping for the specified entity serial id.
			/// </summary>
			/// <param name="entitySerialId">The entity serial id.</param>
			/// <returns>The entity mapping or <c>null</c> if the entity is not
			/// known.</returns>
			public EntityDataMapping FindMapping(long entitySerialId)
			{
				EntityDataMapping mapping;

				if (this.idToEntityMapping.TryGetValue (entitySerialId, out mapping))
				{
					return mapping;
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// Adds the specified entity into the cache.
			/// </summary>
			/// <param name="entitySerialId">The entity serial id.</param>
			/// <param name="mapping">The entity mapping.</param>
			public void Add(long entitySerialId, EntityDataMapping mapping)
			{
				System.Diagnostics.Debug.Assert (!this.entities.Contains (mapping.Entity));
				
				this.idToEntityMapping.Add (entitySerialId, mapping);
				this.mappings.Add (mapping);
				this.entities.Add (mapping.Entity);

				//	If the mapping is read only, then this means that the row key
				//	is already defined and that the mapping's hash value won't
				//	change; we can safely add it to our internal lookup table :

				if (mapping.IsReadOnly)
				{
					this.lookup.Add (mapping, mapping);
				}
			}

			/// <summary>
			/// Removes the specified entity from the cache.
			/// </summary>
			/// <param name="entitySerialId">The entity serial id.</param>
			public void Remove(long entitySerialId)
			{
				EntityDataMapping mapping;

				if (this.idToEntityMapping.TryGetValue (entitySerialId, out mapping))
				{
					if (this.isIteratingList > 0)
					{
						throw new System.InvalidOperationException ("Cannot remove item while an iteration is executing");
					}

					System.Diagnostics.Debug.Assert (this.entities.Contains (mapping.Entity));
					
					this.idToEntityMapping.Remove (entitySerialId);
					this.mappings.Remove (mapping);
					this.entities.Remove (mapping.Entity);

					if (mapping.IsReadOnly)
					{
						this.lookup.Remove (mapping);
					}
				}
			}

			/// <summary>
			/// Defines the database row key for the specified entity mapping.
			/// </summary>
			/// <param name="mapping">The entity mapping.</param>
			/// <param name="key">The row key.</param>
			public void DefineRowKey(EntityDataMapping mapping, DbKey key)
			{
				System.Diagnostics.Debug.Assert (mapping.IsReadOnly == false);
				//System.Diagnostics.Debug.Assert (this.list.Contains (mapping));

				mapping.RowKey = key;

				System.Diagnostics.Debug.Assert (mapping.IsReadOnly);

				this.lookup.Add (mapping, mapping);
			}

			/// <summary>
			/// Gets the number of entities currently stored in the cache.
			/// </summary>
			/// <value>The number of entities in the cache.</value>
			public int Count
			{
				get
				{
					return this.mappings.Count;
				}
			}

			/// <summary>
			/// Finds the entity, based on its database identity.
			/// </summary>
			/// <param name="rowKey">The database row key.</param>
			/// <param name="entityId">The entity id.</param>
			/// <param name="baseEntityId">The base entity id.</param>
			/// <returns>The entity or <c>null</c> if the entity is not known.</returns>
			public AbstractEntity FindEntity(DbKey rowKey, Druid entityId, Druid baseEntityId)
			{
				EntityDataMapping search = new EntityDataMapping (rowKey, entityId, baseEntityId);
				EntityDataMapping item;

				if (this.lookup.TryGetValue (search, out item))
				{
					return item.Entity;
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// Gets the entities stored in the cache.
			/// </summary>
			/// <returns>The collection of entities.</returns>
			public IEnumerable<AbstractEntity> GetEntities()
			{
				//	We cannot use an iterator here, since the list might grow while we
				//	are enumerating it ...

				this.isIteratingList++;

				for (int i = 0; i < this.mappings.Count; i++)
				{
					AbstractEntity entity = this.mappings[i].Entity;
					yield return entity;
				}

				this.isIteratingList--;
			}

			/// <summary>
			/// Gets the entities stored in the cache.
			/// </summary>
			/// <param name="predicate">The filtering predicate.</param>
			/// <returns>The collection of entities.</returns>
			public IEnumerable<AbstractEntity> GetEntities(System.Predicate<AbstractEntity> predicate)
			{
				//	We cannot use an iterator here, since the list might grow while we
				//	are enumerating it ...

				this.isIteratingList++;

				for (int i = 0; i < this.mappings.Count; i++)
				{
					AbstractEntity entity = this.mappings[i].Entity;

					if (predicate (entity))
					{
						yield return entity;
					}
				}

				this.isIteratingList--;
			}

			/// <summary>
			/// Determines whether the cache contains the specified entity.
			/// </summary>
			/// <param name="entity">The entity.</param>
			/// <returns>
			/// 	<c>true</c> if the cache contains the specified entity; otherwise, <c>false</c>.
			/// </returns>
			public bool ContainsEntity(AbstractEntity entity)
			{
				return this.entities.Contains (entity);
			}

			readonly Dictionary<long, EntityDataMapping> idToEntityMapping;
			readonly Dictionary<EntityDataMapping, EntityDataMapping> lookup;			//	we cannot use a HashSet because we need to be able to quickly retrieve an item from the dictionary based on a partial key
			readonly List<EntityDataMapping> mappings;
			readonly HashSet<AbstractEntity> entities;
			int isIteratingList;
		}
	}
}
