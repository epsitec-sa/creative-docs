//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
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
			DataBrowser dataBrowser = new DataBrowser (this.DbInfrastructure);
			DataQuery dataQuery = new DataQuery ();
			
			foreach (var fieldId in example.GetEntityContext ().GetEntityFieldIds (example))
			{
				dataQuery.Columns.Add (new DataQueryColumn (EntityFieldPath.Parse (fieldId)));
			}

			// HACK Probably not the best way to get the CR_TYPE column because of the "[" and "]".
			dataQuery.Columns.Add (new DataQueryColumn (EntityFieldPath.CreateRelativePath ("[" + Tags.ColumnInstanceType + "]")));
			
			using (DbTransaction transaction = dataBrowser.Infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				foreach (DataBrowserRow row in dataBrowser.QueryByExample (transaction, example, dataQuery))
				{
					yield return this.DataContext.ResolveEntity<EntityType> (row);
				}

				transaction.Commit ();
			}
		}

		private DbInfrastructure dbInfrastructure;

		private DataContext dataContext;

	}

}
