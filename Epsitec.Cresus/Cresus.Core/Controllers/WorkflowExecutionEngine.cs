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
			this.controller = controller;
			this.transition = transition;
			this.data       = this.controller.Data;
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
			WorkflowThreadEntity thread = this.transition.Thread;
			WorkflowEdgeEntity   edge   = this.transition.Edge;

			Queue<WorkflowEdgeEntity> edges = new Queue<WorkflowEdgeEntity> ();
			edges.Enqueue (edge);

			int iterationCount = 0;

			while (edges.Count > 0)
			{
				if (this.FollowThreadWorkflowEdge (thread, edges) == false)
				{
					//	Could not lock ... catastrophic failure !
					
					throw new System.Exception ("Fatal error: could not follow workflow edge and store the target into the database");
				}

				if (iterationCount++ > 100)
				{
					throw new System.Exception ("Fatal error: malformed workflow produces too many transitions at once");
				}
			}

			return true;
		}

		private bool FollowThreadWorkflowEdge(WorkflowThreadEntity thread, Queue<WorkflowEdgeEntity> edges)
		{
			var edge = edges.Dequeue ();

			switch (edge.TransitionType)
            {
				case WorkflowTransitionType.Default:
					break;
				
				case WorkflowTransitionType.Call:
					break;
				
				case WorkflowTransitionType.Fork:
					break;

				default:
					throw new System.NotSupportedException (string.Format ("TransitionType {0} not supported", edge.TransitionType));
            }

			if (this.SaveStepIntoThreadHistory (thread, edge, edge.NextNode) == false)
			{
				return false;
			}
			
			WorkflowNodeEntity node = edge.NextNode;
				
			if ((node == null) ||
				(node.Edges.Count == 0))
			{
				//	Reached the end of the workflow. Can we "pop" an edge from the call
				//	stack ?

				int lastIndex = thread.CallGraph.Count - 1;

				if (lastIndex >= 0)
				{
					this.SaveStepIntoThreadHistory (thread, null, thread.CallGraph[lastIndex].ReturnNode);
					
					thread.CallGraph.RemoveAt (lastIndex);
				}
			}
			else if (node.IsAuto)
			{
				var standardEdges = node.Edges.Where (x => x.TransitionType == WorkflowTransitionType.Default || x.TransitionType == WorkflowTransitionType.Call).ToList ();

				if (standardEdges.Count > 1)
                {
					throw new System.NotSupportedException ("Auto-node cannot have more than one standard edge");
                }

				foreach (var autoNode in node.Edges)
				{
					edges.Enqueue (autoNode);
				}
			}

			return true;
		}

		private bool SaveStepIntoThreadHistory(WorkflowThreadEntity thread, WorkflowEdgeEntity edge, WorkflowNodeEntity node)
		{
			using (var bc = this.data.CreateBusinessContext ())
			{
				var threadKey = DataLayer.Context.DataContextPool.Instance.FindEntityKey (thread);

				bc.SetActiveEntity (threadKey);

				if (bc.AcquireLock ())
				{
					thread = bc.GetLocalEntity (thread);
					edge   = bc.GetLocalEntity (edge);
					node   = bc.GetLocalEntity (node);

					var step = bc.CreateEntity<WorkflowStepEntity> ();

					step.Edge  = edge;
					step.Node  = node;
					step.Date  = System.DateTime.UtcNow;
					step.User  = null; // TODO: ...
					step.Owner = null; // TODO: ...
					step.RelationContact = null; // TODO: ...
					step.RelationPerson  = null; // TODO: ...

					thread.History.Add (step);

					return true;
				}
			}

			return false;
		}

		[System.ThreadStatic]
		private static WorkflowExecutionEngine current;

		private readonly WorkflowTransition transition;
		private readonly WorkflowController controller;
		private readonly CoreData			data;

		private bool isDisposed;
	}
}
