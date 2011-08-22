namespace Epsitec.Cresus.Core.Server.Modules
{
	public class PageModule : CoreModule
	{
		public PageModule()
			: base ("/page/")
		{
			Get["/{name}"] = parameters =>
			{
				string pageName = parameters.name;
				return View[pageName];
			};
		}
	}
}
