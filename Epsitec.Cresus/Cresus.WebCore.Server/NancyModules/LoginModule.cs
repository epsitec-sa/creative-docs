using Epsitec.Cresus.WebCore.Server.Core;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using Nancy.Extensions;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	
	
	/// <summary>
	/// Called from the login page to check if the user can access the application
	/// </summary>
	public class LoginModule : AbstractCoreModule
	{


		public LoginModule(CoreServer coreServer)
			: base (coreServer, "/log")
		{
			this.Post["/in"]  = p => this.Login ();
			this.Post["/out"] = p => this.Logout ();
		}


		public Response Login()
		{
			string username = this.Request.Form.username;
			string password = this.Request.Form.password;
            string connectionId = this.Request.Form.connectionId;

			bool loggedIn = this.CheckCredentials (username, password);

			if (loggedIn)
			{
				this.SessionLogin (username, connectionId);

				return CoreResponse.FormSuccess ();
			}
			else
			{
				this.SessionLogout ();
				
				var errors = new Dictionary<string, object> ()
				{
					{ "username" , Res.Strings.IncorrectUsername.ToSimpleText () },
					{ "password" , Res.Strings.IncorrectPassword.ToSimpleText () },
				};
				
				return CoreResponse.FormFailure (errors);
			}
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


		private void SessionLogin(string userName,string connectionId)
		{
			this.CoreServer.AuthenticationManager.NotifySuccessfulLogin (userName,connectionId);
			
			this.Session[LoginModule.LoggedInName] = true;
			this.Session[LoginModule.UserName]     = userName;
            this.Session[LoginModule.ConnectionId] = connectionId;
			this.Session[LoginModule.SessionId]    = LoginModule.CreateSessionId ();
		}
		
		private void SessionLogout()
		{
			this.Session.Delete (LoginModule.UserName);
			this.Session.Delete (LoginModule.SessionId);
            this.Session.Delete(LoginModule.ConnectionId);
			this.Session[LoginModule.LoggedInName] = false;
		}
		
		private bool CheckCredentials(string userName, string password)
		{
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


        public static readonly string ConnectionId = "ConnectionId";


	}


}
