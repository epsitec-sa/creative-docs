//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Server.CoreServer;

using Epsitec.Cresus.Core.Server.NancyHosting;

using Nancy;

using Nancy.Extensions;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Server.NancyModules
{
	
	
	/// <summary>
	/// Called from the login page to check if the user can access the application
	/// </summary>
	public class LoginModule : AbstractCoreModule
	{


		public LoginModule(ServerContext serverContext)
			: base (serverContext, "/log")
		{
			Post["/in"] = parameters => this.Login ();
			Get["/out"] = parameters => this.Logout ();
		}


		public Response Login()
		{
			if (this.CheckCredentials (this.Request.Form))
			{
				var session = this.ServerContext.CoreSessionManager.CreateSession ();

				this.Session[LoginModule.LoggedInName] = true;
				this.Session[LoginModule.CoreSessionName] = session.Id;

				return this.Response.AsCoreSuccess ();
			}
			else
			{
				var dic = new Dictionary<string, object> ();
				dic["username"] = "Incorrect username";

				return this.Response.AsCoreError (dic);
			}
		}


		public Response Logout()
		{
			var sessionId = (string) this.Session[LoginModule.CoreSessionName];

			this.ServerContext.CoreSessionManager.DeleteSession (sessionId);

			this.Session[LoginModule.LoggedInName] = false;

			return "logout";
		}


		private bool CheckCredentials(dynamic form)
		{
			string username = form.username;
			string password = form.password;

			return this.ServerContext.AuthentificationManager.CheckCredentials (username, password);
		}


		/// <summary>
		/// Tell Nancy to check if the user is logged in
		/// </summary>
		public static void CheckIsLoggedIn(NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (nc => LoginModule.RequiresAuthentication (nc));
		}


		/// <summary>
		/// This method is called before each request to a <see cref="CoreModule"/>
		/// to check if the user is logged in. 
		/// It is called by Nancy.
		/// </summary>
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


		public static readonly string LoggedInName = "LOGGED_IN";


		public static readonly string CoreSessionName = "CoreSession";


	}


}
