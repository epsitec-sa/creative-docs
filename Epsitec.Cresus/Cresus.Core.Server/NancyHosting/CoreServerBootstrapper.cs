//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Epsitec.Cresus.Core.Server.CoreServer;

using Nancy;

using Nancy.Bootstrapper;

using Nancy.ErrorHandling;

using Nancy.Session;

using TinyIoC;


namespace Epsitec.Cresus.Core.Server.NancyHosting
{
	
	
	/// <summary>
	/// Called by Nancy when the server is starting
	/// </summary>
	internal class CoreServerBootstrapper : DefaultNancyBootstrapper
	{


		public CoreServerBootstrapper(ServerContext serverContext)
		{
			this.serverContext = serverContext;
		}


		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer (container);

			container.Register<IErrorHandler> (new CoreErrorHandler ());
			container.Register<ServerContext> (this.serverContext);
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


		private readonly ServerContext serverContext;


		
	}


}
