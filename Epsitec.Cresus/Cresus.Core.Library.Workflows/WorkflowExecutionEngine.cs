//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed class WorkflowExecutionEngine : IIsDisposed
	{
		public WorkflowExecutionEngine(WorkflowTransition transition)
		{
			this.transition      = transition;
			this.data            = this.transition.BusinessContext.Data;
			this.businessContext = this.transition.BusinessContext;
		}

		public static WorkflowExecutionEngine Current
		{
			get
			{
				return WorkflowExecutionEngine.current;
			}
		}


		public IBusinessContext BusinessContext
		{
			get
			{
				return this.Transition.BusinessContext;
			}
		}

		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public Logic BusinessLogic
		{
			get
			{
				if (this.businessLogic == null)
				{
					this.businessLogic = this.businessContext.CreateLogic (null);
				}

				return this.businessLogic;
			}
		}

		public WorkflowTransition Transition
		{
			get
			{
				return this.transition;
			}
		}


		public void Execute()
		{
			var previousExecutionEngine = WorkflowExecutionEngine.current;

			try
			{
				WorkflowExecutionEngine.current = this;
				this.BusinessLogic.ApplyAction (this.ExecuteInContext);
			}
			finally
			{
				WorkflowExecutionEngine.current = previousExecutionEngine;
			}
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

		private void ExecuteInContext()
		{
			this.FollowWorkflowEdges (arc => this.ExecuteAction (arc.Edge.TransitionAction));
		}

		private bool ExecuteAction(string transitionAction)
		{
			var action = WorkflowAction.Parse (transitionAction);
			action.Execute ();

			return true;
		}

		private void FollowWorkflowEdges(System.Func<Arc, bool> executor)
		{
			WorkflowThreadEntity thread   = this.transition.Thread;
			WorkflowEdgeEntity   edge     = this.transition.Edge;
			WorkflowEntity       workflow = this.transition.Workflow;
			WorkflowNodeEntity   node     = this.transition.Node;

			Queue<Arc> arcs = new Queue<Arc> ();
			arcs.Enqueue (new Arc (node, edge));

			int iterationCount = 0;
			bool run = true;

			while ((arcs.Count > 0) && run)
			{
				using (this.businessContext.AutoLock (workflow))
				{
					run = this.FollowThreadWorkflowEdge (thread, arcs, executor);
					this.businessContext.SaveChanges ();
				}

				if (iterationCount++ > 100)
				{
					throw new System.Exception ("Fatal error: malformed workflow produces too many transitions at once");
				}
			}
		}

		private struct Arc
		{
			public Arc(WorkflowNodeEntity node, WorkflowEdgeEntity edge)
			{
				this.node = node;
				this.edge = edge;
			}


			public WorkflowNodeEntity Node
			{
				get
				{
					return this.node;
				}
			}

			public WorkflowEdgeEntity Edge
			{
				get
				{
					return this.edge;
				}
			}

			private readonly WorkflowNodeEntity node;
			private readonly WorkflowEdgeEntity edge;
		}

		private bool FollowThreadWorkflowEdge(WorkflowThreadEntity thread, Queue<Arc> arcs, System.Func<Arc, bool> executor)
		{
			var arc  = arcs.Dequeue ();
			var edge = arc.Edge;

			if (executor != null)
			{
				bool result = executor (arc);

				if (result == false)
				{
					System.Diagnostics.Debug.Assert (arcs.Count == 0);
					return false;
				}
			}

			switch (edge.TransitionType)
            {
				case WorkflowTransitionType.Default:
					break;
				
				case WorkflowTransitionType.Call:
					this.PushNodeToThreadCallGraph (thread, edge.GetContinuationOrDefault (arc.Node));
					break;
				
				case WorkflowTransitionType.Fork:
					this.StartNewThread (arc);
					return true;

				default:
					throw new System.NotSupportedException (string.Format ("TransitionType {0} not supported", edge.TransitionType));
            }

			WorkflowNodeEntity node = this.ResolveForeignNode (edge.NextNode);

			this.AddStepToThreadHistory (thread, edge, node);
				
			if ((node == null) ||
				(node.Edges.Count == 0))
			{
				//	Reached the end of the workflow. Can we "pop" an edge from the call
				//	stack ?

				this.PopNodeFromCallGraph (thread);
			}
			else if (node.IsAuto)
			{
				var standardEdges = node.Edges.Where (x => x.TransitionType == WorkflowTransitionType.Default || x.TransitionType == WorkflowTransitionType.Call).ToList ();

				if (standardEdges.Count > 1)
                {
					throw new System.NotSupportedException ("Auto-node cannot have more than one standard edge");
                }

				foreach (var autoEdges in node.Edges)
				{
					arcs.Enqueue (new Arc (node, autoEdges));
				}
			}

			return true;
		}

		private WorkflowNodeEntity ResolveForeignNode(WorkflowNodeEntity node)
		{
			if ((node.IsNotNull ()) &&
				(node.IsForeign))
			{
				//	This is a foreign node, which means that we have to find the real
				//	target node in another workflow. The target must be marked as public
				//	and shares the same item code.

				var repo    = new Repositories.WorkflowNodeRepository (this.data);
				var example = repo.CreateExample ();
				
				example.Code      = node.Code;
				example.IsPublic  = true;
				example.IsForeign = true;
				example.IsForeign = false;

				node = repo.GetByExample (example).FirstOrDefault ();
				node = this.businessContext.GetLocalEntity (node);
			}

			return node;
		}

		private void StartNewThread(Arc arc)
		{
			WorkflowThreadEntity thread = this.businessContext.CreateEntity<WorkflowThreadEntity> ();

			thread.Definition = null;

			this.AddThreadToWorkflow (thread);
			this.AddStepToThreadHistory (thread, arc.Edge, this.ResolveForeignNode (arc.Edge.NextNode));
		}

		private void AddThreadToWorkflow(WorkflowThreadEntity thread)
		{
			this.transition.Workflow.Threads.Add (thread);
		}

		private void PushNodeToThreadCallGraph(WorkflowThreadEntity thread, WorkflowNodeEntity continuation)
		{
			WorkflowCallEntity call = this.businessContext.CreateEntity<WorkflowCallEntity> ();

			call.Continuation = continuation;

			thread.CallGraph.Add (call);
		}

		private void PopNodeFromCallGraph(WorkflowThreadEntity thread)
		{
			int lastIndex = thread.CallGraph.Count - 1;

			if (lastIndex < 0)
			{
				//	We have reached the end of the graph...
			}
			else
			{
				this.AddStepToThreadHistory (thread, null, thread.CallGraph[lastIndex].Continuation);

				thread.CallGraph.RemoveAt (lastIndex);
			}
		}

		private void AddStepToThreadHistory(WorkflowThreadEntity thread, WorkflowEdgeEntity edge, WorkflowNodeEntity node)
		{
			var step = this.businessContext.CreateEntity<WorkflowStepEntity> ();

			step.Edge  = edge;
			step.Node  = node;
			step.Date  = System.DateTime.UtcNow;
//			step.User  = null; // TODO: ...
//			step.Owner = null; // TODO: ...
//			step.RelationContact = null; // TODO: ...
//			step.RelationPerson  = null; // TODO: ...

			thread.History.Add (step);
		}

		[System.ThreadStatic]
		private static WorkflowExecutionEngine current;

		private readonly WorkflowTransition transition;
		private readonly CoreData			data;
		private readonly IBusinessContext	businessContext;

		private Logic						businessLogic;
		private bool isDisposed;
	}
}
