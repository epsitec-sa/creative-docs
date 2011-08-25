//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Epsitec.Cresus.Core.Server.Auth;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	/// <summary>
	/// Called from the login page to check if the user can access the application
	/// </summary>
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
