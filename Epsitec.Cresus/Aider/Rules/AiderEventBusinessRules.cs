//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderEventBusinessRules : GenericBusinessRule<AiderEventEntity>
	{
		public override void ApplyValidateRule(AiderEventEntity entity)
		{
			if (entity.State != Enumerations.EventState.InPreparation)
			{
				var error = "";
				if (!entity.IsCurrentEventValid (out error))
				{
					Logic.BusinessRuleException ("L'acte n'est pas validable en l'état:\n" + error);
				}

				//Side-effects
				switch (entity.Type)
				{
					case Enumerations.EventType.FuneralService:
						entity.GetMainActors ().ForEach ((actor) =>
						{
							if (actor.IsAlive)
							{
								actor.KillPerson (this.GetBusinessContext (), entity.Date.Value, true);
							}
						});
						break;
					case Enumerations.EventType.CelebrationRegisteredPartners:
						entity.GetMainActors ().ForEach ((actor) =>
						{
							if (actor.eCH_Person.AdultMaritalStatus != Enumerations.PersonMaritalStatus.Pacs)
							{
								actor.eCH_Person.AdultMaritalStatus = Enumerations.PersonMaritalStatus.Pacs;
							}
						});
						break;
					case Enumerations.EventType.Marriage:
						entity.GetMainActors ().ForEach ((actor) =>
						{
							if (actor.eCH_Person.AdultMaritalStatus != Enumerations.PersonMaritalStatus.Married)
							{
								actor.eCH_Person.AdultMaritalStatus = Enumerations.PersonMaritalStatus.Married;
							}
						});
						break;
				}
			}
		}
	}
}

