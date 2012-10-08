//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class SoftwareUISettingsEntity
	{
		/// <summary>
		/// Gets the command set settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserCommandSetSettings UserCommandSetSettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();

				return this.commandSetSettings;
			}
		}

		private void SerializeSettings()
		{
			var xml = new XElement (Xml.Settings,
				this.commandSetSettings.Save (Xml.UserCommands));

			this.SerializedCommandSetSettings.XmlData = xml;
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.commandSetSettings == null)
			{
				var xml = this.SerializedCommandSetSettings.XmlData;

				if (xml == null)
				{
					this.commandSetSettings = new UserCommandSetSettings ();
				}
				else
				{
					this.commandSetSettings = UserCommandSetSettings.Restore (xml.Element (Xml.UserCommands));
				}
			}
		}


		private static class Xml
		{
			public const string Settings = "settings";
			public const string UserCommands = "cmds";
		}

		private UserCommandSetSettings commandSetSettings;
	}
}
