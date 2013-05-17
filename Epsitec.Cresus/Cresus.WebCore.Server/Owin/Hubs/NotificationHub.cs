//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP


using Microsoft.AspNet.SignalR;
using Epsitec.Cresus.WebCore.Server.Core;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{

	public class NotificationHub : Hub
	{
		private string BackendConnectionId;

		public void NotifyAll(string title, string message, string clickpath)
		{
			Clients.All.Toast (title, message, clickpath);
		}

		public void Notify(string connectionId, string title, string message, string clickpath)
		{
			Clients.Client (connectionId).Toast (title, message, clickpath);
		}

		public void WarningToast(string connectionId, string title, string message, string datasetId, string entityId)
		{
			Clients.Client (connectionId).StickyWarningNavToast (title, message, datasetId, entityId);
		}

		public void LogIn(string userName, string connectionId)
		{
			var backendClient = NotificationClient.Instance;

			Clients.Client (backendClient.getConnectionId ()).SetConnectionId (userName, connectionId);
		}

		public void LogOut(string userName, string connectionId)
		{
			var backendClient = NotificationClient.Instance;

			Clients.Client (backendClient.getConnectionId ()).FlushConnectionId (userName, connectionId);
		}
	}
}
