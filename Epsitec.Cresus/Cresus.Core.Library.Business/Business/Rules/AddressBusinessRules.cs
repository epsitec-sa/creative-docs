//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class AddressBusinessRules : GenericBusinessRule<AddressEntity>
	{
		public override void ApplySetupRule(AddressEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			entity.Street = businessContext.CreateEntityAndRegisterAsEmpty<StreetEntity> ();
			entity.PostBox = businessContext.CreateEntityAndRegisterAsEmpty<PostBoxEntity> ();
		}
	}
}
