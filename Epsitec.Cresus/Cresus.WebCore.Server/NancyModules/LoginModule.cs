//	Copyright © 2011-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.WebCore.Server.Core;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;
using Nancy.Extensions;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This module handles the http requests relating the log in and the log out of a user. This
	/// is the only module which doesn't require the user to be already authenticated. It also
	/// provides a function that is used by other modules to check if the user is logged in or to
	/// access to some session data.
	/// </summary>
	public class LoginModule : AbstractCoreModule
	{


		public LoginModule(CoreServer coreServer)
			: base (coreServer, "/log")
		{
			// Logs the user in. This request will store some session data in the cookie. This is
			// safe to do, as the content of the cookie is automatically encrypted by Nancy.
			// POST arguments:
			// - username:    the name of the user.
			// - password:    the password of the user.
			this.Post["/in1"]  = p => this.LoginUserPassword ();
            this.Post["/in2"] = p => this.LoginUserPin ();

			// Logs the user out. This request will remove the session data from the cookie.
			this.Post["/out"] = p => this.Logout ();
		}


		public Response LoginUserPassword()
		{
			string username = this.Request.Form.username;
			string password = this.Request.Form.password;

			var authResult = this.CheckCredentials (username, password);

			if (authResult.ValidUserPassword)
			{
                if (authResult.RequirePinValidation)
                {
                    this.SessionLogin1 (username);
                    //  Login needs a PIN to validate this session...
                    return CoreResponse.FormSuccess ("pin");
                }
                else
                {
                    this.SessionLogin1 (username);
                    this.SessionLogin2 ();

                    if (this.CoreServer.AuthenticationManager.NotifySuccessfulLogin (username))
                    {
                        this.CoreServer.AuthenticationManager.NotifyChangePasswordIfNeeded (username);
                        return CoreResponse.FormSuccess ();
                    }
                }
			}
			
			this.SessionLogout ();

			var errors = new Dictionary<string, object> ()
			{
				{ "username" , Res.Strings.IncorrectUsername.ToSimpleText () },
				{ "password" , Res.Strings.IncorrectPassword.ToSimpleText () },
			};

			return CoreResponse.FormFailure (errors);
		}

        public Response LoginUserPin()
        {
            string pin = this.Request.Form.pin;

            bool expectingPin = (bool) this.Session[LoginModule.LoginPin2FA];
            string userName = (string) this.Session[LoginModule.UserName];

            if (expectingPin && this.CheckPin (userName, pin))
            {
                this.SessionLogin2 ();

                if (this.CoreServer.AuthenticationManager.NotifySuccessfulLogin (userName))
                {
                    this.CoreServer.AuthenticationManager.NotifyChangePasswordIfNeeded (userName);
                    return CoreResponse.FormSuccess ();
                }
            }

            var errors = new Dictionary<string, object> ()
            {
                { "pin" , "Le PIN fourni n'est pas correct" }
            };

            return CoreResponse.FormFailure (errors);
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


        private void SessionLogin1(string userName)
        {
            this.Session[LoginModule.LoggedInName] = false;
            this.Session[LoginModule.LoginPin2FA] = true;

            this.Session[LoginModule.UserName] = userName;
        }

        private void SessionLogin2()
        {
            this.Session[LoginModule.LoggedInName] = true;
            this.Session[LoginModule.SessionId] = LoginModule.CreateSessionId ();
            this.Session[LoginModule.LoginPin2FA] = false;
        }


        private void SessionLogout()
		{
			this.Session.Delete (LoginModule.UserName);
			this.Session.Delete (LoginModule.SessionId);
            this.Session.Delete (LoginModule.LoginPin2FA);
            this.Session[LoginModule.LoggedInName] = false;
		}


		private AuthenticationResult CheckCredentials(string userName, string password)
		{
            if (password == null)
            {
                throw new System.ArgumentNullException (nameof (password));
            }
            bool requirePinValidation = true || CoreContext.HasExperimentalFeature ("RequirePinValidation");

            if (CoreContext.HasExperimentalFeature ("DisablePasswordCheck"))
			{
                password = null;
			}

			return this.CoreServer.AuthenticationManager.CheckCredentials (userName, password, requirePinValidation);
		}

        private bool CheckPin(string userName, string pin)
        {
            return this.CoreServer.AuthenticationManager.CheckPin (userName, pin);
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
        public static readonly string LoginPin2FA = "LoginPin2FA";


	}


}
