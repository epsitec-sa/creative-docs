//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderSubscriptionViewController0 : BrickCreationViewController<AiderSubscriptionEntity>
	{
		protected override void GetForm
		(
			ActionBrick<AiderSubscriptionEntity, SimpleBrick<AiderSubscriptionEntity>> action
		)
		{
			action
				.Title ("Créer un nouvel abonnement")
				.Text ("Choisissez un destinataire pour l'abonnement.")
				.Field<AiderHouseholdEntity> ()
					.Title ("Destinataire (ménage)")
				.End ()
				.Field<AiderContactEntity> ()
					.Title ("Destinataire (personne morale)")
					.WithDataset (Res.CommandIds.Base.ShowAiderLegalPersonContact)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create
				<AiderHouseholdEntity, AiderContactEntity, AiderSubscriptionEntity> (this.Execute);
		}

		private AiderSubscriptionEntity Execute
		(
			AiderHouseholdEntity household,
			AiderContactEntity legalPersonContact
		)
		{
			if (household.IsNull () && legalPersonContact.IsNull ())
			{
				var message = "Il faut spécifier un destinataire.";

				throw new BusinessRuleException (message);
			}

			if (household.IsNotNull () && legalPersonContact.IsNotNull ())
			{
				var message = "Il ne faut spécifier qu'un seul destinataire.";

				throw new BusinessRuleException (message);
			}

			return household.IsNotNull ()
				? this.Create (household)
				: this.Create (legalPersonContact);
		}

		private AiderSubscriptionEntity Create(AiderHouseholdEntity receiver)
		{
			var businessContext = this.BusinessContext;
			var user = AiderUserManager.Current.AuthenticatedUser;
			
			if (user.CanBypassSubscriptionCheck ())
			{
				//
			}
			else
			{
				AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (businessContext, receiver);
			}
			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (businessContext, receiver);

			var edition = this.GetEdition (receiver.Address);

			return AiderSubscriptionEntity.Create (businessContext, receiver, edition, 1);
		}

		private AiderSubscriptionEntity Create(AiderContactEntity receiver)
		{
			if (receiver.ContactType != ContactType.Legal)
			{
				var message = "Le destinataire n'est pas une personne morale.";

				throw new BusinessRuleException (message);
			}

			var businessContext = this.BusinessContext;
			var user = AiderUserManager.Current.AuthenticatedUser;

			if (user.CanBypassSubscriptionCheck ())
			{
				//
			}
			else
			{
				AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (businessContext, receiver);
			}

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (businessContext, receiver);

			var edition = this.GetEdition (receiver.Address);

			return AiderSubscriptionEntity.Create (businessContext, receiver, edition, count: 1);
		}

		private AiderGroupEntity GetEdition(AiderAddressEntity address)
		{
			var parishRepository = ParishAddressRepository.Current;
			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			// If we can't find the region code, we default to the region 4, which is the one of
			// Lausanne.

			var regionCode = parishName != null
				? parishRepository.GetDetails (parishName).RegionCode
				: BrickCreationAiderSubscriptionViewController0.DefaultRegionCode;

			return ParishAssigner.FindRegionGroup (this.BusinessContext, regionCode);
		}


		private static readonly int				DefaultRegionCode = 4;
	}
}
