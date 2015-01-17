//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

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

			var subscriptions = AiderSubscriptionEntity.FindSubscriptions (businessContext, entity);

			foreach (var subscription in subscriptions)
			{
				businessContext.Register (subscription);
			}

			var refusals = AiderSubscriptionRefusalEntity.FindRefusals (businessContext, entity);

			foreach (var refusal in refusals)
			{
				businessContext.Register (refusal);
			}
		}

		public override void ApplyUpdateRule(AiderLegalPersonEntity legal)
		{
			AiderLegalPersonBusinessRules.UpdateName (legal);

			this.UpdateParish (legal);

			legal.RefreshCache ();
		}

		private static void UpdateName(AiderLegalPersonEntity legal)
		{
			legal.Name = legal.Name.TrimSpacesAndDashes ();
		}

		private void UpdateParish(AiderLegalPersonEntity legal)
		{
			if (legal.ParishGroup.IsNull ())
			{
				var parishRepository = ParishAddressRepository.Current;
				var businessContext = this.GetBusinessContext ();

				ParishAssigner.AssignToParish (parishRepository, businessContext, legal);
			}
			else
			{
				// TODO Should we create a warning or silently update the parish. For now we don't
				// have the possibility to create a warning for somethint else than a person.
			}
		}
	}
}
