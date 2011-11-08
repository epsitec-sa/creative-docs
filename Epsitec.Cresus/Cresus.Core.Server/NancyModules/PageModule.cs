//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

namespace Epsitec.Cresus.Core.Server.NancyModules
{
	/// <summary>
	/// Proxy to retrieve a page stored in the Views folder
	/// </summary>
	public class PageModule : AbstractLoggedCoreModule
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
