using Nancy;
using Nancy.Session;

namespace Epsitec.Cresus.Core.Server
{
	public class CoreServerBootstrapper : DefaultNancyBootstrapper
	{
		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
		{
			base.InitialiseInternal (container);

			// TODO Mettre en route la sesssion
			//CookieBasedSessions.Enable (this, "MyPassPhrase", "MySaltIsReallyGood", "MyHmacPassphrase");
			//CookieBasedSessions.Enable (this);
		}
	}
}
