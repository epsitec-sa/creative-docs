using Epsitec.Common.Support;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	class NotificationClient : INotificationHub, IDisposable
	{

		private NotificationClient(CoreServer server)
		{
			if (CoreContext.HasExperimentalFeature ("Notifications"))
			{
				Epsitec.Cresus.Core.Library.NotificationManager.RegisterHub (this);

				this.server = server;

				this.hubClients = new List<HubClient> ();
				this.notificationsQueue = new List<QueuedNotification> ();

				this.hubConnection = new HubConnection ("http://localhost:9002/");
				this.hub = hubConnection.CreateHubProxy ("NotificationHub");


				//Register Hub Listener
				this.hub.On ("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
				this.hub.On ("FlushConnectionId", c => RemoveUserConnectionIdWithLock (c));

				this.hubConnection.Start ().Wait ();

				this.setupLock = new ReaderWriterLockWrapper ();
				this.queueLock = new ReaderWriterLockWrapper ();
			}

		}

		public static NotificationClient Instance
		{
			get
			{
				return instance;
			}
		}

		public static NotificationClient Create(CoreServer server)
		{
			if (instance == null)
			{
				instance = new NotificationClient (server);
			}
			return instance;
		}

		void INotificationHub.NotifyAll(NotificationMessage message, bool onConnect)
		{
			if (onConnect)
			{
				using (this.queueLock.LockWrite ())
				{
					this.notificationsQueue.Add (new QueuedNotification ("notifyall", "*", message));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.All.Toast (message.Title, message.Body.ToSimpleText (), "");
			}

		}

		void INotificationHub.Notify(string userName, NotificationMessage message, bool onConnect)
		{
			if (onConnect)
			{
				using (this.queueLock.LockWrite ())
				{
					this.notificationsQueue.Add (new QueuedNotification ("notify", userName, message));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).Toast (message.Title, message.Body.ToSimpleText (), "");
			}
		}

		void INotificationHub.WarnUser(string userName, NotificationMessage message, bool onConnect)
		{
			if (onConnect)
			{
				using (this.queueLock.LockWrite ())
				{
					this.notificationsQueue.Add (new QueuedNotification ("warning", userName, message));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).StickyWarningNavToast (message.Title, message.Body.ToSimpleText (), message.HeaderErrorMessage, this.GetErrorFieldId (message), message.ErrorFieldMessage, DataIO.DruidToString (message.Dataset), EntityIO.GetEntityId (message.EntityKey));
			}
		}

		public string getConnectionId()
		{
			return this.hubConnection.ConnectionId;
		}

		private void SetUserConnectionId(string userName, string connectionId)
		{
			if (!(String.IsNullOrEmpty (userName) || String.IsNullOrEmpty (connectionId)))
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				using (this.setupLock.LockWrite ())
				{

					if (!this.hubClients.Exists (c => c.Id == connectionId))
					{
						this.hubClients.Add (new HubClient (connectionId, userName));
						context.Groups.Add (connectionId, userName);
						//send and flush pending user notification from queue
						using (this.queueLock.LockRead ())
						{
							foreach (var notif in this.notificationsQueue)
							{
								if (notif.DestinationUserName == userName)
								{
									switch (notif.NotificationType)
									{
										case "notifyall":
											context.Clients.All.Toast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "");
											break;
										case "notify":
											context.Clients.Client (connectionId).Toast (notif.Message.Title, notif.Message.Body.ToSimpleText (), "");
											break;
										case "warning":
											context.Clients.Client (connectionId).StickyWarningNavToast (notif.Message.Title, notif.Message.Body.ToSimpleText (), notif.Message.HeaderErrorMessage, this.GetErrorFieldId (notif.Message), notif.Message.ErrorFieldMessage, DataIO.DruidToString (notif.Message.Dataset), EntityIO.GetEntityId (notif.Message.EntityKey));
											break;
									}
								}
							}
						}
						using (this.queueLock.LockWrite ())
						{
							this.notificationsQueue.RemoveAll (m => m.DestinationUserName == userName);
						}

					}
					else
					{
						//remove old connectionId
						var oldCId = this.hubClients.Find (c => c.Id == connectionId).Id;
						this.RemoveUserConnectionId (oldCId);
						context.Groups.Remove (oldCId, userName);
						//and replace with new
						this.hubClients.Add (new HubClient (connectionId, userName));
						context.Groups.Add (connectionId, userName);
					}
				}
			}
		}

		private void RemoveUserConnectionIdWithLock(string connectionId)
		{
			using (this.setupLock.LockWrite ())
			{
				this.RemoveUserConnectionId (connectionId);
			}

		}

		private void RemoveUserConnectionId(string connectionId)
		{
			this.hubClients.RemoveAll (c => c.Id == connectionId);
		}

		private string GetErrorFieldId(NotificationMessage message)
		{
			if (message.ErrorField == null)
			{
				return null;
			}
			else
			{
				return this.server.Caches.PropertyAccessorCache.Get (message.ErrorField).Id;
			}
		}

		private CoreServer server;

		private static NotificationClient instance;

		private HubConnection hubConnection;

		private IHubProxy hub;

		private List<HubClient> hubClients;

		private readonly ReaderWriterLockWrapper setupLock;

		private List<QueuedNotification> notificationsQueue;

		private readonly ReaderWriterLockWrapper queueLock;

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

		#region IDisposable Members

		public void Dispose()
		{
			this.queueLock.Dispose ();
			this.setupLock.Dispose ();
		}

		#endregion
	}
}
