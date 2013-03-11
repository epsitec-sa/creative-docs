//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Override
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


		public override IFilter GetAdditionalFilter(DataSetMetadata dataSetMetadata, AbstractEntity example)
		{
			var entityType = dataSetMetadata.EntityTableMetadata.EntityType;

			if (entityType == typeof (AiderUserEntity))
			{
				return this.GetAiderUserEntityFilter ((AiderUserEntity) example);
			}
			else if (entityType == typeof (AiderGroupDefEntity))
			{
				return this.GetAiderGroupDefEntityFilter ((AiderGroupDefEntity) example);
			}

			return null;
		}

		private IFilter GetAiderUserEntityFilter(AiderUserEntity example)
		{
			var user = this.UserManager.AuthenticatedUser;

			if (!user.HasPowerLevel (UserPowerLevel.Administrator))
			{
				return new LambdaFilter<AiderUserEntity> (x => x.LoginName == user.LoginName);
			}

			return null;
		}

		private IFilter GetAiderGroupDefEntityFilter(AiderGroupDefEntity example)
		{
			return new LambdaFilter<AiderGroupDefEntity> (x => x.Level == AiderGroupIds.TopLevel);
		}

		public override IFilter GetScopeFilter(DataSetMetadata dataSetMetadata, AbstractEntity example)
		{
			var entityType = dataSetMetadata.EntityTableMetadata.EntityType;
			var pattern = this.GetActiveScopePathPattern ();

			if (entityType == typeof (AiderGroupEntity))
			{
				return this.GetAiderGroupEntityFilter ((AiderGroupEntity) example, pattern + AiderGroupIds.SubgroupSqlWildcard);
			}

			if (!string.IsNullOrEmpty (pattern))
			{
				if (entityType == typeof (AiderPersonEntity))
				{
					return this.GetAiderPersonEntityFilter ((AiderPersonEntity) example, pattern + "%");
				}
				else if (entityType == typeof (AiderContactEntity))
				{
					return this.GetAiderContactEntityFilter ((AiderContactEntity) example, pattern + "%");
				}
				else if (entityType == typeof (AiderLegalPersonEntity))
				{
					return this.GetAiderLegalPersonEntityFilter ((AiderLegalPersonEntity) example, pattern + "%");
				}
				else if (entityType == typeof (AiderHouseholdEntity))
				{
					return this.GetAiderHouseholdEntityFilter ((AiderHouseholdEntity) example, pattern + "%");
				}
			}

			return null;
		}

		private IFilter GetAiderPersonEntityFilter(AiderPersonEntity example, string pattern)
		{
			if (example.Parish == null)
			{
				example.Parish = new AiderGroupParticipantEntity ();
			}
			if (example.Parish.Group == null)
			{
				example.Parish.Group = new AiderGroupEntity ();
			}

			return new LambdaFilter<AiderPersonEntity> (x => SqlMethods.Like (x.Parish.Group.Path, pattern));
		}

		private IFilter GetAiderGroupEntityFilter(AiderGroupEntity example, string pattern)
		{
			return new LambdaFilter<AiderGroupEntity> (x => SqlMethods.Like (x.Path, pattern));
		}

		private IFilter GetAiderContactEntityFilter(AiderContactEntity example, string pattern)
		{
			return new LambdaFilter<AiderContactEntity> (x => SqlMethods.Like (x.ParishGroupPathCache, pattern));
		}

		private IFilter GetAiderLegalPersonEntityFilter(AiderLegalPersonEntity example, string pattern)
		{
			return new LambdaFilter<AiderLegalPersonEntity> (x => SqlMethods.Like (x.ParishGroupPathCache, pattern));
		}

		private IFilter GetAiderHouseholdEntityFilter(AiderHouseholdEntity example, string pattern)
		{
			return new LambdaFilter<AiderHouseholdEntity> (x => SqlMethods.Like (x.ParishGroupPathCache, pattern));
		}

		private string GetActiveScopePathPattern()
		{
			var scope = this.GetActiveScope ();

			if (scope == null)
			{
				return null;
			}

			var pattern = scope.GroupPath;

			if (pattern != null)
			{
				var user = this.UserManager.AuthenticatedUser;
				var parishPath = user.ParishGroupPathCache;

				pattern = AiderGroupIds.ReplacePlaceholders (pattern, parishPath);
			}

			return pattern;
		}


		public override IEnumerable<UserScope> GetAvailableUserScopes()
		{
			var user = this.UserManager.AuthenticatedUser;

			return user.Role.DefaultScopes
				.Distinct ()
				.Select (s => this.GetUserScope (s))
				.ToList ();
		}

		public override UserScope GetActiveUserScope()
		{
			var activeScope = this.GetActiveScope ();

			if (activeScope == null)
			{
				return null;
			}

			return this.GetUserScope (activeScope);
		}

		private UserScope GetUserScope(AiderUserScopeEntity scope)
		{
			var dataContext = this.UserManager.BusinessContext.DataContext;

			var id = dataContext.GetNormalizedEntityKey (scope).Value.ToString ();
			var name = scope.Name;

			return new UserScope (id, name);
		}

		public override void SetActiveUserScope(string scopeId)
		{
			var entityKey = EntityKey.Parse (scopeId);
			var dataContext = this.UserManager.BusinessContext.DataContext;

			var scope = (AiderUserScopeEntity) dataContext.ResolveEntity (entityKey);

			this.SetActiveScope (scope);
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
				}

				if (scope.IsNull ())
				{
					scope = null;
				}
			}

			this.SetActiveScope (scope);

			return scope;
		}


		private AiderUserScopeEntity			activeScope;
	}
}
