//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderHouseholdBusinessRules : GenericBusinessRule<AiderHouseholdEntity>
	{
		public override void ApplySetupRule(AiderHouseholdEntity household)
		{
			var businessContext = this.GetBusinessContext ();

			household.Address = businessContext.CreateEntity<AiderAddressEntity> ();
		}
	}
}
