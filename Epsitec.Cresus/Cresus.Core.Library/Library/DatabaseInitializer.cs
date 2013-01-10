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
		/// <summary>
		/// This method is called to initialize the database with initial data after its creation.
		/// </summary>
		/// <remarks>
		/// Don't bother to save changes in your implementation of this method, as it will be saved
		/// by the caller of this method.
		/// </remarks>
		/// <param name="businessContext">The BusinessContext used to create entities. </param>
		public virtual void Run(BusinessContext businessContext)
		{
			this.CreateBasicRoles (businessContext);
			this.CreateBasicGroups (businessContext);
			this.CreateBasicUsers (businessContext);
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

		private void CreateBasicRoles(BusinessContext businessContext)
		{
			this.roleMain = this.CreateRole
			(
                businessContext: businessContext, 
                code: "?", 
                name: "Principal"
			);
		}

		protected SoftwareUserRoleEntity CreateRole(BusinessContext businessContext, string code, string name)
		{
			var role = businessContext.CreateAndRegisterEntity<SoftwareUserRoleEntity> ();

			role.Code = code;
			role.Name = name;

			return role;
		}

		private void CreateBasicGroups(BusinessContext businessContext)
		{
			this.groupSystem = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Système",
				code: "?",
				level: UserPowerLevel.System
			);

			this.groupDeveloper = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Développeurs",
				code: "?",
				level: UserPowerLevel.Developer
			);

			this.groupAdministrator = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Administrateurs",
				code: "?",
				level: UserPowerLevel.Administrator
			);

			this.groupPowerUser = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Utilisateurs avec pouvoir",
				code: "?",
				level: UserPowerLevel.PowerUser
			);

			this.groupStandard = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Utilisateurs standards",
				code: "?",
				level: UserPowerLevel.Standard
			);

			this.groupRestricted = this.CreateUserGroup
			(
				businessContext: businessContext,
				role: this.RoleMain,
				name: "Utilisateurs restreints",
				code: "?",
				level: UserPowerLevel.Restricted
			);
		}

		protected SoftwareUserGroupEntity CreateUserGroup(BusinessContext businessContext, SoftwareUserRoleEntity role, string name, string code, UserPowerLevel level)
		{
			var group = businessContext.CreateAndRegisterEntity<SoftwareUserGroupEntity> ();

			group.Code = code;
			group.Name = name;
			group.UserPowerLevel = level;

			group.Roles.Add (role);

			return group;
		}

		private void CreateBasicUsers(BusinessContext businessContext)
		{
			this.CreateUser
			(
				businessContext: businessContext,
				displayName: "Root",
				userLogin: "Root",
				userPassword: "mySuperRootPassword",
				authentificationMethod: UserAuthenticationMethod.Password,
				groups: this.groupAdministrator
			);
		}

		protected void CreateTestUsers(BusinessContext businessContext)
		{
			this.CreateUser
			(
				businessContext: businessContext,
				displayName: "Pierre Arnaud",
				userLogin: "arnaud",
				userPassword: "smaky",
				authentificationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] { 
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				businessContext: businessContext,
				displayName: "Marc Bettex",
				userLogin: "Marc",
				userPassword: "tiger",
				authentificationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] { 
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				businessContext: businessContext,
				displayName: "Daniel Roux",
				userLogin: "Daniel",
				userPassword: "blupi",
				authentificationMethod: UserAuthenticationMethod.System,
				groups: new SoftwareUserGroupEntity[] { 
					this.GroupDeveloper,
					this.GroupStandard
				}
			);

			this.CreateUser
			(
				businessContext: businessContext,
				displayName: "Epsitec",
				userLogin: "Epsitec",
				userPassword: "admin",
				authentificationMethod: UserAuthenticationMethod.Password,
				groups: new SoftwareUserGroupEntity[] { 
					this.GroupDeveloper,
					this.GroupAdministrator
				}
			);
		}

		protected SoftwareUserEntity CreateUser(BusinessContext businessContext, FormattedText displayName, string userLogin, string userPassword, UserAuthenticationMethod authentificationMethod, params SoftwareUserGroupEntity[] groups)
		{
			var userType = CoreContext.ResolveType (typeof (SoftwareUserEntity));
			var userDruid = EntityInfo.GetTypeId (userType);

			var user = (SoftwareUserEntity) businessContext.CreateEntity (userDruid);
			businessContext.Register (user);

			user.AuthenticationMethod = authentificationMethod;
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

		private SoftwareUserRoleEntity roleMain;
		private SoftwareUserGroupEntity groupSystem;
		private SoftwareUserGroupEntity groupDeveloper;
		private SoftwareUserGroupEntity groupAdministrator;
		private SoftwareUserGroupEntity groupPowerUser;
		private SoftwareUserGroupEntity groupStandard;
		private SoftwareUserGroupEntity groupRestricted;
	}
}
