//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderContactBusinessRules : GenericBusinessRule<AiderContactEntity>
	{
		public override void ApplyBindRule(AiderContactEntity entity)
		{
			var businessContext = this.GetBusinessContext ();

			var household = entity.Household;

			if (household.IsNotNull ())
			{
				businessContext.Register (household);
			}

			var person = entity.Person;

			if (person.IsNotNull ())
			{
				businessContext.Register (person);
			}

			// We don't register legal persons, as there is no dependency from contacts to legal
			// persons.
		}

		public override void ApplyUpdateRule(AiderContactEntity contact)
		{
			if (contact.Person.IsNotNull ())
			{
				//	TODO#PA
			}

			contact.RefreshCache ();
		}
	}
}
