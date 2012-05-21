using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{


	public sealed class CoreSessionManager : IDisposable
	{


		public CoreSessionManager(int maxNbSessions, TimeSpan sessionTimeout)
		{
			this.sessionLock = new object ();
			this.maxNbSessions = maxNbSessions;
			this.sessionTimeout = sessionTimeout;

			this.sessions = new Dictionary<string, SafeCoreSession> ();
			this.sessionLastAccessTimes = new Dictionary<string, DateTime> ();
		}


		public SafeCoreSession CreateSession()
		{
			lock (this.sessionLock)
			{
				var id = this.GetNewId ();

				var session = new SafeCoreSession (id);

				this.PutSession (id, session);

				return session;
			}
		}


		private string GetNewId()
		{
			string id = null;

			while (id == null)
			{
				var tmpId = Guid.NewGuid ().ToString ("D");

				if (!this.sessions.ContainsKey (tmpId))
				{
					id = tmpId;
				}
			}

			return id;
		}


		public SafeCoreSession GetSession(string id)
		{
			lock (this.sessionLock)
			{
				return this.RetrieveSession (id);
			}
		}


		public bool DeleteSession(string id)
		{
			SafeCoreSession session;

			lock (this.sessionLock)
			{
				session = this.RemoveSession (id);
			}

			var found = session != null;

			if (found)
			{
				session.Dispose ();
			}

			return found;
		}


		public void CleanUpSessions()
		{
			var removedSessions = new List<SafeCoreSession> ();

			lock (this.sessionLock)
			{
				// Remove all the sessions that have timed out.
				var timedOutSessions = this.RemoveTimedOutSessions ();

				// If there is still too much sessions, remove the older ones.
				var excessiveSessions = this.RemoveExcessiveSessions ();

				removedSessions.AddRange (timedOutSessions);
				removedSessions.AddRange (excessiveSessions);
			}

			foreach (var session in removedSessions)
			{
				session.Dispose ();
			}
		}


		private IEnumerable<SafeCoreSession> RemoveTimedOutSessions()
		{
			var oldestValidTime = DateTime.UtcNow - this.sessionTimeout;

			var oldSessionIds = this.sessionLastAccessTimes
				.Where (e => e.Value < oldestValidTime)
				.Select (e => e.Key)
				.ToList ();

			foreach (var sessionId in oldSessionIds)
			{
				yield return this.RemoveSession (sessionId);
			}
		}


		private IEnumerable<SafeCoreSession> RemoveExcessiveSessions()
		{
			var nbExcessiveSessions = this.sessions.Count - this.maxNbSessions;

			if (nbExcessiveSessions > 0)
			{
				var excessiveSessionIds = this.sessionLastAccessTimes
					.OrderBy (e => e.Value)
					.Take (nbExcessiveSessions)
					.Select (e => e.Key)
					.ToList ();

				foreach (var sessionId in excessiveSessionIds)
				{
					yield return this.RemoveSession (sessionId);
				}
			}
		}


		public void Dispose()
		{
			foreach (var session in this.sessions.Keys.ToList ())
			{
				this.DeleteSession (session);
			}
		}


		private void PutSession(string id, SafeCoreSession session)
		{
			this.sessionLastAccessTimes[id] = DateTime.UtcNow;
			this.sessions[id] = session;
		}


		private SafeCoreSession RetrieveSession(string id)
		{
			SafeCoreSession session;

			this.sessions.TryGetValue (id, out session);

			if (session != null)
			{
				this.sessionLastAccessTimes[id] = DateTime.UtcNow;
			}

			return session;
		}


		private SafeCoreSession RemoveSession(string id)
		{
			SafeCoreSession session;

			var found = this.sessions.TryGetValue (id, out session);

			if (found)
			{
				this.sessions.Remove (id);
				this.sessionLastAccessTimes.Remove (id);
			}

			return session;
		}


		private readonly object sessionLock;


		private readonly int maxNbSessions;


		private readonly TimeSpan sessionTimeout;


		private readonly Dictionary<string, SafeCoreSession> sessions;


		private readonly Dictionary<string, DateTime> sessionLastAccessTimes;


	}


}
