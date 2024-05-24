//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Threading;
using System.Threading.Tasks;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe Timer implémente les services nécessaires à la réalisation
    /// d'un timer compatible avec l'interface graphique.
    /// </summary>
    public sealed class Timer : System.IDisposable, IIsDisposed
    {
        public Timer()
            : this(new System.TimeSpan(0, 0, 1)) { }

        public Timer(System.TimeSpan period)
        {
            this.timer = new PeriodicTimer(period);
            this.period = period;
            this.remainingTime = System.TimeSpan.Zero;
        }

        ~Timer()
        {
            this.Dispose();
        }

        #region public thread-safe api

        //public bool HigherAccuracy
        //{
        //    get
        //    {
        //        lock (this)
        //        {
        //            RequireNotDisposed();
        //            return higherAccuracy;
        //        }
        //    }
        //    set
        //    {
        //        lock (this)
        //        {
        //            RequireNotDisposed();
        //            higherAccuracy = value;
        //        }
        //    }
        //}

        public double Delay
        {
            get
            {
                lock (this)
                {
                    RequireNotDisposed();
                    //return period;
                    return 0;
                }
            }
            set
            {
                lock (this)
                {
                    RequireNotDisposed();
                    //	Change le délai. Le temps de référence est soit le moment où le
                    //	timer est démarré pour la première fois, soit maintenant si le
                    //	timer est déjà démarré.

                    //SetDelay(value);
                }
            }
        }

        //public System.DateTime ExpirationDate
        //{
        //    get
        //    {
        //        lock (this)
        //        {
        //            RequireNotDisposed();
        //            return expirationDate;
        //        }
        //    }
        //    set
        //    {
        //        lock (this)
        //        {
        //            RequireNotDisposed();
        //            if (expirationDate != value)
        //            {
        //                expirationDate = value;
        //                period = 0;
        //                UpdateTimerSettings();
        //            }
        //        }
        //    }
        //}

        public TimerState State
        {
            get
            {
                lock (this)
                {
                    RequireNotDisposed();
                    return state;
                }
            }
        }

        public double AutoRepeat
        {
            get
            {
                lock (this)
                {
                    RequireNotDisposed();
                    return delaySecondsAutoRepeat;
                }
            }
            set
            {
                lock (this)
                {
                    RequireNotDisposed();
                    if (delaySecondsAutoRepeat != value)
                    {
                        delaySecondsAutoRepeat = value;
                        //SetDelay(value);
                    }
                }
            }
        }

        #region IIsDisposed Members

        public bool IsDisposed
        {
            get { return this.timer == null; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.cancelTokenSource.Cancel();
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
            System.GC.SuppressFinalize(this);
        }

        #endregion

        public void Start()
        {
            RequireNotDisposed();
            switch (this.state)
            {
                case TimerState.Running:
                    // already running, nothing to do
                    return;
                case TimerState.Suspended:
                    this.timerTask = this.StartAsyncTimer();
                    break;
                case TimerState.Stopped:
                    this.timerTask = this.StartAsyncTimer();
                    break;
            }
            this.state = TimerState.Running;
        }

        public void Suspend()
        {
            RequireNotDisposed();
            switch (this.state)
            {
                case TimerState.Suspended:
                    // already suspended, nothing to do
                    return;
                case TimerState.Running:
                    this.cancelTokenSource.Cancel();
                    this.remainingTime = this.expirationDate.Subtract(System.DateTime.Now);
                    System.Console.WriteLine(
                        $"remaining milliseconds {this.remainingTime.TotalMilliseconds}"
                    );
                    if (this.remainingTime.TotalMilliseconds < 1.0)
                    {
                        // the timer has a resolution of 1 ms
                        // if the remaining time is less than this, we fire the event directly
                        this.remainingTime = System.TimeSpan.Zero;
                        this.TimeElapsed.Raise(this);
                    }
                    break;
                case TimerState.Stopped:
                    throw new System.InvalidOperationException("Cannot suspend a stopped timer");
            }
            this.state = TimerState.Suspended;
        }

        public void Stop()
        {
            RequireNotDisposed();
            switch (this.state)
            {
                case TimerState.Stopped:
                    // already stopped, nothing to do
                    break;
                case TimerState.Running:
                    this.cancelTokenSource.Cancel();
                    break;
                case TimerState.Suspended:
                    this.remainingTime = System.TimeSpan.Zero;
                    break;
            }
            this.state = TimerState.Stopped;
        }

        //public void Restart()
        //{
        //    UnsafeStop();
        //    UnsafeStart();
        //}
        #endregion

        #region private unsafe implementation

        private async Task StartAsyncTimer()
        {
            System.Console.WriteLine("start async timer");
            this.cancelTokenSource = new CancellationTokenSource();
            try
            {
                if (this.remainingTime != System.TimeSpan.Zero)
                {
                    this.timer.Period = this.remainingTime;
                    this.expirationDate = System.DateTime.Now.Add(this.remainingTime);
                    System.Console.WriteLine($"wait for remaining time {this.remainingTime}");
                    await this.timer.WaitForNextTickAsync(this.cancelTokenSource.Token);
                    this.TimeElapsed.Raise(this);
                    this.remainingTime = System.TimeSpan.Zero;
                }
                this.timer.Period = this.period;
                while (true)
                {
                    this.expirationDate = System.DateTime.Now.Add(this.period);
                    System.Console.WriteLine($"wait for next tick");
                    await this.timer.WaitForNextTickAsync(this.cancelTokenSource.Token);
                    this.TimeElapsed.Raise(this);
                }
            }
            catch (System.OperationCanceledException)
            {
                System.Console.WriteLine("timer canceled");
                // ignore when canceled
            }
            System.Console.WriteLine("done");
        }

        //private void UnsafeStart()
        //{
        //    //	Démarre le timer s'il était arrêté. Un timer suspendu reprend là où
        //    //	il en était.

        //    switch (state)
        //    {
        //        case TimerState.Invalid:
        //        case TimerState.Elapsed:

        //            //	Le timer n'a jamais servi, ou alors, le timer a déjà atteint la
        //            //	fin de la période de comptage.

        //            state = TimerState.Stopped;
        //            break;

        //        case TimerState.Stopped:
        //            break;

        //        case TimerState.Running:

        //            //	Le timer tourne, on n'a pas besoin de faire quoi que ce soit.

        //            return;

        //        case TimerState.Suspended:

        //            //	Le timer est actuellement arrêté. Il suffit de mettre à jour la
        //            //	date de fin et de le relancer.

        //            expirationDate = System.DateTime.Now.Add(remainingTime);
        //            break;
        //    }

        //    if (state == TimerState.Stopped)
        //    {
        //        //	Si le délai en secondes est spécifé, alors on l'utilise pour réinitialiser
        //        //	la date d'expiration. Utile si on a utilisé la propriété Delay pour définir
        //        //	le délai, puis fait un Start plus tard.

        //        if (period > 0)
        //        {
        //            expirationDate = System.DateTime.Now.AddSeconds(period);
        //        }
        //    }

        //    state = TimerState.Running;

        //    UpdateTimerSettings();
        //}

        //private void UnsafeStop()
        //{
        //    state = TimerState.Stopped;
        //}

        //private void UnsafeSuspend()
        //{
        //    //	Suspend le timer (le temps restant est conservé jusqu'au prochain démarrage
        //    //	du timer).

        //    if (state == TimerState.Running)
        //    {
        //        timer.Stop();
        //        remainingTime = expirationDate.Subtract(System.DateTime.Now);
        //        state = TimerState.Suspended;
        //    }
        //}

        //private void SetDelay(double delay)
        //{
        //    period = delay;
        //    expirationDate = System.DateTime.Now.AddSeconds(delay);
        //    remainingTime = expirationDate.Subtract(System.DateTime.Now);

        //    UpdateTimerSettings();
        //}

        //private void UpdateTimerSettings()
        //{
        //    switch (state)
        //    {
        //        case TimerState.Running:
        //        case TimerState.Suspended:
        //            timer.Stop();

        //            System.DateTime now = System.DateTime.Now;
        //            System.TimeSpan wait = expirationDate.Subtract(now);

        //            int delta = (int)wait.TotalMilliseconds;

        //            if (higherAccuracy == false && delta <= 0)
        //            {
        //                //	Si l'exactitude temporelle des événements n'importe pas trop, il
        //                //	vaut mieux tricher ici et prendre un peu de retard, mais au moins
        //                //	passer par la boucle des événements...

        //                delta = 1;
        //                expirationDate = now.AddMilliseconds(delta);
        //            }

        //            if (delta > 0)
        //            {
        //                timer.Interval = delta;
        //                timer.Start();
        //            }
        //            else
        //            {
        //                //	On arrive trop tard (on a manqué le moment où le timer expirait) et
        //                //	on génère donc manuellement l'événement.

        //                OnTimeElapsed();
        //            }
        //            break;
        //    }
        //}

        //private void OnTimeElapsed()
        //{
        //    // the system timer could fire while we are changing our timer state
        //    // if that happens, we simply ignore the event here
        //    if (state != TimerState.Running)
        //    {
        //        return;
        //    }

        //    state = TimerState.Elapsed;

        //    if (TimeElapsed != null)
        //    {
        //        TimeElapsed(this);
        //    }

        //    if (delaySecondsAutoRepeat > 0 && state == TimerState.Elapsed)
        //    {
        //        //this.ExpirationDate = this.ExpirationDate.AddSeconds(
        //        //    this.delaySecondsAutoRepeat
        //        //);
        //        UnsafeStart();
        //    }
        //}

        private void RequireNotDisposed()
        {
            if (this.timer == null)
            {
                throw new System.ObjectDisposedException(GetType().FullName);
            }
        }
        #endregion

        public event EventHandler TimeElapsed;

        private PeriodicTimer timer;
        private CancellationTokenSource cancelTokenSource;
        private Task? timerTask;

        private TimerState state;
        private System.DateTime expirationDate;
        private System.TimeSpan remainingTime;
        private System.TimeSpan period;
        private double delaySecondsAutoRepeat;
        private bool higherAccuracy;
    }
}
