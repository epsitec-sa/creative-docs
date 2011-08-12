using Nancy;
using Nancy.Session;

namespace Epsitec.Cresus.Core.Server
{
	public class CoreServerBootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			CookieBasedSessions.Enable (this);

			this.AfterRequest += ConfigureCookies;
		}

		private void ConfigureCookies(NancyContext context)
		{
			foreach (var c in context.Response.Cookies)
			{
				c.Path = "/";
			}
		}
	}

}
