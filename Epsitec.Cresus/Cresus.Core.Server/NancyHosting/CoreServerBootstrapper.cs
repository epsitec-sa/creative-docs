//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using Epsitec.Cresus.Core.Server.CoreServer;

using Nancy;

using Nancy.Bootstrapper;

using Nancy.ErrorHandling;

using Nancy.Session;


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


		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			// Register the error handler
			container.Register<IErrorHandler> (new CoreErrorHandler ());

			// Registers the server context.
			container.Register<ServerContext> (this.serverContext);

			/// Enable the sessions
			CookieBasedSessions.Enable (this);

			this.AfterRequest += ConfigureCookies;
		}

		/// <summary>
		/// We want that each cookie has the same path
		/// See: https://github.com/NancyFx/Nancy/issues/256
		/// </summary>
		/// <param name="context"></param>
		private void ConfigureCookies(NancyContext context)
		{
			foreach (var c in context.Response.Cookies)
			{
				c.Path = "/";
			}
		}


		private readonly ServerContext serverContext;


		
	}


}
