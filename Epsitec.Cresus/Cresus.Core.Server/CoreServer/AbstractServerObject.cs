namespace Epsitec.Cresus.Core.Server.CoreServer
{


	internal abstract class AbstractServerObject
	{


		public AbstractServerObject(ServerContext serverContext)
		{
			this.serverContext = serverContext;
		}


		public ServerContext ServerContext
		{
			get
			{
				return this.serverContext;
			}
		}


		private readonly ServerContext serverContext;


	}


}
