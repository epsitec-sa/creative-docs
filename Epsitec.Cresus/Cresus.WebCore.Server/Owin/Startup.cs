//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;
using Owin;

namespace Epsitec.Cresus.WebCore.Server.Owin
{
	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HubConfiguration
			{
				EnableCrossDomain = true
			};

			app.MapHubs (config);
		}
	}
}
