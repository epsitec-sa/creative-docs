//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD
using System.Linq;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;
using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderUserViewController0 : BrickCreationViewController<AiderUserEntity>
	{
		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> action)
		{
			action
				.Title ("Créer un nouvel utilisateur")
				.Field<AiderContactEntity> ()
					.Title ("Contact")
				.End ()
				.Field<AiderUserRoleEntity> ()
					.Title ("Role")
				.End ()
				.Field<bool> ()
					.Title ("Administrateur")
				.End ()
				.Field<string> ()
					.Title ("Mot de passe")
					.Password ()
				.End ()
				.Field<string> ()
					.Title ("Confirmation du mot de passe")
					.Password ()
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<AiderContactEntity, AiderUserRoleEntity, bool, string, string, AiderUserEntity> (this.Execute);
		}

		private AiderUserEntity Execute(AiderContactEntity contact, AiderUserRoleEntity role, bool admin, string password, string confirmation)
		{
			if (this.HasUserPowerLevel (UserPowerLevel.Administrator) == false)
			{
				var message = "Seul un administrateur a le droit de créer des utilisateurs.";

				throw new BusinessRuleException (this.Entity, message);
			}

			if (role.IsNull ())
			{
				throw new BusinessRuleException (this.Entity, "Le rôle est obligatoire");
			}

            if ((contact.IsNull ()) ||
                (contact.Person.IsNull ()))
			{
				throw new BusinessRuleException (this.Entity, "Un contact avec une personne physique est obligatoire");
			}

            var email = contact.Person.MainEmail;

            if (string.IsNullOrEmpty (email))
            {
                throw new BusinessRuleException (this.Entity, "Le contact sélectionné n'a pas d'adresse e-mail");
            }

			var user = AiderUserEntity.Create (this.BusinessContext, contact, role);

            user.SetAdministrator (this.BusinessContext, admin);
			user.SetPassword (password, confirmation);
            
            return user;
		}
    }
}
