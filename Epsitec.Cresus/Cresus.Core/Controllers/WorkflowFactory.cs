//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public static class WorkflowFactory
	{
		public static WorkflowEntity CreateDefaultWorkflow<T>()
			where T : AbstractEntity, new ()
		{
			return null;
		}

		public static WorkflowDefinitionEntity FindDefaultWorkflowDefinition<T>()
			where T : AbstractEntity, new ()
		{
			var repository = CoreProgram.Application.Data.GetRepository<WorkflowDefinitionEntity> ();
			var example = repository.CreateExample ();

			string nakedEntityName = AbstractEntity.GetNakedEntityName<T> ();
			
			example.Name = FormattedText.FromSimpleText ("Default ", nakedEntityName, "Workflow");

			var matches = repository.GetByExample (example);

			return matches.FirstOrDefault ();
		}
	}
}
