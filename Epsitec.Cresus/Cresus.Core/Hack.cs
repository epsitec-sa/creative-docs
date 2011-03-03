//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
#if true
	public static class Hack
	{
		public static void PopulateUsers(DataContext context)
		{
			var role = context.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var groupSystem     = Hack.CreateUserGroup (context, role, "Système", Business.UserManagement.UserPowerLevel.System);
			var groupDev        = Hack.CreateUserGroup (context, role, "Développeurs", Business.UserManagement.UserPowerLevel.Developer);
			var groupAdmin      = Hack.CreateUserGroup (context, role, "Administrateurs", Business.UserManagement.UserPowerLevel.Administrator);
			var groupPowerUser  = Hack.CreateUserGroup (context, role, "Utilisateurs avec pouvoir", Business.UserManagement.UserPowerLevel.PowerUser);
			var groupStandard   = Hack.CreateUserGroup (context, role, "Utilisateurs standards", Business.UserManagement.UserPowerLevel.Standard);
			var groupRestricted = Hack.CreateUserGroup (context, role, "Utilisateurs restreints", Business.UserManagement.UserPowerLevel.Restricted);

#if false
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 1", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 2", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 3", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 4", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 5", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 6", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 7", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 8", Business.UserManagement.UserPowerLevel.Restricted);
#endif

			var userStandard1 = Hack.CreateUser (context, groupDev, "Pierre Arnaud", "arnaud", "smaky", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard2 = Hack.CreateUser (context, groupDev, "Marc Bettex", "Marc", "tiger", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard3 = Hack.CreateUser (context, groupDev, "Daniel Roux", "Daniel", "blupi", Business.UserManagement.UserAuthenticationMethod.System);
			var userEpsitec   = Hack.CreateUser (context, groupDev, "Epsitec", "Epsitec", "admin", Business.UserManagement.UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			context.SaveChanges ();
		}

		private static SoftwareUserGroupEntity CreateUserGroup(DataContext context, SoftwareUserRoleEntity role, string name, Business.UserManagement.UserPowerLevel level)
		{
			var group = context.CreateEntity<SoftwareUserGroupEntity> ();
			var logic = new Logic (group);

			logic.ApplyRules (RuleType.Setup, group);

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);

			return group;
		}

		private static SoftwareUserEntity CreateUser(DataContext context, SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, Business.UserManagement.UserAuthenticationMethod am)
		{
			var user = context.CreateEntity<SoftwareUserEntity> ();
			var logic = new Logic (user);

			logic.ApplyRules (RuleType.Setup, user);

			user.AuthenticationMethod = am;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);

#if false
			FormattedText[] p = displayName.Split (" ");
			if (p.Length == 2)
			{
				var person = this.SearchNaturalPerson (p[0].ToString (), p[1].ToString ());
				if (person.IsNotNull ())
				{
					user.Person = person;
				}
			}
#endif

			return user;
		}

		private static string SearchNaturalPerson(string firstName, string lastName)
		{
			return null;
		}

#if false
		private NaturalPersonEntity SearchNaturalPerson(DataContext context, string firstName, string lastName)
		{
			var example = new NaturalPersonEntity ();
			example.Firstname = firstName;
			example.Lastname = lastName;

			return context.GetByExample<NaturalPersonEntity> (example).FirstOrDefault ();
		}
#endif

	}
#endif
}
