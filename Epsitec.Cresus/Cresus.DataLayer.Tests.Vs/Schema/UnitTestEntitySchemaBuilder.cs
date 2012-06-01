using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestEntitySchemaBuilder
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void GetEntityTableNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaBuilder.GetEntityTableName (Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityTableNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());

			foreach (var type in ete.GetEntityTypes ())
			{
				string expected = Druid.ToFullString (type.CaptionId.ToLong ());

				string actual = EntitySchemaBuilder.GetEntityTableName (type.CaptionId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void GetEntityFieldTableNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaBuilder.GetEntityFieldTableName (Druid.Empty, Druid.FromLong (1))
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaBuilder.GetEntityFieldTableName (Druid.FromLong (1), Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityFieldTableNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());

			var fields =
				from entityType in ete.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from field in ete.GetLocalCollectionFields (entityTypeId)
				select new
				{
					EntityTypeId = entityTypeId,
					FieldId = field.CaptionId
				};

			foreach (var field in fields)
			{
				string expected = Druid.ToFullString (field.EntityTypeId.ToLong ())
					+ ":"
					+ Druid.ToFullString (field.FieldId.ToLong ());

				string actual = EntitySchemaBuilder.GetEntityFieldTableName (field.EntityTypeId, field.FieldId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void GetEntityFieldColumnNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaBuilder.GetEntityFieldColumnName (Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityFieldColumnNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());

			var fieldIds =
				from entityType in ete.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from field in ete.GetLocalFields (entityTypeId)
				where field.Relation == FieldRelation.None || field.Relation == FieldRelation.Reference
				select field.CaptionId;

			foreach (var fieldId in fieldIds)
			{
				string expected = Druid.ToFullString (fieldId.ToLong ());
				string actual = EntitySchemaBuilder.GetEntityFieldColumnName (fieldId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void BuildTablesArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => EntitySchemaBuilder.BuildTables (null)
			);
		}


		[TestMethod]
		public void BuildTablesTest1()
		{
			var entityTypeIds = new List<Druid> ()
			{
				new Druid ("[J1A4]"),
				new Druid ("[J1A6]"),
				new Druid ("[J1A9]"),
				new Druid ("[J1AE]"),
				new Druid ("[J1AG]"),
				new Druid ("[J1AJ]"),
			};

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (entityTypeIds);

			var tables = EntitySchemaBuilder.BuildTables (entityTypeEngine).ToList ();

			this.CheckTables (entityTypeEngine, tables);
		}


		[TestMethod]
		public void BuildTablesTest2()
		{
			var entityTypeIds = new List<Druid> ()
			{
				new Druid ("[J1A4]"),
				new Druid ("[J1A6]"),
				new Druid ("[J1A9]"),
				new Druid ("[J1AE]"),
				new Druid ("[J1AG]"),
				new Druid ("[J1AJ]"),
				new Druid ("[J1AN]"),
				new Druid ("[J1AQ]"),
				new Druid ("[J1AT]"),
				new Druid ("[J1AV]"),
				new Druid ("[J1A11]"),
				new Druid ("[J1A41]"),
				new Druid ("[J1A61]"),
				new Druid ("[J1A81]"),
				new Druid ("[J1AA1]"),
				new Druid ("[J1AB1]"),
				new Druid ("[J1AE1]"),
				new Druid ("[J1AJ1]"),
				new Druid ("[J1AT1]"),
				new Druid ("[J1A02]"),
				new Druid ("[J1A42]"),
				new Druid ("[J1A72]"),
			};

			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (entityTypeIds);

			var tables = EntitySchemaBuilder.BuildTables (entityTypeEngine).ToList ();

			this.CheckTables (entityTypeEngine, tables);
		}


		private void CheckTables(EntityTypeEngine entityTypeEngine, List<DbTable> tables)
		{
			var tablesDictionnary = tables.ToDictionary (t => t.Name);

			foreach (var entityType in entityTypeEngine.GetEntityTypes ())
			{
				this.CheckTable (entityTypeEngine, tablesDictionnary, entityType);
			}

			int nbEntityTypes = entityTypeEngine.GetEntityTypes ().Count;
			int nbCollectionFields = entityTypeEngine.GetEntityTypes ()
				.SelectMany (t => entityTypeEngine.GetLocalCollectionFields (t.CaptionId))
				.Count ();

			Assert.AreEqual (nbEntityTypes + nbCollectionFields, tables.Count);
		}


		private void CheckTable(EntityTypeEngine entityTypeEngine, Dictionary<string, DbTable> tables, StructuredType entityType)
		{
			this.CheckMainTable (entityTypeEngine, tables, entityType);

			foreach (var field in entityTypeEngine.GetLocalCollectionFields (entityType.CaptionId))
			{
				this.CheckCollectionTable (entityTypeEngine, tables, entityType, field);
			}
		}


		private void CheckMainTable(EntityTypeEngine entityTypeEngine, Dictionary<string, DbTable> tables, StructuredType entityType)
		{
			string tableName = EntitySchemaBuilder.GetEntityTableName (entityType.CaptionId);

			DbTable table = tables[tableName];

			Assert.IsNotNull (table);

			this.CheckMainTableColumnId (entityType, table);
			this.CheckMainTableColumnType (entityType, table);
			this.CheckMainTableColumnLog (entityType, table);

			var localValueFields = entityTypeEngine.GetLocalValueFields (entityType.CaptionId);
			var localReferenceFields = entityTypeEngine.GetLocalReferenceFields (entityType.CaptionId);

			foreach (var field in localValueFields)
			{
				this.CheckMainTableValueColumn (field, table);
			}

			foreach (var field in localReferenceFields)
			{
				this.CheckMainTableReferenceColumn (field, table);
			}

			foreach (var field in localValueFields.Concat (localReferenceFields))
			{
				this.CheckMainTableFieldIndex (entityType, field, table);
			}

			int nbBasicColumns = entityType.BaseType == null ? 3 : 1;
			int nbValueColumns = localValueFields.Count;
			int nbReferenceColumns = localReferenceFields.Count;
			int nbColumns = nbBasicColumns + nbValueColumns + nbReferenceColumns;

			Assert.AreEqual (nbColumns, table.Columns.Count);

			int nbValueIndexAsc = localValueFields.Count (f => f.Options.HasFlag (FieldOptions.IndexAscending));
			int nbValueIndexDesc = localValueFields.Count (f => f.Options.HasFlag (FieldOptions.IndexDescending));
			int nbReferenceIndexAsc = localReferenceFields.Count (f => f.Options.HasFlag (FieldOptions.IndexAscending));
			int nbReferenceIndexDesc = localReferenceFields.Count (f => f.Options.HasFlag (FieldOptions.IndexDescending));
			int nbIndexes = nbValueIndexAsc + nbValueIndexDesc + nbReferenceIndexAsc + nbReferenceIndexDesc;

			Assert.AreEqual (nbIndexes, table.Indexes.Count);
		}


		private void CheckMainTableColumnId(StructuredType entityType, DbTable table)
		{
			DbTable fakeTable = new DbTable (table.CaptionId);

			DbColumn expectedColumnId = new DbColumn (EntitySchemaBuilder.EntityTableColumnIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.KeyId, DbElementCat.Internal);

			if (entityType.BaseType == null)
			{
				expectedColumnId.IsAutoIncremented = true;
				expectedColumnId.AutoIncrementStartValue = EntitySchemaBuilder.AutoIncrementStartValue;
			};

			fakeTable.Columns.Add (expectedColumnId);
			fakeTable.DefinePrimaryKey (expectedColumnId);
			fakeTable.UpdatePrimaryKeyInfo ();

			DbColumn actualColumnId = table.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnId, actualColumnId));
		}


		private void CheckMainTableColumnType(StructuredType entityType, DbTable table)
		{
			if (entityType.BaseType == null)
			{
				DbTable fakeTable = new DbTable (table.CaptionId);

				DbColumn expectedColumnType = new DbColumn (EntitySchemaBuilder.EntityTableColumnEntityTypeIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.Data, DbElementCat.Internal);

				fakeTable.Columns.Add (expectedColumnType);

				DbColumn actualColumnType = table.Columns[EntitySchemaBuilder.EntityTableColumnEntityTypeIdName];

				Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnType, actualColumnType));
			}
		}


		private void CheckMainTableColumnLog(StructuredType entityType, DbTable table)
		{
			if (entityType.BaseType == null)
			{
				DbTable fakeTable = new DbTable (table.CaptionId);

				DbColumn expectedColumnLog = new DbColumn (EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.RefInternal, DbElementCat.Internal);

				fakeTable.Columns.Add (expectedColumnLog);

				DbColumn actualColumnLog = table.Columns[EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName];

				Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnLog, actualColumnLog));
			}
		}


		private void CheckMainTableValueColumn(StructuredTypeField field, DbTable table)
		{
			DbTable fakeTable = new DbTable (table.CaptionId);

			DbColumn expectedColumnValue = new DbColumn (field.CaptionId, new DbTypeDef (field.Type), DbColumnClass.Data, DbElementCat.ManagedUserData, null);

			expectedColumnValue.Comment = expectedColumnValue.DisplayName;
			expectedColumnValue.IsNullable = field.IsNullable;

			fakeTable.Columns.Add (expectedColumnValue);

			string columnValueName = EntitySchemaBuilder.GetEntityFieldColumnName (field.CaptionId);
			DbColumn actualColumnValue = table.Columns[columnValueName];

			DbSchemaChecker.AreDbColumnEqual (expectedColumnValue, actualColumnValue);
		}


		private void CheckMainTableReferenceColumn(StructuredTypeField field, DbTable table)
		{
			DbTable fakeTable = new DbTable (table.CaptionId);

			DbColumn expectedColumnValue = new DbColumn (field.CaptionId, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.Data, DbElementCat.ManagedUserData, null);

			expectedColumnValue.Comment = expectedColumnValue.DisplayName;
			expectedColumnValue.IsNullable = true;

			fakeTable.Columns.Add (expectedColumnValue);

			string columnValueName = EntitySchemaBuilder.GetEntityFieldColumnName (field.CaptionId);
			DbColumn actualColumnValue = table.Columns[columnValueName];

			DbSchemaChecker.AreDbColumnEqual (expectedColumnValue, actualColumnValue);
		}


		private void CheckMainTableFieldIndex(StructuredType entityType, StructuredTypeField field, DbTable table)
		{
			List<DbIndex> indexes = new List<DbIndex> ();

			string columnName = EntitySchemaBuilder.GetEntityFieldColumnName (field.CaptionId);
			DbColumn column = table.Columns[columnName];

			string typeName = Druid.ToFullString (entityType.CaptionId.ToLong ());
			string fieldName = Druid.ToFullString (field.CaptionId.ToLong ());
			const string prefix = "IDX";
			const string separator = "_";

			if (field.Options.HasFlag (FieldOptions.IndexAscending))
			{
				string indexName = string.Join (separator, prefix, "ASC", typeName, fieldName);

				indexes.Add (new DbIndex (indexName, new DbColumn[] { column }, SqlSortOrder.Ascending));
			}

			if (field.Options.HasFlag (FieldOptions.IndexDescending))
			{
				string indexName = string.Join (separator, prefix, "DESC", typeName, fieldName);

				indexes.Add (new DbIndex (indexName, new DbColumn[] { column }, SqlSortOrder.Descending));
			}

			foreach (DbIndex index in indexes)
			{
				Assert.IsTrue (table.Indexes.Any (i => DbSchemaChecker.AreDbIndexEqual (index, i)));
			}
		}


		private void CheckCollectionTable(EntityTypeEngine entityTypeEngine, Dictionary<string, DbTable> tables, StructuredType entityType, StructuredTypeField field)
		{
			string tableName = EntitySchemaBuilder.GetEntityFieldTableName (entityType.CaptionId, field.CaptionId);

			DbTable table = tables[tableName];

			Assert.IsNotNull (table);

			Assert.AreEqual (4, table.Columns.Count);

			this.CheckCollectionTableColumnId (table);
			this.CheckCollectionTableColumnSourceId (table);
			this.CheckCollectionTableColumnTargetId (table);
			this.CheckCollectionTableColumnRank (table);
			this.CheckCollectionTableIndexes (entityType, field, table);
		}


		private void CheckCollectionTableColumnId(DbTable table)
		{
			DbTable fakeTable = new DbTable (table.Name);

			DbColumn expectedColumnId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true,
				AutoIncrementStartValue = EntitySchemaBuilder.AutoIncrementStartValue
			};

			fakeTable.Columns.Add (expectedColumnId);
			fakeTable.DefinePrimaryKey (expectedColumnId);
			fakeTable.UpdatePrimaryKeyInfo ();

			DbColumn actualColumnId = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnIdName];

			Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnId, actualColumnId));
		}


		private void CheckCollectionTableColumnSourceId(DbTable table)
		{
			DbTable fakeTable = new DbTable (table.Name);

			DbColumn expectedColumnSourceId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnSourceIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.RefInternal, DbElementCat.Internal);

			fakeTable.Columns.Add (expectedColumnSourceId);

			DbColumn actualColumnSourceId = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnSourceIdName];

			Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnSourceId, actualColumnSourceId));
		}


		private void CheckCollectionTableColumnTargetId(DbTable table)
		{
			DbTable fakeTable = new DbTable (table.Name);

			DbColumn expectedColumnTargetId = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnTargetIdName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId), DbColumnClass.RefInternal, DbElementCat.Internal);

			fakeTable.Columns.Add (expectedColumnTargetId);

			DbColumn actualColumnTargetId = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnTargetIdName];

			Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnTargetId, actualColumnTargetId));
		}


		private void CheckCollectionTableColumnRank(DbTable table)
		{
			DbTable fakeTable = new DbTable (table.Name);

			DbColumn expectedColumnRank = new DbColumn (EntitySchemaBuilder.EntityFieldTableColumnRankName, new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.CollectionRank), DbColumnClass.Data, DbElementCat.Internal);

			fakeTable.Columns.Add (expectedColumnRank);

			DbColumn actualColumnRank = table.Columns[EntitySchemaBuilder.EntityFieldTableColumnRankName];

			Assert.IsTrue (DbSchemaChecker.AreDbColumnEqual (expectedColumnRank, actualColumnRank));
		}


		private void CheckCollectionTableIndexes(StructuredType entityType, StructuredTypeField field, DbTable table)
		{
			List<DbIndex> indexes = new List<DbIndex> ();

			string typeName = Druid.ToFullString (entityType.CaptionId.ToLong ());
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

				indexes.Add (new DbIndex (indexSourceName, new DbColumn[] { sourceColumn }, SqlSortOrder.Ascending));
				indexes.Add (new DbIndex (indexTargetName, new DbColumn[] { targetColumn }, SqlSortOrder.Ascending));
			}

			if (field.Options.HasFlag (FieldOptions.IndexDescending))
			{
				string indexSourceName = string.Join (separator, prefix, "DESC", typeName, fieldName, sourceName);
				string indexTargetName = string.Join (separator, prefix, "DESC", typeName, fieldName, targetName);

				indexes.Add (new DbIndex (indexSourceName, new DbColumn[] { sourceColumn }, SqlSortOrder.Descending));
				indexes.Add (new DbIndex (indexTargetName, new DbColumn[] { targetColumn }, SqlSortOrder.Descending));
			}

			Assert.AreEqual (indexes.Count, table.Indexes.Count);

			foreach (DbIndex index in indexes)
			{
				Assert.IsTrue (table.Indexes.Any (i => DbSchemaChecker.AreDbIndexEqual (index, i)));
			}
		}


	}


}
