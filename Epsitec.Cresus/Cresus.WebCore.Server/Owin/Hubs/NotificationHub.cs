//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP


using Microsoft.AspNet.SignalR;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{

	public class NotificationHub : Hub
    {
		public void NotifyAll(string title,string message,string clickpath)
		{
			Clients.All.Toast (title,message,clickpath);
		}

		public void WarningToast(string connectionId,string title,string message, string datasetId,string entityId)
		{
			Clients.Client (connectionId).StickyWarningNavToast (title,message,datasetId,entityId);		
		}
	}
}
