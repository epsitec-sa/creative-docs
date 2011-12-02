using Epsitec.Cresus.Core.Business.UserManagement;

using System;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	
	
	/// <summary>
	/// This class is used by the <see cref="LoginModule"/> to check the user's credentials.
	/// It should use the users with the Core app.
	/// </summary>
	public sealed class AuthenticationManager : IDisposable
	{


		public AuthenticationManager()
		{
			var coreSession =  new SafeCoreSession ("authentication manager session");
			
			this.safeCoreSession = coreSession;
			this.userManager = this.safeCoreSession.Execute (cs => cs.CoreData.GetComponent<UserManager> ());
		}


		public bool CheckCredentials(string userName, string password)
		{
			Func<CoreSession, bool> function = _ =>
			{
				return this.userManager.CheckUserAuthentication (userName, password);
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
