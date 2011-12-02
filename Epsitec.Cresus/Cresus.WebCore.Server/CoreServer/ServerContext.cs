using System;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{


	public sealed class ServerContext : IDisposable
	{


		public ServerContext(int maxNbSessions, TimeSpan sessionTimeout, TimeSpan sessionCleanupInterval)
		{
			this.coreSessionManager = new CoreSessionManager (maxNbSessions, sessionTimeout);
			this.coreSessionCleaner = new CoreSessionCleaner (this.coreSessionManager, sessionCleanupInterval);
			this.authenticationManager = new AuthenticationManager ();
		}


		public CoreSessionManager CoreSessionManager
		{
			get
			{
				return this.coreSessionManager;
			}
		}


		public AuthenticationManager AuthenticationManager
		{
			get
			{
				return this.authenticationManager;
			}
		}


		public void Dispose()
		{
			this.authenticationManager.Dispose ();
			this.coreSessionCleaner.Dispose ();
			this.coreSessionManager.Dispose ();
		}


		private readonly CoreSessionManager coreSessionManager;


		private readonly CoreSessionCleaner coreSessionCleaner;


		private readonly AuthenticationManager authenticationManager;


	}


}
