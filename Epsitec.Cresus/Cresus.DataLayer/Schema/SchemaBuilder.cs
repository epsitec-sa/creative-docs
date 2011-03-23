//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;
using System;


namespace Epsitec.Cresus.DataLayer.Schema
{


	/// <summary>
	/// The <c>SchemaBuilder</c> class is used internally to build <see cref="DbTable"/> and 
	/// <see cref="DbTypeDef"/> and then register them to the database.
	/// </summary>
	internal sealed class SchemaBuilder
	{


		/// <summary>
		/// Builds a new <c>SchemaBuilder.</c>
		/// </summary>
		public SchemaBuilder(SchemaEngine schemaEngine, EntityTypeEngine entityTypeEngine, DbInfrastructure dbInfrastructure)
		{
			schemaEngine.ThrowIfNull ("schemaEngine");
			entityTypeEngine.ThrowIfNull ("entityTypeEngine");
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			
			this.DbInfrastructure = dbInfrastructure;
			this.SchemaEngine = schemaEngine;
			this.EntityTypeEngine = entityTypeEngine;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get;
			set;
		}


		private SchemaEngine SchemaEngine
		{
			get;
			set;
		}


		private EntityTypeEngine EntityTypeEngine
		{
			get;
			set;
		}


		/// <summary>
		/// Builds and registers the schema of the <see cref="AbstractEntity"/> defined by the given
		/// sequence of <see cref="Druid"/> in the database. This method will also build and register
		/// all the required <see cref="AbstractEntity"/> that are not yet defined in the database.
		/// </summary>
		/// <param name="entityIds">The sequence of <see cref="Druid"/> defining the <see cref="AbstractEntity"/> whose schemas to register.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityIds"/> is <c>null</c>.</exception>
		public void RegisterSchema(IEnumerable<Druid> entityIds)
		{
			entityIds.ThrowIfNull ("entityIds");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				var schema = this.BuildSchema (entityIds, true);

				IList<DbTable> schemaDbTables = schema.Item1;
				IList<DbTypeDef> schemaDbTypesDefs = schema.Item2;

				this.RegisterDbTypeDefs (schemaDbTypesDefs);
				this.RegisterDbTables (schemaDbTables);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// This method will alter the schema that is in the database in order to make it match the
		/// schema of the <see cref="AbstractEntity"/> defined by the given sequence of
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityIds">The sequence of <see cref="Druid"/> defining the <see cref="AbstractEntity"/> whose schemas to update.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityIds"/> is <c>null</c>.</exception>
		public void UpdateSchema(IEnumerable<Druid> entityIds)
		{
			entityIds.ThrowIfNull ("entityIds");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				var schema = this.BuildSchema (entityIds, false);

				IList<DbTable> schemaDbTables = schema.Item1;

				DbSchemaUpdater.UpdateSchema (this.DbInfrastructure, schemaDbTables);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Checks that the schema of the <see cref="AbstractEntity"/> defined by the given sequence
		/// of <see cref="Druid"/> is correctly defined in the database. All the referenced
		/// <see cref="AbstractEntity"/> are also checked.
		/// </summary>
		/// <param name="entityIds">The sequence of <see cref="Druid"/> defining the <see cref="AbstractEntity"/> whose schemas to check.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityIds"/> is <c>null</c>.</exception>
		public bool CheckSchema(IEnumerable<Druid> entityIds)
		{
			entityIds.ThrowIfNull ("entityIds");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				IList<DbTable> schemaDbTables = this.BuildSchema (entityIds, false).Item1;

				bool ok = this.CheckSchema (schemaDbTables);

				transaction.Commit ();

				return ok;
			}
		}


		/// <summary>
		/// Builds the schema of the <see cref="AbstractEntity"/> defined by the given sequence of
		/// <see cref="Druid"/>. The new schema may or may not reference existing stuff in the
		/// database, depending on the value of <paramref name="useDatabase"/>.
		/// </summary>
		/// <param name="entityIds">The sequence of <see cref="Druid"/> defining the schema to build.</param>
		/// <param name="useDatabase">If set to <c>true</c>, the created schema will reference existing stuff in the database, if set to <c>false</c>, the created schema will create everything, event if it already exists in the database.</param>
		/// <returns>A tuple containing the sequence of created <see cref="DbTable"/> and the sequence of created <see cref="DbTypeDef"/>.</returns>
		private Tuple<IList<DbTable>, IList<DbTypeDef>> BuildSchema(IEnumerable<Druid> entityIds, bool useDatabase)
		{
			// We follow the graph of AbstractEntity starting by the ones defined by entityIds in
			// order to get all the StructuredTypes that are connected directly or indirectly to the
			// ones defined by entityIds, including the ones defined by entityIds.
			var structuredTypes = this.GetStructuredTypesUsedInSchema (entityIds);

			// We process these StructuredTypes to get on one side the list of StructuredTypes that
			// are not yet defined in the database, and on the other side the list of DbTables that
			// correspond to a StructuredType that is defined in the database.
			var splitedTables = this.SplitTables (structuredTypes, useDatabase);

			var unregisteredTables = splitedTables.Item1;
			var registeredDbTables = splitedTables.Item2;

			// We get all the INamedTypes that are used in the StructuredTypes that are not yet
			// defined in the database.
			var namedTypes = this.GetNamedTypesUsedInStructuredTypes (unregisteredTables);

			// We process them to get on one side the list of DbTypeDefs that correspond to the
			// INamedType that are not yet defined in the database, and on the other side, the list
			// of DbTypeDefs that correspond to an sINamedType that is defined in the database.
			var splitedDbTypeDefs = this.SplitAndBuildDbTypeDefs (namedTypes, useDatabase);

			var unregisteredDbTypeDefs = splitedDbTypeDefs.Item1;
			var registeredDbTypeDefs = splitedDbTypeDefs.Item2;

			var dbTypeDefs = unregisteredDbTypeDefs.Concat (registeredDbTypeDefs).ToList ();

			// We build the DbTable objects that correspond to the StructuredTypes that must be
			// created.
			var unexistingDbTables = this.BuildDbTables (unregisteredTables, registeredDbTables, dbTypeDefs);

			// We return the created schema, that is the newly created DbTables and the newly created
			// DbTypeDefs.
			return Tuple.Create (unexistingDbTables, unregisteredDbTypeDefs);
		}


		/// <summary>
		/// Gets the sequence of <see cref="StructuredType"/> that are correspond to the given
		/// sequence of entity types ids.
		/// </summary>
		/// <param name="typeIds">The sequence of <see cref="Druid"/> defining the types of the <see cref="AbstractEntity"/>.</param>
		/// <returns>The sequence of <see cref="Druid"/>.</returns>
		private IList<StructuredType> GetStructuredTypesUsedInSchema(IEnumerable<Druid> typeIds)
		{
			return typeIds.Select (id => this.EntityTypeEngine.GetEntityType (id)).ToList ();
		}


		/// <summary>
		/// Gets the sequence of <see cref="INamedType"/> used in the given sequence of
		/// <see cref="StructuredType"/>.
		/// </summary>
		/// <param name="structuredTypes">The sequence of <see cref="StructuredType"/> whose <see cref="INamedType"/> to retrieve.</param>
		/// <returns>The sequence of <see cref="INamedType"/> referenced in the sequence of <see cref="StructuredType"/>.</returns>
		private IList<INamedType> GetNamedTypesUsedInStructuredTypes(IList<StructuredType> structuredTypes)
		{
			var namedTypes = new Dictionary<Druid, INamedType> ();

			foreach (StructuredType structuredType in structuredTypes)
			{
				foreach (StructuredTypeField field in this.EntityTypeEngine.GetLocalValueFields (structuredType.CaptionId))
				{
					INamedType namedType = field.Type;
					Druid namedTypeId = namedType.CaptionId;

					if (namedTypes.ContainsKey (namedTypeId) == false)
					{
						namedTypes[namedTypeId] = namedType;
					}
				}
			}

			return namedTypes.Values.ToList ();
		}


		/// <summary>
		/// Splits the given sequence of <see cref="StructuredType"/> in a sequence which contains
		/// the ones that are not defined in the database, and in a sequence which contains the
		/// <see cref="DbTable"/> corresponding to the ones that are defined in the database.
		/// </summary>
		/// <param name="types">The sequence of <see cref="StructuredType"/> to split.</param>
		/// <param name="useDatabase">If set to <c>true</c>, this method will really look in the database, if set to <c>false</c>, every <see cref="StructuredType"/> will be considered as not defined in the database.</param>
		/// <returns>The pair of resulting sequences.</returns>
		private System.Tuple<IList<StructuredType>, IList<DbTable>> SplitTables(IEnumerable<StructuredType> types, bool useDatabase)
		{
			List<StructuredType> unregisteredTables = new List<StructuredType> ();
			List<DbTable> registeredTables = new List<DbTable> ();

			foreach (StructuredType type in types)
			{
				Druid typeId = type.CaptionId;

				DbTable registeredDbTable = null;

				if (useDatabase)
				{
					registeredDbTable = this.DbInfrastructure.ResolveDbTable (typeId);
				}

				if (registeredDbTable != null)
				{
					registeredTables.Add (registeredDbTable);
				}
				else
				{
					unregisteredTables.Add (type);
				}
			}

			return System.Tuple.Create ((IList<StructuredType>) unregisteredTables, (IList<DbTable>) registeredTables);
		}


		/// <summary>
		/// Splits the given sequence of <see cref="INamedType"/> in a sequence which contains the
		/// corresponding <see cref="DbTypeDef"/> of the ones that are not defined in the database
		/// and in a sequence which contains the corresponding <see cref="DbTypeDef"/> of the ones
		/// that are defined in the database.
		/// </summary>
		/// <param name="namedTypes">The sequence of <see cref="INamedType"/>.</param>
		/// <param name="useDatabase">If set to <c>true</c>, this method will really look in the database, if set to <c>false</c>, every <see cref="INamedType"/> will be considered as not defined in the database.</param>
		/// <returns>The pair of resulting sequences.</returns>
		private System.Tuple<IList<DbTypeDef>, IList<DbTypeDef>> SplitAndBuildDbTypeDefs(IEnumerable<INamedType> namedTypes, bool useDatabase)
		{
			List<DbTypeDef> unregisteredDbTypeDefs = new List<DbTypeDef> ();
			List<DbTypeDef> registeredDbTypeDefs = new List<DbTypeDef> ();

			foreach (INamedType namedType in namedTypes)
			{
				DbTypeDef registeredDbTypeDef = null;

				if (useDatabase)
				{
					registeredDbTypeDef = this.DbInfrastructure.ResolveDbType (namedType);
				}

				if (registeredDbTypeDef != null)
				{
					registeredDbTypeDefs.Add (registeredDbTypeDef);
				}
				else
				{
					DbTypeDef unregisteredDbTypeDef = new DbTypeDef (namedType);

					unregisteredDbTypeDefs.Add (unregisteredDbTypeDef);
				}
			}

			return System.Tuple.Create ((IList<DbTypeDef>) unregisteredDbTypeDefs, (IList<DbTypeDef>) registeredDbTypeDefs);
		}


		/// <summary>
		/// Builds the sequence of <see cref="DbTable"/> that corresponds to the given sequence of
		/// <see cref="StructuredType"/>. In order to build them, this method takes a sequence of
		/// the <see cref="DbTable"/> that already exists and the sequence of <see cref="DbTypeDef"/>
		/// that will be used to build the <see cref="DbTable"/>.
		/// </summary>
		/// <param name="unregisteredTables">The sequence of <see cref="StructuredType"/> whose corresponding <see cref="DbTable"/> to build.</param>
		/// <param name="registeredDbTables">The sequence of <see cref="DbTable"/> that already exist.</param>
		/// <param name="dbTypeDefs">The sequence of <see cref="DbTypeDef"/> that will be used to build the sequence of <see cref="DbTable"/>.</param>
		/// <returns>The sequence of newly created <see cref="DbTable"/>.</returns>
		private IList<DbTable> BuildDbTables(IEnumerable<StructuredType> unregisteredTables, IEnumerable<DbTable> registeredDbTables, IEnumerable<DbTypeDef> dbTypeDefs)
		{
			Dictionary<Druid, DbTable> dbTables = registeredDbTables.ToDictionary (t => t.CaptionId, t => t);
			Dictionary<Druid, DbTypeDef> dbTypeDefsDict = dbTypeDefs.ToDictionary (t => t.TypeId, t => t);
			Dictionary<Druid, StructuredType> unregisteredTablesDict = unregisteredTables.ToDictionary (t => t.CaptionId, t => t);

			List<DbTable> unregisteredEntityDbTables = new List<DbTable> ();
			List<DbTable> unregisteredCollectionDbTables = new List<DbTable> ();

			// We build the sequence of DbTables in two passes. In the first one, we create a basic
			// DbTable that contains only the metadata columns. Then we create the data columns in
			// the second pass. The reason for these two passes is that we must build all DbTables
			// before we can reference them, and there might be cycles in their graph.

			foreach (StructuredType unregisteredTable in unregisteredTablesDict.Values)
			{
				DbTable unregisteredDbTable = this.BuildBasicTable (unregisteredTable);

				dbTables[unregisteredTable.CaptionId] = unregisteredDbTable;
				unregisteredEntityDbTables.Add (unregisteredDbTable);
			}

			foreach (DbTable unregisteredDbTable in unregisteredEntityDbTables)
			{
				StructuredType type = unregisteredTablesDict[unregisteredDbTable.CaptionId];
				Druid typeId = type.CaptionId;

				foreach (var field in this.EntityTypeEngine.GetLocalValueFields (typeId))
				{
					DbColumn column = this.BuildValueColumn (dbTypeDefsDict, field);

					unregisteredDbTable.Columns.Add (column);

					this.AddIndexes (unregisteredDbTable, column, type, field);
				}

				foreach (var field in this.EntityTypeEngine.GetLocalReferenceFields (typeId))
				{
					DbColumn column = this.BuildReferenceColumn (field);

					unregisteredDbTable.Columns.Add (column);

					this.AddIndexes (unregisteredDbTable, column, type, field);
				}

				foreach (var field in this.EntityTypeEngine.GetLocalCollectionFields (typeId))
				{
					DbTable collectionTable = this.BuildCollectionTable (type, field);

					unregisteredCollectionDbTables.Add (collectionTable);
				}
			}

			return unregisteredEntityDbTables.Concat (unregisteredCollectionDbTables).ToList ();
		}


		/// <summary>
		/// Builds a basic <see cref="DbTable"/> for the given <see cref="StructuredType"/>. The
		/// created <see cref="DbTable"/> will only contains a name, comment and the metadata columns.
		/// </summary>
		/// <param name="tableType">The <see cref="StructuredType"/> corresponding to the <see cref="DbTable"/> to create.</param>
		/// <returns>The newly created <see cref="DbTable"/>.</returns>
		private DbTable BuildBasicTable(StructuredType tableType)
		{
			DbTypeDef keyTypeDef = this.DbInfrastructure.ResolveDbType (Tags.TypeKeyId);

			DbTable table = new DbTable (tableType.CaptionId);

			DbColumn columnId = new DbColumn (Tags.ColumnId, keyTypeDef, DbColumnClass.KeyId, DbElementCat.Internal);

			table.Columns.Add (columnId);
			table.PrimaryKeys.Add (columnId);
			table.UpdatePrimaryKeyInfo ();
			table.DefineCategory (DbElementCat.ManagedUserData);
			table.Comment = table.DisplayName;

			if (tableType.BaseType == null)
			{
				// If this entity has no parent in the class hierarchy, then we need to add a
				// special identification column, which can be used to map a row to its proper
				// derived entity class. We also add a column which in order to log info about
				// who made the last change to the entity. Finally, we say that the column is auto
				// incremented.
				// Marc

				columnId.IsAutoIncremented = true;
				columnId.AutoIncrementStartValue = SchemaEngine.AutoIncrementStartValue;

				DbColumn typeColumn = new DbColumn (Tags.ColumnInstanceType, keyTypeDef, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn logColumn = new DbColumn (Tags.ColumnRefLog, keyTypeDef, DbColumnClass.RefInternal, DbElementCat.Internal);

				table.Columns.Add (typeColumn);
				table.Columns.Add (logColumn);
			}
			else
			{
				columnId.IsAutoIncremented = false;
			}

			return table;
		}


		/// <summary>
		/// Builds the <see cref="DbColumn"/> that corresponds to the given value
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="dbTypeDefs">The mapping of <see cref="Druid"/>  to<see cref="DbTypeDef"/> that can be used for the types.</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbColumn"/> to create.</param>
		/// <returns>The newly created <see cref="DbColumn"/>.</returns>
		private DbColumn BuildValueColumn(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredTypeField field)
		{
			DbTypeDef columnType = dbTypeDefs[field.Type.CaptionId];
			DbColumn column = new DbColumn (field.CaptionId, columnType, DbColumnClass.Data, DbElementCat.ManagedUserData);

			column.Comment = column.DisplayName;
			column.IsNullable = field.IsNullable;

			return column;
		}


		/// <summary>
		/// Builds the <see cref="DbColumn"/> that corresponds to the given reference
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbColumn"/> to create.</param>
		/// <returns>The newly created <see cref="DbColumn"/>.</returns>
		private DbColumn BuildReferenceColumn(StructuredTypeField field)
		{
			DbTypeDef keyTypeDef = this.DbInfrastructure.ResolveDbType (Tags.TypeKeyId);
			DbColumn column = new DbColumn (field.CaptionId, keyTypeDef, DbColumnClass.Data, DbElementCat.ManagedUserData);

			column.Comment = column.DisplayName;
			column.IsNullable = true;

			return column;
		}


		/// <summary>
		/// Builds the <see cref="DbTable"/> that corresponds to the given collection
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="type">The <see cref="StructuredType"/> of the entity containing the field</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbTable"/> to create.</param>
		/// <returns>The newly created <see cref="DbTable"/>.</returns>
		private DbTable BuildCollectionTable(StructuredType type, StructuredTypeField field)
		{
			DbTypeDef refIdType = this.DbInfrastructure.ResolveDbType (Tags.TypeKeyId);
			DbTypeDef rankType = this.DbInfrastructure.ResolveDbType (Tags.TypeCollectionRank);

			string relationTableName = this.SchemaEngine.GetCollectionTableName (type.CaptionId, field.CaptionId);
			string entityName = type.Caption.Name;
			string fieldName = this.DbInfrastructure.DefaultContext.ResourceManager.GetCaption (field.CaptionId).Name;

			DbTable relationTable = new DbTable (relationTableName);

			DbColumn columnId = new DbColumn (Tags.ColumnId, refIdType, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true,
				AutoIncrementStartValue = SchemaEngine.AutoIncrementStartValue
			};
			DbColumn columnSourceId = new DbColumn (Tags.ColumnRefSourceId, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal);
			DbColumn columnTargetId = new DbColumn (Tags.ColumnRefTargetId, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal);
			DbColumn columnRank = new DbColumn (Tags.ColumnRefRank, rankType, DbColumnClass.Data, DbElementCat.Internal);

			relationTable.Columns.Add (columnId);
			relationTable.Columns.Add (columnSourceId);
			relationTable.Columns.Add (columnTargetId);
			relationTable.Columns.Add (columnRank);

			relationTable.PrimaryKeys.Add (columnId);
			relationTable.UpdatePrimaryKeyInfo ();

			relationTable.DefineCategory (DbElementCat.ManagedUserData);
			relationTable.Comment = entityName + "." + fieldName;

			this.AddCollectionIndexes (relationTable, type, field);

			return relationTable;
		}


		/// <summary>
		/// Adds the appropriate indexes on the given collection table.
		/// </summary>
		/// <param name="table">The table to index.</param>
		/// <param name="type">The type definition that defines the entity which corresponds to the table.</param>
		/// <param name="field">The field definition that corresponds to the table.</param>
		private void AddCollectionIndexes(DbTable table, StructuredType type, StructuredTypeField field)
		{
			string typeName = Druid.ToFullString (type.CaptionId.ToLong ());
			string fieldName = Druid.ToFullString (field.CaptionId.ToLong ());
			const string separator = "_";
			const string sourceName = "SRC";
			const string targetName = "TGT";
			const string prefix = "IDX";

			DbColumn sourceColumn = table.Columns[Tags.ColumnRefSourceId];
			DbColumn targetColumn = table.Columns[Tags.ColumnRefTargetId];

			if (field.Options.HasFlag (FieldOptions.IndexAscending))
			{
				string indexSourceName = string.Join (separator, prefix, "ASC", typeName, fieldName, sourceName);
				string indexTargetName = string.Join (separator, prefix, "ASC", typeName, fieldName, targetName);

				table.AddIndex (indexSourceName, SqlSortOrder.Ascending, sourceColumn);
				table.AddIndex (indexTargetName, SqlSortOrder.Ascending, targetColumn);
			}

			if (field.Options.HasFlag (FieldOptions.IndexDescending))
			{
				string indexSourceName = string.Join (separator, prefix, "DESC", typeName, fieldName, sourceName);
				string indexTargetName = string.Join (separator, prefix, "DESC", typeName, fieldName, targetName);

				table.AddIndex (indexSourceName, SqlSortOrder.Ascending, sourceColumn);
				table.AddIndex (indexTargetName, SqlSortOrder.Ascending, targetColumn);
			}
		}


		/// <summary>
		/// Adds the appropriate indexes to the given column in the given table, which correspond to
		/// the given type and field.
		/// </summary>
		/// <param name="table">The table to which to add the index.</param>
		/// <param name="column">The column to index.</param>
		/// <param name="type">The type definition that defines the entity which corresponds to the table.</param>
		/// <param name="field">The field definition that corresponds to the column.</param>
		private void AddIndexes(DbTable table, DbColumn column, StructuredType type, StructuredTypeField field)
		{
			string typeName = Druid.ToFullString (type.CaptionId.ToLong ());
			string fieldName = Druid.ToFullString (field.CaptionId.ToLong ());
			const string prefix = "IDX";
			const string separator = "_";

			if (field.Options.HasFlag (FieldOptions.IndexAscending))
			{
				string indexName = string.Join (separator, prefix, "ASC", typeName, fieldName);

				table.AddIndex (indexName, SqlSortOrder.Ascending, column);
			}

			if (field.Options.HasFlag (FieldOptions.IndexDescending))
			{
				string indexName = string.Join (separator, prefix, "DESC", typeName, fieldName);

				table.AddIndex (indexName, SqlSortOrder.Descending, column);
			}
		}
		

		/// <summary>
		/// Registers all the given <see cref="DbTypeDef"/> to the database.
		/// </summary>
		/// <param name="dbTypeDefs">The sequence of <see cref="DbTypeDef"/> to register.</param>
		private void RegisterDbTypeDefs(IList<DbTypeDef> dbTypeDefs)
		{
			foreach (DbTypeDef dbTypeDef in dbTypeDefs)
			{
				this.DbInfrastructure.AddType (dbTypeDef);
			}
		}


		/// <summary>
		/// Registers all the given <see cref="DbTable"/> to the database.
		/// </summary>
		/// <param name="dbTables">The sequence of <see cref="DbTable"/> to register in the database.</param>
		private void RegisterDbTables(IList<DbTable> dbTables)
		{
			this.DbInfrastructure.AddTables (dbTables);
		}


		/// <summary>
		/// Checks that all the given <see cref="DbTable"/> are correctly defined in the database.
		/// </summary>
		/// <param name="schema">The sequence of <see cref="DbTable"/> to check.</param>
		/// <returns><c>true</c> if all the <see cref="DbTable"/> ar </returns>
		private bool CheckSchema(IList<DbTable> schema)
		{
			return DbSchemaChecker.CheckSchema (this.DbInfrastructure, schema);
		}


	}


}
