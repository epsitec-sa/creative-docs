//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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
            var user  = AiderUserManager.Current.AuthenticatedUser;

            if (group.IsNull ())
			{
				Logic.BusinessRuleException (mailingCategory, TextFormatter.FormatText ("Il faut lier cette catégorie à un groupe"));
			}

            if (!user.CanEditGroup (group))
			{
				Logic.BusinessRuleException (mailingCategory, TextFormatter.FormatText ("Vous n'avez pas accès au groupe sélectionné"));
			}
		}
	}
}
