//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowEdgeEntity : IEntityInitializer
	{
		public void InitializeDefaultValues()
		{
			this.Code = (string) ItemCodeGenerator.NewCode ();
		}

		public WorkflowNodeEntity GetContinuationOrDefault(WorkflowNodeEntity defaultNode)
		{
			return this.Continuation.IsNull () ? defaultNode : this.Continuation;
		}
	}
}
