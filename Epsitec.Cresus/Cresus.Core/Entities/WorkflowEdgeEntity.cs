//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowEdgeEntity : IEntityInitializer
	{
		public void InitializeDefaultValues()
		{
			this.Code = System.Guid.NewGuid ().ToString ("N");
		}

		public WorkflowNodeEntity GetContinuationOrDefault(WorkflowNodeEntity defaultNode)
		{
			return this.Continuation.IsNull () ? defaultNode : this.Continuation;
		}
	}
}
