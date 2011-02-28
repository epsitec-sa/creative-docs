//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class WorkflowBusinessRules : GenericBusinessRule<WorkflowEntity>
	{
		public override void ApplySetupRule(WorkflowEntity workflow)
		{
//-			var affair = Logic.Current.BusinessContext.GetMasterEntity<AffairEntity> ();
			throw new System.NotImplementedException ();
//-			workflow.Affair = affair;
		}
	}
}
