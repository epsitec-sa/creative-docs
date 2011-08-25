//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>WorkflowExecutionEngine</c> class manages the execution of workflow steps.
	/// </summary>
	public sealed class WorkflowExecutionEngine : IIsDisposed
	{
		public WorkflowExecutionEngine(WorkflowTransition transition)
		{
			this.transition      = transition;
			this.data            = this.transition.BusinessContext.Data;
			this.businessContext = this.transition.BusinessContext;
		}


		/// <summary>
		/// Gets the current <see cref="WorkflowExecutionEngine"/>; this is only available while
		/// the engine is executing method <see cref="Execute"/>.
		/// </summary>
		public static WorkflowExecutionEngine	Current
		{
			get
			{
				return WorkflowExecutionEngine.current;
			}
		}


		public IBusinessContext					BusinessContext
		{
			get
			{
				return this.Transition.BusinessContext;
			}
		}

		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public Logic							BusinessLogic
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

		public WorkflowTransition				Transition
		{
			get
			{
				return this.transition;
			}
		}


		/// <summary>
		/// Executes one step in the workflow (this might include several edges/nodes, as
		/// there might be automatic transitions) as defined by the <see cref="WorkflowTransition"/>
		/// specified in the constructor.
		/// </summary>
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
					using (this.businessContext.AutoLock (workflow))
					{
						WorkflowExecutionEngine.ChangeThreadStatus (thread, WorkflowState.Cancelled);
						this.businessContext.SaveChanges ();
					}
					
					throw new System.Exception ("Fatal error: malformed workflow produces too many transitions at once -- cancelled");
				}
			}
		}

		private bool FollowThreadWorkflowEdge(WorkflowThreadEntity thread, Queue<Arc> arcs, System.Func<Arc, bool> executor)
		{
			WorkflowExecutionEngine.ChangeThreadStatus (thread, WorkflowState.Active);

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
					this.StartNewThread (thread, arc);
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

		private void StartNewThread(WorkflowThreadEntity runningThread, Arc arc)
		{
			var settings = runningThread.GetSettings ();			
			var thread   = WorkflowFactory.CreateWorkflowThread (this.businessContext, runningThread.Definition, settings);

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

				WorkflowExecutionEngine.ChangeThreadStatus (thread, WorkflowState.Done);
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

			step.Edge			   = edge;
			step.Node			   = node;
			step.Date			   = System.DateTime.UtcNow;
			step.ExecutingUserCode = (string) this.data.GetActiveUserItemCode ();

			thread.History.Add (step);
		}

		private static void ChangeThreadStatus(WorkflowThreadEntity thread, WorkflowState status)
		{
			thread.Status = status;
		}

		#region Arc Structure

		private struct Arc
		{
			public Arc(WorkflowNodeEntity node, WorkflowEdgeEntity edge)
			{
				this.node = node;
				this.edge = edge;
			}


			public WorkflowNodeEntity			Node
			{
				get
				{
					return this.node;
				}
			}

			public WorkflowEdgeEntity			Edge
			{
				get
				{
					return this.edge;
				}
			}

			private readonly WorkflowNodeEntity node;
			private readonly WorkflowEdgeEntity edge;
		}

		#endregion

		[System.ThreadStatic]
		private static WorkflowExecutionEngine	current;

		private readonly WorkflowTransition		transition;
		private readonly CoreData				data;
		private readonly IBusinessContext		businessContext;
		private Logic							businessLogic;
		private bool							isDisposed;
	}
}
