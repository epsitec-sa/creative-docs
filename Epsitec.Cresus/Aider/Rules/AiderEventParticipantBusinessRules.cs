//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderEventParticipantBusinessRules : GenericBusinessRule<AiderEventParticipantEntity>
	{
		public override void ApplyValidateRule(AiderEventParticipantEntity entity)
		{
			// TODO: Refactor this list of exception

			if (entity.Role == Enumerations.EventParticipantRole.None)
			{
				return;
			}

			if (entity.Role == Enumerations.EventParticipantRole.Witness)
			{
				return;
			}

			if (entity.Role == Enumerations.EventParticipantRole.Confirmant)
			{
				return;
			}

			if (entity.Role == Enumerations.EventParticipantRole.Catechumen)
			{
				return;
			}

			if (entity.Role == Enumerations.EventParticipantRole.Minister)
			{
				return;
			}

			if (entity.Event.CountRole (entity.Role) > 1)
			{
				Logic.BusinessRuleException ("Ce rôle est déjà attribué");
				return;
			}
		}
	}
}

