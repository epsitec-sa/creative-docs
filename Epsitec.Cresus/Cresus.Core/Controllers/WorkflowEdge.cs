//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class WorkflowEdge : System.Tuple<BusinessContext, WorkflowDefinitionEntity, WorkflowEdgeEntity>
	{
		public WorkflowEdge(BusinessContext context, WorkflowDefinitionEntity workflowDef, WorkflowEdgeEntity workflowEdge)
			: base (context, workflowDef, workflowEdge)
		{
		}

		public BusinessContext BusinessContext
		{
			get
			{
				return base.Item1;
			}
		}

		public WorkflowDefinitionEntity Definition
		{
			get
			{
				return base.Item2;
			}
		}

		public WorkflowEdgeEntity Edge
		{
			get
			{
				return base.Item3;
			}
		}
	}
}
