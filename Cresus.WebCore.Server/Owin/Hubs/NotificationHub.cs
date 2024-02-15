//	Copyright © 2013-2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class NotificationHub : Hub
	{
		public void NotifyAll(string title, string message, string clickpath)
		{
			this.Clients
				.All
				.Toast (title, message, clickpath);
		}

		public void Notify(string connectionId, string title, string message, string clickpath)
		{
			this.Clients
				.Client (connectionId)
				.Toast (title, message, clickpath);
		}

		public void WarningToast(string connectionId, string title, string header, string errorField, string errorMessage, string message, string datasetId, string entityId)
		{
			this.Clients
				.Client (connectionId)
				.StickyWarningNavToast (title, message, header, errorField, errorMessage, datasetId, entityId);
		}


		public override Task OnDisconnected()
		{
			try
			{
				this.Clients
					.Client (NotificationClient.Instance.GetClientId ())
					.FlushConnectionId (this.Context.ConnectionId);
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
			var userName = this.Clients.Caller.userName;
				
			this.Clients
				.Client (NotificationClient.Instance.GetClientId ())
				.SetUserConnectionId (userName, this.Context.ConnectionId);
		}
	}
}
