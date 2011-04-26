//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class MailContactBusinessRules : GenericBusinessRule<MailContactEntity>
	{
		public override void ApplySetupRule(MailContactEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			entity.Address = businessContext.CreateEntityAndRegisterAsEmpty<AddressEntity> ();
		}
	}
}
