//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Entities
{
	partial class EntityEditionSettingsEntity
	{
		/// <summary>
		/// Gets the display settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public TileEntityDisplaySettings DisplaySettings
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
				this.settings = TileEntityDisplaySettings.Restore (this.SerializedSettings.XmlData);
			}
		}


		private TileEntityDisplaySettings settings;
	}
}
