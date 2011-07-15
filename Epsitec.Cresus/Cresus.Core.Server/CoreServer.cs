//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreServer
	{
		public CoreServer()
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

			session.Dispose ();

			return true;
		}


		private readonly Dictionary<string, CoreSession> sessions;
	}
}
