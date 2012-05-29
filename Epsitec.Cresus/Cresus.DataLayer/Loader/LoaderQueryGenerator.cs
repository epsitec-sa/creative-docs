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
			var sqlSelect = this.BuildSelectForCount (request);

			using (var dbTransaction = this.StartTransaction ())
			{
				var count = this.GetCount (sqlSelect, dbTransaction);

				dbTransaction.Commit ();

				return count;
			}
		}


		public int GetCount(Request request, DbTransaction dbTransaction)
		{
			var sqlSelect = this.BuildSelectForCount (request);

			return this.GetCount (sqlSelect, dbTransaction);
		}


		public int GetCount(SqlSelect sqlSelect, DbTransaction dbTransaction)
		{
			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			return (int) this.DbInfrastructure.ExecuteScalar (dbTransaction);
		}


		private SqlSelect BuildSelectForCount(Request request)
		{
			var builder = this.GetBuilder ();
			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);

			var aggregate = SqlAggregateFunction.Count;
			var predicate = this.GetSqlSelectPredicate (request);
			var entityId = builder.BuildRootId (request.RequestedEntity);
			
			var fieldForCount = SqlField.CreateAggregate (aggregate, predicate, entityId);

			return fromWhereOrderBy
				.PlusSqlFields (fieldForCount)
				.BuildSqlSelect (skip: request.Skip, take: request.Take);
		}


		public IEnumerable<EntityKey> GetEntityKeys(Request request, DbTransaction dbTransaction)
		{
			var sqlSelect = this.BuildSelectForEntityKeys (request);

			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var data = this.DbInfrastructure.ExecuteRetData (dbTransaction);

			var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();
			var rootEntityTypeId = this.TypeEngine.GetRootType (leafEntityTypeId).CaptionId;

			return from DataRow row in data.Tables[0].Rows
			       let rowKey = this.ExtractKey (row[0])
			       select new EntityKey (rootEntityTypeId, rowKey);
		}


		private SqlSelect BuildSelectForEntityKeys(Request request)
		{
			var builder = this.GetBuilder ();
			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);

			var entityId = builder.BuildRootId (request.RequestedEntity);
			var predicate = this.GetSqlSelectPredicate (request);

			return fromWhereOrderBy
				.PlusSqlFields (entityId)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}
		
		
		public int? GetIndex(Request request, EntityKey entityKey, DbTransaction dbTransaction)
		{
			// HACK This method is a big hack that I implemented only because Pierre wanted something
			// quickly. It is totally inefficient and I plan to implement a better way to do this
			// which will be more performant.

			var sqlSelect = this.BuildSelectForEntityKeys (request);

			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var data = this.DbInfrastructure.ExecuteRetData (dbTransaction);

			var expectedId = entityKey.RowKey.Id.Value;
			var i = 0;
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

			using (var dbTransaction = this.StartTransaction ())
			{
				valuesAndReferencesData = this.GetValueAndReferenceData (dbTransaction, request);

				collectionsData = valuesAndReferencesData.Count > 0
					? this.GetCollectionData (dbTransaction, request)
					: new Dictionary<DbKey, CollectionData> ();

				dbTransaction.Commit ();
			}

			return this.GetEntitiesData (request, valuesAndReferencesData, collectionsData);
		}


		private List<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> GetValueAndReferenceData(DbTransaction transaction, Request request)
		{
			var sqlSelect = this.BuildSelectForValueAndReferenceData (request);

			transaction.SqlBuilder.SelectData (sqlSelect);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var valueFields = new List<Tuple<StructuredTypeField, DbColumn>>
			(
				from field in this.GetValueFields (leafEntityTypeId)
				let fieldId = field.CaptionId
				let localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId
				let dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityTypeId, fieldId)
				select Tuple.Create (field, dbColumn)
			);

			var referenceFields = this.GetReferenceFields (leafEntityTypeId).ToList ();
			
			return data.Tables[0].Rows
				.Cast<DataRow> ()
				.Select (r => this.ProcessValueAndReferenceRow (valueFields, referenceFields, r))
				.ToList ();
		}


		private Tuple<DbKey, Druid, long, ValueData, ReferenceData> ProcessValueAndReferenceRow(List<Tuple<StructuredTypeField, DbColumn>> valueFields, List<StructuredTypeField> referenceFields, DataRow row)
		{
			var entityValueData = new ValueData ();
			var entityReferenceData = new ReferenceData ();

			for (int i = 0; i < valueFields.Count; i++)
			{
				var databaseValue = row[i];

				if (databaseValue != DBNull.Value)
				{
					var field = valueFields[i].Item1;
					
					var dbColumn = valueFields[i].Item2;
					var type = field.Type;

					var cresusValue = this.ExtractValue (type, dbColumn, databaseValue);				
					var fieldId = field.CaptionId;

					entityValueData[fieldId] = cresusValue;
				}
			}

			for (int i = 0; i < referenceFields.Count; i++)
			{
				var value = row[valueFields.Count + i];

				if (value != DBNull.Value)
				{
					var key = this.ExtractKey (value);
					var fieldId = referenceFields[i].CaptionId;
					
					entityReferenceData[fieldId] = key;
				}
			}

			var rowLength = row.ItemArray.Length;

			var logId = this.ExtractLong (row[rowLength - 3]);
			var entityTypeId = this.ExtractDruid (row[rowLength - 2]);
			var rowKey = this.ExtractKey (row[rowLength - 1]);
			
			return Tuple.Create (rowKey, entityTypeId, logId, entityValueData, entityReferenceData);
		}


		private SqlSelect BuildSelectForValueAndReferenceData(Request request)
		{
			var builder = this.GetBuilder ();
			
			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);
			var select = this.BuildSelectForValuesAndReferences (builder, request);
			var predicate = this.GetSqlSelectPredicate (request);

			return fromWhereOrderBy
				.PlusSqlFields (select.ToArray ())
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}


		private IEnumerable<SqlField> BuildSelectForValuesAndReferences(SqlFieldBuilder builder, Request request)
		{
			var entity = request.RequestedEntity;
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var valueFields = this.GetValueFields (leafEntityTypeId);
			var referenceFields = this.GetReferenceFields (leafEntityTypeId);

			foreach (var field in valueFields.Concat (referenceFields))
			{
				var fieldId = field.CaptionId;

				yield return builder.BuildEntityField (entity, fieldId);
			}

			yield return builder.BuildRootLogId (entity);
			yield return builder.BuildRootTypeId (entity);
			yield return builder.BuildRootId (entity);
		}


		private IEnumerable<StructuredTypeField> GetValueFields(Druid leafEntityTypeId)
		{
			return from field in this.TypeEngine.GetValueFields (leafEntityTypeId)
			       where field.Type.SystemType != typeof (byte[])
			       let fieldId = field.CaptionId
			       let fieldName = fieldId.ToResourceId ()
			       orderby fieldName
			       select field;
		}


		private IEnumerable<StructuredTypeField> GetReferenceFields(Druid leafEntityTypeId)
		{
			return from field in this.TypeEngine.GetReferenceFields (leafEntityTypeId)
				   let fieldId = field.CaptionId
				   let fieldName = fieldId.ToResourceId ()
				   orderby fieldName
				   select field;
		}


		private Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			var collectionData = new Dictionary<DbKey, CollectionData> ();

			var entity = request.RequestedEntity;
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var fields = this.TypeEngine.GetCollectionFields (leafEntityTypeId);

			foreach (var field in fields)
			{
				var fieldId = field.CaptionId;

				foreach (var relation in this.GetCollectionData (transaction, request, fieldId))
				{
					CollectionData data;

					var sourceKey = relation.Item1;
					var targetKey = relation.Item2;

					if (!collectionData.TryGetValue (sourceKey, out data))
					{
						data = new CollectionData ();

						collectionData[sourceKey] = data;
					}

					data[fieldId].Add (targetKey);
				}
			}

			return collectionData;
		}


		private IEnumerable<Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Request request, Druid fieldId)
		{
			var select = this.BuildSelectForCollectionData (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			return from DataRow row in data.Tables[0].Rows
			       let targetKey = this.ExtractKey (row[0])
			       let sourceKey = this.ExtractKey (row[1])
			       select Tuple.Create (sourceKey, targetKey);
		}


		private SqlSelect BuildSelectForCollectionData(Request request, Druid fieldId)
		{
			var builder = this.GetBuilder ();
			
			var innerSelect = this.BuildInnerSelectForCollectionData (request, builder);
			
			return this.BuildOuterSelectForCollectionData (builder, request, fieldId, innerSelect);
		}


		private SqlField BuildInnerSelectForCollectionData(Request request, SqlFieldBuilder builder)
		{
			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);
			var select = builder.BuildRootId (request.RequestedEntity);
			var predicate = this.GetSqlSelectPredicate (request);

			var sqlSelect = fromWhereOrderBy
				.PlusSqlFields (select)
				.BuildSqlSelect (predicate, request.Skip, request.Take);

			return SqlField.CreateSubQuery (sqlSelect);
		}


		private SqlSelect BuildOuterSelectForCollectionData(SqlFieldBuilder builder, Request request, Druid fieldId, SqlField innerSelect)
		{
			var entity = request.RequestedEntity;
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			var tableAlias = builder.AliasManager.GetAlias ();
			var dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			
			var table = builder.BuildTable (tableAlias, dbTable);

			var targetId = builder.BuildRelationTargetId (tableAlias, localEntityId, fieldId);
			var sourceId = builder.BuildRelationSourceId (tableAlias, localEntityId, fieldId);

			var condition = new SqlFunction (SqlFunctionCode.SetIn, sourceId, innerSelect);

			var rank = builder.BuildRelationRank (tableAlias, localEntityId, fieldId);
			rank.SortOrder = SqlSortOrder.Ascending;

			return SqlContainer.CreateSqlTables (table)
				.PlusSqlFields (targetId, sourceId)
				.PlusSqlConditions (condition)
				.PlusSqlOrderBys (rank)
				.BuildSqlSelect ();
		}


		private IEnumerable<EntityData> GetEntitiesData(Request request, IEnumerable<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData, Dictionary<DbKey, CollectionData> collectionsData)
		{
			return from valueAndReferenceData in valuesAndReferencesData
				let loadedTypeId = request.RequestedEntity.GetEntityStructuredTypeId ()
				let id = valueAndReferenceData.Item1
				let leafTypeId = valueAndReferenceData.Item2
				let logId = valueAndReferenceData.Item3
				let values = valueAndReferenceData.Item4
				let references = valueAndReferenceData.Item5
				let collections = collectionsData.ContainsKey (id)
					? collectionsData[id]
					: new CollectionData ()
				select new EntityData (id, leafTypeId, loadedTypeId, logId, values, references, collections);
		}
		
		
		public object GetValueField(AbstractEntity entity, Druid fieldId)
		{
			var request = this.BuildRequestForValueField (entity, fieldId);

			object value;

			using (var transaction = this.StartTransaction ())
			{
				value = this.GetValueField (transaction, request, fieldId);

				transaction.Commit ();
			}

			return value;
		}


		private Request BuildRequestForValueField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;

			var example = EntityClassFactory.CreateEmptyEntity (localEntityTypeId);
			var key = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			return Request.Create (example, key);
		}


		private object GetValueField(DbTransaction transaction, Request request, Druid fieldId)
		{
			var select = this.BuildSelectForSingleValue (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			var table = data.Tables[0];

			object value = null;

			if (table.Rows.Count == 1)
			{
				var databaseValue = table.Rows[0][0];

				if (databaseValue != DBNull.Value)
				{
					var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();

					var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);
					var type = field.Type;

					var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;
					var dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityTypeId, fieldId);

					value = this.ExtractValue (type, dbColumn, databaseValue);
				}
			}

			return value;
		}


		private SqlSelect BuildSelectForSingleValue(Request request, Druid fieldId)
		{
			var builder = this.GetBuilder ();

			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);
			var select = builder.BuildEntityField (request.RequestedEntity, fieldId);

			return fromWhereOrderBy
				.PlusSqlFields (select)
				.BuildSqlSelect ();
		}
		
		
		public EntityData GetReferenceField(AbstractEntity entity, Druid fieldId)
		{
			var request = this.GetRequestForReferenceField (entity, fieldId);

			return this.GetEntitiesData (request).FirstOrDefault ();
		}


		private Request GetRequestForReferenceField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var source = EntityClassFactory.CreateEmptyEntity (leafEntityTypeId);
			var sourceKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;
			
			var target = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			using (source.DefineOriginalValues ())
			{
				var fieldName = fieldId.ToResourceId ();

				source.SetField (fieldName, target);
			}

			return Request.Create (source, sourceKey, target);
		}
		
		
		public IEnumerable<EntityData> GetCollectionField(AbstractEntity entity, Druid fieldId)
		{
			// TODO This method could be improved by doing only one request with an ORDER BY clause
			// instead of doing one request to get the data and one request to get the order.

			Dictionary<DbKey, EntityData> targetsData;
			List<DbKey> targetKeys;

			using (var transaction = this.StartTransaction ())
			{
				targetsData = this.GetCollectionEntityData (entity, fieldId)
					.ToDictionary (data => data.RowKey);

				if (targetsData.Count > 0)
				{
					targetKeys = this.GetCollectionKeys (transaction, entity, fieldId)
						.Select (d => d.Item2)
						.ToList ();
				}
				else
				{
					targetKeys = new List<DbKey> ();
				}

				transaction.Commit ();
			}

			var result = new List<EntityData> ();

			foreach (var targetKey in targetKeys)
			{
				EntityData entityData;

				if (targetsData.TryGetValue (targetKey, out entityData))
				{
					result.Add (entityData);
				}
			}

			return result;
		}


		private IEnumerable<EntityData> GetCollectionEntityData(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var source = EntityClassFactory.CreateEmptyEntity (leafEntityTypeId);
			var sourceKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;
			
			var target = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			using (source.DefineOriginalValues ())
			{
				var fieldName = fieldId.ToResourceId ();
				var collection = source.GetFieldCollection<AbstractEntity> (fieldName);

				collection.Add (target);
			}

			var request = Request.Create (source, sourceKey, target);

			return this.GetEntitiesData (request);
		}


		private IEnumerable<Tuple<DbKey, DbKey>> GetCollectionKeys(DbTransaction transaction, AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var source = EntityClassFactory.CreateEmptyEntity (leafEntityTypeId);
			var sourceKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			var request = Request.Create (source, sourceKey);

			return this.GetCollectionData (transaction, request, fieldId);
		}
		
		
		private SqlContainer BuildFromWhereAndOrderBy(SqlFieldBuilder builder, Request request)
		{
			var nonPersistentEntities = request.GetNonPersistentEntities (this.dataContext);

			var from = this.BuildFrom (builder, request, nonPersistentEntities);
			var where = this.BuildWhere (builder, request, nonPersistentEntities);
			var orderBy = this.BuildOrderBy (builder, request);

			return from
				.PlusSqlConditions (where.ToArray ())
				.PlusSqlOrderBys (orderBy.ToArray ());
		}


		private SqlContainer BuildFrom(SqlFieldBuilder builder, Request request, ICollection<AbstractEntity> nonPersistentEntities)
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
				.Where (e => EntityHelper.HasValueFieldDefined (this.TypeEngine, e))
				.ToList ();

			todo.ExceptWith (entitiesWithValueFieldsDefined);
			mandatoryEntities.UnionWith (entitiesWithValueFieldsDefined);

			var entitiesWithRelationToPersistentTarget = todo
				.Where (e => EntityHelper.HasRelationToPersistentTarget (this.TypeEngine, this.dataContext, e))
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
				mandatoryEntities.UnionWith (newMandatory);

				oldMandatory = newMandatory;
			}
			while (newMandatory.Count > 0);

			return mandatoryEntities;
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


		private IEnumerable<SqlFunction> BuildWhere(SqlFieldBuilder builder, Request request, IEnumerable<AbstractEntity> nonPersistentEntities)
		{
			var conditions = this.BuildConditions (builder, nonPersistentEntities);
			var constraints = this.BuildConstraints (builder, request);

			return conditions.Concat (constraints);
		}


		private IEnumerable<SqlFunction> BuildConstraints(SqlFieldBuilder builder, Request request)
		{
			return from condition in request.Conditions
			       select condition.CreateSqlCondition (builder);
		}


		private IEnumerable<SqlFunction> BuildConditions(SqlFieldBuilder builder, IEnumerable<AbstractEntity> entities)
		{
			var conditions = from entity in entities
			                 let leafEntityTypeId = entity.GetEntityStructuredTypeId ()
			                 from field in this.TypeEngine.GetFields (leafEntityTypeId)
			                 where entity.IsFieldDefined (field.Id)
			                 select this.BuildCondition (builder, entity, field);

			return conditions.SelectMany (c => c);
		}


		private IEnumerable<SqlFunction> BuildCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var conditions = new List<SqlFunction> ();

			switch(field.Relation)
			{
				case FieldRelation.None:
					conditions.Add (this.BuildValueCondition (builder, entity, field));
					break;

				case FieldRelation.Reference:

					var condition = this.BuildReferenceCondition (builder, entity, field);

					if (condition != null)
					{
						conditions.Add (condition);
					}
					break;

				case FieldRelation.Collection:
					conditions.AddRange (this.BuildCollectionCondition (builder, entity, field));
					break;

				default:
					throw new InvalidOperationException ();
			}

			return conditions;
		}


		private SqlFunction BuildValueCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			
			var sqlFieldColumn = builder.BuildEntityField (entity, fieldId);
			var sqlFieldValue = builder.BuildConstantForField (entity, fieldId);

			var sqlFunctionCode = sqlFieldValue.RawType == DbRawType.String
			    ? SqlFunctionCode.CompareLike
			    : SqlFunctionCode.CompareEqual;

			return new SqlFunction (sqlFunctionCode, sqlFieldColumn, sqlFieldValue);
		}


		private SqlFunction BuildReferenceCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			var fieldName = fieldId.ToResourceId ();

			var target = source.GetField<AbstractEntity> (fieldName);

			if (!dataContext.IsPersistent (target))
			{
				return null;
			}
			else
			{
				var sourceFieldColumn = builder.BuildEntityField (source, fieldId);
				var targetIdValue = builder.BuildConstantForKey (target);

				var sqlFunctionCode = SqlFunctionCode.CompareEqual;

				return new SqlFunction (sqlFunctionCode, sourceFieldColumn, targetIdValue);
			}
		}


		private IEnumerable<SqlFunction> BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			var fieldName = fieldId.ToResourceId ();

			return from target in entity.GetFieldCollection<AbstractEntity> (fieldName)
			       where dataContext.IsPersistent (target)
			       select this.BuildCollectionCondition (builder, entity, field, target);
		}


		private SqlFunction BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			var fieldId = field.CaptionId;

			var leafSourceTypeId = source.GetEntityStructuredTypeId ();
			var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

			var relationTableAlias = builder.AliasManager.GetAlias (source, fieldId, target);
			var relationColumnTargetId = builder.BuildRelationTargetId (relationTableAlias, localSourceTypeId, fieldId);

			var targetIdValue = builder.BuildConstantForKey (target);

			var sqlFunctionCode = SqlFunctionCode.CompareEqual;

			return new SqlFunction (sqlFunctionCode, relationColumnTargetId, targetIdValue);
		}


		private IEnumerable<SqlField> BuildOrderBy(SqlFieldBuilder builder, Request request)
		{
			return from sortClause in request.SortClauses
			       select sortClause.CreateSqlField (builder);
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

				if (EntityHelper.HasCollectionFieldDefined (this.TypeEngine, entity))
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


		private object ExtractValue(INamedType type, DbColumn dbColumn, object value)
		{
			var dbTypeDef = dbColumn.Type;
			var dbRawType = dbTypeDef.RawType;
			var dbSimpleType = dbTypeDef.SimpleType;
			var dbNumDef = dbTypeDef.NumDef;

			return this.DataConverter.FromDatabaseToCresusValue (type, dbRawType, dbSimpleType, dbNumDef, value);
		}


		private DbKey ExtractKey(object value)
		{
			return new DbKey (new DbId ((long) value));
		}


		private Druid ExtractDruid(object value)
		{
			return Druid.FromLong ((long) value);
		}


		private long ExtractLong(object value)
		{
			return (long) value;
		}


		private SqlFieldBuilder GetBuilder()
		{
			return new SqlFieldBuilder (this.dataContext);
		}


		private DbTransaction StartTransaction()
		{
			var mode = DbTransactionMode.ReadOnly;

			return this.DbInfrastructure.InheritOrBeginTransaction (mode);
		}


		private readonly DataContext dataContext;


	}


}
