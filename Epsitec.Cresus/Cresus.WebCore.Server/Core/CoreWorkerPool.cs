using Epsitec.Common.IO;

using Epsitec.Common.Support;

using Epsitec.Common.Types.Collections.Concurrent;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.Threading.Tasks;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	public sealed class CoreWorkerPool : IDisposable
	{


		public CoreWorkerPool(int nbCoreWorkers, CultureInfo uiCulture)
		{
			this.workers = new List<CoreWorker> ();
			this.idleWorkers = new BlockingBag<CoreWorker> ();

			foreach (var worker in this.StartCoreWorkers (nbCoreWorkers, uiCulture))
			{
				this.workers.Add (worker);
				this.idleWorkers.Add (worker);
			}

			this.safeSectionManager = new SafeSectionManager ();

			Logger.LogToConsole ("Core worker pool started");
		}


		private IList<CoreWorker> StartCoreWorkers(int nbCoreWorkers, CultureInfo uiCulture)
		{
			var workers = new List<CoreWorker> ();

			if (nbCoreWorkers > 0)
			{
				// Here we start the first core worker alone. So we are sure that any code that must be
				// run to initialize global stuff that is not yet initialized is initialized on a single
				// thread and so that we don't have race conditions or other threading problems.

				var firstWorker = new CoreWorker (uiCulture);
				Logger.LogToConsole ("Core worker #1 started");

				workers.Add (firstWorker);

				if (nbCoreWorkers > 1)
				{
					var exclusion = new object ();

					Parallel.For (2, nbCoreWorkers + 1, i =>
					{
						var newWorker = new CoreWorker (uiCulture);
						Logger.LogToConsole ("Core worker #" + i + " started");

						lock (exclusion)
						{
							workers.Add (newWorker);
						}
					});
				}
			}

			return workers;
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
