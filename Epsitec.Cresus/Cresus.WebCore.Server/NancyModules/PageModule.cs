using Epsitec.Cresus.WebCore.Server.CoreServer;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Proxy to retrieve a page stored in the Views folder
	/// </summary>
	public class PageModule : AbstractCoreSessionModule
	{


		public PageModule(ServerContext serverContext)
			: base (serverContext, "/page/")
		{
			Get["/{name}"] = parameters => this.ExecuteWithCoreSession (coreSession =>
			{
				string pageName = parameters.name;
				return View[pageName];
			});
		}


	}


}
