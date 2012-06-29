using Epsitec.Cresus.Core.Business.UserManagement;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	
	
	/// <summary>
	/// This class is used by the <see cref="LoginModule"/> to check the user's credentials.
	/// </summary>
	public sealed class AuthenticationManager
	{


		public AuthenticationManager(CoreWorkerPool coreWorkerPool)
		{
			this.coreWorkerPool = coreWorkerPool;
		}


		public bool CheckCredentials(string userName, string password)
		{
			Func<UserManager, bool> function = userManager =>
			{
				return userManager.CheckUserAuthentication (userName, password);
			};

			return this.coreWorkerPool.Execute (function);
		}


		private readonly CoreWorkerPool coreWorkerPool;


	}


}
