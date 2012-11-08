//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
#if true
	public static class Hack
	{
		public static void PopulateUsers(BusinessContext businessContext)
		{
			var context = businessContext.DataContext;

			var role = context.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var groupSystem     = Hack.CreateUserGroup (context, role, "Système", UserPowerLevel.System);
			var groupDev        = Hack.CreateUserGroup (context, role, "Développeurs", UserPowerLevel.Developer);
			var groupAdmin      = Hack.CreateUserGroup (context, role, "Administrateurs", UserPowerLevel.Administrator);
			var groupPowerUser  = Hack.CreateUserGroup (context, role, "Utilisateurs avec pouvoir", UserPowerLevel.PowerUser);
			var groupStandard   = Hack.CreateUserGroup (context, role, "Utilisateurs standards", UserPowerLevel.Standard);
			var groupRestricted = Hack.CreateUserGroup (context, role, "Utilisateurs restreints", UserPowerLevel.Restricted);

			var userStandard1 = Hack.CreateUser (context, groupDev, "Pierre Arnaud", "arnaud", "smaky", UserAuthenticationMethod.System);
			var userStandard2 = Hack.CreateUser (context, groupDev, "Marc Bettex", "Marc", "tiger", UserAuthenticationMethod.System);
			var userStandard3 = Hack.CreateUser (context, groupDev, "Daniel Roux", "Daniel", "blupi", UserAuthenticationMethod.System);
			var userEpsitec   = Hack.CreateUser (context, groupDev, "Epsitec", "Epsitec", "admin", UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			context.SaveChanges ();
		}

		private static SoftwareUserGroupEntity CreateUserGroup(DataContext context, SoftwareUserRoleEntity role, string name, UserPowerLevel level)
		{
			var group = context.CreateEntity<SoftwareUserGroupEntity> ();
			var logic = new Logic (group);

			logic.ApplyRule (RuleType.Setup, group);

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);

			return group;
		}

		private static SoftwareUserEntity CreateUser(DataContext context, SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, UserAuthenticationMethod am)
		{
			var userType  = CoreContext.ResolveType (typeof (SoftwareUserEntity));
			var userDruid = EntityInfo.GetTypeId (userType);

			var user = context.CreateEntity (userDruid) as SoftwareUserEntity;
			var settings = context.CreateEntity<SoftwareUISettingsEntity> ();
			var logic = new Logic (user);

			logic.ApplyRule (RuleType.Setup, user);

			user.AuthenticationMethod = am;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);
			user.CustomUISettings = settings;

			return user;
		}
	}
#endif
}
