//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed class WorkflowExecutionEngine : System.IDisposable, IIsDisposed
	{
		public WorkflowExecutionEngine(WorkflowController controller, WorkflowTransition transition)
		{
			this.controller      = controller;
			this.transition      = transition;
			this.data            = this.controller.Data;
			this.businessContext = this.transition.BusinessContext;
		}

		public static WorkflowExecutionEngine Current
		{
			get
			{
				return WorkflowExecutionEngine.current;
			}
		}


		public WorkflowTransition Transition
		{
			get
			{
				return this.transition;
			}
		}

		public WorkflowController Controller
		{
			get
			{
				return this.controller;
			}
		}


		public void Execute()
		{
			var previousExecutionEngine = WorkflowExecutionEngine.current;

			try
			{
				WorkflowExecutionEngine.current = this;
				this.ExecuteInContext ();
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
			if (this.FollowWorkflowEdge ())
			{
				WorkflowEdgeEntity edge = this.transition.Edge;

				System.Diagnostics.Debug.WriteLine ("Executed " + edge.TransitionAction);
			}
		}

		private bool FollowWorkflowEdge()
		{
			WorkflowThreadEntity thread   = this.transition.Thread;
			WorkflowEdgeEntity   edge     = this.transition.Edge;
			WorkflowEntity       workflow = this.transition.Workflow;
			WorkflowNodeEntity   node     = this.transition.Node;

			Queue<Arc> arcs = new Queue<Arc> ();
			arcs.Enqueue (new Arc (node, edge));

			int iterationCount = 0;

			while (arcs.Count > 0)
			{
				using (this.businessContext.AutoLock (workflow))
				{
					this.FollowThreadWorkflowEdge (thread, arcs);
					this.businessContext.SaveChanges ();
				}

				if (iterationCount++ > 100)
				{
					throw new System.Exception ("Fatal error: malformed workflow produces too many transitions at once");
				}
			}

			return true;
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

		private void FollowThreadWorkflowEdge(WorkflowThreadEntity thread, Queue<Arc> arcs)
		{
			var arc  = arcs.Dequeue ();
			var edge = arc.Edge;

			switch (edge.TransitionType)
            {
				case WorkflowTransitionType.Default:
					break;
				
				case WorkflowTransitionType.Call:
					this.PushNodeToThreadCallGraph (thread, arc.Node);
					break;
				
				case WorkflowTransitionType.Fork:
					break;

				default:
					throw new System.NotSupportedException (string.Format ("TransitionType {0} not supported", edge.TransitionType));
            }

			this.AddStepToThreadHistory (thread, edge, edge.NextNode);
			
			WorkflowNodeEntity node = edge.NextNode;
				
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
		}

		private void PushNodeToThreadCallGraph(WorkflowThreadEntity thread, WorkflowNodeEntity returnNode)
		{
			WorkflowCallEntity call = this.businessContext.CreateEntity<WorkflowCallEntity> ();

			call.ReturnNode = returnNode;

			thread.CallGraph.Add (call);
		}

		private void PopNodeFromCallGraph(WorkflowThreadEntity thread)
		{
			int lastIndex = thread.CallGraph.Count - 1;

			if (lastIndex >= 0)
			{
				this.AddStepToThreadHistory (thread, null, thread.CallGraph[lastIndex].ReturnNode);

				thread.CallGraph.RemoveAt (lastIndex);
			}
		}

		private void AddStepToThreadHistory(WorkflowThreadEntity thread, WorkflowEdgeEntity edge, WorkflowNodeEntity node)
		{
			var step = this.businessContext.CreateEntity<WorkflowStepEntity> ();

			step.Edge  = edge;
			step.Node  = node;
			step.Date  = System.DateTime.UtcNow;
			step.User  = null; // TODO: ...
			step.Owner = null; // TODO: ...
			step.RelationContact = null; // TODO: ...
			step.RelationPerson  = null; // TODO: ...

			thread.History.Add (step);
		}

		[System.ThreadStatic]
		private static WorkflowExecutionEngine current;

		private readonly WorkflowTransition transition;
		private readonly WorkflowController controller;
		private readonly CoreData			data;
		private readonly BusinessContext	businessContext;

		private bool isDisposed;
	}
}
