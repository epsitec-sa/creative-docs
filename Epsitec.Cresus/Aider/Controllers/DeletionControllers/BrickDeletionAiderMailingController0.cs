//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using System.Linq;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

namespace Epsitec.Aider.Controllers.DeletionControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderMailingController0 : BrickDeletionViewController<AiderMailingEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> action)
		{
			action
				.Title ("Détruire le publipostage")
				.Text ("Êtes vous sûr de vouloir détruire ce publipostage ?")
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create (this.Execute);
		}

		private void Execute()
		{
			
			var user = UserManager.Current.AuthenticatedUser;

			var aiderUserExample = new AiderUserEntity ()
			{
				People = user.People
			};
			var aiderUser = this.BusinessContext.DataContext.GetByExample<AiderUserEntity> (aiderUserExample).FirstOrDefault ();
			
			if (this.Entity.CreatedBy != aiderUser)
			{
				var message = "Vous ne pouvez pas détruire le publipostage d'un autre utilisateur";

				throw new BusinessRuleException (this.Entity, message);
			}


			AiderMailingEntity.Delete (this.BusinessContext, this.Entity);
		}
	}
}
