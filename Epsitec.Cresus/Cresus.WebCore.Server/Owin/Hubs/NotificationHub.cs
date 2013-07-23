//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP


using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{

	public class NotificationHub : Hub
	{

		public void NotifyAll(string title, string message, string clickpath)
		{
			Clients.All.Toast (title, message, clickpath);
		}

		public void Notify(string connectionId, string title, string message, string clickpath)
		{
			Clients.Client (connectionId).Toast (title, message, clickpath);
		}

		public void WarningToast(string connectionId, string title, string header, string errorField, string errorMessage, string message, string datasetId, string entityId)
		{
			Clients.Client (connectionId).StickyWarningNavToast (title, message, header, errorField, errorMessage, datasetId, entityId);
		}


		public override Task OnDisconnected()
		{
			var backendClient = NotificationClient.Instance;


			Clients.Client (backendClient.getConnectionId ()).FlushConnectionId (Context.ConnectionId);

			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			var backendClient = NotificationClient.Instance;

			Clients.Client (backendClient.getConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Context.ConnectionId);
			return base.OnReconnected ();
		}

		public void SetupUserConnection()
		{
			var backendClient = NotificationClient.Instance;
			Clients.Client (backendClient.getConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Clients.Caller.connectionId);
		}

	}
}
