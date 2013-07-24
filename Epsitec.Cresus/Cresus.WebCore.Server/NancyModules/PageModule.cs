using Epsitec.Cresus.WebCore.Server.Core;

using Nancy.Responses.Negotiation;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This module is used to get the data of static pages.
	/// </summary>
	public class PageModule : AbstractAuthenticatedModule
	{


		public PageModule(CoreServer coreServer)
			: base (coreServer, "/page/")
		{
			// Gets a static page.
			// URL argument:
			// - name:   the name of the static page.
			Get["/{name}"] = p =>
				this.GetPageView (p);
		}


		private Negotiator GetPageView(dynamic parameters)
		{
			string pageName = parameters.name;

			return View[pageName];
		}


	}


}
