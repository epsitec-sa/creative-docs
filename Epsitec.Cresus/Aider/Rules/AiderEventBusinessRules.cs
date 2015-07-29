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
			}
		}
	}
}

