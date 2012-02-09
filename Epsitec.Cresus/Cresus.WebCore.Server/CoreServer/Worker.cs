using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections.Concurrent;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	internal sealed class Worker : System.IDisposable
	{
		public Worker()
		{
			this.actionQueue = new BlockingQueue<System.Action> ();
			this.stopTokenCancellationSource = new CancellationTokenSource ();

			this.workerThreadStop = new ManualResetEvent (false);
			this.workerThread = new Thread (() => this.DoWork ());
			this.workerThread.Start ();
		}


		public void Execute(System.Action action)
		{
			using (var actionWaitHandle = new ManualResetEvent (false))
			{
				System.Action actionWithSignal = () =>
				{
					try
					{
						action ();
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
