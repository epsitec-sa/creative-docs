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
            this.timer = new System.Timers.Timer();
            // /!\ THREAD SAFETY Timer.Elapsed can fire and interupt anytime !
            this.timer.Elapsed += this.HandleTimerTick;
        }

        private void HandleTimerTick(object sender, System.EventArgs e)
        {
            // this is our safe event handler
            lock (this)
            {
                this.timer.Stop();
                this.OnTimeElapsed();
            }
        }

        #region public thread-safe api

        public bool HigherAccuracy
        {
            get
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    return this.higherAccuracy;
                }
            }
            set
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    this.higherAccuracy = value;
                }
            }
        }

        public double Delay
        {
            get
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    return this.delaySeconds;
                }
            }
            set
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    //	Change le délai. Le temps de référence est soit le moment où le
                    //	timer est démarré pour la première fois, soit maintenant si le
                    //	timer est déjà démarré.

                    this.SetDelay(value);
                }
            }
        }

        public System.DateTime ExpirationDate
        {
            get
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    return this.expirationDate;
                }
            }
            set
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    if (this.expirationDate != value)
                    {
                        this.expirationDate = value;
                        this.delaySeconds = 0;
                        this.UpdateTimerSettings();
                    }
                }
            }
        }

        public TimerState State
        {
            get
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    return this.state;
                }
            }
        }

        public double AutoRepeat
        {
            get
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    return this.delaySecondsAutoRepeat;
                }
            }
            set
            {
                lock (this)
                {
                    this.RequireNotDisposed();
                    if (this.delaySecondsAutoRepeat != value)
                    {
                        this.delaySecondsAutoRepeat = value;
                        this.SetDelay(value);
                    }
                }
            }
        }

        #region IIsDisposed Members

        public bool IsDisposed
        {
            get
            {
                lock (this)
                {
                    return this.state == TimerState.Disposed;
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                this.timer.Stop();
                this.timer.Elapsed -= this.HandleTimerTick;
                this.timer.Dispose();
                this.state = TimerState.Disposed;
                this.delaySecondsAutoRepeat = 0;
            }
        }

        #endregion

        public void Start()
        {
            lock (this)
            {
                this.RequireNotDisposed();
                this.UnsafeStart();
            }
        }

        public void Suspend()
        {
            lock (this)
            {
                this.RequireNotDisposed();
                this.UnsafeSuspend();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                this.RequireNotDisposed();
                this.UnsafeStop();
            }
        }

        public void Restart()
        {
            lock (this)
            {
                this.UnsafeStop();
                this.UnsafeStart();
            }
        }
        #endregion

        #region private unsafe implementation

        private void UnsafeStart()
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

                case TimerState.Suspended:

                    //	Le timer est actuellement arrêté. Il suffit de mettre à jour la
                    //	date de fin et de le relancer.

                    this.expirationDate = System.DateTime.Now.Add(this.remainingTime);
                    break;
            }

            if (this.state == TimerState.Stopped)
            {
                //	Si le délai en secondes est spécifé, alors on l'utilise pour réinitialiser
                //	la date d'expiration. Utile si on a utilisé la propriété Delay pour définir
                //	le délai, puis fait un Start plus tard.

                if (this.delaySeconds > 0)
                {
                    this.expirationDate = System.DateTime.Now.AddSeconds(this.delaySeconds);
                }
            }

            this.state = TimerState.Running;

            this.UpdateTimerSettings();
        }

        private void UnsafeStop()
        {
            this.state = TimerState.Stopped;
        }

        private void UnsafeSuspend()
        {
            //	Suspend le timer (le temps restant est conservé jusqu'au prochain démarrage
            //	du timer).

            if (this.state == TimerState.Running)
            {
                this.timer.Stop();
                this.remainingTime = this.expirationDate.Subtract(System.DateTime.Now);
                this.state = TimerState.Suspended;
            }
        }

        private void SetDelay(double delay)
        {
            this.delaySeconds = delay;
            this.expirationDate = System.DateTime.Now.AddSeconds(delay);
            this.remainingTime = this.expirationDate.Subtract(System.DateTime.Now);

            this.UpdateTimerSettings();
        }

        private void UpdateTimerSettings()
        {
            switch (this.state)
            {
                case TimerState.Running:
                case TimerState.Suspended:
                    this.timer.Stop();

                    System.DateTime now = System.DateTime.Now;
                    System.TimeSpan wait = this.expirationDate.Subtract(now);

                    int delta = (int)wait.TotalMilliseconds;

                    if ((this.higherAccuracy == false) && (delta <= 0))
                    {
                        //	Si l'exactitude temporelle des événements n'importe pas trop, il
                        //	vaut mieux tricher ici et prendre un peu de retard, mais au moins
                        //	passer par la boucle des événements...

                        delta = 1;
                        this.expirationDate = now.AddMilliseconds(delta);
                    }

                    if (delta > 0)
                    {
                        this.timer.Interval = delta;
                        this.timer.Start();
                    }
                    else
                    {
                        //	On arrive trop tard (on a manqué le moment où le timer expirait) et
                        //	on génère donc manuellement l'événement.

                        this.OnTimeElapsed();
                    }
                    break;
            }
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
                    throw new System.InvalidOperationException(
                        string.Format("Timer got event while in {0} state.", this.state)
                    );

                case TimerState.Stopped:
                case TimerState.Running:
                    this.state = TimerState.Elapsed;
                    break;
            }

            this.state = TimerState.Elapsed;

            if (this.TimeElapsed != null)
            {
                this.TimeElapsed(this);
            }

            if ((this.delaySecondsAutoRepeat > 0) && (this.state == TimerState.Elapsed))
            {
                //this.ExpirationDate = this.ExpirationDate.AddSeconds(
                //    this.delaySecondsAutoRepeat
                //);
                this.UnsafeStart();
            }
        }

        private void RequireNotDisposed()
        {
            if (this.state == TimerState.Disposed)
            {
                throw new System.ObjectDisposedException(this.GetType().FullName);
            }
        }
        #endregion

        public event Support.EventHandler TimeElapsed;

        private readonly System.Timers.Timer timer;

        private TimerState state;
        private System.DateTime expirationDate;
        private System.TimeSpan remainingTime;
        private double delaySeconds;
        private double delaySecondsAutoRepeat;
        private bool higherAccuracy;
    }
}
