using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using System.Windows.Navigation;

namespace App.Directories
{
	[DataContract]
	public class AccessToken
	{
		[DataMember (Name = "access_token")]
		public string Token
		{
			private set;
			get;
		}
		[DataMember (Name = "token_type")]
		public string TokenType
		{
			private set;
			get;
		}
		[DataMember (Name = "expires_in")]
		public int ExpiresIn
		{
			private set;
			get;
		}
		[DataMember (Name = "refresh_token")]
		public string RefreshToken
		{
			private set;
			get;
		}
		public DateTime TokenDate
		{
			set;
			get;
		}
	}
}
