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
        /// <summary>
        /// This STUB. In a normal situation, there would be multiple rooms and the user room would have to be 
        /// determined by the user profile
        /// </summary>
        public const string ROOM_ID_STUB = "chatjs-room";

        /// <summary>
        /// Current connections
        /// 1 room has many users that have many connections (2 open browsers from the same user represents 2 connections)
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, List<string>>> connections = new Dictionary<string, Dictionary<string, List<string>>>();

        /// <summary>
        /// This is STUB. This will SIMULATE a database of chat messages
        /// </summary>
        private static readonly List<DbChatMessageStub> dbChatMessagesStub = new List<DbChatMessageStub>();

        /// <summary>
        /// This method is STUB. This will SIMULATE a database of users
        /// </summary>
        private static readonly List<DbUserStub> dbUsersStub = new List<DbUserStub>();

        /// <summary>
        /// This method is STUB. In a normal situation, the user info would come from the database so this method wouldn't be necessary.
        /// It's only necessary because this class is simulating the database
        /// </summary>
        /// <param name="newUser"></param>
        public static void RegisterNewUser(DbUserStub newUser)
        {
            if (newUser == null) throw new ArgumentNullException("newUser");
            dbUsersStub.Add(newUser);
        }

        /// <summary>
        /// This method is STUB. Returns if a user is registered in the FAKE DB.
        /// Normally this wouldn't be necessary.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsUserRegisteredInDbUsersStub(DbUserStub user)
        {
            return dbUsersStub.Any(u => u.Id == user.Id);
        }

        /// <summary>
        /// Tries to find a user with the provided e-mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static DbUserStub FindUserByEmail(string email)
        {
            if (email == null) return null;
            return dbUsersStub.FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// If the specified user is connected, return information about the user
        /// </summary>
        public ChatUser GetUserInfo(string userId)
        {
            var user = dbUsersStub.FirstOrDefault(u => u.Id == userId);
            return user == null ? null : GetChatUserFromDbUserId(userId);
        }

        private ChatUser GetChatUserFromDbUserId(string dbUserId)
        {
            var myRoomId = this.GetMyRoomId();

            // this is STUB. Normally you would go to the database get the real user
            var dbUser = dbUsersStub.First(u => u.Id == dbUserId);

            ChatUser.StatusType userStatus;
            lock (connections)
            {
                userStatus = connections.ContainsKey(myRoomId)
                                 ? (connections[myRoomId].ContainsKey(dbUser.Id)
                                        ? ChatUser.StatusType.Online
                                        : ChatUser.StatusType.Offline)
                                 : ChatUser.StatusType.Offline;
            }
            return new ChatUser()
            {
                Id = dbUser.Id,
                Name = dbUser.FullName,
                Status = userStatus,
                ProfilePictureUrl = GravatarHelper.GetGravatarUrl(GravatarHelper.GetGravatarHash(dbUser.Email), GravatarHelper.Size.s32)
            };
        }

        private ChatMessage GetChatMessage(DbChatMessageStub chatMessage, string clientGuid)
        {
            return new ChatMessage()
            {
                Message = chatMessage.Message,
                UserFrom = this.GetChatUserFromDbUserId(chatMessage.UserFromId),
                UserTo = this.GetChatUserFromDbUserId(chatMessage.UserToId),
                ClientGuid = clientGuid
            };
        }

        /// <summary>
        /// Returns my user id
        /// </summary>
        /// <returns></returns>
        private string GetMyUserId()
        {
            return this.Context.ConnectionId;
        }

        private string GetMyRoomId()
        {
            // This would normally be done like this:
            //var userPrincipal = this.Context.User as AuthenticatedPrincipal;
            //if (userPrincipal == null)
            //    throw new NotAuthorizedException();

            //var userData = userPrincipal.Profile;
            //return userData.MyTenancyIdentifier;

            // But for this example, it will always return "chatjs-room", because we have only one room.
            return ROOM_ID_STUB;
        }

        /// <summary>
        /// Broadcasts to all users in the same room the new users list
        /// </summary>
        private void BroadcastUsersList()
        {
            var myRoomId = this.GetMyRoomId();
            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections.ContainsKey(myRoomId))
                    connectionIds = connections[myRoomId].Keys.SelectMany(userId => connections[myRoomId][userId]).ToList();
            }

            // gets the current room user's list

            // this is STUB. You would normally go to the database to get the real room users
            var dbRoomUsers = dbUsersStub.Where(u => u.TenancyId == myRoomId).OrderBy(u => u.FullName).ToList();
            var usersList = dbRoomUsers.Select(u => this.GetChatUserFromDbUserId(u.Id)).ToList();

            foreach (var connectionId in connectionIds)
                this.Clients.Client(connectionId).usersListChanged(usersList);
        }

        private DbChatMessageStub PersistMessage(string otherUserId, string message)
        {
            var myUserId = this.GetMyUserId();

            // this is STUB. Normally you would go to the real database to get the my user and the other user
            var myUser = dbUsersStub.FirstOrDefault(u => u.Id == myUserId);
            var otherUser = dbUsersStub.FirstOrDefault(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return null;

            var dbChatMessage = new DbChatMessageStub()
            {
                Date = DateTime.UtcNow,
                Message = message,
                UserFromId = myUserId,
                UserToId = otherUserId,
                TenancyId = myUser.TenancyId
            };

            // this is STUB. Normally you would add the dbMessage to the real database
            dbChatMessagesStub.Add(dbChatMessage);

            // normally you would save the database changes
            //this.db.SaveChanges();

            return dbChatMessage;
        }

        /// <summary>
        /// Returns the message history
        /// </summary>
        public List<ChatMessage> GetMessageHistory(int otherUserId)
        {
            var myUserId = this.GetMyUserId();
            // this is STUB. Normally you would go to the real database to get the messages
            var dbMessages = dbChatMessagesStub
                               .Where(
                                   m =>
                                   (m.UserToId == myUserId && m.UserFromId.Equals(otherUserId)) ||
                                   (m.UserToId.Equals(otherUserId) && m.UserFromId == myUserId))
                               .OrderByDescending(m => m.Date).Take(30).ToList();

            dbMessages.Reverse();
            return dbMessages.Select(m => this.GetChatMessage(m, null)).ToList();
        }

        /// <summary>
        /// Sends a message to a particular user
        /// </summary>
        public void SendMessage(string otherUserId, string message, string clientGuid)
        {
            var myUserId = this.GetMyUserId();
            var myRoomId = this.GetMyRoomId();


            var dbChatMessage = PersistMessage(otherUserId, message);
            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections[myRoomId].ContainsKey(otherUserId))
                    connectionIds.AddRange(connections[myRoomId][otherUserId]);
                if (connections[myRoomId].ContainsKey(myUserId))
                    connectionIds.AddRange(connections[myRoomId][myUserId]);
            }
            foreach (var connectionId in connectionIds)
                this.Clients.Client(connectionId).sendMessage(this.GetChatMessage(dbChatMessage, clientGuid));
        }


        public void RegisterMe(string userName)
        {
            var dbUser = new DbUserStub()
            {
                FullName = userName,
                Email = "",
                Id = this.Context.ConnectionId,
                TenancyId = ROOM_ID_STUB
            };
            ChatJSHub.RegisterNewUser(dbUser);
            this.BroadcastUsersList();
        }

        /// <summary>
        /// Sends a typing signal to a particular user
        /// </summary>
        public void SendTypingSignal(string otherUserId)
        {
            var myUserId = this.GetMyUserId();
            var myRoomId = this.GetMyRoomId();

            var connectionIds = new List<string>();
            lock (connections)
            {
                if (connections[myRoomId].ContainsKey(otherUserId))
                    connectionIds.AddRange(connections[myRoomId][otherUserId]);
            }
            foreach (var connectionId in connectionIds)
                this.Clients.Client(connectionId).sendTypingSignal(this.GetUserInfo(myUserId));
        }

        public override Task OnConnected()
        {
            var myRoomId = this.GetMyRoomId();
            var myUserId = this.GetMyUserId();

            lock (connections)
            {
                if (!connections.ContainsKey(myRoomId))
                    connections[myRoomId] = new Dictionary<string, List<string>>();

                if (!connections[myRoomId].ContainsKey(myUserId))
                    connections[myRoomId][myUserId] = new List<string>();

                connections[myRoomId][myUserId].Add(this.Context.ConnectionId);
            }

            this.BroadcastUsersList();

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            var myRoomId = this.GetMyRoomId();
            var myUserId = this.GetMyUserId();

            lock (connections)
            {
                if (connections.ContainsKey(myRoomId))
                    if (connections[myRoomId].ContainsKey(myUserId))
                        if (connections[myRoomId][myUserId].Contains(this.Context.ConnectionId))
                        {
                            connections[myRoomId][myUserId].Remove(this.Context.ConnectionId);
                            if (!connections[myRoomId][myUserId].Any())
                            {
                                connections[myRoomId].Remove(myUserId);                              
                            }
                        }
            }
            dbUsersStub.RemoveAll(u => u.Id == this.Context.ConnectionId);
            this.BroadcastUsersList();
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            dbUsersStub.RemoveAll(u => u.Id == this.Context.ConnectionId);
            this.BroadcastUsersList();
            return base.OnReconnected();
        }
    }

    public class DbChatMessageStub : ChatMessage
    {
        public DateTime Date { get; set; }

        public string UserFromId { get; set; }

        public string UserToId { get; set; }

        public string TenancyId { get; set; }
    }

    public class ChatMessage
    {
        /// <summary>
        /// The user that sent the message
        /// </summary>
        public ChatUser UserFrom { get; set; }

        /// <summary>
        /// The user to whom the message is to
        /// </summary>
        public ChatUser UserTo { get; set; }

        /// <summary>
        /// Message timestamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
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

    public class DbUserStub
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string TenancyId { get; set; }
    }

    /// <summary>
    /// Information about a chat user
    /// </summary>
    public class ChatUser
    {
        /// <summary>
        /// User chat status. For now, it only supports online and offline
        /// </summary>
        public enum StatusType
        {
            Offline = 0,
            Online = 1
        }

        public ChatUser()
        {
            this.Status = StatusType.Offline;
        }

        /// <summary>
        /// User Id (preferebly the same as database user Id)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Profile Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// User profile picture URL (Gravatar, for instance)
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// User's status
        /// </summary>
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
