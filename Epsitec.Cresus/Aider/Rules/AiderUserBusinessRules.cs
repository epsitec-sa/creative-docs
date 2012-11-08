//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Support.Extensions;

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
			this.SetupUserGroups (user);
			this.SetupCustomUISettings (user);
		}


		private void SetupUserGroups(AiderUserEntity user)
		{
			this.AssignGroup (user, UserPowerLevel.Standard);
		}


		private void AssignGroup(AiderUserEntity user, UserPowerLevel powerLevel)
		{
			var group = this.GetGroup (powerLevel);

			if (group != null)
			{
				user.UserGroups.Add (group);
			}
		}


		private SoftwareUserGroupEntity GetGroup(UserPowerLevel powerLevel)
		{
			var example = new SoftwareUserGroupEntity ()
			{
				UserPowerLevel = powerLevel
			};

			var dataContext = this.GetBusinessContext ().DataContext;

			return dataContext.GetByExample (example).FirstOrDefault ();
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
			this.UpdateUserGroups (user);
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


		private void UpdateUserGroups(AiderUserEntity user)
		{
			var shouldBeAdmin = user.IsAdministrator;
			var isAdmin = user.HasPowerLevel (UserPowerLevel.Administrator);

			var powerLevel = UserPowerLevel.Administrator;
			
			if (!isAdmin && shouldBeAdmin)
			{
				this.AssignGroup (user, powerLevel);
			}
			else if (isAdmin && !shouldBeAdmin)
			{
				user.UserGroups.RemoveAll
				(
					g => g.UserPowerLevel != UserPowerLevel.None && g.UserPowerLevel <= powerLevel
				);
			}
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
