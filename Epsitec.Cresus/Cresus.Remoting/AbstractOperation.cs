//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for AbstractOperation.
	/// </summary>
	public abstract class AbstractOperation : System.MarshalByRefObject, IOperation, System.IDisposable
	{
		protected AbstractOperation()
		{
			this.operationId = OperationManager.Register (this);
		}
		
		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recycl� (sinon, apr�s un temps d�fini par ILease, l'objet est retir�
			//	de la table des objets joignables par "remoting").

			return null;
		}

		public ProgressInformation GetProgressInformation()
		{
			this.RefreshProgressInformation ();

			return new ProgressInformation (
				this.progress_percent,
				this.progress_status,
				this.current_step,
				this.last_step, 
				System.DateTime.Now - this.start_time,
				this.ExpectedDuration,
				this.operationId);
		}


		public ProgressStatus ProgressStatus
		{
			get
			{
				this.RefreshProgressInformation ();
				return this.progress_status;
			}
		}

		protected virtual void RefreshProgressInformation()
		{
		}


		protected virtual System.TimeSpan ExpectedDuration
		{
			get
			{
				return System.TimeSpan.FromMilliseconds (-1);
			}
		}
		
		#region IOperation Members
		
		public void CancelOperation()
		{
			ProgressInformation progress;
			this.CancelOperation (out progress);
			this.WaitForProgress (100);
		}
		
		public virtual void CancelOperation(out ProgressInformation progress_information)
		{
			progress_information = ProgressInformation.Immediate;
		}

		public bool WaitForProgress(int minimum_progress)
		{
			return this.WaitForProgress (minimum_progress, System.TimeSpan.FromMilliseconds (-1));
		}
		
		public virtual bool WaitForProgress(int minimum_progress, System.TimeSpan timeout)
		{
			this.RefreshProgressInformation ();

			if (this.progress_percent >= minimum_progress)
			{
				return true;
			}
			
			bool infinite = (timeout.Ticks < 0);
			
			System.DateTime start_time = System.DateTime.Now;
			System.DateTime stop_time  = start_time.Add (timeout);
			
			for (;;)
			{
				bool      got_event = false;
				const int max_wait  = 30*1000;

				this.RefreshProgressInformation ();
				
				lock (this.monitor)
				{
					if (this.progress_percent >= minimum_progress)
					{
						return true;
					}
					if (infinite)
					{
						got_event = System.Threading.Monitor.Wait (this.monitor, max_wait);
					}
					else
					{
						timeout = System.TimeSpan.FromTicks (System.Math.Min (stop_time.Ticks - System.DateTime.Now.Ticks, max_wait*10*1000L));
					
						if (timeout.Ticks > 0)
						{
							got_event = System.Threading.Monitor.Wait (this.monitor, timeout);
						}
					}
				}
				
				if (got_event == false)
				{
					break;
				}
			}
			
			return (this.progress_percent >= minimum_progress);
		}
		#endregion
		
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

				if (this.progressManager != null)
				{
					this.progressManager.Unregister (this.operationId, this);
				}
			}
		}
		
		
		
		protected virtual void SetProgress(int progress)
		{
			if (this.progress_percent == progress)
			{
				return;
			}
			
			lock (this.monitor)
			{
				this.progress_status  = (progress < 100) ? Remoting.ProgressStatus.Running : Remoting.ProgressStatus.Succeeded;
				this.progress_percent = progress;
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}
		
		protected virtual void SetCancelled()
		{
			lock (this.monitor)
			{
				this.progress_status  = Remoting.ProgressStatus.Cancelled;
				this.progress_percent = 100;
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}
		
		protected virtual void SetFailed(string message)
		{
			lock (this.monitor)
			{
				this.failure_message  = message;
				this.progress_status  = Remoting.ProgressStatus.Failed;
				this.progress_percent = 100;
				
				System.Diagnostics.Debug.WriteLine ("*** failure ***\nReason:");
				System.Diagnostics.Debug.WriteLine (message);
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}
		
		protected virtual void SetCurrentStep(int step)
		{
			if ((this.current_step == step) &&
				((step != 0) || (this.progress_status == Remoting.ProgressStatus.Running)))
			{
				return;
			}
			
			lock (this.monitor)
			{
				this.current_step    = step;
				this.progress_status = (this.progress_percent < 100) ? Remoting.ProgressStatus.Running : Remoting.ProgressStatus.Succeeded;
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}
		
		protected virtual void SetLastStep(int step)
		{
			if (this.last_step == step)
			{
				return;
			}
			
			lock (this.monitor)
			{
				this.last_step = step;
			}
		}
		
		
		object									monitor          = new object ();
		System.DateTime							start_time       = System.DateTime.Now;
		volatile int							progress_percent = -1;
		volatile Remoting.ProgressStatus		progress_status  = Remoting.ProgressStatus.None;
		volatile string							failure_message  = null;
		
		protected volatile int					current_step     = 0;
		protected volatile int					last_step	     = -1;

		long									operationId;
		OperationManager							progressManager;

		internal void SetOperationId(long operationId, OperationManager progressManager)
		{
			if (this.operationId == 0)
			{
				this.operationId = operationId;
				this.progressManager = progressManager;
			}
			else
			{
				throw new System.InvalidOperationException ("Operation already associated with an ID");
			}
		}
	}
}
