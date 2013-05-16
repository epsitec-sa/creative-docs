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
            this.notificationsQueue = new List<Tuple<string, NotificationMessage>>();
            this.hubConnection = new HubConnection("http://localhost:9002/");
            this.hub = hubConnection.CreateHubProxy("NotificationHub");
            this.hub.On<string, string>("SetConnectionId", delegate(string u, string c) { this.SetUserConnectionId(u, c); });

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
                hub.Invoke("Notify", connectionId, message.Title, message.Body.ToSimpleText(), "").Wait();
            }
            else
            {
                //queue
                this.notificationsQueue.Add(Tuple.Create(userName, message));
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
                //flush user notification queue
                foreach (var tuple in this.notificationsQueue)
                {
                    if (tuple.Item1 == userName)
                    {
                        if (this.connectionMap.ContainsKey(userName))
                        {
                            var cId = this.connectionMap[userName];
                            hub.Invoke("Notify", cId, tuple.Item2.Title, tuple.Item2.Body.ToSimpleText(), "").Wait();
                        }
                        this.notificationsQueue.Remove(tuple);
                    }
                }
            }
            else
            {
                this.connectionMap[userName] = connectionId;
            }
        }
        private static NotificationClient instance;
        private HubConnection hubConnection;
        private IHubProxy hub;
        private Dictionary<string, string> connectionMap;
        private List<Tuple<string,NotificationMessage>> notificationsQueue;
        private const string secret = "jh1832h4hhf8132ASD9WWD)DN8d^DS)/8n(D&S";
    }
}
