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

		public void PersistEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);
			System.Data.DataRow dataRow;

			if (mapping.RowKey.IsEmpty)
			{
				//	Create a new row in the database for this entity.

				dataRow = this.CreateDataRow (mapping);
			}
			else
			{
				dataRow = this.FindDataRow (mapping);
			}
		}

		private System.Data.DataRow FindDataRow(EntityDataMapping mapping)
		{
			throw new System.Exception ("The method or operation is not implemented.");
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
			long entitySerialId = entity.GetEntitySerialId ();

			this.entityDataMapping[entitySerialId] = new EntityDataMapping (entity);
		}

		readonly DbInfrastructure infrastructure;
		readonly DbRichCommand richCommand;
		readonly SchemaEngine schemaEngine;
		readonly EntityContext entityContext;
		readonly Dictionary<long, EntityDataMapping> entityDataMapping;
	}
}
