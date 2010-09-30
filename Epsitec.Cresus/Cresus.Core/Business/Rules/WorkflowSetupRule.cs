//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class WorkflowSetupRule : GenericBusinessRule<WorkflowEntity>
	{
		protected override void Apply(WorkflowEntity workflow)
		{
			var affair = Logic.Current.BusinessContext.GetMasterEntity<AffairEntity> ();

			if (affair.Workflows.Count == 0)
			{
				workflow.Id = 0;
			}
			else
			{
				workflow.Id = affair.Workflows.Max (x => x.Id) + 1;
			}
		}
	}
}
