using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public sealed class CoreServer : IDisposable
	{


		public CoreServer(int nbCoreWorkers)
		{
			var coreWorkerPool = new CoreWorkerPool (nbCoreWorkers);

			this.coreWorkerPool = coreWorkerPool;
			this.authenticationManager = new AuthenticationManager (coreWorkerPool);
			this.propertyAccessorCache = new PropertyAccessorCache ();
			this.autoCreatorCache = new AutoCreatorCache ();
			this.databaseManager = new DatabaseManager ();
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


		internal PropertyAccessorCache PropertyAccessorCache
		{
			get
			{
				return this.propertyAccessorCache;
			}
		}


		internal AutoCreatorCache AutoCreatorCache
		{
			get
			{
				return this.autoCreatorCache;
			}
		}


		internal DatabaseManager DatabaseManager
		{
			get
			{
				return this.databaseManager;
			}
		}


		public void Dispose()
		{
			this.coreWorkerPool.Dispose ();
			this.propertyAccessorCache.Dispose ();
			this.autoCreatorCache.Dispose ();
		}


		private readonly CoreWorkerPool coreWorkerPool;


		private readonly AuthenticationManager authenticationManager;


		private readonly PropertyAccessorCache propertyAccessorCache;


		private readonly AutoCreatorCache autoCreatorCache;


		private readonly DatabaseManager databaseManager;


	}


}
