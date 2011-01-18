//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class AffairBusinessRules : GenericBusinessRule<AffairEntity>
	{
		public override void ApplySetupRule(AffairEntity affair)
		{
			var generatorPool   = Logic.Current.Data.RefIdGeneratorPool;
			var businessContext = Logic.Current.BusinessContext;

			var generator = generatorPool.GetGenerator<AffairEntity> ();
			var nextId    = generator.GetNextId ();
			var workflow  = businessContext.CreateEntity<WorkflowEntity> ();
			var thread    = businessContext.CreateEntity<WorkflowThreadEntity> ();

			affair.IdA = string.Format ("{0:000000}", nextId);
			affair.Workflow = workflow;

			workflow.Affair = affair;
			workflow.Threads.Add (thread);

			thread.Definition = businessContext.GetLocalEntity (WorkflowFactory.FindDefaultWorkflowDefinition<AffairEntity> ());

			//	TODO: ...compléter...
		}
	}
}