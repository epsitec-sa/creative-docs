//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowTransition
	{
		public WorkflowTransition(BusinessContext businessContext, WorkflowEntity workflow, WorkflowThreadEntity workflowThread, WorkflowNodeEntity workflowNode, WorkflowEdgeEntity workflowEdge)
		{
			this.businessContext = businessContext;
			this.workflow = workflow;
			this.thread = workflowThread;
			this.node = workflowNode;
			this.edge = workflowEdge;
		}

		
		public BusinessContext					BusinessContext
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

		
		private readonly BusinessContext		businessContext;
		private readonly WorkflowEntity			workflow;
		private readonly WorkflowThreadEntity	thread;
		private readonly WorkflowNodeEntity		node;
		private readonly WorkflowEdgeEntity		edge;
	}
}
