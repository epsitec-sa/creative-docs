using Nancy.Authentication.Forms;
using Nancy.Extensions;
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

				//GetCoreSession ();

				//var userGuid = UserDatabase.ValidateUser ((string) this.Request.Form.Username, (string) this.Request.Form.Password);
				var userGuid = UserDatabase.ValidateUser ("admin", "password");

				if (userGuid == null)
				{
					return Context.GetRedirect ("~/log");
				}

				System.DateTime? expiry = null;
				if (this.Request.Form.RememberMe.HasValue)
				{
					expiry = System.DateTime.Now.AddDays (7);
				}

				return this.LoginAndRedirect (userGuid.Value, expiry);
			};

			Get["/out"] = parameters =>
			{
				return this.LogoutAndRedirect ("~/");
			};
		}
	}
}
