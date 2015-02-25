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
	internal class AiderEventPlaceBusinessRules : GenericBusinessRule<AiderEventPlaceEntity>
	{
		public override void ApplyValidateRule(AiderEventPlaceEntity entity)
		{
			if (string.IsNullOrWhiteSpace (entity.Name))
			{
				Logic.BusinessRuleException ("Nom du lieu invalide");
				return;
			}

			if (entity.OfficeOwner.IsNotNull ())
			{
				if (!entity.OfficeOwner.ParishGroup.IsParish ())
				{
					Logic.BusinessRuleException ("La gestion séléctionnée n'est pas de type paroisse");
					return;
				}
			}		
		}
	}
}

