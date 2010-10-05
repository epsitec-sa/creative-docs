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
		public WorkflowEdge(BusinessContext businessContext, WorkflowDefinitionEntity workflowDefinition, WorkflowEdgeEntity workflowEdge)
		{
			this.businessContext = businessContext;
			this.definition = workflowDefinition;
			this.edge = workflowEdge;
			this.thread = null;
		}

		public WorkflowEdge(BusinessContext businessContext, WorkflowThreadEntity workflowThread, WorkflowEdgeEntity workflowEdge)
		{
			this.businessContext = businessContext;
			this.definition = null;
			this.edge = workflowEdge;
			this.thread = workflowThread;
		}

		
		public BusinessContext					BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public WorkflowDefinitionEntity			Definition
		{
			get
			{
				return this.definition;
			}
		}

		public WorkflowEdgeEntity				Edge
		{
			get
			{
				return this.edge;
			}
		}

		public WorkflowThreadEntity				Thread
		{
			get
			{
				return this.thread;
			}
		}


		private readonly BusinessContext businessContext;
		private readonly WorkflowDefinitionEntity definition;
		private readonly WorkflowEdgeEntity edge;
		private readonly WorkflowThreadEntity thread;
	}
}
