//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	class EntityBagClient : IEntityBagHub, System.IDisposable
	{
		private EntityBagClient(CoreServer server)
		{
			if (CoreContext.HasExperimentalFeature ("EntityBag"))
			{
				Epsitec.Cresus.Core.Library.EntityBagManager.RegisterHub (this);

				this.server = server;

				this.hubClients = new List<HubClient> ();
				this.bagEntityCache = new List<BagEntity> ();

				this.hubConnection = new HubConnection ("http://localhost:9002/");
				this.hub = hubConnection.CreateHubProxy ("EntityBagHub");


				//Register Hub Listener
				this.hub.On ("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
				this.hub.On ("FlushConnectionId", c => RemoveUserConnectionIdWithLock (c));
				this.hub.On ("RemoveFromMyBag", (string u, string id) => RemoveFromMyBag (u,id));

				this.hubConnection.Start ().Wait ();

				this.setupLock = new ReaderWriterLockWrapper ();
				this.cacheLock = new ReaderWriterLockWrapper ();
			}

		}

		public static EntityBagClient Instance
		{
			get
			{
				return instance;
			}
		}

		public static EntityBagClient Create(CoreServer server)
		{
			if (instance == null)
			{
				instance = new EntityBagClient (server);
			}
			return instance;
		}

		#region IEntityBagHub Members

		void IEntityBagHub.AddToBag(string userName, string title, FormattedText summary, string entityId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, summary, entityId));
				}

			}
			else
			{

				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
				context.Clients.Group (userName).AddToBag (title, summary, entityId);
				using (this.cacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, summary, entityId));
				}
			}
		}

		void IEntityBagHub.RemoveFromBag(string userName, string entityId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "REMOVE", null, null, entityId));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
				context.Clients.Group (userName).RemoveFromBag (entityId);
				using (this.cacheLock.LockWrite ())
				{
					this.bagEntityCache.RemoveAll(e => e.DestinationUserName == userName && e.EntityId == entityId);
				}
			}
		}

		void IEntityBagHub.SetLoading(string userName,bool state)
		{			
			var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
			context.Clients.Group (userName).SetLoading (state);			
		}

		IEnumerable<string> IEntityBagHub.GetUserBagEntitiesId(string userName)
		{
			return this.bagEntityCache.Where (b => b.DestinationUserName == userName).Select(b => "db:" + b.EntityId.Replace('-',':'));
		}

		#endregion

		public string GetConnectionId()
		{
			return this.hubConnection.ConnectionId;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.cacheLock.Dispose ();
			this.setupLock.Dispose ();
		}

		#endregion

		private void SetUserConnectionId(string userName, string connectionId)
		{
			if (!(string.IsNullOrEmpty (userName) || string.IsNullOrEmpty (connectionId)))
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
				using (this.setupLock.LockWrite ())
				{

					if (!this.hubClients.Exists (c => c.Id == connectionId))
					{
						this.hubClients.Add (new HubClient (connectionId, userName));
						context.Groups.Add (connectionId, userName);
						//send and flush pending user notification from queue
						using (this.cacheLock.LockRead ())
						{
							foreach (var bagEntity in this.bagEntityCache)
							{
								if (bagEntity.DestinationUserName == userName)
								{
									switch (bagEntity.Action)
									{
										case "ADD":
											context.Clients.Client (connectionId).AddToBag (bagEntity.Title,bagEntity.Summary,bagEntity.EntityId);
											break;
										case "REMOVE":
											context.Clients.Client (connectionId).RemoveFromBag (bagEntity.EntityId);
											break;
									}
								}
							}
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

		private void RemoveFromMyBag(string userName,string entityId)
		{
			using (this.cacheLock.LockWrite ())
			{
				this.bagEntityCache.RemoveAll (e => e.DestinationUserName == userName && e.EntityId == entityId);
			}

			var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
			context.Clients.Group (userName).RemoveFromBag (entityId);
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


		private sealed class BagEntity
		{
			public BagEntity(string userName, string action, string title, FormattedText summary,string entityId)
			{
				this.Action = action;
				this.Title = title;
				this.Summary = summary;
				this.EntityId = entityId;
				this.DestinationUserName = userName;
			}

			public string						Action;
			public string						Title;
			public FormattedText				Summary;
			public string						EntityId;
			public string						DestinationUserName;
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

		
		private static EntityBagClient		instance;

		private readonly ReaderWriterLockWrapper setupLock;
		private readonly ReaderWriterLockWrapper cacheLock;

		private readonly CoreServer				server;
		private readonly HubConnection			hubConnection;
		private readonly IHubProxy				hub;
		private readonly List<HubClient>		hubClients;
		private readonly List<BagEntity>		bagEntityCache;

		
	}
}
