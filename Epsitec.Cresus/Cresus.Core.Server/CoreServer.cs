//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Server
{

	public sealed class CoreServer
	{			
		private CoreServer()
		{
			this.sessions = new Dictionary<string, CoreSession> ();
		}

		public static CoreServer Instance
		{
			get
			{
				if (CoreServer.instance == null)
				{
					CoreServer.instance = new CoreServer ();
				}

				return CoreServer.instance;
			}
		}

		public CoreSession CreateSession()
		{
			string sessionId = System.Guid.NewGuid ().ToString ("D");

			return this.CreateSession (sessionId);
		}

		private CoreSession CreateSession(string id)
		{
			lock (this.sessions)
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

			lock (this.sessions)
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

			lock (this.sessions)
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

		private readonly Dictionary<string, CoreSession> sessions;

		private static CoreServer instance;
	}
}
