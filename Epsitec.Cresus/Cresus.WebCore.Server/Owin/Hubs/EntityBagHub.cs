//	Copyright © 2013-2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class EntityBagHub : Hub
	{
		public override Task OnDisconnected()
		{
			try
			{
				this.Clients
					.Client (EntityBagClient.Instance.GetClientId ())
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
			var userName         = this.Clients.Caller.userName;
			var userConnectionId = this.Context.ConnectionId;
			
			this.Clients
				.Client (EntityBagClient.Instance.GetClientId ())
				.SetUserConnectionId (userName, userConnectionId);
		}

		public void RemoveFromMyBag(string entityId)
		{
			var userName = this.Clients.Caller.userName;
			
			this.Clients
				.Client (EntityBagClient.Instance.GetClientId ())
				.RemoveFromMyBag (userName, entityId);
		}

		public void AddToMyBag(string title, string customSummary,string entityId)
		{
			var userName = this.Clients.Caller.userName;

			this.Clients
				.Client (EntityBagClient.Instance.GetClientId ())
				.AddToMyBag (userName, title, customSummary, entityId);
		}
	}
}
