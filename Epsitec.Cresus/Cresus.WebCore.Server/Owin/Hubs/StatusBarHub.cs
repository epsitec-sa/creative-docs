//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class StatusBarHub : Hub
	{
		public override Task OnDisconnected()
		{
			var backendClient = StatusBarClient.Instance;


			Clients.Client (backendClient.GetConnectionId ()).FlushConnectionId (Context.ConnectionId);

			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			var backendClient = StatusBarClient.Instance;

			Clients.Client (backendClient.GetConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Context.ConnectionId);
			return base.OnReconnected ();
		}

		public void SetupUserConnection()
		{
			var backendClient = StatusBarClient.Instance;
			Clients.Client (backendClient.GetConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Clients.Caller.connectionId);
		}
	}
}
