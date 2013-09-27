//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Chat
{
	public class ChatUser
	{
		public ChatUser()
		{
			this.Status = ChatUserStatusType.Offline;
		}

		public string							Id
		{
			get;
			set;
		}

		public string							Name
		{
			get;
			set;
		}

		public string							Url
		{
			get;
			set;
		}

		public string							ProfilePictureUrl
		{
			get;
			set;
		}

		public ChatUserStatusType				Status
		{
			get;
			set;
		}
	}
}

