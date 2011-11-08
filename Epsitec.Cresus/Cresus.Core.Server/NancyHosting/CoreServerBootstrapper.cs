//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Nancy;

using Nancy.Bootstrapper;

using Nancy.ErrorHandling;

using Nancy.Session;


namespace Epsitec.Cresus.Core.Server.NancyHosting
{
	
	
	/// <summary>
	/// Called by Nancy when the server is starting
	/// </summary>
	public class CoreServerBootstrapper : DefaultNancyBootstrapper
	{

		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			// Register the error handler
			container.Register<Nancy.ErrorHandling.IErrorHandler> (new CoreErrorHandler ());

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
	}


}
