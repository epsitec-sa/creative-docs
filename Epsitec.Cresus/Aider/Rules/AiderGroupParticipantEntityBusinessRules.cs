//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderGroupParticipantEntityBusinessRules : GenericBusinessRule<AiderGroupParticipantEntity>
	{
		public override void ApplyBindRule(AiderGroupParticipantEntity entity)
		{
			var businessContext = this.GetBusinessContext ();

			if (entity.Contact.IsNotNull ())
			{
				businessContext.Register (entity.Contact);
			}
		}
	}
}