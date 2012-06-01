//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	
	
	internal static class EntitySchemaBuilder
	{


		// TODO Comment this class
		// Marc


		public static IEnumerable<DbTable> BuildTables(EntityTypeEngine entityTypeEngine)
		{
			entityTypeEngine.ThrowIfNull ("entityTypeEngine");

			var entityTypes = EntitySchemaBuilder.GetEntityTypes (entityTypeEngine);

			var valueFieldTypes = EntitySchemaBuilder.GetValueFieldTypes (entityTypeEngine);
			var builtInTypes = EntitySchemaBuilder.GetBuiltInTypes ();

			var dbTypeDefs = EntitySchemaBuilder.BuildDbTypeDefs (valueFieldTypes.Concat (builtInTypes));
			var dbTables = EntitySchemaBuilder.BuildDbTables (entityTypeEngine, dbTypeDefs, entityTypes);

			return dbTables;
		}


		private static IEnumerable<StructuredType> GetEntityTypes(EntityTypeEngine entityTypeEngine)
		{
			return entityTypeEngine.GetEntityTypes ();
		}


		private static IEnumerable<INamedType> GetValueFieldTypes(EntityTypeEngine entityTypeEngine)
		{
			return entityTypeEngine.GetEntityTypes ()
				.SelectMany (t => entityTypeEngine.GetLocalValueFields (t.CaptionId))
				.Select (f => f.Type)
				.Distinct ();
		}


		private static IEnumerable<INamedType> GetBuiltInTypes()
		{
			yield return Epsitec.Cresus.Database.Res.Types.Num.KeyId;
			yield return Epsitec.Cresus.Database.Res.Types.Num.CollectionRank;
		}


		private static IEnumerable<DbTypeDef> BuildDbTypeDefs(IEnumerable<INamedType> namedTypes)
		{
			return namedTypes.Select(t => new DbTypeDef (t));
		}


		private static IEnumerable<DbTable> BuildDbTables(EntityTypeEngine entityTypeEngine, IEnumerable<DbTypeDef> dbTypeDefs, IEnumerable<StructuredType> entityTypes)
		{
			var dbTypeDefsDictionary = dbTypeDefs.ToDictionary (t => t.TypeId);

			return entityTypes.SelectMany (t => EntitySchemaBuilder.BuildDbTables (entityTypeEngine, dbTypeDefsDictionary, t));
		}


		private static IEnumerable<DbTable> BuildDbTables(EntityTypeEngine entityTypeEngine, Dictionary<Druid, DbTypeDef> dbTypeDefs, StructuredType entityType)
		{
			Druid entityTypeId = entityType.CaptionId;

			DbTable entityTable = EntitySchemaBuilder.BuildBasicTable (dbTypeDefs, entityType);

			foreach (var field in entityTypeEngine.GetLocalValueFields (entityTypeId))
			{
				DbColumn column = EntitySchemaBuilder.BuildValueColumn (dbTypeDefs, field);

				entityTable.Columns.Add (column);

				EntitySchemaBuilder.AddIndexes (entityTable, column, entityType, field);
			}

			foreach (var field in entityTypeEngine.GetLocalReferenceFields (entityTypeId))
			{
				DbColumn column = EntitySchemaBuilder.BuildReferenceColumn (dbTypeDefs, field);

				entityTable.Columns.Add (column);

				EntitySchemaBuilder.AddIndexes (entityTable, column, entityType, field);
			}

			yield return entityTable;

			foreach (var field in entityTypeEngine.GetLocalCollectionFields (entityTypeId))
			{
				DbTable collectionTable = EntitySchemaBuilder.BuildCollectionTable (dbTypeDefs, entityType, field);

				yield return collectionTable;
			}
		}


		/// <summary>
		/// Builds a basic <see cref="DbTable"/> for the given <see cref="StructuredType"/>. The
		/// created <see cref="DbTable"/> will only contains a name, comment and the metadata columns.
		/// </summary>
		/// <param name="dbTypeDefs">The mapping of <see cref="Druid"/>  to<see cref="DbTypeDef"/> that can be used for the types.</param>
		/// <param name="tableType">The <see cref="StructuredType"/> corresponding to the <see cref="DbTable"/> to create.</param>
		/// <returns>The newly created <see cref="DbTable"/>.</returns>
		private static DbTable BuildBasicTable(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredType tableType)
		{
			Druid keyTypeDefId = Epsitec.Cresus.Database.Res.Types.Num.KeyId.CaptionId;
			DbTypeDef keyTypeDef = dbTypeDefs[keyTypeDefId];

			DbTable table = new DbTable (tableType.CaptionId);

			DbColumn columnId = new DbColumn (EntitySchemaBuilder.EntityTableColumnIdName, keyTypeDef, DbColumnClass.KeyId, DbElementCat.Internal);

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
				columnId.AutoIncrementStartValue = EntitySchemaBuilder.AutoIncrementStartValue;

				DbColumn typeColumn = new DbColumn (EntitySchemaBuilder.EntityTableColumnEntityTypeIdName, keyTypeDef, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn logColumn = new DbColumn (EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName, keyTypeDef, DbColumnClass.RefInternal, DbElementCat.Internal);

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
		private static DbColumn BuildValueColumn(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredTypeField field)
		{
			DbTypeDef columnType = dbTypeDefs[field.Type.CaptionId];
			DbColumn column = new DbColumn (field.CaptionId, columnType, DbColumnClass.Data, DbElementCat.ManagedUserData, null);

			column.Comment = column.DisplayName;
			column.IsNullable = field.IsNullable;

			return column;
		}


		/// <summary>
		/// Builds the <see cref="DbColumn"/> that corresponds to the given reference
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="dbTypeDefs">The mapping of <see cref="Druid"/>  to<see cref="DbTypeDef"/> that can be used for the types.</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbColumn"/> to create.</param>
		/// <returns>The newly created <see cref="DbColumn"/>.</returns>
		private static DbColumn BuildReferenceColumn(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredTypeField field)
		{
			Druid keyTypeDefId = Epsitec.Cresus.Database.Res.Types.Num.KeyId.CaptionId;
			DbTypeDef keyTypeDef = dbTypeDefs[keyTypeDefId];

			DbColumn column = new DbColumn (field.CaptionId, keyTypeDef, DbColumnClass.Data, DbElementCat.ManagedUserData, null);

			column.Comment = column.DisplayName;
			column.IsNullable = true;

			return column;
		}


		/// <summary>
		/// Builds the <see cref="DbTable"/> that corresponds to the given collection
		/// <see cref="StructuredTypeField"/>.
		/// </summary>
		/// <param name="dbTypeDefs">The mapping of <see cref="Druid"/>  to<see cref="DbTypeDef"/> that can be used for the types.</param>
		/// <param name="type">The <see cref="StructuredType"/> of the entity containing the field</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> whose corresponding <see cref="DbTable"/> to create.</param>
		/// <returns>The newly created <see cref="DbTable"/>.</returns>
		private static DbTable BuildCollectionTable(IDictionary<Druid, DbTypeDef> dbTypeDefs, StructuredType type, StructuredTypeField field)
		{
			Druid keyTypeDefId = Epsitec.Cresus.Database.Res.Types.Num.KeyId.CaptionId;
			Druid rankTypeDefId = Epsitec.Cresus.Database.Res.Types.Num.CollectionRank.CaptionId;

			DbTypeDef refIdType = dbTypeDefs[keyTypeDefId];
			DbTypeDef rankType = dbTypeDefs[rankTypeDefId];

			string relationTableName = EntitySchemaBuilder.GetEntityFieldTableName (type.CaptionId, field.CaptionId);
			string entityName = type.Caption.Name;
			string fieldName = Epsitec.Common.Support.Resources.DefaultManager.GetCaption (field.CaptionId).Name;

			DbTable relationTable = new DbTable (relationTableName)
			{
				EnableUglyHackInOrderToRemoveSuffixFromTableName = true,
			};

			DbColumn columnId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnIdName, refIdType, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true,
				AutoIncrementStartValue = EntitySchemaBuilder.AutoIncrementStartValue
			};
			DbColumn columnSourceId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnSourceIdName, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal);
			DbColumn columnTargetId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnTargetIdName, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal);
			DbColumn columnRank = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnRankName, rankType, DbColumnClass.Data, DbElementCat.Internal);

			relationTable.Columns.Add (columnId);
			relationTable.Columns.Add (columnSourceId);
			relationTable.Columns.Add (columnTargetId);
			relationTable.Columns.Add (columnRank);

			relationTable.PrimaryKeys.Add (columnId);
			relationTable.UpdatePrimaryKeyInfo ();

			relationTable.DefineCategory (DbElementCat.ManagedUserData);
			relationTable.Comment = entityName + "." + fieldName;

			EntitySchemaBuilder.AddCollectionIndexes (relationTable, type, field);

			return relationTable;
		}


		/// <summary>
		/// Adds the appropriate indexes on the given collection table.
		/// </summary>
		/// <param name="table">The table to index.</param>
		/// <param name="type">The type definition that defines the entity which corresponds to the table.</param>
		/// <param name="field">The field definition that corresponds to the table.</param>
		private static void AddCollectionIndexes(DbTable table, StructuredType type, StructuredTypeField field)
		{
			string typeName = Druid.ToFullString (type.CaptionId.ToLong ());
			string fieldName = Druid.ToFullString (field.CaptionId.ToLong ());
			const string separator = "_";
			const string sourceName = "SRC";
			const string targetName = "TGT";
			const string prefix = "IDX";

			DbColumn sourceColumn = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnSourceIdName];
			DbColumn targetColumn = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnTargetIdName];

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

				table.AddIndex (indexSourceName, SqlSortOrder.Descending, sourceColumn);
				table.AddIndex (indexTargetName, SqlSortOrder.Descending, targetColumn);
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
		private static void AddIndexes(DbTable table, DbColumn column, StructuredType type, StructuredTypeField field)
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
		/// The number that should be used for the auto incremented fields of the entities.
		/// </summary>
		public static int AutoIncrementStartValue
		{
			get
			{
				return 1000000000;
			}
		}


		public static string EntityTableColumnIdName
		{
			get
			{
				return "CR_ID";
			}
		}


		public static string EntityTableColumnEntityTypeIdName
		{
			get
			{
				return "CR_TYPE_ID";
			}
		}


		public static string EntityTableColumnEntityModificationEntryIdName
		{
			get
			{
				return "CR_EM_ID";
			}
		}


		public static string EntityFieldTableColumnIdName
		{
			get
			{
				return "CR_ID";
			}
		}


		public static string EntityFieldTableColumnSourceIdName
		{
			get
			{
				return "CR_SOURCE_ID";
			}
		}


		public static string EntityFieldTableColumnTargetIdName
		{
			get
			{
				return "CR_TARGET_ID";
			}
		}


		public static string EntityFieldTableColumnRankName
		{
			get
			{
				return "CR_RANK";
			}
		}


		/// <summary>
		/// Gets the name of the <see cref="DbTable"/> corresponding to an <see cref="AbstractEntity"/>
		/// <see cref="Druid"/>.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> whose <see cref="DbTable"/> name to get.</param>
		/// <returns>The name of the <see cref="DbTable"/>.</returns>
		public static string GetEntityTableName(Druid entityId)
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
		public static string GetEntityFieldTableName(Druid localEntityId, Druid fieldId)
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
		public static string GetEntityFieldColumnName(Druid fieldId)
		{
			fieldId.ThrowIf (id => !id.IsValid, "fieldId is not valid");

			return DbColumn.GetColumnName (fieldId);
		}


	}


}
