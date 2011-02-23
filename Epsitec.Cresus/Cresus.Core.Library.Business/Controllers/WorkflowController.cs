//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
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
	public class WorkflowController : IIsDisposed
	{
		internal WorkflowController(DataViewOrchestrator orchestrator)
		{
			this.orchestrator         = orchestrator;
			this.mainViewController   = this.orchestrator.MainViewController;
			this.actionViewController = this.mainViewController.ActionViewController;
			this.data                 = this.orchestrator.Data;
			this.businessContexts     = new List<BusinessContext> ();
			this.activeTransitions    = new List<WorkflowTransition> ();
			this.dataContext          = this.data.CreateDataContext ("WorkflowController");
		}


		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}


		public void Update()
		{
			this.UpdateEnabledTransitions ();
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

		#region IDisposable Members

		public void Dispose()
		{
			this.isDisposed = true;
		}

		#endregion

		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion

		
		private void MakeDirty()
		{
			if (this.isDirty)
            {
				return;
            }

			this.isDirty = true;
			this.QueueAsyncUpdate ();
		}

		private void QueueAsyncUpdate()
		{
			var job = new TaskletJob (this, this.Update, TaskletRunMode.Async);
			
			Dispatcher.QueueTasklets ("WorkflowController.Update", job);
		}

		private void UpdateEnabledTransitions()
		{
			this.activeTransitions.Clear ();
			this.activeTransitions.AddRange (this.GetEnabledTransitions ());

			bool panelVisibility = this.UpdateActionButtons ();

			this.UpdateActionPanelVisibility (panelVisibility);
		}

		private bool UpdateActionButtons()
		{
			this.actionViewController.ClearButtons ();

			int index = 0;

			this.activeTransitions.ForEach (edge => this.CreateActionButton (edge, index++));

			return index > 0;
		}

		private void UpdateActionPanelVisibility(bool panelVisibility)
		{
			this.mainViewController.SetActionPanelVisibility (panelVisibility);
		}

		private void CreateActionButton(WorkflowTransition edge, int index)
		{
			var buttonId    = WorkflowController.GetActionButtonId (index);
			var title       = edge.Edge.Name;
			var description = edge.Edge.Description;
			var action      = this.GetActionCallback (edge);

			this.actionViewController.AddButton (buttonId, title, description, action);
		}

		private System.Action GetActionCallback(WorkflowTransition transition)
		{
			return () => this.ExecuteAction (transition);
		}

		private static string GetActionButtonId(int index)
		{
			return string.Format ("WorkflowEdge.{0}", index);
		}

		private void ExecuteAction(WorkflowTransition transition)
		{
			using (var engine = new WorkflowExecutionEngine (transition))
			{
				engine.Execute ();
			}
			
			orchestrator.Navigator.PreserveNavigation (() => orchestrator.ClearActiveEntity ());
		}

		private void HandleBusinessContextMasterEntitiesChanged(object sender)
		{
			this.MakeDirty ();
		}

		private IEnumerable<WorkflowTransition> GetEnabledTransitions()
		{
			return from context in this.businessContexts
				   from edge in WorkflowController.GetEnabledTransitions (context)
				   select edge;
		}

		
		private static IEnumerable<WorkflowTransition> GetEnabledTransitions(BusinessContext context)
		{
			return from workflow in WorkflowController.GetEnabledWorkflows (context).Distinct ()
				   from thread in workflow.Threads
				   let  node = WorkflowController.GetCurrentNode (thread)
				   from edge in WorkflowController.GetEnabledEdges (thread, node)
				   select new WorkflowTransition (context, workflow, thread, node, edge);
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

		private static WorkflowNodeEntity GetCurrentNode(WorkflowThreadEntity thread)
		{
			int lastIndex = thread.History.Count - 1;

			if (lastIndex < 0)
			{
				return thread.Definition;
			}
			else
			{
				var lastStep = thread.History[lastIndex];
				return lastStep.Node;
			}
		}

		private static IEnumerable<WorkflowEdgeEntity> GetEnabledEdges(WorkflowThreadEntity thread, WorkflowNodeEntity node)
		{
			if (node.IsNotNull ())
			{
				return WorkflowController.GetEnabledEdges (node) ?? thread.Definition.Edges;
			}
			else
			{
				return EmptyEnumerable<WorkflowEdgeEntity>.Instance;
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


		private readonly DataViewOrchestrator	orchestrator;
		private readonly MainViewController		mainViewController;
		private readonly ActionViewController	actionViewController;
		private readonly CoreData				data;
		private readonly List<BusinessContext>	businessContexts;
		private readonly List<WorkflowTransition>		activeTransitions;
		private readonly DataContext			dataContext;

		private bool							isDirty;
		private bool							isDisposed;
	}
}
