//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserFieldEditionSettings : System.IEquatable<UserFieldEditionSettings>
	{
		public UserFieldEditionSettings(Druid fieldId)
		{
			this.fieldId = fieldId;
		}

		
		public TileUserCategory					UserCategory
		{
			get;
			set;
		}

		public string							UserIdentity
		{
			get;
			set;
		}

		public TileFieldEditionSettings			FieldSettings
		{
			get;
			set;
		}

		public MergeSettingsMode				MergeSettingsMode
		{
			get;
			set;
		}

		public Druid							FieldId
		{
			get
			{
				return this.fieldId;
			}
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				this.GetXmlAttributes (),
				this.FieldSettings.Save (Xml.FieldSettings));
		}

		public static UserFieldEditionSettings Restore(Druid fieldId, XElement xml)
		{
			var userCategory = (int?)   xml.Attribute (Xml.UserCategory);
			var userIdentity = (string) xml.Attribute (Xml.UserIdentity);
			var settingsMode = (int?)   xml.Attribute (Xml.MergeSettingsMode);
			var field        = TileFieldEditionSettings.Restore (xml.Element (Xml.FieldSettings));

			return new UserFieldEditionSettings (fieldId)
			{
				UserCategory = (TileUserCategory) userCategory.GetValueOrDefault (),
				UserIdentity = userIdentity,
				MergeSettingsMode = (MergeSettingsMode) settingsMode.GetValueOrDefault (),
				FieldSettings = field
			};
		}


		public static TileFieldEditionSettings Combine(TileFieldEditionSettings a, TileFieldEditionSettings b, MergeSettingsMode mode)
		{
			switch (mode)
			{
				case MergeSettingsMode.Inclusive:	return a + b;
				case MergeSettingsMode.Exclusive:	return a - b;
				case MergeSettingsMode.Override:	return b;

				default:
					throw new System.NotSupportedException (string.Format ("Mode {0} not supported", mode));
			}
		}


		public override bool Equals(object obj)
		{
			return this.Equals (obj as UserFieldEditionSettings);
		}

		#region IEquatable<UserFieldEditionSettings> Members

		public bool Equals(UserFieldEditionSettings other)
		{
			if (other == null)
			{
				return false;
			}

			return this.UserCategory == other.UserCategory
				&& this.UserIdentity == other.UserIdentity
				&& this.FieldSettings.Equals (other.FieldSettings)
				&& this.MergeSettingsMode == other.MergeSettingsMode
				&& this.FieldId == other.FieldId;
		}

		#endregion
		
		private IEnumerable<XAttribute> GetXmlAttributes()
		{
			if (this.UserCategory != TileUserCategory.Any)
			{
				yield return new XAttribute (Xml.UserCategory, (int) this.UserCategory);
			}
			if (!string.IsNullOrEmpty (this.UserIdentity))
			{
				yield return new XAttribute (Xml.UserIdentity, this.UserIdentity);
			}
			if (this.MergeSettingsMode != MergeSettingsMode.Inclusive)
			{
				yield return new XAttribute (Xml.MergeSettingsMode, (int) this.MergeSettingsMode);
			}
		}


		private static class Xml
		{
			public const string UserCategory = "cat";
			public const string UserIdentity = "uid";
			public const string MergeSettingsMode = "m";
			public const string FieldSettings = "f";
		}

		private readonly Druid fieldId;
	}
}
