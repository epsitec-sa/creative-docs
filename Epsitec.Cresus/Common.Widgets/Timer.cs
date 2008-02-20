//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Timer impl�mente les services n�cessaires � la r�alisation
	/// d'un timer compatible avec l'interface graphique.
	/// </summary>
	public sealed class Timer : System.IDisposable
	{
		public Timer()
		{
		}
		
		
		public bool								HigherAccuracy
		{
			get
			{
				return this.higherAccuracy;
			}
			set
			{
				this.higherAccuracy = value;
			}
		}
		
		public double							Delay
		{
			get
			{
				return this.delaySeconds;
			}
			set
			{
				//	Change le d�lai. Le temps de r�f�rence est soit le moment o� le
				//	timer est d�marr� pour la premi�re fois, soit maintenant si le
				//	timer est d�j� d�marr�.
				
				this.delaySeconds   = value;
				this.expirationDate = System.DateTime.Now.AddSeconds (value);
				this.remainingTime  = this.expirationDate.Subtract (System.DateTime.Now);
				
				this.UpdateTimerSettings ();
			}
		}
		
		public System.DateTime					ExpirationDate
		{
			get
			{
				return this.expirationDate;
			}
			set
			{
				if (this.expirationDate != value)
				{
					this.expirationDate = value;
					this.delaySeconds   = 0;
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
			get { return this.delaySecondsAutoRepeat; }
			set
			{
				if (this.delaySecondsAutoRepeat != value)
				{
					this.delaySecondsAutoRepeat = value;
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

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.CleanupTimerIfNeeded ();
				this.state = TimerState.Disposed;
				this.delaySecondsAutoRepeat = 0;
			}
		}


		private void SetupTimerIfNeeded()
		{
			if (this.timer == null)
			{
				this.timer = new System.Windows.Forms.Timer ();
				this.timer.Tick += new System.EventHandler (this.HandleTimerTick);
			}
		}

		private void CleanupTimerIfNeeded()
		{
			System.Windows.Forms.Timer timer = this.timer;
			
			//	Work on a copy of the timer variable, since the internal field
			//	could change inexpectedly (this has been observed by YR).
			
			this.timer = null;
			
			if (timer != null)
			{
				timer.Stop ();
				timer.Tick -= new System.EventHandler (this.HandleTimerTick);
				timer.Dispose ();
			}
		}

		private void UpdateTimerSettings()
		{
			switch (this.state)
			{
				case TimerState.Running:
				case TimerState.Suspended:
					this.timer.Stop ();
					
					System.DateTime now  = System.DateTime.Now;
					System.TimeSpan wait = this.expirationDate.Subtract (now);
					
					int delta = (int) wait.TotalMilliseconds;
					
					if ((this.higherAccuracy == false) &&
						(delta <= 0))
					{
						//	Si l'exactitude temporelle des �v�nements n'importe pas trop, il
						//	vaut mieux tricher ici et prendre un peu de retard, mais au moins
						//	passer par la boucle des �v�nements...
						
						delta = 1;
					}
					
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
		
		
		public void Start()
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
					
					this.expirationDate = System.DateTime.Now.Add (this.remainingTime);
					break;
			}
			
			this.SetupTimerIfNeeded ();
			
			if (this.state == TimerState.Stopped)
			{
				//	Si le d�lai en secondes est sp�cif�, alors on l'utilise pour r�initialiser
				//	la date d'expiration. Utile si on a utilis� la propri�t� Delay pour d�finir
				//	le d�lai, puis fait un Start plus tard.
				
				if (this.delaySeconds > 0)
				{
					this.expirationDate = System.DateTime.Now.AddSeconds (this.delaySeconds);
				}
			}
			
			this.state = TimerState.Running;
			
			this.UpdateTimerSettings ();
		}
		
		public void Suspend()
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
				this.remainingTime = this.expirationDate.Subtract (System.DateTime.Now);
				this.state = TimerState.Suspended;
			}
		}
		
		public void Stop()
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

		public void Restart()
		{
			this.Stop ();
			this.Start ();
		}

		private void HandleTimerTick(object sender, System.EventArgs e)
		{
			this.timer.Stop ();
			this.OnTimeElapsed ();
		}

		private void OnTimeElapsed()
		{
			switch (this.state)
			{
				case TimerState.Disposed:
					return;
				
				case TimerState.Invalid:
				case TimerState.Elapsed:
				case TimerState.Suspended:
					throw new System.InvalidOperationException (string.Format ("Timer got event while in {0} state.", this.state));
				
				case TimerState.Stopped:
				case TimerState.Running:
					this.state = TimerState.Elapsed;
					break;
			}
			
			this.state = TimerState.Elapsed;
			
			if (this.TimeElapsed != null)
			{
				this.TimeElapsed (this);
			}
			
			if ((this.delaySecondsAutoRepeat > 0) &&
				(this.state == TimerState.Elapsed))
			{
				this.ExpirationDate = this.ExpirationDate.AddSeconds (this.delaySecondsAutoRepeat);
				this.Start ();
			}
		}
		
		
		
		public event Support.EventHandler		TimeElapsed;


		private System.Windows.Forms.Timer		timer;
		private TimerState						state;
		private System.DateTime					expirationDate;
		private System.TimeSpan					remainingTime;
		private double							delaySeconds;
		private double							delaySecondsAutoRepeat;
		private bool							higherAccuracy;
	}
}
