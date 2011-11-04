//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Server.AdditionalResponses;

using Nancy;

using Nancy.Extensions;

using System;

using System.Collections.Generic;



namespace Epsitec.Cresus.Core.Server.Authentification
{
	
	
	/// <summary>
	/// This class is used by the <see cref="LoginModule"/> to check the user's credentials.
	/// It should use the users with the Core app.
	/// </summary>
	internal sealed class AuthentificationManager : IDisposable
	{


		public AuthentificationManager()
		{
			this.coreSession = new CoreSession ("authentifier session");
		}


		public Response Login(NancyModule module)
		{
			if (this.CheckCredentials (module.Request.Form))
			{
				var session = CoreServer.Instance.CreateSession ();

				module.Session[AuthentificationManager.LoggedInName] = true;
				module.Session["CoreSession"] = session.Id;

				return module.Response.AsCoreSuccess ();
			}
			else
			{
				var dic = new Dictionary<string, object> ();
				dic["username"] = "Incorrect username";

				return module.Response.AsCoreError (dic);
			}
		}


		private bool CheckCredentials(dynamic form)
		{
			string username = form.username;
			string password = form.password;

			var userManager = this.coreSession.CoreData.GetComponent<UserManager> ();

			return userManager.CheckUserAuthentication (username, password);
		}


		public void Dispose()
		{
			this.coreSession.Dispose ();
		}


		public static Response Logout(NancyModule module)
		{
			CoreServer.Instance.DeleteSession (module.Session["CoreSession"] as string);

			module.Session[AuthentificationManager.LoggedInName] = false;

			return "logout";
		}


		/// <summary>
		/// Tell Nancy to check if the user is logged in
		/// </summary>
		public static void CheckIsLoggedIn(NancyModule module)
		{
			module.Before.AddItemToEndOfPipeline (nc => AuthentificationManager.RequiresAuthentication (nc));
		}


		/// <summary>
		/// This method is called before each request to a <see cref="CoreModule"/>
		/// to check if the user is logged in. 
		/// It is called by Nancy.
		/// </summary>
		private static Response RequiresAuthentication(NancyContext context)
		{
			var session = context.Request.Session;

			var loggedIn = session[AuthentificationManager.LoggedInName];

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


		private readonly CoreSession coreSession;

		
		private static readonly string LoggedInName = "LOGGED_IN";


		public static AuthentificationManager Instance
		{
			get
			{
				if (AuthentificationManager.instance == null)
				{
					AuthentificationManager.instance = new AuthentificationManager ();
				}
				return AuthentificationManager.instance;
			}
		}


		private static AuthentificationManager instance;


	}


}
