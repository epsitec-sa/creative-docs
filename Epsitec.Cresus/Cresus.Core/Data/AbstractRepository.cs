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
			get;
			private set;
		}

		public DataContext DataContext
		{
			get;
			private set;
		}

		public EntityType GetEntityByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			return this.GetEntitiesByExample (example).FirstOrDefault ();
		}

		public IEnumerable<EntityType> GetEntitiesByExample<EntityType>(EntityType example) where EntityType : AbstractEntity, new ()
		{
			EntityType dummyEntity = this.DataContext.EntityContext.CreateEmptyEntity<EntityType> ();
			Druid askedEntityId = dummyEntity.GetEntityStructuredTypeId ();
			Druid baseEntityId =  this.DataContext.EntityContext.GetBaseEntityId (askedEntityId);

			DataBrowser dataBrowser = new DataBrowser (this.DbInfrastructure);
			DataQuery dataQuery = this.BuildQuery (askedEntityId);

			using (DbTransaction transaction = dataBrowser.Infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				using (DataTable dataTable = this.BuildDataTable (askedEntityId))
				{
					EntityFieldPath typePath = EntityFieldPath.CreateAbsolutePath (baseEntityId, AbstractRepository.FieldPathType);

					foreach (DataBrowserRow dataBrowserRow in dataBrowser.QueryByExample (transaction, example, dataQuery))
					{
						DbKey rowKey = dataBrowserRow.Keys[0];
						Druid realEntityTypeId = Druid.FromLong ((long) dataBrowserRow[typePath]);
						DataRow dataRow = this.BuildDataRow (dataTable, dataBrowserRow, typePath);

						yield return this.DataContext.ResolveEntity (realEntityTypeId, askedEntityId, baseEntityId, rowKey, dataRow) as EntityType;
					}
				}

				transaction.Commit ();
			}
		}

		private DataQuery BuildQuery(Druid entityId)
		{
			DataQuery dataQuery = new DataQuery ()
			{
				Distinct = true,
			};

			foreach (DataQueryColumn dataQueryColumn in this.BuildQueryColumns (entityId))
			{
				dataQuery.Columns.Add (dataQueryColumn);
			}

			foreach (DataQueryJoin dataQueryJoin in this.BuildQueryJoins (entityId))
			{
				dataQuery.Joins.Add (dataQueryJoin);
			}
			
			return dataQuery;
		}

		private IEnumerable<DataQueryColumn> BuildQueryColumns(Druid entityId)
		{
			Druid currentId = entityId;

			while (currentId.IsValid)
			{
				foreach (StructuredTypeField field in this.DataContext.EntityContext.GetEntityFieldDefinitions (currentId))
				{
					if (field.Relation == FieldRelation.None && field.Membership == FieldMembership.Local && field.Expression == null)
					{
						yield return new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (currentId, field.Id));
					}
				}

				Druid nextId = (this.DataContext.EntityContext.GetStructuredType (currentId) as StructuredType).BaseTypeId;

				if (!nextId.IsValid)
				{
					yield return new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (currentId, AbstractRepository.FieldPathType));
				}

				currentId = nextId;
			}
		}

		private IEnumerable<DataQueryJoin> BuildQueryJoins(Druid entityId)
		{
			Druid leftId = entityId;
			Druid rightId = (this.DataContext.EntityContext.GetStructuredType (entityId) as StructuredType).BaseTypeId;

			while (rightId.IsValid)
			{
				DataQueryColumn left = new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (leftId,  AbstractRepository.FieldPathId));
				DataQueryColumn right = new DataQueryColumn (EntityFieldPath.CreateAbsolutePath (rightId,  AbstractRepository.FieldPathId));

				SqlJoinCode type = SqlJoinCode.Inner;

				yield return new DataQueryJoin (left, right, type);

				leftId = rightId;
				rightId = (this.DataContext.EntityContext.GetStructuredType (rightId) as StructuredType).BaseTypeId;
			}
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

		private DataRow BuildDataRow(DataTable dataTable, DataBrowserRow dataBrowserRow, EntityFieldPath typePath)
		{
			DataRow dataRow = dataTable.NewRow ();

			foreach (DataQueryColumn column in dataBrowserRow.Query.Columns)
			{
				if (column.FieldPath != typePath)
				{
					object value = dataBrowserRow[column];

					dataRow[this.DataContext.SchemaEngine.GetDataColumnName (column.FieldPath.Fields.Last ())] = value;
				}
			}

			return dataRow;
		}

		private static EntityFieldPath FieldPathId = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnId + "]");

		private static EntityFieldPath FieldPathType = EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnInstanceType + "]");

	}

}
