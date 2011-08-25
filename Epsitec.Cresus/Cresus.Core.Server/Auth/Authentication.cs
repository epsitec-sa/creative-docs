//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Nancy.Extensions;

namespace Epsitec.Cresus.Core.Server.Auth
{
	/// <summary>
	/// This class is used by the <see cref="LoginModule"/> to check the user's credentials.
	/// It should use the users with the Core app.
	/// </summary>
	static class Authentication
	{
		public static Response Login(this NancyModule module)
		{
			if (Authentication.CheckCredentials (module.Request.Form))
			{
				var session = CoreServer.Instance.CreateSession ();

				module.Session[Authentication.LoggedInName] = true;
				module.Session["CoreSession"] = session.Id;

				return module.Response.AsCoreSuccess ();
			}
			else
			{
				var dic = new System.Collections.Generic.Dictionary<string, object> ();
				dic["username"] = "Incorrect username";

				return module.Response.AsCoreError (dic);
			}
		}

        public static Response Logout(this NancyModule module)
		{
			CoreServer.Instance.DeleteSession (module.Session["CoreSession"] as string);
			module.Session[Authentication.LoggedInName] = false;
			return "logout";
		}

		/// <summary>
		/// Tell Nancy to check if the user is logged in
		/// </summary>
		/// <param name="module"></param>
		public static void CheckIsLoggedIn(this NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (RequiresAuthentication);
		}

		/// <summary>
		/// This method is called before each request to a <see cref="CoreModule"/>
		/// to check if the user is logged in. 
		/// It is called by Nancy
		/// </summary>
		/// <returns></returns>
		private static Response RequiresAuthentication(NancyContext context)
		{
			var session = context.Request.Session;

			var loggedIn = session[Authentication.LoggedInName];

			if (loggedIn != null && loggedIn.GetType () == typeof (bool) && (bool) loggedIn)
			{
				// He is logged in, we don't want to do anything
				return null;
			}

			// Not logged in, break to usual path and redirect the user to the main page where is can log in.
			return context.GetRedirect ("~/");
		}

		private static bool CheckCredentials(dynamic form)
		{
			return form.username == "jonas";
		}

		private static readonly string LoggedInName = "LOGGED_IN";
	}
}
