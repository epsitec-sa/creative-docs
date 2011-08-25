//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class AffairBusinessRules : GenericBusinessRule<AffairEntity>
	{
		public override void ApplySetupRule(AffairEntity affair)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			businessContext.AssignIds (affair, generatorPool);

			affair.Code = (string) ItemCodeGenerator.NewCode ();
			affair.Workflow = WorkflowFactory.CreateDefaultWorkflow<AffairEntity> (businessContext);

			//	TODO: ...compléter...
		}
	}
}