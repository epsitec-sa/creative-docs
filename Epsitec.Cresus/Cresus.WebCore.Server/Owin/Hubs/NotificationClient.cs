using Epsitec.Cresus.Core.Library;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
    class NotificationClient : INotificationHub
    {
        public static NotificationClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationClient();
                }
                return instance;
            }
        }

        private NotificationClient()
        {
            Epsitec.Cresus.Core.Library.NotificationManager.RegisterHub(this);

            this.connectionMap = new Dictionary<string, string>();
            this.notificationsQueue = new List<QueuedNotification>();
            this.hubConnection = new HubConnection("http://localhost:9002/");
            this.hub = hubConnection.CreateHubProxy("NotificationHub");
			this.hub.On("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
			this.hub.On("FlushConnectionId", (string u, string c) => this.RemoveUserConnectionId (u, c));
            this.hubConnection.Start().Wait();
        }

		public string getConnectionId()
		{
			return this.hubConnection.ConnectionId;
		}

        void INotificationHub.NotifyAll(NotificationMessage message)
        {
            hub.Invoke("NotifyAll", message.Title, message.Body.ToSimpleText (), "").Wait();
        }

        void INotificationHub.Notify(string userName, NotificationMessage message)
        {
            if (this.connectionMap.ContainsKey(userName))
            {
                //var connectionId = this.connectionMap[userName];
				//hub.Invoke ("Notify", connectionId, message.Title, message.Body.ToSimpleText (), "").Wait ();
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).Toast (message.Title, message.Body.ToSimpleText (), "");
            }
            else
            {
                //queue
				this.notificationsQueue.Add (new QueuedNotification ("notify", userName, message));
            }
        }

		void INotificationHub.WarnUser(string userName, NotificationMessage message)
		{
			if (this.connectionMap.ContainsKey (userName))
			{
				//var connectionId = this.connectionMap[userName];
				//hub.Invoke ("WarningToast", connectionId, message.Title, message.Body.ToSimpleText (), "","").Wait ();
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).StickyWarningNavToast (message.Title, message.Body.ToSimpleText (), "", "");
			}
			else
			{
				//queue
				this.notificationsQueue.Add (new QueuedNotification("warning",userName,message));
			}
		}


        private void SetUserConnectionId(string userName,string connectionId)
        {
			if(!(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(connectionId)))
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				if (!this.connectionMap.ContainsKey(userName))
				{
					this.connectionMap.Add(userName, connectionId);
					context.Groups.Add (connectionId, userName);
					//send and flush pending user notification from queue
					foreach (var notif in this.notificationsQueue)
					{
						if (notif.DestinationUserName == userName)
						{
							if (this.connectionMap.ContainsKey(userName))
							{
								var cId = this.connectionMap[userName];

								

								switch (notif.NotificationType)
								{
									case "notify":
										//hub.Invoke ("Notify", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "").Wait ();

										context.Clients.Group (userName).Toast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "");
										break;
									case "warning":
										//hub.Invoke ("WarningToast", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "", "").Wait ();

										context.Clients.Group (userName).StickyWarningNavToast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "", "");
										break;
								}                  
							}
						}
					}
					this.notificationsQueue.RemoveAll(m => m.DestinationUserName == userName);
				}
				else
				{
					//remove old
					var oldCId = this.connectionMap[userName];
					this.RemoveUserConnectionId (userName, oldCId);
					context.Groups.Remove (oldCId, userName);
					//replace with new
					this.connectionMap[userName] = connectionId;
					context.Groups.Add (connectionId, userName);
				}
			} 
        }

		private void RemoveUserConnectionId(string userName, string connectionId)
		{
			var userKey = userName;

			if (String.IsNullOrEmpty (userKey) && !String.IsNullOrEmpty (connectionId))
			{
				foreach (var con in this.connectionMap)
				{
					if (con.Value == connectionId)
					{
						userKey = con.Key;
						break;
					}
				}
			}
			if (!String.IsNullOrEmpty (userKey))
			{
				this.connectionMap.Remove (userKey);
			}
			
		}
        private static NotificationClient instance;
        private HubConnection hubConnection;
        private IHubProxy hub;
        private Dictionary<string, string> connectionMap;
        private List<QueuedNotification> notificationsQueue;

		private class QueuedNotification
		{
			public QueuedNotification(string type, string userName, NotificationMessage payload)
			{
				this.NotificationType = type;
				this.DestinationUserName = userName;
				this.Message = payload;
			}

			public string NotificationType;
			public string DestinationUserName;
			public NotificationMessage Message;
		}
    }
}
