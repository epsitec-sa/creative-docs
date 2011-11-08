//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Business.UserManagement;

using System;



namespace Epsitec.Cresus.Core.Server.CoreServer
{
	
	
	/// <summary>
	/// This class is used by the <see cref="LoginModule"/> to check the user's credentials.
	/// It should use the users with the Core app.
	/// </summary>
	internal sealed class AuthentificationManager : AbstractServerObject, IDisposable
	{


		public AuthentificationManager(ServerContext serverContext)
			: base (serverContext)
		{
			var coreSession = new CoreSession ("authentification manager session");

			this.coreSession = coreSession;
			this.userManager = coreSession.CoreData.GetComponent<UserManager> ();

			this.checkLock = new object ();
		}


		public bool CheckCredentials(string username, string password)
		{
			lock (this.checkLock)
			{
				return this.userManager.CheckUserAuthentication (username, password);
			}
		}


		public void Dispose()
		{
			this.coreSession.Dispose ();
		}


		private readonly CoreSession coreSession;


		private readonly UserManager userManager;


		private readonly object checkLock;


	}


}
