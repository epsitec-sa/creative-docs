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
		public override void ApplyUpdateRule(AiderContactEntity contact)
		{
			contact.RefreshCache ();

			if (contact.Household.IsNotNull ())
			{
				contact.Household.RefreshCache ();
			}

			if (contact.Person.IsNotNull ())
			{
				//	TODO#PA
			}
		}
	}
}
