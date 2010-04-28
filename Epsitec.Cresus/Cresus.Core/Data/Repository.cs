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

	class Repository
	{

		protected Repository(DbInfrastructure dbInfrastructure, DataContext dataContext)
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
			DataBrowser dataBrowser = new DataBrowser (this.DbInfrastructure, this.DataContext);

			return dataBrowser.QueryByExample<EntityType> (example);
		}

	}

}
