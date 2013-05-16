using Epsitec.Cresus.Core.Library;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;

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

            var hubConnection = new HubConnection("http://localhost:9002/");
            this.hub = hubConnection.CreateHubProxy("NotificationHub");
            hubConnection.Start().Wait();
        }

        void INotificationHub.NotifyAll(NotificationMessage message)
        {
            hub.Invoke("NotifyAll", message.Title, message.Body.ToSimpleText (), "").Wait();
        }

        void INotificationHub.Notify(string connectionId, NotificationMessage message)
        {
            //...
        }

        private static NotificationClient instance;
        private IHubProxy hub;
    }
}
