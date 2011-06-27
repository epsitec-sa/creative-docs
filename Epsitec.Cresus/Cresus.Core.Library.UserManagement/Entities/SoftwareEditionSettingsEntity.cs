//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class SoftwareEditionSettingsEntity
	{
		/// <summary>
		/// Gets the display settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserCommandSettings UserCommandSettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();

				return this.settings;
			}
		}

		private void SerializeSettings()
		{
			var xml = new XElement (Xml.Settings,
				this.settings.Save (Xml.UserCommands));

			this.SerializedCommandSettings.XmlData = xml;
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.settings == null)
			{
				this.settings = UserCommandSettings.Restore (this.SerializedCommandSettings.XmlData);
			}
		}


		private static class Xml
		{
			public const string Settings = "settings";
			public const string UserCommands = "cmds";
		}

		private UserCommandSettings settings;
	}
}
