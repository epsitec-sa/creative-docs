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

			this.connectionMap = new List<HubClient> ();
            this.notificationsQueue = new List<QueuedNotification>();
            this.hubConnection = new HubConnection("http://localhost:9002/");
            this.hub = hubConnection.CreateHubProxy("NotificationHub");
			this.hub.On("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
			this.hub.On("FlushConnectionId", c => this.RemoveUserConnectionId (c));
            this.hubConnection.Start().Wait();
        }

		public string getConnectionId()
		{
			return this.hubConnection.ConnectionId;
		}

        void INotificationHub.NotifyAll(NotificationMessage message,bool onConnect)
        {
			if (onConnect)
			{

				//queue
				this.notificationsQueue.Add (new QueuedNotification ("notify", "*", message));
			}
			else
			{
				//var connectionId = this.connectionMap[userName];
				//hub.Invoke ("Notify", connectionId, message.Title, message.Body.ToSimpleText (), "").Wait ();
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.All.Toast (message.Title, message.Body.ToSimpleText (), "");
			}
            
        }

        void INotificationHub.Notify(string userName, NotificationMessage message,bool onConnect)
        {
            if (onConnect)
            {
                
				//queue
				this.notificationsQueue.Add (new QueuedNotification ("notify", userName, message));
            }
            else
            {
				//var connectionId = this.connectionMap[userName];
				//hub.Invoke ("Notify", connectionId, message.Title, message.Body.ToSimpleText (), "").Wait ();
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).Toast (message.Title, message.Body.ToSimpleText (), "");
            }
        }

		void INotificationHub.WarnUser(string userName, NotificationMessage message,bool onConnect)
		{
			if (onConnect)
			{
				//queue
				this.notificationsQueue.Add (new QueuedNotification ("warning", userName, message));
			}
			else
			{
				
				//var connectionId = this.connectionMap[userName];
				//hub.Invoke ("WarningToast", connectionId, message.Title, message.Body.ToSimpleText (), "","").Wait ();
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).StickyWarningNavToast (message.Title, message.Body.ToSimpleText (), "", "");
			}
		}


        private void SetUserConnectionId(string userName,string connectionId)
        {
			if(!(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(connectionId)))
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				if (!this.connectionMap.Exists(c => c.Id == connectionId))
				{
					this.connectionMap.Add(new HubClient(connectionId,userName));
					context.Groups.Add (connectionId, userName);
					//send and flush pending user notification from queue
					foreach (var notif in this.notificationsQueue)
					{
						if (notif.DestinationUserName == userName)
						{

							switch (notif.NotificationType)
							{
								case "notify":
									//hub.Invoke ("Notify", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "").Wait ();

									context.Clients.Client (connectionId).Toast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "");
									break;
								case "warning":
									//hub.Invoke ("WarningToast", cId, notif.Message.Title, notif.Message.Body.ToSimpleText (), "", "").Wait ();

									context.Clients.Client (connectionId).StickyWarningNavToast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "", "");
									break;
							}                  
							
						}
					}
					this.notificationsQueue.RemoveAll(m => m.DestinationUserName == userName);
				}
				else
				{
					//remove old
					var oldCId = this.connectionMap.Find (c => c.Id == connectionId).Id;
					this.RemoveUserConnectionId (oldCId);
					context.Groups.Remove (oldCId, userName);
					//replace with new
					this.connectionMap.Add (new HubClient (connectionId, userName));
					context.Groups.Add (connectionId, userName);
				}
			} 
        }

		private void RemoveUserConnectionId(string connectionId)
		{
			this.connectionMap.RemoveAll (c => c.Id == connectionId);
			
		}
        private static NotificationClient instance;
        private HubConnection hubConnection;
        private IHubProxy hub;
        private List<HubClient> connectionMap;
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

		private class HubClient
		{
			public HubClient(string connectionId, string userName)
			{
				this.Id = connectionId;
				this.UserName = userName;
			}

			public string Id;
			public string UserName;
		}
    }
}
