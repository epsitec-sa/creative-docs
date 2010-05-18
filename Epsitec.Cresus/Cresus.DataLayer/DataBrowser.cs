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

				DbReader valuesReader = this.CreateValueReader (entityId, example);
				DbReader referencesReader = this.CreateReferenceReader (entityId, example);

				valuesReader.CreateDataReader (transaction);
				referencesReader.CreateDataReader (transaction);

				DataTable valuesTable = this.BuildValuesTable (entityId, valuesReader);
				DataTable referencesTable = this.BuildReferencesTable (entityId);

				IEnumerator<DbReader.RowData> referencesRows = referencesReader.Rows.GetEnumerator ();


				foreach (DbReader.RowData valuesRow in valuesReader.Rows)
				{
					referencesRows.MoveNext ();
					DbReader.RowData referencesRow = referencesRows.Current;

					DbKey rowKey = this.ExtractDbKey (valuesRow);
					Druid realEntityId = this.ExtractRealEntityId (valuesRow);
					DataRow valuesDataRow = this.ExtractDataRow (valuesTable, valuesRow);
					DataRow referencesDataRow = this.ExtractDataRow (referencesTable, referencesRow);

					yield return this.DataContext.ResolveEntity (realEntityId, entityId, rowKey, valuesDataRow, referencesDataRow) as EntityType;
				}
			}
		}


		private DataTable BuildValuesTable(Druid entityId, DbReader reader)
		{
			DataTable dataTable = new DataTable ();

			foreach (Druid id in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				StructuredTypeField[] localFields = this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (id).ToArray ();

				StructuredTypeField[] localValueFields = localFields.Where (field => field.Relation == FieldRelation.None).ToArray ();
				
				foreach (StructuredTypeField field in localValueFields)
				{
					string name = this.DataContext.SchemaEngine.GetDataColumnName (field.Id);
					System.Type type = reader.GetColumnType (name);

					dataTable.Columns.Add (new DataColumn (name, type));
				}
			}

			return dataTable;
		}



		private DataTable BuildReferencesTable(Druid entityId)
		{
			DataTable dataTable = new DataTable ();

			StructuredTypeField[] referenceFields = this.DataContext.EntityContext.GetEntityFieldDefinitions(entityId).
				Where (field => field.Relation == FieldRelation.Reference).
				Where (field => field.Source == FieldSource.Value).
				ToArray ();

			foreach (StructuredTypeField field in referenceFields)
			{
				string name = this.DataContext.SchemaEngine.GetDataColumnName (field.Id);
				System.Type type = typeof (string);

				dataTable.Columns.Add (new DataColumn (name, type));
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
		
		private DbReader CreateValueReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForValues (tableAliasManager, reader, entityId, example);
			this.AddSortOrder (tableAliasManager, reader, entityId, example);

			return reader;
		}


		private DbReader CreateReferenceReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForReferences (tableAliasManager, reader, entityId, example);
			this.AddSortOrder (tableAliasManager, reader, entityId, example);

			return reader;
		}


		private DbReader CreateCollectionReader(Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForCollection (reader, entityId, example, field);
			this.AddSortOrder (tableAliasManager, reader, entityId, example);

			return reader;
		}









		private void AddConditionsForEntity(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();
			Druid[] heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToArray ();

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				bool isRootType = currentEntityId == heritedEntityIds.Last ();
				tableAliasManager.CreateSubtypeAlias (currentEntityId.ToResourceId (), isRootType);

				StructuredTypeField[] localDefinedFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).Where (field => definedFieldIds.Contains (field.Id)).ToArray ();
				StructuredTypeField[] localDefinedValueFields = localDefinedFields.Where (field => field.Relation == FieldRelation.None).ToArray ();
				
				if (!isRootType && localDefinedValueFields.Length > 0)
				{
					this.AddSubTypingJoin (tableAliasManager, reader, currentEntityId, heritedEntityIds.Last ());
				}

				foreach (StructuredTypeField field in localDefinedFields)
				{
					this.AddConditionForField (tableAliasManager, reader, currentEntityId, example, field);
				}
			}
		}


		private void AddConditionForField(TableAliasManager tableAliasManager, DbReader reader, Druid currentEntityId, AbstractEntity example, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					this.AddConditionForValue (tableAliasManager, reader, currentEntityId, example, field);
					break;

				case FieldRelation.Reference:
					this.AddConditionForReference (tableAliasManager, reader, currentEntityId, example, field, example.InternalGetValue (field.Id) as AbstractEntity);
					break;


				case FieldRelation.Collection:
					this.AddConditionForCollection (tableAliasManager, reader, currentEntityId, example, field);
					break;
			}
		}



		private void AddConditionForValue(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			DbTableColumn tableColumn = this.GetEntityTableColumn (entityId, tableAliasManager.GetCurrentSubtypeAlias (), field.Id);
			
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


#if false


		private void QueryDataForString(DbReader reader, DbTableColumn tableColumn, StructuredTypeField field, AbstractType fieldType, IFieldPropertyStore fieldProperties, string fieldValue)
		{
			if (!string.IsNullOrEmpty (fieldValue))
			{
				DbSelectCondition condition = new DbSelectCondition (this.DbInfrastructure.Converter);

				IStringType fieldStringType = fieldType as IStringType;

				StringSearchBehavior searchBehavior = fieldStringType.DefaultSearchBehavior;
				StringComparisonBehavior comparisonBehavior = fieldStringType.DefaultComparisonBehavior;

				if (fieldProperties != null)
				{
					if (fieldProperties.ContainsValue (field.Id, StringType.DefaultSearchBehaviorProperty))
					{
						searchBehavior = (StringSearchBehavior) fieldProperties.GetValue (field.Id, StringType.DefaultSearchBehaviorProperty);
					}

					if (fieldProperties.ContainsValue (field.Id, StringType.DefaultComparisonBehaviorProperty))
					{
						comparisonBehavior = (StringComparisonBehavior) fieldProperties.GetValue (field.Id, StringType.DefaultComparisonBehaviorProperty);
					}
				}

				// TODO Do something useful withcomparisonBehavior or delete it.

				string pattern = this.CreateSearchPattern (fieldValue, searchBehavior);

				if (pattern.Contains (DbSqlStandard.CompareLikeEscape))
				{
					condition.AddCondition (tableColumn, DbCompare.LikeEscape, pattern);
				}
				else
				{
					condition.AddCondition (tableColumn, DbCompare.Like, pattern);
				}

				reader.AddCondition (condition);
			}
		}


#endif


#if false


		private string CreateSearchPattern(string searchPattern, StringSearchBehavior searchBehavior)
		{
			string pattern;

			switch (searchBehavior)
			{
				case StringSearchBehavior.ExactMatch:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					break;

				case StringSearchBehavior.WildcardMatch:
					pattern = DbSqlStandard.ConvertToCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					break;

				case StringSearchBehavior.MatchStart:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat (pattern, "%");
					break;

				case StringSearchBehavior.MatchEnd:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat ("%", pattern);
					break;

				case StringSearchBehavior.MatchAnywhere:
					pattern = DbSqlStandard.EscapeCompareLikeWildcards (this.DbInfrastructure.DefaultSqlBuilder, searchPattern);
					pattern = string.Concat ("%", pattern, "%");
					break;

				default:
					throw new System.ArgumentException (string.Format ("Unsupported search behavior {0}", searchBehavior));
			}

			return pattern;
		}


#endif


		private void AddConditionForReference(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField, AbstractEntity targetExample)
		{
			Druid targetEntityId = targetExample.GetEntityStructuredTypeId ();

			this.AddRelationJoinToRelationTable (tableAliasManager, reader, sourceEntityId, sourceField, SqlJoinCode.Inner);
			this.AddRelationJoinToTargetTable (tableAliasManager, reader, sourceEntityId, sourceField, targetEntityId, SqlJoinCode.Inner);

			this.AddConditionsForEntity (tableAliasManager, reader, targetEntityId, targetExample);

			tableAliasManager.GetPreviousEntityAlias ();
			tableAliasManager.GetPreviousEntityAlias ();
		}


		private void AddConditionForCollection(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			foreach (object targetExample in sourceExample.InternalGetFieldCollection (sourceField.Id))
			{
				this.AddConditionForReference (tableAliasManager, reader, sourceEntityId, sourceExample, sourceField, targetExample as AbstractEntity);
			}
		}


		private void AddSubTypingJoin(TableAliasManager tableAliasManager, DbReader reader, Druid subTypeEntityId, Druid rootTypeEntityId)
		{
			string subTypeTableAlias = tableAliasManager.GetCurrentSubtypeAlias ();
			string rootTypeTableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn subEntityColumn = this.GetEntityTableColumn (subTypeEntityId, subTypeTableAlias, DataBrowser.idColumn);
			DbTableColumn superEntityColumn = this.GetEntityTableColumn (rootTypeEntityId, rootTypeTableAlias, DataBrowser.idColumn);
			
			SqlJoinCode type = SqlJoinCode.Inner;

			reader.AddJoin (subEntityColumn, superEntityColumn, type);
		}


		private void AddRelationJoinToRelationTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, SqlJoinCode joinType)
		{
			Druid rootSourceEntityId = this.DataContext.EntityContext.GetBaseEntityId (sourceEntityId);
			
			string sourceTableAlias = tableAliasManager.GetCurrentEntityAlias ();
			string relationTableAlias = tableAliasManager.CreateEntityAlias (sourcefield.Id);
			
			DbTableColumn sourceColumnId = this.GetEntityTableColumn (rootSourceEntityId, sourceTableAlias, DataBrowser.idColumn);
			DbTableColumn relationSourceColumnId = this.GetRelationTableColumn (sourceEntityId, Druid.Parse(sourcefield.Id), relationTableAlias, DataBrowser.relationSourceColumn);

			reader.AddJoin (sourceColumnId, relationSourceColumnId, joinType);
		}


		private void AddRelationJoinToTargetTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, Druid targetEntityId, SqlJoinCode joinType)
		{
			Druid rootTargetEntityId = this.DataContext.EntityContext.GetBaseEntityId (targetEntityId);

			string relationTableAlias = tableAliasManager.GetCurrentEntityAlias ();
			string targetTableAlias = tableAliasManager.CreateEntityAlias (targetEntityId.ToResourceId ());

			DbTableColumn relationTargetColumnId = this.GetRelationTableColumn (sourceEntityId, Druid.Parse(sourcefield.Id), relationTableAlias, DataBrowser.relationTargetColumn);
			DbTableColumn targetColumnId = this.GetEntityTableColumn (rootTargetEntityId, targetTableAlias, DataBrowser.idColumn);

			reader.AddJoin (relationTargetColumnId, targetColumnId, joinType);
		}














		private void AddQueryForValues(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();
			Druid[] heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToArray ();

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				bool isRootType = currentEntityId == heritedEntityIds.Last ();
				
				StructuredTypeField[] localValueFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).Where (field => field.Relation == FieldRelation.None).ToArray ();

				if (isRootType || localValueFields.Any(field => definedFieldIds.Contains (field.Id)))
				{
					tableAliasManager.GetNextSubtypeAlias (currentEntityId.ToResourceId ());
				}
				else
				{
					tableAliasManager.CreateSubtypeAlias (currentEntityId.ToResourceId (), isRootType);
					this.AddSubTypingJoin (tableAliasManager, reader, currentEntityId, heritedEntityIds.Last ());
				}

				foreach (StructuredTypeField field in localValueFields)
				{
					this.AddQueryField (tableAliasManager, reader, currentEntityId, field.Id);
				}

				if (isRootType)
				{
					this.AddQueryField (tableAliasManager, reader, currentEntityId, DataBrowser.typeColumn);
					this.AddQueryField (tableAliasManager, reader, currentEntityId, DataBrowser.idColumn);
				}
			}
		}











		private void AddQueryForReferences(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();
			Druid[] heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToArray ();

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				StructuredTypeField[] localReferenceFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).Where (field => field.Relation == FieldRelation.Reference).ToArray ();

				foreach (StructuredTypeField field in localReferenceFields)
				{
					this.AddQueryForReference (tableAliasManager, reader, currentEntityId, field, definedFieldIds.Contains (field.Id));
				}

				if (currentEntityId == heritedEntityIds.Last ())
				{
					this.AddQueryField (tableAliasManager, reader, example.GetEntityContext ().GetBaseEntityId (currentEntityId), DataBrowser.idColumn);
				}

			}
		}


		private void AddQueryForReference(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, StructuredTypeField field, bool useExistingJoin)
		{
			if (!useExistingJoin)
			{
				this.AddRelationJoinToRelationTable (tableAliasManager, reader, entityId, field, SqlJoinCode.OuterLeft);
			}
			else
			{
				tableAliasManager.GetNextEntityAlias (field.Id);
			}

			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn tableColumn = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, DataBrowser.relationTargetColumn);

			reader.AddQueryField (tableColumn);

			tableAliasManager.GetPreviousEntityAlias ();
		}












		private void AddQueryForCollection(DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			
		}

















		private void AddSortOrder(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example)
		{
			Druid rootEntityId = this.DataContext.EntityContext.GetBaseEntityId (entityId);
			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn column = this.GetEntityTableColumn (rootEntityId, tableAlias, DataBrowser.idColumn);

			reader.AddSortOrder (column, SqlSortOrder.Ascending);
		}










		private void AddQueryField(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, string columnName)
		{
			string tableAlias = tableAliasManager.GetCurrentSubtypeAlias ();
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


		private static string relationSourceColumn = "[" + Tags.ColumnRefSourceId + "]";


		private static string relationTargetColumn = "[" + Tags.ColumnRefTargetId + "]";


		private static string idColumn = "[" + Tags.ColumnId + "]";


		private static string typeColumn = "[" + Tags.ColumnInstanceType + "]";


	}


}
