//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core
{
#if false
	public class Hack
	{
		public void PopulateUsers()
		{
			var role = this.DataContext.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var groupSystem     = this.CreateUserGroup (role, "Système", Business.UserManagement.UserPowerLevel.System);
			var groupDev        = this.CreateUserGroup (role, "Développeurs", Business.UserManagement.UserPowerLevel.Developer);
			var groupAdmin      = this.CreateUserGroup (role, "Administrateurs", Business.UserManagement.UserPowerLevel.Administrator);
			var groupPowerUser  = this.CreateUserGroup (role, "Utilisateurs avec pouvoir", Business.UserManagement.UserPowerLevel.PowerUser);
			var groupStandard   = this.CreateUserGroup (role, "Utilisateurs standards", Business.UserManagement.UserPowerLevel.Standard);
			var groupRestricted = this.CreateUserGroup (role, "Utilisateurs restreints", Business.UserManagement.UserPowerLevel.Restricted);

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

			var userStandard1 = this.CreateUser (groupDev, "Pierre Arnaud", "arnaud", "smaky", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard2 = this.CreateUser (groupDev, "Marc Bettex", "Marc", "tiger", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard3 = this.CreateUser (groupDev, "Daniel Roux", "Daniel", "blupi", Business.UserManagement.UserAuthenticationMethod.System);
			var userEpsitec   = this.CreateUser (groupDev, "Epsitec", "Epsitec", "admin", Business.UserManagement.UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			this.DataContext.SaveChanges ();
		}

		private SoftwareUserGroupEntity CreateUserGroup(SoftwareUserRoleEntity role, string name, Business.UserManagement.UserPowerLevel level)
		{
			var group = this.DataContext.CreateEntity<SoftwareUserGroupEntity> ();
			var logic = new Logic (group, null);

			logic.ApplyRules (RuleType.Setup, group);

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);

			return group;
		}

		private SoftwareUserEntity CreateUser(SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, Business.UserManagement.UserAuthenticationMethod am)
		{
			var user = this.DataContext.CreateEntity<SoftwareUserEntity> ();
			var logic = new Logic (user, null);

			logic.ApplyRules (RuleType.Setup, user);

			user.AuthenticationMethod = am;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);

			FormattedText[] p = displayName.Split (" ");
			if (p.Length == 2)
			{
				var person = this.SearchNaturalPerson (p[0].ToString (), p[1].ToString ());
				if (person.IsNotNull ())
				{
					user.Person = person;
				}
			}

			return user;
		}

		private string SearchNaturalPerson(string firstName, string lastName)
		{
			return null;
		}

#if false
		private NaturalPersonEntity SearchNaturalPerson(string firstName, string lastName)
		{
			var example = new NaturalPersonEntity ();
			example.Firstname = firstName;
			example.Lastname = lastName;

			return this.DataContext.GetByExample<NaturalPersonEntity> (example).FirstOrDefault ();
		}
#endif

	}
#endif
}
