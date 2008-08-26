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
		class EntityDataCache
		{
			public EntityDataCache()
			{
				this.idToEntityMapping = new Dictionary<long, EntityDataMapping> ();
				this.lookup = new Dictionary<EntityDataMapping, EntityDataMapping> ();
				this.list = new List<EntityDataMapping> ();
			}

			public EntityDataMapping GetMapping(long entitySerialId)
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

			public void Add(long entitySerialId, EntityDataMapping mapping)
			{
				this.idToEntityMapping.Add (entitySerialId, mapping);
				this.list.Add (mapping);

				if (mapping.IsReadOnly)
				{
					this.lookup.Add (mapping, mapping);
				}
			}

			public void Remove(long entitySerialId)
			{
				EntityDataMapping mapping;

				if (this.idToEntityMapping.TryGetValue (entitySerialId, out mapping))
				{
					this.idToEntityMapping.Remove (entitySerialId);
					this.list.Remove (mapping);

					if (mapping.IsReadOnly)
					{
						this.lookup.Remove (mapping);
					}
				}
			}

			public void DefineRowKey(EntityDataMapping mapping, DbKey key)
			{
				System.Diagnostics.Debug.Assert (mapping.IsReadOnly == false);
				System.Diagnostics.Debug.Assert (this.list.Contains (mapping));

				mapping.RowKey = key;

				System.Diagnostics.Debug.Assert (mapping.IsReadOnly);

				this.lookup.Add (mapping, mapping);
			}

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public AbstractEntity FindEntity(DbKey rowKey, Druid entityId, Druid baseEntityId)
			{
				EntityDataMapping search = new EntityDataMapping (rowKey, entityId, baseEntityId);
				EntityDataMapping item;

				if (this.lookup.TryGetValue (search, out item))
				{
					return item.Entity;
				}

				item = this.list.Find (x => x.Equals (search));

				if (item == null)
				{
					return null;
				}

				if (item.IsReadOnly)
				{
					this.lookup.Add (item, item);
				}

				return item.Entity;
			}

			public IEnumerable<AbstractEntity> GetEntities()
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					AbstractEntity entity = this.list[i].Entity;
					yield return entity;
				}
			}

			public IEnumerable<AbstractEntity> GetEntities(System.Predicate<AbstractEntity> predicate)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					AbstractEntity entity = this.list[i].Entity;

					if (predicate (entity))
					{
						yield return entity;
					}
				}
			}

			readonly Dictionary<long, EntityDataMapping> idToEntityMapping;
			readonly Dictionary<EntityDataMapping, EntityDataMapping> lookup;
			readonly List<EntityDataMapping> list;
		}
	}
}
