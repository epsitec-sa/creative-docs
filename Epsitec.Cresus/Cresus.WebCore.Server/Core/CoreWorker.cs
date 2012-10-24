//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using System;

using System.Globalization;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	/// <summary>
	/// The <c>CoreWorker</c> class is the combination of a <c>WorkerApp</c> and of a
	/// <c>WorkerThread</c>. It enables users to execute synchronously execute functions with a
	/// <c>WorkerApp</c> with the guarantee that the functions will be executed on the same thread
	/// that was used to instantiate the WorkerApp.
	/// </summary>
	public sealed class CoreWorker : IDisposable
	{


		/// <summary>
		/// Creates a new instance of <c>CoreWorker</c>.
		/// </summary>
		/// <param name="uiCulture">The culture that should be used for doing UI stuff</param>
		public CoreWorker(CultureInfo uiCulture)
		{
			this.workerThread = new WorkerThread ();

			WorkerApp workerApp = null;

			try
			{
				this.workerThread.ExecuteSynchronously
				(
					() => Thread.CurrentThread.CurrentUICulture = uiCulture
				);

				this.workerThread.ExecuteSynchronously (() => workerApp = new WorkerApp ());

				this.workerApp = workerApp;
			}
			catch
			{
				if (workerApp != null)
				{
					this.workerThread.ExecuteSynchronously (() => workerApp.Dispose ());
				}

				this.workerThread.Dispose ();

				throw;
			}

			this.safeSectionManager = new SafeSectionManager ();
		}


		/// <summary>
		/// Synchronously executes the given function, using a fresh <c>BusinessContext</c> created
		/// by the <c>WorkerApp</c> owned by this instance and that will be automatically disposed
		/// at the end of the call.
		/// </summary>
		/// <remarks>
		/// See remarks in the other Execute overload.
		/// </remarks>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="username">The username for which to execute the function.</param>
		/// <param name="sessionId">The session id for which to execute the function.</param>
		/// <param name="function">The function to execute.</param>
		/// <returns>The result of the execution of the function.</returns>
		public T Execute<T>(string username, string sessionId, Func<BusinessContext, T> function)
		{
			return this.Execute (coreWorkerApp => coreWorkerApp.Execute (username, sessionId, function));
		}


		/// <summary>
		/// Synchronously executes the given function, using the <c>WorkerApp</c> owned by this
		/// instance.
		/// </summary>
		/// <remarks>
		/// See remarks in the other Execute overload.
		/// </remarks>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="username">The username for which to execute the function.</param>
		/// <param name="sessionId">The session id for which to execute the function.</param>
		/// <param name="function">The function to execute.</param>
		/// <returns>The result of the execution of the function.</returns>
		public T Execute<T>(string username, string sessionId, Func<WorkerApp, T> function)
		{
			return this.Execute (coreWorkerApp => coreWorkerApp.Execute (username, sessionId, function));
		}


		/// <summary>
		/// Synchronously executes the given function, using the <c>UserManager</c> of the
		/// <c>WorkerApp</c> owned by this instance.
		/// </summary>
		/// <remarks>
		/// See remarks in the other Execute overload.
		/// </remarks>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="function">The function to execute.</param>
		/// <returns>The result of the execution of the function.</returns>
		public T Execute<T>(Func<UserManager, T> function)
		{
			return this.Execute (coreWorkerApp => coreWorkerApp.Execute (function));
		}


		/// <summary>
		/// Synchronously executes the given function, using the <c>WorkerApp</c> owned by this
		/// instance.
		/// </summary>
		/// <remarks>
		/// The execution will take place on the same thread that was used to instantiate the
		/// <c>WorkerApp</c> owned by this instance.
		/// 
		/// This method will block the calls to the Dispose method until the end of its execution.
		/// 
		/// When calling this function, you must ensure that the function does not return anything
		/// involving something whose execution is delayed, such as a LINQ statement.
		/// </remarks>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="function">The function to execute.</param>
		/// <returns>The result of the execution of the function.</returns>
		private T Execute<T>(Func<WorkerApp, T> function)
		{
			using (this.safeSectionManager.Create())
			{
				var value = default (T);
				
				this.workerThread.ExecuteSynchronously (() => value = function (this.workerApp));

				return value;
			}
		}


		/// <summary>
		/// Dispose this instance. This method will wait the end of all pending executions before
		/// disposing this instance and returning.
		/// </summary>
		public void Dispose()
		{
			this.safeSectionManager.Dispose ();

			this.workerThread.ExecuteSynchronously (() => this.workerApp.Dispose ());
			this.workerThread.Dispose ();
		}


		/// <summary>
		/// The WorkerThread used to instantiate, dispose and use the WorkerApp owned by this
		/// instance.
		/// </summary>
		private readonly WorkerThread workerThread;


		/// <summary>
		/// The WorkerApp owned by this instance.
		/// </summary>
		private readonly WorkerApp workerApp;


		/// <summary>
		/// The SafeSectionManager used to synchronize the Execute and the Dispose method.
		/// </summary>
		private readonly SafeSectionManager safeSectionManager;


	}


}
