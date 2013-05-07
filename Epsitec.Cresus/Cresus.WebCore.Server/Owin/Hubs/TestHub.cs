using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	class TestHub : Hub
	{
		public class MessageRecievingHub : Hub
		{
			public void SendMessage(string message)
			{
				Console.WriteLine (message);
				string it = new string (message.Reverse ().ToArray ());
				Clients.All.broadCastToClients (it);
			}
		}
	}
}
