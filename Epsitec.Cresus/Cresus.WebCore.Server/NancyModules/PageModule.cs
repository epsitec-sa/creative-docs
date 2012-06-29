using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

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
			Get["/{name}"] = p => this.Execute (b => this.GetPageView (b));
		}


		private Response GetPageView(dynamic parameters)
		{
			string pageName = parameters.name;

			Dumper.Instance.Dump ("Data for page: " + pageName);

			return View[pageName];
		}


	}


}
