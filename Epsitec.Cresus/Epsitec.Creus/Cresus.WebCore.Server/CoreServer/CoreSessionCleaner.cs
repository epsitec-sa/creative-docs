using System;

using System.Timers;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{


	internal sealed class CoreSessionCleaner : IDisposable
	{


		public CoreSessionCleaner(CoreSessionManager manager, TimeSpan interval)
		{
			this.manager = manager;
			
			this.timer = new Timer ()
			{
				AutoReset = false,
				Interval = interval.TotalMilliseconds
			};

			this.timer.Elapsed += (s, e) => this.HandleElapsed (s, e);

			this.timer.Start ();
		}


		public void Dispose()
		{
			this.timer.Stop ();
			this.timer.Dispose ();
		}


		public void HandleElapsed(object sender, ElapsedEventArgs e)
		{
			this.manager.CleanUpSessions ();

			this.timer.Start ();
		}


		private readonly CoreSessionManager manager;


		private readonly Timer timer;


	}


}
