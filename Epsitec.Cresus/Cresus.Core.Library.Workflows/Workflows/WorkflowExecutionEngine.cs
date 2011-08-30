//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.Core.Workflows
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


		/// <summary>
		/// Gets the executing user's <see cref="IItemCode"/> code.
		/// </summary>
		/// <returns>The user's code.</returns>
		public string GetActiveUserCode()
		{
			return (string) this.data.GetActiveUserItemCode ();
		}

		/// <summary>
		/// Gets the current time stamp (basically UTC 'now').
		/// </summary>
		/// <returns>The current time stamp.</returns>
		public static System.DateTime GetCurrentTimeStamp()
		{
			return System.DateTime.UtcNow;
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
			this.FollowWorkflowEdges (arc => this.ExecuteActions (arc.Edge.TransitionActions));
		}

		/// <summary>
		/// Parses, compiles and executes the actions specified in source form.
		/// </summary>
		/// <param name="transitionActions">The transition actions.</param>
		/// <returns><c>true</c> if the actions were successfully executed; otherwise, <c>false</c>.</returns>
		private void ExecuteActions(string transitionActions)
		{
			var action = WorkflowAction.Parse (transitionActions);
			
			action.Execute ();
		}

		private enum Flow
		{
			Continue,
			Abort,
		}

		private void FollowWorkflowEdges(System.Action<Arc> executor)
		{
			WorkflowThreadEntity thread   = this.transition.Thread;
			WorkflowEdgeEntity   edge     = this.transition.Edge;
			WorkflowEntity       workflow = this.transition.Workflow;
			WorkflowNodeEntity   node     = this.transition.Node;

			Queue<Arc> arcs = new Queue<Arc> ();
			arcs.Enqueue (new Arc (node, edge));

			int iterationCount = 0;
			Flow run = Flow.Continue;

			while ((arcs.Count > 0) && run == Flow.Continue)
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
						WorkflowExecutionEngine.ChangeThreadState (thread, WorkflowState.Cancelled);
						this.businessContext.SaveChanges ();
					}
					
					throw new System.Exception ("Fatal error: malformed workflow produces too many transitions at once -- cancelled");
				}
			}
		}

		private Flow FollowThreadWorkflowEdge(WorkflowThreadEntity thread, Queue<Arc> arcs, System.Action<Arc> executor)
		{
			WorkflowExecutionEngine.ChangeThreadState (thread, WorkflowState.Active);

			var arc  = arcs.Dequeue ();
			var edge = arc.Edge;

			if (WorkflowExecutionEngine.ExecuteArc (executor, arc) == Flow.Abort)
			{
				return Flow.Abort;
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
					return Flow.Continue;

				default:
					throw new System.NotSupportedException (string.Format ("TransitionType {0} not supported", edge.TransitionType));
            }

			this.PrepareNextNode (thread, arcs, edge);

			return Flow.Continue;
		}

		private static Flow ExecuteArc(System.Action<Arc> executor, Arc arc)
		{
			try
			{
				if (executor != null)
				{
					executor (arc);
				}
				
				return Flow.Continue;
			}
			catch (WorkflowException ex)
			{
				switch (ex.Cancellation)
				{
					case WorkflowCancellation.Action:
						return Flow.Continue;

					case WorkflowCancellation.Transition:
						return Flow.Abort;

					default:
						throw new System.NotImplementedException (string.Format ("{0} not implemented", ex.Cancellation.GetQualifiedName ()));
				}
			}
		}

		private void PrepareNextNode(WorkflowThreadEntity thread, Queue<Arc> arcs, WorkflowEdgeEntity edge)
		{
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
				WorkflowExecutionEngine.EnsureAtMostOneStandardTransition (node);

				foreach (var autoEdges in node.Edges)
				{
					arcs.Enqueue (new Arc (node, autoEdges));
				}
			}
		}

		private static void EnsureAtMostOneStandardTransition(WorkflowNodeEntity node)
		{
			var standardEdges = node.Edges.Where (x => x.TransitionType == WorkflowTransitionType.Default || x.TransitionType == WorkflowTransitionType.Call).ToList ();

			if (standardEdges.Count > 1)
			{
				throw new System.NotSupportedException ("Auto-node cannot have more than one standard edge");
			}
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
			var args   = runningThread.GetArgs ();			
			var thread = WorkflowFactory.CreateWorkflowThread (this.businessContext, runningThread.Definition, args);

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

				WorkflowExecutionEngine.ChangeThreadState (thread, WorkflowState.Done);
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
			step.Date			   = WorkflowExecutionEngine.GetCurrentTimeStamp ();
			step.ExecutingUserCode = this.GetActiveUserCode ();

			thread.History.Add (step);
		}

		private static void ChangeThreadState(WorkflowThreadEntity thread, WorkflowState status)
		{
			thread.State = status;
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
