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
