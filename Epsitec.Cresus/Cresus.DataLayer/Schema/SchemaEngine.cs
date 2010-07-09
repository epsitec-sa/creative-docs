//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Schema
{
	
	
	internal sealed class SchemaEngine
	{


		static SchemaEngine()
		{
			SchemaEngine.instances = new Dictionary<DbInfrastructure, SchemaEngine> ();
		}

		
		private SchemaEngine(DbInfrastructure dbInfrastructure)
		{
			this.DbInfrastructure = dbInfrastructure;

			this.tableDefinitionCache = new Dictionary<Druid, DbTable> ();
			this.typeDefinitionCache = new Dictionary<Druid, DbTypeDef> ();
		}


		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		public bool CreateSchema<TEntity>() where TEntity : AbstractEntity, new ()
		{
			Druid entityId = new TEntity ().GetEntityStructuredTypeId ();

			bool createTable = (this.GetTableDefinition (entityId) == null);

			if (createTable)
			{
				SchemaBuilder tableBuilder = new SchemaBuilder(this);
				
				tableBuilder.Add(entityId);

				foreach (var item in tableBuilder.GetNewTableDefinitions ())
				{
					this.AddTableDefinitionToCache (item.Key, item.Value);
				}

				foreach (var item in tableBuilder.GetNewTypeDefinitions ())
				{
					this.AddTypeDefinitionToCache (item.Key, item.Value);
				}

			}

			return createTable;
		}


		public void LoadSchema(Druid entityId)
		{
			Druid localEntityId = entityId;

			while(localEntityId.IsValid &&  !this.IsTableDefinitionInCache (entityId))
			{
				DbTable tableDefinition = this.GetTableDefinition (entityId);

				this.LoadRelations (tableDefinition);

				ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
				StructuredType entityType = TypeRosetta.CreateTypeObject (manager, entityId) as StructuredType;

				localEntityId = entityType.BaseTypeId;
			}
		}


		private void LoadRelations(DbTable tableDefinition)
		{
			using (DbTransaction transaction = DbInfrastructure.BeginTransaction())
			{
				foreach (DbColumn columnDefinition in tableDefinition.Columns)
				{
					if (columnDefinition.Cardinality != DbCardinality.None)
					{
						string relationTableName = tableDefinition.GetRelationTableName (columnDefinition);
						DbTable relationTableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, relationTableName);

						this.AddTableDefinitionToCache (columnDefinition.CaptionId, relationTableDefinition);
					}
				}

				transaction.Commit ();
			}
		}

		
		public DbTable GetTableDefinition(Druid entityId)
		{
			if (!this.IsTableDefinitionInCache (entityId))
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, entityId);

					transaction.Commit ();

					this.AddTableDefinitionToCache (entityId, tableDefinition);
				}
			}

			return this.GetTableDefinitionFromCache (entityId);
		}


		
		public DbTypeDef GetTypeDefinition(INamedType type)
		{
			Druid typeId = type.CaptionId;

			if (!this.IsTypeDefinitionInCache (typeId))
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTypeDef typeDefinition = this.DbInfrastructure.ResolveDbType (transaction, type);
					
					transaction.Commit ();

					this.typeDefinitionCache[type.CaptionId] = typeDefinition;
				}
			}

			return this.GetTypeDefinitionFromCache(typeId);
		}


		private void AddTableDefinitionToCache(Druid entityId, DbTable table)
		{
			this.tableDefinitionCache[entityId] = table;
		}


		private DbTable GetTableDefinitionFromCache(Druid entityId)
		{
			return this.tableDefinitionCache[entityId];
		}


		private bool IsTableDefinitionInCache(Druid entityId)
		{
			return this.tableDefinitionCache.ContainsKey (entityId);
		}


		private void AddTypeDefinitionToCache(Druid typeId, DbTypeDef type)
		{
			this.typeDefinitionCache[typeId] = type;
		}


		private DbTypeDef GetTypeDefinitionFromCache(Druid typeId)
		{
			return this.typeDefinitionCache[typeId];
		}


		private bool IsTypeDefinitionInCache(Druid typeId)
		{
			return this.typeDefinitionCache.ContainsKey (typeId);
		}


		public string GetDataTableName(Druid entityId)
		{
			return this.GetTableDefinition (entityId).Name;
		}


		public string GetDataColumnName(string fieldId)
		{
			System.Diagnostics.Debug.Assert (fieldId.StartsWith ("["));
			System.Diagnostics.Debug.Assert (fieldId.EndsWith ("]"));

			return fieldId.Substring (1, fieldId.Length - 2);
		}
		
		
		private static void SetSchemaEngine(SchemaEngine schemaEngine, DbInfrastructure dbInfrastructure)
		{
			if (schemaEngine == null)
			{
				SchemaEngine.instances.Remove (dbInfrastructure);
			}
			else
			{
				SchemaEngine.instances[dbInfrastructure] = schemaEngine;
			}
		}


		public static SchemaEngine GetSchemaEngine(DbInfrastructure dbInfrastructure)
		{
			if (!SchemaEngine.instances.ContainsKey (dbInfrastructure))
			{
				SchemaEngine schemaEngine = new SchemaEngine (dbInfrastructure);
				SchemaEngine.SetSchemaEngine (schemaEngine, dbInfrastructure);
			}
			
			return SchemaEngine.instances[dbInfrastructure];
		}


		private readonly Dictionary<Druid, DbTable> tableDefinitionCache;
		private readonly Dictionary<Druid, DbTypeDef> typeDefinitionCache;


		private static Dictionary<DbInfrastructure, SchemaEngine> instances;


	}


}
