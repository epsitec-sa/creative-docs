namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Timer impl�mente les services n�cessaires � la r�alisation
	/// d'un timer compatible avec l'interface graphique.
	/// </summary>
	public class Timer : System.IDisposable
	{
		public Timer()
		{
		}
		
		
		public double							Delay
		{
			get
			{
				return this.delay_seconds;
			}
			set
			{
				//	Change le d�lai. Le temps de r�f�rence est soit le moment o� le
				//	timer est d�marr� pour la premi�re fois, soit maintenant si le
				//	timer est d�j� d�marr�.
				
				this.delay_seconds   = value;
				this.expiration_date = System.DateTime.Now.AddSeconds (value);
				this.remaining_time  = this.expiration_date.Subtract (System.DateTime.Now);
				
				this.UpdateTimerSettings ();
			}
		}
		
		public System.DateTime					ExpirationDate
		{
			get
			{
				return this.expiration_date;
			}
			set
			{
				if (this.expiration_date != value)
				{
					this.expiration_date = value;
					this.delay_seconds   = 0;
					this.UpdateTimerSettings ();
				}
			}
		}
		
		public TimerState						State
		{
			get { return this.state; }
		}
		
		public double							AutoRepeat
		{
			get { return this.auto_repeat; }
			set
			{
				if (this.auto_repeat != value)
				{
					this.auto_repeat = value;
					this.Delay = value;
				}
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
				this.CleanupTimerIfNeeded ();
				this.state = TimerState.Disposed;
				this.auto_repeat = 0;
			}
		}
		
		
		protected virtual void SetupTimerIfNeeded()
		{
			if (this.timer == null)
			{
				this.timer = new System.Windows.Forms.Timer ();
				this.timer.Tick += new System.EventHandler (this.HandleTimerTick);
			}
		}
		
		protected virtual void CleanupTimerIfNeeded()
		{
			if (this.timer != null)
			{
				this.timer.Stop ();
				this.timer.Tick -= new System.EventHandler (this.HandleTimerTick);
				this.timer.Dispose ();
				this.timer = null;
			}
		}
		
		protected virtual void UpdateTimerSettings()
		{
			switch (this.state)
			{
				case TimerState.Running:
				case TimerState.Suspended:
					this.timer.Stop ();
					
					System.DateTime now  = System.DateTime.Now;
					System.TimeSpan wait = this.expiration_date.Subtract (now);
					
					int delta = (int) wait.TotalMilliseconds;
					
					if (delta > 0)
					{
						this.timer.Interval = delta;
						this.timer.Start ();
					}
					else
					{
						//	On arrive trop tard (on a manqu� le moment o� le timer expirait) et
						//	on g�n�re donc manuellement l'�v�nement.
						
						this.OnTimeElapsed ();
					}
					break;
			}
		}
		
		
		public virtual void Start()
		{
			//	D�marre le timer s'il �tait arr�t�. Un timer suspendu reprend l� o�
			//	il en �tait.
			
			switch (this.state)
			{
				case TimerState.Invalid:
				case TimerState.Elapsed:
					
					//	Le timer n'a jamais servi, ou alors, le timer a d�j� atteint la
					//	fin de la p�riode de comptage.
					
					this.state = TimerState.Stopped;
					break;
				
				case TimerState.Stopped:
					break;
				
				case TimerState.Running:
					
					//	Le timer tourne, on n'a pas besoin de faire quoi que ce soit.
					
					return;
				
				case TimerState.Disposed:
					throw new System.InvalidOperationException ("Timer has been disposed");
				
				case TimerState.Suspended:
					
					//	Le timer est actuellement arr�t�. Il suffit de mettre � jour la
					//	date de fin et de le relancer.
					
					this.expiration_date = System.DateTime.Now.Add (this.remaining_time);
					break;
			}
			
			this.SetupTimerIfNeeded ();
			
			if (this.state == TimerState.Stopped)
			{
				//	Si le d�lai en secondes est sp�cif�, alors on l'utilise pour r�initialiser
				//	la date d'expiration. Utile si on a utilis� la propri�t� Delay pour d�finir
				//	le d�lai, puis fait un Start plus tard.
				
				if (this.delay_seconds > 0)
				{
					this.expiration_date = System.DateTime.Now.AddSeconds (this.delay_seconds);
				}
			}
			
			this.state = TimerState.Running;
			
			this.UpdateTimerSettings ();
		}
		
		public virtual void Suspend()
		{
			//	Suspend le timer (le temps restant est conserv� jusqu'au prochain d�marrage
			//	du timer).
			
			if (this.state == TimerState.Disposed)
			{
				throw new System.InvalidOperationException ("Timer has been disposed");
			}
			
			if (this.state == TimerState.Running)
			{
				this.timer.Stop ();
				this.remaining_time = this.expiration_date.Subtract (System.DateTime.Now);
				this.state = TimerState.Suspended;
			}
		}
		
		public virtual void Stop()
		{
			//	Arr�te le timer. Ceci va aussi lib�rer les ressources associ�es
			//	au timer interne.
			
			if (this.state == TimerState.Disposed)
			{
				throw new System.InvalidOperationException ("Timer has been disposed");
			}
			
			this.CleanupTimerIfNeeded ();
			
			this.state = TimerState.Stopped;
		}
		
		
		protected virtual void HandleTimerTick(object sender, System.EventArgs e)
		{
			this.timer.Stop ();
			this.OnTimeElapsed ();
		}
		
		protected virtual void OnTimeElapsed()
		{
			System.Diagnostics.Debug.Assert (this.state != TimerState.Disposed);
			System.Diagnostics.Debug.Assert (this.state != TimerState.Invalid);
			System.Diagnostics.Debug.Assert (this.state != TimerState.Elapsed);
			System.Diagnostics.Debug.Assert (this.state != TimerState.Suspended);
			
			this.state = TimerState.Elapsed;
			
			if (this.TimeElapsed != null)
			{
				this.TimeElapsed (this);
			}
			
			if (this.auto_repeat > 0)
			{
				this.ExpirationDate = this.ExpirationDate.AddSeconds (this.auto_repeat);
				this.Start ();
			}
		}
		
		
		
		public event Support.EventHandler		TimeElapsed;
		
		
		protected System.Windows.Forms.Timer	timer;
		protected TimerState					state;
		protected System.DateTime				expiration_date;
		protected double						delay_seconds;
		protected double						auto_repeat;
		protected System.TimeSpan				remaining_time;
	}
	
	public enum TimerState
	{
		Invalid,
		Disposed,
		Stopped,
		Running,
		Suspended,
		Elapsed
	}
}
