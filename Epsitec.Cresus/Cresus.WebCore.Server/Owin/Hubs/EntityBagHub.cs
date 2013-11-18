//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class EntityBagHub : Hub
	{

		public void AddToBag(string connectionId, string title, string summary, string entityId)
		{
			Clients.Client (connectionId).AddToBag (title, summary, entityId);
		}

		public void RemoveFromBag(string connectionId, string title, string summary, string entityId)
		{
			Clients.Client (connectionId).RemoveFromBag (title, summary, entityId);
		}


		public override Task OnDisconnected()
		{
			var backendClient = EntityBagClient.Instance;


			Clients.Client (backendClient.GetConnectionId ()).FlushConnectionId (Context.ConnectionId);

			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			var backendClient = EntityBagClient.Instance;

			Clients.Client (backendClient.GetConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Context.ConnectionId);
			return base.OnReconnected ();
		}

		public void SetupUserConnection()
		{
			var backendClient = EntityBagClient.Instance;
			Clients.Client (backendClient.GetConnectionId ()).SetUserConnectionId (Clients.Caller.userName, Clients.Caller.connectionId);
		}
	}
}
