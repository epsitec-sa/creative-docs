using Epsitec.Cresus.Core.Server.Auth;
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
				return this.Login ();
			};

			Get["/out"] = parameters =>
			{
				return this.Logout ();
			};
		}
	}
}
