using Epsitec.Cresus.Core.Server.CoreServer;
using Epsitec.Cresus.Core.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	public abstract class AbstractCoreSessionModule : AbstractCoreModule
	{


		protected AbstractCoreSessionModule(ServerContext serverContext)
			: base (serverContext)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected AbstractCoreSessionModule(ServerContext serverContext, string modulePath)
			: base (serverContext, modulePath)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected Response ExecuteWithCoreSession(Func<CoreSession, Response> function)
		{
			var safeCoreSession = this.GetSafeCoreSession ();

			if (safeCoreSession == null)
			{
				return CoreResponse.Error (CoreResponse.ErrorCode.SessionTimeout);
			}
			else
			{
				return safeCoreSession.Execute ((c) => function (c));
			}
		}


		private SafeCoreSession GetSafeCoreSession()
		{
			var sessionId = (string) this.Session[LoginModule.CoreSessionName];

			return this.ServerContext.CoreSessionManager.GetSession (sessionId);
		}


	}


}
