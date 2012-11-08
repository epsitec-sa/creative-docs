//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
#if true
	public static class Hack
	{
		public static void PopulateUsers(BusinessContext businessContext)
		{
			var role = businessContext.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var groupSystem     = Hack.CreateUserGroup (businessContext, role, "Système", UserPowerLevel.System);
			var groupDev        = Hack.CreateUserGroup (businessContext, role, "Développeurs", UserPowerLevel.Developer);
			var groupAdmin      = Hack.CreateUserGroup (businessContext, role, "Administrateurs", UserPowerLevel.Administrator);
			var groupPowerUser  = Hack.CreateUserGroup (businessContext, role, "Utilisateurs avec pouvoir", UserPowerLevel.PowerUser);
			var groupStandard   = Hack.CreateUserGroup (businessContext, role, "Utilisateurs standards", UserPowerLevel.Standard);
			var groupRestricted = Hack.CreateUserGroup (businessContext, role, "Utilisateurs restreints", UserPowerLevel.Restricted);

			var userStandard1 = Hack.CreateUser (businessContext, groupDev, "Pierre Arnaud", "arnaud", "smaky", UserAuthenticationMethod.System);
			var userStandard2 = Hack.CreateUser (businessContext, groupDev, "Marc Bettex", "Marc", "tiger", UserAuthenticationMethod.System);
			var userStandard3 = Hack.CreateUser (businessContext, groupDev, "Daniel Roux", "Daniel", "blupi", UserAuthenticationMethod.System);
			var userEpsitec   = Hack.CreateUser (businessContext, groupDev, "Epsitec", "Epsitec", "admin", UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			businessContext.SaveChanges (LockingPolicy.ReleaseLock);
		}

		private static SoftwareUserGroupEntity CreateUserGroup(BusinessContext businessContext, SoftwareUserRoleEntity role, string name, UserPowerLevel level)
		{
			var group = businessContext.CreateAndRegisterEntity<SoftwareUserGroupEntity> ();

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);

			return group;
		}

		private static SoftwareUserEntity CreateUser(BusinessContext businessContext, SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, UserAuthenticationMethod am)
		{
			var userType  = CoreContext.ResolveType (typeof (SoftwareUserEntity));
			var userDruid = EntityInfo.GetTypeId (userType);

			var user = businessContext.CreateEntity (userDruid) as SoftwareUserEntity;
			businessContext.Register (user);
			var settings = businessContext.CreateAndRegisterEntity<SoftwareUISettingsEntity> ();

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
