//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Timer implémente les services nécessaires à la réalisation
	/// d'un timer compatible avec l'interface graphique.
	/// </summary>
	public sealed class Timer : System.IDisposable, IIsDisposed
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
				//	Change le délai. Le temps de référence est soit le moment où le
				//	timer est démarré pour la première fois, soit maintenant si le
				//	timer est déjà démarré.
				
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


		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return this.state == TimerState.Disposed;
			}
		}

		#endregion
		
		#region IDisposable Members

		public void Dispose()
		{
			this.CleanupTimerIfNeeded ();
			this.state = TimerState.Disposed;
			this.delaySecondsAutoRepeat = 0;
		}
	
		#endregion


		private void SetupTimerIfNeeded()
		{
			if (this.timer == null)
			{
				this.timer = new System.Windows.Forms.Timer ();
				this.timer.Tick += this.HandleTimerTick;
			}
		}

		private void CleanupTimerIfNeeded()
		{
			var timer = this.timer;
			
			//	Work on a copy of the timer variable, since the internal field
			//	could change inexpectedly (this has been observed by YR).
			
			this.timer = null;
			
			if (timer != null)
			{
				timer.Stop ();
				timer.Tick -= this.HandleTimerTick;
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
						//	Si l'exactitude temporelle des événements n'importe pas trop, il
						//	vaut mieux tricher ici et prendre un peu de retard, mais au moins
						//	passer par la boucle des événements...

						delta = 1;
						this.expirationDate = now.AddMilliseconds (delta);
					}
					
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
		
		
		public void Start()
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
					
					this.expirationDate = System.DateTime.Now.Add (this.remainingTime);
					break;
			}
			
			this.SetupTimerIfNeeded ();
			
			if (this.state == TimerState.Stopped)
			{
				//	Si le délai en secondes est spécifé, alors on l'utilise pour réinitialiser
				//	la date d'expiration. Utile si on a utilisé la propriété Delay pour définir
				//	le délai, puis fait un Start plus tard.
				
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
			//	Suspend le timer (le temps restant est conservé jusqu'au prochain démarrage
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
			//	Arrête le timer. Ceci va aussi libérer les ressources associées
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
			if (this.notifyingTimeElapsed)
			{
				this.notifyTimeElapsedMissed = true;
				return;
			}

			do
			{
				this.notifyTimeElapsedMissed = false;

				try
				{
					this.notifyingTimeElapsed = true;

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
				finally
				{
					this.notifyingTimeElapsed = false;
				}
			}
			while (this.notifyTimeElapsedMissed);
		}
		
		
		
		public event Support.EventHandler		TimeElapsed;


		private System.Windows.Forms.Timer		timer;
		private TimerState						state;
		private System.DateTime					expirationDate;
		private System.TimeSpan					remainingTime;
		private double							delaySeconds;
		private double							delaySecondsAutoRepeat;
		private bool							higherAccuracy;
		private bool							notifyingTimeElapsed;
		private bool							notifyTimeElapsedMissed;
	}
}
