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
            this.period = period;
            this.autoRepeat = false;
            this.remainingTime = System.TimeSpan.Zero;
            this.SetState(TimerState.Stopped);
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
            get { return this.isDisposed; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.TerminateTimerTask();
            this.isDisposed = true;
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
                    this.SetState(TimerState.Running);
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
                    if (this.remainingTime.TotalMilliseconds < 1.0)
                    {
                        // the timer has a resolution of 1 ms
                        // if the remaining time is less than this, we fire the event directly
                        this.remainingTime = System.TimeSpan.Zero;
                        Timer.AddToPendingQueue(this);
                    }
                    break;
                case TimerState.Stopped:
                    // suspending when the timer is stopped does nothing
                    return;
            }
            this.SetState(TimerState.Suspended);
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
            this.SetState(TimerState.Stopped);
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

        private void SetState(TimerState newState)
        {
            //System.Console.WriteLine($"new timer state: {newState}");
            this.state = newState;
        }

        private async Task StartAsyncTimer()
        {
            //System.Console.WriteLine("start async timer");
            this.cancelTokenSource = new CancellationTokenSource();
            if (this.remainingTime != System.TimeSpan.Zero)
            {
                //System.Console.WriteLine(
                //    $"wait for remaining time {this.remainingTime.TotalMilliseconds}ms"
                //);
                this.expirationDate = System.DateTime.Now.Add(this.remainingTime);
                try
                {
                    await this.QueueEventAfterDelay(this.cancelTokenSource.Token);
                }
                catch (System.OperationCanceledException)
                {
                    //System.Console.WriteLine("timer canceled");
                    // ignore when canceled
                    return;
                }

                this.remainingTime = System.TimeSpan.Zero;
                if (!this.autoRepeat)
                {
                    //System.Console.WriteLine($"no autorepeat -> done");
                    this.SetState(TimerState.Stopped);
                    return;
                }
            }
            this.expirationDate = System.DateTime.Now;
            do
            {
                //System.Console.WriteLine($"wait for next tick");
                this.expirationDate = this.expirationDate.Add(this.period);
                try
                {
                    await this.QueueEventAfterDelay(this.cancelTokenSource.Token);
                }
                catch (System.OperationCanceledException)
                {
                    //System.Console.WriteLine("timer canceled");
                    // ignore when canceled
                    return;
                }
            } while (this.autoRepeat);
            this.SetState(TimerState.Stopped);
            //System.Console.WriteLine("timer done");
        }

        private async Task QueueEventAfterDelay(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            System.TimeSpan duration = this.expirationDate.Subtract(System.DateTime.Now);
            int durationMS = (int)duration.TotalMilliseconds;
            if (durationMS > 0)
            {
                await Task.Delay(durationMS, cancelToken);
                cancelToken.ThrowIfCancellationRequested();
            }
            Timer.AddToPendingQueue(this);
        }

        private void TerminateTimerTask()
        {
            if (this.cancelTokenSource != null)
            {
                this.cancelTokenSource.Cancel();
            }
            if (this.timerTask != null)
            {
                this.timerTask.Wait();
                this.timerTask.Dispose();
                this.timerTask = null;
            }
            if (this.cancelTokenSource != null)
            {
                this.cancelTokenSource.Dispose();
                this.cancelTokenSource = null;
            }
        }

        private void FireTimerEvent()
        {
            this.TimeElapsed.Raise(this);
        }

        private void RequireNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new System.ObjectDisposedException(GetType().FullName);
            }
        }

        private static void AddToPendingQueue(Timer timer)
        {
            Timer.pendingTimersQueue.Enqueue(timer);
            if (Timer.PendingTimers != null)
            {
                Timer.PendingTimers(null);
            }
        }
        #endregion

        public static event EventHandler PendingTimers;

        private static ConcurrentQueue<Timer> pendingTimersQueue = new ConcurrentQueue<Timer>();

        public event EventHandler TimeElapsed;

        private CancellationTokenSource cancelTokenSource;
        private Task timerTask;

        private TimerState state;
        private System.DateTime expirationDate;
        private System.TimeSpan remainingTime;
        private System.TimeSpan period;
        private bool autoRepeat;

        private bool isDisposed;
    }
}
