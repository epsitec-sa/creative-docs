//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Workflows;

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
			this.data                 = this.orchestrator.Data;
			this.businessContexts     = new List<BusinessContext> ();
			this.activeTransitions    = new List<WorkflowTransition> ();
			this.actionButtonInfos    = new List<ActionButtonInfo> ();
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


		/// <summary>
		/// Gets the executing user's <see cref="IItemCode"/> code.
		/// </summary>
		/// <returns>The user's code.</returns>
		public string GetActiveUserCode()
		{
			return (string) this.data.GetActiveUserItemCode ();
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
			this.ClearActionButtons ();

			int index = 0;

			this.activeTransitions.ForEach (edge => this.CreateActionButton (edge, index++));

			return index > 0;
		}

		private void UpdateActionPanelVisibility(bool panelVisibility)
		{
			this.mainViewController.SetActionPanelVisibility (panelVisibility);
		}

		private void CreateActionButton(WorkflowTransition transition, int uniqueId)
		{
			var buttonId    = WorkflowController.GetActionButtonId (uniqueId);
			var title       = transition.Edge.Name;
			var description = transition.Edge.Description;
			var callback    = this.CreateActionCallback (transition);
			var button      = this.CreateActionButton (buttonId, title, description, callback);

			this.AddActionButtonInfo (button, transition);
		}

		private void ClearActionButtons()
		{
			foreach (var button in this.actionButtonInfos.Select (x => x.Button))
			{
				button.Dispose ();
			}

			this.actionButtonInfos.Clear ();
		}

		private System.Action CreateActionCallback(WorkflowTransition transition)
		{
			return () => this.ExecuteAction (transition);
		}

		private void AddActionButtonInfo(Button button, WorkflowTransition transition)
		{
			var actionInfo  = new ActionButtonInfo (button, transition);

			this.actionButtonInfos.Add (actionInfo);
		}
		
		private static string GetActionButtonId(int index)
		{
			return string.Format ("WorkflowEdge.{0}", index);
		}

		private void ExecuteAction(WorkflowTransition transition)
		{
			using (var engine = new WorkflowExecutionEngine (transition))
			{
				engine.Associate (this.orchestrator.Navigator);
				engine.Execute ();
			}

			this.RefreshNavigation ();
		}

		private void RefreshNavigation()
		{
			this.orchestrator.Navigator.PreserveNavigation (() => this.orchestrator.ClearActiveEntity ());
		}

		private void HandleBusinessContextMasterEntitiesChanged(object sender)
		{
			this.MakeDirty ();
		}

		private IEnumerable<WorkflowTransition> GetEnabledTransitions()
		{
			string activeUserCode = this.GetActiveUserCode ();

			return from context in this.businessContexts
				   from edge in WorkflowController.GetEnabledTransitions (context, activeUserCode)
				   select edge;
		}


		private static IEnumerable<WorkflowTransition> GetEnabledTransitions(BusinessContext context, string activeUserCode)
		{
			return from workflow in WorkflowController.GetEnabledWorkflows (context).Distinct ()
				   from thread in workflow.Threads
				   where WorkflowController.IsActiveThread (thread, activeUserCode)
				   let  node = WorkflowController.GetCurrentNode (thread)
				   from edge in WorkflowController.GetEnabledEdges (thread, node)
				   select new WorkflowTransition (context, workflow, thread, node, edge);
		}

		private static bool IsActiveThread(WorkflowThreadEntity thread, string activeUserCode)
		{
			WorkflowState state = thread.State;

			switch (state)
			{
				case WorkflowState.None:
				case WorkflowState.Active:
				case WorkflowState.Pending:
					return true;

				case WorkflowState.Done:
				case WorkflowState.Cancelled:
				case WorkflowState.TimedOut:
					return false;

				case WorkflowState.Restricted:
					return thread.RestrictedUserCode == activeUserCode;

				default:
					throw new System.NotImplementedException (string.Format ("{0} not implemented", state.GetQualifiedName ()));
			}
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
			if (node.IsNull ())
			{
				return null;
			}
			else if (node.Edges.Count == 0)
			{
				return EmptyEnumerable<WorkflowEdgeEntity>.Instance;
			}
			else
			{
				return node.Edges;
			}
		}


		private static IEnumerable<Widget> GetAllButtons()
		{
			return WorkflowController.workflowButtonsContainer.Children.OfType<Widget> ();
		}

		private Button CreateActionButton(string id, FormattedText title, FormattedText description, System.Action callback)
		{
			var button = new Button
			{
				Parent = WorkflowController.workflowButtonsContainer,
				Name = id,
				FormattedText = title,
				Padding = new Margins (5, 5, 2, 2),  // le texte ne doit pas toucher les bords du bouton
				ButtonStyle = ButtonStyle.Confirmation,
				Dock = DockStyle.Stacked,
				PreferredWidth = 100,
				PreferredHeight = Library.UI.Constants.ButtonLargeWidth+10,
			};

			if (!description.IsNullOrWhiteSpace)
			{
				ToolTip.Default.SetToolTip (button, description);
			}

			button.Clicked += (sender, e) => callback ();

			return button;
		}


		struct ActionButtonInfo : System.IEquatable<ActionButtonInfo>
		{
			public ActionButtonInfo(Button button, WorkflowTransition transition)
			{
				this.button     = button;
				this.transition = transition;
			}

			public Button						Button
			{
				get
				{
					return this.button;
				}
			}

			public WorkflowTransition			Transition
			{
				get
				{
					return this.transition;
				}
			}

			#region IEquatable<ActionButtonInfo> Members

			public bool Equals(ActionButtonInfo other)
			{
				return this.button == other.button;
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is ActionButtonInfo)
				{
					return this.Equals ((ActionButtonInfo) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return this.button == null ? 0 : (int) this.button.GetVisualSerialId ();
			}

			
			private readonly Button				button;
			private readonly WorkflowTransition	transition;
		}


		public static void SetWorkflowButtonsContainer(Widget container)
		{
			WorkflowController.workflowButtonsContainer = container;
		}

		private static Widget						workflowButtonsContainer;

		private readonly DataViewOrchestrator		orchestrator;
		private readonly MainViewController			mainViewController;
		private readonly CoreData					data;
		private readonly List<BusinessContext>		businessContexts;
		private readonly List<WorkflowTransition>	activeTransitions;
		private readonly DataContext				dataContext;
		private readonly List<ActionButtonInfo>		actionButtonInfos;

		private bool								isDirty;
		private bool								isDisposed;
	}
}
