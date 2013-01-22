//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX


using Epsitec.Cresus.Core;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Cresus.Core.Metadata;

using System;

using System.Diagnostics;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public class WorkerApp : CoreApp
	{


		public WorkerApp()
		{
			this.coreData = this.GetComponent<CoreData> ();
			this.userManager = this.coreData.GetComponent<UserManager> ();
			this.dataStoreMetadata = CoreContext.GetMetadata<DataStoreMetadata> ();
			this.dataSetGetter = this.coreData.GetComponent<DataSetGetter> ();

			Services.SetApplication (this);

			CoreApp.current = null;
		}


		public override string ApplicationIdentifier
		{
			get
			{
				return "Worker app";
			}
		}


		public override string ShortWindowTitle
		{
			get
			{
				return "Worker app";
			}
		}


		public CoreData CoreData
		{
			get
			{
				return this.coreData;
			}
		}


		public UserManager UserManager
		{
			get
			{
				return this.userManager;
			}
		}


		public DataStoreMetadata DataStoreMetaData
		{
			get
			{
				return this.dataStoreMetadata;
			}
		}


		public DataSetGetter DataSetGetter
		{
			get
			{
				return this.dataSetGetter;
			}
		}

		
		public T Execute<T>(Func<UserManager, T> action)
		{
			return this.Execute (() => action (this.userManager));
		}


		public T Execute<T>(string username, string sessionId, Func<BusinessContext, T> action)
		{
			return this.Execute (username, sessionId, w => w.Execute (action));
		}


		public T Execute<T>(string username, string sessionId, Func<WorkerApp, T> action)
		{
			return this.Execute (() =>
			{
				try
				{
					var user = this.userManager.FindUser (username);

					this.userManager.SetAuthenticatedUser (user.Code);
					this.userManager.SetActiveSessionId (sessionId);

					return action (this);
				}
				finally
				{
					this.userManager.SetAuthenticatedUser (null);
					this.userManager.SetActiveSessionId (null);
				}
			});
		}


		public T Execute<T>(Func<BusinessContext, T> action)
		{
			using (var businessContext = new BusinessContext (this.CoreData, false))
			{
				try
				{
					return action (businessContext);
				}
				finally
				{
					if (businessContext != null)
					{
						// We discard the BusinessContext so any unsaved changes won't be
						// persisted to the database. Such changes could happen if an exception
						// is thrown after some entities have been modified. In such a case, we
						// want to make sure that the changed are not persisted to the databse.

						businessContext.Discard ();
					}
				}
			}
		}


		private T Execute<T>(Func<T> action)
		{
			Debug.Assert (CoreApp.current == null);

			try
			{
				CoreApp.current = this;

				return action ();
			}
			finally
			{
				CoreApp.current = null;

				this.userManager.Flush ();
			}
		}


		private readonly CoreData coreData;
		private readonly UserManager userManager;
		private readonly DataStoreMetadata dataStoreMetadata;
		private readonly DataSetGetter dataSetGetter;


	}


}
