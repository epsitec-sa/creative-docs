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
			action
				.Title ("Supprimer l'abonnement")
				.Text ("Êtes vous sûr de vouloir supprimer cet abonnement ?")
				.Field<bool> ()
					.Title ("Créer un refus correspondant")
					.InitialValue (false)
				.End ()
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool> (this.Execute);
		}

		private void Execute(bool createRefusal)
		{
			if (createRefusal)
			{
				AiderSubscriptionRefusalEntity.Create (this.BusinessContext, this.Entity);
			}

			AiderSubscriptionEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
