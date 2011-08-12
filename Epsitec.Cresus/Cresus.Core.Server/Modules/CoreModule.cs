using Nancy;
using Nancy.Security;
using Nancy.Extensions;

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

		private void CheckIsLoggedIn()
		{
			this.Before.AddItemToEndOfPipeline (RequiresAuthentication);
		}

		private static Response RequiresAuthentication(NancyContext context)
		{
			var s = context.Request.Session;

			var l = s["loggedin"];

			if (l != null && l.GetType () == typeof(bool) && (bool) l)
			{
				return null;
			}

			return context.GetRedirect ("~/log");
		}
	}
}
