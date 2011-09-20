//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreServer
	{

		private static CoreServer instance;
		public static CoreServer Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CoreServer ();
				}

				return instance;
			}
			set
			{
				instance = value;
			}
		}

		private CoreServer()
		{
			this.sessions = new Dictionary<string, CoreSession> ();
		}

		
		public CoreSession CreateSession()
		{
			return this.CreateSession (System.Guid.NewGuid ().ToString ("D"));
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
			CoreSession session;
			if (id == null || !this.sessions.TryGetValue (id, out session))
			{
				return null;
			}
			else
			{
				return session;
			}
		}

		public bool DeleteSession(CoreSession session)
		{
			return this.DeleteSession (session.Id);
		}

		public bool DeleteSession(string id)
		{
			CoreSession session;

			lock (this.sessions)
			{
				if (this.sessions.TryGetValue (id, out session) == false)
				{
					return false;
				}

				this.sessions.Remove (id);
			}

			session.DisposeBusinessContext ();
			session.Dispose ();

			return true;
		}


		private readonly Dictionary<string, CoreSession> sessions;
	}
}
