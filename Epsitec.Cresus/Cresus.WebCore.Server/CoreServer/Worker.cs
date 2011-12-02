using Epsitec.Common.Support;

using System;

using System.Collections.Concurrent;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	
	
	internal sealed class Worker : IDisposable 
	{


		public Worker()
		{
			this.actionQueue = new BlockingCollection<Action> (new ConcurrentQueue<Action> ());
			this.stopTokenCancellationSource = new CancellationTokenSource ();

			this.workerThreadStop = new ManualResetEvent (false);
			this.workerThread = new Thread (() => this.DoWork ());
			this.workerThread.Start ();
		}


		public void Execute(Action action)
		{
			using (var actionWaitHandle = new ManualResetEvent (false))
			{
				Action actionWithSignal = () =>
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
					throw new OperationCanceledException ("The operation has been canceled.");
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
				catch (OperationCanceledException e)
				{
					if (e.CancellationToken == cancellationToken)
					{
						cancelled = true;
					}
				}
			}

			this.workerThreadStop.Set ();
		}


		private readonly Thread workerThread;


		private readonly ManualResetEvent workerThreadStop;


		private readonly CancellationTokenSource stopTokenCancellationSource;


		private readonly BlockingCollection<Action> actionQueue;


	}


}
