﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Server.Authentification;

using Nancy;


namespace Epsitec.Cresus.Core.Server.Modules
{


	/// <summary>
	/// Base module thats allows to easly get the CoreSession for a defined user,
	/// and that requires the user to be logged in.
	/// </summary>
	public abstract class CoreModule : NancyModule
	{
		
		
		protected CoreModule()
		{
			AuthentificationManager.CheckIsLoggedIn (this);
		}


		protected CoreModule(string modulePath) : base (modulePath)
		{
			AuthentificationManager.CheckIsLoggedIn (this);
		}


		internal CoreSession GetCoreSession()
		{
			var sessionId = Session["CoreSession"] as string;
			var session = CoreServer.Instance.GetCoreSession (sessionId);

			if (session == null)
			{
				throw new System.Exception ("CoreSession not found");
			}

			return session;
		}


	}


}
