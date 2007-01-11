using System.Collections.Generic;
using System.Threading;

namespace Epsitec.Common.IO
{
	public class WaitCondition
	{
		public WaitCondition()
		{
		}

		public bool Wait(ConditionPredicate condition, int timeout)
		{
			if (condition ())
			{
				return true;
			}

			int endTicks = System.Environment.TickCount + timeout;
			AutoResetEvent wait = new AutoResetEvent (false);

			try
			{
				lock (this.waits)
				{
					this.waits.Add (wait);
				}

				if (condition ())
				{
					return true;
				}
				
				while (true)
				{
					if (timeout == -1)
					{
						wait.WaitOne ();
					}
					else
					{
						int nowTicks = System.Environment.TickCount;
						int milliseconds = endTicks - nowTicks;

						if (milliseconds <= 0)
						{
							return condition ();
						}
						
						wait.WaitOne (milliseconds, false);
					}
					
					if (condition ())
					{
						return true;
					}
				}
			}
			finally
			{
				lock (this.waits)
				{
					this.waits.Remove (wait);
				}
			}
		}

		public void Signal()
		{
			lock (this.waits)
			{
				foreach (AutoResetEvent wait in this.waits)
				{
					wait.Set ();
				}
			}
		}

		public delegate bool ConditionPredicate();

		private List<AutoResetEvent> waits = new List<AutoResetEvent> ();
	}
}
