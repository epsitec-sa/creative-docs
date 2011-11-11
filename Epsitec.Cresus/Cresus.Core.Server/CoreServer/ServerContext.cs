using System;


namespace Epsitec.Cresus.Core.Server.CoreServer
{


	public sealed class ServerContext : IDisposable
	{


		public ServerContext(int maxNbSessions, TimeSpan sessionTimeout, TimeSpan sessionCleanupInterval)
		{
			this.coreSessionManager = new CoreSessionManager (maxNbSessions, sessionTimeout);
			this.coreSessionCleaner = new CoreSessionCleaner (this.coreSessionManager, sessionCleanupInterval);
			this.authentificationManager = new AuthentificationManager ();
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
			this.coreSessionCleaner.Dispose ();
			this.coreSessionManager.Dispose ();
		}


		private readonly CoreSessionManager coreSessionManager;


		private readonly CoreSessionCleaner coreSessionCleaner;


		private readonly AuthentificationManager authentificationManager;


	}


}
