﻿//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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


	// TODO Improve request for ids in targets.
	// Marc


	// TODO Try to recycle the condition container for the three requests. Or use the ids that have
	// been obtained in the first request in order to build the 2 others.
	// Marc


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


		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.SchemaEngine;
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
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valuesData;
			Dictionary<DbKey, ReferenceData> referencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (DbTransaction innerTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (innerTransaction, request, false);
				referencesData = this.GetReferenceData (innerTransaction, request);
				collectionsData = this.GetCollectionData (innerTransaction, request);

				innerTransaction.Commit ();
			}

			foreach (DbKey rowKey in valuesData.Keys)
			{
				System.Tuple<Druid, long, ValueData> tuple = valuesData[rowKey];
				
				Druid loadedEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();
				Druid leafEntityId = tuple.Item1;
				long logSequenceNumber = tuple.Item2;
				ValueData entityValueData = tuple.Item3;
				ReferenceData entityReferenceData = referencesData[rowKey];
				CollectionData entityCollectionData = collectionsData.ContainsKey (rowKey) ? collectionsData[rowKey] : new CollectionData ();

				yield return new EntityData (rowKey, leafEntityId, loadedEntityId, logSequenceNumber, entityValueData, entityReferenceData, entityCollectionData);
			}
		}


		#endregion


		#region GET VALUE FIELD


		public object GetValueField(AbstractEntity entity, Druid fieldId)
		{
			// TODO Make a more optimized request that only fetches the requested field value and which
			// is not as overkill (and overslow?). If this method is improved, then it might be a
			// good idea to remove the returnBinaryBlob arguments from the GetValueData(...) method.
			// Marc

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AbstractEntity example = EntityClassFactory.CreateEmptyEntity (localEntityId);
			DbKey exampleKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			Request request = new Request ()
			{
				RootEntity = example,
				RootEntityKey = exampleKey,
			};

			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valuesData;

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (transaction, request, true);

				transaction.Commit ();
			}

			ValueData valueData = valuesData[exampleKey].Item3;

			return valueData[fieldId];
		}


		#endregion


		#region GET REFERENCE FIELD


		public EntityData GetReferenceField(AbstractEntity entity, Druid fieldId)
		{
			string fieldName = fieldId.ToResourceId ();

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldName);

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
				targetsData = this.GetCollectionEntityData(entity, fieldId)
					.ToDictionary (data => data.RowKey, data => data);

				targetKeys = this.GetCollectionKeys (transaction, entity, fieldId)
					.Select (d => d.Item2).ToList ();

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
			StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldName);

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


		public long? GetLogSequenceNumberForEntity(EntityKey normalizedEntityKey)
		{
			
			DbTable entityDbTable = this.SchemaEngine.GetEntityTableDefinition (normalizedEntityKey.EntityId);
			DbTable logDbTable = this.DbInfrastructure.ResolveDbTable (Tags.TableLog);

			DbColumn entityIdDbColumn = entityDbTable.Columns[Tags.ColumnId];
			DbColumn entityLogIdDbColumn = entityDbTable.Columns[Tags.ColumnRefLog];
			DbColumn logIdDbColumn = logDbTable.Columns[Tags.ColumnId];
			DbColumn logSequenceNumberIdDbColumn = logDbTable.Columns[Tags.ColumnSequenceNumber];

			SqlField entitySqlTable = SqlField.CreateAliasedName (entityDbTable.GetSqlName (), "entity");
			SqlField logSqlTable = SqlField.CreateAliasedName (logDbTable.GetSqlName (), "log");

			SqlField entityIdColumn = SqlField.CreateAliasedName ("entity", entityIdDbColumn.GetSqlName (), Tags.ColumnId);
			SqlField entityLogIdColumn = SqlField.CreateAliasedName ("entity", entityLogIdDbColumn.GetSqlName (), Tags.ColumnRefLog);
			SqlField logLogIdColumn = SqlField.CreateAliasedName ("log", logIdDbColumn.GetSqlName (), Tags.ColumnId);
			SqlField logSequenceNumberIdColumn = SqlField.CreateAliasedName ("log", logSequenceNumberIdDbColumn.GetSqlName (), Tags.ColumnSequenceNumber);

			SqlJoin sqlJoin = new SqlJoin (entityLogIdColumn, logLogIdColumn, SqlJoinCode.Inner);

			SqlField condition = SqlField.CreateFunction
			(
				new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					entityIdColumn,
					SqlField.CreateConstant (normalizedEntityKey.RowKey.Id.Value, DbRawType.Int64)
				)
			);

			SqlSelect query = new SqlSelect ();

			query.Tables.Add (entitySqlTable);
			query.Tables.Add (logSqlTable);

			query.Joins.Add (sqlJoin);

			query.Conditions.Add (condition);

			query.Fields.Add (logSequenceNumberIdColumn);

			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				dbTransaction.SqlBuilder.SelectData (query);
				object value = this.DbInfrastructure.ExecuteScalar (dbTransaction);
				
				dbTransaction.Commit ();

				long? result = null;

				if (value != null)
				{
					result = (long) value;
				}

				return result;
			}
		}


		#region GET VALUE DATA


		private Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> GetValueData(DbTransaction transaction, Request request, bool returnBinaryBlobs)
		{
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> ();

			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var fields = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.Where (field => returnBinaryBlobs || field.Type.SystemType != typeof (byte[]))
				.OrderBy (field => field.CaptionId.ToResourceId ())
				.ToList ();

			SqlSelect select = this.CreateSqlSelectForValueData (request, returnBinaryBlobs);

			transaction.SqlBuilder.SelectData (select);
			DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				ValueData entityValueData = new ValueData ();
				long logSequenceNumber = (long) dataRow[dataRow.ItemArray.Length - 3];
				Druid realEntityId = Druid.FromLong ((long) dataRow[dataRow.ItemArray.Length - 2]);
				DbKey entityKey = new DbKey (new DbId ((long) dataRow[dataRow.ItemArray.Length - 1]));

				for (int i = 0; i < dataRow.ItemArray.Length - 3; i++)
				{
					object internalValue = dataRow[i];

					if (internalValue != System.DBNull.Value)
					{
						StructuredTypeField field = fields[i];
						INamedType type = field.Type;

						Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, field.CaptionId);
						string columnName = this.SchemaEngine.GetEntityColumnName (field.CaptionId);

						DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
						DbColumn dbColumn = dbTable.Columns[columnName];

						DbTypeDef typeDef = dbColumn.Type;
						DbRawType rawType = typeDef.RawType;
						DbSimpleType simpleType = typeDef.SimpleType;
						DbNumDef numDef = typeDef.NumDef;

						object externalValue = this.DataConverter.FromDatabaseToCresusValue (type, rawType, simpleType, numDef, internalValue);

						entityValueData[field.CaptionId] = externalValue;
					}
				}

				valueData[entityKey] = System.Tuple.Create (realEntityId, logSequenceNumber, entityValueData);
			}

			return valueData;
		}


		private SqlSelect CreateSqlSelectForValueData(Request request, bool returnBinaryBlobs)
		{
			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			AliasNode rootEntityAlias = new AliasNode (rootEntityId.ToResourceId ());

			SqlContainer sqlContainerForConditions = this.BuildSqlContainerForRequest (request, rootEntityAlias, entity);

			AbstractEntity requestedEntity = request.RequestedEntity;

			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, rootEntityAlias);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			SqlContainer sqlContainerForValues = this.BuildSqlContainerForValues (requestedAlias, requestedEntity, returnBinaryBlobs);

			return sqlContainerForConditions.Plus (sqlContainerForValues).BuildSqlSelect ();
		}


		#endregion


		#region GET REFERENCE DATA


		private Dictionary<DbKey, ReferenceData> GetReferenceData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, ReferenceData> references = new Dictionary<DbKey, ReferenceData> ();

			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var fieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			SqlSelect select = this.CreateSqlSelectForReferenceData (request);

			transaction.SqlBuilder.SelectData (select);
		    DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

			foreach (DataRow dataRow in data.Tables[0].Rows)
			{
				DbKey sourceKey = new DbKey (new DbId ((long) dataRow[dataRow.ItemArray.Length - 1]));

				ReferenceData entityReferenceData = new ReferenceData ();

				for (int i = 0; i < dataRow.ItemArray.Length - 1; i++)
				{
					if (dataRow[i] != System.DBNull.Value)
					{
						entityReferenceData[fieldIds[i]] = new DbKey (new DbId ((long) dataRow[i]));
					}
				}

				references[sourceKey] = entityReferenceData;
			}

			return references;
		}


		private SqlSelect CreateSqlSelectForReferenceData(Request request)
		{
			AbstractEntity entity = request.RootEntity;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			AliasNode rootEntityAlias = new AliasNode (rootEntityId.ToResourceId ());

			SqlContainer  sqlContainerForConditions = this.BuildSqlContainerForRequest (request, rootEntityAlias, entity);
			
			AbstractEntity requestedEntity = request.RequestedEntity;
			
			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, rootEntityAlias);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			SqlContainer sqlContainerForReferences = this.BuildSqlContainerForReferences (requestedAlias, requestedEntity);

			return sqlContainerForConditions.Plus (sqlContainerForReferences).BuildSqlSelect ();
		}


		#endregion


		#region GET COLLECTION DATA


		private Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, CollectionData> collectionData = new Dictionary<DbKey, CollectionData> ();

			AbstractEntity requestedEntity = request.RequestedEntity;
			Druid leafRequestedEntityId = requestedEntity.GetEntityStructuredTypeId ();

			var definedFieldIds = this.EntityContext.GetEntityFieldDefinitions (leafRequestedEntityId)
				.Where (f => f.Relation == FieldRelation.Collection)
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
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

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
			if (this.DataContext.IsForeignEntity (entity))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			SqlField sqlTableForRequestRootEntity = this.BuildTableForRootEntity (rootEntityAlias, entity);
			SqlContainer sqlContainerForEntity = this.BuildSqlContainerForEntity (request, rootEntityAlias, entity);

			return SqlContainer.CreateSqlTables (sqlTableForRequestRootEntity).Plus (sqlContainerForEntity);
		}


		private SqlField BuildTableForRootEntity(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			DbTable rootDbTable = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);

			return SqlField.CreateAliasedName (rootDbTable.GetSqlName (), rootEntityAlias.Alias);
		}


		private SqlContainer BuildSqlContainerForEntity(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			if (this.DataContext.IsForeignEntity (entity))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			SqlContainer sqlContainerForRequestRootEntityId = this.BuildSqlContainerForRequestRootEntityId (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForSubEntities = this.BuildSqlContainerForSubEntities (rootEntityAlias, entity);
			SqlContainer sqlContainerForFields = this.BuildSqlContainerForFields (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForLocalConstraints = this.BuildSqlContainerForLocalConstraints (request, rootEntityAlias, entity);

			return sqlContainerForRequestRootEntityId.Plus (sqlContainerForSubEntities).Plus (sqlContainerForFields).Plus (sqlContainerForLocalConstraints);
		}


		private SqlContainer BuildSqlContainerForRequestRootEntityId(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			DbKey? rootEntityKey = request.RootEntityKey;

			SqlContainer sqlContainer = SqlContainer.Empty;

			if (rootEntityKey.HasValue && entity == request.RootEntity)
			{
				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

				long id = rootEntityKey.Value.Id.Value;

				SqlFunction sqlCondition = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, Tags.ColumnId),
					SqlField.CreateConstant (id, DbRawType.Int64)
				);

				sqlContainer = sqlContainer.PlusSqlConditions (sqlCondition);
			}

			return sqlContainer;
		}


		private SqlContainer BuildSqlContainerForSubEntities(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			return this.EntityContext
				.GetInheritedEntityIds (leafEntityId)
				.Where (id => id != rootEntityId)
				.Reverse ()
				.Select (id => this.BuildJoinToSubEntity (rootEntityAlias, rootEntityId, id))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildJoinToSubEntity(AliasNode rootEntityAlias, Druid rootEntityId, Druid localEntityId)
		{
			AliasNode localEntityAlias = rootEntityAlias.CreateChild (localEntityId.ToResourceId ());

			DbTable rootDbTable = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);
			DbTable localDbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);

			DbColumn rootIdDbColumn = rootDbTable.Columns[Tags.ColumnId];
			DbColumn localIdDbColumn = localDbTable.Columns[Tags.ColumnId];

			SqlField sqlTable = SqlField.CreateAliasedName (localDbTable.GetSqlName (), localEntityAlias.Alias);

			SqlField rootIdColumn = SqlField.CreateAliasedName (rootEntityAlias.Alias, rootIdDbColumn.GetSqlName (), Tags.ColumnId);
			SqlField localIdColumn = SqlField.CreateAliasedName (localEntityAlias.Alias, localIdDbColumn.GetSqlName (), Tags.ColumnId);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (rootIdColumn, localIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForFields(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			return this.EntityContext
				.GetDefinedFieldIds (entity)
				.Select (id => Druid.Parse (id))
				.Select (id => this.BuildSqlContainerForField (entity, request, rootEntityAlias, id))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForField(AbstractEntity entity, Request request, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldId.ToResourceId ());

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
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, field.CaptionId);

			string columnName = this.SchemaEngine.GetEntityColumnName (field.CaptionId);

			object fieldValue = entity.InternalGetValue (field.CaptionId.ToResourceId ());

			AbstractType fieldType = (AbstractType) field.Type;

			SqlFunction sqlCondition;

			switch (fieldType.TypeCode)
			{
				case TypeCode.String:
					
					// TODO The call to FormattedText is not very nice, but it must be done in order
					// to allow research on regular text as well as on formatted text. This might
					// indicate some kind of design flow in how the text in entities are implemented.
					// So we can't much right here to improve the situation. The correction must be
					// done elsewhere first.
					// Marc

					string value = FormattedText.CastToString (fieldValue);

					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareLike,
						this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant (value, DbRawType.String)
					);
					break;

				case TypeCode.Decimal:
					throw new System.NotImplementedException ();

				case TypeCode.Double:
					throw new System.NotImplementedException ();

				case TypeCode.Integer:
					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant ((int) fieldValue, DbRawType.String)
					);
					break;

				case TypeCode.LongInteger:
					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant ((long) fieldValue, DbRawType.String)
					);
					break;

				case TypeCode.Boolean:
					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						this.BuildSqlFieldForEntityColumn(rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant ((bool) fieldValue, DbRawType.String)
					);
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

			return SqlContainer.CreateSqlConditions (sqlCondition);
		}


		private SqlContainer BuildSqlContainerForReferenceField(Request request, AliasNode rootEntityAlias, AbstractEntity entity, StructuredTypeField field)
		{
			AbstractEntity target = entity.GetField<AbstractEntity> (field.Id);

			return this.BuildSqlContainerForRelation (request, rootEntityAlias, entity, field, target);
		}


		private SqlContainer BuildSqlContainerForCollectionField(Request request, AliasNode rootEntityAlias, AbstractEntity entity, StructuredTypeField field)
		{
			return entity
				.GetFieldCollection<AbstractEntity> (field.Id)
				.Select (r => this.BuildSqlContainerForRelation (request, rootEntityAlias, entity, field, r))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForRelation(Request request, AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			if (this.DataContext.IsForeignEntity (target))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			if (this.DataContext.IsPersistent (target))
			{
				DbKey targetKey = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;

				return this.BuildSqlContainerForRelationByReference (rootSourceAlias, source, field, targetKey);
			}
			else
			{
				return this.BuildSqlContainerForRelationByValue (request, rootSourceAlias, source, field, target);
			}
		}


		private SqlContainer BuildSqlContainerForRelationByReference(AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, DbKey targetKey)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			long targetId = targetKey.Id.Value;

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToRelation(rootSourceAlias, localEntityId, fieldId, SqlJoinCode.Inner);
			
			SqlFunction conditionForRelationTargetId = this.BuildConditionForRelationTargetId (rootSourceAlias, localEntityId, fieldId, targetId);

			return joinToRelation.PlusSqlConditions (conditionForRelationTargetId);
		}


		private SqlContainer BuildSqlContainerForRelationByValue(Request request, AliasNode rootSourceAlias, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = source.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			Druid leafTargetId = target.GetEntityStructuredTypeId ();
			Druid rootTargetId = this.EntityContext.GetRootEntityId (leafTargetId);

			SqlContainer joinToRelation = this.BuildSqlContainerForJoinToRelation(rootSourceAlias, localEntityId, fieldId, SqlJoinCode.Inner);
			AliasNode relationAlias = rootSourceAlias.GetChildren (field.Id).Last ();

			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToTarget (relationAlias, localEntityId, fieldId, rootTargetId);
			AliasNode targetAlias = relationAlias.GetChild (rootTargetId.ToResourceId ());

			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, targetAlias, target);

			return joinToRelation.Plus (joinToTarget).Plus (containerForTarget);
		}


		private SqlContainer BuildSqlContainerForJoinToRelation(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, SqlJoinCode sqlJoinCode)
		{
			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);

			AliasNode relationAlias = rootEntityAlias.CreateChild (fieldId.ToResourceId ());

			DbTable rootSourceDbTable = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);
			DbTable relationDbTable = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);

			DbColumn sourceIdDbColumn = rootSourceDbTable.Columns[Tags.ColumnId];
			DbColumn relationSourceIdDbColumn = relationDbTable.Columns[Tags.ColumnRefSourceId];

			SqlField sqlTable = SqlField.CreateAliasedName (relationDbTable.GetSqlName (), relationAlias.Alias);

			SqlField sourceIdColumn = SqlField.CreateAliasedName (rootEntityAlias.Alias, sourceIdDbColumn.GetSqlName (), Tags.ColumnId);
			SqlField relationSourceIdColumn = SqlField.CreateAliasedName (relationAlias.Alias, relationSourceIdDbColumn.GetSqlName (), Tags.ColumnRefSourceId);
			
			SqlJoin sqlJoin = new SqlJoin (sourceIdColumn, relationSourceIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlContainer BuildSqlContainerForJoinToTarget(AliasNode relationAlias, Druid localEntityId, Druid fieldId, Druid rootTargetId)
		{
			AliasNode rootTargetAlias = relationAlias.CreateChild (rootTargetId.ToResourceId ());

			DbTable relationDbTable = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			DbTable rootTargetDbTable = this.SchemaEngine.GetEntityTableDefinition (rootTargetId);

			DbColumn relationTargetIdDbColumn = relationDbTable.Columns[Tags.ColumnRefTargetId];
			DbColumn targetIdDbColumn = rootTargetDbTable.Columns[Tags.ColumnId];

			SqlField sqlTable = SqlField.CreateAliasedName (rootTargetDbTable.GetSqlName (), rootTargetAlias.Alias);

			SqlField relationTargetIdColumn = SqlField.CreateAliasedName (relationAlias.Alias, relationTargetIdDbColumn.GetSqlName (), Tags.ColumnRefTargetId);
			SqlField targetIdColumn = SqlField.CreateAliasedName (rootTargetAlias.Alias, targetIdDbColumn.GetSqlName (), Tags.ColumnId);

			SqlJoinCode sqlJoinCode = SqlJoinCode.Inner;

			SqlJoin sqlJoin = new SqlJoin (relationTargetIdColumn, targetIdColumn, sqlJoinCode);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlFunction BuildConditionForRelationTargetId(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, long targetId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, Tags.ColumnRefTargetId),
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
					Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);
					
					string columnName = this.SchemaEngine.GetEntityColumnName (fieldId);

					return this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName);
				}
			);
		}


#endregion


		#region VALUE QUERY GENERATION


		private SqlContainer BuildSqlContainerForValues(AliasNode rootEntityAlias, AbstractEntity entity, bool returnBinaryBlobs)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			SqlContainer sqlContainerForJoinToLog = this.BuildSqlContainerForJoinToLog (rootEntityAlias, rootEntityId);

			return this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.Where (field => returnBinaryBlobs || field.Type.SystemType != typeof (byte[]))
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForValueField (rootEntityAlias, entity, field))
				.Append (this.BuildSqlFieldForLogSequenceNumber (rootEntityAlias))
				.Append (this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, Tags.ColumnInstanceType))
				.Append (this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, Tags.ColumnId))
				.Aggregate (sqlContainerForJoinToLog, (acc, e) => acc.PlusSqlFields (e));
		}


		private SqlField BuildSqlFieldForValueField(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			string fieldName = this.SchemaEngine.GetEntityColumnName (fieldId);

			return this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, fieldName);
		}


		private SqlContainer BuildSqlContainerForJoinToLog(AliasNode entityAlias, Druid rootEntityId)
		{
			AliasNode logAlias = entityAlias.CreateChild ("log");

			DbTable entityDbTable = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);
			DbTable logDbTable = this.DbInfrastructure.ResolveDbTable (Tags.TableLog);

			DbColumn entityLogIdDbColumn = entityDbTable.Columns[Tags.ColumnRefLog];
			DbColumn logLogIdDbColumn = logDbTable.Columns[Tags.ColumnId];

			SqlField sqlTable = SqlField.CreateAliasedName (logDbTable.GetSqlName (), logAlias.Alias);

			SqlField entityLogIdColumn = SqlField.CreateAliasedName (entityAlias.Alias, entityLogIdDbColumn.GetSqlName (), Tags.ColumnRefLog);
			SqlField logLogIdColumn = SqlField.CreateAliasedName (logAlias.Alias, logLogIdDbColumn.GetSqlName (), Tags.ColumnId);

			SqlJoin sqlJoin = new SqlJoin (entityLogIdColumn, logLogIdColumn, SqlJoinCode.Inner);

			return SqlContainer.CreateSqlTables (sqlTable).PlusSqlJoins (sqlJoin);
		}


		private SqlField BuildSqlFieldForLogSequenceNumber(AliasNode entityAlias)
		{
			AliasNode logAlias = entityAlias.GetChild ("log");

			DbTable dbTable = this.DbInfrastructure.ResolveDbTable (Tags.TableLog);
			DbColumn dbColumn = dbTable.Columns[Tags.ColumnSequenceNumber];

			return SqlField.CreateAliasedName (logAlias.Alias, dbColumn.GetSqlName (), Tags.ColumnSequenceNumber);
		}


		#endregion


		#region REFERENCE QUERY GENERATION


		private SqlContainer BuildSqlContainerForReferences(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			return this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.Select (field => this.BuildSqlFieldForReferenceField (rootEntityAlias, entity, field))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e))
				.PlusSqlFields (this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, Tags.ColumnId));
		}


		private SqlContainer BuildSqlFieldForReferenceField(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			SqlContainer sqlContainerForRelationJoin = SqlContainer.Empty;

			if (!this.EntityContext.IsFieldDefined (fieldId.ToResourceId (), entity))
			{
				sqlContainerForRelationJoin = this.BuildSqlContainerForJoinToRelation (rootEntityAlias, localEntityId, fieldId, SqlJoinCode.OuterLeft);
			}

			SqlField sqlFieldForRelation = this.BuildSqlFieldForRelationColumn(rootEntityAlias, localEntityId, fieldId, Tags.ColumnRefTargetId);

			return sqlContainerForRelationJoin.PlusSqlFields (sqlFieldForRelation);
		}


		#endregion


		#region COLLECTION QUERY GENERATION


		private SqlContainer BuildSqlContainerForCollection(AliasNode rootEntityAlias, AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);
			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);

			SqlContainer sqlContainerForRelationJoin = this.BuildSqlContainerForJoinToRelation (rootEntityAlias, localEntityId, fieldId, SqlJoinCode.Inner);

			SqlField sqlFieldForTargetId = this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, Tags.ColumnRefTargetId);
			SqlField sqlFieldForRank = this.BuildSqlFieldForRelationColumn (rootEntityAlias, localEntityId, fieldId, Tags.ColumnRefRank);
			SqlField sqlFieldForSourceId = this.BuildSqlFieldForEntityColumn (rootEntityAlias, rootEntityId, Tags.ColumnId);

			sqlFieldForRank.SortOrder = SqlSortOrder.Ascending;

			return sqlContainerForRelationJoin.PlusSqlFields (sqlFieldForTargetId, sqlFieldForRank, sqlFieldForSourceId);
		}


		#endregion


		#region MISC


		private SqlField BuildSqlFieldForEntityColumn(AliasNode rootEntityAlias, Druid localEntityId, string columnName)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId);

			DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return SqlField.CreateAliasedName (localAlias.Alias, dbColumn.GetSqlName (), columnName);
		}


		private SqlField BuildSqlFieldForRelationColumn(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId, string columnName)
		{
			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			DbTable dbTable = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
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
				List<string> definedFieldIds = this.EntityContext.GetDefinedFieldIds (entity).ToList ();

				for (int i = 0; i < definedFieldIds.Count && requestedEntityAlias == null; i++)
				{
					string fieldId = definedFieldIds[i];
					StructuredTypeField field = this.EntityContext.GetEntityFieldDefinition (leafEntityId, fieldId);

					switch (field.Relation)
					{
						case FieldRelation.Reference:
							{
								AbstractEntity targetEntity = entity.GetField<AbstractEntity> (fieldId);

								Druid leafTargetId = targetEntity.GetEntityStructuredTypeId ();
								Druid rootTargetId = this.EntityContext.GetRootEntityId (leafTargetId);

								AliasNode targetAlias = entityAliasNode.GetChild (fieldId).GetChild (rootTargetId.ToResourceId ());

								requestedEntityAlias = this.RetreiveRequestedEntityAliasRec (targetEntity, requestedEntity, targetAlias);

								break;
							}
						case FieldRelation.Collection:
							{

								IList<AbstractEntity> targetEntities = entity.GetFieldCollection<AbstractEntity> (fieldId);
								ReadOnlyCollection<AliasNode> targetAliases = entityAliasNode.GetChildren (fieldId);

								for (int j = 0; j < targetEntities.Count && requestedEntityAlias == null; j++)
								{
									AbstractEntity targetEntity = targetEntities[j];

									Druid leafTargetId = targetEntity.GetEntityStructuredTypeId ();
									Druid rootTargetId = this.EntityContext.GetRootEntityId (leafTargetId);

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
