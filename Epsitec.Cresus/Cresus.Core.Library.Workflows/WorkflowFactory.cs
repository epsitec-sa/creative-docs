//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public static class WorkflowFactory
	{
		public static WorkflowEntity CreateDefaultWorkflow<T>(IBusinessContext businessContext, string workflowName = null)
			where T : AbstractEntity, new ()
		{
			var workflow  = businessContext.CreateEntity<WorkflowEntity> ();
			var thread    = businessContext.CreateEntity<WorkflowThreadEntity> ();

			thread.Definition = businessContext.GetLocalEntity (WorkflowFactory.FindDefaultWorkflowDefinition<T> (businessContext.Data, workflowName));

			workflow.Threads.Add (thread);
			
			return workflow;
		}

		/// <summary>
		/// Finds the default workflow definition associated with the specified entity
		/// type.
		/// </summary>
		/// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="data">The core data.</param>
		/// <param name="workflowName">Optional name of the workflow.</param>
		/// <returns>
		/// The default workflow definition associated with the specified entity type.
		/// </returns>
		public static WorkflowDefinitionEntity FindDefaultWorkflowDefinition<T>(CoreData data, string workflowName = null)
			where T : AbstractEntity, new ()
		{
			var repository = data.GetRepository<WorkflowDefinitionEntity> ();
			var example = repository.CreateExample ();

			if (string.IsNullOrEmpty (workflowName))
			{
				string nakedEntityName = AbstractEntity.GetNakedEntityName<T> ();
				workflowName = string.Concat (WorkflowFactory.DefaultWorkflowPrefix, nakedEntityName);
			}
			
			example.WorkflowName = FormattedText.FromSimpleText (workflowName);

			var matches = repository.GetByExample (example);

			return matches.FirstOrDefault ();
		}
		
		private const string DefaultWorkflowPrefix = "DefaultWorkflow/";
	}
}
