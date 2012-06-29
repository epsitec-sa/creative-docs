using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.ErrorHandling;
using Nancy.Session;

using TinyIoC;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{
	
	
	/// <summary>
	/// Called by Nancy when the server is starting
	/// </summary>
	internal class CoreServerBootstrapper : DefaultNancyBootstrapper
	{


		public CoreServerBootstrapper(CoreServer coreServer)
		{
			this.coreServer = coreServer;
		}


		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer (container);

			container.Register<IErrorHandler> (new CoreErrorHandler ());
			container.Register<CoreServer> (this.coreServer);
		}


		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			base.ApplicationStartup (container, pipelines);

			CookieBasedSessions.Enable (pipelines);

			pipelines.AfterRequest += ConfigureCookies;
		}


		
		private void ConfigureCookies(NancyContext context)
		{
			// We want that each cookie has the same path
			// See: https://github.com/NancyFx/Nancy/issues/256

			foreach (var cookie in context.Response.Cookies)
			{
				cookie.Path = "/";
			}
		}


		private readonly CoreServer coreServer;


		
	}


}
