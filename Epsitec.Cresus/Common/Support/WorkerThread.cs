//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections.Concurrent;

using System.Threading;


namespace Epsitec.Common.Support
{
	using PendingActionsQueue = BlockingQueue<System.Action>;

	/// <summary>
	/// The <c>WorkerThread</c> class wraps a thread and allow its user to execute actions either
	/// synchronously or asynchronously on it.
	/// </summary>
	public sealed class WorkerThread : System.IDisposable
	{
		// NOTE This class has been largely inspired by the class found on the following blog post :
		// http://blogs.msdn.com/b/jaredpar/archive/2008/03/30/activeobject.aspx


		/// <summary>
		/// Create a new instance of <c>WorkerThread</c>.
		/// </summary>
		public WorkerThread(string threadName = null)
		{
			this.pendingActionsQueue = new BlockingQueue<System.Action> ();

			this.thread = new Thread (this.DoWork);
			this.thread.Name = threadName ?? "anonymous worker thread";
			this.thread.Start ();
		}


		/// <summary>
		/// Synchronously executes an action on the worker thread, and block until the thread has
		/// executed the action.
		/// </summary>
		/// <param name="action">The action to execute on the worker thread.</param>
		public void ExecuteSynchronously(System.Action action)
		{
			using (var actionWaitHandle = new ManualResetEvent (false))
			{
				System.Exception innerException = null;

				this.ExecuteAsynchronously (action,
											onException: e => innerException = e,
											onFinish: () => actionWaitHandle.Set ());

				actionWaitHandle.WaitOne ();

				if (innerException != null)
				{
					throw new WorkerThreadException ("The operation threw an exception.", innerException);
				}
			}
		}

		/// <summary>
		/// Asynchronously executes an action on the worker thread, which means that this method
		/// will return immediately, probably before the action is executed.
		/// </summary>
		/// <param name="action">The action to execute on the worker thread.</param>
		/// <param param name="onException">An action that will be called if an exception is thrown during the execution of the action.</param>
		/// <param name="onFinish">An action that will be called once the action has been executed.</param>
		public void ExecuteAsynchronously(System.Action action, System.Action<System.Exception> onException = null, System.Action onFinish = null)
		{
			System.Action wrappedAction = () =>
			{
				try
				{
					action ();
				}
				catch (System.Exception e)
				{
					if (onException != null)
					{
						try
						{
							onException (e);
						}
						catch
						{
							// Swallow exceptions because we don't want to kill the thread if the
							// caller has given us a crappy onException action.

							var message = "Exception thrown during the execution of the 'onException' callback";

							System.Diagnostics.Debug.Fail (message);
						}
					}
				}
				finally
				{
					if (onFinish != null)
					{
						try
						{
							onFinish ();
						}
						catch
						{
							// Swallow exceptions because we don't want to kill the thread if the
							// caller has given us a crappy onFinish action.

							var message = "Exception thrown during the execution of the 'onFinish' callback";

							System.Diagnostics.Debug.Fail (message);
						}
					}
				}
			};

			this.pendingActionsQueue.Add (wrappedAction);
		}


		#region IDisposable Members

		/// <summary>
		/// Disposes the worker thread. This method will eventually forbid any new call to the
		/// ExecuteSynchronously and to the ExecuteAsynchronously methods. It will then wait for all
		/// the pending actions to be executed and finally dispose the worker thread.
		/// </summary>
		public void Dispose()
		{
			System.Action action = () =>
			{
				this.isDisposing = true;
			};

			try
			{
				this.ExecuteAsynchronously (action);
				this.pendingActionsQueue.CompleteAdding ();

				this.thread.Join ();
				this.pendingActionsQueue.Dispose ();
			}
			catch (System.ObjectDisposedException)
			{
				// Swallow the exception if we have already disposed the queue. That means that this
				// is the second time that we are executing this method and there is no reason to
				// execute this stuff a second time.
			}
			catch (System.InvalidOperationException)
			{
				// Swallow the exception if the queue does not accept new items to be added. That
				// means that this is the second time that we are executing this method and there is
				// no reason to execute this stuff a second time.
			}
		}

		#endregion


		/// <summary>
		/// The method that is executed by the worker thread.
		/// </summary>
		private void DoWork()
		{
			System.Action action;

			// Executes the actions until the Dispose method has been called.
			while (!this.isDisposing)
			{
				action = this.pendingActionsQueue.Take ();

				action ();
			}

			// Once the Dispose method has been called, execute any pending actions and terminate.
			while (this.pendingActionsQueue.TryTake (out action))
			{
				action ();
			}
		}


		/// <summary>
		/// The worker thread that will execute all actions given to this instance.
		/// </summary>
		private readonly Thread					thread;
		private readonly PendingActionsQueue	pendingActionsQueue;
		
		private volatile bool					isDisposing;
	}
}
