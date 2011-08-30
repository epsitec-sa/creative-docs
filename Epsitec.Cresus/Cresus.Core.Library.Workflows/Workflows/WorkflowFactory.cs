//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.Core.Workflows
{
	public static class WorkflowFactory
	{
		public static WorkflowEntity CreateDefaultWorkflow<T>(IBusinessContext businessContext, string workflowName = null, SettingsCollection settings = null)
			where T : AbstractEntity, new ()
		{
			var definition = businessContext.GetLocalEntity (WorkflowFactory.FindDefaultWorkflowDefinition<T> (businessContext, workflowName));
			var workflow   = businessContext.CreateEntity<WorkflowEntity> ();
			var thread     = WorkflowFactory.CreateWorkflowThread (businessContext, definition, settings);

			workflow.Threads.Add (thread);
			
			return workflow;
		}

		public static WorkflowThreadEntity CreateWorkflowThread(IBusinessContext businessContext, WorkflowDefinitionEntity definition, SettingsCollection args)
		{
			System.Diagnostics.Debug.Assert (definition.IsNotNull ());

			WorkflowThreadEntity thread = businessContext.CreateEntity<WorkflowThreadEntity> ();

			thread.State	  = WorkflowState.Pending;
			thread.Definition = definition;
			thread.SetArgs (businessContext, args);

			return thread;
		}

		/// <summary>
		/// Finds the default workflow definition associated with the specified entity
		/// type.
		/// </summary>
		/// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="businessContext">The business context.</param>
		/// <param name="workflowName">Optional name of the workflow.</param>
		/// <returns>
		/// The default workflow definition associated with the specified entity type.
		/// </returns>
		public static WorkflowDefinitionEntity FindDefaultWorkflowDefinition<T>(IBusinessContext businessContext, string workflowName = null)
			where T : AbstractEntity, new ()
		{
			var data = businessContext.Data;
			var repository = data.GetRepository<WorkflowDefinitionEntity> ();
			var example = repository.CreateExample ();

			if (string.IsNullOrEmpty (workflowName))
			{
				string nakedEntityName = EntityInfo<T>.GetNakedName ();
				workflowName = string.Concat (WorkflowFactory.DefaultWorkflowPrefix, nakedEntityName);
			}
			
			example.WorkflowName = FormattedText.FromSimpleText (workflowName);

			var matches = repository.GetByExample (example);

			return matches.FirstOrDefault ();
		}
		
		private const string DefaultWorkflowPrefix = "DefaultWorkflow/";
	}
}
