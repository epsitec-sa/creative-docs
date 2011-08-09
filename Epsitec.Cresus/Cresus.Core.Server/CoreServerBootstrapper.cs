using Nancy;
using Nancy.Session;

namespace Epsitec.Cresus.Core.Server
{
	public class CoreServerBootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			var cookieBasedSessions = CookieBasedSessions.Enable (this) as CookieBasedSessions;
			cookieBasedSessions.CookieConfigurator = cookie =>
			{
				cookie.Path = "/";
			};
		}
	}
}
