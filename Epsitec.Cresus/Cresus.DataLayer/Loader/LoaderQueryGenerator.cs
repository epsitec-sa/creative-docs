//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;
using System.Collections.ObjectModel;

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


		public IEnumerable<EntityData> GetEntitiesData(Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData, ReferenceData>> valuesAndReferencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesAndReferencesData = this.GetValueAndReferenceData (transaction, request);

				if (valuesAndReferencesData.Any ())
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


		private IEnumerable<EntityData> GetEntitiesData(Request request, Dictionary<DbKey, System.Tuple<Druid, long, ValueData, ReferenceData>> valuesAndReferencesData, Dictionary<DbKey, CollectionData> collectionsData)
		{
			foreach (DbKey rowKey in valuesAndReferencesData.Keys)
			{
				var tuple = valuesAndReferencesData[rowKey];

				Druid loadedEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();
				Druid leafEntityId = tuple.Item1;
				long logId = tuple.Item2;
				ValueData entityValueData = tuple.Item3;
				ReferenceData entityReferenceData = tuple.Item4;
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

			Request request = new Request ()
			{
				RootEntity = example,
				RootEntityKey = exampleKey,
			};

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

			rootExample.SetField<AbstractEntity> (fieldName, targetExample);

			Request request = new Request ()
			{
				RootEntity = rootExample,
				RootEntityKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey,
				RequestedEntity = targetExample,
			};

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

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			AbstractEntity targetExample = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			rootExample.GetFieldCollection<AbstractEntity> (fieldName).Add (targetExample);

			Request request = new Request ()
			{
				RootEntity = rootExample,
				RootEntityKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey,
				RequestedEntity = targetExample,
			};

			return this.GetEntitiesData (request);
		}


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionKeys(DbTransaction transaction, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			Request request = new Request ()
			{
				RootEntity = rootExample,
				RootEntityKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey,
			};

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
			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			AliasNode rootEntityAlias = new AliasNode (rootEntityId.ToResourceId ());

			SqlContainer sqlContainerForConditions = this.BuildSqlContainerForRequest (request, rootEntityAlias, entity);

			AbstractEntity requestedEntity = request.RequestedEntity;

			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, rootEntityAlias);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			SqlField sqlFieldForSingleValue = this.BuildSqlFieldForValueField(requestedAlias, requestedEntity, fieldId);

			return sqlContainerForConditions.PlusSqlFields (sqlFieldForSingleValue).BuildSqlSelect ();
		}
		
		
		#endregion


		#region GET VALUE AND REFERENCE DATA
		

		private Dictionary<DbKey, System.Tuple<Druid, long, ValueData, ReferenceData>> GetValueAndReferenceData(DbTransaction transaction, Request request)
		{
			var valueData = new Dictionary<DbKey, System.Tuple<Druid, long, ValueData, ReferenceData>> ();

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

				valueData[entityKey] = System.Tuple.Create (realEntityId, logId, entityValueData, entityReferenceData);
			}

			return valueData;
		}


		private SqlSelect CreateSqlSelectForValueAndReferenceData(Request request)
		{
			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			AliasNode rootEntityAlias = new AliasNode (rootEntityId.ToResourceId ());

			SqlContainer sqlContainerForConditions = this.BuildSqlContainerForRequest (request, rootEntityAlias, entity);

			AbstractEntity requestedEntity = request.RequestedEntity;

			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, rootEntityAlias);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			SqlContainer sqlContainerForValuesAndReferences = this.BuildSqlContainerForValuesAndReferences (requestedAlias, requestedEntity);

			return sqlContainerForConditions.Plus (sqlContainerForValuesAndReferences).BuildSqlSelect ();
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
			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			AliasNode rootEntityAlias = new AliasNode (rootEntityId.ToResourceId ());

			SqlContainer  sqlContainerForConditions = this.BuildSqlContainerForRequest (request, rootEntityAlias, entity);

			AbstractEntity requestedEntity = request.RequestedEntity;

			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, rootEntityAlias);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			SqlContainer sqlContainerForCollection = this.BuildSqlContainerForCollection (requestedAlias, requestedEntity, fieldId);

			return sqlContainerForConditions.Plus (sqlContainerForCollection).BuildSqlSelect ();
		}


		#endregion


		#region CONDITION GENERATION


		private SqlContainer BuildSqlContainerForRequest(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			SqlField sqlTableForRequestRootEntity = this.BuildTableForRootEntity (rootEntityAlias, entity);
			SqlContainer sqlContainerForEntity = this.BuildSqlContainerForEntity (request, rootEntityAlias, entity);

			return SqlContainer.CreateSqlTables (sqlTableForRequestRootEntity).Plus (sqlContainerForEntity);
		}


		private SqlField BuildTableForRootEntity(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			DbTable rootDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);

			return SqlField.CreateAliasedName (rootDbTable.GetSqlName (), rootEntityAlias.Alias);
		}


		private SqlContainer BuildSqlContainerForEntity(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			SqlContainer sqlContainerForRequestRootEntityId = this.BuildSqlContainerForRequestRootEntityId (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForRequestRequestedEntity = this.BuildSqlContainerForRequestRequestedEntity (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForSubEntities = this.BuildSqlContainerForSubEntities (rootEntityAlias, entity);
			SqlContainer sqlContainerForFields = this.BuildSqlContainerForFields (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForLocalConstraints = this.BuildSqlContainerForLocalConstraints (request, rootEntityAlias, entity);

			return sqlContainerForRequestRootEntityId
				.Plus (sqlContainerForRequestRequestedEntity)
				.Plus (sqlContainerForSubEntities)
				.Plus (sqlContainerForFields)
				.Plus (sqlContainerForLocalConstraints);
		}


		private SqlContainer BuildSqlContainerForRequestRootEntityId(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			DbKey? rootEntityKey = request.RootEntityKey;

			SqlContainer sqlContainer = SqlContainer.Empty;

			if (rootEntityKey.HasValue && entity == request.RootEntity)
			{
				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

				long id = rootEntityKey.Value.Id.Value;

				SqlFunction sqlCondition = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName),
					SqlField.CreateConstant (id, DbRawType.Int64)
				);

				sqlContainer = sqlContainer.PlusSqlConditions (sqlCondition);
			}

			return sqlContainer;
		}


		private SqlContainer BuildSqlContainerForRequestRequestedEntity(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			SqlContainer sqlContainer = SqlContainer.Empty;

			long? minimumLogId = request.RequestedEntityMinimumLogId;

			if (minimumLogId.HasValue)
			{
				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

				SqlFunction sqlCondition = new SqlFunction
				(
					SqlFunctionCode.CompareGreaterThanOrEqual,
					this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName),
					SqlField.CreateConstant (minimumLogId.Value, DbRawType.Int64)
				);

				sqlContainer = sqlContainer.PlusSqlConditions (sqlCondition);
			}

			return sqlContainer;
		}


		private SqlContainer BuildSqlContainerForSubEntities(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			return this.TypeEngine.GetBaseTypes(leafEntityId)
				.Select(t => t.CaptionId)
				.Where (id => id != rootEntityId)
				.Reverse ()
				.Select (id => this.BuildJoinToSubEntity (rootEntityAlias, rootEntityId, id))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildJoinToSubEntity(AliasNode rootEntityAlias, Druid rootEntityId, Druid localEntityId)
		{
			AliasNode localEntityAlias = rootEntityAlias.CreateChild (localEntityId.ToResourceId ());

			DbTable rootDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);
			DbTable localDbTable = this.SchemaEngine.GetEntityTable (localEntityId);

			DbColumn rootIdDbColumn = rootDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];
			DbColumn localIdDbColumn = localDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (localDbTable.GetSqlName (), localEntityAlias.Alias);

			SqlField rootIdColumn = SqlField.CreateAliasedName (rootEntityAlias.Alias, rootIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);
			SqlField localIdColumn = SqlField.CreateAliasedName (localEntityAlias.Alias, localIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (rootIdColumn, localIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForFields(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			return this.TypeEngine.GetFields (entity.GetEntityStructuredTypeId ())
				.Where (f => entity.GetEntityContext ().IsFieldDefined (f.Id, entity))
				.Select (f => this.BuildSqlContainerForField (entity, request, rootEntityAlias, f.CaptionId))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForField(AbstractEntity entity, Request request, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (leafEntityId, fieldId);

			switch (field.Relation)
			{
				case FieldRelation.None:
					return this.BuildSqlContainerForValueField (rootEntityAlias, entity, field);

				case FieldRelation.Reference:
					return this.BuildSqlContainerForReferenceField (request, rootEntityAlias, entity, field);

				case FieldRelation.Collection:
					return this.BuildSqlContainerForCollectionField (request, rootEntityAlias, entity, field);

				default:
					throw new System.NotImplementedException ();
			}
		}


		private SqlContainer BuildSqlContainerForValueField(AliasNode rootEntityAlias, AbstractEntity entity, StructuredTypeField field)
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
				this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, dbColumn.Name),
				SqlField.CreateConstant (convertedValue, convertedRawType)
			);

			return SqlContainer.CreateSqlConditions (sqlCondition);
		}


		private SqlContainer BuildSqlContainerForReferenceField(Request request, AliasNode rootEntityAlias, AbstractEntity entity, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			if (this.DataContext.IsPersistent (target))
			{
				DbKey targetKey = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;

				return this.BuildSqlContainerForReferenceByReference (rootEntityAlias, entity, field, targetKey);
			}
			else
			{
				return this.BuildSqlContainerForReferenceByValue (request, rootEntityAlias, entity, field, target);
			}
		}


		private SqlContainer BuildSqlContainerForReferenceByReference(AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, DbKey targetKey)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			long targetId = targetKey.Id.Value;

			AliasNode localSourceAlias = this.GetLocalEntityAlias (rootSourceAlias, localEntityId);

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			SqlFunction condition = new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForEntityColumn (localSourceAlias, localEntityId, fieldName),
				SqlField.CreateConstant (targetId, DbRawType.Int64)
			);

			return SqlContainer.CreateSqlConditions (condition);
		}


		private SqlContainer BuildSqlContainerForReferenceByValue(Request request, AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToReferenceTarget (rootSourceAlias, localEntityId, fieldId, rootTargetId);
			AliasNode fakeRelationAlias = rootSourceAlias.GetChild (fieldId.ToResourceId ());
			AliasNode targetAlias = fakeRelationAlias.GetChild (rootTargetId.ToResourceId ());

			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, targetAlias, target);

			return joinToTarget.Plus (containerForTarget);
		}


		private SqlContainer BuildSqlContainerForJoinToReferenceTarget(AliasNode sourceAlias, Druid localEntityId, Druid fieldId, Druid rootTargetId)
		{
			AliasNode localSourceAlias = this.GetLocalEntityAlias (sourceAlias, localEntityId);
			AliasNode fakeRelationAlias = sourceAlias.CreateChild (fieldId.ToResourceId ());
			AliasNode rootTargetAlias = fakeRelationAlias.CreateChild (rootTargetId.ToResourceId ());

			DbColumn localSourceRefIdDbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);

			DbTable rootTargetDbTable = this.SchemaEngine.GetEntityTable (rootTargetId);
			DbColumn targetIdDbColumn = rootTargetDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (rootTargetDbTable.GetSqlName (), rootTargetAlias.Alias);

			SqlField sourceRefIdColumn = SqlField.CreateAliasedName (localSourceAlias.Alias, localSourceRefIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField targetIdColumn = SqlField.CreateAliasedName (rootTargetAlias.Alias, targetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (sourceRefIdColumn, targetIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForCollectionField(Request request, AliasNode rootEntityAlias, AbstractEntity entity, StructuredTypeField field)
		{
			return entity
				.GetFieldCollection<AbstractEntity> (field.Id)
				.Select (r => this.BuildSqlContainerForCollection (request, rootEntityAlias, entity, field, r))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForCollection(Request request, AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			if (this.DataContext.IsPersistent (target))
			{
				DbKey targetKey = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;

				return this.BuildSqlContainerForCollectionByReference (rootSourceAlias, source, field, targetKey);
			}
			else
			{
				return this.BuildSqlContainerForCollectionByValue (request, rootSourceAlias, source, field, target);
			}
		}


		private SqlContainer BuildSqlContainerForCollectionByReference(AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, DbKey targetKey)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			long targetId = targetKey.Id.Value;

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToCollectionTable (rootSourceAlias, localEntityId, fieldId, SqlJoinCode.Inner);

			SqlFunction conditionForRelationTargetId = this.BuildConditionForCollectionTargetId (rootSourceAlias, localEntityId, fieldId, targetId);

			return joinToRelation.PlusSqlConditions (conditionForRelationTargetId);
		}


		private SqlContainer BuildSqlContainerForCollectionByValue(Request request, AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, field.CaptionId).CaptionId;

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToCollectionTable (rootSourceAlias, localEntityId, fieldId, SqlJoinCode.Inner);
			AliasNode relationAlias = rootSourceAlias.GetChildren (field.Id).Last ();

			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToCollectionTarget (relationAlias, localEntityId, fieldId, rootTargetId);
			AliasNode targetAlias = relationAlias.GetChild (rootTargetId.ToResourceId ());

			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, targetAlias, target);

			return joinToRelation.Plus (joinToTarget).Plus (containerForTarget);
		}


		private SqlContainer BuildSqlContainerForJoinToCollectionTable(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, SqlJoinCode sqlJoinCode)
		{
			Druid rootEntityId = this.TypeEngine.GetRootType (localEntityId).CaptionId;

			AliasNode relationAlias = rootEntityAlias.CreateChild (fieldId.ToResourceId ());

			DbTable rootSourceDbTable = this.SchemaEngine.GetEntityTable (rootEntityId);
			DbTable relationDbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);

			DbColumn sourceIdDbColumn = rootSourceDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];
			DbColumn relationSourceIdDbColumn = relationDbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnSourceIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (relationDbTable.GetSqlName (), relationAlias.Alias);

			SqlField sourceIdColumn = SqlField.CreateAliasedName (rootEntityAlias.Alias, sourceIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);
			SqlField relationSourceIdColumn = SqlField.CreateAliasedName (relationAlias.Alias, relationSourceIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnSourceIdName);

			SqlJoin sqlJoin = new SqlJoin (sourceIdColumn, relationSourceIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForJoinToCollectionTarget(AliasNode relationAlias, Druid localEntityId, Druid fieldId, Druid rootTargetId)
		{
			AliasNode rootTargetAlias = relationAlias.CreateChild (rootTargetId.ToResourceId ());

			DbTable relationDbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			DbTable rootTargetDbTable = this.SchemaEngine.GetEntityTable (rootTargetId);

			DbColumn relationTargetIdDbColumn = relationDbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnTargetIdName];
			DbColumn targetIdDbColumn = rootTargetDbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName];

			SqlField sqlTable = SqlField.CreateAliasedName (rootTargetDbTable.GetSqlName (), rootTargetAlias.Alias);

			SqlField relationTargetIdColumn = SqlField.CreateAliasedName (relationAlias.Alias, relationTargetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField targetIdColumn = SqlField.CreateAliasedName (rootTargetAlias.Alias, targetIdDbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnIdName);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (relationTargetIdColumn, targetIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlFunction BuildConditionForCollectionTargetId(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, long targetId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnTargetIdName),
				SqlField.CreateConstant (targetId, DbRawType.Int64)
			);
		}


		private SqlContainer BuildSqlContainerForLocalConstraints(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			return request
				.GetLocalConstraints (entity)
				.Select (c => this.BuildConditionForLocalConstraint (rootEntityAlias, entity, c))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.PlusSqlConditions (e));
		}


		private SqlFunction BuildConditionForLocalConstraint(AliasNode rootEntityAlias, AbstractEntity entity, Expression constraint)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return constraint.CreateSqlCondition
			(
				(dbRawType, dbSimpleType, dbNumDef, value) =>
				{
					object convertedValue = this.DataConverter.FromCresusToDatabaseValue (dbRawType, dbSimpleType, dbNumDef, value);
					DbRawType convertedType = this.DataConverter.FromDotNetToDatabaseType (dbRawType);

					return SqlField.CreateConstant (convertedValue, convertedType);
				},
				(fieldId) =>
				{
					Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;
					DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
					string columnName = dbColumn.Name;

					return this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName);
				}
			);
		}


		#endregion


		#region VALUE AND REFERENCE QUERY GENERATION


		private SqlContainer BuildSqlContainerForValuesAndReferences(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.TypeEngine.GetRootType (leafEntityId).CaptionId;

			SqlContainer sqlContainerForValues = this.TypeEngine.GetValueFields (leafEntityId)
				.Where (field => field.Type.SystemType != typeof (byte[]))
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForValueField (rootEntityAlias, entity, field))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.PlusSqlFields (e));

			SqlContainer sqlContainerForReferences = this.TypeEngine.GetReferenceFields (leafEntityId)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForReferenceField (rootEntityAlias, entity, field))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.PlusSqlFields (e));

			SqlContainer sqlContainerForMetaData = SqlContainer.Empty
				.PlusSqlFields (this.BuildSqlFieldForLogId (rootEntityAlias, rootEntityId))
				.PlusSqlFields (this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, EntitySchemaBuilder.EntityTableColumnEntityTypeIdName))
				.PlusSqlFields (this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, EntitySchemaBuilder.EntityTableColumnIdName));

			return sqlContainerForValues
				.Plus (sqlContainerForReferences)
				.Plus (sqlContainerForMetaData);
		}


		private SqlField BuildSqlFieldForValueField(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			return this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, fieldName);
		}


		private SqlField BuildSqlFieldForLogId(AliasNode rootEntityAlias, Druid rootEntityId)
		{
			DbTable dbTable = this.DbInfrastructure.ResolveDbTable (rootEntityId);
			DbColumn dbColumn = dbTable.Columns[EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName];

			return SqlField.CreateAliasedName (rootEntityAlias.Alias, dbColumn.GetSqlName (), EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName);
		}


		private SqlField BuildSqlFieldForReferenceField(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			DbColumn dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityId, fieldId);
			string fieldName = dbColumn.Name;

			return this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, fieldName);
		}
		

		#endregion


		#region COLLECTION QUERY GENERATION


		private SqlContainer BuildSqlContainerForCollection(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			SqlContainer sqlContainerForRelationJoin = this.BuildSqlContainerForJoinToCollectionTable (rootEntityAlias, localEntityId, fieldId, SqlJoinCode.Inner);

			SqlField sqlFieldForTargetId = this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnTargetIdName);
			SqlField sqlFieldForSourceId = this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnSourceIdName);

			SqlField sqlFieldForRank = this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, EntitySchemaBuilder.EntityFieldTableColumnRankName);
			sqlFieldForRank.SortOrder = SqlSortOrder.Ascending;

			return sqlContainerForRelationJoin
				.PlusSqlFields (sqlFieldForTargetId, sqlFieldForSourceId)
				.PlusSqlOrderBys (sqlFieldForRank);
		}


		#endregion


		#region MISC


		private SqlField BuildSqlFieldForEntityColumn(AliasNode rootEntityAlias, Druid localEntityId, string columnName)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId);

			DbTable dbTable = this.SchemaEngine.GetEntityTable (localEntityId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return SqlField.CreateAliasedName (localAlias.Alias, dbColumn.GetSqlName (), columnName);
		}


		private SqlField BuildSqlFieldForRelationColumn(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, string columnName)
		{
			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			DbTable dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return SqlField.CreateAliasedName (relationAlias.Alias, dbColumn.GetSqlName (), columnName);
		}


		#endregion


		#region ALIAS


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


		private AliasNode RetreiveRequestedEntityAlias(Request request, AliasNode rootEntityAlias)
		{
			AbstractEntity rootEntity = request.RootEntity;
			AbstractEntity requestedEntity = request.RequestedEntity;

			return this.RetreiveRequestedEntityAliasRec (rootEntity, requestedEntity, rootEntityAlias);
		}


		private AliasNode RetreiveRequestedEntityAliasRec(AbstractEntity entity, AbstractEntity requestedEntity, AliasNode entityAliasNode)
		{
			if (entity == requestedEntity)
			{
				return entityAliasNode;
			}
			else
			{
				AliasNode requestedEntityAlias = null;

				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				var definedFields = this.TypeEngine.GetFields (leafEntityId)
					.Where (f => entity.GetEntityContext ().IsFieldDefined (f.Id, entity))
					.ToList ();

				for (int i = 0; i < definedFields.Count && requestedEntityAlias == null; i++)
				{
					StructuredTypeField field = definedFields[i];
					Druid fieldId = field.CaptionId;

					switch (field.Relation)
					{
						case FieldRelation.Reference:
							{
								AbstractEntity targetEntity = entity.GetField<AbstractEntity> (fieldId.ToResourceId ());

								Druid leafTargetId = targetEntity.GetEntityStructuredTypeId ();
								Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

								AliasNode fakeRelationAlias = entityAliasNode.GetChild (fieldId.ToResourceId ());
								AliasNode targetAlias = fakeRelationAlias.GetChild (rootTargetId.ToResourceId ());

								requestedEntityAlias = this.RetreiveRequestedEntityAliasRec (targetEntity, requestedEntity, targetAlias);

								break;
							}
						case FieldRelation.Collection:
							{

								IList<AbstractEntity> targetEntities = entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ());
								ReadOnlyCollection<AliasNode> targetAliases = entityAliasNode.GetChildren (fieldId.ToResourceId ());

								for (int j = 0; j < targetEntities.Count && requestedEntityAlias == null; j++)
								{
									AbstractEntity targetEntity = targetEntities[j];

									Druid leafTargetId = targetEntity.GetEntityStructuredTypeId ();
									Druid rootTargetId = this.TypeEngine.GetRootType (leafTargetId).CaptionId;

									AliasNode targetAlias = targetAliases[j].GetChild (rootTargetId.ToResourceId ());

									requestedEntityAlias = this.RetreiveRequestedEntityAliasRec (targetEntity, requestedEntity, targetAlias);
								}

								break;
							}
					}
				}

				return requestedEntityAlias;
			}
		}


		#endregion


	}


}
