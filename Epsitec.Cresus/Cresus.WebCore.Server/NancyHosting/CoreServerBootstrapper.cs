//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;
using Nancy.Bootstrapper;
using Nancy.ErrorHandling;
using Nancy.Session;
using Nancy.TinyIoc;


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

			container.Register<IStatusCodeHandler> (new CoreErrorHandler ());
			container.Register<CoreServer> (this.coreServer);
		}


		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			base.ApplicationStartup (container, pipelines);

			CookieBasedSessions.Enable (pipelines);

			pipelines.AfterRequest += this.ConfigureCookies;
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


		private readonly CoreServer				coreServer;
	}


}
