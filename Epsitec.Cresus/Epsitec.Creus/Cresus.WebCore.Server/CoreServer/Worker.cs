//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections.Concurrent;

using System.Threading;

namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	/// <summary>
	/// The <c>Worker</c> class provides a single method, <see cref="ExecuteSync"/>, which is
	/// used to execute work on a thread, which will always be the same, independently of the
	/// calling thread.
	/// </summary>
	internal sealed class Worker : System.IDisposable
	{
		public Worker()
		{
			this.actionQueue = new BlockingQueue<System.Action> ();
			this.stopTokenCancellationSource = new CancellationTokenSource ();

			this.workerThreadStop = new ManualResetEvent (false);
			this.workerThread = new Thread (this.DoWork);
			this.workerThread.Start ();
		}


		/// <summary>
		/// Synchronously executes an action on the worker thread, and block
		/// until the thread has finished executing the specified action.
		/// </summary>
		/// <param name="action">The action.</param>
		public void ExecuteSync(System.Action action)
		{
			using (var actionWaitHandle = new ManualResetEvent (false))
			{
				System.Exception innerException = null;

				System.Action actionWithSignal = () =>
				{
					try
					{
						action ();
					}
					catch (System.Exception ex)
					{
						innerException = ex;
					}
					finally
					{
						actionWaitHandle.Set ();
					}
				};

				this.actionQueue.Add (actionWithSignal);

				var firedWaitHandle = ThreadUtils.WaitAny (actionWaitHandle, this.workerThreadStop);

				if (firedWaitHandle == this.workerThreadStop)
				{
					throw new System.OperationCanceledException ("The operation has been canceled.");
				}
				if (innerException != null)
				{
					throw new System.Exception ("The operation threw an exception.", innerException);
				}
			}
		}

		public void Dispose()
		{
			this.stopTokenCancellationSource.Cancel ();
			this.workerThread.Join ();

			this.stopTokenCancellationSource.Dispose ();
			this.workerThreadStop.Dispose ();
			this.actionQueue.Dispose ();
		}

		private void DoWork()
		{
			bool cancelled = false;

			while (!cancelled)
			{
				var cancellationToken = this.stopTokenCancellationSource.Token;

				try
				{
					var action = this.actionQueue.Take (cancellationToken);

					action ();
				}
				catch (System.OperationCanceledException e)
				{
					if (e.CancellationToken == cancellationToken)
					{
						cancelled = true;
					}
				}
			}

			this.workerThreadStop.Set ();
		}


		private readonly Thread						workerThread;
		private readonly ManualResetEvent			workerThreadStop;
		private readonly CancellationTokenSource	stopTokenCancellationSource;
		private readonly BlockingQueue<System.Action> actionQueue;
	}
}
