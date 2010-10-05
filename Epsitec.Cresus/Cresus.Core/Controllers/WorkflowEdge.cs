//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowEdge
	{
		public WorkflowEdge(BusinessContext businessContext, WorkflowEntity workflow, WorkflowThreadEntity workflowThread, WorkflowEdgeEntity workflowEdge)
		{
			this.businessContext = businessContext;
			this.workflow = workflow;
			this.thread = workflowThread;
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

		public WorkflowEdgeEntity				Edge
		{
			get
			{
				return this.edge;
			}
		}


		private readonly BusinessContext businessContext;
		private readonly WorkflowEntity workflow;
		private readonly WorkflowThreadEntity thread;
		private readonly WorkflowEdgeEntity edge;
	}
}
