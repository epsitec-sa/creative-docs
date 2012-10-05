using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;

using System;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public abstract class AbstractAuthenticatedModule : AbstractCoreModule
	{


		protected AbstractAuthenticatedModule(CoreServer coreServer)
			: base (coreServer)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected AbstractAuthenticatedModule(CoreServer coreServer, string modulePath)
			: base (coreServer, modulePath)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected Response Execute(Func<BusinessContext, Response> function)
		{
			var userName  = this.GetUserName ();
			var sessionId = this.GetSessionId ();

			return this.CoreServer.CoreWorkerPool.Execute (userName, sessionId, function);
		}


		protected Response Execute(Func<UserManager, Response> function)
		{
			var userName  = this.GetUserName ();
			var sessionId = this.GetSessionId ();

			return this.CoreServer.CoreWorkerPool.Execute (userName, sessionId, function);
		}


		private string GetUserName()
		{
			return LoginModule.GetUserName (this);
		}


		private string GetSessionId()
		{
			return LoginModule.GetSessionId (this);
		}


	}


}
