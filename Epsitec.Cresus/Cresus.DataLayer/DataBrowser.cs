﻿//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.EntityData;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Helpers;
using Epsitec.Cresus.DataLayer.TableAlias;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer
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


		public IEnumerable<T> GetByExample<T>(T example, bool loadFromDatabase = true)
			where T : AbstractEntity
		{
			return this.GetByExample<T> (example, new EntityConstrainer (), loadFromDatabase);
		}


		public IEnumerable<T> GetByExample<T>(T example, EntityConstrainer searchData, bool loadFromDatabase = true)
			where T : AbstractEntity
		{
			foreach (EntityDataContainer entityData in this.GetEntitiesData (example, searchData))
			{
				T entity = this.DataContext.ResolveEntity (entityData, loadFromDatabase) as T;

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

			return this.GetByExample (example, loadFromDatabase).Select (sourceEntity => System.Tuple.Create (sourceEntity, sourceFieldPath));
		}


		private IEnumerable<EntityDataContainer> GetEntitiesData(AbstractEntity example, EntityConstrainer searchData)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valuesData;
			Dictionary<DbKey, EntityReferenceData> referencesData;
			Dictionary<DbKey, EntityCollectionData> collectionsData;

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (transaction, example, searchData);
				referencesData = this.GetReferenceData (transaction, example, searchData);
				collectionsData = this.GetCollectionData (transaction, example, searchData);

				transaction.Commit ();
			}

			foreach (DbKey entityKey in valuesData.Keys)
			{
				Druid loadedEntityId = example.GetEntityStructuredTypeId ();
				Druid realEntityId = valuesData[entityKey].Item1;
				EntityValueData entityValueData = valuesData[entityKey].Item2;
				EntityReferenceData entityReferenceData = referencesData[entityKey];
				EntityCollectionData entityCollectionData = collectionsData.ContainsKey(entityKey) ? collectionsData[entityKey] : new EntityCollectionData();

				yield return new EntityDataContainer (entityKey, loadedEntityId, realEntityId, entityValueData, entityReferenceData, entityCollectionData);
			}
		}


		private Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> GetValueData(DbTransaction transaction, AbstractEntity example, EntityConstrainer searchData)
		{
			Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, EntityValueData>> ();

			Druid leafEntityId = example.GetEntityStructuredTypeId ();

			var fieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			using (DbReader reader = this.CreateValuesReader (leafEntityId, example, searchData))
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


		private DbReader CreateValuesReader(Druid entityId, AbstractEntity example, EntityConstrainer searchData)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AliasNode entityRootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (reader, example, searchData, entityRootAliasNode);
			this.AddQueryForValues (reader, example, entityRootAliasNode);
			
			return reader;
		}


		private Dictionary<DbKey, EntityReferenceData> GetReferenceData(DbTransaction transaction, AbstractEntity example, EntityConstrainer searchData)
		{
			Dictionary<DbKey, EntityReferenceData> references = new Dictionary<DbKey, EntityReferenceData> ();

			Druid leafEntityId = example.GetEntityStructuredTypeId ();

			var fieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			using (DbReader reader = this.CreateReferencesReader (leafEntityId, example, searchData))
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


		private DbReader CreateReferencesReader(Druid entityId, AbstractEntity example, EntityConstrainer searchData)
		{
			DbReader dbReader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AliasNode entityRootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (dbReader, example, searchData, entityRootAliasNode);
			this.AddQueryForReferences (dbReader, example, entityRootAliasNode);

			return dbReader;
		}


		private Dictionary<DbKey, EntityCollectionData> GetCollectionData(DbTransaction transaction, AbstractEntity example, EntityConstrainer searchData)
		{
			Dictionary<DbKey, EntityCollectionData> collectionData = new Dictionary<DbKey, EntityCollectionData> ();

			Druid entityId = example.GetEntityStructuredTypeId ();
		
			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entityId))
			{
				foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (currentId).Where (f => f.Relation == FieldRelation.Collection))
				{
					foreach (System.Tuple<DbKey, DbKey> relation in this.GetCollectionData (transaction, currentId, example, field, searchData))
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


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Druid entityId, AbstractEntity example, StructuredTypeField field, EntityConstrainer searchData)
		{
			using (DbReader reader = this.CreateCollectionReader (entityId, example, field, searchData))
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


		private DbReader CreateCollectionReader(Druid entityId, AbstractEntity example, StructuredTypeField field, EntityConstrainer searchData)
		{
			DbReader reader = new DbReader (this.DbInfrastructure)
			{
				SelectPredicate = SqlSelectPredicate.Distinct,
				IncludeRowKeys  = false,
			};

			AliasNode rootAliasNode = new AliasNode (this.EntityContext.GetRootEntityId (entityId).ToResourceId ());

			this.AddConditionsForEntity (reader, example, searchData, rootAliasNode);
			this.AddQueryForCollection (reader, example, rootAliasNode, field.CaptionId);
			this.AddSortOrderOnRank (reader, example, rootAliasNode, field.CaptionId);

			return reader;
		}


		private void AddConditionsForEntity(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias)
		{
			this.AddConditionForRootEntityStatus (dbReader, entity, rootEntityAlias);
			this.AddJoinToSubEntities (dbReader, entity, rootEntityAlias);
			this.AddConditionForFields (dbReader, entity, searchData, rootEntityAlias);
			this.AddConditionsForLocalConstraints (dbReader, entity, searchData, rootEntityAlias);
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


		private void AddConditionForFields(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias)
		{
			var definedFieldIds = this.EntityContext.GetDefinedFieldIds (entity)
				.Select<string, Druid> (id => Druid.Parse (id))
				.ToList ();

			foreach (Druid fieldId in definedFieldIds)
			{
				this.AddConditionForField (dbReader, entity, searchData, rootEntityAlias, fieldId);
			}
		}


		private void AddConditionForField(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldId.ToResourceId ());

			switch (field.Relation)
			{
				case FieldRelation.None:
					this.AddConditionForValue (dbReader, entity, rootEntityAlias, field);
					break;

				case FieldRelation.Reference:
					this.AddConditionForReference (dbReader, entity, searchData, rootEntityAlias, field);
					break;


				case FieldRelation.Collection:
					this.AddConditionForCollection (dbReader, entity, searchData, rootEntityAlias, field);
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


		private void AddConditionForReference(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			this.AddConditionForRelation (dbReader, entity, searchData, rootEntityAlias, field, target);
		}


		private void AddConditionForRelation(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias, StructuredTypeField field, AbstractEntity target)
		{
			EntityDataMapping mapping = this.DataContext.FindEntityDataMapping (target);

			if (mapping == null)
			{
				this.AddConditionForRelationByValue (dbReader, entity, searchData, rootEntityAlias, field, target);
			}
			else
			{
				this.AddConditionForRelationByReference (dbReader, entity, rootEntityAlias, field, mapping);
			}
		}


		private void AddConditionForRelationByValue(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias, StructuredTypeField field, AbstractEntity target)
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

			this.AddConditionsForEntity (dbReader, target, searchData, targetAlias);
		}


		private void AddConditionForRelationByReference(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, StructuredTypeField field, EntityDataMapping mapping)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			long targetId = mapping.RowKey.Id.Value;

			this.AddJoinToRelationWithId (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.Inner, targetId);			
		}


		private void AddConditionForCollection(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias, StructuredTypeField field)
		{
			foreach (AbstractEntity target in entity.GetFieldCollection<AbstractEntity> (field.Id))
			{
				this.AddConditionForRelation (dbReader, entity, searchData, rootEntityAlias, field, target);
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


		private void AddConditionsForLocalConstraints(DbReader dbReader, AbstractEntity entity, EntityConstrainer searchData, AliasNode rootEntityAlias)
		{
			foreach (Expression constraint in searchData.GetLocalConstraints (entity))
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
