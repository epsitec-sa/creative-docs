using System;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{


	public sealed class SafeCoreSession : IDisposable
	{


		public SafeCoreSession(string id)
		{
			this.worker = new Worker ();

			try
			{
				CoreSession coreSession = null;

				this.worker.Execute (() => coreSession = new CoreSession (id));

				this.coreSession = coreSession;
			}
			catch
			{
				this.worker.Dispose ();

				throw;
			}
		}


		public string Id
		{
			get
			{
				return this.coreSession.Id;
			}
		}


		public T Execute<T>(Func<CoreSession, T> function)
		{
			// NOTE: When calling this function, you MUST ensure that the function does not return
			// anything involving something whose execution is delayed, such as a LINQ query. Failure
			// to comply with this will be met will NASTY bug generation. And I mean, REAL nasty...

			T value = default (T);

			this.worker.Execute (() => value = function (this.coreSession));

			return value;
		}


		public void Dispose()
		{
			this.worker.Dispose ();
			this.coreSession.Dispose ();
		}


		private readonly Worker worker;


		private readonly CoreSession coreSession;


	}


}
