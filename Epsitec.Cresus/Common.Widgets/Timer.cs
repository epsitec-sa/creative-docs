namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Timer implémente les services nécessaires à la réalisation
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
				//	Change le délai. Le temps de référence est soit le moment où le
				//	timer est démarré pour la première fois, soit maintenant si le
				//	timer est déjà démarré.
				
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
						//	On arrive trop tard (on a manqué le moment où le timer expirait) et
						//	on génère donc manuellement l'événement.
						
						this.OnTimeElapsed ();
					}
					break;
			}
		}
		
		
		public virtual void Start()
		{
			//	Démarre le timer s'il était arrêté. Un timer suspendu reprend là où
			//	il en était.
			
			switch (this.state)
			{
				case TimerState.Invalid:
				case TimerState.Elapsed:
					
					//	Le timer n'a jamais servi, ou alors, le timer a déjà atteint la
					//	fin de la période de comptage.
					
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
					
					//	Le timer est actuellement arrêté. Il suffit de mettre à jour la
					//	date de fin et de le relancer.
					
					this.expiration_date = System.DateTime.Now.Add (this.remaining_time);
					break;
			}
			
			this.SetupTimerIfNeeded ();
			
			if (this.state == TimerState.Stopped)
			{
				//	Si le délai en secondes est spécifé, alors on l'utilise pour réinitialiser
				//	la date d'expiration. Utile si on a utilisé la propriété Delay pour définir
				//	le délai, puis fait un Start plus tard.
				
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
			//	Suspend le timer (le temps restant est conservé jusqu'au prochain démarrage
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
			//	Arrête le timer. Ceci va aussi libérer les ressources associées
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
