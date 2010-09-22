using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class SoftwareUserGroupRepository : Repository<SoftwareUserGroupEntity>
	{
		public SoftwareUserGroupRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}
	}
}
