//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{
	// TODO Comment this class
	// Marc

	internal sealed class LoaderQueryGenerator
	{
		public LoaderQueryGenerator(DataContext dataContext)
		{
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}

		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DataInfrastructure.DbInfrastructure;
			}
		}

		private EntityTypeEngine TypeEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}

		private EntitySchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.EntityEngine.EntitySchemaEngine;
			}
		}

		private DataConverter DataConverter
		{
			get
			{
				return this.DataContext.DataConverter;
			}
		}


		#region REQUEST ENTRY POINT


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


		private IEnumerable<EntityData> GetEntitiesData(Request request, List<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData, Dictionary<DbKey, CollectionData> collectionsData)
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


		#endregion

		#region GET VALUE FIELD


		public object GetValueField(AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			AbstractEntity example = EntityClassFactory.CreateEmptyEntity (localEntityId);
			DbKey exampleKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			Request request = Request.Create (example, exampleKey);

			object value;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				value = this.GetSingleValue (transaction, request, fieldId);

				transaction.Commit ();
			}

			return value;
		}


		#endregion

		#region GET REFERENCE FIELD


		public EntityData GetReferenceField(AbstractEntity entity, Druid fieldId)
		{
			string fieldName = fieldId.ToResourceId ();

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			AbstractEntity targetExample = EntityClassFactory.CreateEmptyEntity (field.TypeId);
			DbKey rootExampleKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;
			
			using (rootExample.DefineOriginalValues ())
			{
				rootExample.SetField<AbstractEntity> (fieldName, targetExample);
			}

			Request request = Request.Create(rootExample, rootExampleKey, targetExample);

			List<EntityData> data = this.GetEntitiesData (request).ToList ();

			if (data.Count > 1)
			{
				throw new System.Exception ("More than one target in reference field.");
			}

			return data.FirstOrDefault ();
		}


		#endregion

		#region GET COLLECTION FIELD


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
			DbKey rootExampleKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			using (rootExample.DefineOriginalValues ())
			{
				rootExample.GetFieldCollection<AbstractEntity> (fieldName).Add (targetExample);
			}

			Request request = Request.Create(rootExample, rootExampleKey, targetExample);

			return this.GetEntitiesData (request);
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionKeys(DbTransaction transaction, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			DbKey rootExampleKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			Request request = Request.Create (rootExample, rootExampleKey);

			return this.GetCollectionData (transaction, request, fieldId);
		}


		#endregion

		#region GET SINGLE VALUE


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
			var aliasManager = this.CreateAliasManager ();
			var sqlContainerForConditions = this.BuildSqlContainerForConditions (request, aliasManager);

			AbstractEntity requestedEntity = request.RequestedEntity;
			
			SqlField sqlFieldForSingleValue = this.BuildSqlFieldForValueField(aliasManager, requestedEntity, fieldId);

			return sqlContainerForConditions.PlusSqlFields (sqlFieldForSingleValue).BuildSqlSelect ();
		}
		
		
		#endregion


		#region GET COUNT


		private SqlSelect CreateSqlSelectForCount(Request request)
		{
			var aliasManager = this.CreateAliasManager ();
			var sqlContainerForConditions = this.BuildSqlContainerForConditions (request, aliasManager);

			AbstractEntity requestedEntity = request.RequestedEntity;
			
			SqlSelectPredicate predicate = this.GetSqlSelectPredicate (request);
			SqlContainer sqlContainerForCount = this.BuildSqlContainerForCount (aliasManager, requestedEntity, predicate);

			return sqlContainerForConditions
				.Plus (sqlContainerForCount)
				.BuildSqlSelect (skip: request.Skip, take: request.Take);
		}


		#endregion


		#region GET ENTITY KEYS


		private SqlSelect CreateSqlSelectForEntityKeys(Request request)
		{
			var aliasManager = this.CreateAliasManager ();
			var sqlContainerForConditions = this.BuildSqlContainerForConditions (request, aliasManager);

			AbstractEntity requestedEntity = request.RequestedEntity;
			
			var sqlContainerForOrderBy = this.BuildSqlContainerForOrderBy (request, aliasManager);
			var sqlContainerForEntityKeys = this.BuildSqlContainerForEntityKeys (aliasManager, requestedEntity);

			var predicate = this.GetSqlSelectPredicate (request);

			return sqlContainerForConditions
				.Plus (sqlContainerForOrderBy)
				.Plus (sqlContainerForEntityKeys)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}


		#endregion


		#region GET VALUE AND REFERENCE DATA


		private IEnumerable<Tuple<DbKey, Druid, long, ValueData, ReferenceData>> GetValueAndReferenceData(DbTransaction transaction, Request request)
		{
			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var valueFields = this.TypeEngine.GetValueFields(leafEntityId)
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

				yield return Tuple.Create(entityKey, realEntityId, logId, entityValueData, entityReferenceData);
			}
		}


		private SqlSelect CreateSqlSelectForValueAndReferenceData(Request request)
		{
			var aliasManager = this.CreateAliasManager ();
			var sqlContainerForConditions = this.BuildSqlContainerForConditions (request, aliasManager);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlContainer sqlContainerForValuesAndReferences = this.BuildSqlContainerForValuesAndReferences (aliasManager, requestedEntity);

			SqlContainer sqlContainerForOrderBy = this.BuildSqlContainerForOrderBy (request, aliasManager);

			var predicate = this.GetSqlSelectPredicate (request);

			return sqlContainerForConditions
				.Plus (sqlContainerForValuesAndReferences)
				.Plus (sqlContainerForOrderBy)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}


		#endregion

		#region GET COLLECTION DATA


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
			var aliasManager = this.CreateAliasManager ();
			var sqlContainerForConditions = this.BuildSqlContainerForConditions (request, aliasManager);

			AbstractEntity requestedEntity = request.RequestedEntity;

			SqlContainer sqlContaierForOrderBy = this.BuildSqlContainerForOrderBy (request, aliasManager);
			SqlContainer sqlContainerForCollectionSourceIds = this.BuildSqlContainerForCollectionSourceIds (aliasManager, requestedEntity);

			var predicate = this.GetSqlSelectPredicate (request);

			SqlSelect sqlSelectForSourceIds = sqlContainerForConditions
				.Plus (sqlContaierForOrderBy)
				.Plus (sqlContainerForCollectionSourceIds)
				.BuildSqlSelect (predicate, request.Skip, request.Take);

			SqlContainer sqlContainerForCollectionData = this.BuildSqlContainerForCollection (aliasManager, requestedEntity, fieldId, sqlSelectForSourceIds);
			
			return sqlContainerForCollectionData.BuildSqlSelect();
		}


		#endregion


		#region CONDITION GENERATION


		private SqlContainer BuildSqlContainerForConditions(Request request, AliasManager aliasManager)
		{
			AbstractEntity entity = request.RootEntity;

			SqlField sqlTableForRequestRootEntity = this.BuildTableForRootEntity (aliasManager, entity);
			SqlContainer sqlContainerForEntity = this.BuildSqlContainerForEntity (request, aliasManager, entity);
			SqlContainer sqlContainerForConstraints = this.BuildSqlContainerForConstraints (request, aliasManager);

			return SqlContainer
				.CreateSqlTables (sqlTableForRequestRootEntity)
				.Plus (sqlContainerForEntity)
				.Plus (sqlContainerForConstraints);
		}


		private SqlField BuildTableForRootEntity(AliasManager aliasManager, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			DbTable rootDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);

			string alias = aliasManager.GetAlias (entity, rootEntityId);

			return SqlField.CreateAliasedName (rootDbTable.GetSqlName (), alias);
		}


		private SqlContainer BuildSqlContainerForEntity(Request request, AliasManager aliasManager, AbstractEntity entity)
		{
			SqlContainer sqlContainerForSubEntities = this.BuildSqlContainerForSubEntities (aliasManager, entity);
			SqlContainer sqlContainerForFields = this.BuildSqlContainerForFields (request, aliasManager, entity);

			return sqlContainerForSubEntities.Plus (sqlContainerForFields);
		}


		private SqlContainer BuildSqlContainerForSubEntities(AliasManager aliasManager, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			return this.TypeEngine.GetBaseTypes(leafEntityId)
				.Select(t => t.CaptionId)
				.Where (id => id != rootEntityId)
				.Reverse ()
				.Select (id => this.BuildJoinToSubEntity (aliasManager, entity, rootEntityId, id))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildJoinToSubEntity(AliasManager aliasManager, AbstractEntity entity, Druid rootEntityId, Druid localEntityId)
		{
			DbTable rootDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);
			DbTable localDbTable = this.SchemaEngine.GetEntityTable (localEntityId);

			DbColumn rootIdDbColumn = rootDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];
			DbColumn localIdDbColumn = localDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			string rootAlias = aliasManager.GetAlias (entity, rootEntityId);
			string localAlias = aliasManager.GetAlias (entity, localEntityId);
			
			SqlField sqlTable = SqlField.CreateAliasedName (localDbTable.GetSqlName (), localAlias);

			SqlField rootIdColumn = SqlField.CreateAliasedName (rootAlias, rootIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);
			SqlField localIdColumn = SqlField.CreateAliasedName (localAlias, localIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (rootIdColumn, localIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForFields(Request request, AliasManager aliasManager, AbstractEntity entity)
		{
			return this.TypeEngine.GetFields (entity.GetEntityStructuredTypeId ())
				.Where (f => entity.IsFieldNotEmpty (f.Id))
				.Select (f => this.BuildSqlContainerForField (entity, request, aliasManager, f.CaptionId))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForField(AbstractEntity entity, Request request, AliasManager aliasManager, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);

			switch (field.Relation)
			{
				case FieldRelation.None:
					return this.BuildSqlContainerForValueField (aliasManager, entity, field);

				case FieldRelation.Reference:
					return this.BuildSqlContainerForReferenceField (request, aliasManager, entity, field);

				case FieldRelation.Collection:
					return this.BuildSqlContainerForCollectionField (request, aliasManager, entity, field);

				default:
					throw new System.NotImplementedException ();
			}
		}


		private SqlContainer BuildSqlContainerForValueField(AliasManager aliasManager, AbstractEntity entity, StructuredTypeField field)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, field.CaptionId);

			object fieldValue = entity.InternalGetValue (field.CaptionId.ToResourceId ());

			DbTypeDef columnType = dbColumn.Type;
			DbRawType rawType = columnType.RawType;
			DbSimpleType simpleType = columnType.SimpleType;
			DbNumDef numDef = columnType.NumDef;

			object convertedValue = this.DataConverter.FromCresusToDatabaseValue (rawType, simpleType, numDef, fieldValue);
			DbRawType convertedRawType = this.DataConverter.FromDotNetToDatabaseType (columnType.RawType);
			SqlFunctionCode sqlFunctionCode = SqlFunctionCode.Unknown;

			switch (convertedRawType)
			{
				case DbRawType.String:
					sqlFunctionCode = SqlFunctionCode.CompareLike;
					break;

				case DbRawType.Boolean:
				case DbRawType.ByteArray:
				case DbRawType.Date:
				case DbRawType.DateTime:
				case DbRawType.Guid:
				case DbRawType.Int16:
				case DbRawType.Int32:
				case DbRawType.Int64:
				case DbRawType.LargeDecimal:
				case DbRawType.SmallDecimal:
				case DbRawType.Time:
					sqlFunctionCode = SqlFunctionCode.CompareEqual;
					break;

				default:
					throw new System.NotImplementedException ();
			}

			SqlFunction sqlCondition = new SqlFunction
			(
				sqlFunctionCode,
				this.BuildSqlFieldForEntityColumn (aliasManager, entity, localEntityId, dbColumn.Name),
				SqlField.CreateConstant (convertedValue, convertedRawType)
			);

			return SqlContainer.CreateSqlConditions (sqlCondition);
		}


		private SqlContainer BuildSqlContainerForReferenceField(Request request, AliasManager aliasManager, AbstractEntity entity, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			if (this.DataContext.IsPersistent (target))
			{
				DbKey targetKey = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;

				return this.BuildSqlContainerForReferenceByReference (aliasManager, entity, field, targetKey);
			}
			else
			{
				return this.BuildSqlContainerForReferenceByValue (request, aliasManager, entity, field, target);
			}
		}


		private SqlContainer BuildSqlContainerForReferenceByReference(AliasManager aliasManager, AbstractEntity source, StructuredTypeField field, DbKey targetKey)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			long targetId = targetKey.Id.Value;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			SqlFunction condition = new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForEntityColumn (aliasManager, source, localEntityId, fieldName),
				SqlField.CreateConstant (targetId, DbRawType.Int64)
			);

			return SqlContainer.CreateSqlConditions (condition);
		}


		private SqlContainer BuildSqlContainerForReferenceByValue(Request request, AliasManager aliasManager, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToReferenceTarget (aliasManager, source, target, localEntityId, fieldId, rootTargetId);
			
			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, aliasManager, target);

			return joinToTarget.Plus (containerForTarget);
		}


		private SqlContainer BuildSqlContainerForJoinToReferenceTarget(AliasManager aliasManager, AbstractEntity source, AbstractEntity target, Druid localEntityId, Druid fieldId, Druid rootTargetId)
		{
			var localSourceAlias = aliasManager.GetAlias (source, localEntityId);
			var rootTargetAlias = aliasManager.GetAlias (target, rootTargetId);

			DbColumn localSourceRefIdDbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);

			DbTable rootTargetDbTable = this.SchemaEngine.GetEntityTable (rootTargetId);
			DbColumn targetIdDbColumn = rootTargetDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (rootTargetDbTable.GetSqlName (), rootTargetAlias);

			SqlField sourceRefIdColumn = SqlField.CreateAliasedName (localSourceAlias, localSourceRefIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField targetIdColumn = SqlField.CreateAliasedName (rootTargetAlias, targetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (sourceRefIdColumn, targetIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForCollectionField(Request request, AliasManager aliasManager, AbstractEntity entity, StructuredTypeField field)
		{
			return entity
				.GetFieldCollection<AbstractEntity> (field.Id)
				.Select (r => this.BuildSqlContainerForCollection (request, aliasManager, entity, field, r))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForCollection(Request request, AliasManager aliasManager, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			if (this.DataContext.IsPersistent (target))
			{
				DbKey targetKey = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;

				return this.BuildSqlContainerForCollectionByReference (aliasManager, source, field, targetKey);
			}
			else
			{
				return this.BuildSqlContainerForCollectionByValue (request, aliasManager, source, field, target);
			}
		}


		private SqlContainer BuildSqlContainerForCollectionByReference(AliasManager aliasManager, AbstractEntity source, StructuredTypeField field, DbKey targetKey)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			long targetId = targetKey.Id.Value;

			var relationAlias = aliasManager.GetAlias ();

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToCollectionTable (aliasManager, source, relationAlias, localEntityId, fieldId, SqlJoinCode.Inner);

			SqlFunction conditionForRelationTargetId = this.BuildConditionForCollectionTargetId (relationAlias, localEntityId, fieldId, targetId);

			return joinToRelation.PlusSqlConditions (conditionForRelationTargetId);
		}


		private SqlContainer BuildSqlContainerForCollectionByValue(Request request, AliasManager aliasManager, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

			var relationAlias = aliasManager.GetAlias ();

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToCollectionTable (aliasManager, source, relationAlias, localEntityId, fieldId, SqlJoinCode.Inner);
			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToCollectionTarget (aliasManager, relationAlias, localEntityId, fieldId, target, rootTargetId);

			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, aliasManager, target);

			return joinToRelation.Plus (joinToTarget).Plus (containerForTarget);
		}


		private SqlContainer BuildSqlContainerForJoinToCollectionTable(AliasManager aliasManager, AbstractEntity entity, string relationAlias, Druid localEntityId, Druid fieldId, SqlJoinCode sqlJoinCode)
		{
			Druid rootEntityId = this.TypeEngine.GetRootType (localEntityId).CaptionId;

			DbTable rootSourceDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);
			DbTable relationDbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);

			DbColumn sourceIdDbColumn = rootSourceDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];
			DbColumn relationSourceIdDbColumn = relationDbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnSourceIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (relationDbTable.GetSqlName (), relationAlias);

			SqlField sourceIdColumn = SqlField.CreateAliasedName (aliasManager.GetAlias (entity), sourceIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);
			SqlField relationSourceIdColumn = SqlField.CreateAliasedName (relationAlias, relationSourceIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnSourceIdName);

			SqlJoin sqlJoin = new SqlJoin (sourceIdColumn, relationSourceIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForJoinToCollectionTarget(AliasManager aliasManager, string relationAlias, Druid localEntityId, Druid fieldId, AbstractEntity target, Druid rootTargetId)
		{
			var rootTargetAlias = aliasManager.GetAlias (target);

			DbTable relationDbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			DbTable rootTargetDbTable = this.SchemaEngine.GetEntityTable (rootTargetId);

			DbColumn relationTargetIdDbColumn = relationDbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnTargetIdName];
			DbColumn targetIdDbColumn = rootTargetDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (rootTargetDbTable.GetSqlName (), rootTargetAlias);

			SqlField relationTargetIdColumn = SqlField.CreateAliasedName (relationAlias, relationTargetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField targetIdColumn = SqlField.CreateAliasedName (rootTargetAlias, targetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (relationTargetIdColumn, targetIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlFunction BuildConditionForCollectionTargetId(string relationAlias, Druid localEntityId, Druid fieldId, long targetId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForRelationColumn (relationAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnTargetIdName),
				SqlField.CreateConstant (targetId, DbRawType.Int64)
			);
		}


		private SqlContainer BuildSqlContainerForConstraints(Request request, AliasManager aliasManager)
		{
			var sqlFieldBuilder = this.CreateSqlFieldBuilder (aliasManager);

			var sqlFunctions = from condition in request.Conditions
							   select condition.CreateSqlCondition(sqlFieldBuilder);

			return SqlContainer.CreateSqlConditions (sqlFunctions.ToArray ());
		}


		#endregion

		#region VALUE AND REFERENCE QUERY GENERATION


		private SqlContainer BuildSqlContainerForValuesAndReferences(AliasManager aliasManager, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			SqlContainer sqlContainerForValues = this.TypeEngine.GetValueFields (leafEntityId)
				.Where (field => field.Type.SystemType != typeof (byte[]))
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForValueField (aliasManager, entity, field))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.PlusSqlFields (e));

			SqlContainer sqlContainerForReferences = this.TypeEngine.GetReferenceFields (leafEntityId)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForReferenceField (aliasManager, entity, field))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.PlusSqlFields (e));

			SqlContainer sqlContainerForMetaData = SqlContainer.Empty
				.PlusSqlFields (this.BuildSqlFieldForLogId (aliasManager, entity, rootEntityId))
				.PlusSqlFields (this.BuildSqlFieldForEntityColumn (aliasManager, entity, rootEntityId, EntitySchemaBuilder.EntityTableColumnEntityTypeIdName))
				.PlusSqlFields (this.BuildSqlFieldForEntityColumn (aliasManager, entity, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName));

			return sqlContainerForValues
				.Plus (sqlContainerForReferences)
				.Plus (sqlContainerForMetaData);
		}


		private SqlField BuildSqlFieldForValueField(AliasManager aliasManager, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			return this.BuildSqlFieldForEntityColumn (aliasManager, entity, localEntityId, fieldName);
		}


		private SqlField BuildSqlFieldForLogId(AliasManager aliasManager, AbstractEntity entity, Druid rootEntityId)
		{
			var alias = aliasManager.GetAlias (entity, rootEntityId);

			DbTable dbTable = this.SchemaEngine.GetEntityTable (rootEntityId);
			DbColumn dbColumn = dbTable.Columns[EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName];

			return SqlField.CreateAliasedName (alias, dbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName);
		}


		private SqlField BuildSqlFieldForReferenceField(AliasManager aliasManager, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			return this.BuildSqlFieldForEntityColumn (aliasManager, entity, localEntityId, fieldName);
		}
		

		#endregion

		#region COLLECTION QUERY GENERATION


		private SqlContainer BuildSqlContainerForCollectionSourceIds(AliasManager aliasManager, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			var sqlField = this.BuildSqlFieldForEntityColumn(aliasManager, entity, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName);
			return SqlContainer.CreateSqlFields(sqlField);
		}


		private SqlContainer BuildSqlContainerForCollection(AliasManager aliasManager, AbstractEntity entity, Druid fieldId, SqlSelect sqlSelectForSourceIds)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			var relationAlias = aliasManager.GetAlias ();

			DbTable relationDbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			SqlField sqlTable = SqlField.CreateAliasedName (relationDbTable.GetSqlName (), relationAlias);

			SqlField sqlFieldForTargetId = this.BuildSqlFieldForRelationColumn (relationAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField sqlFieldForSourceId = this.BuildSqlFieldForRelationColumn (relationAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnSourceIdName);

			SqlField sqlFieldForRank = this.BuildSqlFieldForRelationColumn (relationAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnRankName);
			sqlFieldForRank.SortOrder = SqlSortOrder.Ascending;

			SqlFunction sqlFunctionForCondition = new SqlFunction
			(
				SqlFunctionCode.SetIn,
				this.BuildSqlFieldForRelationColumn (relationAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnSourceIdName),
				SqlField.CreateSubQuery (sqlSelectForSourceIds)
			);

			return SqlContainer.CreateSqlTables (sqlTable)
				.PlusSqlFields (sqlFieldForTargetId, sqlFieldForSourceId)
				.PlusSqlConditions (sqlFunctionForCondition)
				.PlusSqlOrderBys (sqlFieldForRank);
		}


		#endregion


		#region COUNT QUERY GENERATION


		private SqlContainer BuildSqlContainerForCount(AliasManager aliasManager, AbstractEntity entity, SqlSelectPredicate predicate)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			SqlField sqlField = this.BuildSqlFieldForEntityColumn (aliasManager, entity, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName);
			SqlField sqlAggregateField = SqlField.CreateAggregate (SqlAggregateFunction.Count, predicate, sqlField);

			return SqlContainer.CreateSqlFields (sqlAggregateField);
		}


		#endregion


		#region ENTITY KEYS GENERATION


		private SqlContainer BuildSqlContainerForEntityKeys(AliasManager aliasManager, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			var sqlFieldForEntityId = this.BuildSqlFieldForEntityColumn (aliasManager, entity, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName);

			return SqlContainer.CreateSqlFields (sqlFieldForEntityId);
		}


		#endregion


		#region ORDER BY GENERATION


		private SqlContainer BuildSqlContainerForOrderBy(Request request, AliasManager aliasManager)
		{
			var sqlFieldBuilder = this.CreateSqlFieldBuilder (aliasManager);

			var sqlFields = from sortClause in request.SortClauses
							select sortClause.CreateSqlField (sqlFieldBuilder);

			return SqlContainer.CreateSqlOrderBys (sqlFields.ToArray ());
		}


		#endregion


		#region MISC


		private SqlField BuildSqlFieldForEntityColumn(AliasManager aliasManager, AbstractEntity entity, Druid localEntityId, string columnName)
		{
			var alias = aliasManager.GetAlias (entity, localEntityId);

			DbTable dbTable = this.SchemaEngine.GetEntityTable (localEntityId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return SqlField.CreateAliasedName (alias, dbColumn.GetSqlName (), columnName);
		}


		private SqlField BuildSqlFieldForRelationColumn(string alias, Druid localEntityId, Druid fieldId, string columnName)
		{
			DbTable dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return SqlField.CreateAliasedName (alias, dbColumn.GetSqlName (), columnName);
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
				var leafEntityId = entity.GetEntityStructuredTypeId ();

				var hasCollectionDefined = this.TypeEngine
					.GetCollectionFields (leafEntityId)
					.Any (f => entity.IsFieldNotEmpty (f.Id));

				if (hasCollectionDefined)
				{
					return true;
				}

				var children = this.TypeEngine
					.GetReferenceFields (leafEntityId)
					.Where (f => entity.IsFieldNotEmpty (f.Id))
					.Select (f => entity.GetField<AbstractEntity> (f.CaptionId.ToResourceId ()));

				foreach (var child in children)
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


		#endregion

		
		private AliasManager CreateAliasManager()
		{
			return new AliasManager (this.TypeEngine);
		}


		private SqlFieldBuilder CreateSqlFieldBuilder(AliasManager aliasManager)
		{
			return new SqlFieldBuilder
			(
				aliasManager,
				this.ResolveSqlConstantField,
				this.ResolveSqlPublicField,
				this.ResolveSqlInternalField
			);
		}


		private SqlField ResolveSqlConstantField(DbRawType dbRawType, DbSimpleType dbSimpleType, DbNumDef dbNumDef, object value)
		{
			object convertedValue = this.DataConverter.FromCresusToDatabaseValue (dbRawType, dbSimpleType, dbNumDef, value);
			DbRawType convertedType = this.DataConverter.FromDotNetToDatabaseType (dbRawType);

			return SqlField.CreateConstant (convertedValue, convertedType);
		}


		private SqlField ResolveSqlPublicField(AliasManager aliasManager, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string columnName = dbColumn.Name;

			return this.BuildSqlFieldForEntityColumn (aliasManager, entity, localEntityId, columnName);
		}


		private SqlField ResolveSqlInternalField(AliasManager aliasManager, AbstractEntity entity, string name)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			return this.BuildSqlFieldForEntityColumn (aliasManager, entity, rootEntityId, name);
		}


	}


}
