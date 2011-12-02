using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Base module thats allows to easly get the CoreSession for a defined user,
	/// and that requires the user to be logged in.
	/// </summary>
	public abstract class AbstractCoreModule : NancyModule
	{
		
		
		protected AbstractCoreModule(ServerContext serverContext): base()
		{
			this.serverContext = serverContext;			
		}


		protected AbstractCoreModule(ServerContext serverContext, string modulePath) : base (modulePath)
		{
			this.serverContext = serverContext;
		}


		internal ServerContext ServerContext
		{
			get
			{
				return this.serverContext;
			}
		}


		private readonly ServerContext serverContext;


	}


}
