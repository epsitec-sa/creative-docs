using Epsitec.Common.Support;

using System;

using System.Collections.Generic;

using System.Net;

using System.Threading;


namespace Epsitec.Cresus.Core.Server.NancyHosting
{


	internal sealed class HttpServer : IDisposable
	{
		
		// This class has been largely inspired from source code found here :
		// http://stackoverflow.com/questions/4672010


		public HttpServer(Uri uri)
		{
			this.requestQueue = new Queue<HttpListenerContext> ();
			this.stopEvent = new ManualResetEvent (false);
			this.readyEvent = new AutoResetEvent (false);
			this.httpListener = new HttpListener ();
			this.httpListener.Prefixes.Add (uri.ToString ());

			this.httpListenerLock = new object ();
			this.requestQueueLock = new object ();
		}


		public void Start(Action<HttpListenerContext> requestProcessor, int nbThreads)
		{
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
			this.readyEvent.Dispose ();
		}


		public void Stop()
		{
			this.stopEvent.Set ();
			
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
				lock (this.requestQueueLock)
				{
					if (this.httpListener.IsListening)
					{
						this.requestQueue.Enqueue (this.httpListener.EndGetContext (asyncResult));

						this.readyEvent.Set ();
					}
				}
			}
		}


		private void ProcessRequests(Action<HttpListenerContext> requestProcessor)
		{
			while (ThreadUtils.WaitAny (this.readyEvent, this.stopEvent) != this.stopEvent)
			{
				HttpListenerContext context;

				lock (this.requestQueueLock)
				{
					if (this.requestQueue.Count > 0)
					{
						context = this.requestQueue.Dequeue ();
					}
					else
					{
						context = null;
					}
				}

				if (context != null)
				{
					requestProcessor (context);
				}
			}
		}


		private readonly HttpListener httpListener;


		private Queue<HttpListenerContext> requestQueue;


		private Thread httpListenerThread;


		private	List<Thread> workerThreads;


		/// <summary>
		/// This event is raised when the Stop() method is called to notify the listener thread and
		/// the worker thread that they should stop their execution once they have finished what
		/// they are doing.
		/// </summary>
		private readonly ManualResetEvent stopEvent;


		/// <summary>
		/// This event is raised when a request is enqueued on the queue of requests by the
		/// asynchronous callback of the http listener to notify the worker threads that there is
		/// work to do.
		/// </summary>
		private readonly AutoResetEvent readyEvent;


		/// <summary>
		/// This lock is here to synchronize the queue of requests, to make sure that we don't
		/// enqueue one while another is dequeued.
		/// </summary>
		private readonly object requestQueueLock;


		/// <summary>
		/// This lock is used to ensure that we are not messing with the http listener in the
		/// EnqueueRequest method if it is called back asynchronously after the listener and the
		/// worker thread have been joined.
		/// </summary>
		private readonly object httpListenerLock;


	}

}
