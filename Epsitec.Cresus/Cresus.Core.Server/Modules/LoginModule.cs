using Epsitec.Cresus.Core.Server.Auth;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class LoginModule : NancyModule
	{
		public LoginModule()
			: base ("/log")
		{
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
