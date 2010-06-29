//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>AbstractOperation</c> class implements the foundations for all
	/// operation classes, providing means to query for its progress, cancel
	/// the it, etc.
	/// </summary>
	public abstract class AbstractOperation : System.MarshalByRefObject, System.IDisposable
	{
		protected AbstractOperation()
		{
			//	Tell the OperationManager about this new operation and assign it a
			//	unique ID. This ID will be unique, even across multiple app domains,
			//	if the OperationManager was properly set up.

			this.startTime        = AbstractOperation.GetNowUtc ();
			this.progressPercent  = -1;
			this.expectedLastStep = -1;
			this.expectedDuration = System.TimeSpan.FromMilliseconds (-1);
			
			OperationManager.Register (this, operationId => this.operationId = operationId);
		}
		
		
		public override object InitializeLifetimeService()
		{
			//	By returning null, we explicitely state that we do not want to have
			//	automatic object recycling (see ILease and remoting).

			return null;
		}


		/// <summary>
		/// Gets the progress state of this operation.
		/// </summary>
		/// <value>The progress state.</value>
		public ProgressState					ProgressState
		{
			get
			{
				this.RefreshProgressInformation ();

				lock (this.monitor)
				{
					return this.progressState;
				}
			}
		}


		public long OperationId
		{
			get
			{
				return this.operationId;
			}
		}


		/// <summary>
		/// Gets the failure message.
		/// </summary>
		/// <value>The failure message or <c>null</c>.</value>
		public string							FailureMessage
		{
			get
			{
				return this.failureMessage;
			}
		}

		/// <summary>
		/// Gets the progress information for the current operation.
		/// </summary>
		/// <returns>The progress information.</returns>
		public ProgressInformation GetProgressInformation()
		{
			this.RefreshProgressInformation ();

			lock (this.monitor)
			{
				System.TimeSpan duration = (this.stopTime.Ticks == 0 ? AbstractOperation.GetNowUtc () : this.stopTime) - this.startTime;

				return new ProgressInformation (
					this.progressPercent,
					this.progressState,
					this.currentStep,
					this.expectedLastStep,
					duration,
					this.expectedDuration,
					this.operationId);
			}
		}



		/// <summary>
		/// Cancels the operation, using an infinite timeout.
		/// </summary>
		/// <returns>Returns <c>true</c> if the cancel operation was successfull.</returns>
		public bool CancelOperation()
		{
			return this.CancelOperation (-1);
		}

		/// <summary>
		/// Cancels the operation, using the specified timeout.
		/// </summary>
		/// <param name="timeout">The timeout, in milliseconds.</param>
		/// <returns>Returns <c>true</c> if the cancel operation was successfull.</returns>
		public bool CancelOperation(int timeout)
		{
			AbstractOperation op = this.CancelOperationAsync ();

			if (op == null)
			{
				return true;
			}
			else
			{
				return op.WaitForProgress (100, System.TimeSpan.FromMilliseconds (timeout));
			}
		}

		/// <summary>
		/// Cancels the operation asynchronously.
		/// </summary>
		/// <returns>Returns the operation which is cancelling or <c>null</c> if the cancellation was immediate.</returns>
		public virtual AbstractOperation CancelOperationAsync()
		{
			return null;
		}

		/// <summary>
		/// Waits for progress, using an infinite timeout.
		/// </summary>
		/// <param name="progress">The expected progress.</param>
		/// <returns>Returns <c>true</c> if the specified progress percentage was reached.</returns>
		public bool WaitForProgress(int progress)
		{
			return this.WaitForProgress (progress, System.TimeSpan.FromMilliseconds (-1));
		}

		/// <summary>
		/// Waits for progress, using the specified timeout.
		/// </summary>
		/// <param name="progress">The progress.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns>Returns <c>true</c> if the specified progress percentage was reached.</returns>
		public virtual bool WaitForProgress(int progress, System.TimeSpan timeout)
		{
			this.RefreshProgressInformation ();

			lock (this.monitor)
			{
				if (this.progressPercent >= progress)
				{
					return true;
				}
			}

			bool infinite = (timeout.Ticks < 0);

			System.DateTime startTime = AbstractOperation.GetNowUtc ();
			System.DateTime stopTime  = startTime.Add (timeout);
			
			for (;;)
			{
				bool      gotEvent = false;
				const int maxWait  = 30*1000;

				this.RefreshProgressInformation ();
				
				lock (this.monitor)
				{
					if (this.progressPercent >= progress)
					{
						return true;
					}
					if (infinite)
					{
						gotEvent = System.Threading.Monitor.Wait (this.monitor, maxWait);
					}
					else
					{
						timeout = System.TimeSpan.FromTicks (System.Math.Min (stopTime.Ticks - AbstractOperation.GetNowUtc ().Ticks, maxWait*10*1000L));

						if (timeout.Ticks > 0)
						{
							gotEvent = System.Threading.Monitor.Wait (this.monitor, timeout);
						}
					}
				}
				
				if (gotEvent == false)
				{
					break;
				}
			}

			lock (this.monitor)
			{
				return (this.progressPercent >= progress);
			}
		}

		#region IDisposable Members
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (System.Threading.Monitor.TryEnter (this.monitor, 1))
				{
					System.Threading.Monitor.PulseAll (this.monitor);
					System.Threading.Monitor.Exit (this.monitor);
				}

				long operationId = this.operationId;

				OperationManager.Unregister (this, operationId);
			}
		}


		/// <summary>
		/// Refreshes the progress information.
		/// </summary>
		protected virtual void RefreshProgressInformation()
		{
		}


		/// <summary>
		/// Sets the current progress (in percent).
		/// </summary>
		/// <param name="progress">The progress.</param>
		protected void SetProgress(int progress)
		{
			lock (this.monitor)
			{
				if (this.progressPercent == progress)
				{
					return;
				}
				
				this.progressState   = (progress < 100) ? ProgressState.Running : ProgressState.Succeeded;
				this.progressPercent = progress;
				this.stopTime        = (progress < 100) ? new System.DateTime (0) : AbstractOperation.GetNowUtc ();
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}

		/// <summary>
		/// Sets the canceled state.
		/// </summary>
		protected void SetCanceled()
		{
			lock (this.monitor)
			{
				if (this.progressState == ProgressState.Canceled)
				{
					return;
				}
				
				this.progressState   = ProgressState.Canceled;
				this.progressPercent = 100;
				this.stopTime        = AbstractOperation.GetNowUtc ();
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}

		/// <summary>
		/// Sets the failed state.
		/// </summary>
		/// <param name="message">The message.</param>
		protected void SetFailed(string message)
		{
			lock (this.monitor)
			{
				if (this.progressState == ProgressState.Failed)
				{
					this.failureMessage = message;
					return;
				}
				
				this.failureMessage  = message;
				this.progressState   = ProgressState.Failed;
				this.progressPercent = 100;
				this.stopTime        = AbstractOperation.GetNowUtc ();
				
				System.Diagnostics.Debug.WriteLine ("*** failure ***\nReason:");
				System.Diagnostics.Debug.WriteLine (message);
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}

		/// <summary>
		/// Sets the current step index.
		/// </summary>
		/// <param name="step">The step.</param>
		protected void SetCurrentStep(int step)
		{
			this.SetCurrentStep (step, -1);
		}

		/// <summary>
		/// Sets the current step index and progress.
		/// </summary>
		/// <param name="step">The step.</param>
		/// <param name="progress">The progress.</param>
		protected void SetCurrentStep(int step, int progress)
		{
			lock (this.monitor)
			{
				if (progress < 0)
				{
					progress = this.progressPercent;
				}

				if ((this.currentStep == step) &&
					(this.progressPercent == progress) &&
					((step != 0) || (this.progressState == ProgressState.Running)))
				{
					return;
				}
				
				this.currentStep     = step;
				this.progressPercent = progress;
				this.progressState   = (progress < 100) ? ProgressState.Running : ProgressState.Succeeded;
				this.stopTime        = (progress < 100) ? new System.DateTime (0) : AbstractOperation.GetNowUtc ();
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}

		/// <summary>
		/// Gets the current time, as UTC.
		/// </summary>
		/// <returns>The current time, as UTC.</returns>
		private static System.DateTime GetNowUtc()
		{
			return System.DateTime.Now.ToUniversalTime ();
		}

		/// <summary>
		/// Sets the last expected step.
		/// </summary>
		/// <param name="step">The step.</param>
		protected void SetLastExpectedStep(int step)
		{
			lock (this.monitor)
			{
				this.expectedLastStep = step;
			}
		}


		readonly object							monitor = new object ();
		long									operationId;
		
		readonly System.DateTime				startTime;
		System.DateTime							stopTime;
		int										progressPercent;
		ProgressState							progressState;
		int										currentStep;
		int										expectedLastStep;
		System.TimeSpan							expectedDuration;
		string									failureMessage;
	}
}
