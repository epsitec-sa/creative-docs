//	Copyright © 2013-2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class StatusBarHub : Hub
	{
		public override Task OnDisconnected()
		{
			try
			{
				var connectionId = this.Context.ConnectionId;

				this.Clients
					.Client (StatusBarClient.Instance.GetConnectionId ())
					.FlushConnectionId (connectionId);
			}
			catch
			{
			}
			
			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			return base.OnReconnected ();
		}

		public void SetupUserConnection()
		{
			try
			{
				var userName     = this.Clients.Caller.userName;
				var connectionId = this.Clients.Caller.connectionId;

				this.Clients
					.Client (StatusBarClient.Instance.GetConnectionId ())
					.SetUserConnectionId (userName, connectionId);
			}
			catch
			{
			}
		}
	}
}
