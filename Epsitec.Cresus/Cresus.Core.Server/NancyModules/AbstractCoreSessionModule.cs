using Epsitec.Cresus.Core.Server.CoreServer;

using System;


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


		protected T ExecuteWithCoreSession<T>(Func<CoreSession, T> function)
		{
			var coreSession = this.GetCoreSession ();

			return coreSession.LockAndExecute (() => function (coreSession));
		}


		private CoreSession GetCoreSession()
		{
			var sessionId = (string) this.Session[LoginModule.CoreSessionName];

			var session = this.ServerContext.CoreSessionManager.GetSession (sessionId);

			if (session == null)
			{
				throw new Exception ("CoreSession not found");
			}

			return session;
		}


	}


}
