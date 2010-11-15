//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

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
			this.ExpressionConverter = new ExpressionConverter (dataContext);
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private ExpressionConverter ExpressionConverter
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
			// HACK This method acts as a switch between the old and the new way of generating
			// sql requests to the database. The new way might contain some bugs, therefore I left
			// the code for the old way and you can switch which way you want to take. You should
			// use the new way, but if this creates a bug, tell me of it and use the old way until
			// I correct the bug. If you modify this here, modify it also in the three other places.
			// Marc

			// Old version of the loader query generator.
			// Should be reliable.
			
			//return this.GetEntitiesDataOld (request);
			
			// New version of the loader query generator.
			// Might contain some bugs.
			
			return this.GetEntitiesDataNew (request);
		}


		private IEnumerable<EntityData> GetEntitiesDataOld(Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valuesData;
			Dictionary<DbKey, ReferenceData> referencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (DbTransaction innerTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (innerTransaction, request);
				referencesData = this.GetReferenceData (innerTransaction, request);
				collectionsData = this.GetCollectionData (innerTransaction, request);

				innerTransaction.Commit ();
			}

			foreach (DbKey rowKey in valuesData.Keys)
			{
				Druid loadedEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();
				Druid leafEntityId = valuesData[rowKey].Item1;
				ValueData entityValueData = valuesData[rowKey].Item3;
				ReferenceData entityReferenceData = referencesData[rowKey];
				CollectionData entityCollectionData = collectionsData.ContainsKey (rowKey) ? collectionsData[rowKey] : new CollectionData ();

				yield return new EntityData (rowKey, leafEntityId, loadedEntityId, 0, entityValueData, entityReferenceData, entityCollectionData);
			}
		}


		private IEnumerable<EntityData> GetEntitiesDataNew(Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valuesData;
			Dictionary<DbKey, ReferenceData> referencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (DbTransaction innerTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				valuesData = this.GetValueData (innerTransaction, request);
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
			// is not as overkill (and overslow?).

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
				valuesData = this.GetValueData (transaction, request);

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
				targetsData = this.GetCollectionData (transaction, entity, fieldId)
					.ToDictionary (data => data.RowKey, data => data);

				targetKeys = this.GetCollectionKeys (transaction, entity, fieldId)
					.Select (d => d.Item2).ToList ();
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


		private IEnumerable<EntityData> GetCollectionData(DbTransaction transaction, AbstractEntity entity, Druid fieldId)
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


		public Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> GetValueData(DbTransaction transaction, Request request)
		{
			// HACK This method acts as a switch between the old and the new way of generating
			// sql requests to the database. The new way might contain some bugs, therefore I left
			// the code for the old way and you can switch which way you want to take. You should
			// use the new way, but if this creates a bug, tell me of it and use the old way until
			// I correct the bug. If you modify this here, modify it also in the three other places.
			// Marc

			// Old version of the loader query generator.
			// Should be reliable.

			//return this.GetValueDataOld (transaction, request);

			// New version of the loader query generator.
			// Might contain some bugs.

			return this.GetValueDataNew (transaction, request);
		}


		private Dictionary<DbKey, System.Tuple<Druid, ValueData>> GetValueDataOld(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, ValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, ValueData>> ();

			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var fields = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.OrderBy (field => field.CaptionId.ToResourceId ())
				.ToList ();

			using (DbReader reader = this.CreateValuesReader (request))
			{
				reader.CreateDataReader (transaction);

				foreach (DbReader.RowData rowData in reader.Rows)
				{
					ValueData entityValueData = new ValueData ();
					Druid realEntityId = Druid.FromLong ((long) rowData.Values[rowData.Values.Length - 2]);
					DbKey entityKey = new DbKey (new DbId ((long) rowData.Values[rowData.Values.Length - 1]));

					for (int i = 0; i < rowData.Values.Length - 2; i++)
					{
						object internalValue = rowData.Values[i];

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

			AbstractEntity requestedEntity = request.RequestedEntity;
			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, entityRootAliasNode);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			this.AddQueryForValues (reader, requestedEntity, requestedAlias);

			return reader;
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


		private Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> GetValueDataNew(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> valueData = new Dictionary<DbKey, System.Tuple<Druid, long, ValueData>> ();

			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var fields = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
				.OrderBy (field => field.CaptionId.ToResourceId ())
				.ToList ();

			SqlSelect select = this.CreateSqlSelectForValueData (request);

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


		private SqlSelect CreateSqlSelectForValueData(Request request)
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

			SqlContainer sqlContainerForValues = this.BuildSqlContainerForValues (requestedAlias, requestedEntity);

			return sqlContainerForConditions.Plus (sqlContainerForValues).BuildSqlSelect ();
		}


		#endregion


		#region GET REFERENCE DATA


		public Dictionary<DbKey, ReferenceData> GetReferenceData(DbTransaction transaction, Request request)
		{
			// HACK This method acts as a switch between the old and the new way of generating
			// sql requests to the database. The new way might contain some bugs, therefore I left
			// the code for the old way and you can switch which way you want to take. You should
			// use the new way, but if this creates a bug, tell me of it and use the old way until
			// I correct the bug. If you modify this here, modify it also in the three other places.
			// Marc

			// Old version of the loader query generator.
			// Should be reliable.

			//return this.GetReferenceDataOld (transaction, request);

			// New version of the loader query generator.
			// Might contain some bugs.

			return this.GetReferenceDataNew (transaction, request);
		}
		
		
		private Dictionary<DbKey, ReferenceData> GetReferenceDataOld(DbTransaction transaction, Request request)
		{
			Dictionary<DbKey, ReferenceData> references = new Dictionary<DbKey, ReferenceData> ();

			Druid leafEntityId = request.RequestedEntity.GetEntityStructuredTypeId ();

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

					ReferenceData entityReferenceData = new ReferenceData ();

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

			AbstractEntity requestedEntity = request.RequestedEntity;
			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, entityRootAliasNode);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			this.AddQueryForReferences (dbReader, requestedEntity, requestedAlias);

			return dbReader;
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


		private Dictionary<DbKey, ReferenceData> GetReferenceDataNew(DbTransaction transaction, Request request)
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


		public Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			// HACK This method acts as a switch between the old and the new way of generating
			// sql requests to the database. The new way might contain some bugs, therefore I left
			// the code for the old way and you can switch which way you want to take. You should
			// use the new way, but if this creates a bug, tell me of it and use the old way until
			// I correct the bug. If you modify this here, modify it also in the three other places.
			// Marc

			// Old version of the loader query generator.
			// Should be reliable.

			//return this.GetCollectionDataOld (transaction, request);

			// New version of the loader query generator.
			// Might contain some bugs.

			return this.GetCollectionDataNew (transaction, request);
		}


		private Dictionary<DbKey, CollectionData> GetCollectionDataOld(DbTransaction transaction, Request request)
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
			using (DbReader reader = this.CreateCollectionReader (request, fieldId))
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


		private DbReader CreateCollectionReader(Request request, Druid fieldId)
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

			AbstractEntity requestedEntity = request.RequestedEntity;

			AliasNode requestedAlias = this.RetreiveRequestedEntityAlias (request, entityRootAliasNode);

			if (requestedAlias == null)
			{
				throw new System.Exception ("Requested entity not found.");
			}

			this.AddQueryForCollection (reader, requestedEntity, requestedAlias, fieldId);
			this.AddSortOrderOnRank (reader, requestedEntity, requestedAlias, fieldId);

			return reader;
		}
		
		
		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


		private Dictionary<DbKey, CollectionData> GetCollectionDataNew(DbTransaction transaction, Request request)
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
				foreach (System.Tuple<DbKey, DbKey> relation in this.GetCollectionData2 (transaction, request, fieldId))
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


		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData2(DbTransaction transaction, Request request, Druid fieldId)
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

		
		private void AddConditionsForEntity(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias)
		{
			if (this.DataContext.IsForeignEntity (entity))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			this.AddConditionForRootEntityStatus (dbReader, entity, rootEntityAlias);
			this.AddConditionForRootEntityId (dbReader, entity, request, rootEntityAlias);
			this.AddJoinToSubEntities (dbReader, entity, rootEntityAlias);
			this.AddConditionForFields (dbReader, entity, request, rootEntityAlias);
			this.AddConditionsForLocalConstraints (dbReader, entity, request, rootEntityAlias);
		}


		private void AddConditionForRootEntityStatus(DbReader dbReader, AbstractEntity entity, AliasNode rootAliasNode)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			this.AddConditionForEntityStatus (dbReader, rootAliasNode, rootEntityId);
		}


		private void AddConditionForEntityStatus(DbReader dbReader, AliasNode rootAliasNode, Druid localEntityId)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootAliasNode, localEntityId);

			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, localAlias.Alias, Tags.ColumnStatus);

			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = DbSimpleCondition.CreateCondition (tableColumn, DbSimpleConditionOperator.Equal, (short) DbRowStatus.Live),
			};

			dbReader.AddCondition (condition);
		}


		private void AddConditionForRootEntityId(DbReader dbReader, AbstractEntity entity, Request request, AliasNode rootEntityAlias)
		{
			DbKey? rootEntityKey = request.RootEntityKey;

			if (rootEntityKey.HasValue && entity == request.RootEntity)
			{
				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

				long id = rootEntityKey.Value.Id.Value;
				DbTableColumn columnId = this.GetEntityColumn (rootEntityId, rootEntityAlias.Alias, Tags.ColumnId);

				DbSelectCondition condition = new DbSelectCondition ()
				{
					Condition = DbSimpleCondition.CreateCondition (columnId, DbSimpleConditionOperator.Equal, id),
				};

				dbReader.AddCondition (condition);
			}
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
				this.AddJoinToSubEntity (dbReader, rootEntityId, localEntityId, rootEntityAlias);
			}
		}


		private void AddJoinToSubEntity(DbReader dbReader, Druid rootEntityId, Druid localEntityId, AliasNode rootEntityAlias)
		{
			AliasNode localEntityAlias = rootEntityAlias.CreateChild (localEntityId.ToResourceId ());

			string rootTableAlias = rootEntityAlias.Alias;
			string localTableAlias = localEntityAlias.Alias;

			DbTableColumn localEntityIdColumn = this.GetEntityColumn (localEntityId, localTableAlias, Tags.ColumnId);
			DbTableColumn localEntityStatusColumn = this.GetEntityColumn (localEntityId, localTableAlias, Tags.ColumnStatus);
			DbTableColumn rootEntityIdColumn = this.GetEntityColumn (rootEntityId, rootTableAlias, Tags.ColumnId);

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

			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, alias, field.CaptionId);

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
			if (this.DataContext.IsForeignEntity (target))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			if (this.DataContext.IsPersistent (target))
			{
				DbKey key = this.DataContext.GetNormalizedEntityKey (target).Value.RowKey;
				this.AddConditionForRelationByReference (dbReader, entity, rootEntityAlias, field, key);
			}
			else
			{
				this.AddConditionForRelationByValue (dbReader, entity, request, rootEntityAlias, field, target);
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


		private void AddConditionForRelationByReference(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, StructuredTypeField field, DbKey key)
		{
			Druid fieldId = field.CaptionId;

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			long targetId = key.Id.Value;

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

			DbTableColumn idColumn = this.GetRelationColumn (localEntityId, fieldId, relationAlias.Alias, Tags.ColumnRefTargetId);

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

			DbTableColumn sourceIdColumn = this.GetEntityColumn (rootEntityId, entityTableAlias, Tags.ColumnId);
			DbTableColumn relationSourceIdColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, Tags.ColumnRefSourceId);
			DbTableColumn relationSourceStatusColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, Tags.ColumnStatus);

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

			DbTableColumn relationTargetIdColumn = this.GetRelationColumn (localEntityId, fieldId, relationTableAlias, Tags.ColumnRefTargetId);
			DbTableColumn targetIdColumn = this.GetEntityColumn (rootTargetId, targetTableAlias, Tags.ColumnId);
			DbTableColumn targetStatusColumn = this.GetEntityColumn (rootTargetId, targetTableAlias, Tags.ColumnStatus);

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
				Condition = constraint.CreateDbCondition (this.ExpressionConverter, fieldId =>
				{
					Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

					string tableAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId).Alias;

					return this.GetEntityColumn (localEntityId, tableAlias, fieldId);
				}),
			};

			dbReader.AddCondition (selectCondition);
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


		private SqlContainer BuildSqlContainerForRequest(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			if (this.DataContext.IsForeignEntity (entity))
			{
				throw new System.InvalidOperationException ("Usage of a foreign entity in a request is not allowed.");
			}

			SqlContainer sqlContainerForRequestRootEntity = this.BuildSqlContainerForRequestRootEntity (request, rootEntityAlias, entity);
			SqlContainer sqlContainerForEntity = this.BuildSqlContainerForEntity (request, rootEntityAlias, entity);

			return sqlContainerForRequestRootEntity.Plus (sqlContainerForEntity);
		}


		private SqlContainer BuildSqlContainerForRequestRootEntity(Request request, AliasNode rootEntityAlias, AbstractEntity entity)
		{
			SqlField tableForRootEntity = this.BuildTableForRootEntity (rootEntityAlias, entity);
			SqlFunction conditionForRootEntityStatus = this.BuildConditionForRootEntityStatus (rootEntityAlias, entity);
			
			return SqlContainer.CreateSqlTables (tableForRootEntity).PlusSqlConditions (conditionForRootEntityStatus);
		}


		private SqlField BuildTableForRootEntity(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			DbTable rootDbTable = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);

			return SqlField.CreateAliasedName (rootDbTable.GetSqlName (), rootEntityAlias.Alias);
		}


		private SqlFunction BuildConditionForRootEntityStatus(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			return this.BuildConditionForEntityStatus (rootEntityAlias, rootEntityId);
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
				.Select (id => this.BuildSqlContainerForSubEntity (rootEntityAlias, rootEntityId, id))
				.Aggregate (SqlContainer.Empty, (acc, e) => acc.Plus (e));
		}


		private SqlContainer BuildSqlContainerForSubEntity(AliasNode rootEntityAlias, Druid rootEntityId, Druid localEntityId)
		{
			SqlContainer joinToSubEntity = this.BuildJoinToSubEntity (rootEntityAlias, rootEntityId, localEntityId);
			SqlFunction conditionForSubEntityStatus = this.BuildConditionForEntityStatus (rootEntityAlias, localEntityId);

			return joinToSubEntity.PlusSqlConditions (conditionForSubEntityStatus);
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


		private SqlFunction BuildConditionForEntityStatus(AliasNode rootEntityAlias, Druid localEntityId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, Tags.ColumnStatus),
				SqlField.CreateConstant ((short) DbRowStatus.Live, DbRawType.Int16)
			);
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
					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareLike,
						this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant ((string) fieldValue, DbRawType.String)
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
						SqlField.CreateConstant ((int) fieldValue, DbRawType.String)
					);
					break;

				case TypeCode.Boolean:
					sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						this.BuildSqlFieldForEntityColumn(rootEntityAlias, localEntityId, columnName),
						SqlField.CreateConstant ((int) fieldValue, DbRawType.String)
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
			
			SqlFunction conditionForRelationStatus = this.BuildConditionForRelationStatus (rootSourceAlias, localEntityId, fieldId);
			SqlFunction conditionForRelationTargetId = this.BuildConditionForRelationTargetId (rootSourceAlias, localEntityId, fieldId, targetId);

			return joinToRelation.PlusSqlConditions (conditionForRelationStatus, conditionForRelationTargetId);
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

			SqlFunction conditionForRelationStatus = this.BuildConditionForRelationStatus (rootSourceAlias, localEntityId, fieldId);

			SqlContainer joinToTarget = this.BuildSqlContainerForJoinToTarget (relationAlias, localEntityId, fieldId, rootTargetId);
			AliasNode targetAlias = relationAlias.GetChild (rootTargetId.ToResourceId ());

			SqlFunction conditionForTargetStatus = this.BuildConditionForRootEntityStatus (targetAlias, target);

			SqlContainer containerForTarget = this.BuildSqlContainerForEntity (request, targetAlias, target);

			return joinToRelation.PlusSqlConditions (conditionForRelationStatus).Plus (joinToTarget).PlusSqlConditions(conditionForTargetStatus).Plus (containerForTarget);
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


		private SqlFunction BuildConditionForRelationStatus(AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				this.BuildSqlFieldForEntityColumn (rootEntityAlias, localEntityId, Tags.ColumnStatus),
				SqlField.CreateConstant ((short) DbRowStatus.Live, DbRawType.Int16)
			);
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

				this.AddEntityQueryField (dbReader, rootEntityAlias, localEntityId, fieldId);
			}

			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, Tags.ColumnInstanceType);
			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, Tags.ColumnId);
		}


		private void AddEntityQueryField(DbReader dbReader, AliasNode rootEntityAlias, Druid localEntityId, Druid fieldId)
		{
			string columnName = this.SchemaEngine.GetEntityColumnName (fieldId);

			this.AddEntityQueryField (dbReader, rootEntityAlias, localEntityId, columnName);
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


		private SqlContainer BuildSqlContainerForValues(AliasNode rootEntityAlias, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			SqlContainer sqlContainerForJoinToLog = this.BuildSqlContainerForJoinToLog (rootEntityAlias, rootEntityId);

			return this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.None)
				.Where (field => field.Source == FieldSource.Value)
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


		private void AddQueryForReferences(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			var relationFieldIds = this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				.Where (field => field.Relation == FieldRelation.Reference)
				.Where (field => field.Source == FieldSource.Value)
				.Select (field => field.CaptionId)
				.OrderBy (field => field.ToResourceId ())
				.ToList ();

			foreach (Druid fieldId in relationFieldIds)
			{
				this.AddQueryForReference (dbReader, entity, rootEntityAlias, fieldId);
			}

			this.AddEntityQueryField (dbReader, rootEntityAlias, rootEntityId, Tags.ColumnId);
		}


		private void AddQueryForReference(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			if (!this.EntityContext.IsFieldDefined (fieldId.ToResourceId (), entity))
			{
				this.AddJoinToRelation (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.OuterLeft);
			}

			AliasNode relationAlias = rootEntityAlias.GetChild (fieldId.ToResourceId ());

			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, Tags.ColumnRefTargetId);
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


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


		private void AddQueryForCollection(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			this.AddJoinToRelation (dbReader, localEntityId, fieldId, rootEntityAlias, SqlJoinCode.Inner);

			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, Tags.ColumnRefTargetId);
			this.AddRelationQueryField (dbReader, relationAlias.Alias, localEntityId, fieldId, Tags.ColumnRefRank);

			this.AddEntityQueryField (dbReader, rootEntityAlias, this.EntityContext.GetRootEntityId (localEntityId), Tags.ColumnId);
		}


		private void AddSortOrderOnRank(DbReader dbReader, AbstractEntity entity, AliasNode rootEntityAlias, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AliasNode relationAlias = rootEntityAlias.GetChildren (fieldId.ToResourceId ()).Last ();

			DbTableColumn column = this.GetRelationColumn (localEntityId, fieldId, relationAlias.Alias, Tags.ColumnRefRank);

			dbReader.AddSortOrder (column, SqlSortOrder.Ascending);
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\


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


		private void AddEntityQueryField(DbReader dbReader, AliasNode rootEntityAlias, Druid localEntityId, string columnName)
		{
			AliasNode localAlias = this.GetLocalEntityAlias (rootEntityAlias, localEntityId);
			DbTableColumn tableColumn = this.GetEntityColumn (localEntityId, localAlias.Alias, columnName);

			dbReader.AddQueryField (tableColumn);
		}


		private DbTableColumn GetEntityColumn(Druid entityId, string entityAlias, Druid fieldId)
		{
			string columName = this.SchemaEngine.GetEntityColumnName (fieldId);

			return this.GetEntityColumn (entityId, entityAlias, columName);
		}


		private DbTableColumn GetEntityColumn(Druid entityId, string entityAlias, string columName)
		{
			DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (entityId);
			DbColumn dbColumn = dbTable.Columns[columName];

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
			DbTable dbRelationTable = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			DbColumn dbColumn = dbRelationTable.Columns[columName];

			return new DbTableColumn (dbColumn)
			{
				TableAlias = relationAlias,
				ColumnAlias = columName,
			};
		}


		// ======================================================================== \\
		// ======================================================================== \\
		// ============================= SEPARATION =============================== \\
		// ======================================================================== \\
		// ======================================================================== \\

		
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
