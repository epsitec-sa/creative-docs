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
			Stack<Druid> parents = new Stack<Druid> ();

			parents.Push (Druid.Empty);

			for (Druid baseTypeId = this.entityContext.GetEntityBaseTypeId (entity); baseTypeId.IsValid; )
			{
				StructuredType baseType = this.entityContext.GetStructuredType (baseTypeId) as StructuredType;

				System.Diagnostics.Debug.Assert (baseType != null);
				System.Diagnostics.Debug.Assert (baseType.CaptionId == baseTypeId);

				parents.Push (baseTypeId);

				baseTypeId = baseType.BaseTypeId;
			}

			foreach (Druid parentId in parents)
			{
				//	Either create and fill a new row in the database for this entity
				//	or use and update an existing row.

				System.Data.DataRow dataRow;
				DbKey rowKey = mapping[parentId];

				if (rowKey.IsEmpty)
				{
					dataRow = this.CreateDataRow (mapping, parentId);
				}
				else
				{
					dataRow = this.FindDataRow (mapping, parentId);
				}

				dataRow.BeginEdit ();
				Druid entityId = entity.GetEntityStructuredTypeId ();
				this.SerializeEntity (entity, entityId, dataRow, parentId);
				dataRow.EndEdit ();
			}
		}

		private void SerializeEntity(AbstractEntity entity, Druid entityId, System.Data.DataRow dataRow, Druid parentId)
		{
			System.Diagnostics.Debug.Assert (entityId.IsValid);
			System.Diagnostics.Debug.Assert (entityId != parentId);

			dataRow[Tags.ColumnInstanceType] = this.GetInstanceTypeValue (parentId.IsValid ? parentId : entityId);

			this.SerializeEntityLocal (entity, dataRow, parentId);
		}

		private void SerializeEntityLocal(AbstractEntity entity, System.Data.DataRow dataRow, Druid definingTypeId)
		{
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityFieldDefinitions (entity))
			{
				if (fieldDef.Membership == FieldMembership.Inherited)
				{
					if (fieldDef.DefiningTypeId == definingTypeId)
					{
						System.Diagnostics.Debug.Assert (fieldDef.DefiningTypeId.IsValid);

						//	The field is defined by the parent entity class
						//	we are currently serializing.
					}
					else
					{
						continue;
					}
				}
				else
				{
					if (definingTypeId.IsEmpty)
					{
						System.Diagnostics.Debug.Assert (fieldDef.DefiningTypeId.IsEmpty);

						//	The field is defined locally and that's what we
						//	are currently serializing : local fields.
					}
					else
					{
						continue;
					}
				}
				
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
			long typeValueId = (long) dataRow[Tags.ColumnInstanceType];
			Druid entityId = Druid.FromLong (typeValueId);
			AbstractEntity entity = this.entityContext.CreateEmptyEntity (entityId);
			this.DeserializeEntity (entity, dataRow);
			return entity;
		}

		private void DeserializeEntity(AbstractEntity entity, System.Data.DataRow dataRow)
		{
			using (entity.DefineOriginalValues ())
			{
				//	TODO: ...
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

		private System.Data.DataRow FindDataRow(EntityDataMapping mapping, Druid parentId)
		{
			string tableName = this.GetDataTableName (mapping);
			return this.richCommand.FindRow (tableName, mapping[parentId].Id);
		}

		private System.Data.DataRow CreateDataRow(EntityDataMapping mapping, Druid parentId)
		{
			System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
			System.Diagnostics.Debug.Assert (mapping[parentId].IsEmpty);

			string tableName = this.GetDataTableName (mapping);
			System.Data.DataRow row = this.richCommand.CreateRow (tableName);
			mapping[parentId] = new DbKey (row);
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
