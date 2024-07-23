/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.Widgets.Behaviors
{
    /// <summary>
    /// The <c>AutoScrollBehavior</c> class handles automatic scrolling after
    /// an initial delay. The user of this helper class simply states in which
    /// direction to initiate a scroll.
    /// </summary>
    public sealed class AutoScrollBehavior : System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScrollBehavior"/> class.
        /// </summary>
        /// <param name="owner">The owner widget.</param>
        /// <param name="callback">The callback which will be called when scrolling
        /// should happen; the argument specifies the magnitude of the scroll
        /// (see <see cref="ProcessEvent"/>).</param>
        public AutoScrollBehavior(Widget owner, System.Action<Point> callback)
        {
            this.owner = owner;

            this.autoScrollTimer = new Timer();
            this.autoScrollTimer.TimeElapsed += this.HandleAutoScrollTimeElapsed;

            this.callback = callback;

            this.InitialDelay =
                this.owner == null
                    ? SystemInformation.InitialKeyboardDelay
                    : this.owner.AutoEngageDelay;
            this.RepeatPeriod =
                this.owner == null
                    ? SystemInformation.KeyboardRepeatPeriod
                    : this.owner.AutoEngageRepeatPeriod;
        }

        public AutoScrollBehavior(System.Action<Point> callback)
            : this(null, callback) { }

        public double InitialDelay { get; set; }

        public double RepeatPeriod { get; set; }

        /// <summary>
        /// Processes the event.
        /// </summary>
        /// <param name="scrollMagnitude">The scroll magnitude and direction; specify
        /// <c>Point.Zero</c> to cancel any previous event and stop the automatic
        /// scrolling behavior.</param>
        public void ProcessEvent(Point scrollMagnitude, Message message = null)
        {
            if (scrollMagnitude != Point.Zero)
            {
                this.scrollMagnitude = scrollMagnitude;
                this.scrollWindow =
                    (message == null || message.WindowRoot == null)
                        ? null
                        : message.WindowRoot.Window;

                if (this.autoScrollTimer.State == TimerState.Stopped)
                {
                    this.InvokeCallback();
                    this.AutoScrollTimerStart(Phase.InitialDelay);
                }
            }
            else
            {
                this.AutoScrollTimerStop();

                this.scrollMagnitude = scrollMagnitude;
                this.scrollWindow = null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!this.autoScrollTimer.IsDisposed)
            {
                this.autoScrollTimer.TimeElapsed -= this.HandleAutoScrollTimeElapsed;
                this.autoScrollTimer.Dispose();
            }
        }

        #endregion

        private void HandleAutoScrollTimeElapsed(object sender)
        {
            this.AutoScrollTimerStart(Phase.RepeatPeriod);
            this.InvokeCallback();
            this.DispatchDummyMouseMoveEvent();
        }

        private void AutoScrollTimerStart(Phase phase)
        {
            this.autoScrollTimer.Stop();

            switch (phase)
            {
                case Phase.InitialDelay:
                    this.autoScrollTimer.Period = this.InitialDelay;
                    this.autoScrollTimer.AutoRepeat = true;
                    break;

                case Phase.RepeatPeriod:
                    this.autoScrollTimer.Period = this.RepeatPeriod;
                    this.autoScrollTimer.AutoRepeat = true;
                    break;
            }

            this.autoScrollTimer.Start();
        }

        private void AutoScrollTimerStop()
        {
            this.autoScrollTimer.Stop();
        }

        private void InvokeCallback()
        {
            if ((this.callback != null) && (this.scrollMagnitude != Point.Zero))
            {
                this.callback(this.scrollMagnitude);
            }
        }

        private void DispatchDummyMouseMoveEvent()
        {
            if (this.owner != null)
            {
                Window window = this.owner.Window;

                if (window != null)
                {
                    window.DispatchMessage(Message.CreateDummyMouseMoveEvent());
                }
            }
        }

        enum Phase
        {
            InitialDelay,
            RepeatPeriod
        }

        private readonly Widget owner;
        private readonly System.Action<Point> callback;
        private readonly Timer autoScrollTimer;
        private Point scrollMagnitude;
        private Window scrollWindow;
    }
}
