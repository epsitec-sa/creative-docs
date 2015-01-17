//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Epsitec.Cresus.Core.Metadata;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// This class is the basic component that is used by WebCore to access to the resources
	/// provided by the application, such as BusinessContexts, UserManager, etc.
	/// </summary>
	/// <remarks>
	/// Because of intensive use of ThreadLocal storage within a CoreApp, it is mandatory to
	/// always use it within the same thread. So for a given instance of CoreApp, it must have a
	/// dedicated thread that calls its constructor, uses it and disposes it. This thread should
	/// never be used to access another instance of CoreApp.
	/// </remarks>
	public class WorkerApp : CoreApp
	{
		public WorkerApp(CoreWorker coreWorker)
		{
			this.coreWorker = coreWorker;
			this.coreData = this.GetComponent<CoreData> ();
			this.userManager = this.coreData.GetComponent<UserManager> ();
			this.dataStoreMetadata = CoreContext.GetMetadata<DataStoreMetadata> ();
			this.dataSetGetter = this.coreData.GetComponent<DataSetGetter> ();

			Services.SetApplication (this);

			CoreApp.current = null;
		}


		public override string					ApplicationIdentifier
		{
			get
			{
				return "Worker app";
			}
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return "Worker app";
			}
		}

		public CoreWorker						CoreWorker
		{
			get
			{
				return this.coreWorker;
			}
		}

		public CoreData							CoreData
		{
			get
			{
				return this.coreData;
			}
		}

		public UserManager						UserManager
		{
			get
			{
				return this.userManager;
			}
		}

		public DataStoreMetadata				DataStoreMetaData
		{
			get
			{
				return this.dataStoreMetadata;
			}
		}

		public DataSetGetter					DataSetGetter
		{
			get
			{
				return this.dataSetGetter;
			}
		}

		
		public static WorkerApp					Current
		{
			get
			{
				return CoreApp.current as WorkerApp;
			}
		}


		public T Execute<T>(System.Func<UserManager, T> action)
		{
			T result = default (T);
			this.ConfigureCurrentCoreAppAndExecute (() => { result = action (this.userManager); });
			return result;
		}


		public T Execute<T>(string username, string sessionId, System.Func<BusinessContext, T> action)
		{
			T result = default (T);
			this.Execute (username, sessionId, context => { result = action (context); });
			return result;
		}

		public T Execute<T>(string username, string sessionId, System.Func<WorkerApp, T> function)
		{
			T result = default (T);
			this.Execute (username, sessionId, app => { result = function (app); });
			return result;
		}

		public void Execute(string username, string sessionId, System.Action<BusinessContext> action)
		{
			this.Execute (username, sessionId, app => app.ExecuteWithBusinessContext (action));
		}

		public void Execute(string username, string sessionId, System.Action<WorkerApp> action)
		{
			this.ConfigureCurrentCoreAppAndExecute (
				() =>
				{
					try
					{
						var user = this.userManager.FindUser (username);

						this.userManager.SetAuthenticatedUser (user);
						this.userManager.SetActiveSessionId (sessionId);

						action (this);
					}
					finally
					{
						this.userManager.SetAuthenticatedUser ((SoftwareUserEntity) null);
						this.userManager.SetActiveSessionId (null);
					}
				});
		}


		private void ExecuteWithBusinessContext(System.Action<BusinessContext> action)
		{
			using (var businessContext = new BusinessContext (this.CoreData, false))
			{
				try
				{
					action (businessContext);
				}
				finally
				{
					if (businessContext != null)
					{
						// We discard the BusinessContext so any unsaved changes won't be
						// persisted to the database. Such changes could happen if an exception
						// is thrown after some entities have been modified. In such a case, we
						// want to make sure that the changed are not persisted to the database.

						businessContext.Discard ();
					}
				}
			}
		}

		private void ConfigureCurrentCoreAppAndExecute(System.Action action)
		{
			System.Diagnostics.Debug.Assert (WorkerApp.Current == null);

			try
			{
				CoreApp.current = this;

				action ();
			}
			finally
			{
				System.Diagnostics.Debug.Assert (WorkerApp.Current == this);

				CoreApp.current = null;

				// We flush the user manager so that it does not hold any reference to an entity
				// anymore. This way, we are sure that the next time it is used, there is no
				// outdated cached data within it.

				this.userManager.Flush ();
			}
		}


		private readonly CoreWorker				coreWorker;
		private readonly CoreData				coreData;
		private readonly UserManager			userManager;
		private readonly DataStoreMetadata		dataStoreMetadata;
		private readonly DataSetGetter			dataSetGetter;
	}
}
