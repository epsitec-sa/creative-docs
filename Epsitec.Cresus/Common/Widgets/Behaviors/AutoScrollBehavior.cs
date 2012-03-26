//	Copyright © 2009-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

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

			this.autoScrollTimer = new Timer ();
			this.autoScrollTimer.TimeElapsed += this.HandleAutoScrollTimeElapsed;

			this.callback = callback;

			this.InitialDelay = this.owner == null ? SystemInformation.InitialKeyboardDelay : this.owner.AutoEngageDelay;
			this.RepeatPeriod = this.owner == null ? SystemInformation.KeyboardRepeatPeriod : this.owner.AutoEngageRepeatPeriod;
		}

		public AutoScrollBehavior(System.Action<Point> callback)
			: this (null, callback)
		{
		}


		public double							InitialDelay
		{
			get;
			set;
		}

		public double							RepeatPeriod
		{
			get;
			set;
		}


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
				this.scrollWindow    = (message == null || message.WindowRoot == null) ? null : message.WindowRoot.Window;
				
				if (this.autoScrollTimer.State == TimerState.Stopped)
				{
					this.InvokeCallback ();
					this.AutoScrollTimerStart (Phase.InitialDelay);
				}
			}
			else
			{
				this.AutoScrollTimerStop ();

				this.scrollMagnitude = scrollMagnitude;
				this.scrollWindow    = null;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.autoScrollTimer.State != TimerState.Disposed)
			{
				this.autoScrollTimer.TimeElapsed -= this.HandleAutoScrollTimeElapsed;
				this.autoScrollTimer.Dispose ();
			}
		}

		#endregion

		private void HandleAutoScrollTimeElapsed(object sender)
		{
			this.AutoScrollTimerStart (Phase.RepeatPeriod);
			this.InvokeCallback ();
			this.DispatchDummyMouseMoveEvent ();
		}

		private void AutoScrollTimerStart(Phase phase)
		{
			this.autoScrollTimer.Stop ();

			switch (phase)
			{
				case Phase.InitialDelay:
					this.autoScrollTimer.AutoRepeat = this.InitialDelay;
					break;
				
				case Phase.RepeatPeriod:
					this.autoScrollTimer.AutoRepeat = this.RepeatPeriod;
					break;
			}

			this.autoScrollTimer.Start ();
		}

		private void AutoScrollTimerStop()
		{
			this.autoScrollTimer.Stop ();
		}

		private void InvokeCallback()
		{
			if ((this.callback != null) &&
				(this.scrollMagnitude != Point.Zero))
			{
				this.callback (this.scrollMagnitude);
			}
		}
		
		private void DispatchDummyMouseMoveEvent()
		{
			if (this.owner != null)
			{
				Window window = this.owner.Window;

				if (window != null)
				{
					window.DispatchMessage (Message.CreateDummyMouseMoveEvent ());
				}
			}
		}

		enum Phase
		{
			InitialDelay,
			RepeatPeriod
		}

		private readonly Widget					owner;
		private readonly System.Action<Point>	callback;
		private readonly Timer					autoScrollTimer;
		private Point							scrollMagnitude;
		private Window							scrollWindow;
	}
}
