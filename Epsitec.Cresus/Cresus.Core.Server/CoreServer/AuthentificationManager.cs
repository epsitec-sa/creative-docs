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
	public sealed class AuthentificationManager : IDisposable
	{


		public AuthentificationManager()
		{
			var coreSession =  new SafeCoreSession ("authentification manager session");
			
			this.safeCoreSession = coreSession;
			this.userManager = this.safeCoreSession.Execute (cs => cs.CoreData.GetComponent<UserManager> ());
		}


		public bool CheckCredentials(string username, string password)
		{
			Func<CoreSession, bool> function = _ =>
			{
				return this.userManager.CheckUserAuthentication (username, password);
			};

			return this.safeCoreSession.Execute (function);
		}


		public void Dispose()
		{
			this.safeCoreSession.Dispose ();
		}


		private readonly SafeCoreSession safeCoreSession;


		private readonly UserManager userManager;


	}


}
