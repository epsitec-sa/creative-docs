//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderSubscriptionViewController0 : BrickDeletionViewController<AiderSubscriptionEntity>
	{
		protected override void GetForm(ActionBrick<AiderSubscriptionEntity, SimpleBrick<AiderSubscriptionEntity>> action)
		{
			action
				.Title ("Supprimer l'abonnement")
				.Text (new FormattedText ("Êtes-vous sûr de vouloir supprimer cet abonnement ?<br/>Ceci créera un refus pour cet abonnement."))
				.Field<bool> ()
					.Title ("Je confirme l'opération")
					.InitialValue (true)
				.End ()
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}

		private void Execute(bool dummy)
		{
			var subscription = this.Entity;
			switch (subscription.SubscriptionType)
			{
			case SubscriptionType.Household:
				if (this.Entity.Household.IsNotNull ())
				{
					if (this.Entity.Household.Members.Count > 0)
					{
						AiderSubscriptionRefusalEntity.Create (this.BusinessContext, this.Entity);
					}
					else
					{
						AiderHouseholdEntity.DeleteEmptyHouseholds (this.BusinessContext, this.Entity.Household);
					}
				}
				break;
			case SubscriptionType.LegalPerson:
				if (this.Entity.LegalPersonContact.IsNotNull ())
				{
					AiderSubscriptionRefusalEntity.Create (this.BusinessContext, this.Entity);
				}
				break;
			}

			AiderSubscriptionEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
