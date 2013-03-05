using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;


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
			this.CreateAiderTestUsers ();
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
				name: "Administrateur",
				scopes: new AiderUserScopeEntity[] { 
					this.scopeCounty
				}
			);

			this.roleCounty = this.CreateAiderRole
			(
				name: "Accès cantonal",
				scopes: new AiderUserScopeEntity[] { 
					this.scopeCounty,
					this.scopeRegion,
					this.scopeParish
				}
			);

			this.roleRegion = this.CreateAiderRole
			(
				name: "Accès régional",
				scopes: new AiderUserScopeEntity[] { 
					this.scopeRegion,
					this.scopeParish
				}
			);
			this.roleParish = this.CreateAiderRole
			(
				name: "Accès paroissial",
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


		private void CreateAiderTestUsers()
		{
			// Don't forget to assign them to the persons once the db is initialized.

			this.CreateAiderTestUser ("p.arnaud", "Pierre Arnaud", false);
			this.CreateAiderTestUser ("m.bettex", "Marc Bettex", false);
			this.CreateAiderTestUser ("b.bolay", "Bernard Bolay", false);
			this.CreateAiderTestUser ("e.bornand", "Eric Bornand", false);
			this.CreateAiderTestUser ("j.brand", "Jacques Brand", true);
			this.CreateAiderTestUser ("g.butticaz", "Geneviève Butticaz", false);
			this.CreateAiderTestUser ("b.corbaz", "Benjamin Corbaz", false);
			this.CreateAiderTestUser ("c.cuendet", "Claude Cuendet", false);
			this.CreateAiderTestUser ("l.dewarrat", "Laurence Dewarrat", false);
			this.CreateAiderTestUser ("d.fankhauser", "Damaris Fankhauser", false);
			this.CreateAiderTestUser ("m.genoux", "Michel Genoux", false);
			this.CreateAiderTestUser ("m.gonce", "Maurice Gonce", false);
			this.CreateAiderTestUser ("c.jackson", "Cheryl Jackson", false);
			this.CreateAiderTestUser ("p.jarne", "Pierrette Jarne", false);
			this.CreateAiderTestUser ("g.jaton", "Gérard Jaton", true);
			this.CreateAiderTestUser ("y.knecht", "Yvonne Knecht", false);
			this.CreateAiderTestUser ("v.mennet", "Vanina Mennet", false);
			this.CreateAiderTestUser ("l.pestalozzi", "Lorenzo Pestalozzi", false);
			this.CreateAiderTestUser ("d.rochat", "Dorothée Rochat", false);
			this.CreateAiderTestUser ("p.rouge", "Pascal Rouge", false);
			this.CreateAiderTestUser ("j.sordet", "Jean-Michel Sordet", true);
			this.CreateAiderTestUser ("j.sotornik", "Jeanette Sotornik", false);
			this.CreateAiderTestUser ("j.spothelfer", "Jean-Marc Spothelfer", false);
			this.CreateAiderTestUser ("s.wohlhauser", "Sylvie Wohlhauser", false);
		}


		private void CreateAiderTestUser(string login, string name, bool admin)
		{
			var groups = admin
				? new SoftwareUserGroupEntity[] { this.GroupStandard, this.GroupAdministrator }
				: new SoftwareUserGroupEntity[] { this.GroupStandard };

			var user = (AiderUserEntity) this.CreateUser
			(
				displayName: FormattedText.FromSimpleText (name),
				userLogin: login,
				userPassword: "monsupermotdepasse",
				authentificationMethod: UserAuthenticationMethod.Password,
				groups: groups
			);

			user.Role = this.roleCounty;
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
