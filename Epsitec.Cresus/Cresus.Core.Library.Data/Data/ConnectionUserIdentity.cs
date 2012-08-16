//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>ConnectionUserIdentity</c> class represents the user identity attached to a
	/// database connection.
	/// </summary>
	public sealed class ConnectionUserIdentity : System.IEquatable<ConnectionUserIdentity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionUserIdentity"/> class
		/// based on the user's <see cref="SoftwareUserEntity.Code"/>.
		/// </summary>
		/// <param name="code">The user's <see cref="SoftwareUserEntity.Code"/>.</param>
		public ConnectionUserIdentity(ItemCode code)
		{
			var userCode    = (string) code;
			var userName    = System.Environment.UserName;
			var machineName = System.Environment.MachineName;
			var osVersion   = System.Environment.OSVersion.VersionString;
			var clrVersion  = System.Environment.Version.ToString ();
			var coreVersion = typeof (CoreData).Assembly.GetVersionString ();
			var processId   = System.Diagnostics.Process.GetCurrentProcess ().Id.ToString (System.Globalization.CultureInfo.InvariantCulture);

			this.data = string.Concat (userCode, " ", userName, "@", machineName,
				/**/				   "/pid={", processId, "}",
				/**/				   "/OS={", osVersion, "}",
				/**/				   "/CLR={", clrVersion, "}",
				/**/				   "/Core={", coreVersion, "}");
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
				//	"code user@machine/..."
				//	      ^^^^
				return this.data.Split (' ')[1].Split ('/')[0].Split ('@')[0];
			}
		}

		public string							SystemMachineName
		{
			get
			{
				//	"code user@machine/..."
				//	           ^^^^^^^
				return this.data.Split (' ')[1].Split ('/')[0].Split ('@')[1];
			}
		}


		/// <summary>
		/// Parses the serialized value of the connection user identity.
		/// </summary>
		/// <param name="value">The serialized value.</param>
		/// <returns>The connection user identity.</returns>
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

		/// <summary>
		/// Gets the low level informations associated with the connection (such as <c>OS</c>, <c>CLR</c>, <c>Core</c>).
		/// </summary>
		/// <returns>The collection of low level informations.</returns>
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

		public override int GetHashCode()
		{
			return this.data.GetHashCode ();
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
