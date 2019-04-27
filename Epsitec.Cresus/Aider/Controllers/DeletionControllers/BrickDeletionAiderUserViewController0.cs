//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD
using System.Linq;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
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
            var user = this.Entity;

            if (this.HasUserPowerLevel (UserPowerLevel.Administrator) == false)
            {
                var message = "Seul un administrateur a le droit de détruire des utilisateurs.";

                throw new BusinessRuleException (user, message);
            }

            if (this.MatchesUser (user))
            {
                var message = "Vous ne pouvez pas vous détruire vous-même.";

                throw new BusinessRuleException (user, message);
            }

            if (user.Mutability != Mutability.Customizable)
            {
                var message = "Vous ne pouvez pas détruire un utilisateur système.";

                throw new BusinessRuleException (user, message);
            }

            user.Delete (this.BusinessContext);
        }

        private bool MatchesUser(AiderUserEntity user)
        {
            // We check the value of the keys, because the two entities belong to two diffent
            // DataContext and checking them directly for equality would always return false.

            var manager = UserManager.Current;
            var key1 = manager.BusinessContext.DataContext.GetNormalizedEntityKey (manager.AuthenticatedUser);
            var key2 = this.BusinessContext.DataContext.GetNormalizedEntityKey (user);

            return key1 == key2;
        }
	}
}
