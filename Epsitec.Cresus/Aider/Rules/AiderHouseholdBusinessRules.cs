//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			household.HouseholdMrMrs = Enumerations.HouseholdMrMrs.Auto;
			household.Address        = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();
		}

		public override void ApplyBindRule(AiderHouseholdEntity entity)
		{
			// Registering the contacts will also register the members, as they are registered
			// by the contacts.

			this.GetBusinessContext ().Register (entity.Contacts);
		}

		public override void ApplyUpdateRule(AiderHouseholdEntity household)
		{
			household.RefreshCache ();
		}
	}
}
