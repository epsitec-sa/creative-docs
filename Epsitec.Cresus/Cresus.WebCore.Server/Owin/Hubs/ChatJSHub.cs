using System;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Cryptography;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
    public class ChatJSHub : Hub
    {
        public const string ROOM_ID_STUB = "aider-room";

		public static List<ChatUser> Users = new List<ChatUser> ();
		public static List<ChatMessage> Messages = new List<ChatMessage> ();


		public ChatUser GetUserInfo(string connectionId)
        {
			return ChatJSHub.Users.FirstOrDefault (u => u.Id == connectionId);
        }

		public void UpdateMyInfo(string username,string email)
		{
			lock(ChatJSHub.Users)
			{
				var toUpdate = ChatJSHub.Users.FirstOrDefault (u => u.Id == this.Context.ConnectionId);
				toUpdate.Name = username;
				toUpdate.ProfilePictureUrl = GravatarHelper.GetGravatarUrl (GravatarHelper.GetGravatarHash (email), GravatarHelper.Size.s32);
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
					ProfilePictureUrl = GravatarHelper.GetGravatarUrl (GravatarHelper.GetGravatarHash (""), GravatarHelper.Size.s32)
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
        public const string Ampersand = "&";
        public const string BadgeSymbol = "&#9679;";

        public static String GetGravatarUrl(String gravatarEMailHash, Size size)
        {
            string sizeAsString;
            // this code CAN BE BETTER. I'm jot not feeling like fixing it right now
            switch (size)
            {
                case Size.s16:
                    sizeAsString = "16";
                    break;
                case Size.s24:
                    sizeAsString = "24";
                    break;
                case Size.s32:
                    sizeAsString = "32";
                    break;
                case Size.s64:
                    sizeAsString = "64";
                    break;
                case Size.s128:
                    sizeAsString = "128";
                    break;
                default:
                    throw new Exception("Size not supported");
            }

            return "https://www.gravatar.com/avatar/" + gravatarEMailHash + "?s=" + sizeAsString + GravatarHelper.Ampersand + "r=PG&d=mm";
        }

        // Create an md5 sum string of this string
        public static string GetGravatarHash(string email)
        {
            if (String.IsNullOrEmpty(email))
                email = "meu@email.com";

            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();  // Return the hexadecimal string.
        }

        /// <summary>
        /// Gravatar image size
        /// </summary>
        public enum Size
        {
            s16,
            s24,
            s32,
            s64,
            s128
        }
    }
}
