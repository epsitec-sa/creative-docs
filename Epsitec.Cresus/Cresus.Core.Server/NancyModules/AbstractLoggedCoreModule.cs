using Epsitec.Cresus.Core.Server.CoreServer;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	public abstract class AbstractLoggedCoreModule : AbstractCoreModule
	{


		protected AbstractLoggedCoreModule()
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		protected AbstractLoggedCoreModule(string modulePath)
			: base (modulePath)
		{
			LoginModule.CheckIsLoggedIn (this);
		}


		internal CoreSession GetCoreSession()
		{
			var sessionId = Session["CoreSession"] as string;
			var session = this.ServerContext.CoreSessionManager.GetCoreSession (sessionId);

			if (session == null)
			{
				throw new System.Exception ("CoreSession not found");
			}

			return session;
		}
	}


}
