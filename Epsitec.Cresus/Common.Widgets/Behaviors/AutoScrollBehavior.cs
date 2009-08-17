//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets.Behaviors
{
	public sealed class AutoScrollBehavior
	{
		public AutoScrollBehavior(Widget owner, System.Action<Point> callback)
		{
			this.owner = owner;

			this.autoScrollTimer = new Timer ();
			this.autoScrollTimer.AutoRepeat = this.owner.AutoEngageDelay;
			this.autoScrollTimer.TimeElapsed += this.HandleAutoScrollTimeElapsed;

			this.callback = callback;
		}


		public void ProcessEvent(Point scrollMagnitude)
		{
			if (scrollMagnitude != Point.Zero)
			{
				this.scrollMagnitude = scrollMagnitude;
				
				if (this.autoScrollTimer.State == TimerState.Stopped)
				{
					this.InvokeCallback ();
					this.AutoScrollTimerStart ();
				}
			}
			else
			{
				this.AutoScrollTimerStop ();
			}
		}

		
		private void AutoScrollTimerStart()
		{
			this.autoScrollTimer.Stop ();
			this.autoScrollTimer.AutoRepeat = this.owner.AutoEngageDelay;
			this.autoScrollTimer.Start ();
		}

		private void AutoScrollTimerStop()
		{
			this.autoScrollTimer.Stop ();
		}

		
		private void HandleAutoScrollTimeElapsed(object sender)
		{
			this.autoScrollTimer.AutoRepeat = this.owner.AutoEngageRepeatPeriod;
			this.InvokeCallback ();
			this.DispatchDummyMouseMoveEvent ();
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
			Window window = this.owner.Window;
			
			if (window != null)
			{
				window.DispatchMessage (Message.CreateDummyMouseMoveEvent ());
			}
		}

		private readonly Widget					owner;
		private readonly System.Action<Point>	callback;
		private readonly Timer					autoScrollTimer;
		private Point scrollMagnitude;
	}
}
