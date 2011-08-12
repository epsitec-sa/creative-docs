using Nancy;
using Nancy.Session;
using Nancy.Authentication.Forms;

namespace Epsitec.Cresus.Core.Server
{
	public class CoreServerBootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			CookieBasedSessions.Enable (this);

			var formsAuthConfiguration = 
                new FormsAuthenticationConfiguration ()
				{
					RedirectUrl = "~/log",
					UsernameMapper = container.Resolve<IUsernameMapper> ()
				};

			FormsAuthentication.Enable (this, formsAuthConfiguration);

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
