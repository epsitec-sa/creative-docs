using System;

using System.Collections.Generic;

using System.Net;

using System.Threading;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The HttpServer class is a simple http server that will listen to requests made to an uri and
	/// call a callback to handle them. It uses the thread pool to process the requests in order to
	/// be more efficient, so expect the callbacks to be called from any thread.
	/// </summary>
	public sealed class HttpServer : IDisposable
	{


		/// <summary>
		/// Creates a new HttpServer which will immediately start to process the requests.
		/// </summary>
		/// <param name="uri">The uri which the server will listen to.</param>
		/// <param name="requestHandler">The callback that will be called to handle the requests.</param>
		public HttpServer(Uri uri, Action<HttpListenerRequest, HttpListenerResponse> requestHandler)
		{
			this.requestHandler = requestHandler;

			this.safeSectionManager = new SafeSectionManager ();
			this.httpListenerExclusion = new object ();

			// Set up and starts the http listener. Note that here we don't need to lock around
			// those calls, as the listener thread has not been created yet and thus nobody else can
			// access it.
			this.httpListener = new HttpListener ();
			this.httpListener.Prefixes.Add (uri.ToString ());
			this.httpListener.Start ();

			// Set up and starts the thread that will asynchronously handle the requests of the
			// HttpListener.
			this.listenerThread = new Thread (this.HandleRequests);
			this.listenerThread.Start ();
		}


		/// <summary>
		/// Terminates this instance and closes the server. This method will block until all pending
		/// requests have been executed before closing the server.
		/// </summary>
		public void Dispose()
		{
			// Waits until all pending requests have been executed.
			this.safeSectionManager.Dispose ();

			// Closes the HttpListener, which will prevent any new requests to be made and which
			// will signal the listener thread to terminate.
			lock (this.httpListenerExclusion)
			{
				this.httpListener.Stop ();
			}

			// Waits for the listener thread to terminate.
			this.listenerThread.Join ();

			// Now that the http listener don't accept new requests and that all the pending ones
			// have been executed, we can finally close the http listener.
			this.httpListener.Close ();
		}


		/// <summary>
		/// This method is executed by the thread that will asynchronously handle the requests of
		/// the HttpListener. Basically it will handle those requests until the Stop() method is
		/// called on the HttpListener. At this point, it will terminate.
		/// </summary>
		private void HandleRequests()
		{
			while (true)
			{
				IAsyncResult result;

				lock (this.httpListenerExclusion)
				{
					// Checks if the HttpListener has been stopped. If that's the case, the Dispose
					// method has been called and we must terminate.
					var isStopped = !this.httpListener.IsListening;

					if (isStopped)
					{
						break;
					}

					// Starts to wait asynchronously for a new request. The request will be handled
					// on a thread of the thread pool which will execute the callback that we give
					// it.
					result = this.httpListener.BeginGetContext (this.HandleRequest, null);
				}

				// Wait for a new request to arrive or for the HttpListener to be closed.
				result.AsyncWaitHandle.WaitOne ();
			}
		}


		/// <summary>
		/// This method is the callback that will be called to handle the http requests by the
		/// HttpListener after the call to the BeginGetContext method. Basically it will try to
		/// start a new safe section. If the creation is successful, it will process the http
		/// request, knowing that the HttpListener won't be stopped until it finishes, thanks to the
		/// safe section. If the safe section cannot be started, that means that the Dispose()
		/// method has been called and that we should not process the request but simply exit.
		/// </summary>
		/// <remarks>
		/// This method will always be executed by a thread on the thread pool, as it is the
		/// callback of an asynchronous operation.
		/// </remarks>
		/// <param name="result">The result of the asynchronous operation.</param>
		private void HandleRequest(IAsyncResult result)
		{
			// Try to enter a new safe section to ensure that the HttpListener won't be closed while
			// we are executing this method.
			using (var safeSection = this.safeSectionManager.TryCreate ())
			{
				// The safe section could not be created because the Dispose method has been called,
				// thus we exit this function.
				if (safeSection == null)
				{
					return;
				}

				// The safe section has been created and we can process the request. The first part
				// of the job is to obtain it from the HttpListener.
				HttpListenerContext httpListenerContext;

				lock (this.httpListenerExclusion)
				{
					httpListenerContext = this.httpListener.EndGetContext (result);
				}
				
				// Now that everything is setup, we process the http request by executing the
				// callback given in the constructor of this instance.
				var request = httpListenerContext.Request;
				var response = httpListenerContext.Response;
				
				this.requestHandler (request, response);
				
				// Closes the response to ensure that the data has been sent to the client.
				response.Close();
			}
		}


		/// <summary>
		/// The action that will be executed for each http request received by this instance.
		/// </summary>
		private readonly Action<HttpListenerRequest, HttpListenerResponse> requestHandler;


		/// <summary>
		/// The HttpListener used to listen to http requests.
		/// </summary>
		private readonly HttpListener httpListener;


		/// <summary>
		/// The worker thread that will use the HttpListener to handle the requests.
		/// </summary>
		private readonly Thread listenerThread;


		/// <summary>
		/// The SafeSectionManager used to ensure that this instance can't be disposed while there
		/// are pending requests.
		/// </summary>
		private readonly SafeSectionManager safeSectionManager;


		/// <summary>
		/// The instance members of the HttpListener class are not thread safe, according to the
		/// MSDN documentation, which is really surprising. Therefore, we must take care of the
		/// synchronization by ourselves. That's the purpose of this member. Every call to a method
		/// of the HttpListener is surrounded by a lock on this member.
		/// </summary>
		private readonly object httpListenerExclusion;


	}


}
