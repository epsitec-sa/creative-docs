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
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	class StatusBarClient : IStatusBarHub, System.IDisposable
	{
		private StatusBarClient(CoreServer server)
		{
			if (CoreContext.HasExperimentalFeature ("StatusBar"))
			{
				Epsitec.Cresus.Core.Library.StatusBarManager.RegisterHub (this);

				this.server = server;

				this.hubClients = new List<HubClient> ();
				this.statusEntityCache = new List<StatusEntity> ();

				this.hubConnection = new HubConnection ("http://localhost:9002/");
				this.hub = hubConnection.CreateHubProxy ("StatusBarHub");


				//Register Hub Listener
				this.hub.On ("SetUserConnectionId", (string u, string c) => this.SetUserConnectionId (u, c));
				this.hub.On ("FlushConnectionId", c => RemoveUserConnectionIdWithLock (c));
				this.hub.On ("RemoveFromMyBar", (string u, string id) => RemoveFromAllBar (u,id));
				this.hub.On ("AddToMyBar", (string u, string t,string s, string id) => AddToAllBar (u,t,s,id));

				this.hubConnection.Start ().Wait ();

				this.setupLock = new ReaderWriterLockWrapper ();
				this.cacheLock = new ReaderWriterLockWrapper ();
			}

		}

		public static StatusBarClient Instance
		{
			get
			{
				return instance;
			}
		}

		public static StatusBarClient Create(CoreServer server)
		{
			if (instance == null)
			{
				instance = new StatusBarClient (server);
			}
			return instance;
		}

		#region IEntityBagHub Members

		void IStatusBarHub.AddToBar(string userName, string title, FormattedText summary, string entityId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity (userName, "ADD", title, summary, entityId));
				}

			}
			else
			{

				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
				context.Clients.All.AddToBar (title, summary, entityId);
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity (userName, "ADD", title, summary, entityId));
				}
			}
		}

		void IStatusBarHub.RemoveFromBar(string userName, string entityId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity (userName, "REMOVE", null, FormattedText.Null, entityId));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
				context.Clients.All.RemoveFromBar (entityId);
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.RemoveAll(e => e.EntityId == entityId);
				}
			}
		}

		void IStatusBarHub.SetLoading(string userName, bool state)
		{			
			var context = GlobalHost.ConnectionManager.GetHubContext<EntityBagHub> ();
			context.Clients.Group (userName).SetLoading (state);			
		}

		IEnumerable<string> IStatusBarHub.GetStatusEntitiesId()
		{
			return this.statusEntityCache.Select(b => "db:" + b.EntityId.Replace('-',':'));
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
				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
				using (this.setupLock.LockWrite ())
				{

					if (!this.hubClients.Exists (c => c.Id == connectionId))
					{
						this.hubClients.Add (new HubClient (connectionId, userName));
						context.Groups.Add (connectionId, userName);
						//send and flush pending user notification from queue
						using (this.cacheLock.LockRead ())
						{
							foreach (var bagEntity in this.statusEntityCache)
							{
								switch (bagEntity.Action)
								{
									case "ADD":
										context.Clients.All.AddToBar (bagEntity.Title,bagEntity.Summary,bagEntity.EntityId);
										break;
									case "REMOVE":
										context.Clients.All.RemoveFromBar (bagEntity.EntityId);
										break;
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

		private void RemoveFromAllBar(string userName,string entityId)
		{
			using (this.cacheLock.LockWrite ())
			{
				this.statusEntityCache.RemoveAll (e => e.EntityId == entityId);
			}

			var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
			context.Clients.All.RemoveFromBag (entityId);
		}

		private void AddToAllBar(string userName, string title,string clientSummary, string entityId)
		{
			var entity = this.server.CoreWorkerPool.Execute (userName, null, (b) => EntityIO.ResolveEntity (b, entityId));
			if (entity.IsNotNull ())
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();

				try //with GetSummary() on from AbstractEntity
				{
					var summary = entity.GetSummary ();

					context.Clients.All.AddToBag (title, summary, entityId);
					using (this.cacheLock.LockWrite ())
					{
						this.statusEntityCache.Add (new StatusEntity (userName, "ADD", title, summary, entityId));
					}
				}
				catch //use client summary instead
				{
					context.Clients.All.AddToBag (title, clientSummary, entityId);
					using (this.cacheLock.LockWrite ())
					{
						this.statusEntityCache.Add (new StatusEntity (userName, "ADD", title, clientSummary, entityId));
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


		private sealed class StatusEntity
		{
			public StatusEntity(string userName, string action, string title, FormattedText summary, string entityId)
			{
				this.Action = action;
				this.Title = title;
				this.Summary = summary;
				this.EntityId = entityId;
			}

			public string						Action;
			public string						Title;
			public FormattedText				Summary;
			public string						EntityId;
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


		private static StatusBarClient		instance;

		private readonly ReaderWriterLockWrapper setupLock;
		private readonly ReaderWriterLockWrapper cacheLock;

		private readonly CoreServer				server;
		private readonly HubConnection			hubConnection;
		private readonly IHubProxy				hub;
		private readonly List<HubClient>		hubClients;
		private readonly List<StatusEntity>		statusEntityCache;

		
	}
}
