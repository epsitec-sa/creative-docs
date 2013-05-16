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
	public sealed class BrickCreationAiderSubscriptionRefusalViewController0 : BrickCreationViewController<AiderSubscriptionRefusalEntity>
	{
		protected override void GetForm
		(
			ActionBrick<AiderSubscriptionRefusalEntity, SimpleBrick<AiderSubscriptionRefusalEntity>> action
		)
		{
			action
				.Title ("Créer un nouveau refus")
				.Text ("Choisissez un destinataire pour le refus de l'abonnement.")
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
				<AiderHouseholdEntity, AiderContactEntity, AiderSubscriptionRefusalEntity> (this.Execute);
		}

		private AiderSubscriptionRefusalEntity Execute
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

		private AiderSubscriptionRefusalEntity Create(AiderHouseholdEntity receiver)
		{
			this.CheckRefusalDoesNotExist (receiver);

			return AiderSubscriptionRefusalEntity.Create (this.BusinessContext, receiver);
		}

		private AiderSubscriptionRefusalEntity Create(AiderContactEntity receiver)
		{
			if (receiver.ContactType != ContactType.Legal)
			{
				var message = "Le destinataire n'est pas une personne morale.";

				throw new BusinessRuleException (message);
			}

			this.CheckRefusalDoesNotExist (receiver);

			return AiderSubscriptionRefusalEntity.Create (this.BusinessContext, receiver);
		}

		private void CheckRefusalDoesNotExist(AiderHouseholdEntity receiver)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (this.BusinessContext, receiver);

			this.CheckRefusalDoesNotExist (result);
		}

		private void CheckRefusalDoesNotExist(AiderContactEntity receiver)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (this.BusinessContext, receiver);

			this.CheckRefusalDoesNotExist (result);
		}

		private void CheckRefusalDoesNotExist(AiderSubscriptionRefusalEntity result)
		{
			if (result != null)
			{
				var message = "Un refus existe déjà pour ce destinataire.";

				throw new BusinessRuleException (message);
			}
		}
	}
}
