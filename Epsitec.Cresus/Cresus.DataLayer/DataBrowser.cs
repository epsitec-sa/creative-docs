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
	// TODO Implement that read/write stuff that we talked about with Pierre to ensure the consistency of write operations.
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
				foreach (var entityData in this.GetEntitiesData(transaction, entityId, example))
				{
					DbKey rowKey = entityData.Item1;
					Druid realEntityId = entityData.Item2;
					Dictionary<StructuredTypeField, object> fieldValues = entityData.Item3;
					Dictionary<StructuredTypeField, DbKey> referencesKeys = entityData.Item4;
					Dictionary<StructuredTypeField, List<DbKey>> collectionsKeys = entityData.Item5;

					yield return this.DataContext.ResolveEntity (realEntityId, entityId, rowKey, fieldValues, referencesKeys, collectionsKeys) as EntityType;
				}
			}
		}


		private IEnumerable<System.Tuple<DbKey, Druid, Dictionary<StructuredTypeField, object>, Dictionary<StructuredTypeField, DbKey>, Dictionary<StructuredTypeField, List<DbKey>>>> GetEntitiesData(DbTransaction transaction, Druid entityId, AbstractEntity example)
		{
			List<System.Tuple<DbKey, Druid, Dictionary<StructuredTypeField, object>>> valuesData = this.GetValues (transaction, entityId, example);
			List<System.Tuple<DbKey, Dictionary<StructuredTypeField, DbKey>>> referencesData = this.GetReferences (transaction, entityId, example);
			Dictionary<StructuredTypeField, List<System.Tuple<DbKey, DbKey>>> collectionsData = this.GetCollections (transaction, entityId, example);

			if (valuesData.Count != referencesData.Count)
			{
				throw new System.Exception ("Invalid data.");
			}

			Dictionary<StructuredTypeField, int> collectionIterators = new Dictionary<StructuredTypeField, int> ();

			foreach (StructuredTypeField field in collectionsData.Keys)
			{
				collectionIterators[field] = 0;
			}

			for (int i = 0; i < valuesData.Count; i++)
			{
				if (valuesData[i].Item1 != referencesData[i].Item1)
				{
					throw new System.Exception ("Invalid data");
				}

				DbKey rowKey = valuesData[i].Item1;
				Druid realEntityId = valuesData[i].Item2;
				Dictionary<StructuredTypeField, object> fieldValues = valuesData[i].Item3;
				Dictionary<StructuredTypeField, DbKey> referencesKeys = referencesData[i].Item2;

				Dictionary<StructuredTypeField, List<DbKey>> collectionsKeys = new Dictionary<StructuredTypeField, List<DbKey>> ();

				foreach (StructuredTypeField field in collectionsData.Keys)
				{
					List<System.Tuple<DbKey, DbKey>> keys = collectionsData[field];
					List<DbKey> currentKeys = new List<DbKey> ();

					for (int index = collectionIterators[field]; index < keys.Count && keys[index].Item1 == rowKey; index++)
					{
						currentKeys.Add (keys[index].Item2);
					}

					collectionIterators[field] = collectionIterators[field] + currentKeys.Count;
					collectionsKeys[field] = currentKeys;
				}

				yield return System.Tuple.Create (rowKey, realEntityId, fieldValues, referencesKeys, collectionsKeys);
			}
		}


		private List<System.Tuple<DbKey, Druid, Dictionary<StructuredTypeField, object>>> GetValues(DbTransaction transaction, Druid entityId, AbstractEntity example)
		{
			List<System.Tuple<DbKey, Druid, Dictionary<StructuredTypeField, object>>> values = new List<System.Tuple<DbKey, Druid, Dictionary<StructuredTypeField, object>>> ();
			List<StructuredTypeField> fields = new List<StructuredTypeField> ();

			foreach (Druid currentEntityId in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (currentEntityId).Where (f => f.Relation == FieldRelation.None))
				{
					fields.Add (field);
				}
			}

			using (DbReader reader = this.CreateValuesReader (entityId, example))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					Dictionary<StructuredTypeField, object> currentValues = new Dictionary<StructuredTypeField, object> ();
					Druid realEntityId = Druid.FromLong ((long) rowData.Values[rowData.Values.Length - 2]);
					DbKey entityKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					for (int i = 0; i < rowData.Values.Length - 2; i++)
					{
						if (rowData.Values[i] != System.DBNull.Value)
						{
							StructuredTypeField field = fields[i];
							object value = rowData.Values[i];
							currentValues[field] = value;
						}
					}

					values.Add (System.Tuple.Create (entityKey, realEntityId, currentValues));
				}
			}

			return values;
		}


		private DbReader CreateValuesReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForValues (tableAliasManager, reader, entityId, example);
			this.AddSortOrderOnId (tableAliasManager, reader, entityId);

			return reader;
		}


		private List<System.Tuple<DbKey, Dictionary<StructuredTypeField, DbKey>>> GetReferences(DbTransaction transaction, Druid entityId, AbstractEntity example)
		{
			List<System.Tuple<DbKey, Dictionary<StructuredTypeField, DbKey>>> references = new List<System.Tuple<DbKey, Dictionary<StructuredTypeField, DbKey>>> ();
			List<StructuredTypeField> fields = new List<StructuredTypeField> ();

			foreach (Druid currentEntityId in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (currentEntityId).Where (f => f.Relation == FieldRelation.Reference))
				{
					fields.Add(field);	
				}
			}

			using (DbReader reader = this.CreateReferencesReader (entityId, example))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					Dictionary<StructuredTypeField, DbKey> currentReferences = new Dictionary<StructuredTypeField, DbKey> ();
					DbKey sourceKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					for (int i = 0; i < rowData.Values.Length - 1; i++)
					{
						if (rowData.Values[i] != System.DBNull.Value)
						{
							StructuredTypeField field = fields[i];
							DbKey targetKey = new DbKey (new DbId ((long) rowData.Values[i]));
							currentReferences[field] = targetKey;
						}
					}

					references.Add (System.Tuple.Create (sourceKey, currentReferences));
				}
			}

			return references;
		}


		private DbReader CreateReferencesReader(Druid entityId, AbstractEntity example)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForReferences (tableAliasManager, reader, entityId, example);
			this.AddSortOrderOnId (tableAliasManager, reader, entityId);

			return reader;
		}


		private Dictionary<StructuredTypeField, List<System.Tuple<DbKey, DbKey>>> GetCollections(DbTransaction transaction, Druid entityId, AbstractEntity example)
		{
			Dictionary<StructuredTypeField, List<System.Tuple<DbKey, DbKey>>> collections = new Dictionary<StructuredTypeField, List<System.Tuple<DbKey, DbKey>>> ();

			foreach (Druid currentId in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (currentId).Where (f => f.Relation == FieldRelation.Collection))
				{
					collections[field] = this.GetCollection (transaction, currentId, example, field);
				}
			}

			return collections;
		}


		private List<System.Tuple<DbKey, DbKey>> GetCollection(DbTransaction transaction, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			List<System.Tuple<DbKey, DbKey>> collection = new List<System.Tuple<DbKey, DbKey>> ();

			using (DbReader reader = this.CreateCollectionReader (entityId, example, field))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					DbKey sourceKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));
					DbKey targetKey = new DbKey (new DbId ((long) rowData.Values[0]));

					collection.Add (new System.Tuple<DbKey, DbKey> (sourceKey, targetKey));
				}
			}

			return collection;
		}


		private DbReader CreateCollectionReader(Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.All,
				IncludeRowKeys  = false,
			};

			TableAliasManager tableAliasManager = new TableAliasManager (this.DataContext.EntityContext.GetBaseEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (tableAliasManager, reader, entityId, example);
			this.AddQueryForCollection (tableAliasManager, reader, entityId, example, field);
			this.AddSortOrderOnId (tableAliasManager, reader, entityId);
			this.AddSortOrderOnRank (tableAliasManager, reader, entityId, field);

			return reader;
		}


		private void AddConditionsForEntity(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();
			Druid[] heritedEntityIds = example.GetEntityContext ().GetHeritedEntityIds (entityId).ToArray ();

			foreach (Druid currentEntityId in heritedEntityIds)
			{
				bool isLeafType = currentEntityId == heritedEntityIds.First();
				bool isRootType = currentEntityId == heritedEntityIds.Last ();

				tableAliasManager.CreateSubtypeAlias (currentEntityId.ToResourceId (), isRootType);

				StructuredTypeField[] localDefinedFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).Where (field => definedFieldIds.Contains (field.Id)).ToArray ();
				StructuredTypeField[] localDefinedValueFields = localDefinedFields.Where (field => field.Relation == FieldRelation.None).ToArray ();
				
				if (!isRootType && (localDefinedValueFields.Length > 0 || isLeafType))
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
				bool isLeafType = currentEntityId == heritedEntityIds.First ();
				bool isRootType = currentEntityId == heritedEntityIds.Last ();
				
				StructuredTypeField[] localValueFields = example.GetEntityContext ().GetEntityLocalFieldDefinitions (currentEntityId).Where (field => field.Relation == FieldRelation.None).ToArray ();

				if (isRootType || isLeafType || localValueFields.Any (field => definedFieldIds.Contains (field.Id)))
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


		private void AddQueryForCollection(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			this.AddRelationJoinToRelationTable (tableAliasManager, reader, entityId, field, SqlJoinCode.OuterLeft);
			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn tableColumnTarget = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, DataBrowser.relationTargetColumn);
			reader.AddQueryField (tableColumnTarget);

			DbTableColumn tableColumnRank = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, DataBrowser.relationRankColumn);
			reader.AddQueryField (tableColumnRank);

			tableAliasManager.GetPreviousEntityAlias ();

			this.AddQueryField (tableAliasManager, reader, example.GetEntityContext ().GetBaseEntityId (entityId), DataBrowser.idColumn);
		}


		private void AddSortOrderOnId(TableAliasManager tableAliasManager, DbReader reader, Druid entityId)
		{
			Druid rootEntityId = this.DataContext.EntityContext.GetBaseEntityId (entityId);
			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn column = this.GetEntityTableColumn (rootEntityId, tableAlias, DataBrowser.idColumn);

			reader.AddSortOrder (column, SqlSortOrder.Ascending);
		}


		private void AddSortOrderOnRank(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, StructuredTypeField field)
		{
			string tableAlias = tableAliasManager.GetNextEntityAlias (field.Id);

			DbTableColumn column = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, DataBrowser.relationRankColumn);

			reader.AddSortOrder (column, SqlSortOrder.Ascending);

			tableAliasManager.GetPreviousEntityAlias ();
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


		private static string relationRankColumn = "[" + Tags.ColumnRefRank + "]";


		private static string idColumn = "[" + Tags.ColumnId + "]";


		private static string typeColumn = "[" + Tags.ColumnInstanceType + "]";


	}


}
