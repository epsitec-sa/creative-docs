//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	public sealed class SafeCoreSession : System.IDisposable
	{
		public SafeCoreSession(string id)
		{
			this.worker = new Worker ();

			try
			{
				CoreSession coreSession = null;

				this.worker.ExecuteSync (() => coreSession = new CoreSession (id));

				this.coreSession = coreSession;
			}
			catch
			{
				this.worker.Dispose ();

				throw;
			}
		}


		public string							Id
		{
			get
			{
				return this.coreSession.Id;
			}
		}


		/// <summary>
		/// Executes the specified function on the work thread associated to this core
		/// session object. This ensures that the code will always get executed against
		/// the same thread.
		/// </summary>
		/// <typeparam name="T">The type of the function result value.</typeparam>
		/// <param name="function">The function.</param>
		/// <returns>The return value of the function.</returns>
		public T Execute<T>(System.Func<CoreSession, T> function)
		{
			// NOTE: When calling this function, you MUST ensure that the function does not return
			// anything involving something whose execution is delayed, such as a LINQ query. Failure
			// to comply with this will be met will NASTY bug generation. And I mean, REAL nasty...

			T value = default (T);

			this.worker.ExecuteSync (() => value = function (this.coreSession));

			return value;
		}


		public void Dispose()
		{
			this.worker.Dispose ();
			this.coreSession.Dispose ();
		}


		private readonly Worker					worker;
		private readonly CoreSession			coreSession;
	}
}
