//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (18)]
	public sealed class ActionAiderPersonViewController18ReactivatePerson : ActionViewController<AiderPersonEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.Text ("Réactiver la personne");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			var person = this.Entity;

            var mainContact      = person.MainContact;
            var householdContact = person.HouseholdContact;

            //  Try to repair the person, making it visible again and ensuring
            //  that there is at least one contact and an associated household.

            person.Visibility = PersonVisibilityStatus.Default;

            if (householdContact.IsNull ())
			{
                var address   = person.Address;
				var household = AiderHouseholdEntity.Create (this.BusinessContext, address);

                if (mainContact.IsNull ())
                {
                    AiderContactEntity.Create (this.BusinessContext, person, household, true);
                }
                else
                {
                    AiderContactEntity.ChangeHousehold (this.BusinessContext, mainContact, household, true);
                }
			}
		}
	}
}