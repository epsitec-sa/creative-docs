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
			this.EntityContext = new EntityContext ();

			this.tableDefinitionCache = new Dictionary<string, DbTable> ();
			this.referencingFieldsCache = new Dictionary<Druid, IList<System.Tuple<StructuredType, StructuredTypeField>>> ();
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
		/// The <see cref="EntityContext"/> associated with this instance.
		/// </summary>
		private EntityContext EntityContext
		{
			get;
			set;
		}


		/// <summary>
		/// Clears the data cached in this instance.
		/// </summary>
		public void Clear()
		{
			this.tableDefinitionCache.Clear ();
			this.referencingFieldsCache.Clear ();
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
				// This call loads the table in the cache if it is not yet loaded.
				// Marc

				this.GetEntityTableDefinition (localEntityId);

				ResourceManager manager = this.DbInfrastructure.DefaultContext.ResourceManager;
				StructuredType entityType = TypeRosetta.CreateTypeObject (manager, localEntityId) as StructuredType;

				var collectionFields =
					from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
					where field.Relation == FieldRelation.Collection
					select field;

				foreach (StructuredTypeField field in collectionFields)
				{
					// This call loads the relation tables in the cache if it is not yet loaded.
					// Marc

					this.GetCollectionTableDefinition (entityType.CaptionId, field.CaptionId);
				}

				localEntityId = entityType.BaseTypeId;
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
		/// Gets the <see cref="DbTable"/> describing the schema of a collection field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetCollectionTableDefinition(Druid localEntityId, Druid fieldId)
		{
			string relationTableName = this.GetCollectionTableName (localEntityId, fieldId);

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
		/// Gets the <see cref="DbColumn"/> describing the schema of a value or reference field of
		/// an <see cref="AbstractEntity"/>
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
		/// Gets all the table definition for the entities which are defined in the database.
		/// </summary>
		/// <returns>The sequence of <see cref="DbTable"/>.</returns>
		public IEnumerable<DbTable> GetEntityTableDefinitions()
		{
			foreach (StructuredType entityType in this.GetEntityStructuredTypes ())
			{
				yield return this.GetEntityTableDefinition (entityType.CaptionId);
			}
		}


		/// <summary>
		/// Gets all the tables definition for the collection of the entities which are defined in
		/// the database.
		/// </summary>
		/// <returns>The sequence of <see cref="DbTable"/>.</returns>
		public IEnumerable<DbTable> GetEntityCollectionTableDefinitions()
		{
			foreach (StructuredType entityType in this.GetEntityStructuredTypes ())
			{
				var collectionFields =
					from field in this.EntityContext.GetEntityLocalFieldDefinitions (entityType.CaptionId)
					where field.Relation == FieldRelation.Collection
					select field;

				foreach (StructuredTypeField field in collectionFields)
				{
					yield return this.GetCollectionTableDefinition (entityType.CaptionId, field.CaptionId);
				}
			}
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
		/// by the given <see cref="Druid"/>.
		/// </summary>
		/// <remarks>
		/// This method retrieves only the source references for the given <see cref="Druid"/>, but
		/// do not retrieves the ones of base or derived entities.
		/// </remarks>
		/// <param name="targetEntityTypeId">The type of entities of the target.</param>
		/// <returns>The description of the field and entities that references the given target entity.</returns>
		public IEnumerable<System.Tuple<StructuredType, StructuredTypeField>> GetReferencingFields(Druid targetEntityTypeId)
		{
			if (!this.referencingFieldsCache.ContainsKey (targetEntityTypeId))
			{
				this.BuildReferencingFieldsCache ();
			}

			if (!this.referencingFieldsCache.ContainsKey (targetEntityTypeId))
			{
				return new List<System.Tuple<StructuredType, StructuredTypeField>> ();
			}
			else
			{
				return this.referencingFieldsCache[targetEntityTypeId];
			}
		}

		/// <summary>
		/// Builds the referencing fields cache.
		/// </summary>
		private void BuildReferencingFieldsCache()
		{
			List<StructuredType> entityStructuredTypes = this.GetEntityStructuredTypes ().ToList ();
				
			foreach (var entityStructuredType in entityStructuredTypes)
			{
				Druid entityId = entityStructuredType.CaptionId;
				
				var referencingFields = 
					from structuredType in entityStructuredTypes
					from structuredField in structuredType.Fields.Values
					where structuredField.Membership != FieldMembership.Inherited
					where structuredField.Source == FieldSource.Value
					let relation = structuredField.Relation
					where relation == FieldRelation.Reference || relation == FieldRelation.Collection
					where structuredField.TypeId == entityId
					select System.Tuple.Create (structuredType, structuredField);

				this.referencingFieldsCache[entityId] = referencingFields.ToList ();
			}
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTable"/> corresponding to an <see cref="AbstractEntity"/>
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose <see cref="DbTable"/> name to get.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		internal string GetEntityTableName(Druid entityId)
		{
			return DbTable.GetEntityTableName (entityId);
		}


		/// <summary>
		/// Gets the name of the <see cref="DbColumn"/> corresponding to the <see cref="Druid"/>
		/// of a field.
		/// </summary>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbColumn"/>.</returns>
		internal string GetEntityColumnName(Druid fieldId)
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
		internal string GetCollectionTableName(Druid localEntityId, Druid fieldId)
		{
			string fieldName = Druid.ToFullString (fieldId.ToLong ());
			string localEntityName = Druid.ToFullString (localEntityId.ToLong ());

			return string.Concat (localEntityName, ":", fieldName);
		}
		

		/// <summary>
		/// Gets all the <see cref="StructuredType"/> of the entities that should be defined in the
		/// database.
		/// </summary>
		/// <returns>The sequence of <see cref="StructuredType"/>.</returns>
		private IEnumerable<StructuredType> GetEntityStructuredTypes()
		{
			// TODO Check that we get all the structured types that we want here. No more, no less.
			// Marc

			return from id in EntityClassFactory.GetAllEntityIds ()
				   let type = (StructuredType) EntityInfo.GetStructuredType (id)
				   where type.Class == StructuredTypeClass.Entity
				   where type.Flags.HasFlag (StructuredTypeFlags.GenerateSchema)
				   select type;
		}


		/// <summary>
		/// The cache containing the <see cref="DbTable"/> that have been processed by the current
		/// instance.
		/// </summary>
		private readonly IDictionary<string, DbTable> tableDefinitionCache;


		/// <summary>
		/// The cache containing all the referencing fields, that is, the information about which
		/// field of which entity does reference a given entity <see cref="Druid"/>.
		/// </summary>
		private readonly IDictionary<Druid, IList<System.Tuple<StructuredType, StructuredTypeField>>> referencingFieldsCache;


		/// <summary>
		/// The number that should be used for the auto incremented fields of the entities.
		/// </summary>
		public static readonly int AutoIncrementStartValue = 1000000000;


	}


}
