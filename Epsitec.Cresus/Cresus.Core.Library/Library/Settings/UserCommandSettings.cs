//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserCommandSettings
	{
		public UserCommandSettings()
		{
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

		public Druid							CommandId
		{
			get;
			set;
		}

		public MergeSettingsMode			MergeSettingsMode
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				this.GetXmlAttributes ());
		}

		public static UserCommandSettings Restore(XElement xml)
		{
			var userCategory = (int?)   xml.Attribute (Xml.UserCategory);
			var userIdentity = (string) xml.Attribute (Xml.UserIdentity);
			var settingsMode = (int?)   xml.Attribute (Xml.MergeSettingsMode);
			var field        = TileFieldEditionSettings.Restore (xml.Element (Xml.FieldSettings));

			return new UserCommandSettings ()
			{
				UserCategory = (TileUserCategory) userCategory.GetValueOrDefault (),
				UserIdentity = userIdentity,
				MergeSettingsMode = (MergeSettingsMode) settingsMode.GetValueOrDefault ()
			};
		}


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
	}
}
