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
			AiderUserBusinessRules.SetupUserGroups (user);
			AiderUserBusinessRules.SetupCustomUISettings (user);
		}


		private static void SetupUserGroups(AiderUserEntity user)
		{
			var standardGroup = AiderUserBusinessRules.GetStandardUserGroup (user);

			user.UserGroups.Add (standardGroup);
		}


		private static void SetupCustomUISettings(AiderUserEntity user)
		{
			var businessContext = BusinessContextPool.GetCurrentContext (user);

			user.CustomUISettings = businessContext.CreateEntity<SoftwareUISettingsEntity> ();
		}


		private static SoftwareUserGroupEntity GetStandardUserGroup(AiderUserEntity user)
		{
			var example = new SoftwareUserGroupEntity ()
			{
				UserPowerLevel = UserPowerLevel.Standard
			};

			var dataContext = BusinessContextPool.GetCurrentContext (user).DataContext;

			var standardGroups = dataContext.GetByExample (example);

			return standardGroups[0];
		}


		public override void ApplyValidateRule(AiderUserEntity user)
		{
			AiderUserBusinessRules.CheckLoginNameIsNotEmpty (user);
			AiderUserBusinessRules.CheckLoginNameIsUnique (user);
			AiderUserBusinessRules.UpdateDisplayName (user);
			AiderUserBusinessRules.UpdatePassword (user);
		}


		private static void CheckLoginNameIsNotEmpty(AiderUserEntity user)
		{
			if (string.IsNullOrEmpty (user.LoginName))
			{
				var message = Res.Strings.AiderUserLoginNameEmpty.ToString ();

				throw new BusinessRuleException (user, message);
			}
		}


		private static void CheckLoginNameIsUnique(AiderUserEntity user)
		{
			var example = new AiderUserEntity ()
			{
				LoginName = user.LoginName
			};

			var dataContext = BusinessContextPool.GetCurrentContext (user).DataContext;

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


		private static void UpdatePassword(AiderUserEntity user)
		{
			var password = user.ClearPassword;
			var confirmation = user.ClearPasswordConfirmation;

			if (password == null && confirmation == null)
			{
				return;
			}

			if (password.Length < 8)
			{
				var message = Res.Strings.AiderUserPasswordTooShort.ToString ();

				throw new BusinessRuleException (user, message);
			}

			if (password != confirmation)
			{
				var message = Res.Strings.AiderUserPasswordMismatch.ToString ();

				throw new BusinessRuleException (user, message);
			}

			user.SetPassword (password);
		}
			

	}


}
