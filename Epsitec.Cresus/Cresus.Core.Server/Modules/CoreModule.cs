using Epsitec.Cresus.Core.Server.Auth;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public abstract class CoreModule : NancyModule
	{
		protected CoreModule()
		{
			this.CheckIsLoggedIn ();
		}

		protected CoreModule(string modulePath) : base (modulePath)
		{
			this.CheckIsLoggedIn ();
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
