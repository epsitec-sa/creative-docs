//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Linq;
using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer
{
	

	// TODO Add sorting criteria
	// TODO Add comparison criteria
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


		public IEnumerable<EntityType> GetByExample<EntityType>(EntityType example, bool loadFromDatabase) where EntityType : AbstractEntity
		{
			foreach (EntityData entityData in this.GetEntitiesData (example))
			{
				EntityType entity = this.DataContext.ResolveEntity (entityData, loadFromDatabase) as EntityType;

				if (entity != null)
				{
					yield return entity;
				}
			}
		}


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target, bool loadFromDatabase)
		{
			EntityDataMapping targetMapping = this.DataContext.FindEntityDataMapping (target);

			if (targetMapping != null)
			{
				foreach (Druid targetEntityId in this.DataContext.EntityContext.GetHeritedEntityIds (target.GetEntityStructuredTypeId ()))
				{
					foreach (EntityFieldPath sourceFieldPath in this.DbInfrastructure.GetSourceReferences (targetEntityId))
					{
						foreach (System.Tuple<AbstractEntity, EntityFieldPath> item in this.GetReferencers (sourceFieldPath, target, loadFromDatabase))
						{
							yield return item;
						}
					}
				}
			}
		}


		private IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(EntityFieldPath sourceFieldPath, AbstractEntity target, bool loadFromDatabase)
		{
			Druid sourceEntityId = sourceFieldPath.EntityId;
			string sourceFieldId = sourceFieldPath.Fields.First ();

			AbstractEntity example = this.DataContext.EntityContext.CreateEmptyEntity (sourceEntityId);
			StructuredTypeField field = this.DataContext.EntityContext.GetStructuredTypeField (example, sourceFieldId);

			using (example.DefineOriginalValues ())
			{
				if (field.Relation == FieldRelation.Reference)
				{
					example.SetField<object> (field.Id, target);
				}
				else if (field.Relation == FieldRelation.Collection)
				{
					example.InternalGetFieldCollection (field.Id).Add (target);
				}
			}
			
			return this.GetByExample (example, loadFromDatabase).Select (sourceEntity => System.Tuple.Create (sourceEntity, sourceFieldPath));
		}


		private IEnumerable<EntityData> GetEntitiesData(AbstractEntity example)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valuesData;
			Dictionary<DbKey, EntityReferenceData> referencesData;
			Dictionary<DbKey, EntityCollectionData> collectionsData;

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (transaction, example);
				referencesData = this.GetReferenceData (transaction, example);
				collectionsData = this.GetCollectionData (transaction, example);

				transaction.Commit ();
			}

			foreach (DbKey entityKey in valuesData.Keys)
			{
				Druid loadedEntityId = example.GetEntityStructuredTypeId ();
				Druid realEntityId = valuesData[entityKey].Item1;
				EntityValueData entityValueData = valuesData[entityKey].Item2;
				EntityReferenceData entityReferenceData = referencesData[entityKey];
				EntityCollectionData entityCollectionData = collectionsData.ContainsKey(entityKey) ? collectionsData[entityKey] : new EntityCollectionData();

				yield return new EntityData (entityKey, loadedEntityId, realEntityId, entityValueData, entityReferenceData, entityCollectionData);
			}
		}


		private Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> GetValueData(DbTransaction transaction, AbstractEntity example)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> ();

			Druid entityId = example.GetEntityStructuredTypeId ();
			
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
					EntityValueData entityValueData = new EntityValueData ();
					Druid realEntityId = Druid.FromLong ((long) rowData.Values[rowData.Values.Length - 2]);
					DbKey entityKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					for (int i = 0; i < rowData.Values.Length - 2; i++)
					{
						if (rowData.Values[i] != System.DBNull.Value)
						{
							entityValueData[fields[i]] = rowData.Values[i];
						}
					}

					valueData[entityKey] = System.Tuple.Create (realEntityId, entityValueData);
				}
			}

			return valueData;
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

			return reader;
		}


		private Dictionary<DbKey, EntityReferenceData> GetReferenceData(DbTransaction transaction, AbstractEntity example)
		{
			Dictionary<DbKey, EntityReferenceData> references = new Dictionary<DbKey, EntityReferenceData> ();

			Druid entityId = example.GetEntityStructuredTypeId ();

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
					DbKey sourceKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					EntityReferenceData entityReferenceData = new EntityReferenceData();

					for (int i = 0; i < rowData.Values.Length - 1; i++)
					{
						if (rowData.Values[i] != System.DBNull.Value)
						{
							entityReferenceData[fields[i]] = new DbKey (new DbId ((long) rowData.Values[i]));
						}
					}

					references[sourceKey] = entityReferenceData;
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

			return reader;
		}


		private Dictionary<DbKey, EntityCollectionData> GetCollectionData(DbTransaction transaction, AbstractEntity example)
		{
			Dictionary<DbKey, EntityCollectionData> collectionData = new Dictionary<DbKey, EntityCollectionData> ();

			Druid entityId = example.GetEntityStructuredTypeId ();

			foreach (Druid currentId in this.DataContext.EntityContext.GetHeritedEntityIds (entityId))
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityLocalFieldDefinitions (currentId).Where (f => f.Relation == FieldRelation.Collection))
				{
					foreach (System.Tuple<DbKey, DbKey> relation in this.GetCollectionData (transaction, currentId, example, field))
					{
						if (!collectionData.ContainsKey (relation.Item1))
						{
							collectionData[relation.Item1] = new EntityCollectionData ();
						}

						collectionData[relation.Item1][field].Add (relation.Item2);
					}
				}
			}

			return collectionData;
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			using (DbReader reader = this.CreateCollectionReader (entityId, example, field))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					DbKey sourceKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));
					DbKey targetKey = new DbKey (new DbId ((long) rowData.Values[0]));

					yield return System.Tuple.Create (sourceKey, targetKey);
				}
			}
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
			this.AddQueryForCollection (tableAliasManager, reader, entityId, example, field);
			this.AddSortOrderOnRank (tableAliasManager, reader, entityId, example, field);

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
					this.AddJoinFromSubEntityTableToSuperEntityTable (tableAliasManager, reader, currentEntityId, heritedEntityIds.Last ());
				}

				if (isRootType)
				{
					this.AddConditionForEntityStatus (tableAliasManager, reader, currentEntityId);
				}

				foreach (StructuredTypeField field in localDefinedFields)
				{
					this.AddConditionForField (tableAliasManager, reader, currentEntityId, example, field);
				}
			}
		}


		private void AddConditionForEntityStatus(TableAliasManager tableAliasManager, DbReader reader, Druid currentEntityId)
		{
			DbTableColumn tableColumn = this.GetEntityTableColumn (currentEntityId, tableAliasManager.GetCurrentSubtypeAlias (), DataBrowser.statusColumn);

			DbSelectCondition condition = new DbSelectCondition ();
			condition.AddCondition (tableColumn, DbCompare.Equal, (short) DbRowStatus.Live);
			reader.AddCondition (condition);
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


		private void AddConditionForValue(TableAliasManager tableAliasManager, DbReader reader, Druid currentEntityId, AbstractEntity example, StructuredTypeField field)
		{
			DbTableColumn tableColumn = this.GetEntityTableColumn (currentEntityId, tableAliasManager.GetCurrentSubtypeAlias (), field.Id);
			
			IFieldPropertyStore fieldProperties = example as IFieldPropertyStore;
			AbstractType fieldType = field.Type as AbstractType;
			object fieldValue = example.InternalGetValue (field.Id);
			
			DbSelectCondition condition = new DbSelectCondition ();

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
			if (this.DataContext.FindEntityDataMapping (targetExample) == null)
			{
				this.AddContidionForReferenceByValue (tableAliasManager, reader, sourceEntityId, sourceExample, sourceField, targetExample);
			}
			else
			{
				this.AddConditionForReferenceByReference (tableAliasManager, reader, sourceEntityId, sourceExample, sourceField, targetExample);	
			}
		}


		private void AddContidionForReferenceByValue(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField, AbstractEntity targetExample)
		{
			Druid targetEntityId = targetExample.GetEntityStructuredTypeId ();

			this.AddJoinFromEntityTableToRelationTable (tableAliasManager, reader, sourceEntityId, sourceField, SqlJoinCode.Inner);
			this.AddJoinFromRelationTableToTargetTable (tableAliasManager, reader, sourceEntityId, sourceField, targetEntityId, SqlJoinCode.Inner);

			this.AddConditionsForEntity (tableAliasManager, reader, targetEntityId, targetExample);

			tableAliasManager.GetPreviousEntityAlias ();
			tableAliasManager.GetPreviousEntityAlias ();
		}


		private void AddConditionForReferenceByReference(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField, AbstractEntity targetExample)
		{
			EntityDataMapping mapping = this.DataContext.FindEntityDataMapping (targetExample);
			
			this.AddJoinFromEntityTableToRelationTable (tableAliasManager, reader, sourceEntityId, sourceField, SqlJoinCode.Inner, mapping.RowKey);

			tableAliasManager.GetPreviousEntityAlias ();
		}


		private void AddConditionForCollection(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, AbstractEntity sourceExample, StructuredTypeField sourceField)
		{
			foreach (object targetExample in sourceExample.InternalGetFieldCollection (sourceField.Id))
			{
				this.AddConditionForReference (tableAliasManager, reader, sourceEntityId, sourceExample, sourceField, targetExample as AbstractEntity);
			}
		}


		private void AddJoinFromSubEntityTableToSuperEntityTable(TableAliasManager tableAliasManager, DbReader reader, Druid subTypeEntityId, Druid rootTypeEntityId)
		{
			string subTypeTableAlias = tableAliasManager.GetCurrentSubtypeAlias ();
			string rootTypeTableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn subEntityIdColumn = this.GetEntityTableColumn (subTypeEntityId, subTypeTableAlias, DataBrowser.idColumn);
			DbTableColumn subEntityStatusColumn = this.GetEntityTableColumn (subTypeEntityId, subTypeTableAlias, DataBrowser.statusColumn);
			DbTableColumn superEntityIdColumn = this.GetEntityTableColumn (rootTypeEntityId, rootTypeTableAlias, DataBrowser.idColumn);

			SqlJoinCode type = SqlJoinCode.Inner;

			DbJoin join = new DbJoin (subEntityIdColumn, superEntityIdColumn, type);

			join.AddCondition (subEntityStatusColumn, DbCompare.Equal, (short) DbRowStatus.Live);
			
			reader.AddJoin (join);
		}


		private void AddJoinFromEntityTableToRelationTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, SqlJoinCode joinType)
		{
			DbJoin join = this.BuildJoinFromEntityTableToRelationTable (tableAliasManager, reader, sourceEntityId, sourcefield, joinType);

			reader.AddJoin (join);
		}


		private void AddJoinFromEntityTableToRelationTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, SqlJoinCode joinType, DbKey targetKey)
		{
			DbJoin join = this.BuildJoinFromEntityTableToRelationTable (tableAliasManager, reader, sourceEntityId, sourcefield, joinType);

			string relationTableAlias = tableAliasManager.GetCurrentEntityAlias ();
			DbTableColumn relationTargetIdColumn = this.GetRelationTableColumn (sourceEntityId, Druid.Parse (sourcefield.Id), relationTableAlias, DataBrowser.relationTargetColumn);

			join.AddCondition (relationTargetIdColumn, DbCompare.Equal, targetKey.Id.Value);

			reader.AddJoin (join);
		}


		private DbJoin BuildJoinFromEntityTableToRelationTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, SqlJoinCode joinType)
		{
			Druid rootSourceEntityId = this.DataContext.EntityContext.GetBaseEntityId (sourceEntityId);

			string sourceTableAlias = tableAliasManager.GetCurrentEntityAlias ();
			string relationTableAlias = tableAliasManager.CreateEntityAlias (sourcefield.Id);

			DbTableColumn sourceIdColumn = this.GetEntityTableColumn (rootSourceEntityId, sourceTableAlias, DataBrowser.idColumn);
			DbTableColumn relationSourceIdColumn = this.GetRelationTableColumn (sourceEntityId, Druid.Parse (sourcefield.Id), relationTableAlias, DataBrowser.relationSourceColumn);
			DbTableColumn relationSourceStatusColumn = this.GetRelationTableColumn (sourceEntityId, Druid.Parse (sourcefield.Id), relationTableAlias, DataBrowser.statusColumn);

			DbJoin join = new DbJoin (sourceIdColumn, relationSourceIdColumn, joinType);

			join.AddCondition (relationSourceStatusColumn, DbCompare.Equal, (short) DbRowStatus.Live);

			return join;
		}


		private void AddJoinFromRelationTableToTargetTable(TableAliasManager tableAliasManager, DbReader reader, Druid sourceEntityId, StructuredTypeField sourcefield, Druid targetEntityId, SqlJoinCode joinType)
		{
			Druid rootTargetEntityId = this.DataContext.EntityContext.GetBaseEntityId (targetEntityId);

			string relationTableAlias = tableAliasManager.GetCurrentEntityAlias ();
			string targetTableAlias = tableAliasManager.CreateEntityAlias (targetEntityId.ToResourceId ());

			DbTableColumn relationTargetIdColumn = this.GetRelationTableColumn (sourceEntityId, Druid.Parse (sourcefield.Id), relationTableAlias, DataBrowser.relationTargetColumn);
			DbTableColumn targetIdColumn = this.GetEntityTableColumn (rootTargetEntityId, targetTableAlias, DataBrowser.idColumn);
			DbTableColumn targetStatusColumn = this.GetEntityTableColumn (rootTargetEntityId, targetTableAlias, DataBrowser.statusColumn);

			DbJoin join = new DbJoin (relationTargetIdColumn, targetIdColumn, joinType);

			join.AddCondition (targetStatusColumn, DbCompare.Equal, (short) DbRowStatus.Live);

			reader.AddJoin (join);
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
					this.AddJoinFromSubEntityTableToSuperEntityTable (tableAliasManager, reader, currentEntityId, heritedEntityIds.Last ());
				}

				foreach (StructuredTypeField field in localValueFields)
				{
					this.AddEntityQueryField (tableAliasManager, reader, currentEntityId, field.Id);
				}

				if (isRootType)
				{
					this.AddEntityQueryField (tableAliasManager, reader, currentEntityId, DataBrowser.typeColumn);
					this.AddEntityQueryField (tableAliasManager, reader, currentEntityId, DataBrowser.idColumn);
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
					this.AddEntityQueryField (tableAliasManager, reader, example.GetEntityContext ().GetBaseEntityId (currentEntityId), DataBrowser.idColumn);
				}
			}
		}


		private void AddQueryForReference(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, StructuredTypeField field, bool useExistingJoin)
		{
			if (!useExistingJoin)
			{
				this.AddJoinFromEntityTableToRelationTable (tableAliasManager, reader, entityId, field, SqlJoinCode.OuterLeft);
			}
			else
			{
				tableAliasManager.GetNextEntityAlias (field.Id);
			}

			this.AddRelationQueryField (tableAliasManager, reader, entityId, field, DataBrowser.relationTargetColumn);

			tableAliasManager.GetPreviousEntityAlias ();
		}


		private void AddQueryForCollection(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			this.AddJoinFromEntityTableToRelationTable (tableAliasManager, reader, entityId, field, SqlJoinCode.Inner);

			this.AddRelationQueryField (tableAliasManager, reader, entityId, field, DataBrowser.relationTargetColumn);
			this.AddRelationQueryField (tableAliasManager, reader, entityId, field, DataBrowser.relationRankColumn);

			tableAliasManager.GetPreviousEntityAlias ();

			this.AddEntityQueryField (tableAliasManager, reader, example.GetEntityContext ().GetBaseEntityId (entityId), DataBrowser.idColumn);
		}


		private void AddSortOrderOnRank(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, AbstractEntity example, StructuredTypeField field)
		{
			string[] definedFieldIds = example.GetEntityContext ().GetDefinedFieldIds (example).ToArray ();

			if (definedFieldIds.Contains (field.Id))
			{
				tableAliasManager.GetNextEntityAlias (field.Id, 1);
			}
			else
			{
				tableAliasManager.GetNextEntityAlias (field.Id, 0);
			}

			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();

			DbTableColumn column = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, DataBrowser.relationRankColumn);

			reader.AddSortOrder (column, SqlSortOrder.Ascending);

			tableAliasManager.GetPreviousEntityAlias ();
		}


		private void AddRelationQueryField(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, StructuredTypeField field, string columnName)
		{
			string tableAlias = tableAliasManager.GetCurrentEntityAlias ();
			DbTableColumn tableColumnRank = this.GetRelationTableColumn (entityId, Druid.Parse (field.Id), tableAlias, columnName);

			reader.AddQueryField (tableColumnRank);
		}


		private void AddEntityQueryField(TableAliasManager tableAliasManager, DbReader reader, Druid entityId, string columnName)
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


		private static string statusColumn = "[" + Tags.ColumnStatus + "]";

	}


}
