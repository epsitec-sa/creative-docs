//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Chat
{
	public class ChatMessage
	{
		public ChatUser							UserFrom
		{
			get;
			set;
		}

		public ChatUser							UserTo
		{
			get;
			set;
		}

		public long								Timestamp
		{
			get;
			set;
		}

		public string							Message
		{
			get;
			set;
		}

		/// <summary>
		/// Client GUID
		/// </summary>
		/// <remarks>
		/// Every time a message is sent from the client, the client must specify an unique message client GUID. This is
		/// because when you send a message to the server, the message comes back to the client. This is useful for 2 reasons:
		/// 1) It allows the client to know that probably the other user received the message
		/// 2) It allows for different browser windows to be synchronized
		/// </remarks>
		public string							ClientGuid
		{
			get;
			set;
		}
	}
}

