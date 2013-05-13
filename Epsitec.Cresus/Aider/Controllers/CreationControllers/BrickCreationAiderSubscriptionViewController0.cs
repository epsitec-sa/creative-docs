using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;

using System.Linq;

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
				var message = "Il faut spécifier qu'un seul destinataire.";

				throw new BusinessRuleException (message);
			}

			return household.IsNotNull ()
				? this.Create (household)
				: this.Create (legalPersonContact);
		}

		private AiderSubscriptionEntity Create(AiderHouseholdEntity receiver)
		{
			this.CheckSubscriptionDoesNotExist (receiver);

			var edition = this.GetEdition (receiver.Address);

			return AiderSubscriptionEntity.Create (this.BusinessContext, receiver, edition, 1);
		}

		private AiderSubscriptionEntity Create(AiderContactEntity receiver)
		{
			if (receiver.ContactType != ContactType.Legal)
			{
				var message = "Le destinataire n'est pas une personne morale.";

				throw new BusinessRuleException (message);
			}

			this.CheckSubscriptionDoesNotExist (receiver);

			var edition = this.GetEdition (receiver.Address);

			return AiderSubscriptionEntity.Create (this.BusinessContext, receiver, edition, 1);
		}

		private void CheckSubscriptionDoesNotExist(AiderHouseholdEntity receiver)
		{
			var result = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, receiver);

			this.CheckSubscriptionDoesNotExist (result);
		}

		private void CheckSubscriptionDoesNotExist(AiderContactEntity receiver)
		{
			var result = AiderSubscriptionEntity.FindSubscription (this.BusinessContext, receiver);

			this.CheckSubscriptionDoesNotExist (result);
		}

		private void CheckSubscriptionDoesNotExist(AiderSubscriptionEntity result)
		{
			if (result != null)
			{
				var format = "Un abonnement existe déjà pour ce destinataire: n°{0}.";
				var message = string.Format (format, result.Id);

				throw new BusinessRuleException (message);
			}
		}

		private AiderGroupEntity GetEdition(AiderAddressEntity address)
		{
			var parishRepository = ParishAddressRepository.Current;
			var parishName = ParishAssigner.FindParishName (parishRepository, address);

			// If we can't find the region code, we default to the region 4, which is the one of
			// Lausanne.

			var regionCode = parishName != null
				? parishRepository.GetDetails (parishName).RegionCode
				: 4;

			return ParishAssigner.FindRegionGroup (this.BusinessContext, regionCode);
		}
	}
}
