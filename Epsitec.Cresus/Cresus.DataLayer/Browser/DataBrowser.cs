//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.EntityData;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Browser
{
	

	public class DataBrowser
	{

		
		public DataBrowser(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure) ?? new SchemaEngine (this.DbInfrastructure);
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


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
		}


		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}


		public IEnumerable<T> GetByExample<T>(T example)
			where T : AbstractEntity
		{
			Request request = new Request ()
			{
				RootEntity = example,
				RequestedEntity = example,
				LoadFromDatabase = true,
				IsRootEntityReference = false,
			};

			return this.GetByRequest<T> (request);
		}



		public IEnumerable<T> GetByRequest<T>(Request request)
			where T : AbstractEntity
		{
			foreach (EntityDataContainer entityData in this.GetEntitiesData (request))
			{
				T entity = this.DataContext.ResolveEntity (entityData, request.LoadFromDatabase) as T;

				if (entity != null)
				{
					yield return entity;
				}
			}
		}


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target, bool loadFromDatabase = true)
		{
			EntityDataMapping targetMapping = this.DataContext.FindEntityDataMapping (target);

			if (targetMapping != null)
			{
				foreach (Druid targetEntityId in this.EntityContext.GetInheritedEntityIds (target.GetEntityStructuredTypeId ()))
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

			AbstractEntity example = this.EntityContext.CreateEmptyEntity (sourceEntityId);
			StructuredTypeField field = this.EntityContext.GetStructuredTypeField (example, sourceFieldId);

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

			Request request = new Request ()
			{
				RootEntity = example,
				LoadFromDatabase = loadFromDatabase,
			};

			return this.GetByRequest<AbstractEntity> (request).Select (sourceEntity => System.Tuple.Create (sourceEntity, sourceFieldPath));
		}


		private IEnumerable<EntityDataContainer> GetEntitiesData(Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valuesData;
			Dictionary<DbKey, EntityReferenceData> referencesData;
			Dictionary<DbKey, EntityCollectionData> collectionsData;

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (transaction, request);
				referencesData = this.GetReferenceData (transaction, request);
				collectionsData = this.GetCollectionData (transaction, request);

				transaction.Commit ();
			}

			foreach (DbKey entityKey in valuesData.Keys)
			{
				Druid loadedEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();
				Druid realEntityId = valuesData[entityKey].Item1;
				EntityValueData entityValueData = valuesData[entityKey].Item2;
				EntityReferenceData entityReferenceData = referencesData[entityKey];
				EntityCollectionData entityCollectionData = collectionsData.ContainsKey(entityKey) ? collectionsData[entityKey] : new EntityCollectionData();

				yield return new EntityDataContainer (entityKey, loadedEntityId, realEntityId, entityValueData, entityReferenceData, entityCollectionData);
			}
		}


		private Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> GetValueData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> ();

			Druid leafEntityId = request.RootEntity.GetEntityStructuredTypeId ();

			var fieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			using (DbReader reader = this.CreateValuesReader (request))
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
							entityValueData[fieldIds[i]] = rowData.Values[i];
						}
					}

					valueData[entityKey] = System.Tuple.Create (realEntityId, entityValueData);
				}
			}

			return valueData;
		}


		private DbReader CreateValuesReader(Request request)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			
			AliasNode entityRootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (leafEntityId).ToResourceId ());

			this.AddConditionsForEntity (reader, entity, request, entityRootAliasNode);
			this.AddQueryForValues (reader, entity, entityRootAliasNode);
			
			return reader;
		}


		private Dictionary<DbKey, EntityReferenceData> GetReferenceData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, EntityReferenceData> references = new Dictionary<DbKey, EntityReferenceData> ();

			Druid leafEntityId = request.RootEntity.GetEntityStructuredTypeId ();

			var fieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			using (DbReader reader = this.CreateReferencesReader (request))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					DbKey sourceKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					EntityReferenceData entityReferenceData = new EntityReferenceData ();

					for (int i = 0; i < rowData.Values.Length - 1; i++)
					{
						if (rowData.Values[i] != System.DBNull.Value)
						{
							entityReferenceData[fieldIds[i]] = new DbKey (new DbId ((long) rowData.Values[i]));
						}
					}

					references[sourceKey] = entityReferenceData;
				}
			}

			return references;
		}


		private DbReader CreateReferencesReader(Request request)
		{
			DbReader dbReader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			AliasNode entityRootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (leafEntityId).ToResourceId ());

			this.AddConditionsForEntity (dbReader, entity, request, entityRootAliasNode);
			this.AddQueryForReferences (dbReader, entity, entityRootAliasNode);

			return dbReader;
		}


		private Dictionary<DbKey, EntityCollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, EntityCollectionData> collectionData = new Dictionary<DbKey, EntityCollectionData> ();

			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
		
			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId).Where (f => f.Relation == FieldRelation.Collection))
				{
					foreach (System.Tuple<DbKey, DbKey> relation in this.GetCollectionData (transaction, request, field))
					{
						if (!collectionData.ContainsKey (relation.Item1))
						{
							collectionData[relation.Item1] = new EntityCollectionData ();
						}

						collectionData[relation.Item1][field.CaptionId].Add (relation.Item2);
					}
				}
			}

			return collectionData;
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Request request, StructuredTypeField field)
		{
			using (DbReader reader = this.CreateCollectionReader (request, field))
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


		private DbReader CreateCollectionReader(Request request, StructuredTypeField field)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			AliasNode rootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (leafEntityId).ToResourceId ());

			this.AddConditionsForEntity (reader, entity, request, rootAliasNode);
			this.AddQueryForCollection (reader, entity, rootAliasNode, field.CaptionId);
			this.AddSortOrderOnRank (reader, entity, rootAliasNode, field.CaptionId);

			return reader;
		}


		private void AddConditionsForEntity(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias)
		{
			this.AddConditionForRootEntityStatus (dbReader, entity, rootEntityAlias);
			this.AddJoinToSubEntities (dbReader, entity, rootEntityAlias);
			this.AddConditionForFields (dbReader, entity, request, rootEntityAlias);
			this.AddConditionsForLocalConstraints (dbReader, entity, request, rootEntityAlias);
		}


		private void AddConditionForRootEntityStatus(DbReader dbReader, AbstractEntity entity, AliasNode rootAliasNode)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);
			
			this.AddConditionForEntityStatus (dbReader, rootAliasNode, rootEntityId);
		}


		private void AddConditionForEntityStatus(DbReader dbReader, AliasNode rootAliasNode, Druid localEntityId)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootAliasNode, localEntityId);

			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, localAlias.Alias, DataBrowser.statusColumn);

			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Equal, (short) DbRowStatus.Live),
			};

			dbReader.AddCondition (condition);
		}


		private void AddJoinToSubEntities(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			var heritedEntityIds = this.EntityContext.GetInheritedEntityIds (leafEntityId)
				.Where (id => id != rootEntityId)
				.ToList ();
			
			foreach (Druid localEntityId in heritedEntityIds)
			{
				this.AddJoinToSubEntity(dbReader, rootEntityId, localEntityId, rootEntityAlias);
			}
		}


		private void AddJoinToSubEntity(DbReader dbReader, Druid rootEntityId, Druid localEntityId, AliasNode rootEntityAlias)
		{
			AliasNode localEntityAlias = rootEntityAlias.CreateChild (localEntityId.ToResourceId ());

			string rootTableAlias = rootEntityAlias.Alias;
			string localTableAlias = localEntityAlias.Alias;

			DbTableColumn localEntityIdColumn = this.GetEntityColumn (localEntityId, localTableAlias, DataBrowser.idColumn);
			DbTableColumn localEntityStatusColumn = this.GetEntityColumn (localEntityId, localTableAlias, DataBrowser.statusColumn);
			DbTableColumn rootEntityIdColumn = this.GetEntityColumn (rootEntityId, rootTableAlias, DataBrowser.idColumn);

			SqlJoinCode type = SqlJoinCode.Inner;

			DbJoin join = new DbJoin (localEntityIdColumn, rootEntityIdColumn, type)
			{
				Condition = DbSimpleCondition.CreateCondition (localEntityStatusColumn, DbSimpleConditionOperator.Equal, (short) DbRowStatus.Live),
			};

			dbReader.AddJoin (join);
		}


		private void AddConditionForFields(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias)
		{
			var definedFieldIds = this.EntityContext.GetDefinedFieldIds (entity)
				.Select<string, Druid> (id => Druid.Parse (id))
				.ToList ();

			foreach (Druid fieldId in definedFieldIds)
			{
				this.AddConditionForField (dbReader, entity, request, rootEntityAlias, fieldId);
			}
		}


		private void AddConditionForField(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldId.ToResourceId ());

			switch (field.Relation)
			{
				case FieldRelation.None:
					this.AddConditionForValue (dbReader, entity, rootEntityAlias, field);
					break;

				case FieldRelation.Reference:
					this.AddConditionForReference (dbReader, entity, request, rootEntityAlias, field);
					break;


				case FieldRelation.Collection:
					this.AddConditionForCollection (dbReader, entity, request, rootEntityAlias, field);
					break;
			}
		}


		private void AddConditionForValue(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, StructuredTypeField field)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, field.CaptionId);

			string alias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId).Alias;
			string fieldId = field.Id;
			
			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, alias, fieldId);

			AbstractType fieldType = field.Type as AbstractType;
			object fieldValue = entity.InternalGetValue (fieldId);

			DbSelectCondition condition = new DbSelectCondition ();

			switch (fieldType.TypeCode)
			{
				case TypeCode.String:
					condition.Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Like, (string) fieldValue);
					break;

				case TypeCode.Decimal:
					throw new System.NotImplementedException ();

				case TypeCode.Double:
					throw new System.NotImplementedException ();

				case TypeCode.Integer:
					condition.Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Equal, (int) fieldValue);
					break;

				case TypeCode.LongInteger:
					condition.Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Equal, (long) fieldValue);
					break;

				case TypeCode.Boolean:
					condition.Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Equal, (bool) fieldValue);
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

			dbReader.AddCondition (condition);
		}


		private void AddConditionForReference(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			this.AddConditionForRelation (dbReader, entity, request, rootEntityAlias, field, target);
		}


		private void AddConditionForRelation(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias, StructuredTypeField field, AbstractEntity target)
		{
			EntityDataMapping mapping = this.DataContext.FindEntityDataMapping (target);

			if (mapping == null)
			{
				this.AddConditionForRelationByValue (dbReader, entity, request, rootEntityAlias, field, target);
			}
			else
			{
				this.AddConditionForRelationByReference (dbReader, entity, rootEntityAlias, field, mapping);
			}
		}


		private void AddConditionForRelationByValue(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.EntityContext.GetRootEntityId (leafTargetId);

			this.AddJoinToRelation (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.Inner);
			AliasNode relationAlias = rootEntityAlias.GetChildren (field.Id).Last ();

			this.AddJoinToTarget (dbReader, localEntityId, fieldId, rootTargetId, relationAlias, SqlJoinCode.Inner);
			AliasNode targetAlias = relationAlias.GetChild (rootTargetId.ToResourceId ());

			this.AddConditionsForEntity (dbReader, target, request, targetAlias);
		}


		private void AddConditionForRelationByReference(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, StructuredTypeField field, EntityDataMapping mapping)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			long targetId = mapping.RowKey.Id.Value;

			this.AddJoinToRelationWithId (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.Inner, targetId);			
		}


		private void AddConditionForCollection(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias, StructuredTypeField field)
		{
			foreach (AbstractEntity target in entity.GetFieldCollection<AbstractEntity> (field.Id))
			{
				this.AddConditionForRelation (dbReader, entity, request, rootEntityAlias, field, target);
			}
		}


		private void AddJoinToRelation(DbReader dbReader, Druid localEntityId, Druid fieldId, AliasNode rootEntityAlias, SqlJoinCode joinType)
		{
			DbJoin join = this.BuildJoinToRelation (localEntityId, fieldId, rootEntityAlias, joinType);

			dbReader.AddJoin (join);
		}


		private void AddJoinToRelationWithId(DbReader dbReader, Druid localEntityId, Druid fieldId, AliasNode rootEntityAlias, SqlJoinCode joinType, long targetId)
		{
			DbJoin join = this.BuildJoinToRelation (localEntityId, fieldId, rootEntityAlias, joinType);
			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			DbTableColumn idColumn = this.GetRelationColumn (localEntityId, fieldId, relationAlias.Alias, DataBrowser.relationTargetColumn);

			DbAbstractCondition part1 = join.Condition;
			DbAbstractCondition part2 = new DbSimpleCondition (idColumn, DbSimpleConditionOperator.Equal, targetId, DbRawType.Int64);

			join.Condition = new DbConditionCombiner (
				DbConditionCombinerOperator.And,
				part1,
				part2
			);

			dbReader.AddJoin (join);
		}


		private DbJoin BuildJoinToRelation(Druid localEntityId, Druid fieldId, AliasNode rootEntityAlias, SqlJoinCode joinType)
		{
			AliasNode relationAlias = rootEntityAlias.CreateChild (fieldId.ToResourceId ());

			string entityTableAlias =  rootEntityAlias.Alias;
			string relationTableAlias = relationAlias.Alias;

			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);

			DbTableColumn sourceIdColumn = this.GetEntityColumn (rootEntityId, entityTableAlias, DataBrowser.idColumn);
			DbTableColumn relationSourceIdColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, DataBrowser.relationSourceColumn);
			DbTableColumn relationSourceStatusColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, DataBrowser.statusColumn);

			DbJoin join = new DbJoin (sourceIdColumn, relationSourceIdColumn, joinType)
			{
				Condition = DbSimpleCondition.CreateCondition (relationSourceStatusColumn, DbSimpleConditionOperator.Equal, (short) DbRowStatus.Live),
			};

			return join;
		}


		private void AddJoinToTarget(DbReader dbReader, Druid localEntityId, Druid fieldId, Druid rootTargetId, AliasNode relationAlias, SqlJoinCode joinType)
		{
			DbJoin join = this.BuildJoinToTarget (localEntityId, fieldId, rootTargetId, relationAlias, joinType);

			dbReader.AddJoin (join);
		}


		private DbJoin BuildJoinToTarget(Druid localEntityId, Druid fieldId, Druid rootTargetId, AliasNode relationAlias, SqlJoinCode joinType)
		{
			AliasNode targetAlias = relationAlias.CreateChild (rootTargetId.ToResourceId ());

			string relationTableAlias = relationAlias.Alias;
			string targetTableAlias = targetAlias.Alias;

			DbTableColumn relationTargetIdColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, DataBrowser.relationTargetColumn);
			DbTableColumn targetIdColumn = this.GetEntityColumn (rootTargetId, targetTableAlias, DataBrowser.idColumn);
			DbTableColumn targetStatusColumn = this.GetEntityColumn (rootTargetId, targetTableAlias, DataBrowser.statusColumn);

			DbJoin join = new DbJoin (relationTargetIdColumn, targetIdColumn, joinType)
			{
				Condition = DbSimpleCondition.CreateCondition (targetStatusColumn, DbSimpleConditionOperator.Equal, (short) DbRowStatus.Live),
			};

			return join;
		}


		private void AddConditionsForLocalConstraints(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias)
		{
			foreach (Expression constraint in request.GetLocalConstraints (entity))
			{
				this.AddConditionForLocalConstraint (dbReader, entity, constraint, rootEntityAlias);
			}
		}


		private void AddConditionForLocalConstraint(DbReader dbReader, AbstractEntity entity, Expression constraint, AliasNode rootEntityAlias)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			DbSelectCondition selectCondition = new DbSelectCondition ()
			{
				Condition = constraint.CreateDbAbstractCondition (entity, fieldId =>
				{
					Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

					string tableAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId).Alias;
					string columnName = fieldId.ToResourceId ();

					return this.GetEntityColumn (localEntityId, tableAlias, columnName);
				}),
			};

			dbReader.AddCondition (selectCondition);
		}


		private void AddQueryForValues(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			var valueFieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			foreach (Druid fieldId in valueFieldIds)
			{
				Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

				this.AddEntityQueryField (dbReader, rootEntityAlias, localEntityId, fieldId.ToResourceId ());
			}

			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, DataBrowser.typeColumn);
			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, DataBrowser.idColumn);
		}


		private void AddQueryForReferences(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			var relationFieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId())
				.ToList ();

			foreach (Druid fieldId in relationFieldIds)
			{
				this.AddQueryForReference (dbReader, entity, rootEntityAlias, fieldId);
			}

			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, DataBrowser.idColumn);
		}


		private void AddQueryForReference(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);
			
			if (!this.EntityContext.IsFieldDefined(fieldId.ToResourceId(), entity))
			{
				this.AddJoinToRelation (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.OuterLeft);
			}
			
			AliasNode relationAlias = rootEntityAlias.GetChild (fieldId.ToResourceId ());

			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, DataBrowser.relationTargetColumn);
		}


		private void AddQueryForCollection(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			this.AddJoinToRelation (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.Inner);

			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, DataBrowser.relationTargetColumn);
			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, DataBrowser.relationRankColumn);

			this.AddEntityQueryField (dbReader, rootEntityAlias, this.EntityContext.GetRootEntityId (localEntityId), DataBrowser.idColumn);
		}


		private void AddSortOrderOnRank(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			DbTableColumn column = this.GetRelationColumn (localEntityId, fieldId, relationAlias.Alias, DataBrowser.relationRankColumn);

			dbReader.AddSortOrder (column, SqlSortOrder.Ascending);
		}


		private void AddEntityQueryField(DbReader dbReader, AliasNode rootEntityAlias, Druid localEntityId, string columnName)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId);
			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, localAlias.Alias, columnName);

			dbReader.AddQueryField (tableColumn);
		}


		private DbTableColumn GetEntityColumn(Druid entityId, string entityAlias, string columName)
		{
			DbTable dbTable = this.SchemaEngine.FindTableDefinition (entityId);
			DbColumn dbColumn = dbTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = entityAlias,
				ColumnAlias = columName,
			};
		}


		private void AddRelationQueryField(DbReader dbReader, string relationAlias, Druid localEntityId, Druid fieldId, string columnName)
		{
			DbTableColumn tableColumnRank = this.GetRelationColumn (localEntityId, fieldId, relationAlias, columnName);

			dbReader.AddQueryField (tableColumnRank);
		}


		private DbTableColumn GetRelationColumn(Druid localEntityId, Druid fieldId, string relationAlias, string columName)
		{
			string sourceTableName = this.SchemaEngine.GetDataTableName (localEntityId);
			string sourceColumnName = this.SchemaEngine.GetDataColumnName (fieldId.ToString ());
			string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);

			DbTable dbRelationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);
			DbColumn dbColumn = dbRelationTable.Columns[this.SchemaEngine.GetDataColumnName (columName)];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = relationAlias,
				ColumnAlias = columName,
			};
		}


		private AliasNode GetLocalEntityAlias(AliasNode rootEntityAlias, Druid localEntityId)
		{
			string name = localEntityId.ToResourceId ();

			if (rootEntityAlias.Name == name)
			{
				return rootEntityAlias;
			}
			else
			{
				return rootEntityAlias.GetChild (name);
			}
		}


		private static string relationSourceColumn = "[" + Tags.ColumnRefSourceId + "]";


		private static string relationTargetColumn = "[" + Tags.ColumnRefTargetId + "]";


		private static string relationRankColumn = "[" + Tags.ColumnRefRank + "]";

		
		private static string idColumn = "[" + Tags.ColumnId + "]";


		private static string typeColumn = "[" + Tags.ColumnInstanceType + "]";


		private static string statusColumn = "[" + Tags.ColumnStatus + "]";


	}


}
