//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

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
				.Text (new FormattedText ("Êtes-vous sûr de vouloir supprimer ce refus ?<br/>Ceci créera un nouvel abonnement."))
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
			AiderSubscriptionEntity.Create (this.BusinessContext, this.Entity);
			AiderSubscriptionRefusalEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
