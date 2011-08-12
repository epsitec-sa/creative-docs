using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Nancy.Extensions;

namespace Epsitec.Cresus.Core.Server.Auth
{
	static class Authentication
	{
		public static Response Login(this NancyModule module)
		{
			if (Authentication.CheckCredentials (module.Request.Form))
			{
				var session = CoreServer.Instance.CreateSession ();

				module.Session["loggedin"] = true;
				module.Session["CoreSession"] = session.Id;

				return module.Response.AsSuccessExtJsForm ();
			}
			else
			{
				var dic = new System.Collections.Generic.Dictionary<string, object> ();
				dic["username"] = "Incorrect username";

				return module.Response.AsErrorExtJsForm (dic);
			}
		}

        public static Response Logout(this NancyModule module)
		{
			module.Session["loggedin"] = false;
			return "logout";
		}

		public static void CheckIsLoggedIn(this NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (RequiresAuthentication);
		}

		private static Response RequiresAuthentication(NancyContext context)
		{
			var s = context.Request.Session;

			var l = s["loggedin"];

			if (l != null && l.GetType () == typeof (bool) && (bool) l)
			{
				return null;
			}

			return context.GetRedirect ("~/log");
		}

		private static bool CheckCredentials(dynamic form)
		{
			return form.username == "jonas";
		}
	}
}
