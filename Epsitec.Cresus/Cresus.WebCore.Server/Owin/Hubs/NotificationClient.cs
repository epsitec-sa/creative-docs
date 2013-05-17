using Epsitec.Cresus.Core.Library;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;

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
			this.hub.On("SetConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
			this.hub.On("FlushConnectionId", (string u, string c) => this.RemoveUserConnectionId (u, c));
            this.hubConnection.Start().Wait();

            
        }

        void INotificationHub.NotifyAll(NotificationMessage message)
        {
            hub.Invoke("NotifyAll", message.Title, message.Body.ToSimpleText (), "").Wait();
        }

        void INotificationHub.Notify(string userName, NotificationMessage message)
        {
            if (this.connectionMap.ContainsKey(userName))
            {
                var connectionId = this.connectionMap[userName];
				hub.Invoke ("Notify", connectionId, message.Title, message.Body.ToSimpleText (), "").Wait ();
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
				var connectionId = this.connectionMap[userName];
				hub.Invoke ("WarningToast", connectionId, message.Title, message.Body.ToSimpleText (), "","").Wait ();
			}
			else
			{
				//queue
				this.notificationsQueue.Add (new QueuedNotification("warning",userName,message));
			}
		}

        public string getConnectionId()
        {
            return this.hubConnection.ConnectionId;
        }

        private void SetUserConnectionId(string userName,string connectionId)
        {
            if (!this.connectionMap.ContainsKey(userName))
            {
                this.connectionMap.Add(userName, connectionId);
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
									hub.Invoke ("Notify", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "").Wait ();
									break;
								case "warning":
									hub.Invoke ("WarningToast", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "", "").Wait ();
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
				//replace with new
                this.connectionMap[userName] = connectionId;
            }
        }

		private void RemoveUserConnectionId(string userName, string connectionId)
		{
			if (this.connectionMap.ContainsKey (userName))
			{
				this.connectionMap.Remove (userName);
				//flush pending user notification from queue
				foreach (var notif in this.notificationsQueue)
				{
					if (notif.DestinationUserName == userName)
					{
						this.notificationsQueue.Remove (notif);
					}
				}
			}
		}
        private static NotificationClient instance;
        private HubConnection hubConnection;
        private IHubProxy hub;
        private Dictionary<string, string> connectionMap;
        private List<QueuedNotification> notificationsQueue;
        private const string secret = "jh1832h4hhf8132ASD9WWD)DN8d^DS)/8n(D&S";

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
