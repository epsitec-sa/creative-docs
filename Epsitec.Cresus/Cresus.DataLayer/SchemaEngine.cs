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
	/// <summary>
	/// The <c>SchemaEngine</c> class manages the mapping between entities and
	/// database table definitions.
	/// </summary>
	public class SchemaEngine
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SchemaEngine"/> class.
		/// </summary>
		/// <param name="infrastructure">The database engine.</param>
		public SchemaEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure  = infrastructure;
			this.context         = this.infrastructure.DefaultContext;
			this.resourceManager = this.context.ResourceManager;

			this.tableDefinitionCache = new Dictionary<Druid, DbTable> ();
			this.typeDefinitionCache  = new Dictionary<Druid, DbTypeDef> ();
		}


		/// <summary>
		/// Gets the associated database engine.
		/// </summary>
		/// <value>The <see cref="DbInfrastructure"/> instance.</value>
		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		/// <summary>
		/// Gets a table definition for the specified entity id, creating it
		/// if it was not yet known to the engine.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The root <see cref="DbTable"/> for the specified entity.</returns>
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

		/// <summary>
		/// Finds a table definition for the specified entity id.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The root <see cref="DbTable"/> for the specified entity or
		/// <c>null</c> if the entity is not yet known to the engine.</returns>
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
					transaction.Commit ();
				}

				if (table != null)
				{
					this.tableDefinitionCache[entityId] = table;
				}

				return table;
			}
		}

		/// <summary>
		/// Finds the type definition for the specified type object.
		/// </summary>
		/// <param name="type">The type object.</param>
		/// <returns>The <see cref="DbTypeDef"/> instance or <c>null</c> if
		/// the type is not yet known to the engine.</returns>
		public DbTypeDef FindTypeDefinition(INamedType type)
		{
			DbTypeDef typeDef;

			if (type == null)
			{
				return null;
			}
			else if (this.typeDefinitionCache.TryGetValue (type.CaptionId, out typeDef))
			{
				return typeDef;
			}
			else
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					typeDef = this.infrastructure.ResolveDbType (transaction, type);
					transaction.Commit ();
				}

				if (typeDef != null)
				{
					this.typeDefinitionCache[type.CaptionId] = typeDef;
				}

				return typeDef;
			}
		}

		#region Internal Support Methods

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

		#endregion

		DbInfrastructure infrastructure;
		DbContext context;
		ResourceManager resourceManager;

		Dictionary<Druid, DbTable> tableDefinitionCache;
		Dictionary<Druid, DbTypeDef> typeDefinitionCache;
	}
}
