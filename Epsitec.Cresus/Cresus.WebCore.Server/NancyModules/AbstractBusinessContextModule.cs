using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;

using System;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public abstract class AbstractBusinessContextModule : AbstractCoreModule
	{


		protected AbstractBusinessContextModule(CoreServer coreServer)
			: base (coreServer)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected AbstractBusinessContextModule(CoreServer coreServer, string modulePath)
			: base (coreServer, modulePath)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected Response Execute(Func<BusinessContext, Response> function)
		{
			return this.CoreServer.CoreWorkerPool.Execute (function);
		}


	}


}
