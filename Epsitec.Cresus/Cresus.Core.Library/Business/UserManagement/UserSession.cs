//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Settings;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	public class UserSession : System.IDisposable
	{
		public UserSession(UserManager manager, string sessionId)
		{
			this.userManager = manager;
			this.sessionId = sessionId;
		}


		public UserManager						UserManager
		{
			get
			{
				return this.userManager;
			}
		}
		
		public string							SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public SoftwareUISettingsEntity			SoftwareUISettings
		{
			get
			{
				if ((this.userManager == null) ||
					(this.userManager.AuthenticatedUser.IsNull ()) ||
					(this.userManager.AuthenticatedUser.CustomUISettings.IsNull ()))
				{
					return null;
				}
				
				return this.userManager.AuthenticatedUser.CustomUISettings;
			}
		}

		public UserEntityTableSettings GetTableSettings(System.Type entityType)
		{
			var context = this.userManager.BusinessContext;

			return this.GetEntityUISettingsEntity (entityType, context).TableSettings;
		}

		public void SetTableSettings(System.Type entityType, UserEntityTableSettings settings)
		{
			var context = this.userManager.BusinessContext;

			var entitySettings = this.GetEntityUISettingsEntity (entityType, context);

			entitySettings.TableSettings = settings;
			entitySettings.PersistSettings (context);

			context.SaveChanges (LockingPolicy.ReleaseLock);
		}


		public virtual IFilter GetScopeFilter(System.Type entityType, AbstractEntity example)
		{
			return null;
		}

		public virtual IEnumerable<UserScope> GetAvailableUserScopes()
		{
			return Enumerable.Empty<UserScope> ();
		}

		public virtual UserScope GetActiveUserScope()
		{
			return null;
		}

		public virtual void SetActiveUserScope(string scopeId)
		{
		}
		

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}

		private EntityUISettingsEntity GetEntityUISettingsEntity(System.Type entityType, BusinessContext context = null)
		{
			var entityId = EntityInfo.GetTypeId (entityType);

			if (entityId.IsEmpty)
			{
				return null;
			}

			var softwareSettings = this.SoftwareUISettings;

			if (softwareSettings == null)
			{
				return null;
			}

			var entityIdString = entityId.ToCompactString ();
			var entitySettings = softwareSettings.EntityUISettings.FirstOrDefault (x => x.EntityId == entityIdString);

			if ((entitySettings == null) &&
				(context != null))
			{
				entitySettings = context.CreateEntity<EntityUISettingsEntity> ();
				entitySettings.EntityId = entityIdString;

				softwareSettings.EntityUISettings.Add (entitySettings);
			}

			return entitySettings;
		}


		private readonly UserManager userManager;
		private readonly string sessionId;
	}
}
