//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections.Concurrent;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// This class contains a blocking set of CoreWorkers. Its purpose is to provide them in a
	/// thread safe way to the Nancy modules that want to access them. The core of this class is
	/// the Execute(...) method.
	/// </summary>
	public sealed class CoreWorkerPool : System.IDisposable
	{
		public CoreWorkerPool(int coreWorkerCount, CultureInfo uiCulture)
		{
			this.workers            = new List<CoreWorker> ();
			this.idleWorkers        = new BlockingBag<CoreWorker> ();
			this.safeSectionManager = new SafeSectionManager ();

			var workers = CoreWorkerPool.StartCoreWorkers (coreWorkerCount, uiCulture);

			this.workers.AddRange (workers);
			this.idleWorkers.AddRange (workers);

			Logger.LogToConsole ("Core worker pool started");
		}

		public T Execute<T>(string username, string sessionId, System.Func<BusinessContext, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (username, sessionId, function));
		}

		public T Execute<T>(string username, string sessionId, System.Func<WorkerApp, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (username, sessionId, function));
		}

		public T Execute<T>(System.Func<UserManager, T> function)
		{
			return this.Execute (coreWorker => coreWorker.Execute (function));
		}

		
		private static IList<CoreWorker> StartCoreWorkers(int coreWorkerCount, CultureInfo uiCulture)
		{
			var workers = new List<CoreWorker> ();

			if (coreWorkerCount > 0)
			{
				//	Here we start the first core worker alone. So we are sure that any code that must be
				//	run to initialize global stuff that is not yet initialized is initialized on a single
				//	thread and so that we don't have race conditions or other threading problems.

				CoreWorkerPool.CreateCoreWorker (workers, uiCulture, 0);

				if (coreWorkerCount > 1)
				{
					Parallel.For (1, coreWorkerCount, i => CoreWorkerPool.CreateCoreWorker (workers, uiCulture, i));
				}
			}

			return workers;
		}

		private static void CreateCoreWorker(IList<CoreWorker> workers, CultureInfo uiCulture, int index)
		{
			var workerName = CoreWorkerPool.GetWorkerName (index);
			var worker     = new CoreWorker (workerName, uiCulture);

			lock (workers)
			{
				workers.Add (worker);
			}

			Logger.LogToConsole (string.Format ("{0} started", workerName));
		}

		private static string GetWorkerName(int index)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "CoreWorker #{0:00}", index+1);
		}

		private T Execute<T>(System.Func<CoreWorker, T> function)
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

		#region IDisposable Members

		public void Dispose()
		{
			this.safeSectionManager.Dispose ();

			foreach (var coreWorker in this.workers)
			{
				coreWorker.Dispose ();
			}

			this.idleWorkers.Dispose ();
		}

		#endregion

		private readonly List<CoreWorker>		 workers;
		private readonly BlockingBag<CoreWorker> idleWorkers;
		private readonly SafeSectionManager		 safeSectionManager;
	}
}
