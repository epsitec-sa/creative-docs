//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// This is the base class used to setup the database with initial data after its creation. You
	/// should create a descendent in your app project and configure the program to use your class
	/// instead of this one, by a type substitution in the .crconfig file.
	/// </summary>
	public class DatabaseInitializer
	{
		public DatabaseInitializer(BusinessContext businessContext)
		{
			this.businessContext = businessContext;
		}

		/// <summary>
		/// This method is called to initialize the database with initial data after its creation.
		/// </summary>
		/// <remarks>
		/// Don't bother to save changes in your implementation of this method, as it will be saved
		/// by the caller of this method.
		/// </remarks>
		/// <param name="businessContext">The BusinessContext used to create entities. </param>
		public virtual void Run()
		{
			this.CreateBasicRoles ();
			this.CreateBasicGroups ();
			this.CreateBasicUsers ();
		}

		protected BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		protected SoftwareUserRoleEntity RoleMain
		{
			get
			{
				return this.roleMain;
			}
		}

		protected SoftwareUserGroupEntity GroupSystem
		{
			get
			{
				return this.groupSystem;
			}
		}

		protected SoftwareUserGroupEntity GroupDeveloper
		{
			get
			{
				return this.groupDeveloper;
			}
		}

		protected SoftwareUserGroupEntity GroupAdministrator
		{
			get
			{
				return this.groupAdministrator;
			}
		}

		protected SoftwareUserGroupEntity GroupPowerUser
		{
			get
			{
				return this.groupPowerUser;
			}
		}

		protected SoftwareUserGroupEntity GroupStandard
		{
			get
			{
				return this.groupStandard;
			}
		}

		protected SoftwareUserGroupEntity GroupRestricted
		{
			get
			{
				return this.groupRestricted;
			}
		}

		protected SoftwareUserEntity RootUser
		{
			get
			{
				return this.rootUser;
			}
		}

		private void CreateBasicRoles()
		{
			this.roleMain = this.CreateRole
			(
				code: "?", 
				name: "Principal"
			);
		}

		protected SoftwareUserRoleEntity CreateRole(string code, string name)
		{
			var role = this.businessContext.CreateAndRegisterEntity<SoftwareUserRoleEntity> ();

			role.Code = code;
			role.Name = name;

			return role;
		}

		private void CreateBasicGroups()
		{
			this.groupSystem = this.CreateUserGroup
			(
				role: this.RoleMain,
				name: "Système",
				code: "?",
				level: UserPowerLevel.System
			);

			this.groupDeveloper = this.CreateUserGroup
			(
				role: this.RoleMain,
				name: "Développeurs",
				code: "?",
				level: UserPowerLevel.Developer
			);

            this.groupAdministrator = this.CreateUserGroup
            (
                role: this.RoleMain,
                name: "Administrateurs système",
                code: "?",
                level: UserPowerLevel.Administrator
            );

            this.groupAdminUser = this.CreateUserGroup
            (
                role: this.RoleMain,
                name: "Administrateurs standards",
                code: "?",
                level: UserPowerLevel.AdminUser
            );

            this.groupPowerUser = this.CreateUserGroup
			(
				role: this.RoleMain,
				name: "Utilisateurs avec pouvoir",
				code: "?",
				level: UserPowerLevel.PowerUser
			);

			this.groupStandard = this.CreateUserGroup
			(
				role: this.RoleMain,
				name: "Utilisateurs standards",
				code: "?",
				level: UserPowerLevel.Standard
			);

			this.groupRestricted = this.CreateUserGroup
			(
				role: this.RoleMain,
				name: "Utilisateurs restreints",
				code: "?",
				level: UserPowerLevel.Restricted
			);
		}

		private void CreateBasicUsers()
		{
			this.rootUser = this.CreateUser
			(
				displayName: "Root",
				userLogin: "root",
				userPassword: "mysuperrootpassword",
                authenticationMethod: UserAuthenticationMethod.Password,
				groups: new SoftwareUserGroupEntity[] {
					this.groupAdministrator,
					this.GroupStandard
				}
			);
		}

		protected void CreateTestUsers()
		{
			this.CreateUser
			(
				displayName: "Pierre Arnaud",
				userLogin: "arnaud",
				userPassword: "smaky",
                authenticationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] {
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				displayName: "Marc Bettex",
				userLogin: "Marc",
				userPassword: "tiger",
                authenticationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] {
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				displayName: "Daniel Roux",
				userLogin: "Daniel",
				userPassword: "blupi",
                authenticationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] {
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				displayName: "Epsitec",
				userLogin: "Epsitec",
				userPassword: "admin",
                authenticationMethod: UserAuthenticationMethod.Password,
				groups: new SoftwareUserGroupEntity[] {
					this.GroupAdministrator,
					this.GroupDeveloper,
					this.groupStandard
				}
			);
		}

		protected SoftwareUserEntity CreateUser(string displayName, string userLogin, string userPassword, UserAuthenticationMethod authenticationMethod, params SoftwareUserGroupEntity[] groups)
        {
            return DatabaseInitializer.CreateUser (this.businessContext, displayName, userLogin, userPassword, authenticationMethod, groups);
        }

        protected SoftwareUserGroupEntity CreateUserGroup(SoftwareUserRoleEntity role, string name, string code, UserPowerLevel level)
        {
            return DatabaseInitializer.CreateUserGroup (this.businessContext, role, name, code, level);
        }


        public static SoftwareUserGroupEntity CreateUserGroup(BusinessContext businessContext, SoftwareUserRoleEntity role, string name, string code, UserPowerLevel level)
        {
            var group = businessContext.CreateAndRegisterEntity<SoftwareUserGroupEntity> ();

            group.Code = code;
            group.Name = name;
            group.UserPowerLevel = level;

            group.Roles.Add (role);

            return group;
        }

        public static SoftwareUserEntity CreateUser(BusinessContext businessContext, string displayName, string userLogin, string userPassword, UserAuthenticationMethod authenticationMethod, params SoftwareUserGroupEntity[] groups)
        {
			var userType = CoreContext.ResolveType (typeof (SoftwareUserEntity));
			var userDruid = EntityInfo.GetTypeId (userType);

            var user = (SoftwareUserEntity)businessContext.CreateEntity (userDruid);
            businessContext.Register (user);

			user.AuthenticationMethod = authenticationMethod;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.SetPassword (userPassword);
			user.CustomUISettings = businessContext.CreateAndRegisterEntity<SoftwareUISettingsEntity> ();

			foreach (var group in groups)
			{
				user.UserGroups.Add (group);
			}

			return user;
		}


        private readonly BusinessContext businessContext;

        private SoftwareUserRoleEntity roleMain;

		private SoftwareUserGroupEntity groupSystem;
		private SoftwareUserGroupEntity groupDeveloper;
		private SoftwareUserGroupEntity groupAdministrator;
        private SoftwareUserGroupEntity groupAdminUser;
        private SoftwareUserGroupEntity groupPowerUser;
		private SoftwareUserGroupEntity groupStandard;
		private SoftwareUserGroupEntity groupRestricted;

		private SoftwareUserEntity rootUser;
	}
}
