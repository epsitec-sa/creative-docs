//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Entities;


namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderUserBusinessRules : GenericBusinessRule<AiderUserEntity>
	{
		public override void ApplySetupRule(AiderUserEntity user)
		{
			this.SetupMutability (user);
			this.SetupAuthenticationMethod (user);
			this.SetupUserGroups (user);
			this.SetupCustomUISettings (user);
		}

		public override void ApplyValidateRule(AiderUserEntity user)
		{
			this.CheckLoginNameIsNotEmpty (user);
			this.CheckLoginNameIsUnique (user);
			this.CheckDisplayNameIsNotEmpty (user);
			this.CheckParishIsParishGroup (user);
			this.CheckEmail (user);
		}


		private void SetupAuthenticationMethod(AiderUserEntity user)
		{
			user.AuthenticationMethod = UserAuthenticationMethod.Password;
		}

		private void SetupMutability(AiderUserEntity user)
		{
			user.Mutability = Mutability.Customizable;
		}

		private void SetupUserGroups(AiderUserEntity user)
		{
			user.AssignGroup (this.GetBusinessContext (), UserPowerLevel.Standard);
		}

		private void SetupCustomUISettings(AiderUserEntity user)
		{
			var businessContext = this.GetBusinessContext ();

			user.CustomUISettings = businessContext.CreateAndRegisterEntity<SoftwareUISettingsEntity> ();
		}


		private void CheckLoginNameIsNotEmpty(AiderUserEntity user)
		{
			if (string.IsNullOrEmpty (user.LoginName))
			{
				var message = Res.Strings.AiderUserLoginNameEmpty.ToString ();

				Logic.BusinessRuleException (user, message);
			}
		}

		private void CheckLoginNameIsUnique(AiderUserEntity user)
		{
			var example = new AiderUserEntity ()
			{
				LoginName = user.LoginName
			};

			var dataContext = this.GetBusinessContext ().DataContext;

			var usersWithSameLoginName = dataContext.GetByExample (example);

			var isUnique = (usersWithSameLoginName.Count == 0)
				|| (usersWithSameLoginName.Count == 1 && usersWithSameLoginName[0] == user);

			if (!isUnique)
			{
				var message = Res.Strings.AiderUserLoginNameDuplicate.ToString ();

				Logic.BusinessRuleException (user, message);
			}
		}

		private void CheckDisplayNameIsNotEmpty(AiderUserEntity user)
		{
			if (user.DisplayName.IsNullOrEmpty ())
			{
				var message = "Le nom pour l'affichage de l'utilisateur ne peut pas être vide.";

				Logic.BusinessRuleException (user, message);
			}
		}

		private void CheckParishIsParishGroup(AiderUserEntity user)
		{
			if (user.Parish.IsNull () || user.Parish.IsParish ())
			{
				return;
			}

			var message = "La paroisse n'est pas un groupe de paroisse";

			Logic.BusinessRuleException (user, message);
		}

		private void CheckEmail(AiderUserEntity user)
		{
			var email = user.Email;

			if (string.IsNullOrEmpty (email))
			{
				return;
			}

			var fixedEmail = Epsitec.Common.IO.UriBuilder.FixScheme (email);
			var isValidEmail = Epsitec.Common.IO.UriBuilder.IsValidMailTo (fixedEmail);

			if (!isValidEmail)
			{
				var message = Resources.Text ("L'adresse e-mail n'est pas valide.");

				Logic.BusinessRuleException (user, message);
			}
		}
	}
}
