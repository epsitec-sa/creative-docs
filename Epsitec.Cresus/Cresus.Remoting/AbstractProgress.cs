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
			this.progress_percent = -1;
			this.start_time       = System.DateTime.Now;
		}
		
		
		#region IProgressInformation Members
		public virtual int						ProgressPercent
		{
			get
			{
				return this.progress_percent;
			}
		}
		
		public bool								HasFinished
		{
			get
			{
				return this.ProgressPercent == 100;
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
				return new System.TimeSpan (0, 0, 0, 0, -1);
			}
		}
		
		
		public virtual bool WaitForProgress(int minimum_progress, System.TimeSpan timeout)
		{
			if (this.ProgressPercent >= minimum_progress)
			{
				return true;
			}
			
			System.Threading.WaitHandle wait = this.GetProgressEvent (true);
			
			bool infinite = (timeout.Ticks < 0);
			
			System.DateTime start_time = System.DateTime.Now;
			System.DateTime stop_time  = start_time.Add (timeout);
			
			for (;;)
			{
				bool got_event = false;
				
				if (infinite)
				{
					got_event = wait.WaitOne ();
				}
				else
				{
					timeout = stop_time - System.DateTime.Now;
					
					if (timeout.Ticks > 0)
					{
						got_event = wait.WaitOne (timeout, false);
					}
				}
				
				if (got_event == false)
				{
					break;
				}
				
				if (this.ProgressPercent >= minimum_progress)
				{
					return true;
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
				if (this.wait_progress_event != null)
				{
					this.wait_progress_event.Close ();
					this.wait_progress_event = null;
				}
			}
		}
		
		protected System.Threading.AutoResetEvent GetProgressEvent(bool create)
		{
			lock (this)
			{
				if ((this.wait_progress_event == null) &&
					(create))
				{
					this.wait_progress_event = new System.Threading.AutoResetEvent (false);
				}
				
				return this.wait_progress_event;
			}
		}
		
		
		protected virtual void SetProgress(int progress)
		{
			if (this.progress_percent == progress)
			{
				return;
			}
			
			lock (this)
			{
				this.progress_percent = progress;
				
				if (this.wait_progress_event != null)
				{
					this.wait_progress_event.Set ();
				}
			}
		}
		
		
		private System.Threading.AutoResetEvent	wait_progress_event;
		private System.DateTime					start_time;
		private volatile int					progress_percent;
	}
}
