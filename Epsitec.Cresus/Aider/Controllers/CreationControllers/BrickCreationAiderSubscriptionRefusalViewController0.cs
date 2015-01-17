//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
				var message = "Il ne faut spécifier qu'un seul destinataire.";

				throw new BusinessRuleException (message);
			}

			return household.IsNotNull ()
				? this.Create (household)
				: this.Create (legalPersonContact);
		}

		private AiderSubscriptionRefusalEntity Create(AiderHouseholdEntity receiver)
		{
			var businessContext = this.BusinessContext;

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (businessContext, receiver);
			AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (businessContext, receiver);

			return AiderSubscriptionRefusalEntity.Create (businessContext, receiver);
		}

		private AiderSubscriptionRefusalEntity Create(AiderContactEntity receiver)
		{
			if (receiver.ContactType != ContactType.Legal)
			{
				var message = "Le destinataire n'est pas une personne morale.";

				throw new BusinessRuleException (message);
			}

			var businessContext = this.BusinessContext;

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (businessContext, receiver);
			AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (businessContext, receiver);

			return AiderSubscriptionRefusalEntity.Create (businessContext, receiver);
		}
	}
}
