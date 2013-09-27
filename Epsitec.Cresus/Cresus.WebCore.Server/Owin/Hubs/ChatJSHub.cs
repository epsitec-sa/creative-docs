//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class ChatJSHub : Hub
	{
		public static List<ChatUser> Users
		{
			get
			{
				return ChatJSHub.users;
			}
		}

		public static List<ChatMessage> Messages
		{
			get
			{
				return ChatJSHub.messages;
			}
		}
		
		public ChatUser GetUserInfo(string connectionId)
		{
			lock (ChatJSHub.Users)
			{
				return ChatJSHub.Users.FirstOrDefault (u => u.Id == connectionId);
			}
		}

		public void UpdateMyInfo(string username,string email)
		{
			lock (ChatJSHub.Users)
			{
				var toUpdate = ChatJSHub.Users.FirstOrDefault (u => u.Id == this.Context.ConnectionId);
				toUpdate.Name = username;
				toUpdate.ProfilePictureUrl = GravatarHelper.GetGravatarUrl (GravatarHelper.GetGravatarHash (email), GravatarHelper.GravatarSize.s32);
			}
			
			this.BroadcastUsersList();
		}

		private void BroadcastUsersList()
		{
			this.Clients.All.usersListChanged (ChatJSHub.Users);
		}

		public List<ChatMessage> GetMessageHistory(string otherConnectionId)
		{
			var messages = ChatJSHub.Messages
							   .Where(
								   m =>
								   (m.UserTo== this.GetUserInfo(this.Context.ConnectionId) && m.UserFrom == this.GetUserInfo(otherConnectionId)) ||
								   (m.UserTo == this.GetUserInfo(otherConnectionId) && m.UserFrom == this.GetUserInfo(this.Context.ConnectionId)))
							   .OrderByDescending(m => m.Timestamp).Take(30).ToList();
			return messages;
		}

		public void SendMessage(string otherConnectionId, string message, string clientGuid)
		{
			var chatMessage = new ChatMessage()
			{
				Message = message,
				UserFrom = this.GetUserInfo(this.Context.ConnectionId),
				UserTo = this.GetUserInfo (otherConnectionId),
				ClientGuid = clientGuid
			};

			lock (ChatJSHub.Messages)
			{
				ChatJSHub.Messages.Add (chatMessage);
			}
			

			this.Clients.Client (otherConnectionId).sendMessage (chatMessage);
		}

		public void SendTypingSignal(string otherConnectionId)
		{
			this.Clients.Client (otherConnectionId).sendTypingSignal (this.GetUserInfo (this.Context.ConnectionId));
		}

		public override Task OnConnected()
		{
			lock (ChatJSHub.Users)
			{
				ChatJSHub.Users.Add (new ChatUser ()
				{
					Id = this.Context.ConnectionId,
					Name = this.Context.ConnectionId,
					Status = ChatUser.StatusType.Online,
					ProfilePictureUrl = GravatarHelper.GetGravatarUrl (GravatarHelper.GetGravatarHash (""), GravatarHelper.GravatarSize.s32)
				});
			}
	
			this.BroadcastUsersList();

			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			lock (ChatJSHub.Users)
			{
				ChatJSHub.Users.RemoveAll (u => u.Id == this.Context.ConnectionId);
			}
			this.BroadcastUsersList();
			return base.OnDisconnected();
		}

		public override Task OnReconnected()
		{
			lock (ChatJSHub.Users)
			{
				ChatJSHub.Users.RemoveAll (u => u.Id == this.Context.ConnectionId);
			}
			this.BroadcastUsersList();
			return base.OnReconnected();
		}

		public const string ROOM_ID_STUB = "aider-room";

		private static readonly List<ChatUser>		users		= new List<ChatUser> ();
		private static readonly List<ChatMessage>	messages	= new List<ChatMessage> ();
	}

	public class ChatMessage
	{

		public ChatUser UserFrom { get; set; }

		public ChatUser UserTo { get; set; }

		public long Timestamp { get; set; }

		public string Message { get; set; }

		/// <summary>
		/// Client GUID
		/// </summary>
		/// <remarks>
		/// Every time a message is sent from the client, the client must specify an unique message client GUID. This is
		/// because when you send a message to the server, the message comes back to the client. This is useful for 2 reasons:
		/// 1) It allows the client to know that probably the other user received the message
		/// 2) It allows for different browser windows to be synchronized
		/// </remarks>
		public string ClientGuid { get; set; }
	}

	public class ChatUser
	{
		public enum StatusType
		{
			Offline = 0,
			Online = 1
		}

		public ChatUser()
		{
			this.Status = StatusType.Offline;
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string ProfilePictureUrl { get; set; }

		public StatusType Status { get; set; }
	}

	public class GravatarHelper
	{
		public static string GetGravatarUrl(string gravatarEMailHash, GravatarSize size)
		{
			string sizeAsString = new string (size.ToString ().Where (x => char.IsDigit (x)).ToArray ());
			
			return "https://www.gravatar.com/avatar/" + gravatarEMailHash + "?s=" + sizeAsString + GravatarHelper.Ampersand + "r=PG&d=mm";
		}

		public static string GetGravatarHash(string email)
		{
			var md5Hasher  = System.Security.Cryptography.MD5.Create ();
			var emailBytes = System.Text.Encoding.Default.GetBytes (email ?? GravatarHelper.DefaultEmail);
			var emailHash  = md5Hasher.ComputeHash (emailBytes);

			return string.Join ("", emailHash.Select (x => x.ToString ("x2")));
		}

		public const string DefaultEmail = "meu@email.com";
		public const string Ampersand = "&";
		public const string BadgeSymbol = "&#9679;";

		/// <summary>
		/// Gravatar image size
		/// </summary>
		public enum GravatarSize
		{
			s16,
			s24,
			s32,
			s64,
			s128,
		}
	}
}
