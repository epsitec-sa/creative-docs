using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

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
				name: "Accès cantonnal",
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

			this.CreateAiderTestUser ("p.arnaud", false);		// Pierre Arnaud
			this.CreateAiderTestUser ("m.bettex", false);		// Marc Bettex
			this.CreateAiderTestUser ("b.bolay", false);		// Bernard Bolay
			this.CreateAiderTestUser ("e.bornand", false);		// Eric Bornand
			this.CreateAiderTestUser ("j.brand", true);			// Jacques Brand
			this.CreateAiderTestUser ("g.butticaz", false);		// Geneviève Butticaz
			this.CreateAiderTestUser ("b.corbaz", false);		// Benjamin Corbaz
			this.CreateAiderTestUser ("c.cuendet", false);		// Claude Cuendet
			this.CreateAiderTestUser ("l.dewarrat", false);		// Laurence Dewarrat
			this.CreateAiderTestUser ("d.fankhauser", false);	// Damaris Fankhauser
			this.CreateAiderTestUser ("m.genoux", false);		// Michel Genoux
			this.CreateAiderTestUser ("c.jackson", false);		// Cheryl Jackson
			this.CreateAiderTestUser ("p.jarne", false);		// Pierrette Jarne
			this.CreateAiderTestUser ("g.jaton", true);			// Gérard Jaton
			this.CreateAiderTestUser ("y.knecht", false);		// Yvonne Knecht
			this.CreateAiderTestUser ("l.pestalozzi", false);	// Lorenzo Pestalozzi
			this.CreateAiderTestUser ("d.rochat", false);		// Dorothée Rochat (is not in imported persons. Her address is: Avenue du Prieuré 2b, 1009 Pully)
			this.CreateAiderTestUser ("p.rouge", false);		// Pascal Rouge
			this.CreateAiderTestUser ("j.sordet", true);		// Jean-Michel Sordet
			this.CreateAiderTestUser ("j.sotornik", false);		// Jeanette Sotornik
			this.CreateAiderTestUser ("j.spothelfer", false);	// Jean-Marc Spothelfer
			this.CreateAiderTestUser ("s.wohlhauser", false);	// Sylvie Wohlhauser
		}


		private void CreateAiderTestUser(string login, bool admin)
		{
			var groups = admin
				? new SoftwareUserGroupEntity[] { this.GroupStandard, this.GroupAdministrator }
				: new SoftwareUserGroupEntity[] { this.GroupStandard };

			var user = (AiderUserEntity) this.CreateUser
			(
				displayName: login,
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
