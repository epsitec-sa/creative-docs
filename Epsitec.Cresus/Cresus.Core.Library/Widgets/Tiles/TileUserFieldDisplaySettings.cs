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

		public TileFieldDisplaySettings			Field
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
				new XAttribute (Xml.UserCategory, (int) this.UserCategory),
				new XAttribute (Xml.UserIdentity, this.UserIdentity),
				new XAttribute (Xml.FieldSettingsMode, (int) this.FieldSettingsMode),
				this.Field.Save (Xml.Field));
		}

		public static TileUserFieldDisplaySettings Restore(XElement xml)
		{
			var userCategory = (int) xml.Attribute (Xml.UserCategory);
			var userIdentity = (string) xml.Attribute (Xml.UserIdentity);
			var settingsMode = (int) xml.Attribute (Xml.FieldSettingsMode);
			var field        = TileFieldDisplaySettings.Restore (xml.Element (Xml.Field));

			return new TileUserFieldDisplaySettings ()
			{
				UserCategory = (TileUserCategory) userCategory,
				UserIdentity = userIdentity,
				FieldSettingsMode = (TileFieldSettingsMode) settingsMode,
				Field = field
			};
		}


		private static class Xml
		{
			public const string UserCategory = "cat";
			public const string UserIdentity = "uid";
			public const string FieldSettingsMode = "m";
			public const string Field = "f";
		}
	}
}
