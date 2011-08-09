using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public abstract class CoreModule : NancyModule
	{
		protected CoreModule()
		{
		}

		protected CoreModule(string modulePath) : base (modulePath)
		{
		}

		internal CoreSession GetCoreSession()
		{
			var sessionId = DebugSession.Session["CoreSession"] as string;
			var session = CoreServer.Instance.GetCoreSession (sessionId);

			if (session == null)
			{
				var server = CoreServer.Instance;
				session = server.CreateSession ();
				PanelBuilder.CoreSession = session;

				DebugSession.Session["CoreSession"] = session.Id;
			}

			return session;
		}
	}
}
