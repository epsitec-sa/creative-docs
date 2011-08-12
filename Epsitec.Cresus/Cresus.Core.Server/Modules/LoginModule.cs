using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class LoginModule : NancyModule
	{
		public LoginModule()
			: base ("/log")
		{
			Get["/"] = parameters =>
			{
				return Response.AsRedirect ("/log/in");
			};

			Get["/in"] = parameters =>
			{
				return "<form method=post><input type=submit></form>";
			};

			Post["/in"] = parameters =>
			{
				Session["loggedin"] = true;
				var session = CoreServer.Instance.CreateSession ();

				Session["CoreSession"] = session.Id;

				return Response.AsSuccessExtJsForm ();
			};

			Get["/out"] = parameters =>
			{
				Session["loggedin"] = false;
				return "logout";
			};
		}
	}
}
