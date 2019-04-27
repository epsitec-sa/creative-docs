using Epsitec.Cresus.WebCore.Server.Core;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using Nancy.Extensions;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This module handles the http requests relating the log in and the log out of a user. This
	/// is the only module which doesn't require the user to be already authenticated. It also
	/// provides a function that is used by other modules to check if the user is logged in or to
	/// access to some session data.
	/// </summary>
	public class LoginModule : AbstractCoreModule
	{


		public LoginModule(CoreServer coreServer)
			: base (coreServer, "/log")
		{
			// Logs the user in. This request will store some session data in the cookie. This is
			// safe to do, as the content of the cookie is automatically encrypted by Nancy.
			// POST arguments:
			// - username:    the name of the user.
			// - password:    the password of the user.
			this.Post["/in"]  = p =>
				this.Login ();

			// Logs the user out. This request will remove the session data from the cookie.
			this.Post["/out"] = p =>
				this.Logout ();
		}


		public Response Login()
		{
			string username = this.Request.Form.username;
			string password = this.Request.Form.password;

			bool loggedIn = this.CheckCredentials (username, password);

			if (loggedIn)
			{
				this.SessionLogin (username);

				if (this.CoreServer.AuthenticationManager.NotifySuccessfulLogin (username))
				{
					this.CoreServer.AuthenticationManager.NotifyChangePasswordIfNeeded (username);
					return CoreResponse.FormSuccess ();
				}
			}
			
			this.SessionLogout ();

			var errors = new Dictionary<string, object> ()
			{
				{ "username" , Res.Strings.IncorrectUsername.ToSimpleText () },
				{ "password" , Res.Strings.IncorrectPassword.ToSimpleText () },
			};

			return CoreResponse.FormFailure (errors);
		}


		public Response Logout()
		{
			this.SessionLogout ();

			var content = new Dictionary<string, object> ()
			{ 
				{ "logout", true }
			};

			return CoreResponse.Success (content);
		}


		private void SessionLogin(string userName)
		{
			this.Session[LoginModule.LoggedInName] = true;
			this.Session[LoginModule.UserName]     = userName;
			this.Session[LoginModule.SessionId]    = LoginModule.CreateSessionId ();
		}


		private void SessionLogout()
		{
			this.Session.Delete (LoginModule.UserName);
			this.Session.Delete (LoginModule.SessionId);
			this.Session[LoginModule.LoggedInName] = false;
		}


		private bool CheckCredentials(string userName, string password)
		{
			if (Epsitec.Cresus.Core.Library.CoreContext.HasExperimentalFeature ("DisablePasswordCheck"))
			{
				return true;
			}

			return this.CoreServer.AuthenticationManager.CheckCredentials (userName, password);
		}


		public static void CheckIsLoggedIn(NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (nc => LoginModule.RequiresAuthentication (nc));
		}


		private static string CreateSessionId()
		{
			return System.Guid.NewGuid ().ToString ("D");
		}


		private static Response RequiresAuthentication(NancyContext context)
		{
			var session = context.Request.Session;

			var loggedIn = session[LoginModule.LoggedInName];

			if (loggedIn != null && loggedIn.GetType () == typeof (bool) && (bool) loggedIn)
			{
				// He is logged in, we don't want to do anything
				return null;
			}
			else
			{
				// Not logged in, break to usual path and redirect the user to the main page where he
				// can log in.
				return context.GetRedirect ("~/");
			}
		}


		public static string GetUserName(NancyModule module)
		{
			return (string) module.Session[LoginModule.UserName];
		}


		public static string GetSessionId(NancyModule module)
		{
			return (string) module.Session[LoginModule.SessionId];
		}


		public static readonly string LoggedInName = "LoggedIn";


		public static readonly string UserName = "UserName";


		public static readonly string SessionId = "UserSessionId";



	}


}
