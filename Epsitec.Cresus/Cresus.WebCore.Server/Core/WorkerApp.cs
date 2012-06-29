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


		public T Execute<T>(Func<UserManager, T> action)
		{
			return action (this.userManager);
		}


		public T Execute<T>(Func<BusinessContext, T> action)
		{
			using (var businessContext = new BusinessContext (this.coreData))
			{
				return action (businessContext);
			}
		}


		private readonly CoreData coreData;


		private readonly UserManager userManager;


	}


}
