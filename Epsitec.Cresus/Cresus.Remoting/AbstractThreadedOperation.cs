//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>AbstractThreadedOperation</c> class provides the basic support to run
	/// an operation in a separate thread.
	/// </summary>
	public abstract class AbstractThreadedOperation : AbstractOperation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractThreadedOperation"/> class.
		/// </summary>
		protected AbstractThreadedOperation()
		{
			this.waitBeforeAbortInMilliseconds = 100;
		}


		/// <summary>
		/// Gets the thread cancel event. If it does not yet exist, create it.
		/// Signaling the cancel event will cancel the threaded operation, as
		/// soon as the thread has the opportunity to do so.
		/// </summary>
		/// <value>The thread cancel event.</value>
		System.Threading.AutoResetEvent			ThreadCancelEvent
		{
			get
			{
				lock (this.exclusion)
				{
					if (this.threadCancelEvent == null)
					{
						this.threadCancelEvent = new System.Threading.AutoResetEvent (false);
					}

					return this.threadCancelEvent;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether to cancel the current thread.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the current thread should be canceled; otherwise, <c>false</c>.
		/// </value>
		protected bool							IsCancelRequested
		{
			get
			{
				lock (this.exclusion)
				{
					return this.isThreadCancelRequested;
				}
			}
		}


		/// <summary>
		/// Starts the associated thread.
		/// </summary>
		public void Start()
		{
			lock (this.exclusion)
			{
				if (this.thread != null)
				{
					new System.InvalidOperationException ("Thread cannot be started twice.");
				}

				this.thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.ProcessOperation));
				this.thread.Start ();
			}
		}

		/// <summary>
		/// Cancels the operation asynchronously.
		/// </summary>
		/// <returns>
		/// Returns the operation which is cancelling or <c>null</c> if the cancellation was immediate.
		/// </returns>
		public override AbstractOperation CancelOperationAsync()
		{
			System.Threading.Thread thread;

			lock (this.exclusion)
			{
				this.isThreadCancelRequested = true;
				thread = this.thread;
			}
			
			if ((thread != null) &&
				(thread.IsAlive))
			{
				//	The thread is currently running and executing this operation; ask it to
				//	stop execution by signalling an event :
				
				this.ThreadCancelEvent.Set ();
				
				return new ThreadJoinOperation (thread);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Does the real work for this operation; override this method to do something
		/// useful.
		/// </summary>
		protected abstract void ProcessOperation();

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.thread != null)
				{
					//	First, ask politely to cancel the operation, then be rude if this
					//	fails, by interrupting the thread.
					
					if (this.CancelOperation (this.waitBeforeAbortInMilliseconds) == false)
					{
						this.thread.Interrupt ();
					}
					
					this.thread.Join ();
					this.thread = null;
				}
			}
			
			base.Dispose (disposing);
		}
		
		
		protected readonly object				exclusion = new object ();
		
		System.Threading.Thread					thread;
		System.Threading.AutoResetEvent			threadCancelEvent;
		volatile bool							isThreadCancelRequested;
		
		protected int							waitBeforeAbortInMilliseconds;
	}
}
