using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public abstract class AbstractCoreModule : NancyModule
	{


		protected AbstractCoreModule(CoreServer coreServer)
			: base ()
		{
			this.coreServer = coreServer;			
		}


		protected AbstractCoreModule(CoreServer coreServer, string modulePath)
			: base (modulePath)
		{
			this.coreServer = coreServer;
		}


		protected CoreServer CoreServer
		{
			get
			{
				return this.coreServer;
			}
		}


		private readonly CoreServer coreServer;


	}


}
