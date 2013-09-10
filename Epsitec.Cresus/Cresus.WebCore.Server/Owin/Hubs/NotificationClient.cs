//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	class NotificationClient : INotificationHub, System.IDisposable
	{
		private NotificationClient(CoreServer server)
		{
			if (CoreContext.HasExperimentalFeature ("Notifications"))
			{
				Epsitec.Cresus.Core.Library.NotificationManager.RegisterHub (this);

				this.server = server;

				this.hubClients = new List<HubClient> ();
				this.notificationQueue = new List<Notification> ();

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

		#region INotificationHub Members

		void INotificationHub.NotifyAll(NotificationMessage message, bool onConnect)
		{
			if (onConnect)
			{
				using (this.queueLock.LockWrite ())
				{
					this.notificationQueue.Add (new Notification ("notifyall", "*", message));
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
					this.notificationQueue.Add (new Notification ("notify", userName, message));
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
					this.notificationQueue.Add (new Notification ("warning", userName, message));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub> ();
				context.Clients.Group (userName).StickyWarningNavToast (message.Title, message.Body.ToSimpleText (), message.HeaderErrorMessage, this.GetErrorFieldId (message), message.ErrorFieldMessage, DataIO.DruidToString (message.Dataset), EntityIO.GetEntityId (message.EntityKey));
			}
		}

		#endregion

		public string GetConnectionId()
		{
			return this.hubConnection.ConnectionId;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.queueLock.Dispose ();
			this.setupLock.Dispose ();
		}

		#endregion

		private void SetUserConnectionId(string userName, string connectionId)
		{
			if (!(string.IsNullOrEmpty (userName) || string.IsNullOrEmpty (connectionId)))
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
							foreach (var notif in this.notificationQueue)
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
							this.notificationQueue.RemoveAll (m => m.DestinationUserName == userName);
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

		
		private sealed class Notification
		{
			public Notification(string type, string userName, NotificationMessage payload)
			{
				this.NotificationType = type;
				this.DestinationUserName = userName;
				this.Message = payload;
			}

			public string						NotificationType;
			public string						DestinationUserName;
			public NotificationMessage			Message;
		}

		
		private sealed class HubClient
		{
			public HubClient(string connectionId, string userName)
			{
				this.Id = connectionId;
				this.UserName = userName;
			}

			public string						Id;
			public string						UserName;
		}

		
		private static NotificationClient		instance;

		private readonly ReaderWriterLockWrapper setupLock;
		private readonly ReaderWriterLockWrapper queueLock;

		private readonly CoreServer				server;
		private readonly HubConnection			hubConnection;
		private readonly IHubProxy				hub;
		private readonly List<HubClient>		hubClients;
		private readonly List<Notification>		notificationQueue;
	}
}
