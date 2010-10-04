//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowController
	{
		public WorkflowController(DataViewOrchestrator orchestrator)
		{
			this.orchestrator = orchestrator;
			this.businessContexts = new List<BusinessContext> ();
			this.workflowDefs = new List<WorkflowDefinitionEntity> ();
			this.activeEdges = new List<WorkflowEdgeEntity> ();
		}


		public void Update()
		{
			this.MakeReady ();
			this.UpdateActiveEdges ();
			this.isDirty = false;
		}

		public void AttachBusinessContext(BusinessContext context)
		{
			this.businessContexts.Add (context);
			context.MasterEntitiesChanged += this.HandleBusinessContextMasterEntitiesChanged;
		}

		public void DetachBusinessContext(BusinessContext context)
		{
			context.MasterEntitiesChanged -= this.HandleBusinessContextMasterEntitiesChanged;
			this.businessContexts.Remove (context);
		}

		private void MakeReady()
		{
			if (this.isReady)
			{
				return;
			}

			this.UpdateWorkflowDefs ();

			this.isReady = true;
		}

		private void MakeDirty()
		{
			if (this.isDirty)
            {
				return;
            }

			this.isDirty = true;

			CoreApplication.QueueTasklets ("Refresh WorkflowController", new TaskletJob (() => this.Update (), TaskletRunMode.Async));
		}

		private void UpdateActiveEdges()
		{
			this.activeEdges.Clear ();
			this.activeEdges.AddRange (this.GetStartingEdges ());

			var actionViewController = this.orchestrator.MainViewController.ActionViewController;

			actionViewController.ClearButtons ();

			int index = 0;

			foreach (var edge in this.activeEdges)
			{
				string id = string.Format ("WorkflowEdge.{0}", index++);
				var title = edge.Name;
				var description = edge.Description;
				actionViewController.AddButton (id, title, description, () => System.Diagnostics.Debug.WriteLine ("Executed " + id));
			}

			this.orchestrator.MainViewController.SetActionPanelVisibility (index > 0);
		}

		private void UpdateWorkflowDefs()
		{
			this.workflowDefs.Clear ();
			this.workflowDefs.AddRange (this.orchestrator.Data.GetAllEntities<WorkflowDefinitionEntity> ());
		}

		private void HandleBusinessContextMasterEntitiesChanged(object sender)
		{
			this.MakeDirty ();
		}

		private IEnumerable<WorkflowEdgeEntity> GetStartingEdges()
		{
			return this.businessContexts.SelectMany (x => this.GetBusinessContextStartingEdges (x));
		}

		private IEnumerable<WorkflowEdgeEntity> GetBusinessContextStartingEdges(BusinessContext context)
		{
			return this.GetBusinessContextWorkflowDefs (context).Distinct ().SelectMany (x => x.StartingEdges);
		}

		private IEnumerable<WorkflowDefinitionEntity> GetBusinessContextWorkflowDefs(BusinessContext context)
		{
			if (context ==  null)
			{
				yield break;
			}

			foreach (var masterEntity in context.GetMasterEntities ())
			{
				string typeKey = string.Concat ("MasterEntity=", masterEntity.GetEntityStructuredTypeKey ());

				foreach (var workflowDef in this.workflowDefs)
				{
					if (workflowDef.CheckEnableCondition (typeKey))
                    {
						yield return workflowDef;
                    }
				}
			}
		}

		private readonly DataViewOrchestrator orchestrator;
		private readonly List<BusinessContext> businessContexts;
		private readonly List<WorkflowDefinitionEntity> workflowDefs;
		private readonly List<WorkflowEdgeEntity> activeEdges;

		private bool isReady;
		private bool isDirty;
	}
}
