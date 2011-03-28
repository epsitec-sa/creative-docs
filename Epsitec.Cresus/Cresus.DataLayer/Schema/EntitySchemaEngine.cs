//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Database;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	/// <summary>
	/// The <c>SchemaEngine</c> class provides functions used to manipulate the schema of the
	/// <see cref="AbstractEntity"/> in the database.
	/// </summary>
	internal sealed class EntitySchemaEngine
	{

		/*
		 * All the method of this class are thread safe, but the DbTable, DbColumn, etc objects that
		 * it returns are not. There is no formal guarantee whatsoever that they are thread safe.
		 * However, given how these objects are used within the DataLayer project (they are accessed
		 * only for read operations) and that they are not modified by the DataBase project
		 * (they are supposed to be accessed only for read operation and are not supposed to be
		 * modified and that this class calls the appropriate methods so that their internal state
		 * is supposed to be stable at the end of the constructor execution, they can be used in a
		 * thread safe way by the DataLayer project.
		 * However, I repeat, there are no formal guarantees on that. These objects are not
		 * synchronized and are mutable. This is some kind of "we know that it will work, so finger
		 * crossed" situation. And of course, if they are modified in any way, all those assumptions
		 * might turn out to be false and then we'll be screwed up.
		 * Marc
		 */

		// TODO Add more checks so that we are sure that everything is defined properly in the
		// database as it is defined in the EntityTypeEngine.
		// Marc

		// TODO Add methods to retrieve extra columns from the tables, such as the ones for the
		// row id, the relation source id, the relation target id and the relation rank.
 		// Marc

		/// <summary>
		/// Builds a new <see cref="EntitySchemaEngine"/> which will be associated with a given
		/// <see cref="DbInfrastructure"/>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> that will be used by the new instance.</param>
		/// <param name="entityTypeEngine">The <see cref="EntityTypeEngine"/> that will be used by the new instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is null.</exception>
		public EntitySchemaEngine(DbInfrastructure dbInfrastructure, EntityTypeEngine entityTypeEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityTypeEngine.ThrowIfNull ("entityTypeEngine");

			using (DbTransaction transaction = dbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadOnly))
			{
				this.entityTableCache = this.ComputeEntityTableCache (dbInfrastructure, entityTypeEngine);
				this.entityFieldTableCache = this.ComputeEntityFieldTableCache (dbInfrastructure, entityTypeEngine);

				transaction.Commit ();
			}

			this.entityFieldColumnCache = this.ComputeEntityFieldColumnCache (entityTypeEngine);

			this.EnsureReferencedObjectsAreDeserialized ();
		}


		private ReadOnlyDictionary<Druid, DbTable> ComputeEntityTableCache(DbInfrastructure dbInfrastructure, EntityTypeEngine entityTypeEngine)
		{
			return entityTypeEngine.GetEntityTypes ()
				.ToDictionary
				(
					t => t.CaptionId,
					t => this.ComputeEntityTable (dbInfrastructure, t.CaptionId)
				)
				.AsReadOnlyDictionary ();
		}


		private DbTable ComputeEntityTable(DbInfrastructure dbInfrastructure, Druid entityTypeId)
		{
			string tableName = EntitySchemaEngine.GetEntityTableName (entityTypeId);

			DbTable table = dbInfrastructure.ResolveDbTable (tableName);

			if (table == null)
			{
				throw new System.ArgumentException ("Table for type " + entityTypeId + " is not defined in the database.");
			}

			return table;
		}


		private ReadOnlyDictionary<Tuple<Druid, Druid>, DbTable> ComputeEntityFieldTableCache(DbInfrastructure dbInfrastructure, EntityTypeEngine entityTypeEngine)
		{
			var collectionFields =
				from entityType in entityTypeEngine.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from collectionField in entityTypeEngine.GetLocalCollectionFields (entityTypeId)
				let fieldId = collectionField.CaptionId
				select new
				{
					EntityTypeId = entityTypeId,
					FieldId = fieldId
				};

			return collectionFields
				.ToDictionary
				(
					f => EntitySchemaEngine.GetEntityFieldTableKey (f.EntityTypeId, f.FieldId),
					f => this.ComputeEntityFieldTable (dbInfrastructure, f.EntityTypeId, f.FieldId)
				)
				.AsReadOnlyDictionary ();
		}


		private DbTable ComputeEntityFieldTable(DbInfrastructure dbInfrastructure, Druid entityTypeId, Druid fieldId)
		{
			string tableName = EntitySchemaEngine.GetEntityFieldTableName (entityTypeId, fieldId);

			DbTable table = dbInfrastructure.ResolveDbTable (tableName);

			if (table == null)
			{
				throw new System.ArgumentException ("Table for field " + fieldId + " of type " + entityTypeId + " is not defined in the database");
			}

			return table;
		}


		private ReadOnlyDictionary<Tuple<Druid, string>, DbColumn> ComputeEntityFieldColumnCache(EntityTypeEngine entityTypeEngine)
		{
			var fields =
				from entityType in entityTypeEngine.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from field in entityTypeEngine.GetLocalFields (entityTypeId)
				where field.Relation == FieldRelation.None || field.Relation == FieldRelation.Reference
				let fieldId = field.CaptionId
				select new
				{
					EntityTypeId = entityTypeId,
					FieldId = fieldId
				};

			return fields
				.ToDictionary
				(
					f => EntitySchemaEngine.GetEntityFieldColumnKey (f.EntityTypeId, f.FieldId),
					f => this.ComputeEntityFieldColumn (f.EntityTypeId, f.FieldId)
				)
				.AsReadOnlyDictionary ();
		}


		private DbColumn ComputeEntityFieldColumn(Druid entityTypeId, Druid fieldId)
		{
			string columnName = EntitySchemaEngine.GetEntityFieldColumnName (fieldId);

			DbTable table = this.entityTableCache[entityTypeId];
			DbColumn column = table.Columns[columnName];

			if (column == null)
			{
				throw new System.ArgumentException ("Column for field " + fieldId + " of type " + entityTypeId + " is not defined in the database");
			}

			return column;
		}


		private void EnsureReferencedObjectsAreDeserialized()
		{
			IEnumerable<DbTable> tables = this.GetEntityTables ().Concat (this.GetEntityFieldTables ());

			foreach (DbTable table in tables)
			{
				var tableCaption = table.Caption;
				var tablePrimaryKeys = table.PrimaryKeys;
				var tableIndexes = table.Indexes;
				
				foreach (DbColumn column in table.Columns)
				{
					var columnCaption = column.Caption;
					var columnType = column.Type;

					if (columnType != null)
					{
						var columnTypeCaption = columnType.Caption;
					}
				}
			}
		}


		/// <summary>
		/// Gets the <see cref="DbTable"/> describing the schema of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> whose schema to load.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetEntityTable(Druid entityId)
		{
			var cache = this.entityTableCache;
			var key = entityId;

			return EntitySchemaEngine.GetFromCache (cache, key);
		}


		/// <summary>
		/// Gets the <see cref="DbTable"/> describing the schema of a collection field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbTable"/>.</returns>
		public DbTable GetEntityFieldTable(Druid entityTypeId, Druid fieldId)
		{
			var cache = this.entityFieldTableCache;
			var key = EntitySchemaEngine.GetEntityFieldTableKey (entityTypeId, fieldId);

			return EntitySchemaEngine.GetFromCache (cache, key);
		}


		/// <summary>
		/// Gets the <see cref="DbColumn"/> describing the schema of a value or reference field of
		/// an <see cref="AbstractEntity"/>
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The corresponding <see cref="DbColumn"/>.</returns>
		public DbColumn GetEntityFieldColumn(Druid entityTypeId, Druid fieldId)
		{
			var cache = this.entityFieldColumnCache;
			var key = EntitySchemaEngine.GetEntityFieldColumnKey (entityTypeId, fieldId);

			return EntitySchemaEngine.GetFromCache (cache, key);
		}


		/// <summary>
		/// Gets all the table definition for the entities which are defined in the database.
		/// </summary>
		/// <returns>The sequence of <see cref="DbTable"/>.</returns>
		public IEnumerable<DbTable> GetEntityTables()
		{
			return this.entityTableCache.Values;
		}


		/// <summary>
		/// Gets all the tables definition for the collection of the entities which are defined in
		/// the database.
		/// </summary>
		/// <returns>The sequence of <see cref="DbTable"/>.</returns>
		public IEnumerable<DbTable> GetEntityFieldTables()
		{
			return this.entityFieldTableCache.Values;
		}


		/// <summary>
		/// The number that should be used for the auto incremented fields of the entities.
		/// </summary>
		internal static int AutoIncrementStartValue
		{
			get
			{
				return 1000000000;
			}
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTable"/> corresponding to an <see cref="AbstractEntity"/>
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose <see cref="DbTable"/> name to get.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		internal static string GetEntityTableName(Druid entityId)
		{
			entityId.ThrowIf (id => !id.IsValid, "entityId is not valid");

			return DbTable.GetEntityTableName (entityId);
		}


		/// <summary>
		/// Gets the name of the relation <see cref="DbTable"/> corresponding to the field of an
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="localEntityId">The <see cref="Druid"/> of the <see cref="AbstractEntity"/>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		internal static string GetEntityFieldTableName(Druid localEntityId, Druid fieldId)
		{
			localEntityId.ThrowIf (id => !id.IsValid, "localEntityId is not valid");
			fieldId.ThrowIf (id => !id.IsValid, "fieldId is not valid");

			string fieldName = Druid.ToFullString (fieldId.ToLong ());
			string localEntityName = Druid.ToFullString (localEntityId.ToLong ());

			return string.Concat (localEntityName, ":", fieldName);
		}


		/// <summary>
		/// Gets the name of the <see cref="DbColumn"/> corresponding to the <see cref="Druid"/>
		/// of a field.
		/// </summary>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <returns>The name of the <see cref="DbColumn"/>.</returns>
		internal static string GetEntityFieldColumnName(Druid fieldId)
		{
			fieldId.ThrowIf (id => !id.IsValid, "fieldId is not valid");

			return DbColumn.GetColumnName (fieldId);
		}


		private static Tuple<Druid, Druid> GetEntityFieldTableKey(Druid entityTypeId, Druid fieldId)
		{
			return Tuple.Create (entityTypeId, fieldId);
		}


		private static Tuple<Druid, string> GetEntityFieldColumnKey(Druid entityTypeId, Druid fieldId)
		{
			return Tuple.Create (entityTypeId, fieldId.ToResourceId ());
		}


		private static TValue GetFromCache<TKey, TValue>(IDictionary<TKey, TValue> cache, TKey key)
		{
			TValue value;

			if (!cache.TryGetValue (key, out value))
			{
				throw new System.ArgumentException ("Element not found!");
			}

			return value;
		}


		private readonly ReadOnlyDictionary<Druid, DbTable> entityTableCache;


		private readonly ReadOnlyDictionary<Tuple<Druid, Druid>, DbTable> entityFieldTableCache;


		private readonly ReadOnlyDictionary<Tuple<Druid, string>, DbColumn> entityFieldColumnCache;


	}


}
