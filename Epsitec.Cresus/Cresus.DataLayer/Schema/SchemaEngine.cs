//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


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

			this.tableDefinitionCache = new Dictionary<string, DbTable> ();
			this.sourceReferencesCache = new Dictionary<Druid, IList<EntityFieldPath>> ();
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
		/// Clears the data cached in this instance.
		/// </summary>
		public void Clear()
		{
			this.tableDefinitionCache.Clear ();
			this.sourceReferencesCache.Clear ();
		}


		/// <summary>
		/// Loads the schema of an <see cref="AbstractEntity"/> from the database in memory.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose schema to load.</param>
		public void LoadSchema(Druid entityId)
		{
			Druid localEntityId = entityId;

			while (localEntityId.IsValid && !this.IsTableDefinitionInCache (this.GetEntityTableName (localEntityId)))
			{
				DbTable tableDefinition = this.GetEntityTableDefinition (localEntityId);

				this.LoadRelations (tableDefinition);

				ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
				StructuredType entityType = TypeRosetta.CreateTypeObject (manager, localEntityId) as StructuredType;

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

						this.AddTableDefinitionToCache (relationTableName, relationTableDefinition);
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
			string tableName = this.GetEntityTableName (entityId);

			if (!this.IsTableDefinitionInCache (tableName))
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, tableName);

					transaction.Commit ();

					this.AddTableDefinitionToCache (tableName, tableDefinition);
				}
			}

			return this.GetTableDefinitionFromCache (tableName);
		}


		/// <summary>
		/// Gets the <see cref="DbTable"/> describing the schema of a relation field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetRelationTableDefinition(Druid localEntityId, Druid fieldId)
		{
			string relationTableName = this.GetRelationTableName (localEntityId, fieldId);

			if (!this.IsTableDefinitionInCache (relationTableName))
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDefinition = this.DbInfrastructure.ResolveDbTable (transaction, relationTableName);

					transaction.Commit ();

					this.AddTableDefinitionToCache (relationTableName, tableDefinition);
				}
			}

			return this.GetTableDefinitionFromCache (relationTableName);
		}


		/// <summary>
		/// Gets the <see cref="DbColumn"/> describing the schema of a value field of an
		/// <see cref="AbstractEntity"/>
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbColumn"/>.</returns>
		public DbColumn GetEntityFieldColumnDefinition(Druid localEntityId, Druid fieldId)
		{
			string columnName = this.GetEntityColumnName (fieldId);

			DbTable dbTable = this.GetEntityTableDefinition (localEntityId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return dbColumn;
		}


		/// <summary>
		/// Adds a <see cref="DbTable"/> to the cache.
		/// </summary>
		/// <param name="tableName">The name that identifies the <see cref="DbTable"/>.</param>
		/// <param name="table">The <see cref="DbTable"/> to add.</param>
		private void AddTableDefinitionToCache(string tableName, DbTable table)
		{
			this.tableDefinitionCache[tableName] = table;
		}


		/// <summary>
		/// Gets a <see cref="DbTable"/> out of the cache.
		/// </summary>
		/// <param name="tableName">The name that identifies the <see cref="DbTable"/>.</param>
		/// <returns>The requested <see cref="DbTable"/>.</returns>
		private DbTable GetTableDefinitionFromCache(string tableName)
		{
			return this.tableDefinitionCache[tableName];
		}


		/// <summary>
		/// Checks if a <see cref="DbTable"/> is in the cache.
		/// </summary>
		/// <param name="tableName">The name that identifies the <see cref="DbTable"/>.</param>
		/// <returns><c>true</c> if the <see cref="DbTable"/> is in the cache, <c>false</c> if it is not.</returns>
		private bool IsTableDefinitionInCache(string tableName)
		{
			return this.tableDefinitionCache.ContainsKey (tableName)
				&& this.GetTableDefinitionFromCache (tableName) != null;
		}


		/// <summary>
		/// Gets the description of which fields of which entities references the entities defined
		/// by the given <see cref="Druid"/>. The result is return as a sequence of
		/// <see cref="EntityFieldPath"/> that contains the <see cref="Druid"/> of an entity and the
		/// <see cref="Druid"/> of a field.
		/// </summary>
		/// <remarks>
		/// This method retrieves only the source references for the given <see cref="Druid"/>, but
		/// do not retrieves the ones of base on derived entities.
		/// </remarks>
		/// <param name="targetEntity">The kind of entities of the target.</param>
		/// <returns>The description of the field and entities that references the given target entity.</returns>
		public IEnumerable<EntityFieldPath> GetSourceReferences(Druid targetEntity)
		{
			if (!this.sourceReferencesCache.ContainsKey (targetEntity))
			{
				this.sourceReferencesCache[targetEntity] = this.RetrieveSourceReferences (targetEntity).ToList ();
			}

			return this.sourceReferencesCache[targetEntity];
		}


		/// <summary>
		/// Builds the source references for the given <see cref="Druid"/>.
		/// </summary>
		/// <returns>The source referenced resolver.</returns>
		private IEnumerable<EntityFieldPath> RetrieveSourceReferences(Druid targetEntity)
		{
			string tableName = this.GetEntityTableName (targetEntity);

			foreach (var item in this.DbInfrastructure.GetSourceReferences (tableName))
			{
				Druid sourceId = Druid.Parse ("[" + item.Item1 + "]");
				Druid sourceFieldId = Druid.Parse ("[" + item.Item2 + "]");

				EntityFieldPath fieldPath = EntityFieldPath.CreateRelativePath (sourceFieldId.ToResourceId ());
				EntityFieldPath sourcePath = EntityFieldPath.CreateAbsolutePath (sourceId, fieldPath);

				yield return sourcePath;
			}
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTable"/> corresponding to an <see cref="AbstractEntity"/>
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose <see cref="DbTable"/> name to get.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		private string GetEntityTableName(Druid entityId)
		{
			return DbTable.GetEntityTableName(entityId);
		}


		/// <summary>
		/// Gets the name of the <see cref="DbColumn"/> corresponding to the <see cref="Druid"/>
		/// of a field.
		/// </summary>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbColumn"/>.</returns>
		private string GetEntityColumnName(Druid fieldId)
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
		private string GetRelationTableName(Druid localEntityId, Druid fieldId)
		{
			string sourceTableName = this.GetEntityTableName (localEntityId);
			string sourceColumnName = this.GetEntityColumnName (fieldId);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}


		/// <summary>
		/// The cache containing the <see cref="DbTable"/> that have been processed by the current
		/// instance.
		/// </summary>
		private readonly IDictionary<string, DbTable> tableDefinitionCache;

		
		/// <summary>
		/// The cache containing all the source references, that is, the information about which
		/// field of which entity does reference a given entity <see cref="Druid"/>.
		/// </summary>
		private readonly IDictionary<Druid, IList<EntityFieldPath>> sourceReferencesCache;


	}


}
