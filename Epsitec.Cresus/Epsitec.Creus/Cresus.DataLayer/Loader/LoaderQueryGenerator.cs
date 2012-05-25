//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;

using System;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class LoaderQueryGenerator
	{


		public LoaderQueryGenerator(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dataContext.DataInfrastructure.DbInfrastructure;
			}
		}


		private EntityTypeEngine TypeEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}


		private EntitySchemaEngine SchemaEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntitySchemaEngine;
			}
		}


		private DataConverter DataConverter
		{
			get
			{
				return this.dataContext.DataConverter;
			}
		}


		public int GetCount(Request request)
		{
			var sqlSelect = this.CreateSqlSelectForCount (request);

			using (var dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				var count = this.GetCount (sqlSelect, dbTransaction);

				dbTransaction.Commit ();

				return count;
			}
		}


		public int GetCount(Request request, DbTransaction dbTransaction)
		{
			var sqlSelect = this.CreateSqlSelectForCount (request);

			return this.GetCount (sqlSelect, dbTransaction);
		}


		public int GetCount(SqlSelect sqlSelect, DbTransaction dbTransaction)
		{
			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			return (int) this.DbInfrastructure.ExecuteScalar (dbTransaction);
		}


		public IEnumerable<EntityKey> GetEntityKeys(Request request, DbTransaction dbTransaction)
		{
			var sqlSelect = this.CreateSqlSelectForEntityKeys (request);

			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var data = this.DbInfrastructure.ExecuteRetData (dbTransaction);

			var leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();
			var rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				var rowKey = new DbKey (new DbId ((long) dataRow[0]));

				yield return new EntityKey (rootEntityId, rowKey);
			}
		}


		public int? GetIndex(Request request, EntityKey entityKey, DbTransaction dbTransaction)
		{
			// HACK This method is a big hack that I implemented only because Pierre wanted something
			// quickly. It is totally inefficient and I plan to implement a better way to do this
			// which will be more performant.

			var sqlSelect = this.CreateSqlSelectForEntityKeys (request);

			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var data = this.DbInfrastructure.ExecuteRetData (dbTransaction);

			long expectedId = entityKey.RowKey.Id;
			int i = 0;
			int? result = null;

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				var rowId = (long) dataRow[0];

				if (rowId == expectedId)
				{
					result = i;

					break;
				}

				i++;
			}

			return result;
		}


		public IEnumerable<EntityData> GetEntitiesData(Request request)
		{
			List<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesAndReferencesData = this.GetValueAndReferenceData (transaction, request).ToList ();

				if (valuesAndReferencesData.Count > 0)
				{
					collectionsData = this.GetCollectionData (transaction, request);
				}
				else
				{
					collectionsData = new Dictionary<DbKey, CollectionData> ();
				}

				transaction.Commit ();
			}

			return this.GetEntitiesData (request, valuesAndReferencesData, collectionsData);
		}


		private IEnumerable<EntityData> GetEntitiesData(Request request, IEnumerable<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData, Dictionary<DbKey, CollectionData> collectionsData)
		{
			foreach (var valueAndReferenceData in valuesAndReferencesData)
			{
				Druid loadedEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

				DbKey rowKey = valueAndReferenceData.Item1;
				Druid leafEntityId = valueAndReferenceData.Item2;
				long logId = valueAndReferenceData.Item3;
				ValueData entityValueData = valueAndReferenceData.Item4;
				ReferenceData entityReferenceData = valueAndReferenceData.Item5;
				CollectionData entityCollectionData = collectionsData.ContainsKey (rowKey) ? collectionsData[rowKey] : new CollectionData ();

				yield return new EntityData (rowKey, leafEntityId, loadedEntityId, logId, entityValueData, entityReferenceData, entityCollectionData);
			}
		}

		public object GetValueField(AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			AbstractEntity example = EntityClassFactory.CreateEmptyEntity (localEntityId);
			DbKey exampleKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			Request request = Request.Create (example, exampleKey);

			object value;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				value = this.GetSingleValue (transaction, request, fieldId);

				transaction.Commit ();
			}

			return value;
		}


		public EntityData GetReferenceField(AbstractEntity entity, Druid fieldId)
		{
			string fieldName = fieldId.ToResourceId ();

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			AbstractEntity targetExample = EntityClassFactory.CreateEmptyEntity (field.TypeId);
			DbKey rootExampleKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			using (rootExample.DefineOriginalValues ())
			{
				rootExample.SetField (fieldName, targetExample);
			}

			Request request = Request.Create (rootExample, rootExampleKey, targetExample);

			List<EntityData> data = this.GetEntitiesData (request).ToList ();

			if (data.Count > 1)
			{
				throw new System.Exception ("More than one target in reference field.");
			}

			return data.FirstOrDefault ();
		}


		public IEnumerable<EntityData> GetCollectionField(AbstractEntity entity, Druid fieldId)
		{
			Dictionary<DbKey, EntityData> targetsData;
			IEnumerable<DbKey> targetKeys;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				targetsData = this.GetCollectionEntityData (entity, fieldId)
					.ToDictionary (data => data.RowKey);

				if (targetsData.Any ())
				{
					targetKeys = this.GetCollectionKeys (transaction, entity, fieldId)
						.Select (d => d.Item2).ToList ();
				}
				else
				{
					targetKeys = new List<DbKey> ();
				}

				transaction.Commit ();
			}

			List<EntityData> entityData = new List<EntityData> ();

			foreach (DbKey rowKey in targetKeys)
			{
				if (targetsData.ContainsKey (rowKey))
				{
					entityData.Add (targetsData[rowKey]);
				}
			}

			return entityData;
		}


		private IEnumerable<EntityData> GetCollectionEntityData(AbstractEntity entity, Druid fieldId)
		{
			string fieldName = fieldId.ToResourceId ();

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);

			AbstractEntity rootExample   = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			AbstractEntity targetExample = EntityClassFactory.CreateEmptyEntity (field.TypeId);
			DbKey rootExampleKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			using (rootExample.DefineOriginalValues ())
			{
				rootExample.GetFieldCollection<AbstractEntity> (fieldName).Add (targetExample);
			}

			Request request = Request.Create (rootExample, rootExampleKey, targetExample);

			return this.GetEntitiesData (request);
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionKeys(DbTransaction transaction, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			DbKey rootExampleKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			Request request = Request.Create (rootExample, rootExampleKey);

			return this.GetCollectionData (transaction, request, fieldId);
		}


		private object GetSingleValue(DbTransaction transaction, Request request, Druid fieldId)
		{
			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			SqlSelect select = this.CreateSqlSelectForSingleValue (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

			if (data.Tables.Count != 1)
			{
				throw new System.Exception ("Problem with sql query.");
			}

			DataTable table = data.Tables[0];

			object value = null;

			if (table.Rows.Count > 1)
			{
				throw new System.Exception ("Problem with sql query.");
			}
			else if (table.Rows.Count == 1)
			{
				DataRow dataRow = table.Rows[0];

				object internalValue = dataRow[0];

				if (internalValue != System.DBNull.Value)
				{
					StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);
					INamedType type = field.Type;

					Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;
					DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, field.CaptionId);

					DbTypeDef typeDef = dbColumn.Type;
					DbRawType rawType = typeDef.RawType;
					DbSimpleType simpleType = typeDef.SimpleType;
					DbNumDef numDef = typeDef.NumDef;

					value = this.DataConverter.FromDatabaseToCresusValue (type, rawType, simpleType, numDef, internalValue);
				}
			}

			return value;
		}


		private SqlSelect CreateSqlSelectForSingleValue(Request request, Druid fieldId)
		{
			var builder = this.GetBuilder ();
			var sqlContainerForConditions = this.BuildFromWhereAndOrderByClause (builder, request);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlField sqlFieldForSingleValue = builder.BuildEntityField (requestedEntity, fieldId);

			return sqlContainerForConditions.PlusSqlFields (sqlFieldForSingleValue).BuildSqlSelect ();
		}


		private SqlSelect CreateSqlSelectForCount(Request request)
		{
			var builder = this.GetBuilder ();
			var sqlContainerForConditions = this.BuildFromWhereAndOrderByClause (builder, request);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlSelectPredicate predicate = this.GetSqlSelectPredicate (request);
			SqlContainer sqlContainerForCount = this.BuildSqlContainerForCount (builder, requestedEntity, predicate);

			return sqlContainerForConditions
				.Plus (sqlContainerForCount)
				.BuildSqlSelect (skip: request.Skip, take: request.Take);
		}


		private SqlSelect CreateSqlSelectForEntityKeys(Request request)
		{
			var builder = this.GetBuilder ();
			var sqlContainerForConditions = this.BuildFromWhereAndOrderByClause (builder, request);

			AbstractEntity requestedEntity = request.RequestedEntity;

			var sqlContainerForEntityKeys = this.BuildSqlContainerForEntityKeys (builder, requestedEntity);

			var predicate = this.GetSqlSelectPredicate (request);

			return sqlContainerForConditions
				.Plus (sqlContainerForEntityKeys)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}


		private IEnumerable<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> GetValueAndReferenceData(DbTransaction transaction, Request request)
		{
			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var valueFields = this.TypeEngine.GetValueFields (leafEntityId)
				.Where (field => field.Type.SystemType != typeof (byte[]))
				.OrderBy (field => field.CaptionId.ToResourceId ())
				.ToList ();

			var referenceFields = this.TypeEngine.GetReferenceFields (leafEntityId)
				.OrderBy (field => field.CaptionId.ToResourceId ())
				.ToList ();

			SqlSelect select = this.CreateSqlSelectForValueAndReferenceData (request);

			transaction.SqlBuilder.SelectData (select);
			DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				ValueData entityValueData = new ValueData ();
				ReferenceData entityReferenceData = new ReferenceData ();
				long logId = (long) dataRow[dataRow.ItemArray.Length - 3];
				Druid realEntityId = Druid.FromLong ((long) dataRow[dataRow.ItemArray.Length - 2]);
				DbKey entityKey = new DbKey (new DbId ((long) dataRow[dataRow.ItemArray.Length - 1]));

				for (int i = 0; i < valueFields.Count; i++)
				{
					object internalValue = dataRow[i];

					if (internalValue != System.DBNull.Value)
					{
						StructuredTypeField field = valueFields[i];
						INamedType type = field.Type;

						Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;
						DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, field.CaptionId);

						DbTypeDef typeDef = dbColumn.Type;
						DbRawType rawType = typeDef.RawType;
						DbSimpleType simpleType = typeDef.SimpleType;
						DbNumDef numDef = typeDef.NumDef;

						object externalValue = this.DataConverter.FromDatabaseToCresusValue (type, rawType, simpleType, numDef, internalValue);

						entityValueData[field.CaptionId] = externalValue;
					}
				}

				for (int i = 0; i < referenceFields.Count; i++)
				{
					object value = dataRow[valueFields.Count + i];

					if (value != System.DBNull.Value)
					{
						entityReferenceData[referenceFields[i].CaptionId] = new DbKey (new DbId ((long) value));
					}
				}

				yield return Tuple.Create (entityKey, realEntityId, logId, entityValueData, entityReferenceData);
			}
		}


		private SqlSelect CreateSqlSelectForValueAndReferenceData(Request request)
		{
			var builder = this.GetBuilder ();
			var sqlContainerForConditions = this.BuildFromWhereAndOrderByClause (builder, request);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlContainer sqlContainerForValuesAndReferences = this.BuildSqlContainerForValuesAndReferences (builder, requestedEntity);

			var predicate = this.GetSqlSelectPredicate (request);

			return sqlContainerForConditions
				.Plus (sqlContainerForValuesAndReferences)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}


		private Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, CollectionData> collectionData = new Dictionary<DbKey, CollectionData> ();

			AbstractEntity requestedEntity = request.RequestedEntity;
			Druid leafRequestedEntityId = requestedEntity.GetEntityStructuredTypeId ();

			var definedFieldIds = this.TypeEngine.GetCollectionFields (leafRequestedEntityId)
				.Select (f => f.CaptionId)
				.ToList ();

			foreach (Druid fieldId in definedFieldIds)
			{
				foreach (System.Tuple<DbKey, DbKey> relation in this.GetCollectionData (transaction, request, fieldId))
				{
					if (!collectionData.ContainsKey (relation.Item1))
					{
						collectionData[relation.Item1] = new CollectionData ();
					}

					collectionData[relation.Item1][fieldId].Add (relation.Item2);
				}
			}

			return collectionData;
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Request request, Druid fieldId)
		{
			SqlSelect select = this.CreateSqlSelectForCollectionData (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				DbKey sourceKey = new DbKey (new DbId ((long) dataRow[dataRow.ItemArray.Length - 1]));
				DbKey targetKey = new DbKey (new DbId ((long) dataRow[0]));

				yield return System.Tuple.Create (sourceKey, targetKey);
			}
		}


		private SqlSelect CreateSqlSelectForCollectionData(Request request, Druid fieldId)
		{
			var builder = this.GetBuilder ();
			var sqlContainerForConditions = this.BuildFromWhereAndOrderByClause (builder, request);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlContainer sqlContainerForCollectionSourceIds = this.BuildSqlContainerForCollectionSourceIds (builder, requestedEntity);

			var predicate = this.GetSqlSelectPredicate (request);

			SqlSelect sqlSelectForSourceIds = sqlContainerForConditions
				.Plus (sqlContainerForCollectionSourceIds)
				.BuildSqlSelect (predicate, request.Skip, request.Take);

			SqlContainer sqlContainerForCollectionData = this.BuildSqlContainerForCollection (builder, requestedEntity, fieldId, sqlSelectForSourceIds);

			return sqlContainerForCollectionData.BuildSqlSelect ();
		}


		private SqlContainer BuildFromWhereAndOrderByClause(SqlFieldBuilder builder, Request request)
		{
			var nonPersistentEntities = request.GetNonPersistentEntities (this.dataContext);

			var fromClause = this.BuildFromClause (builder, request, nonPersistentEntities);
			var whereClause = this.BuildWhereClause (builder, request, nonPersistentEntities);
			var orderByClause = this.BuildOrderByClause (builder, request);

			return fromClause.Plus (whereClause).Plus (orderByClause);
		}


		private SqlContainer BuildFromClause(SqlFieldBuilder builder, Request request, ICollection<AbstractEntity> nonPersistentEntities)
		{
			var tables = new Dictionary<string, SqlField> ();
			var joins = new List<Tuple<string, SqlField, string, SqlField, bool>> ();

			this.BuildTablesAndJoins (builder, request, nonPersistentEntities, tables, joins);

			return this.BuildFromClause (tables, joins);
		}


		private void BuildTablesAndJoins(SqlFieldBuilder builder, Request request, ICollection<AbstractEntity> nonPersistentEntities, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var targetsWithsources = this.GetTargetsWithSources (nonPersistentEntities);
			var mandatoryEntities = this.GetMandatoryEntities (request, targetsWithsources, nonPersistentEntities);

			this.BuildTablesAndJoinsForEntities (builder, nonPersistentEntities, tables, joins);
			this.BuildTablesAndJoinsForRelations (builder, mandatoryEntities, targetsWithsources, tables, joins);
		}


		private Dictionary<AbstractEntity, HashSet<Tuple<AbstractEntity, Druid>>> GetTargetsWithSources(IEnumerable<AbstractEntity> entities)
		{
			var targetsWithsources = new Dictionary<AbstractEntity, HashSet<Tuple<AbstractEntity, Druid>>> ();

			foreach (var source in entities)
			{
				var fieldsWithChildren = EntityHelper.GetFieldsWithChildren (this.TypeEngine, source);

				foreach (var fieldWithTarget in fieldsWithChildren)
				{
					var fieldId = fieldWithTarget.Item1;
					var target = fieldWithTarget.Item2;
					
					HashSet<Tuple<AbstractEntity, Druid>> parents;

					if (!targetsWithsources.TryGetValue (target, out parents))
					{
						parents = new HashSet<Tuple<AbstractEntity, Druid>> ();

						targetsWithsources[target] = parents;
					}

					var element = Tuple.Create (source, fieldId);

					parents.Add (element);
				}
			}

			return targetsWithsources;
		}


		private HashSet<AbstractEntity> GetMandatoryEntities(Request request, Dictionary<AbstractEntity, HashSet<Tuple<AbstractEntity, Druid>>> targetsWithsources, IEnumerable<AbstractEntity> nonPersistentEntities)
		{
			var todo = new HashSet<AbstractEntity> (nonPersistentEntities);
			var mandatoryEntities = new HashSet<AbstractEntity> ();

			var rootEntity = request.RootEntity;

			todo.Remove (rootEntity);
			mandatoryEntities.Add (rootEntity);

			var requestedEntity = request.RequestedEntity;

			if (requestedEntity != rootEntity)
			{
				todo.Remove (requestedEntity);
				mandatoryEntities.Add (requestedEntity);
			}

			var entitiesWithinConditions = request
				.Conditions
				.SelectMany (c => c.GetEntities ())
				.Distinct ()
				.ToList ();

			todo.ExceptWith (entitiesWithinConditions);
			mandatoryEntities.UnionWith (entitiesWithinConditions);

			var entitiesWithValueFieldsDefined = todo
				.Where (this.HasValueFieldDefined)
				.ToList ();

			todo.ExceptWith (entitiesWithValueFieldsDefined);
			mandatoryEntities.UnionWith (entitiesWithValueFieldsDefined);

			var entitiesWithRelationToPersistentTarget = todo
				.Where (this.HasRelationToPersistentTarget)
				.ToList ();

			todo.ExceptWith (entitiesWithRelationToPersistentTarget);
			mandatoryEntities.UnionWith (entitiesWithRelationToPersistentTarget);

			var oldMandatory = new HashSet<AbstractEntity> (mandatoryEntities);
			HashSet<AbstractEntity> newMandatory;

			do
			{
				newMandatory = new HashSet<AbstractEntity> ();

				foreach (var entity in oldMandatory)
				{
					HashSet<Tuple<AbstractEntity, Druid>> sources;

					if (targetsWithsources.TryGetValue (entity, out sources))
					{
						foreach (var sourceData in sources)
						{
							var source = sourceData.Item1;

							if (!mandatoryEntities.Contains (source))
							{
								newMandatory.Add (source);
							}
						}
					}
				}

				todo.UnionWith (newMandatory);
				mandatoryEntities.AddRange (newMandatory);

				oldMandatory = newMandatory;
			}
			while (newMandatory.Count > 0);

			return mandatoryEntities;
		}


		private bool HasValueFieldDefined(AbstractEntity entity)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			return this.TypeEngine
				.GetValueFields (leafEntityTypeId)
				.Any (f => entity.IsFieldDefined (f.Id));
		}


		private bool HasRelationToPersistentTarget(AbstractEntity entity)
		{
			return EntityHelper.GetFieldsWithChildren (this.TypeEngine, entity)
				.Any (e => this.dataContext.IsPersistent (e.Item2));
		}


		private void BuildTablesAndJoinsForEntities(SqlFieldBuilder builder, IEnumerable<AbstractEntity> entities, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			foreach(var entity in entities)
			{
				this.BuildTablesAndJoinsForEntity (builder, entity, tables, joins);
			}
		}


		private void BuildTablesAndJoinsForEntity(SqlFieldBuilder builder, AbstractEntity entity, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var rootEntityTypeId = this.TypeEngine.GetRootType (leafEntityTypeId).CaptionId;

			var rootTableAlias = builder.AliasManager.GetAlias (entity, rootEntityTypeId);
			var rootColumnId = builder.BuildRootId (entity);

			var entityTypes = this.TypeEngine.GetBaseTypes (leafEntityTypeId);

			foreach (var entityType in entityTypes)
			{
				var localEntityTypeId = entityType.CaptionId;

				var localTableAlias = builder.AliasManager.GetAlias (entity, localEntityTypeId);
				var localTable = builder.BuildEntityTable (entity, localEntityTypeId);
				
				tables[localTableAlias] = localTable;

				if (localEntityTypeId != rootEntityTypeId)
				{
					var localColumnId = builder.BuildEntityId (entity, localEntityTypeId);

					var join = Tuple.Create (rootTableAlias, rootColumnId, localTableAlias, localColumnId, true);
					joins.Add (join);
				}
			}
		}


		private void BuildTablesAndJoinsForRelations(SqlFieldBuilder builder, HashSet<AbstractEntity> mandatoryEntities, Dictionary<AbstractEntity, HashSet<Tuple<AbstractEntity, Druid>>> targetsWithsources, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			foreach(var targetWithsourcesItem in targetsWithsources)
			{
				var target = targetWithsourcesItem.Key;

				foreach(var sourceItem in targetWithsourcesItem.Value)
				{
					var source = sourceItem.Item1;
					var fieldId = sourceItem.Item2;

					this.BuildTablesAndJoinsForRelations (builder, mandatoryEntities, source, fieldId, target, tables, joins);
				}
			}
		}


		private void BuildTablesAndJoinsForRelations(SqlFieldBuilder builder, HashSet<AbstractEntity> mandatoryEntities, AbstractEntity source, Druid fieldId, AbstractEntity target, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var leafEntityTypeId = source.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var isMandatory = mandatoryEntities.Contains (target);

			switch (field.Relation)
			{
				case FieldRelation.Reference:
					this.BuildTablesAndJoinsForReference (builder, source, fieldId, target, isMandatory, joins);
					break;

				case FieldRelation.Collection:
					this.BuildTablesAndJoinsForCollection (builder, source, fieldId, target, isMandatory, tables, joins);
					break;

				default:
					throw new InvalidOperationException();
			}
		}


		private void BuildTablesAndJoinsForReference(SqlFieldBuilder builder, AbstractEntity source, Druid fieldId, AbstractEntity target, bool isMandatory, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			if (!this.dataContext.IsPersistent (target))
			{
				var leafSourceTypeId = source.GetEntityStructuredTypeId ();
				var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

				var sourceTableAlias = builder.AliasManager.GetAlias (source, localSourceTypeId);
				var sourceColumn = builder.BuildEntityField (source, fieldId);

				var leafTargetTypeId = target.GetEntityStructuredTypeId ();
				var rootTargetTypeId = this.TypeEngine.GetRootType (leafTargetTypeId).CaptionId;

				var targetTableAlias = builder.AliasManager.GetAlias (target, rootTargetTypeId);
				var targetColumn = builder.BuildRootId (target);

				var join = Tuple.Create (sourceTableAlias, sourceColumn, targetTableAlias, targetColumn, isMandatory);

				joins.Add (join);
			}
		}


		private void BuildTablesAndJoinsForCollection(SqlFieldBuilder builder, AbstractEntity source, Druid fieldId, AbstractEntity target, bool isMandatory, Dictionary<string, SqlField> tables, List<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var leafSourceTypeId = source.GetEntityStructuredTypeId ();
			var rootSourceTypeId = this.TypeEngine.GetRootType (leafSourceTypeId).CaptionId;
			var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

			var relationTableAlias = builder.AliasManager.GetAlias (source, fieldId, target);

			var relationTable = builder.BuildRelationTable (source, fieldId, target);
			tables[relationTableAlias] = relationTable;

			var sourceTableAlias = builder.AliasManager.GetAlias (source, rootSourceTypeId);
			var sourceColumnId = builder.BuildRootId (source);

			var relationColumnSourceId = builder.BuildRelationSourceId (relationTableAlias, localSourceTypeId, fieldId);

			var joinToRelation = Tuple.Create (sourceTableAlias, sourceColumnId, relationTableAlias, relationColumnSourceId, isMandatory);
			joins.Add (joinToRelation);
		
			if (!this.dataContext.IsPersistent (target))
			{
				var leafTargetTypeId = target.GetEntityStructuredTypeId ();
				var rootTargetTypeId = this.TypeEngine.GetRootType (leafTargetTypeId).CaptionId;

				var relationColumnTargetId = builder.BuildRelationTargetId (relationTableAlias, localSourceTypeId, fieldId);

				var targetTableAlias = builder.AliasManager.GetAlias (target, rootTargetTypeId);
				var targetColumn = builder.BuildRootId (target);

				var joinToTarget = Tuple.Create (relationTableAlias, relationColumnTargetId, targetTableAlias, targetColumn, true);
				joins.Add (joinToTarget);
			}
		}


		private SqlContainer BuildFromClause(Dictionary<string, SqlField> tables, IEnumerable<Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var aliasesToJoins = joins
				.GroupBy (j => j.Item3)
				.ToDictionary
				(
					g => g.Key,
					g => g.ToList ()
				);

			var dependencies = aliasesToJoins
				.ToDictionary
				(
					x => x.Key,
					x => (ISet<string>) new HashSet<string> (x.Value.Select (j => j.Item1))
				);

			foreach (var tableAlias in tables.Keys)
			{
				if (!dependencies.ContainsKey (tableAlias))
				{
					dependencies[tableAlias] = new HashSet<string> ();
				}
			}

			var ordering = TopologicalSort.GetOrdering (dependencies);

			var mainTableAlias = ordering[0];
			var sqlMainTable = tables[mainTableAlias];

			var sqlJoins = new List<SqlJoin> ();

			foreach(var secondaryTableAlias in ordering.Skip (1))
			{
				var joinsWithTable = aliasesToJoins[secondaryTableAlias];

				var sqlJoinCode = joinsWithTable.Any (j => j.Item5)
				    ? SqlJoinCode.Inner
				    : SqlJoinCode.OuterLeft;

				var sqlSecondaryTable = tables[secondaryTableAlias];

				SqlFunction sqlConditions = null;

				foreach(var join in joinsWithTable)
				{
					var sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						join.Item2,
						join.Item4
					);

					if (sqlConditions == null)
					{
						sqlConditions = sqlCondition;
					}
					else
					{
						sqlConditions = new SqlFunction
						(
							SqlFunctionCode.LogicAnd,
							SqlField.CreateFunction (sqlConditions),
							SqlField.CreateFunction (sqlCondition)
						);
					}
				}

				var sqlJoin = new SqlJoin (sqlJoinCode, sqlSecondaryTable, sqlConditions);

				sqlJoins.Add (sqlJoin);
			}

			return SqlContainer.CreateSqlTables (sqlMainTable).PlusSqlJoins (sqlJoins.ToArray ());
		}


		private SqlContainer BuildWhereClause(SqlFieldBuilder builder, Request request, IEnumerable<AbstractEntity> nonPersistentEntities)
		{
			var conditions = this.BuildConditions (builder, nonPersistentEntities);
			var constraints = this.BuildConstraints (builder, request);

			return conditions.PlusSqlConditions (constraints.ToArray ());
		}


		private List<SqlFunction> BuildConstraints(SqlFieldBuilder builder, Request request)
		{
			return request.Conditions
				.Select (c => c.CreateSqlCondition (builder))
				.ToList ();
		}


		private SqlContainer BuildConditions(SqlFieldBuilder builder, IEnumerable<AbstractEntity> entities)
		{
			var containers = from entity in entities
			                 let leafEntityTypeId = entity.GetEntityStructuredTypeId ()
			                 from field in this.TypeEngine.GetFields (leafEntityTypeId)
			                 where entity.IsFieldDefined (field.Id)
			                 select this.BuildCondition (builder, entity, field);

			return containers.Aggregate (SqlContainer.Empty, (acc, c) => acc.Plus (c));
		}


		private SqlContainer BuildCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			switch(field.Relation)
			{
				case FieldRelation.None:
					return this.BuildValueCondition (builder, entity, field);

				case FieldRelation.Reference:
					return this.BuildReferenceCondition (builder, entity, field);

				case FieldRelation.Collection:
					return this.BuildCollectionCondition (builder, entity, field);

				default:
					throw new InvalidOperationException ();
			}
		}


		private SqlContainer BuildValueCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			
			var sqlFieldColumn = builder.BuildEntityField (entity, fieldId);
			var sqlFieldValue = builder.BuildConstantForField (entity, fieldId);

			var sqlFunctionCode = sqlFieldValue.RawType == DbRawType.String
			    ? SqlFunctionCode.CompareLike
			    : SqlFunctionCode.CompareEqual;

			var sqlFunction = new SqlFunction (sqlFunctionCode, sqlFieldColumn, sqlFieldValue);

			return SqlContainer.CreateSqlConditions (sqlFunction);
		}


		private SqlContainer BuildReferenceCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			var fieldName = fieldId.ToResourceId ();

			var target = source.GetField<AbstractEntity> (fieldName);

			if (dataContext.IsPersistent (target))
			{
				var sourceFieldColumn = builder.BuildEntityField (source, fieldId);
				var targetIdValue = builder.BuildConstantForKey (target);

				var sqlFunction = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					sourceFieldColumn,
					targetIdValue
				);

				return SqlContainer.CreateSqlConditions (sqlFunction);
			}
			else
			{
				return SqlContainer.Empty;
			}
		}


		private SqlContainer BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			return entity
				   .GetFieldCollection<AbstractEntity> (field.Id)
				   .Select (t => this.BuildCollectionCondition (builder, entity, field, t))
				   .Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			if (dataContext.IsPersistent (target))
			{
				var leafSourceTypeId = source.GetEntityStructuredTypeId ();
				var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, field.CaptionId).CaptionId;

				var fieldId = field.CaptionId;

				var relationTableAlias = builder.AliasManager.GetAlias (source, fieldId, target);
				var relationColumnTargetId = builder.BuildRelationTargetId (relationTableAlias, localSourceTypeId, fieldId);

				var targetIdValue = builder.BuildConstantForKey (target);
				
				var sqlFunction = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					relationColumnTargetId,
					targetIdValue
				);

				return SqlContainer.CreateSqlConditions (sqlFunction);
			}
			else
			{
				return SqlContainer.Empty;
			}
		}


		private SqlContainer BuildOrderByClause(SqlFieldBuilder builder, Request request)
		{
			var sqlFields = from sortClause in request.SortClauses
							select sortClause.CreateSqlField (builder);

			return SqlContainer.CreateSqlOrderBys (sqlFields.ToArray ());
		}


		private SqlContainer BuildSqlContainerForValuesAndReferences(SqlFieldBuilder builder, AbstractEntity entity)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var fields = new List<SqlField> ();

			fields.AddRange
			(
				from field in this.TypeEngine.GetValueFields (leafEntityTypeId)
				where field.Type.SystemType != typeof (byte[])
				let fieldId = field.CaptionId
				let fieldName = fieldId.ToResourceId ()
				orderby fieldName
				select builder.BuildEntityField (entity, fieldId)
			);

			fields.AddRange
			(
				from field in this.TypeEngine.GetReferenceFields (leafEntityTypeId)
				let fieldId = field.CaptionId
				let fieldName = fieldId.ToResourceId ()
				orderby fieldName
				select builder.BuildEntityField (entity, fieldId)
			);

			var logId = builder.BuildRootLogId (entity);
			fields.Add (logId);

			var typeId = builder.BuildRootTypeId (entity);
			fields.Add (typeId);

			var entityId = builder.BuildRootId (entity);
			fields.Add (entityId);
			
			return SqlContainer.CreateSqlFields (fields.ToArray ());
		}


		private SqlContainer BuildSqlContainerForCollectionSourceIds(SqlFieldBuilder builder, AbstractEntity entity)
		{
			var sqlField = builder.BuildRootId (entity);
			return SqlContainer.CreateSqlFields (sqlField);
		}


		private SqlContainer BuildSqlContainerForCollection(SqlFieldBuilder builder, AbstractEntity entity, Druid fieldId, SqlSelect sqlSelectForSourceIds)
		{
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			var tableAlias = builder.AliasManager.GetAlias ();
			var dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			var table = builder.BuildTable (tableAlias, dbTable);

			var sqlFieldForTargetId = builder.BuildRelationTargetId (tableAlias, localEntityId, fieldId);
			var sqlFieldForSourceId = builder.BuildRelationSourceId (tableAlias, localEntityId, fieldId);

			var sqlFieldForRank = builder.BuildRelationRank (tableAlias, localEntityId, fieldId);
			sqlFieldForRank.SortOrder = SqlSortOrder.Ascending;

			var sqlFunctionForCondition = new SqlFunction
			(
				SqlFunctionCode.SetIn,
				builder.BuildRelationSourceId (tableAlias, localEntityId, fieldId),
				SqlField.CreateSubQuery (sqlSelectForSourceIds)
			);

			return SqlContainer.CreateSqlTables (table)
				.PlusSqlFields (sqlFieldForTargetId, sqlFieldForSourceId)
				.PlusSqlConditions (sqlFunctionForCondition)
				.PlusSqlOrderBys (sqlFieldForRank);
		}


		private SqlContainer BuildSqlContainerForCount(SqlFieldBuilder builder, AbstractEntity entity, SqlSelectPredicate predicate)
		{
			var sqlField = builder.BuildRootId (entity);
			var sqlAggregateField = SqlField.CreateAggregate (SqlAggregateFunction.Count, predicate, sqlField);

			return SqlContainer.CreateSqlFields (sqlAggregateField);
		}


		private SqlContainer BuildSqlContainerForEntityKeys(SqlFieldBuilder builder, AbstractEntity entity)
		{
			var sqlFieldForEntityId = builder.BuildRootId (entity);

			return SqlContainer.CreateSqlFields (sqlFieldForEntityId);
		}


		private SqlSelectPredicate GetSqlSelectPredicate(Request request)
		{
			return this.UseDistinct (request)
				? SqlSelectPredicate.Distinct
				: SqlSelectPredicate.All;
		}


		private bool UseDistinct(Request request)
		{
			// The only queries that must contain a DISTINCT clause are the queries where a
			// collection is involved. If a collection is involved in a WHERE or a ORDER BY clause,
			// there might be duplicate rows in the result if the collection contains more than one
			// element. Therefore, we need to add a DISTINCT clause in the query.

			var todo = new Stack<AbstractEntity> ();
			var done = new HashSet<AbstractEntity> ();

			todo.Push (request.RootEntity);
			done.Add (request.RootEntity);

			while (todo.Count > 0)
			{
				var entity = todo.Pop ();

				if (this.HasCollectionFieldDefined (entity))
				{
					return true;
				}

				foreach (var child in EntityHelper.GetChildren (this.TypeEngine, entity))
				{
					if (!done.Contains (child))
					{
						todo.Push (child);
						done.Add (child);
					}
				}
			}

			return false;
		}


		private bool HasCollectionFieldDefined(AbstractEntity entity)
		{
			var leafEntityId = entity.GetEntityStructuredTypeId ();

			return this.TypeEngine
				.GetCollectionFields (leafEntityId)
				.Any (f => entity.IsFieldNotEmpty (f.Id));
		}


		private SqlFieldBuilder GetBuilder()
		{
			return new SqlFieldBuilder (this.dataContext);
		}


		private readonly DataContext dataContext;


	}


}
