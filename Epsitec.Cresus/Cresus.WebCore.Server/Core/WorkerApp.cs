//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX


using Epsitec.Cresus.Core;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public class WorkerApp : CoreApp
	{


		public WorkerApp()
		{
			this.coreData = this.GetComponent<CoreData> ();
			this.userManager = this.coreData.GetComponent<UserManager> ();

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

		
		public T Execute<T>(Func<UserManager, T> action)
		{
			return action (this.userManager);
		}


		public T Execute<T>(string username, string sessionId, Func<BusinessContext, T> action)
		{
			return this.Execute (username, sessionId, w =>
			{
				using (var businessContext = new BusinessContext (w.CoreData))
				{
					return action (businessContext);
				}
			});
		}


		public T Execute<T>(string username, string sessionId, Func<WorkerApp, T> action)
		{
			System.Diagnostics.Debug.Assert (CoreApp.current == null);

			try
			{
				var user = this.userManager.FindUser (username);

				this.userManager.SetAuthenticatedUser (user.Code);
				this.userManager.SetActiveSessionId (sessionId);
				CoreApp.current = this;

				return action (this);
			}
			finally
			{
				this.userManager.SetAuthenticatedUser (null);
				this.userManager.SetActiveSessionId (null);
				CoreApp.current = null;
			}
		}


		private readonly CoreData coreData;
		private readonly UserManager userManager;


	}


}
