//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{

	abstract class AbstractRepository
	{

		protected AbstractRepository(DbInfrastructure dbInfrastructure, DataContext dataContext)
		{
			this.DbInfrastructure = dbInfrastructure;
			this.DataContext = dataContext;
		}

		public DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
			private set
			{
				if (value == null)
				{
					throw new System.ArgumentNullException ();
				}

				this.dbInfrastructure = value;
			}
		}

		public DataContext DataContext
		{
			get
			{
				return dataContext;
			}
			private set
			{
				if (value == null)
				{
					throw new System.ArgumentNullException ();
				}

				this.dataContext = value;
			}
		}

		public EntityType GetEntityByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			return this.GetEntitiesByExample (example).FirstOrDefault ();
		}

		public IEnumerable<EntityType> GetEntitiesByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			EntityType dummyEntity = this.DataContext.EntityContext.CreateEmptyEntity<EntityType> ();
			Druid entityId = dummyEntity.GetEntityStructuredTypeId ();

			DataBrowser dataBrowser = new DataBrowser (this.DbInfrastructure);
			DataQuery dataQuery = this.BuildQuery (this.DataContext.EntityContext.GetStructuredType(dummyEntity) as StructuredType);

			using (DbTransaction transaction = dataBrowser.Infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				using (DataTable dataTable = this.BuildDataTable (entityId))
				{
					foreach (DataBrowserRow dataBrowserRow in dataBrowser.QueryByExample (transaction, example, dataQuery))
					{
						DbKey rowKey = dataBrowserRow.Keys[0];
						Druid realEntityTypeId = Druid.FromLong ((long) dataBrowserRow[AbstractRepository.InstanceTypeFieldPath]);
						DataRow dataRow = this.BuildDataRow (dataTable, dataBrowserRow);

						yield return this.DataContext.ResolveEntity (realEntityTypeId, entityId, rowKey, dataRow) as EntityType;
					}
				}

				transaction.Commit ();
			}
		}

		private DataQuery BuildQuery(StructuredType entityType)
		{
			DataQuery dataQuery = new DataQuery ();

			foreach (string fieldId in entityType.GetFieldIds())
			{
				dataQuery.Columns.Add (new DataQueryColumn (EntityFieldPath.Parse (fieldId)));
			}

			dataQuery.Columns.Add (new DataQueryColumn (AbstractRepository.InstanceTypeFieldPath));

			return dataQuery;
		}

		private DataTable BuildDataTable(Druid entityId)
		{
			DataTable dataTable = new DataTable ();

			foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions(entityId))
			{
				dataTable.Columns.Add (new DataColumn (this.DataContext.SchemaEngine.GetDataColumnName (field.Id)));
			}

			return dataTable;
		}

		private DataRow BuildDataRow(DataTable dataTable, DataBrowserRow dataBrowserRow)
		{
			DataRow dataRow = dataTable.NewRow ();

			foreach (DataQueryColumn column in dataBrowserRow.Query.Columns)
			{
				if (column.FieldPath != AbstractRepository.InstanceTypeFieldPath)
				{
					string name = column.FieldPath.Fields.Last ();
					object value = dataBrowserRow[column];

					dataRow[this.DataContext.SchemaEngine.GetDataColumnName (name)] = value;
				}
			}

			return dataRow;
		}

		private DbInfrastructure dbInfrastructure;

		private DataContext dataContext;

		private static EntityFieldPath InstanceTypeFieldPath = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnInstanceType + "]");

	}

}
