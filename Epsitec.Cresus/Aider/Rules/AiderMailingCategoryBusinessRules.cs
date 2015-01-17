//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderMailingCategoryBusinessRules : GenericBusinessRule<AiderMailingCategoryEntity>
	{
		public override void ApplyUpdateRule(AiderMailingCategoryEntity mailingCategory)
		{
			mailingCategory.RefreshCache ();
		}

		public override void ApplyValidateRule(AiderMailingCategoryEntity mailingCategory)
		{
			var group = mailingCategory.Group;

			if (group.IsNull ())
			{
				Logic.BusinessRuleException (mailingCategory, TextFormatter.FormatText ("Il faut lier cette catégorie à un groupe"));
			}

			if (group.CanBeEditedByCurrentUser () == false)
			{
				Logic.BusinessRuleException (mailingCategory, TextFormatter.FormatText ("Vous n'avez pas accès au groupe sélectionné"));
			}
		}
	}
}