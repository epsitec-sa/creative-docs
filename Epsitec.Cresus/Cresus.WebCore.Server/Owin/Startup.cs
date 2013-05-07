using Microsoft.AspNet.SignalR;
using Owin;

namespace Epsitec.Cresus.WebCore.Server.Owin
{
	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			/*app.MapConnection<RawConnection> ("/raw", new ConnectionConfiguration
			{
				EnableCrossDomain = true
			});*/

			var config = new HubConfiguration
			{
				EnableCrossDomain = true
			};

			app.MapHubs (config);
		}
	}
}
