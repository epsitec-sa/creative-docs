using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderUserViewController0 : BrickCreationViewController<AiderUserEntity>
	{
		protected override void GetForm(ActionBrick<AiderUserEntity, SimpleBrick<AiderUserEntity>> action)
		{
			action
				.Title ("Créer un nouvel utilisateur")
				.Field<string> ()
					.Title ("Nom d'utilisateur")
				.End ()
				.Field<string> ()
					.Title ("Nom d'affichage")
				.End ()
				.Field<string> ()
					.Title ("Email")
				.End ()
				.Field<AiderUserRoleEntity> ()
					.Title ("Role")
				.End ()
				.Field<AiderGroupEntity> ()
					.Title ("Paroisse")
					.WithSpecialField<AiderGroupSpecialField<AiderUserEntity>> ()
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
			return FunctionExecutor.Create<string, string, string, AiderUserRoleEntity, AiderGroupEntity, bool, string, string, AiderUserEntity> (this.Execute);
		}

		private AiderUserEntity Execute(string username, string displayname, string email, AiderUserRoleEntity role, AiderGroupEntity parish, bool admin, string password, string confirmation)
		{
			if (this.HasUserPowerLevel (UserPowerLevel.Administrator))
			{
				var message = "Seul un administrateur a le droit de créer des utilisateurs.";

				throw new BusinessRuleException (this.Entity, message);
			}

			if (role.IsNull ())
			{
				throw new BusinessRuleException (this.Entity, "Le rôle est obligatoire");
			}

			var user = this.BusinessContext.CreateAndRegisterEntity<AiderUserEntity> ();

			user.LoginName = username;
			user.DisplayName = displayname;
			user.Email = email;
			user.Role = role;
			user.Parish = parish;

			user.SetAdmininistrator (this.BusinessContext, admin);
			user.SetPassword (password, confirmation);

			return user;
		}
	}
}
