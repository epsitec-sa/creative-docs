//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	/// <summary>
	/// The <c>UserCommandSettings</c> class stores enable/disable information related
	/// to a command for a given user/group/role.
	/// </summary>
	public sealed class UserCommandSettings : System.IEquatable<UserCommandSettings>, IUserIdentity
	{
		public UserCommandSettings(Druid commandId)
		{
			this.commandId = commandId;
		}

		
		public UserCategory						UserCategory
		{
			get;
			set;
		}

		public string							UserIdentity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the command id (which is not serialized as part of the settings).
		/// </summary>
		/// <value>
		/// The command id.
		/// </value>
		public Druid							CommandId
		{
			get
			{
				return this.commandId;
			}
		}

		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				this.GetXmlAttributes ());
		}

		public static UserCommandSettings Restore(Druid commandId, XElement xml)
		{
			var userCategory = (int?)   xml.Attribute (Xml.UserCategory);
			var userIdentity = (string) xml.Attribute (Xml.UserIdentity);

			return new UserCommandSettings (commandId)
			{
				UserCategory = (UserCategory) userCategory.GetValueOrDefault (),
				UserIdentity = userIdentity,
			};
		}


		public override int GetHashCode()
		{
			//	Use a partial hash code here, since this will usually not be used, it
			//	is fully sufficient:

			return this.commandId.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as UserCommandSettings);
		}

		#region IEquatable<UserCommandSettings> Members

		public bool Equals(UserCommandSettings other)
		{
			if (other == null)
			{
				return false;
			}

			return this.UserCategory == other.UserCategory
				&& this.UserIdentity == other.UserIdentity
				&& this.CommandId == other.CommandId;
		}

		#endregion

		private IEnumerable<XAttribute> GetXmlAttributes()
		{
			if (this.UserCategory != UserCategory.Any)
			{
				yield return new XAttribute (Xml.UserCategory, (int) this.UserCategory);
			}
			if (!string.IsNullOrEmpty (this.UserIdentity))
			{
				yield return new XAttribute (Xml.UserIdentity, this.UserIdentity);
			}
		}


		private static class Xml
		{
			public const string UserCategory = "cat";
			public const string UserIdentity = "uid";
		}

		private readonly Druid commandId;
	}
}
