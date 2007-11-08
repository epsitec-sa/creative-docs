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
	public sealed class DataContext : System.IDisposable
	{
		public DataContext(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
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
		readonly SchemaEngine schemaEngine;
		readonly EntityContext entityContext;
		readonly Dictionary<long, EntityDataMapping> entityDataMapping;
	}
}
