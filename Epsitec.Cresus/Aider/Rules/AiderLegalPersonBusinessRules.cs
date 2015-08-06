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
using Epsitec.Common.Support;
using Epsitec.Common.Types;

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
			var businessContext = this.GetBusinessContext ();
			if (legal.ParishGroup.IsNull ())
			{
				var parishRepository = ParishAddressRepository.Current;
				ParishAssigner.AssignToParish (parishRepository, businessContext, legal);
			}
			else
			{
				if (AiderLegalPersonBusinessRules.IsReassignNeeded (businessContext, legal))
				{
					AiderLegalPersonBusinessRules.AssignParish (businessContext, legal);
				}
			}

		}

		private static bool IsReassignNeeded(BusinessContext context, AiderLegalPersonEntity person)
		{
			if (ParishAssigner.IsInNoParishGroup (person))
			{
				return true;
			}
			if (ParishAssigner.IsInValidParish (ParishAddressRepository.Current, person))
			{
				return false;
			}

			return true;
		}

		private static void AssignParish(BusinessContext context, AiderLegalPersonEntity person)
		{
			ParishAssigner.AssignToParish (ParishAddressRepository.Current, context, person);
		}
	}
}
