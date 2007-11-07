//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class SchemaEngine
	{
		public SchemaEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure  = infrastructure;
			this.context         = this.infrastructure.DefaultContext;
			this.resourceManager = this.context.ResourceManager;

			this.tableDefinitionCache = new Dictionary<Druid, DbTable> ();
			this.typeDefinitionCache  = new Dictionary<Druid, DbTypeDef> ();
		}


		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		public DbTable CreateTableDefinition(Druid entityId)
		{
			SchemaEngineTableBuilder builder = new SchemaEngineTableBuilder (this);

			using (builder.BeginTransaction ())
			{
				builder.Add (entityId);
				builder.CommitTransaction ();
			}

			builder.UpdateCache ();

			return builder.GetRootTable ();
		}

		public DbTable FindTableDefinition(Druid entityId)
		{
			DbTable table;

			if (this.tableDefinitionCache.TryGetValue (entityId, out table))
			{
				return table;
			}
			else
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					table = this.infrastructure.ResolveDbTable (transaction, entityId);
				}

				if (table != null)
				{
					this.tableDefinitionCache[entityId] = table;
				}

				return table;
			}
		}



		internal StructuredType GetEntityType(Druid entityId)
		{
			return TypeRosetta.CreateTypeObject (this.resourceManager, entityId) as StructuredType;
		}

		internal void AddTableDefinitionToCache(Druid entityId, DbTable table)
		{
			if (this.tableDefinitionCache.ContainsKey (entityId))
			{
				//	Nothing to do.
			}
			else
			{
				this.tableDefinitionCache[entityId] = table;
			}
		}

		internal void AddTypeDefinitionToCache(Druid typeId, DbTypeDef type)
		{
			if (this.typeDefinitionCache.ContainsKey (typeId))
			{
				//	Nothing to do.
			}
			else
			{
				this.typeDefinitionCache[typeId] = type;
			}
		}


		DbInfrastructure infrastructure;
		DbContext context;
		ResourceManager resourceManager;
		
		Dictionary<Druid, DbTable> tableDefinitionCache;
		Dictionary<Druid, DbTypeDef> typeDefinitionCache;
	}
}
