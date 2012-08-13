using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Proxy to retrieve a page stored in the Views folder
	/// </summary>
	public class PageModule : AbstractBusinessContextModule
	{


		public PageModule(CoreServer coreServer)
			: base (coreServer, "/page/")
		{
			Get["/{name}"] = p => this.GetPageView (p);
		}


		private Response GetPageView(dynamic parameters)
		{
			string pageName = parameters.name;

			return View[pageName];
		}


	}


}
