//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Server.CoreServer
{


	public sealed class CoreSessionManager : AbstractServerObject, IDisposable
	{


		public CoreSessionManager(ServerContext serverContext)
			: base (serverContext)
		{
			this.sessionLock = new object ();
			this.sessions = new Dictionary<string, CoreSession> ();
		}


		public CoreSession CreateSession()
		{
			string sessionId = System.Guid.NewGuid ().ToString ("D");

			return this.CreateSession (sessionId);
		}


		private CoreSession CreateSession(string id)
		{
			lock (this.sessionLock)
			{
				if (this.sessions.ContainsKey (id))
				{
					return null;
				}

				var session = new CoreSession (id);

				this.sessions.Add (id, session);

				return session;
			}
		}


		public CoreSession GetCoreSession(string id)
		{
			CoreSession session = null;

			lock (this.sessionLock)
			{
				if (id != null)
				{
					this.sessions.TryGetValue (id, out session);
				}
			}

			return session;
		}


		public bool DeleteSession(CoreSession session)
		{
			return this.DeleteSession (session.Id);
		}


		public bool DeleteSession(string id)
		{
			CoreSession session = null;
			bool found;

			lock (this.sessionLock)
			{
				found = this.sessions.TryGetValue (id, out session);

				if (found)
				{
					this.sessions.Remove (id);
				}	
			}

			if (found)
			{
				session.DisposeBusinessContext ();
				session.Dispose ();
			}

			return found;
		}


		public void Dispose()
		{
			foreach (var session in this.sessions.Keys.ToList ())
			{
				this.DeleteSession (session);
			}
		}


		private readonly object sessionLock;


		private readonly Dictionary<string, CoreSession> sessions;


	}


}
