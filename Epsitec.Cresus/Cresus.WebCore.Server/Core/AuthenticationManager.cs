//	Copyright © 2011-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// The <c>AuthenticationManager</c> class is used by the <see cref="LoginModule"/> to
	/// check the user's credentials.
	/// </summary>
	public sealed class AuthenticationManager
	{
		public AuthenticationManager(CoreWorkerPool coreWorkerPool)
		{
			this.coreWorkerPool = coreWorkerPool;
		}


		public AuthenticationResult CheckCredentials(string userName, string password, bool requirePin)
		{
			System.Func<UserManager, AuthenticationResult> function = userManager =>
			{
				var validUserPassword = password == null || userManager.CheckUserAuthentication (userName, password);
                var requirePinValidation  = requirePin && validUserPassword && userManager.Start2FALogin (userName);
                
                return new AuthenticationResult (validUserPassword, requirePinValidation);
            };

			return this.coreWorkerPool.Execute (function);
		}

        public bool CheckPin(string userName, string pin)
        {
            System.Func<UserManager, bool> function = userManager =>
            {
                return userManager.CheckUserPin (userName, pin);
            };

            return this.coreWorkerPool.Execute (function);
        }

		public bool NotifySuccessfulLogin(string userName)
		{
			System.Func<UserManager, bool> function = userManager =>
			{
				var user = userManager.FindUser (userName);

				if (user == null)
				{
					return false;
				}

				userManager.NotifySusccessfulLogin (user);
				
				return true;
			};

			return this.coreWorkerPool.Execute (function);
		}

		public bool NotifyChangePasswordIfNeeded(string userName)
		{
			System.Func<UserManager, bool> function = userManager =>
			{
				var user = userManager.FindUser (userName);
				if (user.CheckPassword ("monsupermotdepasse"))
				{
					userManager.NotifyChangePassword (user);
				}

				return true;
			};

			return this.coreWorkerPool.Execute (function);
		}


		private readonly CoreWorkerPool			coreWorkerPool;
	}
}
