using Epsitec.Common.IO;

using Epsitec.Cresus.WebCore.Server.Core.Databases;

using System;

using System.Globalization;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public sealed class CoreServer : IDisposable
	{


		public CoreServer(int nbCoreWorkers, CultureInfo uiCulture)
		{
			var coreWorkerPool = new CoreWorkerPool (nbCoreWorkers, uiCulture);

			this.coreWorkerPool = coreWorkerPool;
			this.authenticationManager = new AuthenticationManager (coreWorkerPool);
			this.databaseManager = new DatabaseManager ();
			this.caches = new Caches ();

			Logger.LogToConsole ("Core server started");
		}


		public CoreWorkerPool CoreWorkerPool
		{
			get
			{
				return this.coreWorkerPool;
			}
		}
		
		
		public AuthenticationManager AuthenticationManager
		{
			get
			{
				return this.authenticationManager;
			}
		}


		internal DatabaseManager DatabaseManager
		{
			get
			{
				return this.databaseManager;
			}
		}


		internal Caches Caches
		{
			get
			{
				return this.caches;
			}
		}


		public void Dispose()
		{
			this.coreWorkerPool.Dispose ();
			this.caches.Dispose ();
		}


		private readonly CoreWorkerPool coreWorkerPool;


		private readonly AuthenticationManager authenticationManager;


		private readonly DatabaseManager databaseManager;


		private readonly Caches caches;


	}


}
