//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Epsitec.Cresus.DataLayer
{
	// TODO Add sorting criteria
	// TODO Add comparison criteria
	// TODO Make sure that the references can be written in a consistent way after being loaded with the DataBrowser.
	public class DataBrowser
	{

		
		public DataBrowser(DbInfrastructure infrastructure, DataContext dataContext)
		{
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure) ?? new SchemaEngine (this.DbInfrastructure);
			this.DataContext = dataContext;
			this.tableAliasManager = new TableAliasManager ();
		}


		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		public SchemaEngine SchemaEngine
		{
			get;
			private set;
		}


		public DataContext DataContext
		{
			get;
			private set;
		}


		public IEnumerable<EntityType> QueryByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			Druid entityId = example.GetEntityContext ().CreateEmptyEntity<EntityType> ().GetEntityStructuredTypeId ();

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				using (DbReader reader = this.CreateReader (entityId, example))
				{
					reader.CreateDataReader (transaction);
					
					using (DataTable dataTable = this.BuildDataTable (entityId, reader))
					{
						foreach (DbReader.RowData row in reader.Rows)
						{
							DbKey rowKey = this.ExtractDbKey (row);
							Druid realEntityId = this.ExtractRealEntityId (row);
							DataRow dataRow = this.ExtractDataRow (dataTable, row);

							yield return this.DataContext.ResolveEntity (realEntityId, entityId, rowKey, dataRow) as EntityType;
						}
					}
				}
			}
		}


		private DataTable BuildDataTable(Druid entityId, DbReader reader)
		{
			DataTable dataTable = new DataTable ();

			foreach (Druid id in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				StructuredTypeField[] localFields = this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (id).ToArray ();

				StructuredTypeField[] localValueFields = localFields.Where (field => field.Relation == FieldRelation.None).ToArray ();
				StructuredTypeField[] localReferenceFields = localFields.Where (field => field.Relation == FieldRelation.Reference).ToArray ();

				foreach (StructuredTypeField field in localReferenceFields.Concat(localValueFields))
				{
					string name = this.DataContext.SchemaEngine.GetDataColumnName (field.Id);
					System.Type type = (field.Relation == FieldRelation.None) ?  reader.GetColumnType (name) : typeof (string);

					dataTable.Columns.Add (new DataColumn (name, type));
				}
			}

			return dataTable;
		}


		private DataRow ExtractDataRow(DataTable dataTable, DbReader.RowData row)
		{
			DataRow dataRow = dataTable.NewRow ();

			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				string name = dataTable.Columns[i].ColumnName;
				object value = row.Values[i];

				dataRow[name] = value;
			}

			return dataRow;
		}


		private Druid ExtractRealEntityId(DbReader.RowData row)
		{
			long realEntityIdAsLong = (long) row.Values[row.Values.Length - 2];

			return Druid.FromLong (realEntityIdAsLong);
		}


		private DbKey ExtractDbKey(DbReader.RowData row)
		{
			long dbKeyAsLong = (long) row.Values[row.Values.Length - 1];

			return new DbKey (new DbId (dbKeyAsLong));
		}


		private DbReader CreateReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			this.tableAliasManager.resetTableAliases ();

			this.tableAliasManager.PushRootTypeTableAlias ();
			this.AddQueryDataForEntity (reader, entityId, example, true);
			this.tableAliasManager.PopRootTypeTableAlias ();
			
			return reader;
		}


		private void AddQueryDataForEntity(DbReader reader, Druid entityId, AbstractEntity example, bool getFields)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();
			Druid[] heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToArray ();
			Druid rootEntityId =  heritedEntityIds[heritedEntityIds.Length - 1];

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				bool isRootType = currentEntityId == rootEntityId;
				this.tableAliasManager.PushSubTypeTableAlias (isRootType);

				StructuredTypeField[] localFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).ToArray ();
				
				StructuredTypeField[] localValueFields = localFields.Where (field => field.Relation == FieldRelation.None).ToArray ();
				StructuredTypeField[] localReferenceFields = localFields.Where (field => field.Relation == FieldRelation.Reference).ToArray ();
				StructuredTypeField[] localCollectionFields = localFields.Where (field => field.Relation == FieldRelation.Collection).ToArray ();

				StructuredTypeField[] localDefinedValueFields = localValueFields.Where (field => definedFieldIds.Contains (field.Id)).ToArray ();
				StructuredTypeField[] localDefinedReferenceFields = localReferenceFields.Where (field => definedFieldIds.Contains (field.Id)).ToArray ();
				StructuredTypeField[] localDefinedCollectionFields = localCollectionFields.Where (field => definedFieldIds.Contains (field.Id)).ToArray ();

				if (!isRootType && ((getFields && localValueFields.Length > 0) || localDefinedValueFields.Length > 0))
				{
					this.AddSubTypingJoin (reader, currentEntityId, heritedEntityIds.Last ());
				}

				foreach (StructuredTypeField field in localDefinedValueFields)
				{
					this.AddQueryDataForValue (reader, currentEntityId, example, field);
				}

				foreach (StructuredTypeField field in localReferenceFields)
				{
					if (localDefinedReferenceFields.Contains (field) && getFields)
					{
						if (getFields)
						{
							this.AddQueryDataForReferenceIdJoin (reader, currentEntityId, example, field);
						}
						else
						{
							this.AddQueryDataForReferenceJoin (reader, currentEntityId, example, field);
						}
					}
					else if (getFields)
					{
						this.AddQueryDataForReferenceId (reader, currentEntityId, field);
					}
				}

				foreach (StructuredTypeField field in localDefinedCollectionFields)
				{
					this.AddQueryDataForCollection (reader, currentEntityId, example, field);
				}

				if (getFields && localValueFields.Length > 0)
				{
					foreach (StructuredTypeField field in localValueFields)
					{
						this.AddQueryField (reader, currentEntityId, field.Id);
					}
				}

				if (getFields && isRootType)
				{
					this.AddQueryField (reader, currentEntityId, DataBrowser.typeColumn);
					this.AddQueryField (reader, currentEntityId, DataBrowser.idColumn);
				}

				this.tableAliasManager.PopSubTypeTableAlias ();
			}

		}


		private void AddQueryDataForValue(DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			DbTableColumn tableColumn = this.GetEntityTableColumn (entityId, this.tableAliasManager.PeekSubTypeTableAlias (), field.Id);
			
			IFieldPropertyStore fieldProperties = example as IFieldPropertyStore;
			AbstractType fieldType = field.Type as AbstractType;
			object fieldValue = example.InternalGetValue (field.Id);
			
			DbSelectCondition condition = new DbSelectCondition (this.DbInfrastructure.Converter);

			switch (fieldType.TypeCode)
			{
				case TypeCode.String:
					condition.AddCondition (tableColumn, DbCompare.Like, (string) fieldValue);
					break;

				case TypeCode.Decimal:
					throw new System.NotImplementedException ();

				case TypeCode.Double:
					throw new System.NotImplementedException ();

				case TypeCode.Integer:
					condition.AddCondition (tableColumn, DbCompare.Equal, (int) fieldValue);
					break;

				case TypeCode.LongInteger:
					condition.AddCondition (tableColumn, DbCompare.Equal, (long) fieldValue);
					break;
					
				case TypeCode.Boolean:
					condition.AddCondition (tableColumn, DbCompare.Equal, (bool) fieldValue);
					break;

				case TypeCode.Date:
					throw new System.NotImplementedException ();

				case TypeCode.DateTime:
					throw new System.NotImplementedException ();

				case TypeCode.Time:
					throw new System.NotImplementedException ();

				default:
					throw new System.NotImplementedException ();
			}

			reader.AddCondition (condition);
		}


		//private void QueryDataForString(DbReader reader, DbTableColumn tableColumn, StructuredTypeField field, AbstractType fieldType, IFieldPropertyStore fieldProperties, string fieldValue)
		//{
		//    if (!string.IsNullOrEmpty (fieldValue))
		//    {
		//        DbSelectCondition condition = new DbSelectCondition (this.DbInfrastructure.Converter);
				
		//        IStringType fieldStringType = fieldType as IStringType;

		//        StringSearchBehavior searchBehavior = fieldStringType.DefaultSearchBehavior;
		//        StringComparisonBehavior comparisonBehavior = fieldStringType.DefaultComparisonBehavior;

		//        if (fieldProperties != null)
		//        {
		//            if (fieldProperties.ContainsValue (field.Id, StringType.DefaultSearchBehaviorProperty))
		//            {
		//                searchBehavior = (StringSearchBehavior) fieldProperties.GetValue (field.Id, StringType.DefaultSearchBehaviorProperty);
		//            }

		//            if (fieldProperties.ContainsValue (field.Id, StringType.DefaultComparisonBehaviorProperty))
		//            {
		//                comparisonBehavior = (StringComparisonBehavior) fieldProperties.GetValue (field.Id, StringType.DefaultComparisonBehaviorProperty);
		//            }
		//        }

		//        // TODO Do something useful withcomparisonBehavior or delete it.

		//        string pattern = this.CreateSearchPattern (fieldValue, searchBehavior);

		//        if (pattern.Contains (DbSqlStandard.CompareLikeEscape))
		//        {
		//            condition.AddCondition (tableColumn, DbCompare.LikeEscape, pattern);
		//        }
		//        else
		//        {
		//            condition.AddCondition (tableColumn, DbCompare.Like, pattern);
		//        }

		//        reader.AddCondition (condition);
		//    }
		//}


		///// <summary>
		///// Creates the SQL LIKE compatible search pattern.
		///// </summary>
		///// <param name="searchPattern">The search pattern.</param>
		///// <param name="searchBehavior">The search behavior.</param>
		///// <returns>The SQL LIKE compatible search pattern.</returns>
		//private string CreateSearchPattern(string searchPattern, StringSearchBehavior searchBehavior)
		//{
		//    string pattern;

		//    switch (searchBehavior)
		//    {
		//        case StringSearchBehavior.ExactMatch:
		//            pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
		//            break;

		//        case StringSearchBehavior.WildcardMatch:
		//            pattern = DbSqlStandard.ConvertToCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
		//            break;

		//        case StringSearchBehavior.MatchStart:
		//            pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
		//            pattern = string.Concat (pattern, "%");
		//            break;

		//        case StringSearchBehavior.MatchEnd:
		//            pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
		//            pattern = string.Concat ("%", pattern);
		//            break;

		//        case StringSearchBehavior.MatchAnywhere:
		//            pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
		//            pattern = string.Concat ("%", pattern, "%");
		//            break;

		//        default:
		//            throw new System.ArgumentException (string.Format ("Unsupported search behavior {0}", searchBehavior));
		//    }

		//    return pattern;
		//}


		private void AddQueryDataForReferenceId(DbReader reader, Druid sourceEntityId, StructuredTypeField sourceField)
		{
			Druid targetEntityId = sourceField.TypeId;

			this.AddRelationJoin1 (reader, sourceEntityId, Druid.Parse (sourceField.Id), SqlJoinCode.OuterLeft);

			string tableAlias = this.tableAliasManager.PeekRootTypeTableAlias ();

			DbTableColumn tableColumn = this.GetRelationTableColumn (sourceEntityId, Druid.Parse(sourceField.Id), tableAlias, DataBrowser.relationTargetColumn);

			reader.AddQueryField (tableColumn);

			this.tableAliasManager.PopRootTypeTableAlias ();
		}


		private void AddQueryDataForReferenceJoin(DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			AbstractEntity targetEntity = sourceExample.InternalGetValue (sourceField.Id) as AbstractEntity;

			Druid targetEntityId = targetEntity.GetEntityStructuredTypeId ();

			this.AddRelationJoin1 (reader, sourceEntityId, Druid.Parse (sourceField.Id), SqlJoinCode.Inner);
			this.AddRelationJoin2 (reader, sourceEntityId, Druid.Parse (sourceField.Id), targetEntityId, SqlJoinCode.Inner);

			this.AddQueryDataForEntity (reader, targetEntityId, targetEntity, false);

			this.tableAliasManager.PopRootTypeTableAlias ();
			this.tableAliasManager.PopRootTypeTableAlias ();
		}


		private void AddQueryDataForReferenceIdJoin(DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			AbstractEntity targetEntity = sourceExample.InternalGetValue (sourceField.Id) as AbstractEntity;
		
			Druid targetEntityId = targetEntity.GetEntityStructuredTypeId ();

			this.AddRelationJoin1 (reader, sourceEntityId, Druid.Parse (sourceField.Id), SqlJoinCode.Inner);
			this.AddRelationJoin2 (reader, sourceEntityId, Druid.Parse (sourceField.Id), targetEntityId, SqlJoinCode.Inner);

			string tableAlias = this.tableAliasManager.PeekRootTypeTableAlias ();
			DbTableColumn tableColumn = this.GetEntityTableColumn (targetEntityId, tableAlias, DataBrowser.idColumn);
			reader.AddQueryField (tableColumn);

			this.AddQueryDataForEntity (reader, targetEntityId, targetEntity, false);

			this.tableAliasManager.PopRootTypeTableAlias ();
			this.tableAliasManager.PopRootTypeTableAlias ();
		}


		private void AddQueryDataForCollection(DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			foreach (object referencedObject in sourceExample.InternalGetFieldCollection (sourceField.Id))
			{
				AbstractEntity targetEntity = referencedObject as AbstractEntity;

				Druid targetEntityId = targetEntity.GetEntityStructuredTypeId ();

				this.AddRelationJoin1 (reader, sourceEntityId, Druid.Parse(sourceField.Id), SqlJoinCode.Inner);
				this.AddRelationJoin2 (reader, sourceEntityId, Druid.Parse (sourceField.Id), targetEntityId, SqlJoinCode.Inner);

				this.AddQueryDataForEntity (reader, targetEntityId, targetEntity, false);

				this.tableAliasManager.PopRootTypeTableAlias ();
				this.tableAliasManager.PopRootTypeTableAlias ();
			}
		}


		private void AddSubTypingJoin(DbReader reader, Druid subTypeEntityId, Druid rootTypeEntityId)
		{
			string subTypeTableAlias = this.tableAliasManager.PeekSubTypeTableAlias ();
			string rootTypeTableAlias = this.tableAliasManager.PeekRootTypeTableAlias ();

			DbTableColumn subEntityColumn = this.GetEntityTableColumn (subTypeEntityId, subTypeTableAlias, DataBrowser.idColumn);
			DbTableColumn superEntityColumn = this.GetEntityTableColumn (rootTypeEntityId, rootTypeTableAlias, DataBrowser.idColumn);
			
			SqlJoinCode type = SqlJoinCode.Inner;

			reader.AddJoin (subEntityColumn, superEntityColumn, type);
		}


		private void AddRelationJoin1(DbReader reader, Druid sourceEntityId, Druid sourcefieldId, SqlJoinCode joinType)
		{
			Druid rootSourceEntityId = this.DataContext.EntityContext.GetBaseEntityId (sourceEntityId);
			
			string sourceTableAlias = this.tableAliasManager.PeekRootTypeTableAlias ();
			string relationTableAlias = this.tableAliasManager.PushRootTypeTableAlias ();
			
			DbTableColumn sourceColumnId = this.GetEntityTableColumn (rootSourceEntityId, sourceTableAlias, DataBrowser.idColumn);
			DbTableColumn relationSourceColumnId = this.GetRelationTableColumn (sourceEntityId, sourcefieldId, relationTableAlias, DataBrowser.relationSourceColumn);

			reader.AddJoin (sourceColumnId, relationSourceColumnId, joinType);
		}


		private void AddRelationJoin2(DbReader reader, Druid sourceEntityId, Druid sourcefieldId, Druid targetEntityId, SqlJoinCode joinType)
		{
			Druid rootTargetEntityId = this.DataContext.EntityContext.GetBaseEntityId (targetEntityId);

			string relationTableAlias = this.tableAliasManager.PeekRootTypeTableAlias ();
			string targetTableAlias = this.tableAliasManager.PushRootTypeTableAlias ();

			DbTableColumn relationTargetColumnId = this.GetRelationTableColumn (sourceEntityId, sourcefieldId, relationTableAlias, DataBrowser.relationTargetColumn);
			DbTableColumn targetColumnId = this.GetEntityTableColumn (rootTargetEntityId, targetTableAlias, DataBrowser.idColumn);

			reader.AddJoin (relationTargetColumnId, targetColumnId, joinType);
		}


		private void AddQueryField(DbReader reader, Druid entityId, string columnName)
		{
			string tableAlias = this.tableAliasManager.PeekSubTypeTableAlias ();
			DbTableColumn tableColumn = this.GetEntityTableColumn (entityId, tableAlias, columnName);

			reader.AddQueryField (tableColumn);
		}


		private DbTableColumn GetEntityTableColumn(Druid entityId, string entityAlias, string columName)
		{
			DbTable dbTable = this.SchemaEngine.FindTableDefinition (entityId);
			DbColumn dbColumn = dbTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = entityAlias,
				ColumnAlias = columName,
			};
		}


		private DbTableColumn GetRelationTableColumn(Druid sourceEntityId, Druid sourceFieldId, string relationAlias, string columName)
		{
			string sourceTableName = this.SchemaEngine.GetDataTableName (sourceEntityId);
			string sourceColumnName = this.SchemaEngine.GetDataColumnName (sourceFieldId.ToString ());
			string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);

			DbTable dbRelationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);
			DbColumn dbColumn = dbRelationTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = relationAlias,
				ColumnAlias = columName,
			};
		}

		private TableAliasManager tableAliasManager;

		private static string relationSourceColumn = "[" + Tags.ColumnRefSourceId + "]";

		private static string relationTargetColumn = "[" + Tags.ColumnRefTargetId + "]";

		private static string idColumn = "[" + Tags.ColumnId + "]";

		private static string typeColumn = "[" + Tags.ColumnInstanceType + "]";

	}

}
