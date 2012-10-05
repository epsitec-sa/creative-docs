//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider
{
	public sealed class AiderUserSession : UserSession
	{
		public AiderUserSession(UserManager manager, string sessionId)
			: base (manager, sessionId)
		{
		}


		public new AiderUserManager				UserManager
		{
			get
			{
				return base.UserManager as AiderUserManager;
			}
		}

		
		public IFilter GetScopeFilter(System.Type entityType)
		{
			if (entityType == typeof (AiderPersonEntity))
			{
				var pattern = this.GetActiveScopePathPattern ();

				if (pattern != null)
				{
					var filter = new LambdaFilter<AiderPersonEntity> (x => SqlMethods.Like (x.Parish.Group.Path, pattern));
					return filter;
				}
			}
			
			return null;
		}

		private string GetActiveScopePathPattern()
		{
			if ((this.activeScope == null) ||
				(string.IsNullOrEmpty (this.activeScope.GroupPath)))
			{
				return null;
			}

			var path = this.activeScope.GroupPath;

			return path + "%";
		}


		public void SetActiveScope(AiderUserScopeEntity scope)
		{
			if (this.activeScope == scope)
			{
				return;
			}

			this.activeScope = scope;

			var user    = this.UserManager.AuthenticatedUser;
			var context = this.UserManager.BusinessContext;

			if (user == null)
			{
				return;
			}

			if (scope.IsNull ())
			{
				scope = null;
			}
			else
			{
				scope = context.GetLocalEntity (scope);
			}


			if ((user.PreferredScope.IsNull () && (scope == null)) ||
				(user.PreferredScope == scope))
			{
				return;
			}

			user.PreferredScope = scope;
			context.SaveChanges (LockingPolicy.ReleaseLock);

			this.OnActiveScopeChanged ();
		}
		
		public AiderUserScopeEntity GetActiveScope()
		{
			if ((this.activeScope == null) &&
				(this.UserManager.AuthenticatedUser != null))
			{
				this.activeScope = this.ResolveActiveScope ();
			}

			return this.activeScope;
		}


		private void OnActiveScopeChanged()
		{
		}
		
		private AiderUserScopeEntity ResolveActiveScope()
		{
			var user = this.UserManager.AuthenticatedUser;

			if (user == null)
			{
				return null;
			}

			var scope = user.PreferredScope;

			if (scope.IsNull ())
			{
				var role = user.Role;

				if (role.IsNotNull ())
				{
					scope = role.DefaultScopes.FirstOrDefault ();

					if (scope.IsNull ())
					{
						scope = user.CustomScopes.FirstOrDefault ();
					}
				}
			}

			this.SetActiveScope (scope);

			return scope;
		}


		private AiderUserScopeEntity			activeScope;
	}
}
