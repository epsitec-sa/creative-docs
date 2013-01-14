//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Rules
{


	[BusinessRule]
	internal class AiderUserBusinessRules : GenericBusinessRule<AiderUserEntity>
	{


		public override void ApplySetupRule(AiderUserEntity user)
		{
			this.SetupAuthenticationMethod (user);
			this.SetupUserGroups (user);
			this.SetupCustomUISettings (user);
		}


		private void SetupAuthenticationMethod(AiderUserEntity user)
		{
			user.AuthenticationMethod = UserAuthenticationMethod.Password;
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


		public override void ApplyValidateRule(AiderUserEntity user)
		{
			AiderUserBusinessRules.CheckLoginNameIsNotEmpty (user);
			this.CheckLoginNameIsUnique (user);
			AiderUserBusinessRules.UpdateDisplayName (user);
		}


		private static void CheckLoginNameIsNotEmpty(AiderUserEntity user)
		{
			if (string.IsNullOrEmpty (user.LoginName))
			{
				var message = Res.Strings.AiderUserLoginNameEmpty.ToString ();

				throw new BusinessRuleException (user, message);
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

				throw new BusinessRuleException (user, message);
			}
		}
			

		private static void UpdateDisplayName(AiderUserEntity user)
		{
			user.DisplayName = user.Person.IsNotNull ()
				? user.Person.DisplayName
				: user.LoginName;
		}
			

	}


}
