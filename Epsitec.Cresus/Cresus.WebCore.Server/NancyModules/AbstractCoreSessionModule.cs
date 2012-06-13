using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
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
				return CoreResponse.AsError (CoreResponse.ErrorCode.SessionTimeout);
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
