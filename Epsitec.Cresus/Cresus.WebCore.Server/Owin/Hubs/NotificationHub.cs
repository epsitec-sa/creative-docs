using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Microsoft.AspNet.SignalR;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{

	public class NotificationHub : Hub
	{
		public void NotifyAll(string message,string clickpath)
		{
			Clients.All.Toast (message,clickpath);
		}

		public void WarningToast(string connectionId,string title,string message, string datasetId,string entityId)
		{
			Clients.Client (connectionId).StickyWarningNavToast (title,message,datasetId,entityId);		
		}

		public void SetConnectionId(string loginSessionId)
		{
//			this.Context.ConnectionId;
		}
	}
}
