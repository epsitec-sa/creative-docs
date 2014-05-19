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

		void IStatusBarHub.AddToBar(string type, string text,string iconClass, string statusId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity ("ADD", type, text,iconClass, statusId));
				}

			}
			else
			{

				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
				context.Clients.All.AddToBar (type,text,iconClass,statusId);
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity ("ADD", type, text, iconClass, statusId));
				}
			}
		}

		void IStatusBarHub.RemoveFromBar(string statusId, When when)
		{
			if (when == When.OnConnect)
			{
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.Add (new StatusEntity ("REMOVE", "","","", statusId));
				}

			}
			else
			{
				var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
				context.Clients.All.RemoveFromBar (statusId);
				using (this.cacheLock.LockWrite ())
				{
					this.statusEntityCache.RemoveAll(e => e.StatusId == statusId);
				}
			}
		}

		IEnumerable<string> IStatusBarHub.GetStatusEntitiesId()
		{
			return this.statusEntityCache.Select(s => s.StatusId);
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
							foreach (var status in this.statusEntityCache)
							{
								switch (status.Action)
								{
									case "ADD":
										context.Clients.All.AddToBar (status.Type,status.Text,status.IconClass, status.StatusId);
										break;
									case "REMOVE":
										context.Clients.All.RemoveFromBar (status.StatusId);
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

		private void RemoveFromAllBar(string statusId)
		{
			using (this.cacheLock.LockWrite ())
			{
				this.statusEntityCache.RemoveAll (e => e.StatusId == statusId);
			}

			var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();
			context.Clients.All.RemoveFromBag (statusId);
		}

		private void AddToAllBar(string type,string text, string iconClass, string statusId)
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<StatusBarHub> ();			
			context.Clients.All.AddToBag (type, text,iconClass, statusId);
			using (this.cacheLock.LockWrite ())
			{
				this.statusEntityCache.Add (new StatusEntity ("ADD", type, text,iconClass, statusId));
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
			public StatusEntity(string action, string type, string text, string iconClass, string statusId)
			{
				this.Action = action;
				this.Type = type;
				this.Text = text;
				this.IconClass = iconClass;
				this.StatusId = statusId;
			}

			public string						Action;
			public string						Type;
			public string						Text;
			public string						IconClass;
			public string						StatusId;
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
