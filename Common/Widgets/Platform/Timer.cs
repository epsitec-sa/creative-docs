//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Concurrent;
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
            if (period == System.TimeSpan.Zero)
            {
                throw new System.ArgumentException("Timer period should not be zero.");
            }
            this.timer = new PeriodicTimer(period);
            this.period = period;
            this.autoRepeat = false;
            this.remainingTime = System.TimeSpan.Zero;
            this.state = TimerState.Stopped;
        }

        ~Timer()
        {
            this.Dispose();
        }

        #region public thread-safe api

        public double Period
        {
            get
            {
                RequireNotDisposed();
                return this.period.TotalSeconds;
            }
            set
            {
                RequireNotDisposed();
                if (value <= 0)
                {
                    throw new System.ArgumentException(
                        "Timer Period should be a strictly positive number."
                    );
                }
                if (state != TimerState.Stopped)
                {
                    throw new System.InvalidOperationException(
                        "The timer should be stopped to set the delay"
                    );
                }
                this.period = System.TimeSpan.FromSeconds(value);
            }
        }

        public TimerState State
        {
            get
            {
                RequireNotDisposed();
                return this.state;
            }
        }

        public bool AutoRepeat
        {
            get
            {
                RequireNotDisposed();
                return this.autoRepeat;
            }
            set
            {
                RequireNotDisposed();
                this.autoRepeat = value;
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
                case TimerState.Stopped:
                    this.state = TimerState.Running;
                    this.timerTask = this.StartAsyncTimer();
                    break;
            }
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
                    this.remainingTime = this.expirationDate.Subtract(System.DateTime.Now);
                    this.TerminateTimerTask();
                    System.Console.WriteLine(
                        $"remaining milliseconds {this.remainingTime.TotalMilliseconds}"
                    );
                    if (this.remainingTime.TotalMilliseconds < 1.0)
                    {
                        // the timer has a resolution of 1 ms
                        // if the remaining time is less than this, we fire the event directly
                        this.remainingTime = System.TimeSpan.Zero;
                        Timer.AddToPendingQueue(this);
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
                    this.TerminateTimerTask();
                    break;
                case TimerState.Suspended:
                    this.remainingTime = System.TimeSpan.Zero;
                    break;
            }
            this.state = TimerState.Stopped;
        }

        public static void FirePendingEvents()
        {
            while (Timer.pendingTimersQueue.TryDequeue(out var pendingTimer))
            {
                pendingTimer.FireTimerEvent();
            }
        }
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
                    System.Console.WriteLine($"fire event");
                    Timer.AddToPendingQueue(this);

                    this.remainingTime = System.TimeSpan.Zero;
                    if (!this.autoRepeat)
                    {
                        System.Console.WriteLine($"no autorepeat -> done");
                        this.state = TimerState.Stopped;
                        return;
                    }
                }
                this.timer.Period = this.period;
                do
                {
                    this.expirationDate = System.DateTime.Now.Add(this.period);
                    System.Console.WriteLine($"wait for next tick");
                    await this.timer.WaitForNextTickAsync(this.cancelTokenSource.Token);
                    System.Console.WriteLine($"fire event");
                    Timer.AddToPendingQueue(this);
                } while (this.autoRepeat);
            }
            catch (System.OperationCanceledException)
            {
                System.Console.WriteLine("timer canceled");
                // ignore when canceled
                return;
            }
            this.state = TimerState.Stopped;
            System.Console.WriteLine("timer done");
        }

        private void TerminateTimerTask()
        {
            this.cancelTokenSource.Cancel();
            this.timerTask.Wait();
        }

        private void FireTimerEvent()
        {
            this.TimeElapsed.Raise(this);
        }

        private void RequireNotDisposed()
        {
            if (this.timer == null)
            {
                throw new System.ObjectDisposedException(GetType().FullName);
            }
        }

        private static void AddToPendingQueue(Timer timer)
        {
            Timer.pendingTimersQueue.Enqueue(timer);
        }
        #endregion

        private static ConcurrentQueue<Timer> pendingTimersQueue = new ConcurrentQueue<Timer>();

        public event EventHandler TimeElapsed;

        private PeriodicTimer timer;
        private CancellationTokenSource cancelTokenSource;
        private Task timerTask;

        private TimerState state;
        private System.DateTime expirationDate;
        private System.TimeSpan remainingTime;
        private System.TimeSpan period;
        private bool autoRepeat;
    }
}
