using System;


namespace Epsitec.Cresus.Core.Server.CoreServer
{


	public sealed class ServerContext : IDisposable
	{


		public ServerContext()
		{
			this.coreSessionManager = new CoreSessionManager (this);
			this.authentificationManager = new AuthentificationManager (this);
		}


		public CoreSessionManager CoreSessionManager
		{
			get
			{
				return this.coreSessionManager;
			}
		}


		public AuthentificationManager AuthentificationManager
		{
			get
			{
				return this.authentificationManager;
			}
		}


		public void Dispose()
		{
			this.authentificationManager.Dispose ();
			this.coreSessionManager.Dispose ();
		}


		private readonly CoreSessionManager coreSessionManager;


		private readonly AuthentificationManager authentificationManager;


	}


}
