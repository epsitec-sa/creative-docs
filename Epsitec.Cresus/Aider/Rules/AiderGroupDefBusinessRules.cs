//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Aider.Helpers;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderGroupDefBusinessRules : GenericBusinessRule<AiderGroupDefEntity>
	{
		public override void ApplyValidateRule(AiderGroupDefEntity groupDef)
		{
			if (string.IsNullOrEmpty (groupDef.Name))
			{
				Logic.BusinessRuleException (groupDef, "Le nom de la définition de groupe doit être défini.");
			}
		}
		
		public override void ApplyUpdateRule(AiderGroupDefEntity groupDef)
		{
			var path = groupDef.PathTemplate;

			if ((AiderGroupIds.IsParish (path)) ||
				(AiderGroupIds.IsRegion (path)))
			{
				//	We don't want to update groups named "Régions" and "Paroisses", as their
				//	name is not the same as the group definition, but specific to the group.

				return;
			}

			var name    = groupDef.Name;
			var context = this.GetBusinessContext ();
			var example = new AiderGroupEntity
			{
				GroupDef = groupDef
			};

			var groups = context.GetByExample (example).ToList ();

			foreach (var group in groups)
			{
				group.Name = name;
			}

			if(!groupDef.RoleCacheDisabled)
			{
				AiderParticipationsHelpers.PurgeAndRebuildRoleCache (context);
			}
		}
	}
}