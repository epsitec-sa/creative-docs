using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderSubscriptionRefusalViewController0 : BrickDeletionViewController<AiderSubscriptionRefusalEntity>
	{
		protected override void GetForm(ActionBrick<AiderSubscriptionRefusalEntity, SimpleBrick<AiderSubscriptionRefusalEntity>> action)
		{
			action
				.Title ("Supprimer le refus")
				.Text ("Êtes vous sûr de vouloir supprimer ce refus à un abonnement?")
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			AiderSubscriptionRefusalEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
