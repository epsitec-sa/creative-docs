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
			Post["/in"] = p => this.Login ();
			Post["/out"] = p => this.Logout ();
		}


		public Response Login()
		{
			string username = this.Request.Form.username;
			string password = this.Request.Form.password;

			bool loggedIn = this.CheckCredentials (username, password);

			this.Session[LoginModule.LoggedInName] = loggedIn;

			if (loggedIn)
			{
				this.Session[LoginModule.UserName] = username;
				this.Session[LoginModule.SessionId] = System.Guid.NewGuid ().ToString ("D");

				return CoreResponse.FormSuccess ();
			}
			else
			{
				var errors = new Dictionary<string, object> ()
				{
					{ "username" , "Incorrect username" },
					{ "password" , "Incorrect password" },
				};
				
				return CoreResponse.FormFailure (errors);
			}
		}


		public Response Logout()
		{
			this.Session.Delete (LoginModule.UserName);
			this.Session.Delete (LoginModule.SessionId);
			this.Session[LoginModule.LoggedInName] = false;

			var content = new Dictionary<string, object> ()
			{ 
				{ "logout", true }
			};

			return CoreResponse.Success (content);
		}


		private bool CheckCredentials(string username, string password)
		{
			return this.CoreServer.AuthenticationManager.CheckCredentials (username, password);
		}


		public static void CheckIsLoggedIn(NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (nc => LoginModule.RequiresAuthentication (nc));
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
