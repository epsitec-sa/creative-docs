using Epsitec.Common.Support;

using Epsitec.Common.Types.Collections.Concurrent;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public sealed class CoreWorkerPool : IDisposable
	{


		public CoreWorkerPool(int nbCoreWorkers, CultureInfo uiCulture)
		{
			this.workers = new List<CoreWorker>();
			this.idleWorkers = new BlockingBag<CoreWorker> ();

			for (int i = 0; i < nbCoreWorkers; i++)
			{
				var coreWorker = new CoreWorker (uiCulture);

				this.workers.Add (coreWorker);
				this.idleWorkers.Add (coreWorker);				
			}

			this.safeSectionManager = new SafeSectionManager ();
		}


		public T Execute<T>(string username, string sessionId, Func<BusinessContext, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (username, sessionId, function));
		}


		public T Execute<T>(string username, string sessionId, Func<WorkerApp, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (username, sessionId, function));
		}


		public T Execute<T>(Func<UserManager, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (function));
		}


		private T Execute<T>(Func<CoreWorker, T> function)
		{
			using (this.safeSectionManager.Create ())
			{
				var coreWorker = this.idleWorkers.Take ();

				try
				{
					return function (coreWorker);
				}
				finally
				{
					this.idleWorkers.Add (coreWorker);
				}
			}
		}


		public void Dispose()
		{
			this.safeSectionManager.Dispose ();

			foreach (var coreWorker in this.workers)
			{
				coreWorker.Dispose ();
			}

			this.idleWorkers.Dispose ();
		}


		private readonly List<CoreWorker> workers;


		private readonly BlockingBag<CoreWorker> idleWorkers;


		private readonly SafeSectionManager safeSectionManager;


	}


}
