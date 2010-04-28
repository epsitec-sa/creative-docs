using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Data
{
	
	class GenericRepository<EntityType> : Repository where EntityType : AbstractEntity, new ()
	{

		public GenericRepository(DbInfrastructure dbInfrastructure, DataContext dataContext)
			: base (dbInfrastructure, dataContext)
		{
			
		}

		public EntityType GetEntityByExample(EntityType example)
		{
			return this.GetEntityByExample<EntityType> (example);
		}

		public IEnumerable<EntityType> GetEntitiesByExample(EntityType example)
		{
			return this.GetEntitiesByExample<EntityType> (example);
		}

	}

}
