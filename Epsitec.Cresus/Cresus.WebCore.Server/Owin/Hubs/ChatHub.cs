//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.WebCore.Server.Chat;
using Epsitec.Cresus.WebCore.Server.Gravatar;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class ChatHub : Hub
	{
		public static List<ChatUser>			Users
		{
			get
			{
				return ChatHub.users;
			}
		}

		public static List<ChatMessage>			Messages
		{
			get
			{
				return ChatHub.messages;
			}
		}

		
		public ChatUser FindUserInfo()
		{
			return this.FindUserInfo (this.Context.ConnectionId);
		}

		public ChatUser FindUserInfo(string connectionId)
		{
			lock (ChatHub.Users)
			{
				return ChatHub.Users.FirstOrDefault (u => u.Id == connectionId);
			}
		}

		
		public void UpdateUserInfo(string username, string email)
		{
			//	Called by Hubs.js
			var picture  = GravatarHelper.GetGravatarUrl (email, GravatarImageSize.Size32);
			var userInfo = this.FindUserInfo ();

			if (userInfo == null)
			{
				return;
			}

			userInfo.Name              = username;
			userInfo.ProfilePictureUrl = picture;

			this.BroadcastUsersList ();
			this.Clients.Caller.setMyUserInfo (userInfo);
		}

		public List<ChatMessage> GetMessageHistory(string otherConnectionId)
		{

			var callerUserInfo = this.FindUserInfo ();
			var otherUserInfo = this.FindUserInfo (otherConnectionId);
			lock (ChatHub.Messages)
			{
				var messages = ChatHub.Messages
								   .Where (
									   m =>
									   (m.UserTo == callerUserInfo && m.UserFrom == otherUserInfo) ||
									   (m.UserTo == otherUserInfo && m.UserFrom == callerUserInfo))
								   .OrderByDescending (m => m.Timestamp).Take (30).ToList ();

				return messages;
			}
		
		}

		public void SendMessage(string otherConnectionId, string message, string clientGuid)
		{
			var chatMessage = new ChatMessage ()
			{
				Message    = message,
				UserFrom   = this.FindUserInfo (),
				UserTo     = this.FindUserInfo (otherConnectionId),
				ClientGuid = clientGuid
			};

			lock (ChatHub.Messages)
			{
				ChatHub.Messages.Add (chatMessage);
			}

			this.Clients.Client (otherConnectionId).sendMessage (chatMessage);
		}

		public void SendTypingSignal(string otherConnectionId)
		{
			this.Clients.Client (otherConnectionId).sendTypingSignal (this.FindUserInfo ());
		}

		
		public override Task OnConnected()
		{
			var chatUser = new ChatUser ()
			{
				Id     = this.Context.ConnectionId,
				Name   = this.Context.ConnectionId,
				Status = ChatUserStatusType.Online,
				
				ProfilePictureUrl = GravatarHelper.GetGravatarUrl (null, GravatarImageSize.Size32)
			};

			lock (ChatHub.Users)
			{
				ChatHub.Users.Add (chatUser);
			}

			this.BroadcastUsersList ();

			return base.OnConnected ();
		}

		public override Task OnDisconnected()
		{
			lock (ChatHub.Users)
			{
				ChatHub.Users.RemoveAll (u => u.Id == this.Context.ConnectionId);
			}

			this.BroadcastUsersList ();
			
			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			lock (ChatHub.Users)
			{
				ChatHub.Users.RemoveAll (u => u.Id == this.Context.ConnectionId);
			}
			
			this.BroadcastUsersList ();
			
			return base.OnReconnected ();
		}


		private void BroadcastUsersList()
		{
			this.Clients.All.usersListChanged (ChatHub.Users);
		}

		
		private static readonly List<ChatUser>		users		= new List<ChatUser> ();
		private static readonly List<ChatMessage>	messages	= new List<ChatMessage> ();
	}
}