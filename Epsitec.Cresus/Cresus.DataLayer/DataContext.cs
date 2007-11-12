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

		public void SerializeEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);
			System.Data.DataRow dataRow;

			//	Either create and fill a new row in the database for this entity
			//	or use and update an existing row.

			if (mapping.RowKey.IsEmpty)
			{
				dataRow = this.CreateDataRow (mapping);
			}
			else
			{
				dataRow = this.FindDataRow (mapping);
			}

			dataRow.BeginEdit ();
			this.SerializeEntity (entity, dataRow);
			dataRow.EndEdit ();
		}

		private void SerializeEntity(AbstractEntity entity, System.Data.DataRow dataRow)
		{
			dataRow[Tags.ColumnInstanceType] = this.GetInstanceTypeValue (entity);
			
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityFieldDefinitions (entity))
			{
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.PersistFieldValueIntoDataRow (entity, fieldDef, dataRow);
						break;

					default:
						//	TODO: ...
						break;
				}
			}
		}

		private object GetInstanceTypeValue(AbstractEntity entity)
		{
			Druid entityId = entity.GetEntityStructuredTypeId ();
			return entityId.ToLong ();
		}

		private void PersistFieldValueIntoDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
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

		private System.Data.DataRow FindDataRow(EntityDataMapping mapping)
		{
			string tableName = this.GetDataTableName (mapping);
			return this.richCommand.FindRow (tableName, mapping.RowKey.Id);
		}

		private System.Data.DataRow CreateDataRow(EntityDataMapping mapping)
		{
			System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsEmpty);

			string tableName = this.GetDataTableName (mapping);
			System.Data.DataRow row = this.richCommand.CreateRow (tableName);
			mapping.RowKey = new DbKey (row);
			return row;
		}

		private string GetDataTableName(EntityDataMapping mapping)
		{
			DbTable tableDef = this.schemaEngine.FindTableDefinition (mapping.EntityId);
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
