using Epsitec.Cresus.Core.Server.CoreServer;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	/// <summary>
	/// Proxy to retrieve a page stored in the Views folder
	/// </summary>
	public class PageModule : AbstractCoreSessionModule
	{


		public PageModule(ServerContext serverContext)
			: base (serverContext, "/page/")
		{
			Get["/{name}"] = parameters =>
			{
				string pageName = parameters.name;
				return View[pageName];
			};
		}


	}


}
