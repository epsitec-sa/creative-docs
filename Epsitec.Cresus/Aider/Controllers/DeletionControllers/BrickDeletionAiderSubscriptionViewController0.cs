using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderSubscriptionViewController0 : BrickDeletionViewController<AiderSubscriptionEntity>
	{
		protected override void GetForm(ActionBrick<AiderSubscriptionEntity, SimpleBrick<AiderSubscriptionEntity>> action)
		{
			// TODO Add a boolean to assess wheter we simply must delete the subscription or if we
			// must also create an refusal for its recipient.

			action
				.Title ("Supprimer l'abonnement")
				.Text ("Êtes vous sûr de vouloir supprimer cet abonnement ?")
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			AiderSubscriptionEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
