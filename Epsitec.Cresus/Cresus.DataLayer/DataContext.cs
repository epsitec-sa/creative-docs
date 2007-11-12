//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public sealed partial class DataContext : System.IDisposable
	{
		public DataContext(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.richCommand = new DbRichCommand (this.infrastructure);
			this.schemaEngine = new SchemaEngine (this.infrastructure);
			this.entityContext = EntityContext.Current;
			this.entityDataMapping = new Dictionary<long, EntityDataMapping> ();
			this.entityTableDefinitions = new Dictionary<Druid, DbTable> ();

			this.entityContext.EntityCreated += this.HandleEntityCreated;
		}

		public SchemaEngine SchemaEngine
		{
			get
			{
				return this.schemaEngine;
			}
		}

		public EntityContext EntityContext
		{
			get
			{
				return this.entityContext;
			}
		}

		public DbRichCommand RichCommand
		{
			get
			{
				return this.richCommand;
			}
		}

		/// <summary>
		/// Counts the managed entities.
		/// </summary>
		/// <returns>The number of entities associated to this data context.</returns>
		public int CountManagedEntities()
		{
			return this.entityDataMapping.Count;
		}

		public void SaveChanges()
		{
			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.richCommand.SaveTables (transaction);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Serializes the entity to the in-memory data set. This will either
		/// update or create data rows in one or several data tables.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void SerializeEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);
			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid id = entityId;
			DbKey rowKey = mapping.RowKey;
			bool createRow;

			if (rowKey.IsEmpty)
			{
				rowKey = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
				mapping.RowKey = rowKey;
				createRow = true;
			}
			else
			{
				createRow = false;
			}
			

			while (id.IsValid)
			{
				StructuredType entityType = this.entityContext.GetStructuredType (id) as StructuredType;
				Druid          baseTypeId = entityType.BaseTypeId;

				System.Diagnostics.Debug.Assert (entityType != null);
				System.Diagnostics.Debug.Assert (entityType.CaptionId == id);

				//	Either create and fill a new row in the database for this entity
				//	or use and update an existing row.

				System.Data.DataRow dataRow = createRow ? this.CreateDataRow (mapping, id) : this.FindDataRow (mapping, id);
				
				dataRow.BeginEdit ();

				//	If this is the root entity in the entity hierarchy (it has no base
				//	type), then we will have to save the instance type identifying the
				//	entity.
				
				if (baseTypeId.IsEmpty)
				{
					dataRow[Tags.ColumnInstanceType] = this.GetInstanceTypeValue (entityId);
				}

				this.SerializeEntityLocal (entity, dataRow, id);
				
				dataRow.EndEdit ();

				id = baseTypeId;
			}
		}

		/// <summary>
		/// Serializes fields local to the specified entity into a given data
		/// row.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="dataRow">The data row.</param>
		/// <param name="entityId">The entity id.</param>
		private void SerializeEntityLocal(AbstractEntity entity, System.Data.DataRow dataRow, Druid entityId)
		{
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityFieldDefinitions (entityId))
			{
				//	Only process fields which are defined locally, since inherited
				//	fields do not belong into the same data table.

				if (fieldDef.Membership == FieldMembership.Inherited)
				{
					continue;
				}
				
				//	Depending on the relation (and therefore cardinality), write
				//	the data into the row :
				
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.WriteFieldValueInDataRow (entity, fieldDef, dataRow);
						break;

					default:
						//	TODO: ...
						break;
				}
			}
		}

		private AbstractEntity DeserializeEntity(System.Data.DataRow dataRow)
		{
			DbKey entityKey = new DbKey (dataRow);
			long typeValueId = (long) dataRow[Tags.ColumnInstanceType];
			Druid entityId = Druid.FromLong (typeValueId);
			AbstractEntity entity = this.entityContext.CreateEmptyEntity (entityId);

			using (entity.DefineOriginalValues ())
			{
				this.DeserializeEntity (entity, entityId, entityKey, dataRow);
			}
			
			return entity;
		}

		private void DeserializeEntity(AbstractEntity entity, Druid entityId, DbKey entityKey, System.Data.DataRow dataRow)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);
			Druid id = entityId;

			while (id.IsValid)
			{
				StructuredType entityType = this.entityContext.GetStructuredType (id) as StructuredType;
				Druid baseTypeId = entityType.BaseTypeId;

				System.Diagnostics.Debug.Assert (entityType != null);
				System.Diagnostics.Debug.Assert (entityType.CaptionId == id);

#if false
				this.FindDataRow (mapping, 

				DbKey rowKey  = mapping[id];
				System.Data.DataRow dataRow = rowKey.IsEmpty ? this.CreateDataRow (mapping, id) : this.FindDataRow (mapping, id);

				dataRow.BeginEdit ();

				//	If this is the root entity in the entity hierarchy (it has no base
				//	type), then we will have to save the instance type identifying the
				//	entity.

				if (baseTypeId.IsEmpty)
				{
					dataRow[Tags.ColumnInstanceType] = this.GetInstanceTypeValue (entityId);
				}

				this.SerializeEntityLocal (entity, dataRow, id);

				dataRow.EndEdit ();
#endif
				id = baseTypeId;
			}
		}

		private object GetInstanceTypeValue(Druid entityId)
		{
			return entityId.ToLong ();
		}

		private void WriteFieldValueInDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			object value = entity.InternalGetValue (fieldDef.Id);

			System.Diagnostics.Debug.Assert (!UnknownValue.IsUnknownValue (value));

			AbstractType  fieldType = fieldDef.Type as AbstractType;
			INullableType nullableType = fieldDef.GetNullableType ();

			if ((UndefinedValue.IsUndefinedValue (value)) ||
				(nullableType.IsNullValue (value)))
			{
				if (nullableType.IsNullable)
				{
					value = System.DBNull.Value;
				}
				else
				{
					value = fieldType.DefaultValue;
				}
			}

			string columnName = this.GetDataColumnName (fieldDef);

			dataRow[columnName] = value;
		}

		private System.Data.DataRow FindDataRow(EntityDataMapping mapping, Druid entityId)
		{
			System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsEmpty == false);
			
			string tableName = this.GetDataTableName (entityId);
			return this.richCommand.FindRow (tableName, mapping.RowKey.Id);
		}

		private System.Data.DataRow CreateDataRow(EntityDataMapping mapping, Druid entityId)
		{
			System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsTemporary);

			DbKey rowKey = mapping.RowKey;

			string tableName = this.GetDataTableName (entityId);
			System.Data.DataRow row = this.richCommand.CreateRow (tableName);

			row.BeginEdit ();
			DbKey.SetRowId (row, rowKey.Id);
			DbKey.SetRowStatus (row, rowKey.Status);
			row.EndEdit ();
			
			return row;
		}
		
		private string GetDataTableName(Druid entityId)
		{
			DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);
			return tableDef == null ? null : tableDef.Name;
		}

		private string GetDataColumnName(StructuredTypeField field)
		{
			System.Diagnostics.Debug.Assert (field.Id.StartsWith ("["));
			System.Diagnostics.Debug.Assert (field.Id.EndsWith ("]"));

			string fieldName = field.Id;
			string nakedName = fieldName.Substring (1, fieldName.Length-2);

			return nakedName;
		}

		public EntityDataMapping GetEntityDataMapping(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			EntityDataMapping mapping;

			if (this.entityDataMapping.TryGetValue (entity.GetEntitySerialId (), out mapping))
			{
				return mapping;
			}
			else
			{
				throw new System.ArgumentException ("Entity not managed by the DataContext");
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Dipose (true);
		}

		#endregion

		private void Dipose(bool disposing)
		{
			if (disposing)
			{
				this.entityContext.EntityCreated -= this.HandleEntityCreated;
			}
		}

		private void HandleEntityCreated(object sender, EntityEventArgs e)
		{
			AbstractEntity entity = e.Entity;
			Druid entityId = entity.GetEntityStructuredTypeId ();
			long entitySerialId = entity.GetEntitySerialId ();

			this.entityDataMapping[entitySerialId] = new EntityDataMapping (entity);
			this.LoadEntitySchema (entityId);
		}

		readonly DbInfrastructure infrastructure;
		readonly DbRichCommand richCommand;
		readonly SchemaEngine schemaEngine;
		readonly EntityContext entityContext;
		readonly Dictionary<long, EntityDataMapping> entityDataMapping;
		readonly Dictionary<Druid, DbTable> entityTableDefinitions;
	}
}
