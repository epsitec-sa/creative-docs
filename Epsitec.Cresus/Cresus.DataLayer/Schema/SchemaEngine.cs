//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Schema
{
	

	/// <summary>
	/// The <c>SchemaEngine</c> class provides functions used to manipulate the schema of the
	/// <see cref="AbstractEntity"/> in the database.
	/// </summary>
	internal sealed class SchemaEngine
	{

		
		/// <summary>
		/// Builds a new <see cref="SchemaEngine"/> which will be associated with a given
		/// <see cref="DbInfrastructure"/>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> that will be used by the new instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is null.</exception>
		public SchemaEngine(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			this.DbInfrastructure = dbInfrastructure;
			this.SchemaBuilder = new SchemaBuilder (dbInfrastructure);

			this.tableDefinitionCache = new Dictionary<Druid, DbTable> ();
		}


		/// <summary>
		/// Gets the <see cref="SchemaBuilder"/> used by this instance to build the schemas of the
		/// <see cref="AbstractEntity"/>
		/// </summary>
		private SchemaBuilder SchemaBuilder
		{
			get;
			set;
		}
		
		
		/// <summary>
		/// Gets the <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates the schema of an <see cref="AbstractEntity"/> and all its references and relations
		/// in the database.
		/// </summary>
		/// <typeparam name="TEntity">The type whose schema to create.</typeparam>
		/// <returns><c>true</c> if the schema was created, <c>false</c> if it already existed.</returns>
		public bool CreateSchema<TEntity>() where TEntity : AbstractEntity, new ()
		{
			Druid entityId = new TEntity ().GetEntityStructuredTypeId ();

			bool createTable = (this.GetEntityTableDefinition (entityId) == null);

			if (createTable)
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.SchemaBuilder.RegisterSchema (entityId);

					transaction.Commit ();
				}

				this.LoadSchema (entityId);
			}

			return createTable;
		}


		/// <summary>
		/// Loads the schema of an <see cref="AbstractEntity"/> from the database in memory.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose schema to load.</param>
		public void LoadSchema(Druid entityId)
		{
			Druid localEntityId = entityId;

			while(localEntityId.IsValid && !this.IsTableDefinitionInCache (entityId))
			{
				DbTable tableDefinition = this.GetEntityTableDefinition (entityId);

				this.LoadRelations (tableDefinition);

				ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
				StructuredType entityType = TypeRosetta.CreateTypeObject (manager, entityId) as StructuredType;

				localEntityId = entityType.BaseTypeId;
			}
		}


		/// <summary>
		/// Loads all the relation of a <see cref="DbTable"/> from the database in memory.
		/// </summary>
		/// <param name="tableDefinition">The <see cref="DbTable"/> whose relations to load.</param>
		private void LoadRelations(DbTable tableDefinition)
		{
			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				foreach (DbColumn columnDefinition in tableDefinition.Columns)
				{
					DbCardinality cardinality = columnDefinition.Cardinality;

					if (cardinality == DbCardinality.Reference || cardinality == DbCardinality.Collection)
					{
						string relationTableName = tableDefinition.GetRelationTableName (columnDefinition);
						DbTable relationTableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, relationTableName);

						this.AddTableDefinitionToCache (columnDefinition.CaptionId, relationTableDefinition);
					}
				}

				transaction.Commit ();
			}
		}

		
		/// <summary>
		/// Gets the <see cref="DbTable"/> describing the schema of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> whose schema to load.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetEntityTableDefinition(Druid entityId)
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


		/// <summary>
		/// Gets the <see cref="DbTable"/> describing the schema of the field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetRelationTableDefinition(Druid localEntityId, Druid fieldId)
		{
			if (!this.IsTableDefinitionInCache (fieldId))
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					string relationTableName = this.GetRelationTableName (localEntityId, fieldId);
					DbTable tableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, relationTableName);

					transaction.Commit ();

					this.AddTableDefinitionToCache (fieldId, tableDefinition);
				}
			}

			return this.GetTableDefinitionFromCache (fieldId);
		}


		/// <summary>
		/// Adds a <see cref="DbTable"/> to the cache.
		/// </summary>
		/// <param name="id">The <see cref="Druid"/> that identifies the <see cref="DbTable"/>.</param>
		/// <param name="table">The <see cref="DbTable"/> to add.</param>
		private void AddTableDefinitionToCache(Druid id, DbTable table)
		{
			this.tableDefinitionCache[id] = table;
		}


		/// <summary>
		/// Gets a <see cref="DbTable"/> out of the cache.
		/// </summary>
		/// <param name="id">The <see cref="Druid"/> that identifies the <see cref="DbTable"/>.</param>
		/// <returns>The requested <see cref="DbTable"/>.</returns>
		private DbTable GetTableDefinitionFromCache(Druid id)
		{
			return this.tableDefinitionCache[id];
		}


		/// <summary>
		/// Checks if a <see cref="DbTable"/> is in the cache.
		/// </summary>
		/// <param name="id">The <see cref="Druid"/> that identifies the <see cref="DbTable"/>.</param>
		/// <returns><c>true</c> if the <see cref="DbTable"/> is in the cache, <c>false</c> if it is not.</returns>
		private bool IsTableDefinitionInCache(Druid id)
		{
			return this.tableDefinitionCache.ContainsKey (id)
				&& this.GetTableDefinitionFromCache (id) != null;
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTable"/> corresponding to an <see cref="AbstractEntity"/>
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose <see cref="DbTable"/> name to get.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		public string GetEntityTableName(Druid entityId)
		{
			return DbTable.GetEntityTableName(entityId);
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTableColumn"/> corresponding to the <see cref="Druid"/>
		/// of a field.
		/// </summary>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbTableColumn"/>.</returns>
		public string GetEntityColumnName(Druid fieldId)
		{
			return DbColumn.GetColumnName (fieldId);
		}


		/// <summary>
		/// Gets the name of the relation <see cref="DbTable"/> corresponding to the field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		public string GetRelationTableName(Druid localEntityId, Druid fieldId)
		{
			string sourceTableName = this.GetEntityTableName (localEntityId);
			string sourceColumnName = this.GetEntityColumnName (fieldId);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}


		/// <summary>
		/// The cache containing the <see cref="DbTable"/> that have been processed by the current
		/// instance.
		/// </summary>
		private readonly Dictionary<Druid, DbTable> tableDefinitionCache;


	}


}
