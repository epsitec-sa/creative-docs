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

		public UserDataSetSettings GetDataSetSettings(DataSetMetadata dataSet)
		{
			var context = this.userManager.BusinessContext;

			return this.GetDataSetUISettingsEntity (dataSet, context).DataSetSettings;
		}

		public void SetDataSetSettings(DataSetMetadata dataSet, UserDataSetSettings settings)
		{
			var context = this.userManager.BusinessContext;

			var entitySettings = this.GetDataSetUISettingsEntity (dataSet, context);

			entitySettings.DataSetSettings = settings;
			entitySettings.PersistSettings (context);

			context.SaveChanges (LockingPolicy.ReleaseLock);
		}


		public virtual IFilter GetAdditionalFilter(DataSetMetadata dataSet, AbstractEntity example)
		{
			return null;
		}

		public virtual IFilter GetScopeFilter(DataSetMetadata dataSet, AbstractEntity example)
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

		private DataSetUISettingsEntity GetDataSetUISettingsEntity(DataSetMetadata dataSet, BusinessContext context = null)
		{
			var softwareSettings = this.SoftwareUISettings;

			if (softwareSettings == null)
			{
				return null;
			}

			var commandId = dataSet.Command.Caption.Id;
			var commandIdString = commandId.ToCompactString ();
			
			var dataSetSettings = softwareSettings
				.DataSetUISettings
				.FirstOrDefault (x => x.DataSetCommandId == commandIdString);

			if (dataSetSettings == null && context != null)
			{
				dataSetSettings = context.CreateEntity<DataSetUISettingsEntity> ();
				dataSetSettings.DataSetCommandId = commandIdString;
				dataSetSettings.DataSetSettings = new UserDataSetSettings (commandId);

				softwareSettings.DataSetUISettings.Add (dataSetSettings);
			}

			return dataSetSettings;
		}


		private readonly UserManager userManager;
		private readonly string sessionId;
	}
}
