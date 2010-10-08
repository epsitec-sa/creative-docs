//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>WorkflowController</c> handles the interaction between a workflow and
	/// the other view controllers.
	/// </summary>
	public class WorkflowController
	{
		internal WorkflowController(DataViewOrchestrator orchestrator)
		{
			this.orchestrator = orchestrator;
			this.businessContexts = new List<BusinessContext> ();
			this.workflowDefs = new List<WorkflowDefinitionEntity> ();
			this.activeEdges = new List<WorkflowEdge> ();
			this.dataContext = this.orchestrator.Data.CreateDataContext ("WorkflowController");
		}


		public CoreData Data
		{
			get
			{
				return this.orchestrator.Data;
			}
		}


		public void Update()
		{
			this.MakeReady ();
			this.UpdateEnabledEdges ();
			this.isDirty = false;
		}

		internal void AttachBusinessContext(BusinessContext context)
		{
			this.businessContexts.Add (context);
			context.MasterEntitiesChanged += this.HandleBusinessContextMasterEntitiesChanged;
		}

		internal void DetachBusinessContext(BusinessContext context)
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

			CoreApplication.QueueTasklets ("WorkflowController.Update", new TaskletJob (() => this.Update (), TaskletRunMode.Async));
		}

		private void UpdateEnabledEdges()
		{
			this.activeEdges.Clear ();
			this.activeEdges.AddRange (this.GetEnabledEdges ());

			var mainViewController   = this.orchestrator.MainViewController;
			var actionViewController = mainViewController.ActionViewController;

			actionViewController.ClearButtons ();

			int index = 0;

			foreach (var edge in this.activeEdges)
			{
				this.CreateActionButton (actionViewController, edge, index++);
			}

			mainViewController.SetActionPanelVisibility (index > 0);
		}

		private void CreateActionButton(ActionViewController actionViewController, WorkflowEdge edge, int index)
		{
			var buttonId    = string.Format ("WorkflowEdge.{0}", index++);
			var title       = edge.Edge.Name;
			var description = edge.Edge.Description;
			var action      = this.CreateActionCallback (edge);

			actionViewController.AddButton (buttonId, title, description, action);
		}

		private System.Action CreateActionCallback(WorkflowEdge edge)
		{
			return () => this.ExecuteAction (edge);
		}

		private void ExecuteAction(WorkflowEdge edge)
		{
			var engine = new WorkflowExecutionEngine (this, edge);
			engine.Execute ();
			
			orchestrator.Navigator.PreserveNavigation (() => orchestrator.ClearActiveEntity ());
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

		private IEnumerable<WorkflowEdge> GetEnabledEdges()
		{
			return from context in this.businessContexts
				   from edge in WorkflowController.GetEnabledEdges (context)
				   select edge;
		}

		
		private static IEnumerable<WorkflowEdge> GetEnabledEdges(BusinessContext context)
		{
			return from workflow in WorkflowController.GetEnabledWorkflows (context).Distinct ()
				   from thread in workflow.Threads
				   from edge in WorkflowController.GetEnabledEdges (thread)
				   select new WorkflowEdge (context, workflow, thread, edge);
		}

		private static IEnumerable<WorkflowEntity> GetEnabledWorkflows(BusinessContext context)
		{
			if (context == null)
			{
				return EmptyEnumerable<WorkflowEntity>.Instance;
			}

			return from workflowHost in context.GetMasterEntities ().OfType<IWorkflowHost> ()
				   let workflow = workflowHost.Workflow
				   where workflow.IsNotNull ()
				   select workflow;
		}

		private static IEnumerable<WorkflowEdgeEntity> GetEnabledEdges(WorkflowThreadEntity thread)
		{
			int lastIndex = thread.History.Count - 1;

			if (lastIndex < 0)
			{
				return thread.Definition.Edges;
			}
			else
			{
				return WorkflowController.GetEnabledEdges (thread.History[lastIndex].Edge.NextNode) ?? thread.Definition.Edges;
			}
		}

		private static IEnumerable<WorkflowEdgeEntity> GetEnabledEdges(WorkflowNodeEntity node)
		{
			if ((node.IsNull ()) ||
				(node.Edges.Count == 0))
			{
				return null;
			}
			else
			{
				return node.Edges;
			}
		}


		private readonly DataViewOrchestrator orchestrator;
		private readonly List<BusinessContext> businessContexts;
		private readonly List<WorkflowDefinitionEntity> workflowDefs;
		private readonly List<WorkflowEdge> activeEdges;
		private readonly DataContext dataContext;

		private bool isReady;
		private bool isDirty;
	}
}
