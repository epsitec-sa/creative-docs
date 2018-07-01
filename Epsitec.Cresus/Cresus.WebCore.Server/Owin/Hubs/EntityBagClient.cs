//	Copyright © 2013-2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client.Hubs;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Nancy.Helpers;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	internal class EntityBagClient : IEntityBagHub, System.IDisposable
	{
		private EntityBagClient(CoreServer server)
		{
			if (CoreContext.HasExperimentalFeature ("EntityBag"))
			{
				EntityBagManager.RegisterHub (this);

				this.server = server;

				this.hubClients     = new List<HubClient> ();
				this.bagEntityCache = new List<BagEntity> ();

				this.hubClientsLock     = new ReaderWriterLockWrapper ();
				this.bagEntityCacheLock = new ReaderWriterLockWrapper ();

				this.hubConnection = new HubConnection ("http://localhost:9002/");
				this.hub = hubConnection.CreateHubProxy ("EntityBagHub");

				//	Register Hub Listener
				this.hub.On ("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
				this.hub.On ("FlushConnectionId", (string c) => this.FlushConnectionId (c));
				this.hub.On ("RemoveFromMyBag", (string u, string id) => RemoveFromMyBag (u,id));
				this.hub.On ("AddToMyBag", (string u, string t,string s, string id) => AddToMyBag (u,t,s,id));

				this.hubConnection.Start ().Wait ();
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


		public string GetClientId()
		{
			return this.hubConnection.ConnectionId;
		}

	
		#region IEntityBagHub Members

		void IEntityBagHub.AddToBag(string userName, string title, FormattedText summary, string entityId, When when)
		{
			string content = HttpUtility.HtmlEncode (summary.ToString ());
			
			if (when == When.OnConnect)
			{
				using (this.bagEntityCacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, content, entityId));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();

				context
					.Clients
					.Group (userName)
					.AddToBag (title, content, entityId);

				using (this.bagEntityCacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, content, entityId));
				}
			}
		}

		void IEntityBagHub.RemoveFromBag(string userName, string entityId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.bagEntityCacheLock.LockWrite ())
				{
					this.bagEntityCache.Add (new BagEntity (userName, "REMOVE", null, null, entityId));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
				
				context
					.Clients
					.Group (userName)
					.RemoveFromBag (entityId);
				
				using (this.bagEntityCacheLock.LockWrite ())
				{
					this.bagEntityCache.RemoveAll(e => e.DestinationUserName == userName && e.EntityId == entityId);
				}
			}
		}

		void IEntityBagHub.SetLoading(string userName,bool state)
		{			
			var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
			
			context
				.Clients
				.Group (userName)
				.SetLoading (state);			
		}

		IEnumerable<string> IEntityBagHub.GetUserBagEntitiesId(string userName)
		{
			using (this.bagEntityCacheLock.LockRead ())
			{
				return this.bagEntityCache
					.Where (b => b.DestinationUserName == userName)
					.Select (b => "db:" + b.EntityId.Replace ('-', ':'))
					.ToList ();
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.bagEntityCacheLock.Dispose ();
			this.hubClientsLock.Dispose ();
		}

		#endregion

		private void SetUserConnectionId(string userName, string connectionId)
		{
			if ((string.IsNullOrEmpty (userName) == false) &&
				(string.IsNullOrEmpty (connectionId) == false))
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
				var found   = default (HubClient);

				using (this.hubClientsLock.LockRead ())
				{
					found = this.hubClients.Find (c => c.Id == connectionId);
				}

				if (found != null)
				{
					using (this.hubClientsLock.LockWrite ())
					{
						this.hubClients.RemoveAll (c => c.Id == found.Id);
						this.hubClients.Add (new HubClient (connectionId, userName));
					}

					context
						.Groups
						.Remove (found.Id, userName);

					context
						.Groups
						.Add (connectionId, userName);
				}
				else
				{
					using (this.hubClientsLock.LockWrite ())
					{
						this.hubClients.Add (new HubClient (connectionId, userName));
					}

					context
						.Groups
						.Add (connectionId, userName);

					this.SendQueuedActions (context, connectionId, userName);
				}
			}
		}

		private void SendQueuedActions(IHubContext context, string connectionId, string userName)
		{
			using (this.bagEntityCacheLock.LockRead ())
			{
				foreach (var bagEntity in this.bagEntityCache)
				{
					if (bagEntity.DestinationUserName == userName)
					{
						switch (bagEntity.Action)
						{
							case "ADD":
								context
									.Clients
									.Client (connectionId)
									.AddToBag (bagEntity.Title, bagEntity.Content, bagEntity.EntityId);
								break;

							case "REMOVE":
								context
									.Clients
									.Client (connectionId)
									.RemoveFromBag (bagEntity.EntityId);
								break;
						}
					}
				}
			}
		}
		
		private void RemoveFromMyBag(string userName, string entityId)
		{
			using (this.bagEntityCacheLock.LockWrite ())
			{
				this.bagEntityCache.RemoveAll (e => e.DestinationUserName == userName && e.EntityId == entityId);
			}

			var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
			
			context
				.Clients
				.Group (userName)
				.RemoveFromBag (entityId);
		}

		private void AddToMyBag(string userName, string title, string clientSummary, string entityId)
		{
			var entity = this.server.CoreWorkerPool.Execute (userName, null, (b) => EntityIO.ResolveEntity (b, entityId));
			
			if (entity.IsNotNull ())
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();

				try //with GetSummary() on from AbstractEntity
				{
					var summary = entity.GetSummary ();
					var content = summary.ToString ();

					context
						.Clients
						.Group (userName)
						.AddToBag (title, content, entityId);
					
					using (this.bagEntityCacheLock.LockWrite ())
					{
						this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, content, entityId));
					}
				}
				catch //use client summary instead
				{
					context
						.Clients
						.Group (userName)
						.AddToBag (title, clientSummary, entityId);

					using (this.bagEntityCacheLock.LockWrite ())
					{
						this.bagEntityCache.Add (new BagEntity (userName, "ADD", title, clientSummary, entityId));
					}
				}			
			}
		}

		private void FlushConnectionId(string connectionId)
		{
			using (this.hubClientsLock.LockWrite ())
			{
				this.hubClients.RemoveAll (c => c.Id == connectionId);
			}
		}


		#region BagEntity Class

		private sealed class BagEntity
		{
			public BagEntity(string userName, string action, string title, string content, string entityId)
			{
				this.Action = action;
				this.Title = title;
				this.Content = content;
				this.EntityId = entityId;
				this.DestinationUserName = userName;
			}

			public readonly string				Action;
			public readonly string				Title;
			public readonly string				Content;
			public readonly string				EntityId;
			public readonly string				DestinationUserName;
		}

		#endregion

		#region HubClient Class

		private sealed class HubClient
		{
			public HubClient(string connectionId, string userName)
			{
				this.Id = connectionId;
				this.UserName = userName;
			}

			public readonly string				Id;
			public readonly string				UserName;
		}

		#endregion


		private static EntityBagClient			instance;

		private readonly ReaderWriterLockWrapper hubClientsLock;
		private readonly List<HubClient>		 hubClients;

		private readonly ReaderWriterLockWrapper bagEntityCacheLock;
		private readonly List<BagEntity>		 bagEntityCache;

		private readonly CoreServer				server;
		private readonly HubConnection			hubConnection;
		private readonly IHubProxy				hub;
	}
}
