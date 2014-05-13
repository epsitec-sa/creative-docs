//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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

			var businessContext = this.GetBusinessContext ();

			businessContext.Register (entity.Contacts);
			businessContext.Register (entity.Address);

			var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, entity);

			if (subscription.IsNotNull ())
			{
				businessContext.Register (subscription);
			}

			var refusal = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, entity);

			if (refusal.IsNotNull ())
			{
				businessContext.Register (refusal);
			}
		}

		public override void ApplyUpdateRule(AiderHouseholdEntity household)
		{
			household.RefreshCache ();
		}
	}
}