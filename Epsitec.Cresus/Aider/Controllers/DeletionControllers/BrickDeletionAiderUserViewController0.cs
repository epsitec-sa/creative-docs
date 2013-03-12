﻿using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DeletionControllers;

namespace Epsitec.Aider.Controllers.DeletionControllers
{
	[ControllerSubType (0)]
	public sealed class BrickDeletionAiderUserViewController0 : BrickDeletionViewController<AiderUserEntity>
	{
		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> action)
		{
			action
				.Title ("Détruire l'utilisateur")
				.Text ("Êtes vous sûr de vouloir détruire cet utilisateur ?")
				.Field<string> ()
					.Title ("Nom d'utilisateur")
					.InitialValue (x => x.LoginName)
					.ReadOnly ()
				.End ()
				.Field<FormattedText> ()
					.Title ("Nom d'affichage")
					.InitialValue (x => x.DisplayName)
					.ReadOnly ()
				.End ()
			.End ();
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string> (this.Execute);
		}

		private void Execute(string _1, string _2)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;

			if (!currentUser.HasPowerLevel (UserPowerLevel.Administrator))
			{
				var message = "Seul un administrateur a le droit de détruire des utilisateurs.";

				throw new BusinessRuleException (this.Entity, message);
			}

			var key1 = UserManager.Current.BusinessContext.DataContext.GetNormalizedEntityKey(currentUser);
			var key2 = this.BusinessContext.DataContext.GetNormalizedEntityKey(this.Entity);
			
			// We check the value of the keys, because the two entities belong to two diffent
			// DataContext and checking them directly for equality would always return false.
			if (key1 == key2)
			{
				var message = "Vous ne pouvez pas vous détruire vous-même.";

				throw new BusinessRuleException (this.Entity, message);
			}

			if (this.Entity.Mutability != Mutability.Customizable)
			{
				var message = "Vous ne pouvez pas détruire un utilisateur système.";

				throw new BusinessRuleException (this.Entity, message);
			}

			this.Entity.Delete (this.BusinessContext);
		}
	}
}
