//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class EntityEditionSettingsEntity
	{
		/// <summary>
		/// Gets the display settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserEntityEditionSettings DisplaySettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();
				
				return this.settings;
			}
		}

		
		public void PersistSettings(IBusinessContext context)
		{
			if (this.settings != null)
			{
				if (this.SerializedSettings.IsNull ())
				{
					this.SerializedSettings = context.CreateEntity<XmlBlobEntity> ();
				}

				this.SerializeSettings ();
			}
		}

		
		private void SerializeSettings()
		{
			this.SerializedSettings.XmlData = this.settings.Save ("settings");
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.settings == null)
			{
				var xml = this.SerializedSettings.IsNull () ? null : this.SerializedSettings.XmlData;

				if (xml == null)
				{
					this.settings = new UserEntityEditionSettings ();
				}
				else
				{
					this.settings = UserEntityEditionSettings.Restore (xml);
				}
			}
		}


		private UserEntityEditionSettings settings;
	}
}
