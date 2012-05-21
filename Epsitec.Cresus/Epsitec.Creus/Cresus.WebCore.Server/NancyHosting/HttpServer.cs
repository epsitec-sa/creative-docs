using Epsitec.Common.Support;

using Epsitec.Common.Types.Collections.Concurrent;

using System;

using System.Collections.Generic;

using System.Net;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{


	internal sealed class HttpServer : IDisposable
	{
		
		// This class has been largely inspired from source code found here :
		// http://stackoverflow.com/questions/4672010


		public HttpServer(Uri uri)
		{
			this.requestQueue = new BlockingQueue<HttpListenerContext> ();
			this.stopEvent = new ManualResetEvent (false);
			this.httpListener = new HttpListener ();
			this.httpListener.Prefixes.Add (uri.ToString ());

			this.httpListenerLock = new object ();
		}


		public void Start(Action<HttpListenerContext> requestProcessor, int nbThreads)
		{
			this.stopTokenCancellationSource = new CancellationTokenSource ();
			
			this.httpListener.Start ();

			this.httpListenerThread = new Thread (() => this.HandleRequests ());			
			this.httpListenerThread.Start ();

			this.workerThreads = new List<Thread> (nbThreads);

			for (int i = 0; i < nbThreads; i++)
			{
				var workerThread = new Thread (() => this.ProcessRequests (requestProcessor));

				this.workerThreads.Add (workerThread);

				workerThread.Start ();
			}
		}


		public void Dispose()
		{
			this.Stop ();

			this.httpListener.Close ();
			((IDisposable) this.httpListener).Dispose ();

			this.stopEvent.Dispose ();
			this.stopTokenCancellationSource.Dispose ();
		}


		public void Stop()
		{
			this.stopEvent.Set ();
			this.stopTokenCancellationSource.Cancel ();

			this.httpListenerThread.Join ();

			foreach (var workerThread in this.workerThreads)
			{
				workerThread.Join ();
			}

			lock (this.httpListenerLock)
			{
				this.httpListener.Stop ();
			}

			this.stopEvent.Reset ();
		}


		private void HandleRequests()
		{
			while (this.httpListener.IsListening)
			{
				var asyncResult = this.httpListener.BeginGetContext ((ar) => this.EnqueueRequest (ar), null);

				var firedWaitHandle = ThreadUtils.WaitAny (this.stopEvent, asyncResult.AsyncWaitHandle);

				if (firedWaitHandle == this.stopEvent)
				{
					break;
				}
			}
		}


		private void EnqueueRequest(IAsyncResult asyncResult)
		{
			lock (this.httpListenerLock)
			{
				if (this.httpListener.IsListening)
				{
					var context = this.httpListener.EndGetContext (asyncResult);

					this.requestQueue.Add (context);
				}
			}
		}


		private void ProcessRequests(Action<HttpListenerContext> requestProcessor)
		{
			bool cancelled = false;

			while (!cancelled)
			{
				var cancellationToken = this.stopTokenCancellationSource.Token;

				try
				{
					var context = this.requestQueue.Take (cancellationToken);

					requestProcessor (context);
				}
				catch (OperationCanceledException e)
				{
					if (e.CancellationToken == cancellationToken)
					{
						cancelled = true;
					}
				}
			}
		}


		private readonly HttpListener httpListener;


		private readonly BlockingQueue<HttpListenerContext> requestQueue;


		private Thread httpListenerThread;


		private	List<Thread> workerThreads;


		/// <summary>
		/// This event is raised when the Stop() method is called to notify the listener thread 
		/// that it should stop its execution once it has finished what it is doing.
		/// </summary>
		private readonly ManualResetEvent stopEvent;


		/// <summary>
		/// This token cancellation source is canceled with the Stop() method is called to notify
		/// the listener threads that they should stop their execution once they have finished what
		/// they are doing.
		/// </summary>
		private CancellationTokenSource stopTokenCancellationSource;


		/// <summary>
		/// This lock is used to ensure that we are not messing with the http listener in the
		/// EnqueueRequest method if it is called back asynchronously after the listener and the
		/// worker thread have been joined.
		/// </summary>
		private readonly object httpListenerLock;


	}

}
