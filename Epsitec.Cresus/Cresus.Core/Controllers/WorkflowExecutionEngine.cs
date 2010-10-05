//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowExecutionEngine
	{
		public WorkflowExecutionEngine(WorkflowController controller, WorkflowEdge edge)
		{
			this.controller = controller;
			this.edge = edge;
		}

		public static WorkflowExecutionEngine Current
		{
			get
			{
				return WorkflowExecutionEngine.current;
			}
		}


		public WorkflowEdge Edge
		{
			get
			{
				return this.edge;
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

		private void ExecuteInContext()
		{
			if (this.FollowWorkflowEdge ())
			{
				WorkflowEdgeEntity edge = this.edge.Edge;
				System.Diagnostics.Debug.WriteLine ("Executed " + edge.TransitionAction);
			}
		}

		private bool FollowWorkflowEdge()
		{
			return WorkflowExecutionEngine.FollowThreadWorkflowEdge (this.controller.Data, this.edge.Thread, this.edge.Edge);
		}

		private static bool FollowThreadWorkflowEdge(CoreData data, WorkflowThreadEntity thread, WorkflowEdgeEntity edge)
		{
			using (var bc = data.CreateBusinessContext ())
			{
				var threadKey = DataLayer.Context.DataContextPool.Instance.FindEntityKey (thread);

				bc.SetActiveEntity (threadKey);

				if (bc.AcquireLock ())
				{
					thread = bc.GetLocalEntity (thread);
					edge   = bc.GetLocalEntity (edge);

					var step = bc.CreateEntity<WorkflowStepEntity> ();

					step.Edge  = edge;
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

		private readonly WorkflowEdge edge;
		private readonly WorkflowController controller;
	}
}
