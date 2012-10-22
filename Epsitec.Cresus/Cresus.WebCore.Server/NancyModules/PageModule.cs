using Epsitec.Cresus.WebCore.Server.Core;

using Nancy.Responses.Negotiation;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Proxy to retrieve a page stored in the Views folder
	/// </summary>
	public class PageModule : AbstractAuthenticatedModule
	{


		public PageModule(CoreServer coreServer)
			: base (coreServer, "/page/")
		{
			Get["/{name}"] = p => this.GetPageView (p);
		}


		private Negotiator GetPageView(dynamic parameters)
		{
			string pageName = parameters.name;

			return View[pageName];
		}


	}


}
