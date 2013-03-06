using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;


namespace Epsitec.Aider.Override
{


	public class AiderDatabaseInitializer : DatabaseInitializer
	{


		public AiderDatabaseInitializer(BusinessContext businessContext)
			: base (businessContext)
		{
		}
		

		public override void Run()
		{
			base.Run ();

			this.CreateAiderScopes ();
			this.CreateAiderRoles ();
			this.CreateAdministrator ();
		}


		private void CreateAiderScopes()
		{
			this.scopeCounty = this.CreateAiderScope
			(
				name:"Canton", 
				path:""
			);

			this.scopeRegion = this.CreateAiderScope 
			(
				name:"Région",
				path:"<R>."
			);

			this.scopeParish = this.CreateAiderScope
			(
				name:"Paroisse", 
				path:"<R>.<P>."
			);
		}


		private AiderUserScopeEntity CreateAiderScope(string name, string path)
		{
			var scope = this.BusinessContext.CreateAndRegisterEntity<AiderUserScopeEntity> ();

			scope.Name = name;
			scope.GroupPath = path;
			scope.Mutability = Mutability.SystemDefined;

			return scope;
		}


		private void CreateAiderRoles()
		{
			this.roleAdministrator = this.CreateAiderRole
			(
				name: AiderUserRoleEntity.AdminRole,
				scopes: new AiderUserScopeEntity[] { 
					this.scopeCounty
				}
			);

			this.roleCounty = this.CreateAiderRole
			(
				name: AiderUserRoleEntity.CountyRole,
				scopes: new AiderUserScopeEntity[] { 
					this.scopeCounty,
					this.scopeRegion,
					this.scopeParish
				}
			);

			this.roleRegion = this.CreateAiderRole
			(
				name: AiderUserRoleEntity.RegionRole,
				scopes: new AiderUserScopeEntity[] { 
					this.scopeRegion,
					this.scopeParish
				}
			);
			this.roleParish = this.CreateAiderRole
			(
				name: AiderUserRoleEntity.ParishRole,
				scopes: new AiderUserScopeEntity[] { 
					this.scopeParish
				}
			);
		}


		private AiderUserRoleEntity CreateAiderRole(string name, params AiderUserScopeEntity[] scopes)
		{
			var role = this.BusinessContext.CreateAndRegisterEntity<AiderUserRoleEntity> ();

			role.Name = name;
			role.DefaultScopes.AddRange (scopes);
			role.Mutability = Mutability.SystemDefined;

			return role;
		}


		private void CreateAdministrator()
		{
			var admininstrator = (AiderUserEntity) this.CreateUser
			(
				displayName: "Administrateur",
				userLogin: "administrateur",
				userPassword: "mysuperadministrateurpassword",
				authentificationMethod: UserAuthenticationMethod.Password,
				groups: new SoftwareUserGroupEntity[] {
					this.GroupAdministrator,
					this.GroupStandard
				}
			);

			admininstrator.Role = this.roleAdministrator;
		}


		private AiderUserScopeEntity scopeCounty;
		private AiderUserScopeEntity scopeRegion;
		private AiderUserScopeEntity scopeParish;
		private AiderUserRoleEntity roleAdministrator;
		private AiderUserRoleEntity roleCounty;
		private AiderUserRoleEntity roleRegion;
		private AiderUserRoleEntity roleParish;


	}


}
