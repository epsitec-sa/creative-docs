//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class WorkflowDefinitionRepository : Repository<WorkflowDefinitionEntity>
	{
		public WorkflowDefinitionRepository(CoreData data, DataContext context = null)
			: base (data, context, Data.DataLifetimeExpectancy.Immutable)
		{
		}
	}
}
