//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public sealed class TileUserFieldDisplaySettings
	{
		public TileUserFieldDisplaySettings()
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

		public TileFieldDisplaySettings			FieldSettings
		{
			get;
			set;
		}

		public TileFieldSettingsMode			FieldSettingsMode
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				this.GetXmlAttributes (),
				this.FieldSettings.Save (Xml.FieldSettings));
		}

		public static TileUserFieldDisplaySettings Restore(XElement xml)
		{
			var userCategory = (int?)   xml.Attribute (Xml.UserCategory);
			var userIdentity = (string) xml.Attribute (Xml.UserIdentity);
			var settingsMode = (int?)   xml.Attribute (Xml.FieldSettingsMode);
			var field        = TileFieldDisplaySettings.Restore (xml.Element (Xml.FieldSettings));

			return new TileUserFieldDisplaySettings ()
			{
				UserCategory = (TileUserCategory) userCategory.GetValueOrDefault (),
				UserIdentity = userIdentity,
				FieldSettingsMode = (TileFieldSettingsMode) settingsMode.GetValueOrDefault (),
				FieldSettings = field
			};
		}


		public static TileFieldDisplaySettings Combine(TileFieldDisplaySettings a, TileFieldDisplaySettings b, TileFieldSettingsMode mode)
		{
			switch (mode)
			{
				case TileFieldSettingsMode.Inclusive:	return a + b;
				case TileFieldSettingsMode.Exclusive:	return a - b;
				case TileFieldSettingsMode.Override:	return b;

				default:
					throw new System.NotSupportedException (string.Format ("Mode {0} not supported", mode));
			}
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
			if (this.FieldSettingsMode != TileFieldSettingsMode.Inclusive)
			{
				yield return new XAttribute (Xml.FieldSettingsMode, (int) this.FieldSettingsMode);
			}
		}


		private static class Xml
		{
			public const string UserCategory = "cat";
			public const string UserIdentity = "uid";
			public const string FieldSettingsMode = "m";
			public const string FieldSettings = "f";
		}
	}
}
