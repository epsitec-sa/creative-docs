//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderLegalPersonBusinessRules : GenericBusinessRule<AiderLegalPersonEntity>
	{
		public override void ApplySetupRule(AiderLegalPersonEntity legal)
		{
			var businessContext = this.GetBusinessContext ();

			legal.Address       = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();
			legal.Type          = LegalPersonType.Business;
			legal.Visibility    = PersonVisibilityStatus.Default;
			legal.RemovalReason = RemovalReason.None;
			legal.Language      = Language.French;
		}

		public override void ApplyBindRule(AiderLegalPersonEntity entity)
		{
			var businessContext = this.GetBusinessContext ();

			businessContext.Register (entity.Contacts);
			businessContext.Register (entity.Address);
		}

		public override void ApplyUpdateRule(AiderLegalPersonEntity legal)
		{
			AiderLegalPersonBusinessRules.UpdateName (legal);

			legal.RefreshCache ();
		}

		private static void UpdateName(AiderLegalPersonEntity legal)
		{
			legal.Name = legal.Name.TrimSpacesAndDashes ();
		}
	}
}
