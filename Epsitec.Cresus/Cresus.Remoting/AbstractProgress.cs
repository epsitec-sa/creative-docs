//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for AbstractProgress.
	/// </summary>
	public abstract class AbstractProgress : System.MarshalByRefObject, IProgressInformation, System.IDisposable
	{
		public AbstractProgress()
		{
		}
		
		
		#region IProgressInformation Members
		public virtual int						ProgressPercent
		{
			get
			{
				return this.progress_percent;
			}
		}
		
		public virtual Remoting.ProgressStatus	ProgressStatus
		{
			get
			{
				return this.progress_status;
			}
		}
		
		public virtual int						CurrentStep
		{
			get
			{
				return this.current_step;
			}
		}
		
		public virtual int						LastStep
		{
			get
			{
				return this.last_step;
			}
		}
		
		public System.TimeSpan					RunningDuration
		{
			get
			{
				return System.DateTime.Now - this.start_time;
			}
		}
		
		public virtual System.TimeSpan			ExpectedDuration
		{
			get
			{
				return System.TimeSpan.FromMilliseconds (-1);
			}
		}
		
		public bool WaitForProgress(int minimum_progress)
		{
			return this.WaitForProgress (minimum_progress, System.TimeSpan.FromMilliseconds (-1));
		}
		
		public virtual bool WaitForProgress(int minimum_progress, System.TimeSpan timeout)
		{
			if (this.ProgressPercent >= minimum_progress)
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
				
				lock (this.monitor)
				{
					if (this.ProgressPercent >= minimum_progress)
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
			
			return (this.ProgressPercent >= minimum_progress);
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
				
				System.Threading.Monitor.PulseAll (this.monitor);
			}
		}
		
		protected virtual void SetCurrentStep(int step)
		{
			if (this.current_step == step)
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
	}
}
