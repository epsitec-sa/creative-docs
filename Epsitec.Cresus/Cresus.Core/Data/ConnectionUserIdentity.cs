//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public class ConnectionUserIdentity : System.IEquatable<ConnectionUserIdentity>
	{
		public ConnectionUserIdentity(ItemCode code)
		{
			var userCode    = (string) code;
			var userName    = System.Environment.UserName;
			var machineName = System.Environment.MachineName;
			var osVersion   = System.Environment.OSVersion.VersionString;
			var clrVersion  = System.Environment.Version.ToString ();
			var coreVersion = typeof (CoreData).Assembly.GetVersionString ();

			this.data = string.Concat (userCode, " ", userName, "@", machineName, "/OS={", osVersion, "}/CLR={", clrVersion, "}/Core={", coreVersion, "}");
		}

		private ConnectionUserIdentity(string data)
		{
			this.data = data;
		}

		
		public ItemCode							UserCode
		{
			get
			{
				int pos = this.data.IndexOf (' ');
				return new ItemCode (this.data.Substring (0, pos));
			}
		}

		public string							SystemUserName
		{
			get
			{
				return this.data.Split ('/')[0].Split (' ')[1].Split ('@')[0];
			}
		}

		public string							SystemMachineName
		{
			get
			{
				return this.data.Split ('/')[0].Split (' ')[1].Split ('@')[1];
			}
		}


		public static ConnectionUserIdentity Parse(string value)
		{
			if ((string.IsNullOrEmpty (value)) ||
				(value.Contains (" ") == false) ||
				(value.Contains ("@") == false) ||
				(value.Contains ("/") == false))
            {
				throw new System.FormatException ("Invalid connection user identity format");
            }

			return new ConnectionUserIdentity (value);
		}

		public IEnumerable<System.Tuple<string, string>> GetInfos()
		{
			string args1 = this.data.Substring (this.data.IndexOf (' ') + 1);
			string args2 = args1.Substring (args1.IndexOf ('/')+1);

			string[] vector = args2.Split ("}/");

			foreach (var item in vector)
			{
				var keyValue = item.Split ("={");
				yield return new System.Tuple<string, string> (keyValue[0], keyValue[1]);
			}
		}
		
		
		public override string ToString()
		{
			return this.data;
		}

		public override bool Equals(object obj)
		{
			if (obj is ConnectionUserIdentity)
			{
				return this.Equals ((ConnectionUserIdentity) obj);
			}
			else
			{
				return false;
			}
		}

		#region IEquatable<DataConnectionUserIdentity> Members

		public bool Equals(ConnectionUserIdentity other)
		{
			return (other != null) && (other.data == this.data);
		}

		#endregion

		private readonly string data;
	}
}
