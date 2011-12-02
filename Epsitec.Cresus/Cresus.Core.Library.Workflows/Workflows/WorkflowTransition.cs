//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowTransition</c> class captures a workflow transition (from one node
	/// to another).
	/// </summary>
	public class WorkflowTransition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WorkflowTransition"/> class.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="workflow">The workflow.</param>
		/// <param name="workflowThread">The workflow thread.</param>
		/// <param name="workflowNode">The workflow node.</param>
		/// <param name="workflowEdge">The workflow edge.</param>
		public WorkflowTransition(IBusinessContext businessContext, WorkflowEntity workflow, WorkflowThreadEntity workflowThread, WorkflowNodeEntity workflowNode, WorkflowEdgeEntity workflowEdge)
		{
			this.businessContext = businessContext;
			this.workflow = workflow;
			this.thread = workflowThread;
			this.node = workflowNode;
			this.edge = workflowEdge;
		}

		
		public IBusinessContext					BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public WorkflowEntity					Workflow
		{
			get
			{
				return this.workflow;
			}
		}

		public WorkflowThreadEntity				Thread
		{
			get
			{
				return this.thread;
			}
		}

		public WorkflowNodeEntity				Node
		{
			get
			{
				return this.node;
			}
		}

		public WorkflowEdgeEntity				Edge
		{
			get
			{
				return this.edge;
			}
		}


		public void Execute(params object[] parameters)
		{
			using (var engine = new WorkflowExecutionEngine (this))
			{
				foreach (var parameter in parameters)
				{
					engine.Associate (parameter);
				}

				engine.Execute ();
			}
		}

		public System.Action CreateAction(params object[] parameters)
		{
			return () => this.Execute (parameters);
		}


		private readonly IBusinessContext		businessContext;
		private readonly WorkflowEntity			workflow;
		private readonly WorkflowThreadEntity	thread;
		private readonly WorkflowNodeEntity		node;
		private readonly WorkflowEdgeEntity		edge;
	}
}
