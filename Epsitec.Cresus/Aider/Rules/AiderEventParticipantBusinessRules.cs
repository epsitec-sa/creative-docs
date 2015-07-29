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
			if (!AiderEventParticipantBusinessRules.RoleCanBeSetTwice (entity))
			{
				Logic.BusinessRuleException ("Ce rôle est déjà attribué");
			}

			if (!AiderEventParticipantBusinessRules.RoleIsOkForExternal (entity))
			{
				Logic.BusinessRuleException ("Le rôle choisi ne peut-être utilisé avec une personne externe");
			}	
		}

		public static bool RoleIsOkForExternal(AiderEventParticipantEntity entity)
		{
			if (entity.IsExternal == false)
			{
				return true;
			}

			switch (entity.Role)
			{
				case Enumerations.EventParticipantRole.None:
				case Enumerations.EventParticipantRole.BlessedChild:
				case Enumerations.EventParticipantRole.Catechumen:
				case Enumerations.EventParticipantRole.ChildBatise:
				case Enumerations.EventParticipantRole.Confirmant:
				case Enumerations.EventParticipantRole.DeceasedPerson:
				case Enumerations.EventParticipantRole.Husband:
				case Enumerations.EventParticipantRole.Spouse:
					return false;
				default:
					return true;
			}
		}

		public static bool RoleCanBeSetTwice (AiderEventParticipantEntity entity)
		{
			switch (entity.Role)
			{
				case Enumerations.EventParticipantRole.None:
				case Enumerations.EventParticipantRole.Witness:
				case Enumerations.EventParticipantRole.Confirmant:
				case Enumerations.EventParticipantRole.Catechumen:
				case Enumerations.EventParticipantRole.Minister:
					return true;
				default:
					return !(entity.Event.CountRole (entity.Role) > 1);
			}
		}
	}
}

