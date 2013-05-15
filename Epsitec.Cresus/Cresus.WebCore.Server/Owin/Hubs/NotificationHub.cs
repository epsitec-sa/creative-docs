//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Library;

using Microsoft.AspNet.SignalR;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{

	public class NotificationHub : Hub, INotificationHub
	{
		public NotificationHub()
		{
			Epsitec.Cresus.Core.Library.NotificationManager.RegisterHub (this);
		}

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

		#region INotificationHub Members

		void INotificationHub.NotifyAll(NotificationMessage message)
		{
			this.NotifyAll (message.Body.ToSimpleText (), "");
		}

		void INotificationHub.Notify(string connectionId, NotificationMessage message)
		{
			this.WarningToast (connectionId, message.Title, message.Body.ToSimpleText (), "", "");
		}

		#endregion
	}
}
